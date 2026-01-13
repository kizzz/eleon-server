using Common.EventBus.Module;
using Common.Module.Constants;
using EleonsoftModuleCollector.FileManager.Module.FileManager.Module.Domain.Shared.Constants;
using Logging.Module;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.Managers;
using VPortal.FileManager.Module.Repositories;

namespace VPortal.FileManager.Module.DomainServices
{
    public class FileExternalLinkDomainService : DomainService
    {
        private readonly IVportalLogger<FileExternalLinkDomainService> logger;
        private readonly FileArchivePermissionCheckerDomainService fileArchivePermissionCheckerDomainService;
        private readonly FileEditManager fileEditManager;
        private readonly Managers.FileManager fileManager;
        private readonly FileStatusDomainService fileStatusDomainService;
        private readonly IdentityUserManager identityUserManager;
        private readonly ICurrentUser currentUser;
        private readonly IDistributedEventBus getRequestClient;
        private readonly IArchiveRepository archiveRepository;
        private readonly IDistributedEventBus createRequestClient;
        private readonly IDistributedEventBus updateRequestClient;
        private readonly IDistributedEventBus massTransitPublisher;
        private readonly IdentityRoleManager identityRoleManager;
        private readonly OrganizationUnitManager organizationUnitManager;
        private readonly UnitOfWorkManager unitOfWorkManager;
        private readonly IOrganizationUnitRepository orgUnitRepository;
        private readonly IFileExternalLinkRepository repository;

        public FileExternalLinkDomainService(
            IVportalLogger<FileExternalLinkDomainService> logger,
            FileArchivePermissionCheckerDomainService fileArchivePermissionCheckerDomainService,
            FileEditManager fileEditManager,
            Managers.FileManager fileManager,
            FileStatusDomainService fileStatusDomainService,
            IdentityUserManager identityUserManager,
            ICurrentUser currentUser,
            IDistributedEventBus getRequestClient,
            IArchiveRepository archiveRepository,
            IDistributedEventBus createRequestClient,
            IDistributedEventBus updateRequestClient,
            IDistributedEventBus massTransitPublisher,
            IdentityRoleManager identityRoleManager,
            OrganizationUnitManager organizationUnitManager,
            UnitOfWorkManager unitOfWorkManager,
            IOrganizationUnitRepository orgUnitRepository,
            IFileExternalLinkRepository repository)
        {
            this.logger = logger;
            this.fileArchivePermissionCheckerDomainService = fileArchivePermissionCheckerDomainService;
            this.fileEditManager = fileEditManager;
            this.fileManager = fileManager;
            this.fileStatusDomainService = fileStatusDomainService;
            this.identityUserManager = identityUserManager;
            this.currentUser = currentUser;
            this.getRequestClient = getRequestClient;
            this.archiveRepository = archiveRepository;
            this.createRequestClient = createRequestClient;
            this.updateRequestClient = updateRequestClient;
            this.massTransitPublisher = massTransitPublisher;
            this.identityRoleManager = identityRoleManager;
            this.organizationUnitManager = organizationUnitManager;
            this.unitOfWorkManager = unitOfWorkManager;
            this.orgUnitRepository = orgUnitRepository;
            this.repository = repository;
        }

        public async Task<List<FileExternalLinkEntity>> GetLinksAsync(Guid archiveId, List<FileShareStatus> fileShareStatuses)
        {

            List<FileExternalLinkEntity> result = new List<FileExternalLinkEntity>();
            try
            {
                var links = await repository.GetListAsync();
                result = links
                    .Where(f => f.ArchiveId == archiveId)
                    .Where(f => fileShareStatuses.Contains(f.PermissionType))
                    .ToList();
            }
            catch (Exception e)
            {
                logger.Capture(e);
            }
            finally
            {
            }
            return result;

        }

        public async Task<FileExternalLinkEntity> GetAsync(string fileId, Guid archiveId, bool minimalCheck = false)
        {
            FileExternalLinkEntity result = null;
            try
            {

                if (!await fileArchivePermissionCheckerDomainService.CheckFilePermission(archiveId, fileId, Common.Module.Constants.FileManagerPermissionType.Modify, FileManagerType.FileArchive))
                {
                    throw new AbpAuthorizationException();
                }

                var file = await fileManager.GetEntryById(fileId, archiveId, FileManagerType.FileArchive);

                var links = await repository.GetListAsync(includeDetails: true);
                result = links
                    .FirstOrDefault(f => f.ArchiveId == archiveId && f.FileId == fileId);

                if (result == null)
                {
                    result = new FileExternalLinkEntity()
                    {
                        FileId = fileId,
                        ArchiveId = archiveId,
                        PermissionType = Common.Module.Constants.FileShareStatus.None,
                        Reviewers = new List<FileExternalLinkReviewerEntity>(),
                    };
                }

                foreach (var reviewer in result.Reviewers)
                {
                    if (reviewer.ReviewerType == FileReviewerType.External)
                    {
                        var message = new GetExternalLinkMsg
                        {
                            LinkCode = reviewer.ExternalLinkCode
                        };
                        var response = await getRequestClient.RequestAsync<SendExternalLinkMsg>(message);
                        reviewer.ExternalLink = response.ExternalLinkEto;
                    }
                }

                if (minimalCheck)
                {
                    return result;
                }

                foreach (var reviewer in result.Reviewers)
                {
                    await FillReviewerLabel(reviewer);

                }
            }
            catch (Exception e)
            {
                logger.Capture(e);
            }
            finally
            {
            }
            return result;

        }

        private async Task FillReviewerLabel(FileExternalLinkReviewerEntity reviewer)
        {
            try
            {
                if (reviewer.ReviewerType == Common.Module.Constants.FileReviewerType.User)
                {
                    var user = await identityUserManager.FindByIdAsync(reviewer.ReviewerKey);
                    reviewer.ReviewerKeyLabel = await identityUserManager.GetUserNameAsync(user);
                }
                if (reviewer.ReviewerType == Common.Module.Constants.FileReviewerType.Role)
                {
                    var role = await identityRoleManager.FindByIdAsync(reviewer.ReviewerKey);
                    reviewer.ReviewerKeyLabel = role.Name;
                }
                if (reviewer.ReviewerType == Common.Module.Constants.FileReviewerType.OrganizationUnit
                      && Guid.TryParse(reviewer.ReviewerKey, out Guid orgUnitId))
                {
                    var orgUnit = await orgUnitRepository.FindAsync(orgUnitId);
                    reviewer.ReviewerKeyLabel = orgUnit.DisplayName;
                }

                if (reviewer.ReviewerType == Common.Module.Constants.FileReviewerType.External)
                {
                    var message = new GetExternalLinkMsg
                    {
                        LinkCode = reviewer.ExternalLinkCode
                    };
                    var response = await getRequestClient.RequestAsync<SendExternalLinkMsg>(message);
                    var externalLink = response.ExternalLinkEto;
                    if (externalLink.LoginType == Common.Module.Constants.ExternalLinkLoginType.Email)
                    {
                        string pattern = @"(?<=[\w]{1})[\w\-._\+%]*(?=[\w]{1}@)";
                        reviewer.ReviewerKeyLabel = Regex.Replace(reviewer.ReviewerKey, pattern, m => new string('*', m.Length));
                    }

                    if (externalLink.LoginType == Common.Module.Constants.ExternalLinkLoginType.Sms)
                    {
                        string pattern = @"\d(?!\d{0,3}$)";
                        reviewer.ReviewerKeyLabel = Regex.Replace(reviewer.ReviewerKey, pattern, m => new string('*', m.Length));
                    }

                    if (externalLink.LoginType == Common.Module.Constants.ExternalLinkLoginType.Password)
                    {
                        reviewer.ReviewerKeyLabel = reviewer.ReviewerKey;
                    }
                }
            }
            catch (Exception)
            {
                reviewer.ReviewerKeyLabel = "Not Found";
            }
        }

        public async Task<FileExternalLinkReviewerEntity> GetReviewerAsync(Guid id)
        {

            FileExternalLinkReviewerEntity result = null;
            try
            {
                FileExternalLinkEntity link = await GetReviewerLink(id);

                if (link != null)
                {
                    result = link.Reviewers.FirstOrDefault(r => r.Id == id);
                }
            }
            catch (Exception e)
            {
                logger.Capture(e);
            }
            finally
            {
            }
            return result;

        }

        public async Task<FileExternalLinkEntity> GetReviewerLink(Guid id)
        {
            var links = await repository.GetListAsync(includeDetails: true);
            return links
                .FirstOrDefault(l => l.Reviewers.Any(r => r.Id == id));
        }

        public async Task<FileExternalLinkReviewerEntity> CreateOrUpdateReviewer(
            Guid fileExternalLinkAggregateId, FileExternalLinkReviewerEntity updatedEntity, ExternalLinkEto externalLinkEntity)
        {

            FileExternalLinkReviewerEntity result = null;
            try
            {
                var link = await repository.GetAsync(fileExternalLinkAggregateId, true);

                var reviewer = link.Reviewers.FirstOrDefault(r => r.Id == updatedEntity.Id);


                if (externalLinkEntity != null)
                {
                    externalLinkEntity.DocumentType = "File";
                    externalLinkEntity.ExternalLinkUrl = "/ui/vportal/external/{link}";
                    externalLinkEntity.PrivateParams = link.WebUrl;
                    externalLinkEntity.ExpirationDateTime = updatedEntity.ExpirationDateTime;
                }

                if (updatedEntity.Id != Guid.Empty)
                {
                    reviewer = await GetReviewerAsync(updatedEntity.Id);
                }

                if (reviewer == null)
                {
                    reviewer = new FileExternalLinkReviewerEntity(GuidGenerator.Create())
                    {
                        ExpirationDateTime = updatedEntity.ExpirationDateTime,
                        ReviewerKey = updatedEntity.ReviewerKey,
                        ReviewerType = updatedEntity.ReviewerType,
                        ReviewerStatus = updatedEntity.ReviewerStatus,
                    };

                    if (externalLinkEntity != null)
                    {
                        var message = new CreateExternalLinkMsg
                        {
                            NewExternalLinkEto = externalLinkEntity
                        };
                        var response = await createRequestClient.RequestAsync<SendExternalLinkCreatedMsg>(message);
                        externalLinkEntity = response.ExternalLinkCreated;

                        reviewer.ExternalLinkCode = externalLinkEntity.ExternalLinkCode;
                        reviewer.ExternalLink = externalLinkEntity;
                    }

                    link.Reviewers.Add(reviewer);
                }
                else
                {
                    if (externalLinkEntity != null)
                    {
                        var message = new UpdateExternalLinkMsg
                        {
                            UpdateExternalLinkEto = externalLinkEntity
                        };
                        var response = await updateRequestClient.RequestAsync<SendExternalLinkMsg>(message);
                        externalLinkEntity = response.ExternalLinkEto;

                        reviewer.ExternalLinkCode = externalLinkEntity.ExternalLinkCode;
                    }
                }

                reviewer.ExpirationDateTime = updatedEntity.ExpirationDateTime;
                reviewer.ReviewerKey = updatedEntity.ReviewerKey;
                reviewer.ReviewerType = updatedEntity.ReviewerType;
                reviewer.ReviewerStatus = updatedEntity.ReviewerStatus;

                await repository.UpdateAsync(link, true);

                await FillReviewerLabel(reviewer);

                result = reviewer;
            }
            catch (Exception e)
            {
                logger.Capture(e);
            }
            finally
            {
            }
            return result;

        }

        public async Task<bool> DeleteReviewerAsync(Guid reviewerId)
        {

            bool result = false;
            try
            {
                var link = await GetReviewerLink(reviewerId);

                var reviewer = await GetReviewerAsync(reviewerId);

                if (reviewer.ReviewerType == FileReviewerType.External)
                {
                    var message = new DeleteExternalLinkMsg
                    {
                        LinkCode = reviewer.ExternalLinkCode
                    };
                    await massTransitPublisher.PublishAsync(message);
                }

                link.Reviewers.Remove(reviewer);

                await repository.UpdateAsync(link, true);
            }
            catch (Exception e)
            {
                logger.Capture(e);
            }
            finally
            {
            }
            return result;

        }

        public async Task<bool> Cancel(Guid linkId)
        {

            bool result = false;
            try
            {
                var externalLink = await repository.GetAsync(linkId, true);

                var archive = await archiveRepository.GetAsync(externalLink.ArchiveId);

                await fileEditManager.DeleteFile(externalLink.ExternalFileId);

                foreach (var reviewer in externalLink.Reviewers)
                {
                    if (reviewer.ReviewerType == FileReviewerType.External)
                    {
                        var message = new DeleteExternalLinkMsg
                        {
                            LinkCode = reviewer.ExternalLinkCode
                        };
                        await massTransitPublisher.PublishAsync(message);
                    }
                }

                await repository.DeleteAsync(externalLink.Id, true);

                result = true;
            }
            catch (Exception e)
            {
                logger.Capture(e);
            }
            finally
            {
            }

            return result;
        }

        public async Task<bool> SaveExternalChanges(Guid linkId, bool deleteLinkAfterSave)
        {

            bool result = false;
            try
            {
                var externalLink = await repository.GetAsync(linkId, true);

                if (externalLink.PermissionType == Common.Module.Constants.FileShareStatus.Readonly)
                {
                    return result;
                }

                var archive = await archiveRepository.GetAsync(externalLink.ArchiveId);

                var newFileData = await fileEditManager.DownloadFile(externalLink.ExternalFileId);


                var file = await fileManager.UploadNewVersion(externalLink.ArchiveId, externalLink.FileId, new System.IO.MemoryStream(newFileData), FileManagerType.FileArchive);

                await fileStatusDomainService.UpdateFileStatus(externalLink.ArchiveId, externalLink.FileId, FileStatus.Archived);

                if (deleteLinkAfterSave)
                {
                    await Cancel(linkId);
                }
                else
                {
                    externalLink.FileId = file.Id;
                    await repository.UpdateAsync(externalLink, true);
                }

                result = true;
            }
            catch (Exception e)
            {
                logger.Capture(e);
            }
            finally
            {
            }
            return result;

        }

        public async Task<FileExternalLinkEntity> CreateAsync(FileExternalLinkEntity updatedEntity)
        {

            FileExternalLinkEntity result = null;
            try
            {
                if (!await fileArchivePermissionCheckerDomainService.CheckFilePermission(updatedEntity.ArchiveId, updatedEntity.FileId, Common.Module.Constants.FileManagerPermissionType.Modify, FileManagerType.FileArchive))
                {
                    throw new AbpAuthorizationException();
                }

                using (var uof = unitOfWorkManager.Begin())
                {
                    result = await GetAsync(updatedEntity.FileId, updatedEntity.ArchiveId);
                    if (result == null || result.Id == Guid.Empty)
                    {
                        result = new FileExternalLinkEntity(GuidGenerator.Create())
                        {
                            PermissionType = updatedEntity.PermissionType,
                            ArchiveId = updatedEntity.ArchiveId,
                            FileId = updatedEntity.FileId,
                            //ExternalFileId = googleDriveFileId,
                            //Reviewers = updatedEntity.Reviewers.Select(r => new FileExternalLinkReviewerEntity(GuidGenerator.Create())
                            //{
                            //    ExpirationDateTime = r.ExpirationDateTime,
                            //    ExternalPrivateType = r.ExternalPrivateType,
                            //    ReviewerKey = r.ReviewerKey,
                            //    ReviewerType = r.ReviewerType,
                            //    ReviewerStatus = r.ReviewerStatus,
                            //}).ToList(),
                            //WebUrl = webUrl,
                        };

                        await repository.InsertAsync(result, true);
                        await uof.SaveChangesAsync();
                    }
                    else
                    {
                        return result;
                    }
                }

                string googleDriveFileId = null;
                string webUrl = null;
                if (result == null || result.Id == Guid.Empty || string.IsNullOrEmpty(result.WebUrl))
                {
                    var file = await fileManager.DownloadFile(updatedEntity.FileId, false, updatedEntity.ArchiveId, FileManagerType.FileArchive);

                    var archive = await archiveRepository.GetAsync(updatedEntity.ArchiveId);


                    var name = updatedEntity.PermissionType switch
                    {
                        FileShareStatus.Readonly => "reader",
                        FileShareStatus.Comment => "commenter",
                        FileShareStatus.Modify => "writer",
                        FileShareStatus.None => "none",
                        _ => "none"
                    };

                    (googleDriveFileId, webUrl) = await fileEditManager.Upload(name, file);
                }
                else
                {
                    googleDriveFileId = result.ExternalFileId;

                    webUrl = result.WebUrl;
                }

                result.PermissionType = updatedEntity.PermissionType;
                result.ArchiveId = updatedEntity.ArchiveId;
                result.FileId = updatedEntity.FileId;
                result.WebUrl = webUrl;
                result.ExternalFileId = googleDriveFileId;

                await repository.UpdateAsync(result, true);
            }
            catch (Exception e)
            {
                logger.Capture(e);
            }
            finally
            {
            }
            return result;

        }

        private async Task<string> GetContentType(FileExternalLinkEntity updatedEntity, bool export = false)
        {
            if (export)
            {
                return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            }

            return "application/vnd.google-apps.document";
        }

        public async Task<string> DirectLoginAsync(Guid id, string password)
        {

            string result = null;
            try
            {

                var link = await GetReviewerLink(id);

                var reviewer = await GetReviewerAsync(id);

                if (!currentUser.Id.HasValue)
                {
                    return null;
                }

                if (reviewer.ReviewerType == Common.Module.Constants.FileReviewerType.User)
                {
                    if (Guid.TryParse(reviewer.ReviewerKey, out Guid userId))
                    {
                        if (userId != currentUser.Id)
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }

                    result = link?.WebUrl;
                }

                if (reviewer.ReviewerType == Common.Module.Constants.FileReviewerType.Role)
                {
                    if (Guid.TryParse(reviewer.ReviewerKey, out Guid roleId))
                    {
                        var role = await this.identityRoleManager.GetByIdAsync(roleId);
                        if (!currentUser.IsInRole(role.Name))
                        {
                            return null;
                        }

                        result = link?.WebUrl;
                    }
                    else
                    {
                        return null;
                    }
                }

                if (reviewer.ReviewerType == Common.Module.Constants.FileReviewerType.OrganizationUnit)
                {
                    if (Guid.TryParse(reviewer.ReviewerKey, out Guid orgUnitId))
                    {
                        var user = await identityUserManager.GetByIdAsync(currentUser.Id.Value);
                        var orgUnits = await identityUserManager.GetOrganizationUnitsAsync(user);

                        if (!orgUnits.Any(o => o.Id == orgUnitId))
                        {
                            return null;
                        }
                    }

                    result = link?.WebUrl;
                }
            }
            catch (Exception e)
            {
                logger.Capture(e);
            }
            finally
            {
            }

            return result;
        }
    }
}
