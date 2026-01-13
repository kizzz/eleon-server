namespace Common.Module.Keys
{
  public abstract class CompoundKey
  {
    public abstract string Key { get; }

    protected static readonly string KeyDelimeter = ";";

    protected static string CombineKeyParts(params string[] keyParts)
        => string.Join(KeyDelimeter, keyParts);

    public override string ToString()
    {
      return Key;
    }
  }
}
