public static class Day9
{
  public static bool SkipTests = true;

  public static string Part1(string fname, StreamReader input)
  {
    ICProgram prog = new(input.ReadToEnd());
    prog.QueueInput(1);
    prog.EvalAll();
    return prog.GetOutputs().Last().What.ToString();
  }

  public static string Part2(string fname, StreamReader input)
  {
    ICProgram prog = new(input.ReadToEnd());
    prog.QueueInput(2);
    prog.EvalAll();
    return prog.GetOutputs().Last().What.ToString();
  }
}