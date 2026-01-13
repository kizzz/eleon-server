using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace VPortal.TenantManagement.Module.Users
{
  public interface IUserProfilePictureAppService : IApplicationService
  {
    Task<bool> SetUserProfilePicture(SetUserProfilePictureRequest request);
  }
}
