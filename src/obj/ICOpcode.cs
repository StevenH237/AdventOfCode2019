public class ICOpcode
{
  public readonly int ID;
  public readonly Action<ICOpcodeCall> Method;
  public readonly string Name;

  ICOpcode(int id, string name, Action<ICOpcodeCall> method)
  {
    if (id < 0 || id > 99) throw new ArgumentOutOfRangeException($"Opcodes must be 00 to 99!");

    Method = method;
    ID = id;
    Name = name;
  }

  public static ICOpcode Get(long id)
  {
    int intID = (int)(id % 100);

    if (Codes.ContainsKey(intID)) return Codes[intID];
    else throw new InvalidDataException($"Opcode {intID} doesn't exist!");
  }

  static Dictionary<int, ICOpcode> Codes = new()
  {
    [99] = new ICOpcode(99, "HALT", (call) =>
    {
      call.Debug("");
      call.Program.Halted = true;
    }),
    [1] = new ICOpcode(1, "ADD", (call) =>
    {
      long left = call.GetParameter();
      long right = call.GetParameter();
      int storeAt = call.GetAddress();
      call.Debug($"{left} + {right} = {left + right} (→ {storeAt})");
      call.Program[storeAt] = left + right;
    }),
    [2] = new ICOpcode(2, "MUL", (call) =>
    {
      long left = call.GetParameter();
      long right = call.GetParameter();
      int storeAt = call.GetAddress();
      call.Debug($"{left} × {right} = {left * right} (→ {storeAt})");
      call.Program[storeAt] = left * right;
    }),
    [3] = new ICOpcode(3, "IN", (call) =>
    {
      long input = call.Program.GetInput(call.Position);
      int address = call.GetAddress();
      call.Program[address] = input;
      call.Debug($"Value {input} written to position {address}");
    }),
    [4] = new ICOpcode(4, "OUT", (call) =>
    {
      long value = call.GetParameter();
      call.Debug($"{value}");
      call.Program.Outputs.Add((call.Position, value));
    }),
    [5] = new ICOpcode(5, "JUMP-TRUE", (call) =>
    {
      long test = call.GetParameter();
      long target = call.GetParameter();

      call.Debug($"Test value: {test}");

      if (test != 0)
      {
        call.Debug($"Jumping to {target}");
        call.Program.Pointer = (int)target;
      }
      else
      {
        call.Debug($"Not jumping.");
      }
    }),
    [6] = new ICOpcode(6, "JUMP-FALSE", (call) =>
    {
      long test = call.GetParameter();
      long target = call.GetParameter();

      call.Debug($"Test value: {test}");

      if (test == 0)
      {
        call.Debug($"Jumping to {target}");
        call.Program.Pointer = (int)target;
      }
      else
      {
        call.Debug($"Not jumping.");
      }
    }),
    [7] = new ICOpcode(7, "LT", (call) =>
    {
      long left = call.GetParameter();
      long right = call.GetParameter();
      int target = call.GetAddress();

      call.Debug($"{left} < {right} → {left < right} (→ {target})");
      call.Program[target] = (left < right) ? 1 : 0;
    }),
    [8] = new ICOpcode(8, "EQ", (call) =>
    {
      long left = call.GetParameter();
      long right = call.GetParameter();
      int target = call.GetAddress();

      call.Debug($"{left} == {right} → {left == right} (→ {target})");
      call.Program[target] = (left == right) ? 1 : 0;
    }),
    [9] = new ICOpcode(9, "RBA", (call) =>
    {
      int changeBy = (int)call.GetParameter();

      call.Debug($"Change by {changeBy}.");
      call.Program.RelativeBase += (int)changeBy;
    })
  };
}

public class ICOpcodeCall
{
  public readonly ICProgram Program;
  public readonly ICOpcode Code;
  public readonly long CalledAs;
  public readonly int Position;

  long ParameterModes;

  public ICOpcodeCall(ICProgram program, ICOpcode code, long calledAs, int position)
  {
    Program = program;
    Code = code;
    CalledAs = calledAs;
    Position = position;

    ParameterModes = calledAs / 100;
  }

  int GetParameterMode()
  {
    int mode = (int)(ParameterModes % 10);
    ParameterModes /= 10;
    return mode;
  }

  public long GetParameter()
  {
    int mode = GetParameterMode();

    long nextValue = Program.GetNext();

    switch (mode)
    {
      case 0:
        nextValue = Program[(int)nextValue];
        break;
      case 1:
        break;
      case 2:
        nextValue = Program[(int)nextValue + Program.RelativeBase];
        break;
      default:
        throw new InvalidOperationException($"Unknown parameter mode {mode}.");
    }

    return nextValue;
  }

  public int GetAddress()
  {
    int mode = GetParameterMode();

    switch (mode)
    {
      case 0:
        return (int)Program.GetNext();
      case 2:
        return (int)Program.GetNext() + Program.RelativeBase;
      default:
        throw new InvalidOperationException($"Attempted to get an address in non-address parameter mode.");
    }
  }

  public void Debug(string message)
  {
    if (!Program.Debug) return;

    Console.WriteLine($"{Code.Name} at position {Position}: {message}");
  }
}
