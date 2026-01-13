using Logging.Module;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;
using VPortal.Accounting.Module.Entities;
using VPortal.Accounting.Module.Repositories;

namespace VPortal.Accounting.Module.DomainServices
{

  public class BillingInformationDomainService : DomainService
  {
    private readonly IVportalLogger<BillingInformationDomainService> logger;
    private readonly IBillingInformationRepository billingInformationRepository;
    private readonly CurrentTenant currentTenant;

    public BillingInformationDomainService(IVportalLogger<BillingInformationDomainService> logger,
        IBillingInformationRepository billingInformationRepository,
        CurrentTenant currentTenant)
    {
      this.logger = logger;
      this.billingInformationRepository = billingInformationRepository;
      this.currentTenant = currentTenant;
    }

    public async Task<BillingInformationEntity> GetBillingInfoDetailsById(Guid id)
    {
      BillingInformationEntity result = null;
      try
      {
        using (currentTenant.Change(null))
        {
          result = await billingInformationRepository.FindAsync(id, true);
        }
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

    public async Task<BillingInformationEntity> UpdateBillingInfo(BillingInformationEntity entity)
    {
      BillingInformationEntity result = null;
      try
      {
        using (currentTenant.Change(null))
        {
          var existingEntity = await billingInformationRepository.FindAsync(entity.Id, true);
          if (existingEntity != null)
          {
            existingEntity.BillingAddressLine1 = entity.BillingAddressLine1;
            existingEntity.BillingAddressLine2 = entity.BillingAddressLine2;
            existingEntity.CompanyName = entity.CompanyName;
            existingEntity.CompanyCID = entity.CompanyCID;
            existingEntity.City = entity.City;
            existingEntity.StateOrProvince = entity.StateOrProvince;
            existingEntity.PostalCode = entity.PostalCode;
            existingEntity.Country = entity.Country;
            existingEntity.ContactPersonName = entity.ContactPersonName;
            existingEntity.ContactPersonEmail = entity.ContactPersonEmail;
            existingEntity.ContactPersonTelephone = entity.ContactPersonTelephone;
            existingEntity.PaymentMethod = entity.PaymentMethod;
            existingEntity.TenantId = null;

            result = await billingInformationRepository.UpdateAsync(existingEntity, true);
          }
          else
          {
            entity.TenantId = null;
            result = await billingInformationRepository.InsertAsync(entity, true);
          }
        }
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

    public async Task<BillingInformationEntity> UpdateBillingInfoTenantId(Guid accountId, Guid newAccountTenantId)
    {
      BillingInformationEntity result = null;
      try
      {
        using (currentTenant.Change(null))
        {
          var existingEntity = await billingInformationRepository.GetAsync(accountId);
          existingEntity.TenantId = newAccountTenantId;
          result = await billingInformationRepository.UpdateAsync(existingEntity, true);
        }
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
