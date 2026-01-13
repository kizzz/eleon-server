//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.DependencyInjection.Extensions;
//using System;
//using Volo.Abp.Auditing;

//namespace VPortal.Auditor.Module.RemoteStore
//{
//    public static class RemoteStoreServiceExtensions
//    {
//        public static IServiceCollection AddRemoteAbpAuditingStore(this IServiceCollection services)
//        {
//            services.Replace(new ServiceDescriptor(typeof(IAuditingStore), typeof(RemoteAbpAuditingStore), ServiceLifetime.Singleton));
//            return services;
//        }
//    }
//}
