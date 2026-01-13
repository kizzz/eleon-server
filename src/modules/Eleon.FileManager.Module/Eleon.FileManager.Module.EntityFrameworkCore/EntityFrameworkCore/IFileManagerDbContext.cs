using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using VPortal.FileManager.Module.Entities;

namespace VPortal.FileManager.Module.EntityFrameworkCore;

[ConnectionStringName(ModuleDbProperties.ConnectionStringName)]
public interface IFileManagerDbContext : IEfCoreDbContext
{
  public DbSet<FileArchiveEntity> FileArchives { get; set; }
  public DbSet<FileSystemEntry> FileSystemEntries { get; set; }
  public DbSet<FileStatusEntity> FileStatuses { get; set; }
  public DbSet<FileExternalLinkEntity> FileExternalLinks { get; set; }
  public DbSet<PhysicalFolderEntity> PhysicalFolders { get; set; }
  public DbSet<FileArchivePermissionEntity> FileArchivePermissions { get; set; }
  public DbSet<FileArchiveFavouriteEntity> FileArchiveFavourites { get; set; }
}
