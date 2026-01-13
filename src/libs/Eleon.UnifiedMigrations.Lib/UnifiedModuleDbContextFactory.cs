using EleonsoftSdk.modules.Migration.Module;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using ModuleCollector.UnifiedMigrations;
using SharedModule.modules.AppSettings.Module;
using System.Reflection;

namespace VPortal.Unified.Module.EntityFrameworkCore
{
    public class UnifiedModuleDbContextFactory : DefaultDbContextFactoryBase<UnifiedDbContext>
    {
        public static DbContext CreateLite()
        {
            return new UnifiedDbContext(CreateOptions(), Array.Empty<DbContext>());
        }

        protected override UnifiedDbContext CreateDbContext(
            DbContextOptions<UnifiedDbContext> dbContextOptions)
        {
            var sourceDbContexts = CollectRequiredAssemblies()
                .SelectMany(assembly =>
                {
                    try
                    {
                        return assembly.GetTypes();
                    }
                    catch (ReflectionTypeLoadException)
                    {
                        // Some assemblies may fail to load types
                        return Array.Empty<Type>();
                    }
                })
                .Where(type => typeof(DbContext).IsAssignableFrom(type) 
                    && !type.IsAbstract
                    && type != typeof(UnifiedDbContext) && type != typeof(DbContext))
                .ToArray();

            Console.WriteLine($"Found {sourceDbContexts.Length} DbContext types:\n  {string.Join("\n  ", sourceDbContexts.Select(x => x.Name))}");

            var instances = sourceDbContexts.Select(CreateCustomDbContext).ToList();

            return new UnifiedDbContext(dbContextOptions, instances);
        }

        private static DbContext CreateCustomDbContext(Type dbContextType)
        {
            try
            {
                Console.WriteLine($"Creating {dbContextType.Name}");

                // Create the specific factory type for TDbContext
                var factoryType = typeof(DefaultDbContextFactoryBase<>).MakeGenericType(dbContextType);

                // Get the CreateOptions static method from the factory
                var createOptionsMethod = factoryType.GetMethod(nameof(CreateOptions), BindingFlags.Public | BindingFlags.Static);

                if (createOptionsMethod == null)
                {
                    throw new InvalidOperationException($"Failed to find CreateOptions method for {dbContextType.Name}");
                }

                // Invoke CreateOptions to get DbContextOptions<TDbContext>
                var options = createOptionsMethod.Invoke(null, null);

                if (options == null)
                {
                    throw new InvalidOperationException($"Failed to create options for {dbContextType.Name}");
                }

                // Create instance using the options
                if (Activator.CreateInstance(dbContextType, options) is DbContext instance)
                {
                    return instance;
                }
                else
                {
                    throw new InvalidOperationException($"Failed to create an instance of {dbContextType.Name}");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create DbContext of type {dbContextType.Name}: {ex.Message}", ex);
            }
        }

        private static List<Assembly> CollectRequiredAssemblies()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !a.ReflectionOnly)
                .Where(a => (a.GetName().Name != null && !a.GetName().Name.StartsWith("System.") && !a.GetName().Name.StartsWith("Microsoft.") && !a.GetName().Name.StartsWith("Volo.Abp.") && a.GetName().Name != "ef"))
                .ToList();

            var addAssemblies = new List<Assembly>();

            var assembly = assemblies.First();
            Console.WriteLine($"Loaded Assembly: {assembly.GetName().Name}");

            var refs = assembly.GetReferencedAssemblies() // todo references recurcivelly
                .Where(r => r.Name != null && !r.Name.StartsWith("System") && !r.Name.StartsWith("Microsoft.") && !r.Name.StartsWith("Volo.Abp.") && r.Name != "ef")
                .ToList();

            refs.ForEach(r => Console.WriteLine($"  References: {r.Name}"));

            foreach (var reference in refs)
            {
                try
                {
                    var loadedAssembly = assemblies.FirstOrDefault(a => a.GetName().Name == reference.Name);
                    if (loadedAssembly == null)
                    {
                        addAssemblies.Add(Assembly.Load(reference));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"    Failed to load referenced assembly {reference.Name}: {ex.Message}");
                }
            }

            assemblies.AddRange(addAssemblies);

            Console.WriteLine($"Scanning {assemblies.Count} assemblies for DbContext types:\n{string.Join("\n", assemblies.Select(a => a.GetName().Name))}");

            return assemblies;
        }
    }
}
