using AutoMapper;
using Volo.Abp.AutoMapper;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.FileArchiveFavourites;
using VPortal.FileManager.Module.FileArchivePermissions;
using VPortal.FileManager.Module.FileArchives;
using VPortal.FileManager.Module.FileExternalLink;
using VPortal.FileManager.Module.Files;
using VPortal.FileManager.Module.Folders;
using VPortal.FileManager.Module.PhysicalFolders;
using VPortal.FileManager.Module.ValueObjects;

namespace VPortal.FileManager.Module;

public class ModuleApplicationAutoMapperProfile : Profile
{
  public ModuleApplicationAutoMapperProfile()
  {
    CreateMap<FileSystemEntry, FileSystemEntryDto>().ReverseMap();

    CreateMap<FileSystemEntry, HierarchyFolderDto>()
        .Ignore(c => c.Level)
        .Ignore(c => c.SystemFolderName)
        .Ignore(c => c.Files)
        .ReverseMap();

    CreateMap<PhysicalFolderEntity, PhysicalFolderDto>().ReverseMap();

    CreateMap<FileSourceValueObject, FileSourceDto>().ReverseMap();


    CreateMap<FileArchiveEntity, FileArchiveDto>().ReverseMap();

    CreateMap<FileArchivePermissionDto, FileArchivePermissionValueObject>().ReverseMap();
    CreateMap<FileArchivePermissionKeyDto, FileArchivePermissionKeyValueObject>().ReverseMap();
    CreateMap<FileArchivePermissionEntity, FileArchivePermissionDto>();
    CreateMap<FileArchiveFavouriteEntity, FileArchiveFavouriteDto>();
    CreateMap<FileArchiveFavouriteDto, FileArchiveFavouriteValueObject>().ReverseMap();

    CreateMap<FileExternalLinkReviewerEntity, FileExternalLinkReviewerDto>().ReverseMap();
    CreateMap<FileExternalLinkEntity, FileExternalLinkDto>().ReverseMap();
  }
}
