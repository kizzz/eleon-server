

namespace ProxyRouter.Minimal.HttpApi.ErrorHandling.Exceptions;

public class ProxyException : Exception
{
  public ProxyException() { }
  public ProxyException(string message) : base(message) { }
  public ProxyException(string message, Exception inner) : base(message, inner) { }

  public string TenantId
  {
    get => Data[nameof(TenantId)]?.ToString();
    set => Data[nameof(TenantId)] = value;
  }
  public string Application
  {
    get => Data[nameof(Application)]?.ToString();
    set => Data[nameof(Application)] = value;
  }
  public string RequestedRoute
  {
    get => Data[nameof(RequestedRoute)]?.ToString();
    set => Data[nameof(RequestedRoute)] = value;
  }
  public string RequestedResourceUrl
  {
    get => Data[nameof(RequestedResourceUrl)]?.ToString();
    set => Data[nameof(RequestedResourceUrl)] = value;
  }
}
