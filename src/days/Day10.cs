using Nixill.Utils;

public class Day10
{
  Dictionary<(int X, int Y), D10Asteroid> Asteroids = new();
  int Width = 0;
  int Height = 0;

  public Day10(StreamReader input)
  {
    string[] lines = input.GetAllLines();

    foreach ((string line, int y) in lines.WithIndex())
    {
      foreach ((char c, int x) in line.WithIndex())
      {
        if (c == '#') Asteroids[(x, y)] = new()
        {
          X = x,
          Y = y
        };
      }
    }

    Width = lines[0].Length;
    Height = lines.Length;

    foreach (IEnumerable<D10Asteroid> asteroidPair in Asteroids.Values.Permutations(2))
    {
      D10Asteroid source = asteroidPair.First();
      D10Asteroid target = asteroidPair.Last();

      (int ΔX, int ΔY) vec = GetDisplacement(source, target);
      vec = GetCoprimeVector(vec);
      var los = source.LinesOfSight;

      if (los.ContainsKey(vec))
      {
        D10Asteroid other = los[vec];
        if (GetDistance(source, target) < GetDistance(source, other))
          los[vec] = target;
      }
      else
      {
        los[vec] = target;
      }
    }
  }

  public static (int X, int Y) GetDisplacement(D10Asteroid from, D10Asteroid to)
  {
    return (to.X - from.X, to.Y - from.Y);
  }

  public static int GetDistance(D10Asteroid from, D10Asteroid to)
  {
    return Math.Abs(to.X - from.X) + Math.Abs(to.Y - from.Y);
  }

  public static (int X, int Y) GetCoprimeVector((int x, int y) vector) => GetCoprimeVector(vector.x, vector.y);

  public static (int X, int Y) GetCoprimeVector(int x, int y)
  {
    int GCD = NumberUtils.GCD(x, y);
    return (x / GCD, y / GCD);
  }

  // ------------------------ Boilerplate code ------------------------ //
  static Dictionary<string, Day10> results = new();

  static Day10 Get(string fname, StreamReader input)
  {
    if (!results.ContainsKey(fname))
      results[fname] = new(input);
    return results[fname];
  }

  public static string Part1(string fname, StreamReader input)
    => Get(fname, input)
      .Asteroids
      .Select(e => e.Value.LinesOfSight.Count)
      .OrderDescending()
      .First()
      .ToString();
}

public class D10Asteroid
{
  public int X { get; internal set; }
  public int Y { get; internal set; }

  internal Dictionary<(int X, int Y), D10Asteroid> LinesOfSight = new();
}