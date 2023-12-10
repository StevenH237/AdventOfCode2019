using Nixill.Utils;

public static class Day7
{
  public static string Part1(string fname, StreamReader input)
  {
    string prog = input.ReadToEnd();

    ICProgram amp = new(prog);

    long highestResult = 0;

    foreach (IEnumerable<int> test in Enumerable.Range(0, 5).Permutations())
    {
      long result = 0;
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

  public static string Part2(string fname, StreamReader input)
  {
    string prog = input.ReadToEnd();

    List<ICProgram> amps = EnumerableUtils.Repeat(() => new ICProgram(prog), 5).ToList();
    long highestResult = 0;

    // Do all of the following for every possible order
    foreach (IEnumerable<int> test in Enumerable.Range(5, 5).Permutations())
    {
      long result = 0;
      bool running = true;

      // Reset the amps
      foreach ((int value, int index) in test.Select((x, i) => (x, i)))
      {
        amps[index].Reset();
        amps[index].QueueInput(value);
      }

      while (running)
      {
        foreach (int i in Enumerable.Range(0, 5))
        {
          amps[i].QueueInput(result);
          running = amps[i].EvalToNextInput();
          (int _, result) = amps[i].GetLastOutput();
        }
      }

      if (result > highestResult) highestResult = result;
    }

    return highestResult.ToString();
  }
}
