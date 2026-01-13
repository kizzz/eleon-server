namespace Common.Module.Keys
{
  public class RawCompoundKey : CompoundKey
  {
    private readonly string[] parts;

    public RawCompoundKey(params string[] parts)
    {
      this.parts = parts;
    }

    public string this[int index] => parts[index];

    public static RawCompoundKey Parse(string rawCompoundKey)
    {
      string[] parts = rawCompoundKey.Split(KeyDelimeter);
      return new RawCompoundKey(parts);
    }

    public override string Key => CombineKeyParts(parts);
  }
}
