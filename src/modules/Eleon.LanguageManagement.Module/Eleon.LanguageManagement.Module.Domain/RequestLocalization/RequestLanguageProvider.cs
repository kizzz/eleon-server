//using Common.EventBus.Module;
//using Logging.Module;
//using Messaging.Module.Messages;
//using Volo.Abp.DependencyInjection;
//using Volo.Abp.EventBus.Distributed;
//using Volo.Abp.MultiTenancy;

//namespace Authorization.Module.RequestLocalization
//{
//    public class RequestLanguageProvider : ISingletonDependency
//    {
//        private readonly Dictionary<string, LanguageCacheEntry> languageCache = new();
//        private readonly IVportalLogger<RequestLanguageProvider> logger;
//        private readonly ICurrentTenant currentTenant;
//        private readonly IDistributedEventBus requestClient;
//        private readonly SemaphoreSlim cacheSemaphore = new(1);

//        public RequestLanguageProvider(
//            IVportalLogger<RequestLanguageProvider> logger,
//            ICurrentTenant currentTenant,
//            IDistributedEventBus requestClient)
//        {
//            this.logger = logger;
//            this.currentTenant = currentTenant;
//            this.requestClient = requestClient;
//        }

//        public async Task<LanguageCacheEntry> GetTenantLanguage()
//        {
//            LanguageCacheEntry result = null;
//            try
//            {
//                return new LanguageCacheEntry("en", "en");
//                string key = GetCacheKey();
//                if (languageCache.TryGetValue(key, out var cacheValue))
//                {
//                    result = cacheValue;
//                }
//                else
//                {
//                    await cacheSemaphore.WaitAsync();
//                    languageCache[key] = await RetreiveTenantLanguage();
//                    cacheSemaphore.Release();

//                    result = languageCache[key];
//                }
//            }
//            catch (Exception ex)
//            {
//                logger.Capture(ex);
//            }

//            return result;
//        }

//        public async Task SetTenantLanguage(string cultureName, string uiCultureName)
//        {
//            try
//            {
//                await cacheSemaphore.WaitAsync();
//                languageCache[GetCacheKey()] = new LanguageCacheEntry(cultureName, uiCultureName);
//                cacheSemaphore.Release();
//            }
//            catch (Exception ex)
//            {
//                logger.Capture(ex);
//            }

//        }

//        private async Task<LanguageCacheEntry> RetreiveTenantLanguage()
//        {
//            var request = new GetDefaultTenantLanguageMsg(null, null);
//            var response = await requestClient.RequestAsync<DefaultTenantLanguageGotMsg>(request);
//            return new LanguageCacheEntry(response.CultureName, response.UiCultureName);
//        }

//        private string GetCacheKey() => currentTenant.Id?.ToString() ?? "host";
//    }

//    public record LanguageCacheEntry(string CultureName, string UiCultureName);
//}
