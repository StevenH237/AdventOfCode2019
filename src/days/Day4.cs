// This template runs the results just once and caches answers. Helpful if
// you can write your code more efficiently.

using Nixill.Utils;

public static class Day4
{
  static Dictionary<string, Day4Result> results = new();

  public static string Part1(string fname, StreamReader input)
  {
    if (!results.ContainsKey(fname))
    {
      results[fname] = new Day4Result(input);
    }

    return results[fname].Part1Result;
  }

  public static string Part2(string fname, StreamReader input)
  {
    if (!results.ContainsKey(fname))
    {
      results[fname] = new Day4Result(input);
    }

    return results[fname].Part2Result;
  }
}

public class Day4Result
{
  public readonly string Part1Result;
  public readonly string Part2Result;

  List<int> valids1 = new();
  List<int> valids2 = new();

  public Day4Result(StreamReader input)
  {
    int[] bounds = input.GetLines().Select(int.Parse).ToArray();

    foreach (int i in Enumerable.Range(bounds[0], bounds[1] - bounds[0] + 1))
    {
      string iS = i.ToString();
      if (iS.WhereOrdered().Count() < 6) continue;
      if (iS.Distinct().Count() == 6) continue;
      valids1.Add(i);
      if (!iS.GroupBy(c => c).Where(grp => grp.Count() == 2).Any()) continue;
      valids2.Add(i);
    }

    Part1Result = valids1.Count.ToString();
    Part2Result = valids2.Count.ToString();
  }
}
