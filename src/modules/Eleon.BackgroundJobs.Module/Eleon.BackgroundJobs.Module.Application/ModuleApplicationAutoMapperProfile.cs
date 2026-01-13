using AutoMapper;
using BackgroundJobs.Module.BackgroundJobs;
using System;
using VPortal.BackgroundJobs.Module.Entities;

namespace VPortal.BackgroundJobs.Module;

public class ModuleApplicationAutoMapperProfile : Profile
{
  public ModuleApplicationAutoMapperProfile()
  {
    CreateMap<BackgroundJobDto, BackgroundJobEntity>(MemberList.Source)
        .ForMember(pr => pr.Id, opt => opt.Condition(pr => pr.Id != Guid.Empty));

    CreateMap<BackgroundJobEntity, BackgroundJobDto>(MemberList.None);
    CreateMap<BackgroundJobEntity, FullBackgroundJobDto>()
        .ForMember(dest => dest.ExtraProperties, opt => opt.MapFrom(src => src.ExtraProperties.ToDictionary(x => x.Key, x => x.Value == null ? string.Empty : x.Value.ToString())));

    CreateMap<BackgroundJobEntity, BackgroundJobHeaderDto>();

    CreateMap<BackgroundJobExecutionEntity, BackgroundJobExecutionDto>()
        .ReverseMap()
        .ForMember(pr => pr.Id, opt => opt.Ignore());

    CreateMap<BackgroundJobMessageEntity, BackgroundJobMessageDto>()
        .ReverseMap()
        .ForMember(pr => pr.Id, opt => opt.Ignore());
  }
}
