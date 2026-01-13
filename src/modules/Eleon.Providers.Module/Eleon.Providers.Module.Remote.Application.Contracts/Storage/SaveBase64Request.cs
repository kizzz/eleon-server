namespace VPortal.Storage.Remote.Application.Contracts.Storage
{
  public class SaveBase64Request
  {
    public string SettingsGroup { get; set; }
    public string BlobName { get; set; }
    public string DataBase64 { get; set; }
  }
}
