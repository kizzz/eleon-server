using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;
using Volo.Abp.Uow;
using Volo.Abp.Uow.EntityFrameworkCore;

namespace ModuleCollector.EleoncoreMultiTenancyModule.EleoncoreMultiTenancy.Module.EntityFrameworkCore.Overrides
{
  [Volo.Abp.DependencyInjection.Dependency(ReplaceServices = true)]
  public class CustomDbContextProvider<TDbContext> : UnitOfWorkDbContextProvider<TDbContext>, IDbContextProvider<TDbContext>
  where TDbContext : IEfCoreDbContext
  {
    public CustomDbContextProvider(IUnitOfWorkManager unitOfWorkManager, IConnectionStringResolver connectionStringResolver, ICancellationTokenProvider cancellationTokenProvider, ICurrentTenant currentTenant, IEfCoreDbContextTypeProvider efCoreDbContextTypeProvider) : base(unitOfWorkManager, connectionStringResolver, cancellationTokenProvider, currentTenant, efCoreDbContextTypeProvider)
    {
    }

    protected override async Task<string> ResolveConnectionStringAsync(string connectionStringName)
    {
      // Multi-tenancy unaware contexts should always use the host connection string
      //if (typeof(TDbContext).IsDefined(typeof(IgnoreMultiTenancyAttribute), false))
      //{
      //    using (CurrentTenant.Change(null))
      //    {
      //        return await ConnectionStringResolver.ResolveAsync(connectionStringName);
      //    }
      //}

      return await ConnectionStringResolver.ResolveAsync(connectionStringName);
    }

    [Obsolete("Use ResolveConnectionStringAsync method.")]
    protected override string ResolveConnectionString(string connectionStringName)
    {
      // Multi-tenancy unaware contexts should always use the host connection string
      //if (typeof(TDbContext).IsDefined(typeof(IgnoreMultiTenancyAttribute), false))
      //{
      //    using (CurrentTenant.Change(null))
      //    {
      //        return ConnectionStringResolver.Resolve(connectionStringName);
      //    }
      //}

      return ConnectionStringResolver.Resolve(connectionStringName);
    }
  }
}
