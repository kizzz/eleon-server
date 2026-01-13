using AutoMapper;
using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.Contracts.HealthCheck;
using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Constants;
using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Entities;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;
using System.Linq.Expressions;
using Volo.Abp.AutoMapper;


namespace VPortal.HealthCheckModule.Module;

public class HealthCheckApplicationAutoMapperProfile : Profile
{
  public HealthCheckApplicationAutoMapperProfile()
  {
    Expression<Func<HealthCheck, HealthCheckStatus>> healthCheckStatusMapExpression = src =>
                    src.Status == HealthCheckStatus.Failed || src.InProgress && (src.CreationTime + TimeSpan.FromHours(1) <= DateTime.UtcNow) ? HealthCheckStatus.Failed
                    : (src.InProgress ? HealthCheckStatus.InProgress : HealthCheckStatus.OK);

    CreateMap<HealthCheck, HealthCheckDto>()
        .ForMember(dest => dest.Status, opt => opt.MapFrom(healthCheckStatusMapExpression));
    CreateMap<HealthCheck, FullHealthCheckDto>()
        .ForMember(dest => dest.ExtraProperties, opt => opt.MapFrom(src => src.ExtraProperties.ToDictionary(x => x.Key, x => x.Value == null ? string.Empty : x.Value.ToString())))
        .ForMember(dest => dest.Status, opt => opt.MapFrom(healthCheckStatusMapExpression));

    CreateMap<HealthCheckReport, HealthCheckReportDto>()
        .ForMember(dest => dest.ExtraInformation, opt => opt.MapFrom(src => src.ExtraInformation))
        .ReverseMap();

    CreateMap<HealthCheckReport, HealthCheckReportEto>()
        .ForMember(dest => dest.ExtraInformation, opt => opt.MapFrom(src => src.ExtraInformation))
        .ReverseMap();

    CreateMap<ReportExtraInformation, ReportExtraInformationDto>().ReverseMap().Ignore(dest => dest.Id);
    CreateMap<ReportExtraInformationEto, ReportExtraInformation>()
        .Ignore(dest => dest.Id)
        .ReverseMap();

    CreateMap<SendHealthCheckDto, HealthCheck>(MemberList.None)
        .Ignore(dest => dest.InProgress)
        .Ignore(dest => dest.Status)
        .Ignore(dest => dest.ExtraProperties);
  }
}
