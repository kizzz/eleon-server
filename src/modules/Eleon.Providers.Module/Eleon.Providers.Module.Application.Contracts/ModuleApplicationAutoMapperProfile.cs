using AutoMapper;
using Eleon.Storage.Lib.Models;
using Eleon.Storage.Module.Eleon.Storage.Module.Domain.Shared.Entities;
using EleonsoftModuleCollector.Storage.Module.Storage.Module.Application.Contracts.StorageProviders;
using SharedModule.modules.Blob.Module.Constants;
using SharedModule.modules.Blob.Module.Models;
using VPortal.Storage.Module.Entities;

namespace VPortal.Storage.Module.Application.Contracts;

public class ModuleApplicationAutoMapperProfile : Profile
{
  public ModuleApplicationAutoMapperProfile()
  {
    CreateMap<StorageProviderEntity, MinimalStorageProviderDto>();
    CreateMap<StorageProviderSettingTypeEntity, StorageProviderSettingTypeDto>()
         .ReverseMap();
    CreateMap<StorageProviderEntity, StorageProviderSaveResponseDto>();
    CreateMap<StorageProviderEntity, StorageProviderDto>()
        .ReverseMap();
    CreateMap<StorageProviderTypeEntity, StorageProviderTypeDto>().ReverseMap();
    //.ForMember(x => x.Settings, opt => opt.Ignore())
    //.ForMember(x => x.Id, opt => opt.Condition(x => x.Id != Guid.Empty));
    CreateMap<StorageProviderSettingEntity, StorageProviderSettingDto>()
        .ReverseMap();
    //.ForMember(x => x.Id, opt => opt.Condition(x => x.Id != Guid.Empty));
  }
}
