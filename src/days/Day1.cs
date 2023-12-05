// This template can be used when you don't need to save data between
// parts.

public static class Day1
{
  public static string Part1(string fname, StreamReader input)
  {
    return input
      .GetLines()
      .Select(int.Parse)
      .Select(x => x / 3 - 2)
      .Sum()
      .ToString();
  }

  public static string Part2(string fname, StreamReader input)
  {
    return input
      .GetLines()
      .Select(int.Parse)
      .Select(x => AddFuelRecursively(x))
      .Sum()
      .ToString();
  }

  public static int AddFuelRecursively(int mass)
  {
    int a = mass / 3 - 2;
    int b = 0;
    if (a > 8) b = AddFuelRecursively(a);
    return a + b;
  }
}