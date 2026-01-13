using Common.Module;
using Common.Module.Constants;
using Eleon.Common.Lib.UserToken;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.IO;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Modularity;
using Volo.Abp.Reflection;

namespace Common.EventBus.Module
{
    [DependsOn(typeof(AbpEventBusModule))]
    public class CommonEventBusModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            AddEventHandlers(context.Services);
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddCurrentUserToken();

      context.Services.AddTransient<CurrentEvent>();
      context.Services.AddHostedService<EventBusConfigurationInitializationService>();

      var configuration = context.Services.GetConfiguration();
      var eventBusOptions = ResolveEventBusOptions(configuration, AppContext.BaseDirectory);

      Configure<EventBusOptions>(options =>
      {
        options.Provider = eventBusOptions.Provider;
        options.ProviderOptionsJson = eventBusOptions.ProviderOptionsJson;
      });
    }

        private static void AddEventHandlers(IServiceCollection services)
        {
            services.Configure<AbpDistributedEventBusOptions>(options =>
            {
                var handlers = services.Where(IsHandler).Select(x => x.ImplementationType);
                options.Handlers.AddIfNotContains(handlers);
            });
        }

    private static bool IsHandler(ServiceDescriptor s)
        => s.ImplementationType != null && ReflectionHelper.IsAssignableToGenericType(s.ImplementationType, typeof(IDistributedEventHandler<>));

    private static EventBusOptions ResolveEventBusOptions(IConfiguration configuration, string contentRootPath)
    {
      if (TryGetConfiguredEventBusOptions(configuration, out var options))
      {
        return options;
      }

      if (TryGetEventBusOptionsFromFile(contentRootPath, out options))
      {
        return options;
      }

      throw new InvalidOperationException("EventBus configuration is missing. Ensure EventBus:Provider and EventBus:ProviderOptionsJson are defined.");
    }

    private static bool TryGetConfiguredEventBusOptions(IConfiguration configuration, out EventBusOptions options)
    {
      options = default!;
      if (configuration is null)
      {
        return false;
      }

      var section = configuration.GetSection("EventBus");
      if (!section.Exists())
      {
        return false;
      }

      var providerValue = section.GetValue<int?>("Provider");
      if (!providerValue.HasValue)
      {
        return false;
      }

      var provider = Enum.IsDefined(typeof(EventBusProvider), providerValue.Value)
          ? (EventBusProvider)providerValue.Value
          : EventBusProvider.Undefined;

      if (provider == EventBusProvider.Undefined)
      {
        return false;
      }

      var providerOptionsJson = ConfigurationSerializer.SerializeConfigurationSection(section.GetSection("ProviderOptionsJson"));
      if (string.IsNullOrWhiteSpace(providerOptionsJson))
      {
        return false;
      }

      options = new EventBusOptions
      {
        Provider = provider,
        ProviderOptionsJson = providerOptionsJson
      };

      return true;
    }

    private static bool TryGetEventBusOptionsFromFile(string contentRootPath, out EventBusOptions options)
    {
      options = default!;
      var file = LocateEventBusFile(contentRootPath);
      if (string.IsNullOrWhiteSpace(file))
      {
        return false;
      }

      var builder = new ConfigurationBuilder()
          .SetBasePath(Path.GetDirectoryName(file) ?? string.Empty)
          .AddJsonFile(Path.GetFileName(file), optional: false, reloadOnChange: false);

      var fallbackConfiguration = builder.Build();
      return TryGetConfiguredEventBusOptions(fallbackConfiguration, out options);
    }

    private static string? LocateEventBusFile(string? contentRootPath)
    {
      if (string.IsNullOrWhiteSpace(contentRootPath))
      {
        return null;
      }

      var current = Path.GetFullPath(contentRootPath);
      while (!string.IsNullOrEmpty(current))
      {
        var candidate = Path.Combine(current, "appsettings", "event-bus.json");
        if (File.Exists(candidate))
        {
          return candidate;
        }

        var parent = Path.GetDirectoryName(current);
        if (string.IsNullOrWhiteSpace(parent) || parent == current)
        {
          break;
        }

        current = parent;
      }

      return null;
    }
  }
  public static class ConfigurationSerializer
  {
    public static string SerializeConfigurationSection(IConfigurationSection section)
    {
      var configObject = GetSectionAsObject(section);
      return JsonConvert.SerializeObject(configObject, Formatting.Indented);
    }

        private static object GetSectionAsObject(IConfigurationSection section)
        {
            var children = section.GetChildren().ToList();
            if (children.Count == 0)
            {
                // If there are no children, return the value directly
                return section.Value;
            }
            else
            {
                // Check if the children represent an array
                if (IsArray(children))
                {
                    var list = new List<object>();
                    foreach (var child in children.OrderBy(c => int.Parse(c.Key)))
                    {
                        list.Add(GetSectionAsObject(child));
                    }
                    return list;
                }
                else
                {
                    // Otherwise, create a dictionary to hold the nested values
                    var dict = new Dictionary<string, object>();
                    foreach (var child in children)
                    {
                        dict[child.Key] = GetSectionAsObject(child);
                    }
                    return dict;
                }
            }
        }

        private static bool IsArray(List<IConfigurationSection> children)
        {
            // Check if all keys are numeric and sequential starting from 0
            var indexes = new List<int>();
            foreach (var child in children)
            {
                if (int.TryParse(child.Key, out int index))
                {
                    indexes.Add(index);
                }
                else
                {
                    return false;
                }
            }

            indexes.Sort();
            for (int i = 0; i < indexes.Count; i++)
            {
                if (indexes[i] != i)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
