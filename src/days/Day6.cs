// This template runs the results just once and caches answers. Helpful if
// you can write your code more efficiently.

public static class Day6
{
  static Dictionary<string, Day6Result> results = new();

  public static string Part1(string fname, StreamReader input)
  {
    Day6Result res;

    if (results.ContainsKey(fname))
    {
      res = results[fname];
    }
    else
    {
      res = new Day6Result(input);
      results[fname] = res;
    }

    int sum = 0;
    foreach (string orbiter in res.Orbits.Keys)
    {
      for (string test = orbiter; res.Orbits.ContainsKey(test); test = res.Orbits[test])
      {
        sum += 1;
      }
    }

    return sum.ToString();
  }

  public static string Part2(string fname, StreamReader input)
  {
    Day6Result res;

    if (results.ContainsKey(fname))
    {
      res = results[fname];
    }
    else
    {
      res = new Day6Result(input);
      results[fname] = res;
    }

    var yourOrbits = OrbitChain(res, res.Orbits["YOU"]);
    var santaOrbits = OrbitChain(res, res.Orbits["SAN"]);

    return yourOrbits.Except(santaOrbits).Union(santaOrbits.Except(yourOrbits)).Count().ToString();
  }

  static IEnumerable<string> OrbitChain(Day6Result res, string start)
  {
    for (string test = start; res.Orbits.ContainsKey(test); test = res.Orbits[test])
    {
      yield return test;
    }
  }
}

public class Day6Result
{
  public readonly string Part1Result;
  public readonly string Part2Result;

  internal Dictionary<string, string> Orbits;

  public Day6Result(StreamReader input)
  {
    Orbits = new();

    foreach (string line in input.GetLines())
    {
      string[] orbitInfo = line.Split(")");
      string orbitee = orbitInfo[0];
      string orbiter = orbitInfo[1];
      Orbits[orbiter] = orbitee;
    }
  }
}
