using Nixill.Collections;

public static class Day11
{
  public static bool SkipTests = true;

  static DictionaryGenerator<D11XY, bool> PaintedTiles1 = new();
  static DictionaryGenerator<D11XY, bool> PaintedTiles2 = new() { [(0, 0)] = true };

  public static string Paint(ICProgram prog, DictionaryGenerator<D11XY, bool> painting)
  {
    D11XY position = (0, 0);
    D11XY facing = (0, -1);

    while (!prog.Halted)
    {
      // Input the color underneath the robot.
      prog.QueueInput(painting[position] ? 1 : 0);

      // Now get the next output...
      // 1. Paint the panel underneath the robot using the first output.
      // 0 for black (false), 1 for white (true).
      (int _, long val) = prog.EvalToNextOutput();
      painting[position] = (val == 1);

      // Get another output...
      // 2. Turn using the second output.
      // 0 for left, 1 for right.
      (_, val) = prog.EvalToNextOutput();
      facing = val switch
      {
        0 => facing.Left,
        1 => facing.Right,
        _ => facing
      };

      // 3. Move forward.
      position += facing;

      // And loop back to next input
      prog.EvalToNextInput();
    }

    // Finally, output the number of tiles that were ever painted.
    return painting.Count.ToString();
  }

  public static string Part1(string fname, StreamReader input)
  {
    ICProgram prog = new(input.ReadToEnd());
    return Paint(prog, PaintedTiles1);
  }

  // This was written before I submitted part 1! AoC seems to have at
  // least one of these per year so now... wait wasn't day 6 this as well?
  // Oh well, I'm gonna guess it's happening again.

  // I was so close to getting it right. But I need to run the entire
  // program again. :)
  public static string Part2(string fname, StreamReader input)
  {
    ICProgram prog = new(input.ReadToEnd());
    Paint(prog, PaintedTiles2);

    int top = PaintedTiles2.Min(p => p.Key.Y);
    int left = PaintedTiles2.Min(p => p.Key.X);
    int right = PaintedTiles2.Max(p => p.Key.X);
    int bottom = PaintedTiles2.Max(p => p.Key.Y);

    Console.WriteLine();

    for (int y = top; y < bottom + 1; y++)
    {
      for (int x = left; x < right + 1; x++)
      {
        Console.Write(PaintedTiles2[(x, y)] ? '█' : ' ');
      }
      Console.WriteLine();
    }

    return "";
  }
}

public struct D11XY(int x, int y)
{
  public int X = x;
  public int Y = y;

  public static implicit operator D11XY((int x, int y) input) => new(input.x, input.y);
  public static D11XY operator +(D11XY left, D11XY right) => new(left.X + right.X, left.Y + right.Y);

  public D11XY Left => new(Y, -X);
  public D11XY Right => new(-Y, X);

  public override bool Equals(object obj)
  {
    if (obj is D11XY other)
    {
      return X == other.X && Y == other.Y;
    }

    return false;
  }

  public override int GetHashCode() =>
    (X & 0xFFFF << 16) | (Y & 0xFFFF);
}