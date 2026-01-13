using Common.Module.Constants;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using VPortal.BackgroundJobs.Module.Entities;

namespace BackgroundJobs.Module.JobFiltering
{
  internal class JobFilter
  {
    private readonly List<string> modulesFilter;
    private readonly List<Guid?> tenantsFilter;
    private readonly List<string> envFilter;

    public JobFilter(IConfiguration configuration)
    {
      if (Convert.ToBoolean(configuration["JobOptions:Whitelist:Enabled"]))
      {
        modulesFilter = configuration
            ?.GetSection("JobOptions:Whitelist:Modules")
            ?.Get<string[]>()
            ?.ToList();
        tenantsFilter = configuration
            ?.GetSection("JobOptions:Whitelist:Tenants")
            ?.Get<string[]>()
            ?.Select<string, Guid?>(tenant => tenant.ToLower() == "host" ? null : Guid.Parse(tenant))
            ?.ToList();
        envFilter = configuration
            ?.GetSection("JobOptions:Whitelist:EnvironmentIds")
            ?.Get<string[]>()
            ?.ToList();
      }
    }

    public IEnumerable<BackgroundJobEntity> Filter(IEnumerable<BackgroundJobEntity> jobs)
        => jobs
            .WhereIf(tenantsFilter != null, job => tenantsFilter.Contains(job.TenantId))
            .WhereIf(envFilter != null, job => envFilter.Contains(job.EnvironmentId));
  }
}
