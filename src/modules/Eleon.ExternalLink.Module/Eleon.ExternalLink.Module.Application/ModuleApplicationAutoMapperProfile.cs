using AutoMapper;
using VPortal.ExternalLink.Module.Entities;
using VPortal.ExternalLink.Module.FileExternalLink;
using VPortal.ExternalLink.Module.ValueObjects;

namespace VPortal.ExternalLink.Module;

public class ModuleApplicationAutoMapperProfile : Profile
{
  public ModuleApplicationAutoMapperProfile()
  {
    CreateMap<ExternalLinkEntity, ExternalLinkDto>().ReverseMap();
    CreateMap<ExternalLinkEntity, ExternalLinkLoginInfoDto>().ReverseMap();
    CreateMap<ExternalLinkLoginInfoValueObject, ExternalLinkLoginInfoDto>().ReverseMap();
  }
}
