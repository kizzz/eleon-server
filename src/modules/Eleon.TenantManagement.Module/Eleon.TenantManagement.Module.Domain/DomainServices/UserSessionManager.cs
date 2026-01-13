using Logging.Module;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;
using VPortal.Identity.Module.Entities;
using VPortal.Identity.Module.Repositories;

namespace VPortal.Identity.Module.Sessions
{
    
    public class UserSessionManager : DomainService, ITransientDependency
    {
        private readonly IVportalLogger<UserSessionManager> logger;
        private readonly IUserSessionStateRepository repository;

        public bool RequirePeriodicPasswordChange { get; set; }
        public bool PermissionErrorEncountered { get; set; }

        public UserSessionManager(
            IVportalLogger<UserSessionManager> logger,
            IUserSessionStateRepository repository)
        {
            this.logger = logger;
            this.repository = repository;
        }

        public async Task SetSessionState(
            Guid userId,
            bool requirePeriodicPasswordChange,
            bool permissionErrorEncountered)
        {
            try
            {
                var session = await repository.GetByUser(userId);
                if (session == null)
                {
                    session = new Entities.UserSessionStateEntity(GuidGenerator.Create(), userId);
                    await repository.InsertAsync(session, true);
                }

                session.RequirePeriodicPasswordChange = requirePeriodicPasswordChange;
                session.PermissionErrorEncountered = permissionErrorEncountered;

                await repository.UpdateAsync(session, true);
            }
            catch (Exception ex)
            {
                logger.Capture(ex);
            }

        }

        public async Task<UserSessionStateEntity> GetSessionState(Guid userId)
        {
            UserSessionStateEntity result = null;
            try
            {
                var session = await repository.GetByUser(userId);
                if (session == null)
                {
                    session = new Entities.UserSessionStateEntity(GuidGenerator.Create(), userId);
                    await repository.InsertAsync(session, true);
                }

                result = session;
            }
            catch (Exception ex)
            {
                logger.Capture(ex);
            }

            return result;
        }
    }
}
