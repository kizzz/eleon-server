using Common.Module.Helpers;
using Logging.Module;
using GatewayManagement.Module.Proxies;
using System.Security.Cryptography.X509Certificates;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;
using Volo.Abp.Http.Client.DynamicProxying;
using Volo.Abp.Uow;
using VPortal.GatewayClient.Domain.Shared.Auth;

namespace VPortal.GatewayClient.Domain.Auth
{
    [UnitOfWork]
    public class GatewayAuthDomainService : DomainService, ISingletonDependency
    {
        private const string GatewayClientCertificateName = "VPortalGatewayClientCertificate";
        private readonly IVportalLogger<GatewayAuthDomainService> logger;
        private readonly IGatewayClientAppService gatewayAppService;
        private readonly MachineSecretsProvider machineApiKeyProvider;

        public GatewayAuthDomainService(
            IVportalLogger<GatewayAuthDomainService> logger,
            IHttpClientProxy<IGatewayClientAppService> gatewayAppService,
            MachineSecretsProvider machineApiKeyProvider)
        {
            this.logger = logger;
            this.gatewayAppService = gatewayAppService.Service;
            this.machineApiKeyProvider = machineApiKeyProvider;
        }

        public async Task<GatewayDto> GetGatewayInfo()
        {
            GatewayDto result = null;
            try
            {
                result = await gatewayAppService.GetCurrentGateway();
            }
            catch (Exception ex)
            {
                logger.Capture(ex);
            }

            return result;
        }

        public async Task<bool> ResetRegistration()
        {
            bool result = false;
            try
            {
                LicenseHelper.License.CurrentRegistrationStage = GatewayClientRegistrationStages.NotRegistered;
                LicenseHelper.License.ClientCompoundKey = null;
                LicenseHelper.Save();
                result = true;
            }
            catch (Exception ex)
            {
                logger.Capture(ex);
            }

            return result;
        }

        public async IAsyncEnumerable<string?> EnsureRegistered(string registrationKey)
        {
            while (true)
            {
                string currentRegistrationStage = await GetCurrentRegistrationStage();
                yield return currentRegistrationStage;
                if (currentRegistrationStage == GatewayClientRegistrationStages.Completed)
                {
                    break;
                }

                try
                {
                    await ProcessRegistrationStage(registrationKey, currentRegistrationStage);

                }
                catch (Exception ex)
                {
                    throw new UserFriendlyException($"En error occured on {currentRegistrationStage} stage of registration: {ex.Message}");
                }
            }

        }

        private async Task ProcessRegistrationStage(string registrationKey, string currentRegistrationStage)
        {
            
            try
            {
                switch (currentRegistrationStage)
                {
                    case GatewayClientRegistrationStages.NotRegistered:
                        await Register(registrationKey);
                        break;
                    case GatewayClientRegistrationStages.Registered:
                    case GatewayClientRegistrationStages.Pending:
                        await ConfirmRegistration();
                        break;
                    case GatewayClientRegistrationStages.Confirmed: // No additional actions on 'Confirmed' as for now
                    case GatewayClientRegistrationStages.Completed:
                        break;
                    default:
                        throw new Exception("Failed to read current registration stage or an unknown stage is set.");
                }
            }
            catch (Exception ex)
            {
                logger.Capture(ex);
                throw;
            }

        }

        private async Task Register(string registrationKey)
        {

            string rawMachineKey = await machineApiKeyProvider.GetMachineKey();
            string encryptedMachineKey = EncryptionHelper.Encrypt(rawMachineKey);
            var cert = await CreateCertificate(rawMachineKey);
            string certBase64 = CertificateHelper.GetCertificateBase64(cert, rawMachineKey);
            var registrationResult = await gatewayAppService.RegisterGateway(new RegisterGatewayRequestDto()
            {
                MachineKey = encryptedMachineKey,
                RegistrationKey = registrationKey,
                CertificateBase64 = certBase64,
            });

            string compoundKey = EncryptionHelper.Decrypt(registrationResult.ClientKey);
            LicenseHelper.License.ClientCompoundKey = compoundKey;
            LicenseHelper.License.Certificate = CertificateHelper.GetCertificateBase64(cert, rawMachineKey);
            LicenseHelper.Save();

            var nextStage =
                registrationResult.Status == Common.Module.Constants.GatewayStatus.WaitingForConfirmation
                ? GatewayClientRegistrationStages.Registered
                : GatewayClientRegistrationStages.Pending;

            await SetCurrentRegistrationStage(nextStage);

        }

        public async Task<string> GetCurrentRegistrationStage()
        {
            string stage = null;
            try
            {
                if (LicenseHelper.License.CurrentRegistrationStage == null)
                {
                    await SetCurrentRegistrationStage(GatewayClientRegistrationStages.NotRegistered);
                }

                stage = LicenseHelper.License.CurrentRegistrationStage;
            }
            catch (Exception ex)
            {
                logger.CaptureAndSuppress(ex);
            }

            return stage;
        }

        private async Task ConfirmRegistration()
        {

            bool confirmed = await gatewayAppService.ConfirmGatewayRegistration();
            if (!confirmed)
            {
                throw new Exception("Unable to confirm gateway registration");
            }

            // Uncomment if you need to perform some additional steps after Confirmed but before Completed
            // await SetCurrentRegistrationStage(GatewayClientRegistrationStages.Confirmed);
            await SetCurrentRegistrationStage(GatewayClientRegistrationStages.Completed);

        }

        private async Task<X509Certificate2> CreateCertificate(string machineKey)
        {
            var certificate = CertificateHelper.GenerateSelfSignedCertificate("VPortalGatewayClient", "VPortalGatewayClient", machineKey); //CertificateHelper.GenerateSelfSignedCertificate(GatewayClientCertificateName, GatewayClientCertificateName, machineKey);
            //CertificateHelper.SaveCertificateToLocalMachineRoot(certificate);
            return certificate;
        }

        private async Task SetCurrentRegistrationStage(string newStage)
        {
            LicenseHelper.License.CurrentRegistrationStage = newStage;
            LicenseHelper.Save();
        }
    }
}
