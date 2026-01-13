using Logging.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using VPortal.SitesManagement.Module.Consts;
using VPortal.SitesManagement.Module.Entities;
using VPortal.SitesManagement.Module.Repositories;

namespace VPortal.SitesManagement.Module.DomainServices
{
  public class ApplicationMenuItemDomainService : DomainService
  {
    private readonly IVportalLogger<ApplicationMenuItemDomainService> logger;
    private readonly IApplicationMenuItemRepository applicationMenuItemRepository;
    public ApplicationMenuItemDomainService(
        IVportalLogger<ApplicationMenuItemDomainService> logger, IApplicationMenuItemRepository applicationMenuItemRepository)
    {
      this.logger = logger;
      this.applicationMenuItemRepository = applicationMenuItemRepository;
    }

    public async Task<List<ApplicationMenuItemEntity>> GetListByApplicationId(Guid applicationId)
    {
      List<ApplicationMenuItemEntity> result = new();
      try
      {
        result = await applicationMenuItemRepository.GetListByApplicationId(applicationId);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<List<ApplicationMenuItemEntity>> GetListByApplicationIdAndMenuType(Guid applicationId, MenuType menuType)
    {
      List<ApplicationMenuItemEntity> result = new();
      try
      {
        result = await applicationMenuItemRepository.GetListByApplicationIdAndMenuType(applicationId, menuType);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<List<ApplicationMenuItemEntity>> Update(Guid applicationId, List<ApplicationMenuItemEntity> itemsToUpdate)
    {
      List<ApplicationMenuItemEntity> result = new();
      try
      {
        var existingItems = await applicationMenuItemRepository.GetListByApplicationId(applicationId);

        List<ApplicationMenuItemEntity> itemsToDelete = existingItems
            .Where(existingItem => !itemsToUpdate.Any(itemToUpdate => itemToUpdate.Id == existingItem.Id)).ToList();
        List<ApplicationMenuItemEntity> updatedItems = new List<ApplicationMenuItemEntity>();
        List<ApplicationMenuItemEntity> createdItems = new List<ApplicationMenuItemEntity>();

        foreach (var itemToUpdate in itemsToUpdate)
        {
          var existingItem = existingItems.FirstOrDefault(x => x.Id == itemToUpdate.Id);
          if (existingItem != null)
          {
            existingItem.Order = itemToUpdate.Order;
            existingItem.Path = itemToUpdate.Path;
            existingItem.Label = itemToUpdate.Label;
            existingItem.ParentName = itemToUpdate.ParentName;
            existingItem.Icon = itemToUpdate.Icon;
            existingItem.RequiredPolicy = itemToUpdate.RequiredPolicy;
            existingItem.MenuType = itemToUpdate.MenuType;
            existingItem.ItemType = itemToUpdate.ItemType;
            existingItem.Display = itemToUpdate.Display;
            updatedItems.Add(existingItem);
          }
          else
          {
            itemToUpdate.ApplicationId = applicationId;
            createdItems.Add(itemToUpdate);
          }
        }

        if (itemsToDelete.Count > 0)
        {
          await applicationMenuItemRepository.DeleteManyAsync(itemsToDelete, true);
        }

        if (updatedItems.Count > 0)
        {
          await applicationMenuItemRepository.UpdateManyAsync(updatedItems, true);
        }

        if (createdItems.Count > 0)
        {
          await applicationMenuItemRepository.InsertManyAsync(createdItems, true);
        }

        result = await applicationMenuItemRepository.GetListByApplicationId(applicationId);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
  }
}


