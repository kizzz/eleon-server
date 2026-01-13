using System.Runtime.CompilerServices;

namespace Eleon.Logging.Lib.VportalLogger
{
  [Obsolete("Use Arg.Of(...) and log {@Args} instead.")]
  public struct ArgumentInfo
  {
    public object ArgumentValue { get; init; }
    public string ArgumentName { get; init; }

    public ArgumentInfo(object argumentValue, string argumentName)
    {
      ArgumentValue = argumentValue;
      ArgumentName = argumentName;
    }

    public override string ToString() => $"{ArgumentName}:{ArgumentValue}";
  }

  [Obsolete("Use Arg.Of(...) and log {@Args} instead.")]
  public struct ArgumentsCollection
  {
    private readonly ArgumentInfo[] args;

    public ArgumentsCollection(ArgumentInfo[] args)
    {
      this.args = args;
    }

    public override string ToString() => string.Join(';', args);
  }

  [Obsolete("Use Arg.Of(...) and log {@Args} instead.")]
  public class Arguments
  {
    public static ArgumentsCollection Args(params ArgumentInfo[] args)
        => new(args);
  }

  public static class Arg
  {
    public static (string Name, object? Value) Of<T>(T value, [CallerArgumentExpression("value")] string name = "")
      => (name, value);
  }
}
