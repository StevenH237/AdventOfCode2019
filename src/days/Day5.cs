public static class Day5
{
  public static bool SkipTests = true;

  public static string Part1(string fname, StreamReader input)
  {
    ICProgram program = new(input.ReadToEnd());
    return program.EvalAll().ToString();
  }
}