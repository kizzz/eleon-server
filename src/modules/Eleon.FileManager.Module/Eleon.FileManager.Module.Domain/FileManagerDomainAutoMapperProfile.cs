using AutoMapper;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.ValueObjects;

namespace VPortal.FileManager.Module;
public class FileManagerDomainAutoMapperProfile : Profile
{
  public FileManagerDomainAutoMapperProfile()
  {
    CreateMap<FileSystemEntry, HierarchyFolderValueObject>();
    CreateMap<FileSystemEntry, FileValueObject>();
  }
}
