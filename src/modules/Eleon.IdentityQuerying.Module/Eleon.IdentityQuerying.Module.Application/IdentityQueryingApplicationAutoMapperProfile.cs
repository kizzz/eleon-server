using AutoMapper;
using Common.Module.Helpers;
using Common.Module.ValueObjects;
using Eleon.IdentityQuerying.Module.Full.Eleon.IdentityQuerying.Module.Application.Contracts.OrganizationUnits;
using Eleon.IdentityQuerying.Module.Full.Eleon.IdentityQuerying.Module.Application.Contracts.Roles;
using Eleon.IdentityQuerying.Module.Full.Eleon.IdentityQuerying.Module.Application.Contracts.Tenants;
using Eleon.IdentityQuerying.Module.Full.Eleon.IdentityQuerying.Module.Application.Contracts.Users;
using Messaging.Module.ETO;
using ModuleCollector.SitesManagement.Module.SitesManagement.Module.Application.Contracts.EleoncoreApplicationConfiguration;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Shared.ValueObjects;
using Volo.Abp.AutoMapper;
using Volo.Abp.Data;
using Volo.Abp.Identity;
using Volo.Abp.TenantManagement;
using VPortal.Infrastructure.Module.Result;
using VPortal.SitesManagement.Module.ClientApplications;
using VPortal.SitesManagement.Module.Microservices;
using VPortal.TenantManagement.Module.OrganizationUnits;
using VPortal.TenantManagement.Module.Roles;
using VPortal.TenantManagement.Module.Users;
using VPortal.TenantManagement.Module.ValueObjects;

namespace VPortal.HealthCheckModule.Module;

public class IdentityQueryingApplicationAutoMapperProfile : Profile
{
  public IdentityQueryingApplicationAutoMapperProfile()
  {
    CreateMap<IdentityUser, CommonUserDto>()
       .Ignore(x => x.OrganizationUnits)
       .Ignore(x => x.Roles)
       .Ignore(x => x.LastLoginDate)
       .ForMember(x => x.ProfilePicture, opt => opt.MapFrom(x => x.GetProfilePicture()))
       .ForMember(x => x.ProfilePictureThumbnail, opt => opt.MapFrom(x => x.GetProfilePictureThumbnail()));

    CreateMap<CommonUserDto, IdentityUser>(MemberList.None);

    CreateMap<IdentityUser, IdentityUserDto>();
    CreateMap<CommonRoleValueObject, CommonRoleDto>();
    CreateMap<OrganizationUnit, CommonOrganizationUnitDto>()
        .ForMember(x => x.IsEnabled, opt => opt.MapFrom(src => src.GetProperty("IsEnabled", false)))
        .ReverseMap();

    CreateMap<ResultError, ResultErrorDto>();
    CreateMap<ResultSuccess, ResultSuccessDto>();

    CreateMap<TreeNodeValueObject<OrganizationUnit>, CommonOrganizationUnitTreeNodeDto>();

    CreateMap<UserRoleLookup, UserRoleLookupDto>();
    CreateMap<RoleUserLookup, RoleUserLookupDto>()
        .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));

    CreateMap<UserOrganizationUnitLookup, UserOrganizationUnitLookupDto>();


    CreateMap<ResultError, ResultErrorDto>();
    CreateMap<ResultSuccess, ResultSuccessDto>();

    CreateMap<ResultValueObject<string>, ResultDto<string>>();

    CreateMap<ClientApplicationEto, ClientApplicationDto>();
    CreateMap<ApplicationModuleEto, ApplicationModuleDto>();
    CreateMap<ApplicationPropertyEto, ClientApplicationPropertyDto>();

    CreateMap<OAuthConfigValueObject, OAuthConfigDto>();
    CreateMap<WebPushConfigValueObject, WebPushConfigDto>();
    CreateMap<EleoncoreApplicationConfigurationValueObject, EleoncoreApplicationConfigurationDto>();

    CreateMap<TenantConnectionString, TenantConnectionStringDto>()
        .ReverseMap();

    CreateMap<Tenant, CommonTenantDto>(MemberList.None)
        .ReverseMap();


    CreateMap<Tenant, CommonTenantExtendedDto>()
        .Ignore(c => c.IsRoot)
        .Ignore(c => c.ParentId);
  }
}
