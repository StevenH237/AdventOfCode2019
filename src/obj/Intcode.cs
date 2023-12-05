public class ICProgram
{
  // State of memory, readable but not writable through this[int].
  List<int> Memory;

  // Initial state of memory, not readable except by resetting.
  List<int> OrigMemory;

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
  public ICProgram(IEnumerable<int> input)
  {
    OrigMemory = new(input);
    Memory = new(OrigMemory);
  }

  /// <summary>
  /// Creates a new program with text input.
  /// </summary>
  /// <param name="input">The initial memory (as text).</param>
  public ICProgram(string input)
  {
    OrigMemory = new(input.Split(",").Select(int.Parse));
    Memory = new(OrigMemory);
  }

  /// <summary>
  /// Gets the number at the position indicated by the Pointer, then
  /// increments the Pointer.
  /// </summary>
  /// <returns>The number at that position.</returns>
  public int GetNext() => Memory[Pointer++];

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
    int whichCode = GetNext();
    if (!Codes.ContainsKey(whichCode))
      throw new ArgumentException($"The opcode {whichCode} does not exist.");
    Codes[whichCode](this);
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
  }

  static Dictionary<int, Action<ICProgram>> Codes = new()
  {
    [1] = (prog) =>
    {
      int leftPointer = prog.GetNext();
      int left = prog.Memory[leftPointer];
      int rightPointer = prog.GetNext();
      int right = prog.Memory[rightPointer];
      int resultPointer = prog.GetNext();
      prog.Memory[resultPointer] = left + right;
    },
    [2] = (prog) =>
    {
      int leftPointer = prog.GetNext();
      int left = prog.Memory[leftPointer];
      int rightPointer = prog.GetNext();
      int right = prog.Memory[rightPointer];
      int resultPointer = prog.GetNext();
      prog.Memory[resultPointer] = left * right;
    },
    [99] = (prog) =>
    {
      prog.Halted = true;
    }
  };
}
