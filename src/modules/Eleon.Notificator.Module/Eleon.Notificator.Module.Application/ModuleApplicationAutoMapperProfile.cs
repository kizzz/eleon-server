using AutoMapper;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Application.Contracts.Emails;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Application.Contracts.NotificatorSettings;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Application.Contracts.PushNotifications;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Notificators.Implementations;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Shared.Options;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Shared.ValueObjects;
using EleonsoftSdk.modules.Azure;
using Messaging.Module.ETO;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Application.Contracts.Emails;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.DomainServices;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Shared.ValueObjects;
using VPortal.Notificator.Module.Entities;
using VPortal.Notificator.Module.NotificationLogs;
using VPortal.Notificator.Module.Notifications;
using VPortal.Notificator.Module.UserNotificationSettings;

namespace VPortal.Notificator.Module;

public class ModuleApplicationAutoMapperProfile : Profile
{
  public ModuleApplicationAutoMapperProfile()
  {
    CreateMap<NotificationLogEntity, NotificationLogDto>().ReverseMap();
    CreateMap<UserNotificationSettingsEntity, UserNotificationSettingsDto>();

    CreateMap<NotificationDto, EleonsoftNotification>(MemberList.None).ReverseMap();
    CreateMap<NotificatorRecepientDto, RecipientEto>().ReverseMap();

    CreateMap<NotificatorSettings, NotificatorSettingsDto>().ReverseMap();
    CreateMap<TelegramSettingsDto, TelegramOptions>().ReverseMap();
    CreateMap<AzureEwsSettingsDto, AzureEwsOptions>().ReverseMap();
    CreateMap<GeneralNotificatorSettingsDto, GeneralNotificatorOptions>().ReverseMap();

    CreateMap<SmtpOptions, SmtpSettingsDto>()
        .ForMember(dest => dest.SmtpHost, opt => opt.MapFrom(src => src.Host))
        .ForMember(dest => dest.SmtpPort, opt => opt.MapFrom(src => src.Port))
        .ForMember(dest => dest.SmtpUserName, opt => opt.MapFrom(src => src.Username))
        .ForMember(dest => dest.SmtpPassword, opt => opt.MapFrom(src => src.Password))
        .ForMember(dest => dest.SmtpDomain, opt => opt.MapFrom(src => src.Domain))
        .ForMember(dest => dest.SmtpEnableSsl, opt => opt.MapFrom(src => src.EnableSsl))
        .ForMember(dest => dest.SmtpUseDefaultCredentials, opt => opt.MapFrom(src => src.UseDefaultCredentials))
        .ReverseMap()
        .ForMember(dest => dest.Host, opt => opt.MapFrom(src => src.SmtpHost))
        .ForMember(dest => dest.Port, opt => opt.MapFrom(src => src.SmtpPort))
        .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.SmtpUserName))
        .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.SmtpPassword))
        .ForMember(dest => dest.Domain, opt => opt.MapFrom(src => src.SmtpDomain))
        .ForMember(dest => dest.EnableSsl, opt => opt.MapFrom(src => src.SmtpEnableSsl))
        .ForMember(dest => dest.UseDefaultCredentials, opt => opt.MapFrom(src => src.SmtpUseDefaultCredentials));

    CreateMap<PushNotificationValueObject, PushNotificationDto>();
  }
}
