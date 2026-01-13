using Common.EventBus.Module;
using Common.Module.Constants;
using Common.Module.Keys;
using Logging.Module;
using Messaging.Module.Messages;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.Uow;

namespace VPortal.TenantManagement.Module.DomainServices
{
    
    public class UserProfilePictureDomainService : DomainService
    {
        private readonly IVportalLogger<UserProfilePictureDomainService> logger;
        private readonly IDistributedEventBus saveStorageItemClient;
        private readonly IIdentityUserRepository identityUserRepository;

        public UserProfilePictureDomainService(
            IVportalLogger<UserProfilePictureDomainService> logger,
            IDistributedEventBus saveStorageItemClient,
            IIdentityUserRepository identityUserRepository)
        {
            this.logger = logger;
            this.saveStorageItemClient = saveStorageItemClient;
            this.identityUserRepository = identityUserRepository;
        }

        public async Task SetUserProfilePictures(Guid userId, string profilePictureBase64, string profilePictureThumbnailBase64)
        {
            try
            {
                var user = await identityUserRepository.GetAsync(userId);

                string profilePictureId = GetKey(userId + "-fullAvatar");
                await SavePicture(userId, profilePictureId, profilePictureBase64);

                string profilePictureThumbId = GetKey(userId + "-avatar");
                await SavePicture(userId, profilePictureThumbId, profilePictureThumbnailBase64);

                await identityUserRepository.UpdateAsync(user, true);
            }
            catch (Exception ex)
            {
                logger.Capture(ex);
            }

        }

        private async Task SavePicture(Guid userId, string key, string base64)
        {
            var msg = new SaveLightweightStorageItemMsg
            {
                Key = key,
                BlobBase64 = base64,
                CacheExpiration = TimeSpan.FromDays(2),
                MaxSizeUnit = Common.Module.Helpers.SizeUnits.KB,
                MaxSize = 100
            };

            var response = await saveStorageItemClient.RequestAsync<ActionCompletedMsg>(msg);
            if (!response.Success)
            {
                throw new Exception($"Unable to save picture to blob storage. Error: {response.Error}");
            }
        }

        private string GetKey(string blobName)
            => new LightweightStorageKey(nameof(TenantManagement), blobName).ToString();
    }
}
