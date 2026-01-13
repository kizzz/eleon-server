
namespace abp_sdk.Middlewares;
public class EleonsoftTenantResolveResultAccessor // scoped
{
  public Guid? TenantId { get; private set; }

  public bool IsSuccessful => ResolveException == null;

  public Exception ResolveException { get; private set; }

  public void SetResult(Guid? tenantId, Exception? exception)
  {
    TenantId = tenantId;
    ResolveException = exception;
  }
}
