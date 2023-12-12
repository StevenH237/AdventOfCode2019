using Nixill.Collections;
using Nixill.Utils;

public class Day10
{
  Dictionary<(int X, int Y), D10Asteroid> Asteroids = new();
  D10Asteroid MonitoringStation = null;
  D10Asteroid TwoHundredthVictim = null;

  public Day10(string fname, StreamReader input)
  {
    string[] lines = input.GetAllLines();

    // Find all the asteroids
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

    // Store the asteroids in a list by their angle
    foreach (IEnumerable<D10Asteroid> asteroidPair in Asteroids.Values.Permutations(2))
    {
      D10Asteroid source = asteroidPair.First();
      D10Asteroid target = asteroidPair.Last();

      (int ΔX, int ΔY) vec = GetDisplacement(source, target);
      vec = GetCoprimeVector(vec);
      var los = source.LinesOfSight;

      los[vec].Add(target);
    }

    // Get the monitoring station on the asteroid with the best line of
    // sight
    MonitoringStation = Asteroids
      .OrderByDescending(x => x.Value.LinesOfSight.Count)
      .First().Value;

    // Get the list of lines of sight, sorted by relative angle (luckily,
    // the key of that dictionary is in fact the angle), with each sublist
    // sorted by distance.
    List<List<D10Asteroid>> angleSortedList = MonitoringStation
      .LinesOfSight
      .OrderBy(x => x.Key, D10AngleComparer.Instance)
      .Select(x => x.Value
        .OrderBy(x => GetDistance(MonitoringStation, x))
        .ToList()
      ).ToList();

    int shots = 0;
    D10Asteroid shot = null;

    while (shots < 200 && angleSortedList.Count > 0)
    {
      List<D10Asteroid> nextAngle = angleSortedList.Pop();
      shot = nextAngle.Pop();
      if (nextAngle.Count > 0) angleSortedList.Add(nextAngle);
      shots++;
    }

    TwoHundredthVictim = shot;
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
      results[fname] = new(fname, input);
    return results[fname];
  }

  public static string Part1(string fname, StreamReader input)
    => Get(fname, input)
      .MonitoringStation
      .LinesOfSight
      .Count
      .ToString();

  public static string Part2(string fname, StreamReader input)
  {
    D10Asteroid victim = Get(fname, input).TwoHundredthVictim;

    return (victim.X * 100 + victim.Y).ToString();
  }
}

public class D10Asteroid
{
  public int X { get; internal set; }
  public int Y { get; internal set; }

  internal DictionaryGenerator<(int X, int Y), List<D10Asteroid>> LinesOfSight
    = new(new EmptyConstructorGenerator<(int X, int Y), List<D10Asteroid>>());
}

public class D10AngleComparer : IComparer<(int X, int Y)>
{
  int GetAreaNumber((int X, int Y) angle)
  {
    int area = 0;
    (int x, int y) = angle;

    if (x < 0)
    {
      area += 8;
      (x, y) = (-x, -y);
    }

    if (y >= 0)
    {
      area += 4;
      (x, y) = (y, -x);
    }

    if (x == 0 && y < 0) return area;
    if (x > 0 && y == 0) return area + 4;
    if (-y > x) return area + 1;
    if (-y == x) return area + 2;
    if (-y < x) return area + 3;

    return 0;
  }

  public int Compare((int X, int Y) l, (int X, int Y) r)
  {
    int areaL = GetAreaNumber(l);
    int areaR = GetAreaNumber(r);
    if (areaL != areaR) return areaL - areaR;

    int clx = Math.Abs(l.X);
    int crx = Math.Abs(r.X);

    l = (l.X * crx, l.Y * crx);
    r = (r.X * clx, r.Y * clx);

    if ((l.Y > r.Y) != (areaL > 8)) return 1;
    return -1;
  }

  public static readonly D10AngleComparer Instance = new();

  private D10AngleComparer() { }
}