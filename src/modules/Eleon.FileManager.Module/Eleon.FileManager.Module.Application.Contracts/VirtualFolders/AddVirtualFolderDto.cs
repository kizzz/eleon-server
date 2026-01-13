namespace VPortal.FileManager.Module.VirtualFolders
{
  public class AddVirtualFolderDto
  {
    public string Name { get; set; }
    public string PhysicalFolderId { get; set; }
    public string ParentId { get; set; }
  }
}
