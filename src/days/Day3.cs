using System.Text.RegularExpressions;

public static class Day3
{
  static Dictionary<string, Day3Result> results = new();

  public static string Part1(string fname, StreamReader input)
  {
    if (!results.ContainsKey(fname))
    {
      results[fname] = new Day3Result(input);
    }

    return results[fname].Part1Result;
  }

  public static string Part2(string fname, StreamReader input)
  {
    if (!results.ContainsKey(fname))
    {
      results[fname] = new Day3Result(input);
    }

    return results[fname].Part2Result;
  }
}

public class Day3Result
{
  List<(int X, int Y)> Path1 = new();
  List<(int X, int Y)> Path2 = new();

  public readonly string Part1Result;
  public readonly string Part2Result;

  public Day3Result(StreamReader input)
  {
    // Input is two lines for sure
    string[] lines = input.GetAllLines();

    Map(lines[0], Path1);
    Map(lines[1], Path2);

    Part1Result = Path1
      .Intersect(Path2)
      .Select(c => Math.Abs(c.X) + Math.Abs(c.Y))
      .Order()
      .First()
      .ToString();
    Part2Result = "";
  }

  static Dictionary<char, (int dX, int dY)> Moves = new()
  {
    ['U'] = (0, -1),
    ['R'] = (1, 0),
    ['D'] = (0, 1),
    ['L'] = (-1, 0)
  };

  void Map(string line, List<(int, int)> path)
  {
    int x = 0;
    int y = 0;

    foreach (string move in line.Split(","))
    {
      (int ΔX, int ΔY) = Moves[move[0]];
      foreach (int i in Enumerable.Range(0, int.Parse(move[1..^0])))
      {
        x += ΔX;
        y += ΔY;

        path.Add((x, y));
      }
    }
  }
}
