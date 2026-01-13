using Common.Module.Helpers;

namespace Common.Module.Extensions
{
  public static class DifExtensions
  {
    public delegate void UpdateItem<TItem>(TItem original, TItem changed);

    public static EnumerableDifference<TItem> Difference<TItem, TKey>(this ICollection<TItem> original, ICollection<TItem> changed, Func<TItem, TKey> keySelector)
    {
      return DifHelper.GetDifference(original, changed, keySelector);
    }

    public static void ForEach<TItem>(
        this EnumerableDifference<TItem> dif,
        Action<TItem> addedAction = null,
        Action<TItem> removedAction = null,
        Action<ChangedItem<TItem>> updatedAction = null)
    {
      if (addedAction != null)
      {
        foreach (var added in dif.Added)
        {
          addedAction(added);
        }
      }

      if (removedAction != null)
      {
        foreach (var removed in dif.Removed)
        {
          removedAction(removed);
        }
      }

      if (updatedAction != null)
      {
        foreach (var updated in dif.Updated)
        {
          updatedAction(updated);
        }
      }
    }

    public static void Apply<TItem>(this EnumerableDifference<TItem> dif)
    {
      foreach (var toRemove in dif.Removed)
      {
        dif.Original.Remove(toRemove);
      }

      foreach (var toAdd in dif.Added)
      {
        dif.Original.Add(toAdd);
      }
    }

    public static void Apply<TItem>(this EnumerableDifference<TItem> dif, Action<TItem, TItem> update)
    {
      foreach (var updated in dif.Updated)
      {
        update(updated.Original, updated.Changed);
      }

      Apply(dif);
    }
  }
}
