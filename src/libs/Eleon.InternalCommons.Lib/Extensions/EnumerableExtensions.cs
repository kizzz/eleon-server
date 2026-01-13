using Common.Module.Helpers;
using System.Data;

namespace Common.Module.Extensions
{
  public static class EnumerableExtensions
  {
    public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> seq, int partSize)
    {
      for (int i = 0; i < seq.Count(); i += partSize)
      {
        var part = seq.Skip(i).Take(partSize);
        yield return part;
      }
    }

    /// <summary>
    /// Splits a sequence into two sequences, where first one contains items that match the predicate, and the second one contains items that don't.
    /// </summary>
    /// <typeparam name="T">Type of the sequence item.</typeparam>
    /// <param name="seq">The sequence to divide.</param>
    /// <param name="predicate">A predicate that will be used as a criteria for the division.</param>
    /// <param name="lazy">Set this to true if you want to get lazy sequences as a result. Note, in such case the predicate will run twice for each item of the original sequence.</param>
    /// <returns>Two sequences where first one contains items that match the predicate, and the second one contains items that don't.</returns>
    public static (IEnumerable<T> matched, IEnumerable<T> notMatched) Divide<T>(this IEnumerable<T> seq, Func<T, bool> predicate, bool lazy = false)
    {
      if (lazy)
      {
        return (seq.Where(predicate), seq.Where(x => !predicate(x)));
      }
      else
      {
        var matched = new List<T>();
        var notMatched = new List<T>();
        foreach (var item in seq)
        {
          if (predicate(item))
          {
            matched.Add(item);
          }
          else
          {
            notMatched.Add(item);
          }
        }

        return (matched, notMatched);
      }
    }

    public static IEnumerable<T> ToSingleItemEnumerable<T>(this T item)
    {
      yield return item;
    }

    public static List<T> ToSingleItemList<T>(this T item)
    {
      return new List<T> { item };
    }

    /// <summary>
    /// Selects items from two enumerables pairwise, until reaching the end of the shortest sequence.
    /// </summary>
    /// <typeparam name="T1">Type of the first enumerable.</typeparam>
    /// <typeparam name="T2">Type of the second enumerable.</typeparam>
    /// <typeparam name="TRes">Type of the resulting enumerable</typeparam>
    /// <param name="source">The first enumerable.</param>
    /// <param name="other">The second enumerable.</param>
    /// <param name="selector">A selector that will receive pairs and return resulting items.</param>
    /// <returns>An IEnumerable where each item is the result of applying selector to the corresponding pairs of the source sequences.</returns>
    public static IEnumerable<TRes> SelectPairwise<T1, T2, TRes>(this IEnumerable<T1> source, IEnumerable<T2> other, Func<T1, T2, TRes> selector)
    {
      var enum1 = source.GetEnumerator();
      var enum2 = other.GetEnumerator();
      while (enum1.MoveNext() && enum2.MoveNext())
      {
        yield return selector(enum1.Current, enum2.Current);
      }
    }

    public static DataTable ToDataTable<T>(this IEnumerable<T> rows, string tableName, params (string title, Func<T, string> valueSelector)[] columns)
        => DataTableHelper.CreateDataTable(tableName, rows, columns);

    public static bool IsNullOrEmpty<T>(this IEnumerable<T> seq)
        => seq == null || !seq.Any();
  }
}
