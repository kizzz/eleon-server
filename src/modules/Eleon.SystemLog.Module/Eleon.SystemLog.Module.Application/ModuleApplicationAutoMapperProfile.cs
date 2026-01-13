using AutoMapper;
using EleonsoftModuleCollector.SystemLog.Module.SystemLog.Module.Application.Contracts.SystemLog;
using Volo.Abp.AuditLogging;
using Volo.Abp.AutoMapper;
using Volo.Abp.Identity;
using VPortal.DocMessageLog.Module.DocMessageLogs;
using VPortal.DocMessageLog.Module.Entities;
using VPortal.Infrastructure.Module.AuditLogs;
using VPortal.Infrastructure.Module.SecurityLogs;

namespace VPortal.DocMessageLog.Module;

public class ModuleApplicationAutoMapperProfile : Profile
{
  public ModuleApplicationAutoMapperProfile()
  {
    CreateMap<AuditLog, AuditLogDto>(MemberList.None)
        .ReverseMap();

    CreateMap<EntityChange, EntityChangeDto>(MemberList.None)
      .Ignore(x => x.UpdatedById)
      .Ignore(x => x.UpdatedByName)
      .ReverseMap();

    CreateMap<EntityPropertyChange, EntityPropertyChangeDto>(MemberList.None)
        .ReverseMap();

    CreateMap<AuditLog, AuditLogHeaderDto>(MemberList.None)
       .ReverseMap();

    CreateMap<AuditLogAction, AuditLogActionDto>(MemberList.None)
       .ReverseMap();

    CreateMap<IdentitySecurityLog, SecurityLogDto>()
        .ReverseMap();

    CreateMap<IdentitySecurityLog, FullSecurityLogDto>()
        .ReverseMap();

    CreateMap<SystemLogEntity, SystemLogDto>();
    CreateMap<UnresolvedSystemLogCount, UnresolvedSystemLogCountDto>();

    CreateMap<SystemLogEntity, FullSystemLogDto>()
            .ForMember(dest => dest.ExtraProperties, opt => opt.MapFrom(src => src.ExtraProperties.ToDictionary(x => x.Key, x => x.Value == null ? string.Empty : x.Value.ToString())));
  }
}
