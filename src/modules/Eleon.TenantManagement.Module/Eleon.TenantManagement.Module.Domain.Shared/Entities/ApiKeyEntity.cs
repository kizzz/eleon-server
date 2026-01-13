using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VPortal.Identity.Module.Entities
{
  public class ApiKeyEntity : FullAuditedAggregateRoot<Guid>, IMultiTenant
  {
    public ApiKeyEntity(Guid id, string refId, string key)
    {
      Id = id;
      RefId = refId;
      Key = key;
    }

    public string Name { get; set; }
    public string RefId { get; set; }
    public string Key { get; set; }
    public bool Invalidated { get; set; }
    public bool AllowAuthorize { get; set; }
    public ApiKeyType Type { get; set; }
    public Guid? TenantId { get; set; }
    public DateTime? ExpiresAt { get; set; }

    public string KeySecret { get; set; }

    public string Data { get; set; }

    public List<Claim> GetClaims(string prefix = "")
    {
      return new List<Claim>
            {
                new Claim(prefix + VPortalExtensionGrantsConsts.ApiKey.ApiKeyId, Id.ToString()),
                new Claim(prefix + VPortalExtensionGrantsConsts.ApiKey.ApiKeyTypeClaim, Type.ToString()),
                new Claim(prefix + VPortalExtensionGrantsConsts.ApiKey.ApiKeyRefIdClaim, RefId),
                new Claim(prefix + VPortalExtensionGrantsConsts.ApiKey.ApiKeyNameClaim, Name)
            };
    }
  }
}
