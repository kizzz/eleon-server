using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPortal.SitesManagement.Module.ApplicationConnectionStrings
{
  public interface IApplicationConnectionStringAppService
  {
    Task<bool> AddConnectionString(CreateConnectionStringRequestDto request);
    Task<List<ConnectionStringDto>> GetConnectionStrings(Guid tenantId);
    Task<bool> RemoveConnectionString(RemoveConnectionStringRequestDto request);
    Task<bool> UpdateConnectionString(UpdateConnectionStringRequestDto request);

    Task<ConnectionStringDto?> GetAsync(Guid? tenantId, string applicationName);
    Task SetConnectionStringAsync(SetConnectionStringRequestDto request);
  }

  public class CreateConnectionStringRequestDto
  {
    public Guid TenantId { get; set; }
    public string ApplicationName { get; set; }
    public string ConnectionString { get; set; }
    public string Status { get; set; }
  }

  public class RemoveConnectionStringRequestDto
  {
    public Guid TenantId { get; set; }
    public string ApplicationName { get; set; }
  }

  public class UpdateConnectionStringRequestDto
  {
    public Guid TenantId { get; set; }
    public string ApplicationName { get; set; }
    public string ConnectionString { get; set; }
    public string Status { get; set; }
  }

  public class SetConnectionStringRequestDto
  {
    public Guid TenantId { get; set; }
    public string ApplicationName { get; set; }
    public string ConnectionString { get; set; }
  }
  public class ConnectionStringDto
  {
    public string ApplicationName { get; set; }
    public string Status { get; set; }
    public string ConnectionString { get; set; }
  }
}


