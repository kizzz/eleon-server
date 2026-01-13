using Common.EventBus.Module;
using Eleon.InternalCommons.Lib.Messages.Hostnames;
using Eleon.InternalCommons.Lib.Messages.Identity;
using EleoncoreMultiTenancy.Module.Localization;
using Logging.Module;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Migrations.Module;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Shared.Entities;
using TenantSettings.Module.Cache;
using Volo.Abp;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.TenantManagement;
using Volo.Abp.Uow;
using VPortal.TenantManagement.Module.DomainServices;
using VPortal.TenantManagement.Module.Tenants;


public class TenantDomainService : TenantQueryingDomainService
{
  private readonly IVportalLogger<TenantDomainService> logger;
  private readonly IDbMigrationService dbMigrationService;
  private readonly DatabaseDomainService databaseDomainService;
  private readonly IServiceProvider serviceProvider;
  private readonly ITenantRepository tenantRepository;
  private readonly IUnitOfWorkManager unitOfWorkManager;
  private readonly ITenantManager tenantManager;
  private readonly IStringLocalizer<EleoncoreMultiTenancyResource> localizer;
  private readonly TenantCacheService tenantCacheService;
  private readonly IdentityUserManager identityUserManager;
  private readonly IConfiguration _configuration;
  private readonly IdentityRoleManager _roleManager;
  private readonly IPermissionGrantRepository _permissionGrantRepository;
  private readonly IPermissionDefinitionManager _permissionDefinitionManager;
  private readonly IDistributedEventBus _eventBus;

  public TenantDomainService(
        IVportalLogger<TenantDomainService> logger,
        IDbMigrationService dbMigrationService,
        DatabaseDomainService databaseDomainService,
        IServiceProvider serviceProvider,
        ITenantRepository tenantRepository,
        IUnitOfWorkManager unitOfWorkManager,
        ITenantManager tenantManager,
        IStringLocalizer<EleoncoreMultiTenancyResource> localizer,
        TenantCacheService tenantCacheService,
        IdentityUserManager identityUserManager,
        IConfiguration configuration,
        IdentityRoleManager roleManager,
        IPermissionGrantRepository permissionGrantRepository,
        IPermissionDefinitionManager permissionDefinitionManager,
        IDistributedEventBus eventBus) : base(serviceProvider)
  {
    this.logger = logger;
    this.dbMigrationService = dbMigrationService;
    this.databaseDomainService = databaseDomainService;
    this.serviceProvider = serviceProvider;
    this.tenantRepository = tenantRepository;
    this.unitOfWorkManager = unitOfWorkManager;
    this.tenantManager = tenantManager;
    this.localizer = localizer;
    this.tenantCacheService = tenantCacheService;
    this.identityUserManager = identityUserManager;
    _configuration = configuration;
    _roleManager = roleManager;
    _permissionGrantRepository = permissionGrantRepository;
    _permissionDefinitionManager = permissionDefinitionManager;
    _eventBus = eventBus;
  }

  public async Task ResetAdminPassword(Guid tenantId, string newPassword)
  {
    try
    {
      Tenant tenant = await tenantRepository.GetAsync(tenantId);
      var admins = await identityUserManager.GetUsersInRoleAsync(MigrationConsts.AdminRoleNameDefaultValue);
      var admin = admins.FirstOrDefault(x => x.UserName.Equals(MigrationConsts.AdminRoleNameDefaultValue, StringComparison.InvariantCultureIgnoreCase)) ?? admins.First();
      await identityUserManager.RemovePasswordAsync(admin);
      var result = await identityUserManager.AddPasswordAsync(admin, newPassword);
      if (!result.Succeeded)
      {
        throw new Exception();
      }
    }
    catch (Exception ex)
    {
      logger.Capture(ex);
    }
    finally
    {
    }
  }

  public async Task<TenantCreationResult> CreateTenant(
      string tenantName,
      string adminEmail,
      string adminPassword,
      bool createDatabase,
      string newDatabaseName,
      string newUserName,
      string newUserPassword,
      string defaultConnectionString,
      bool isRoot)
  {
    var result = new TenantCreationResult()
    {
      Success = false,
    };

    try
    {
      var validationError = await ValidateTenantCreation(
          tenantName,
          createDatabase,
          newDatabaseName,
          newUserName);//change to newUserName db

      if (!string.IsNullOrEmpty(validationError))
      {
        result.Error = validationError;
      }
      else
      {
        var newTenant = await CreateTenantWithoutValidation(
            tenantName,
            adminEmail,
            adminPassword,
            createDatabase,
            newDatabaseName,
            newUserName,
            newUserPassword,
            defaultConnectionString,
            isRoot);

        result.Success = true;
        result.TenantId = newTenant.tenantId;
      }
    }
    catch (Exception ex)
    {
      result.Error = "Internal error occured";
      logger.Capture(ex);
    }
    finally
    {
    }

    return result;
  }

  public async Task RemoveTenant(Tenant tenant)
  {
    try
    {
      await tenantRepository.DeleteAsync(tenant, true);
      if (CurrentTenant.Id == null)
      {
        return;
      }

      //using (CurrentTenant.Change(Guid.Empty))
      //{
      //    var dbContextProvider = serviceProvider.GetRequiredService<IDbContextProvider<VPortalDbContext>>();
      //    var currentTenantDbContext = await dbContextProvider.GetDbContextAsync();
      //    currentTenantDbContext.Tenants.Remove(tenant);
      //    await currentTenantDbContext.SaveChangesAsync();
      //}
    }
    catch (Exception ex)
    {
      logger.Capture(ex);
    }
    finally
    {
    }
  }

  public async Task<Tenant> UpdateTenant(Tenant tenant)
  {
    Tenant result = null;
    try
    {
      result = await tenantRepository.UpdateAsync(tenant, true);
      if (CurrentTenant.Id == null)
      {
        return result;
      }

      //using (CurrentTenant.Change(Guid.Empty))
      //{
      //    var dbContextProvider = serviceProvider.GetRequiredService<IDbContextProvider<VPortalDbContext>>();
      //    var currentTenantDbContext = await dbContextProvider.GetDbContextAsync();
      //    currentTenantDbContext.Tenants.Update(tenant);
      //    await currentTenantDbContext.SaveChangesAsync();
      //}
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

  public async Task<string> CreateDatabase(Guid tenantId, string newDatabaseName, string newUserName, string newUserPassword)
  {
    string result = null;
    try
    {
      string newConnectionString = await databaseDomainService.CreateDatabaseBasedOnDefault(newDatabaseName, newUserName, newUserPassword);
      using var uow = unitOfWorkManager.Begin();
      await SetTenantDefaultConnectionString(tenantId, newConnectionString);
      await uow.SaveChangesAsync();
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

  private async Task<(Guid tenantId, string tenantName)> CreateTenantWithoutValidation(
      string tenantName,
      string adminEmail,
      string adminPassword,
      bool createDatabase,
      string newDatabaseName,
      string newUserName,
      string newUserPassword,
      string defaultConnectionString,
      bool isRoot)
  {
    using var uow = unitOfWorkManager.Begin();

    var parentId = CurrentTenant.Id;
    Tenant newTenant = null;

    // check normalized name
    var normalizedName = GetNormilizedTenantName(tenantName);

    var newTenantId = Guid.NewGuid();
    {
      newTenant = new EleoncoreTenantEntity(newTenantId, tenantName);
      newTenant.ExtraProperties.TryAdd("ParentId", parentId);
      newTenant.ExtraProperties.TryAdd("IsRoot", isRoot); // look for parentId and isRoot
      newTenant = await tenantRepository.InsertAsync(newTenant, true);
    }

    await tenantCacheService.ReloadCacheAsync();
    if (createDatabase)
    {
      await CreateDatabase(newTenant.Id, newDatabaseName, newUserName, newUserPassword);
      await dbMigrationService.MigrateTenantAsync(newTenant.Id, adminEmail, adminPassword, newUserName);
    }
    else if (!string.IsNullOrWhiteSpace(defaultConnectionString))
    {
      await SetTenantDefaultConnectionString(newTenant.Id, defaultConnectionString);
      await InitializeAdminUserAsync(newTenantId, newUserName, adminEmail, adminPassword);
    }
    else
    {
      await InitializeAdminUserAsync(newTenantId, newUserName, adminEmail, adminPassword);
    }

    await InitTenantDefaultHostname(newTenant.Id, normalizedName);

    await uow.SaveChangesAsync();
    await uow.CompleteAsync();

    return (newTenant.Id, newTenant.Name);
  }

  private async Task SetTenantDefaultConnectionString(Guid tenantId, string defaultConnectionString)
  {
    {
      Tenant tenant = await tenantRepository.GetAsync(tenantId, true);
      tenant.SetDefaultConnectionString(defaultConnectionString);
      await tenantRepository.UpdateAsync(tenant, true);
    }

    if (CurrentTenant.Id == null)
    {
      return;
    }

    using (CurrentTenant.Change(null))
    {
      Tenant tenant = await tenantRepository.GetAsync(tenantId, true);
      tenant.SetDefaultConnectionString(defaultConnectionString);
      await tenantRepository.UpdateAsync(tenant, true);
    }
    //if (CurrentTenant.Id != null)
    //{
    //    var dbContextProvider = serviceProvider.GetRequiredService<IDbContextProvider<VPortalDbContext>>();
    //    var currentTenantDbContext = await dbContextProvider.GetDbContextAsync();
    //    currentTenantDbContext.Tenants.Update(tenant);
    //    await currentTenantDbContext.SaveChangesAsync();
    //}
  }

  private async Task<string> ValidateTenantCreation(
      string tenantName,
      bool createDatabase,
      string newDatabaseName,
      string newUserName)
  {
    string validationError = null;
    try
    {
      await EnsureTenantNameAvailable(tenantName);

      if (createDatabase)
      {
        await databaseDomainService.EnsureDatabaseCanBeCreated(newDatabaseName, newUserName);
      }

      //var bindings = GetNonExistingTenantBindings(tenantName);
      //iisDomainService.EnsureIISBindingsCanBeCreated(bindings);
    }
    catch (UserFriendlyException ex)
    {
      validationError = ex.Message;
    }

    return validationError;
  }

  private async Task EnsureTenantNameAvailable(string tenantName)
  {
    var existing = await tenantRepository.FindByNameAsync(tenantName); // todo find by normalizedname
    if (existing != null)
    {
      throw new UserFriendlyException(localizer["CreateTenant:Error:TenantNameTaken", existing.Name]);
    }
  }

  private async Task InitTenantDefaultHostname(Guid tenantId, string normalizedTenantName)
  {
    var domain = _configuration["App:Domain"] ?? throw new Exception("Domain was not defined");
    var response = await _eventBus.RequestAsync<AddHostnameResponseMsg>(new AddHostnameRequestMsg
    {
      TenantId = tenantId,
      Domain = domain,
      IsDefault = true,
      IsInternal = true,
      TenantName = normalizedTenantName,
      AcceptClientCertificate = false,
      IsSsl = true,
      Port = 443,
      AppId = null,
      ApplicationType = Common.Module.Constants.VportalApplicationType.Undefined
    });

    if (response?.Success != true)
    {
      throw new Exception("Failed to add tenant hostname: " + response?.Message);
    }
  }

  private string GetNormilizedTenantName(string tenantName)
  {
    if (tenantName.Length < 3)
    {
      throw new Exception("Tenant name is invalid");
    }

    // Validate that tenant name doesn't start or end with hyphen
    if (tenantName.StartsWith('-') || tenantName.EndsWith('-'))
    {
      throw new Exception("Tenant name cannot start or end with a hyphen");
    }

    // Validate that tenant name only contains alphanumeric characters and hyphens
    if (!System.Text.RegularExpressions.Regex.IsMatch(tenantName, @"^[a-zA-Z0-9-]+$"))
    {
      throw new Exception("Tenant name can only contain letters, numbers, and hyphens");
    }

    return tenantName.ToLower();
  }

  private async Task InitializeAdminUserAsync(Guid tenantId, string userName, string email, string userPassword)
  {

    var DefaultIfEmpty = (string value, string defaultValue) => string.IsNullOrEmpty(value) ? defaultValue : value;

    var result = await _eventBus.RequestAsync<SeedIdentityResponseMsg>(new SeedIdentityRequestMsg { AdminEmail = email, AdminPassword = userPassword, AdminUserName = userName, TenantId = tenantId });

    if (!result.Success)
    {
      throw new Exception("Create user failed: " + result.Message);
    }
  }

  //private List<IisBindingInfo> GetNonExistingTenantBindings(string tenantName)
  //{
  //    var bindings = tenantHostnameDomainService.GetNonExistentTenantHostnames(tenantName);
  //    return GetTenantBindings(bindings);
  //}

  //private async Task<List<IisBindingInfo>> GetTenantBindings(Guid tenantId)
  //{
  //    var bindings = await tenantHostnameDomainService.GetTenantHostnames(tenantId);
  //    return GetTenantBindings(bindings);
  //}

  //private List<IisBindingInfo> GetTenantBindings(List<TenantHostnameEntity> bindings)
  //    => bindings
  //    .Select(x => new IisBindingInfo(x.ApplicationType, x.GetHostname(), x.Port, x.AcceptsClientCertificate))
  //    .ToList();
}
