using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore;

namespace ModuleCollector.UnifiedMigrations
{
    public class UnifiedDbContext : AbpDbContext<UnifiedDbContext>, IEfCoreDbContext
    {
        private readonly List<DbContext> _sourceDbContexts;

        public UnifiedDbContext(DbContextOptions<UnifiedDbContext> options) : base(options)
        {
            _sourceDbContexts = new List<DbContext>();
        }
        public UnifiedDbContext(DbContextOptions<UnifiedDbContext> options, IEnumerable<DbContext> sourceDbContexts)
            : base(options)
        {
            if (sourceDbContexts == null || sourceDbContexts.Count() == 0)
            {
                _sourceDbContexts = new List<DbContext>();
            }
            else
                _sourceDbContexts = sourceDbContexts.ToList();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            Console.WriteLine($"UnifiedDbContext OnModelCreating called for DbContexts: ({_sourceDbContexts.Count}): {string.Join(", ", _sourceDbContexts.Select(c => c.GetType().Name))}");

            foreach (var contextInstance in _sourceDbContexts)
            {
                Console.WriteLine(contextInstance);
                // Call OnModelCreating of each DbContext instance
                var methodInfo = contextInstance.GetType().GetMethod("OnModelCreating", BindingFlags.NonPublic | BindingFlags.Instance);
                if (methodInfo != null)
                {
                    methodInfo.Invoke(contextInstance, new object[] { modelBuilder });
                }

                // Fetch and configure DbSets
                var dbSetProperties = contextInstance.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.PropertyType.IsGenericType &&
                                p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

                foreach (var dbSetProperty in dbSetProperties)
                {
                    var entityType = dbSetProperty.PropertyType.GetGenericArguments()[0];
                    Console.WriteLine(entityType);
                    if (entityType.Namespace != null && entityType.Namespace.StartsWith("Volo.Abp"))
                    {
                        continue; // Skip ABP entities
                    }

                    //var entityTypeBuilder = modelBuilder.Entity(entityType, (x) =>
                    //{
                    //    x.ConfigureByConvention();
                    //    x.ToTable(dbSetProperty.Name);
                    //});


                }
            }

        }
        private bool HasPrefix(IMutableEntityType entity, string prefix)
        {
            var tableName = entity.GetTableName();
            return tableName != null && tableName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
        }
    }
}
