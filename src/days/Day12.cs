using System.Text.RegularExpressions;
using Nixill.Utils;

public class Day12
{
  const bool DEBUG_MODE = true;

  D12System System;
  string Filename;

  static readonly Regex MoonRegex = new(@"^<x=(-?\d+), y=(-?\d+), z=(-?\d+)>$");

  public Day12(string fname, StreamReader input)
  {
    Filename = fname;
    List<D12Moon> moons = new();

    foreach (string line in input.GetLines())
    {
      if (MoonRegex.TryMatch(line, out Match mtc))
      {
        D12Moon mun = new(int.Parse(mtc.Groups[1].Value),
          int.Parse(mtc.Groups[2].Value),
          int.Parse(mtc.Groups[3].Value));
        moons.Add(mun);
      }
    }

    System = new(moons);
  }

  //-------------//
  internal static StreamWriter ActiveFile;

  static Dictionary<string, Day12> results = new();

  static Day12 Get(string fname, StreamReader input)
  {
    if (!results.ContainsKey(fname))
      results[fname] = new(fname, input);
    return results[fname];
  }

  public static string Part1(string fname, StreamReader input)
  {
    if (DEBUG_MODE) ActiveFile = new(File.OpenWrite("data/day12/out-" + fname));

    Day12 data = Get(fname, input);
    int stepCount = fname switch
    {
      "example1.txt" => 10,
      "example2.txt" => 100,
      _ => 1000
    };
    data.System.Step(stepCount);
    return data.System.TotalEnergy.ToString();
  }

  public static void Debug(string line)
  {
    if (DEBUG_MODE) ActiveFile.WriteLine(line);
  }

  public static string Vec3(int x, int y, int z) => $"<x={x}, y={y}, z={z}>";
  public static string PosVec3(D12Moon m) => Vec3(m.X, m.Y, m.Z);
  public static string VelVec3(D12Moon m) => Vec3(m.ΔX, m.ΔY, m.ΔZ);
}

public class D12System
{
  List<D12Moon> Moons = new();
  List<D12Moon> OriginalMoons = new();
  public int Age { get; private set; } = 0;
  public int TotalEnergy => Moons.Sum(x => x.TotalEnergy);

  public D12System(IEnumerable<D12Moon> moons)
  {
    OriginalMoons = new(moons);
    Moons = new(OriginalMoons.Select(m => m.Clone()));
  }

  public D12System(IEnumerable<(int x, int y, int z)> moons)
  {
    OriginalMoons = new(moons.Select(m => new D12Moon(m.x, m.y, m.z)));
    Moons = new(OriginalMoons.Select(m => m.Clone()));
  }

  public void Step() => Step(1);

  public void Step(int count)
  {
    foreach (int i in Enumerable.Range(0, count))
    {
      Age += 1;
      Day12.Debug($"Step #{Age}:");
      Day12.Debug($"  Acceleration phase:");
      foreach (IEnumerable<D12Moon> moons in Moons.Combinations(2))
      {
        D12Moon a = moons.First();
        D12Moon b = moons.Last();

        a.Pull(b);
        Day12.Debug($"    Moon {a.ID} pulls moon {b.ID} changing velocity to {Day12.VelVec3(b)}");
        b.Pull(a);
        Day12.Debug($"    Moon {b.ID} pulls moon {a.ID} changing velocity to {Day12.VelVec3(a)}");
      }

      Day12.Debug("  Movement phase:");
      foreach (D12Moon mun in Moons)
      {
        Day12.Debug($"    Moon {mun.ID} moves with velocity {Day12.VelVec3(mun)} to position {Day12.PosVec3(mun)}");
        mun.Move();
      }
    }
  }
}

public class D12Moon(int x, int y, int z, int id = 0)
{
  public static int NextMoonID = 1;

  public int X { get; private set; } = x;
  public int Y { get; private set; } = y;
  public int Z { get; private set; } = z;

  public int ΔX { get; private set; } = 0;
  public int ΔY { get; private set; } = 0;
  public int ΔZ { get; private set; } = 0;

  public readonly int ID = (id == 0) ? NextMoonID++ : id;

  public int PotentialEnergy => Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);
  public int KineticEnergy => Math.Abs(ΔX) + Math.Abs(ΔY) + Math.Abs(ΔZ);
  public int TotalEnergy => PotentialEnergy * KineticEnergy;

  public virtual void Pull(D12Moon other)
  {
    if (other.X > X) other.ΔX -= 1;
    else if (other.X < X) other.ΔX += 1;
    if (other.Y > Y) other.ΔY -= 1;
    else if (other.Y < Y) other.ΔY += 1;
    if (other.Z > Z) other.ΔZ -= 1;
    else if (other.Z < Z) other.ΔZ += 1;
  }

  public virtual void Move()
  {
    X += ΔX;
    Y += ΔY;
    Z += ΔZ;
  }

  public D12Moon Clone() => new D12Moon(X, Y, Z, ID);
}