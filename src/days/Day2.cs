public class Day2
{
  public static string Part1(string fname, StreamReader input)
  {
    ICProgram program = new(input.ReadToEnd());
    if (fname == "input.txt")
    {
      program[1] = 12;
      program[2] = 2;
    }
    return program.EvalAll().ToString();
  }

  public static string Part2(string fname, StreamReader input)
  {
    ICProgram program = new(input.ReadToEnd());
    if (fname != "input.txt")
    {
      return program.EvalAll().ToString();
    }

    int noun = 0;
    int verb = 0;
    foreach (int n in Enumerable.Range(0, 100))
    {
      noun = n;
      foreach (int v in Enumerable.Range(0, 100))
      {
        verb = v;

        program.Reset();
        program[1] = noun;
        program[2] = verb;

        long result = program.EvalAll();

        if (result == 19690720) return (100 * noun + verb).ToString();
      }
    }

    return "Something went wrong! I didn't find the nounverb combo!";
  }
}