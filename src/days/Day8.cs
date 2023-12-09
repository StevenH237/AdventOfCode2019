using Nixill.Utils;

public static class Day8
{
  public static bool SkipTests = true;

  public static string Part1(string fname, StreamReader input)
  {
    string image = input.ReadToEnd();
    var layers = image.Chunk(25 * 6);

    int fewestZeros = int.MaxValue;
    int answer = 0;

    foreach (char[] layer in layers)
    {
      int numberOfZeroes = layer.Where(c => c == '0').Count();
      int numberOfOnes = layer.Where(c => c == '1').Count();
      int numberOfTwos = layer.Where(c => c == '2').Count();

      if (numberOfZeroes < fewestZeros)
      {
        fewestZeros = numberOfZeroes;
        answer = numberOfOnes * numberOfTwos;
      }
    }

    return answer.ToString();
  }

  public static string Part2(string fname, StreamReader input)
  {
    string image = input.ReadToEnd();
    var layers = image.Chunk(25 * 6);

    char[] pixels = Enumerable.Repeat('2', 25 * 6).ToArray();

    foreach (char[] layer in layers)
    {
      for (int i = 0; i < 25 * 6; i++)
      {
        if (pixels[i] == '2')
          pixels[i] = layer[i];
      }
    }

    return "\n" + string.Join("\n", pixels.Chunk(25).Select(x => new string(x))).Replace("0", " ").Replace("1", "█");
  }
}
