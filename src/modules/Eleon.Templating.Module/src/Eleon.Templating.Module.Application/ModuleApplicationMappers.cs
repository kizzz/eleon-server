using AutoMapper;
using Eleon.Templating.Module.Templates;

namespace Eleon.Templating.Module;

public class ModuleApplicationAutoMapperProfile : Profile
{
  public ModuleApplicationAutoMapperProfile()
  {
    CreateMap<Template, MinimalTemplateDto>();
    CreateMap<Template, TemplateDto>(MemberList.None).ReverseMap();
    CreateMap<CreateUpdateTemplateDto, Template>(MemberList.Source);
  }
}
