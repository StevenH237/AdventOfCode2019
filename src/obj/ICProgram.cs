public class ICProgram
{
  // State of memory, readable but not writable through this[int].
  List<int> Memory;

  // Initial state of memory, not readable except by resetting.
  List<int> OrigMemory;

  // List of outputs (with addresses)
  internal List<(int Where, int What)> Outputs = new();

  // List of queued inputs
  List<int> QueuedInputs = new();

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
  ///   Gets the number at the position indicated by the Pointer, then
  ///   increments the Pointer.
  /// </summary>
  /// <returns>The number at that position.</returns>
  public int GetNext() => Memory[Pointer++];

  /// <summary>
  ///   Gets the number at the position indicated by the Pointer WITHOUT
  ///   incrementing the Pointer.
  /// </summary>
  /// <returns>The number at that position.</returns>
  public int GetUpcoming() => Memory[Pointer];

  /// <summary>
  ///   Gets the position of the Pointer and the number at that position,
  ///   then increments the Pointer.
  /// </summary>
  /// <returns>
  ///   A tuple containing the described values, position first.
  /// </returns>
  public (int Where, int What) GetNextWithPos() => (Pointer, Memory[Pointer++]);

  /// <summary>
  ///   Evaluates an opcode at the current Pointer.
  /// </summary>
  /// <return>
  ///   The called opcode.
  /// </return>
  /// <exception cref="InvalidOperationException">
  ///   If the program has Halted.
  /// </exception>
  /// <exception cref="ArgumentException">
  ///   If the Opcode doesn't exist.
  /// </exception>
  public ICOpcode Eval()
  {
    if (Halted)
      throw new InvalidOperationException("The program has halted!");
    (int where, int whichCode) = GetNextWithPos();
    ICOpcode code = ICOpcode.Get(whichCode);
    ICOpcodeCall call = new(this, code, whichCode, where);
    code.Method(call);
    return code;
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
  ///   Evaluates opcodes until an opcode 4 is processed.
  /// </summary>
  /// <returns>The output invoked by that opcode 4.</returns>
  /// <exception cref="InvalidOperationException">The program has already halted before this call.</exception>
  /// <exception cref="InvalidDataException">The program halts without an additional output.</exception>
  public (int Where, int What) EvalToNextOutput()
  {
    if (Halted)
      throw new InvalidOperationException("The program has halted!");

    while (!Halted)
    {
      ICOpcode code = Eval();
      if (code.ID == 4) // Output
      {
        return GetLastOutput();
      }
    }

    throw new InvalidDataException("The program halted without an output!");
  }

  /// <summary>
  ///   Evaluates opcodes until the next to be processed is an input and
  ///   none are in the queue. Pauses before evaluating that input call.
  /// </summary>
  /// <remarks>
  ///   This call will not cause the ICProgram to move forward at all if
  ///   it is called while the ICProgram is already awaiting input and no
  ///   inputs have been queued since. Additionally, while inputs are
  ///   queued, the ICProgram's execution will continue.
  /// </remarks>
  /// <returns>
  ///   True if the program paused awaiting input. False if the program
  ///   halted.
  /// </returns>
  public bool EvalToNextInput()
  {
    if (Halted)
      throw new InvalidOperationException("The program has halted!");

    while (!Halted)
    {
      int nextCode = GetUpcoming();
      ICOpcode code = ICOpcode.Get(nextCode);
      if (code.ID == 3 && QueuedInputs.Count == 0) return true;
      Eval();
    }

    return false;
  }

  /// <summary>
  ///   Returns the most recent output saved to the table.
  /// </summary>
  /// <returns>The most recent output.</returns>
  /// <exception cref="InvalidOperationException">No outputs yet!</exception>
  public (int Where, int What) GetLastOutput()
  {
    if (Outputs.Count == 0)
      throw new InvalidOperationException("No outputs yet!");

    return Outputs[Outputs.Count - 1];
  }

  /// <summary>
  ///   Add a single input to queue.
  /// </summary>
  /// <param name="input">The input.</param>
  public void QueueInput(int input)
  {
    QueuedInputs.Add(input);
  }

  /// <summary>
  ///   Add multiple inputs to queue.
  /// </summary>
  /// <param name="inputs">The inputs.</param>
  public void QueueInputs(IEnumerable<int> inputs)
  {
    QueuedInputs.AddRange(inputs);
  }

  // Returns the next queued input, or gets one from console if nothing queued.
  internal int GetInput(int pos)
  {
    if (QueuedInputs.Count > 0)
    {
      int value = QueuedInputs[0];
      QueuedInputs.RemoveAt(0);
      return value;
    }

    Console.Write($"Integer requested at position {pos}: ");
    string input = "";
    while (input == "")
    {
      input = Console.ReadLine();
      if (int.TryParse(input, out int value))
      {
        return value;
      }
      else
      {
        input = "";
        Console.Write($"That's not an integer, try again: ");
      }
    }

    throw new InvalidDataException("How did you even get this error?");
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
    QueuedInputs = new();
  }

  /// <summary>
  ///   Gets a read-only list of outputs.
  /// </summary>
  public IReadOnlyCollection<(int Where, int What)> GetOutputs()
  {
    return Outputs.AsReadOnly();
  }
}
