
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.BlobStoring;
using Volo.Abp.BlobStoring.FileSystem;
using Volo.Abp.Modularity;

namespace EleonCore.Modules.S3;

[DependsOn(
    typeof(EleonS3DomainModule),
    typeof(EleonS3ApplicationContractsModule),
    typeof(AbpBlobStoringModule),
    typeof(AbpBlobStoringFileSystemModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule)
)]
public class EleonS3ApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<EleonS3ApplicationModule>().
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<EleonS3ApplicationModule>(validate: true);
        });
        Configure<AbpBlobStoringOptions>(opt =>
        {
            opt.Containers.Configure("objects", c =>
            {
                // Default to FileSystem if MinIO provider package not present.
                // You can switch to MinIO/S3/Azure per bucket at runtime via ProviderName/ProviderConfigJson.
                c.UseFileSystem(fs => { fs.BasePath = System.IO.Path.Combine(System.AppContext.BaseDirectory, "blobs"); });
            });
        });
    }
}

public class EleonS3ApplicationContractsModule : AbpModule { }
