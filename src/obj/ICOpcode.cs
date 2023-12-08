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

  public static ICOpcode Get(int id)
  {
    id %= 100;

    if (Codes.ContainsKey(id)) return Codes[id];
    else throw new InvalidDataException($"Opcode {id} doesn't exist!");
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
      int left = call.GetParameter();
      int right = call.GetParameter();
      int storeAt = call.GetAddress();
      call.Debug($"{left} + {right} = {left + right} (→ {storeAt})");
      call.Program[storeAt] = left + right;
    }),
    [2] = new ICOpcode(2, "MUL", (call) =>
    {
      int left = call.GetParameter();
      int right = call.GetParameter();
      int storeAt = call.GetAddress();
      call.Debug($"{left} × {right} = {left * right} (→ {storeAt})");
      call.Program[storeAt] = left * right;
    }),
    [3] = new ICOpcode(3, "IN", (call) =>
    {
      int input = call.Program.GetInput(call.Position);
      int address = call.GetAddress();
      call.Program[address] = input;
      call.Debug($"Value {input} written to position {address}");
    }),
    [4] = new ICOpcode(4, "OUT", (call) =>
    {
      int value = call.GetParameter();
      call.Debug($"{value}");
      call.Program.Outputs.Add((call.Position, value));
    }),
    [5] = new ICOpcode(5, "JUMP-TRUE", (call) =>
    {
      int test = call.GetParameter();
      int target = call.GetParameter();

      call.Debug($"Test value: {test}");

      if (test != 0)
      {
        call.Debug($"Jumping to {target}");
        call.Program.Pointer = target;
      }
      else
      {
        call.Debug($"Not jumping.");
      }
    }),
    [6] = new ICOpcode(6, "JUMP-FALSE", (call) =>
    {
      int test = call.GetParameter();
      int target = call.GetParameter();

      call.Debug($"Test value: {test}");

      if (test == 0)
      {
        call.Debug($"Jumping to {target}");
        call.Program.Pointer = target;
      }
      else
      {
        call.Debug($"Not jumping.");
      }
    }),
    [7] = new ICOpcode(7, "LT", (call) =>
    {
      int left = call.GetParameter();
      int right = call.GetParameter();
      int target = call.GetAddress();

      call.Debug($"{left} < {right} → {left < right} (→ {target})");
      call.Program[target] = (left < right) ? 1 : 0;
    }),
    [8] = new ICOpcode(8, "EQ", (call) =>
    {
      int left = call.GetParameter();
      int right = call.GetParameter();
      int target = call.GetAddress();

      call.Debug($"{left} == {right} → {left == right} (→ {target})");
      call.Program[target] = (left == right) ? 1 : 0;
    })
  };
}

public class ICOpcodeCall
{
  public readonly ICProgram Program;
  public readonly ICOpcode Code;
  public readonly int CalledAs;
  public readonly int Position;

  int ParameterModes;

  public ICOpcodeCall(ICProgram program, ICOpcode code, int calledAs, int position)
  {
    Program = program;
    Code = code;
    CalledAs = calledAs;
    Position = position;

    ParameterModes = calledAs / 100;
  }

  int GetParameterMode()
  {
    int mode = ParameterModes % 10;
    ParameterModes /= 10;
    return mode;
  }

  public int GetParameter()
  {
    int mode = GetParameterMode();

    int nextValue = Program.GetNext();

    switch (mode)
    {
      case 0:
        nextValue = Program[nextValue];
        break;
      case 1:
        break;
      default:
        throw new InvalidOperationException($"Unknown parameter mode {mode}.");
    }

    return nextValue;
  }

  public int GetAddress()
  {
    int mode = GetParameterMode();

    if (mode != 0)
      throw new InvalidOperationException($"Attempted to get an address in non-address parameter mode.");

    return Program.GetNext();
  }

  public void Debug(string message)
  {
    if (!Program.Debug) return;

    Console.WriteLine($"{Code.Name} at position {Position}: {message}");
  }
}
