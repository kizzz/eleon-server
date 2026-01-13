
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace EleonCore.Modules.S3.EntityFrameworkCore;

[ConnectionStringName("Default")]
public class S3DbContext : AbpDbContext<S3DbContext>
{

    public S3DbContext(DbContextOptions<S3DbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);
    }
}
