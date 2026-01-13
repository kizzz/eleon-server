using Common.Module.Helpers;
using Logging.Module;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;
using VPortal.ProxyClient.Domain.Shared.Storage;

namespace VPortal.ProxyClient.Domain.Storage
{
    [UnitOfWork]
    public class EncryptedStorageDomainService : DomainService
    {
        private readonly IVportalLogger<EncryptedStorageDomainService> logger;
        private readonly ILocalStorageManager storageManager;
        private readonly XmlSerializerHelper _xmlSerializerHelper;

        public delegate string FileContentValueSelector<TFileContent>(TFileContent fileContent);
        public delegate void FileContentValueAssigner<TFileContent>(TFileContent fileContent, string encryptedValue);

        public EncryptedStorageDomainService(
            IVportalLogger<EncryptedStorageDomainService> logger,
            ILocalStorageManager storageManager,
            XmlSerializerHelper xmlSerializerHelper)
        {
            this.logger = logger;
            this.storageManager = storageManager;
            _xmlSerializerHelper = xmlSerializerHelper;
        }

        public async Task<string?> ReadEncryptedValue<TFileContent>(string filename, FileContentValueSelector<TFileContent> selector)
        {
            string? value = null;
            try
            {
                var fileContent = await storageManager.ReadFile(filename);
                if (fileContent == null)
                {
                    throw new Exception($"Unable to read requested file {filename}");
                }

                var parsedFile = _xmlSerializerHelper.DeserializeFromXml<TFileContent>(fileContent);
                var encryptedValue = selector(parsedFile);
                string decryptedValue = EncryptionHelper.Decrypt(encryptedValue);
                value = decryptedValue;
            }
            catch (Exception ex)
            {
                logger.Capture(ex);
            }

            return value;
        }

        public async Task<bool> WriteEncryptedValue<TFileContent>(string filename, string unencryptedValue, FileContentValueAssigner<TFileContent> valueAssigner)
            where TFileContent : new()
        {
            bool success = false;
            try
            {
                var rawFileContent = await storageManager.ReadFile(filename);
                TFileContent parsedContent;
                if (rawFileContent != null)
                {
                    parsedContent = _xmlSerializerHelper.DeserializeFromXml<TFileContent>(rawFileContent);
                }
                else
                {
                    parsedContent = new TFileContent();
                }

                string encryptedValue = EncryptionHelper.Encrypt(unencryptedValue);
                valueAssigner(parsedContent, encryptedValue);

                string modifiedRawFileContent = _xmlSerializerHelper.SerializeToXml(parsedContent);
                await storageManager.WriteFile(filename, modifiedRawFileContent);
                success = true;
            }
            catch (Exception ex)
            {
                logger.Capture(ex);
            }

            return success;
        }
    }
}
