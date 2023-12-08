using Nixill.Utils;

public static class EnumerableExtensions
{
  public static IEnumerable<IEnumerable<T>> Permutations<T>(this IEnumerable<T> elems)
  {
    List<T> elemList = elems.ToList();
    int count = elemList.Count;

    // Handles both the final level of recursion *and* empty lists
    if (count < 2)
    {
      yield return elemList;
      yield break;
    }

    foreach (int i in Enumerable.Range(0, count))
    {
      T elem = elemList[0];
      elemList.RemoveAt(0);

      foreach (IEnumerable<T> sublist in elemList.Permutations())
      {
        yield return EnumerableUtils.Of(elem).Union(sublist);
      }

      elemList.Add(elem);
    }
  }
}