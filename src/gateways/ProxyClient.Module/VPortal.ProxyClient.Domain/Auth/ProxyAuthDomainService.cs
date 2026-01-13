using Common.Module.Helpers;
using Logging.Module;
using ProxyManagement.Module.Proxies;
using System.Security.Cryptography.X509Certificates;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;
using Volo.Abp.Http.Client.DynamicProxying;
using Volo.Abp.Uow;
using VPortal.ProxyClient.Domain.Shared.Auth;

namespace VPortal.ProxyClient.Domain.Auth
{
    [UnitOfWork]
    public class ProxyAuthDomainService : DomainService, ISingletonDependency
    {
        private const string ProxyClientCertificateName = "VPortalProxyClientCertificate";
        private readonly IVportalLogger<ProxyAuthDomainService> logger;
        private readonly IProxyClientAppService proxyAppService;
        private readonly MachineSecretsProvider machineApiKeyProvider;

        public ProxyAuthDomainService(
            IVportalLogger<ProxyAuthDomainService> logger,
            IHttpClientProxy<IProxyClientAppService> proxyAppService,
            MachineSecretsProvider machineApiKeyProvider)
        {
            this.logger = logger;
            this.proxyAppService = proxyAppService.Service;
            this.machineApiKeyProvider = machineApiKeyProvider;
        }

        public async Task<ProxyDto> GetProxyInfo()
        {
            ProxyDto result = null;
            try
            {
                result = await proxyAppService.GetProxyByLoggedImpersonation();
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
                LicenseHelper.License.CurrentRegistrationStage = ProxyClientRegistrationStages.NotRegistered;
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
                if (currentRegistrationStage == ProxyClientRegistrationStages.Completed)
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
                    case ProxyClientRegistrationStages.NotRegistered:
                        await Register(registrationKey);
                        break;
                    case ProxyClientRegistrationStages.Registered:
                        await ConfirmRegistration();
                        break;
                    case ProxyClientRegistrationStages.Confirmed: // No additional actions on 'Confirmed' as for now
                    case ProxyClientRegistrationStages.Completed:
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
            string encryptedCompoundKey = await proxyAppService.RegisterProxy(new RegisterProxyRequestDto()
            {
                MachineKey = encryptedMachineKey,
                RegistrationKey = registrationKey,
                CertificateBase64 = certBase64,
            });

            if (encryptedCompoundKey.IsNullOrEmpty())
            {
                throw new Exception("Could not register proxy using the provided registration key.");
            }

            string compoundKey = EncryptionHelper.Decrypt(encryptedCompoundKey);
            LicenseHelper.License.ClientCompoundKey = compoundKey;
            LicenseHelper.License.Certificate = CertificateHelper.GetCertificateBase64(cert, rawMachineKey);
            LicenseHelper.Save();

            await SetCurrentRegistrationStage(ProxyClientRegistrationStages.Registered);

        }

        public async Task<string> GetCurrentRegistrationStage()
        {
            string stage = null;
            try
            {
                if (LicenseHelper.License.CurrentRegistrationStage == null)
                {
                    await SetCurrentRegistrationStage(ProxyClientRegistrationStages.NotRegistered);
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

            bool confirmed = await proxyAppService.ConfirmProxyRegistration();
            if (!confirmed)
            {
                throw new Exception("Unable to confirm proxy registration");
            }

            // Uncomment if you need to perform some additional steps after Confirmed but before Completed
            // await SetCurrentRegistrationStage(ProxyClientRegistrationStages.Confirmed);
            await SetCurrentRegistrationStage(ProxyClientRegistrationStages.Completed);

        }

        private async Task<X509Certificate2> CreateCertificate(string machineKey)
        {
            var certificate = CertificateHelper.GenerateSelfSignedCertificate("VPortalProxyClient", "VPortalProxyClient", machineKey); //CertificateHelper.GenerateSelfSignedCertificate(ProxyClientCertificateName, ProxyClientCertificateName, machineKey);
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
