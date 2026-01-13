namespace Common.Module.Helpers
{
  public class ChangedItem<TItem>
  {
    public TItem Original { get; init; }
    public TItem Changed { get; init; }

    public ChangedItem(TItem original, TItem changed)
    {
      Original = original;
      Changed = changed;
    }

    public void Deconstruct(out TItem original, out TItem changed)
    {
      original = Original;
      changed = Changed;
    }
  }

  public class EnumerableDifference<TItem>
  {
    public IEnumerable<TItem> Removed { get; init; }
    public IEnumerable<TItem> Added { get; init; }
    public IEnumerable<ChangedItem<TItem>> Updated { get; init; }
    public ICollection<TItem> Original { get; init; }
  }

  public static class DifHelper
  {
    public static EnumerableDifference<TItem> GetDifference<TItem, TKey>(
        ICollection<TItem> original,
        IEnumerable<TItem> changed,
        Func<TItem, TKey> keySelector)
       => new EnumerableDifference<TItem>
       {
         Added = changed.Where(c =>
             {
             var key = keySelector(c);
             return !original.Any(o => EqualityComparer<TKey>.Default.Equals(key, keySelector(o)));
           }).ToList(),
         Removed = original.Where(o =>
             {
             var key = keySelector(o);
             return !changed.Any(c => EqualityComparer<TKey>.Default.Equals(key, keySelector(c)));
           }).ToList(),
         Updated = original.Join(changed, keySelector, keySelector, (o, c) => new ChangedItem<TItem>(o, c)),
         Original = original,
       };
  }
}
