using Nixill.Utils;

public static class Day7
{
  public static string Part1(string fname, StreamReader input)
  {
    string prog = input.ReadToEnd();

    ICProgram amp = new(prog);

    int highestResult = 0;

    foreach (IEnumerable<int> test in Enumerable.Range(0, 5).Permutations())
    {
      int result = 0;
      foreach (int value in test)
      {
        amp.Reset();
        amp.QueueInput(value);
        amp.QueueInput(result);
        (int _, result) = amp.EvalToNextOutput();
      }

      if (result > highestResult) highestResult = result;
    }

    return highestResult.ToString();
  }
}
