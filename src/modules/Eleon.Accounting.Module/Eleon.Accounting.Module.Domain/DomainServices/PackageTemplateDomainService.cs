using Common.EventBus.Module;
using Common.Module.Constants;
using Common.Module.Extensions;
using Logging.Module;
using Messaging.Module.Messages;
using ModuleCollector.Infrastructure.Module.Infrastructure.Module.Domain.Currency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VPortal.Accounting.Module.Entities;
using VPortal.Accounting.Module.Repositories;

namespace VPortal.Accounting.Module.DomainServices
{

  public class PackageTemplateDomainService : DomainService
  {
    private readonly IVportalLogger<PackageTemplateDomainService> logger;
    private readonly IPackageTemplateRepository packageTemplateRepository;
    private readonly IdentityUserManager userManager;
    private readonly CurrentUser currentUser;
    private readonly CurrentTenant currentTenant;
    private readonly IDistributedEventBus requestClient;
    private readonly CurrencyDomainService currencyDomainService;

    public PackageTemplateDomainService(IVportalLogger<PackageTemplateDomainService> logger,
        IPackageTemplateRepository packageTemplateRepository,
        IdentityUserManager userManager,
        CurrentUser currentUser,
        CurrentTenant currentTenant,
        IDistributedEventBus requestClient,
        CurrencyDomainService currencyDomainService)
    {
      this.logger = logger;
      this.packageTemplateRepository = packageTemplateRepository;
      this.userManager = userManager;
      this.currentUser = currentUser;
      this.currentTenant = currentTenant;
      this.requestClient = requestClient;
      this.currencyDomainService = currencyDomainService;
    }

    public async Task<PackageTemplateEntity> GetPackageTemplateById(Guid id)
    {
      PackageTemplateEntity result = null;
      try
      {
        result = await packageTemplateRepository.GetAsync(id, true);
        var systemCurrency = await currencyDomainService.GetSystemCurrencyAsync();
        result.SystemCurrency = systemCurrency.Code;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task<KeyValuePair<long, List<PackageTemplateEntity>>> GetPackageTemplatesList(
      string sorting = null,
      int maxResultCount = int.MaxValue,
      int skipCount = 0,
      string searchQuery = null,
      DateTime? dateFilterStart = null,
      DateTime? dateFilterEnd = null,
      IList<BillingPeriodType> billingPeriodTypeFilter = null)
    {
      KeyValuePair<long, List<PackageTemplateEntity>> result = default;
      try
      {
        using (currentTenant.Change(null))
        {
          result = await packageTemplateRepository.GetListAsync(
                  sorting, maxResultCount, skipCount,
                  searchQuery,
                  dateFilterStart,
                  dateFilterEnd);
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<string> RemovePackageTemplate(Guid id)
    {
      string result = string.Empty;
      try
      {
        var existing = await packageTemplateRepository.FindAsync(id);
        if (existing == null)
        {
          throw new Exception(string.Format("Travel Service with id: {0} not found", id));
        }

        await packageTemplateRepository.DeleteAsync(id, true);
      }
      catch (Exception e)
      {
        result = e.Message;
        logger.CaptureAndSuppress(e);
      }

      return result;
    }

    public async Task<PackageTemplateEntity> CreatePackageTemplate(PackageTemplateEntity entity)
    {
      logger.LogStart();
      PackageTemplateEntity result = null;
      try
      {
        PackageTemplateEntity newPackageTemplate = new PackageTemplateEntity(Guid.NewGuid());
        newPackageTemplate.PackageName = entity.PackageName;
        newPackageTemplate.PackageTemplateModules = entity.PackageTemplateModules;
        newPackageTemplate.BillingPeriodType = entity.BillingPeriodType;
        newPackageTemplate.Price = entity.Price;
        newPackageTemplate.Description = entity.Description;
        newPackageTemplate.PackageType = entity.PackageType;
        newPackageTemplate.MaxMembers = entity.MaxMembers;
        newPackageTemplate.PackageTemplateModules = entity.PackageTemplateModules;
        result = await packageTemplateRepository.InsertAsync(newPackageTemplate, true);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
        logger.LogFinish();
      }
      return result;
    }

    public async Task<PackageTemplateEntity> UpdatePackageTemplate(PackageTemplateEntity entity)
    {
      PackageTemplateEntity result = null;
      try
      {
        var existingEntity = await packageTemplateRepository.GetAsync(entity.Id, true);

        existingEntity.PackageName = entity.PackageName;
        existingEntity.BillingPeriodType = entity.BillingPeriodType;
        existingEntity.Price = entity.Price;
        existingEntity.Description = entity.Description;
        existingEntity.PackageType = entity.PackageType;
        existingEntity.MaxMembers = entity.MaxMembers;

        existingEntity.PackageTemplateModules.Difference(entity.PackageTemplateModules, x => x.Id).Apply((original, changed) =>
        {
          original.Name = changed.Name;
        });

        result = await packageTemplateRepository.UpdateAsync(existingEntity, true);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }
  }
}
