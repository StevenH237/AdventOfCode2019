public class ICProgram
{
  // State of memory, readable but not writable through this[int].
  List<int> Memory;

  // Initial state of memory, not readable except by resetting.
  List<int> OrigMemory;

  // List of outputs (with addresses)
  List<(int Where, int What)> Outputs = new();

  /// <summary>
  ///   Whether or not the program is in debug mode.
  /// </summary>
  public bool Debug;

  /// <summary>
  /// The current instruction pointer location.
  /// </summary>
  public int Pointer { get; internal set; } = 0;

  /// <summary>
  /// Whether or not the program has halted. If so, Eval() and EvalAll()
  /// will throw an OperationException.
  /// </summary>
  public bool Halted { get; internal set; } = false;

  /// <summary>
  /// Get the value of an index in program memory
  /// </summary>
  /// <param name="index">The index to retrieve</param>
  /// <returns>The program's value</returns>
  public int this[int index]
  {
    get => Memory[index];
    internal set => Memory[index] = value;
  }

  /// <summary>
  /// Creates a new program.
  /// </summary>
  /// <param name="input">The initial memory.</param>
  public ICProgram(IEnumerable<int> input, bool debug = false)
  {
    OrigMemory = new(input);
    Memory = new(OrigMemory);
    Debug = debug;
  }

  /// <summary>
  /// Creates a new program with text input.
  /// </summary>
  /// <param name="input">The initial memory (as text).</param>
  public ICProgram(string input, bool debug = false)
  {
    OrigMemory = new(input.Split(",").Select(int.Parse));
    Memory = new(OrigMemory);
    Debug = debug;
  }

  /// <summary>
  /// Gets the number at the position indicated by the Pointer, then
  /// increments the Pointer.
  /// </summary>
  /// <returns>The number at that position.</returns>
  public int GetNext() => Memory[Pointer++];

  /// <summary>
  /// Gets the position of the Pointer and the number at that position,
  /// then increments the Pointer.
  /// </summary>
  /// <returns>A tuple containing the described values.</returns>
  public (int Where, int What) GetNextWithPos() => (Pointer, Memory[Pointer++]);

  /// <summary>
  ///   Evaluates an opcode at the current Pointer.
  /// </summary>
  /// <exception cref="InvalidOperationException">
  ///   If the program has Halted.
  /// </exception>
  /// <exception cref="ArgumentException">
  ///   If the Opcode doesn't exist.
  /// </exception>
  public void Eval()
  {
    if (Halted)
      throw new InvalidOperationException("The program has halted!");
    (int where, int whichCode) = GetNextWithPos();
    ICOpcode code = ICOpcode.Get(whichCode);
    ICOpcodeCall call = new(this, code, whichCode, where);
    code.Method(call);
  }

  /// <summary>
  ///   Evaluates all opcodes until the program halts.
  /// </summary>
  /// <return>The value at address 0 once the program halts.</return>
  public int EvalAll()
  {
    if (Halted)
      throw new InvalidOperationException("The program has halted!");
    while (!Halted)
    {
      Eval();
    }

    return Memory[0];
  }

  /// <summary>
  ///   Resets the program to its original state.
  /// </summary>
  public void Reset()
  {
    Memory = new(OrigMemory);
    Pointer = 0;
    Halted = false;
    Outputs = new();
  }

  /// <summary>
  ///   Gets a read-only list of outputs.
  /// </summary>
  public IReadOnlyCollection<(int Where, int What)> GetOutputs()
  {
    return Outputs.AsReadOnly();
  }
}
