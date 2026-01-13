using AutoMapper;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages;
using Messaging.Module.ETO;
using Volo.Abp.AutoMapper;
using VPortal.BackgroundJobs.Module.Entities;

namespace VPortal.BackgroundJobs.Module
{
  public class BackgroundJobsModuleDomainAutoMapperProfile : Profile
  {
    public BackgroundJobsModuleDomainAutoMapperProfile()
    {
      CreateMap<BackgroundJobEntity, BackgroundJobEto>()
          .ReverseMap();
      CreateMap<BackgroundJobExecutionEntity, BackgroundJobExecutionEto>()
          .ReverseMap()
          .Ignore(dest => dest.TenantId)
          .Ignore(dest => dest.Messages);
    }
  }
}
