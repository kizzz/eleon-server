namespace Common.Module.ValueObjects
{
  public class TreeNodeValueObject<T>
  {
    public T Value { get; set; }
    public List<TreeNodeValueObject<T>> Children { get; set; }
  }
}
