using AutoMapper;
using Core.Infrastructure.Module.DashboardSettings;
using Core.Infrastructure.Module.Entities;
using ModuleCollector.Infrastructure.Module.Infrastructure.Module.Application.Contracts.Currency;
using ModuleCollector.Infrastructure.Module.Infrastructure.Module.Domain.Shared.Currency;
using VPortal.Core.Infrastructure.Module.Entities;
using VPortal.Core.Infrastructure.Module.FeatureSettings;
using VPortal.Infrastructure.Module.Addresses;
using VPortal.Infrastructure.Module.Entities;

namespace Infrastructure.Module;
public class ModuleApplicationAutoMapperProfile : Profile
{
  public ModuleApplicationAutoMapperProfile()
  {
    CreateMap<AddressEntity, AddressDto>()
        .ReverseMap();

    CreateMap<SetFeatureSettingDto, FeatureSettingEntity>(MemberList.Source);
    CreateMap<DashboardSettingEntity, DashboardSettingDto>().ReverseMap();
    CreateMap<FeatureSettingEntity, FeatureSettingDto>(MemberList.None).ReverseMap();
    CreateMap<CurrencyEntity, CurrencyDto>(MemberList.None)
        .ReverseMap();
    CreateMap<CurrencyRateEntity, CurrencyRateDto>(MemberList.None)
        .ReverseMap();
  }
}
