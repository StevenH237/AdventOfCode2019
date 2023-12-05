// This template runs the results just once and caches answers. Helpful if
// you can write your code more efficiently.

public static class DayX
{
  static Dictionary<string, DayXResult> results = new();

  public static string Part1(string fname, StreamReader input)
  {
    if (!results.ContainsKey(fname))
    {
      results[fname] = new DayXResult(input);
    }

    return results[fname].Part1Result;
  }

  public static string Part2(string fname, StreamReader input)
  {
    if (!results.ContainsKey(fname))
    {
      results[fname] = new DayXResult(input);
    }

    return results[fname].Part2Result;
  }
}

public class DayXResult
{
  public readonly string Part1Result;
  public readonly string Part2Result;

  public DayXResult(StreamReader input)
  {
    Part1Result = "";
    Part2Result = "";
  }
}
