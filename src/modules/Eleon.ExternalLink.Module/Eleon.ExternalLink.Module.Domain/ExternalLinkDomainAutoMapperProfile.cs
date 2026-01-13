using AutoMapper;
using Messaging.Module.ETO;
using Volo.Abp.AutoMapper;
using VPortal.ExternalLink.Module.Entities;
using VPortal.ExternalLink.Module.ValueObjects;

namespace VPortal.ExternalLink.Module;
public class ExternalLinkDomainAutoMapperProfile : Profile
{
  public ExternalLinkDomainAutoMapperProfile()
  {
    CreateMap<ExternalLinkEntity, ExternalLinkLoginInfoValueObject>();
    CreateMap<ExternalLinkEntity, ExternalLinkEto>().ReverseMap()
        .Ignore(c => c.Id);
  }
}
