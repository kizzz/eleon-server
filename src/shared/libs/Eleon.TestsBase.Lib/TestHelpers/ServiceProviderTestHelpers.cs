using System;
using System.Reflection;
using NSubstitute;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;
using Volo.Abp.Timing;
using Volo.Abp.Guids;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Users;

namespace Eleon.TestsBase.Lib.TestHelpers;

/// <summary>
/// Helpers for setting up service providers and lazy service providers for testing.
/// </summary>
public static class ServiceProviderTestHelpers
{
    /// <summary>
    /// Sets up the LazyServiceProvider for a DomainService using reflection.
    /// </summary>
    /// <param name="service">The DomainService instance.</param>
    /// <param name="guidGenerator">The IGuidGenerator to provide.</param>
    /// <param name="clock">The IClock to provide.</param>
    public static void SetLazyServiceProvider(DomainService service, IGuidGenerator guidGenerator, IClock clock)
    {
        var lazyServiceProvider = Substitute.For<IAbpLazyServiceProvider>();
        lazyServiceProvider.LazyGetRequiredService<IGuidGenerator>().Returns(guidGenerator);
        lazyServiceProvider.LazyGetRequiredService<IClock>().Returns(clock);

        var lazyServiceProviderProp = typeof(DomainService).GetProperty(
            "LazyServiceProvider", 
            BindingFlags.Public | BindingFlags.Instance);
        lazyServiceProviderProp?.SetValue(service, lazyServiceProvider);
    }

    /// <summary>
    /// Sets up the LazyServiceProvider and related properties for an ApplicationService using reflection.
    /// </summary>
    /// <param name="service">The ApplicationService instance (any object that needs app service dependencies).</param>
    /// <param name="objectMapper">The IObjectMapper to provide.</param>
    /// <param name="currentUser">The ICurrentUser to provide.</param>
    /// <param name="settingProvider">Optional ISettingProvider to provide.</param>
    public static void SetAppServiceDependencies(
        object service, 
        Volo.Abp.ObjectMapping.IObjectMapper objectMapper, 
        Volo.Abp.Users.ICurrentUser currentUser,
        Volo.Abp.Settings.ISettingProvider settingProvider = null)
    {
        var lazyServiceProvider = Substitute.For<IAbpLazyServiceProvider>();
        lazyServiceProvider.LazyGetRequiredService<Volo.Abp.ObjectMapping.IObjectMapper>().Returns(objectMapper);
        lazyServiceProvider.LazyGetRequiredService<Volo.Abp.Users.ICurrentUser>().Returns(currentUser);
        lazyServiceProvider.LazyGetService<Volo.Abp.ObjectMapping.IObjectMapper>().Returns(objectMapper);
        lazyServiceProvider.LazyGetService<Volo.Abp.Users.ICurrentUser>().Returns(currentUser);
        lazyServiceProvider.LazyGetService<Volo.Abp.ObjectMapping.IObjectMapper>(Arg.Any<Func<IServiceProvider, object>>())
            .Returns(callInfo =>
            {
                var factory = callInfo.Arg<Func<IServiceProvider, object>>();
                return (Volo.Abp.ObjectMapping.IObjectMapper)factory(new TestServiceProvider(objectMapper, currentUser, settingProvider));
            });
        if (settingProvider != null)
        {
            lazyServiceProvider.LazyGetRequiredService<Volo.Abp.Settings.ISettingProvider>().Returns(settingProvider);
            lazyServiceProvider.LazyGetService<Volo.Abp.Settings.ISettingProvider>().Returns(settingProvider);
        }

        SetPropertyOnTypeHierarchy(service, "LazyServiceProvider", lazyServiceProvider);
        SetPropertyOnTypeHierarchy(service, "ObjectMapper", objectMapper);
        SetPropertyOnTypeHierarchy(service, "CurrentUser", currentUser);
        SetPropertyOnTypeHierarchy(service, "ServiceProvider", new TestServiceProvider(objectMapper, currentUser, settingProvider));
        SetFieldByType(service, typeof(IAbpLazyServiceProvider), lazyServiceProvider);
        SetFieldByType(service, typeof(Volo.Abp.ObjectMapping.IObjectMapper), objectMapper);
        SetFieldByType(service, typeof(Volo.Abp.Users.ICurrentUser), currentUser);
        if (settingProvider != null)
        {
            SetPropertyOnTypeHierarchy(service, "SettingProvider", settingProvider);
            SetFieldByType(service, typeof(Volo.Abp.Settings.ISettingProvider), settingProvider);
        }
        SetFieldByType(service, typeof(IServiceProvider), new TestServiceProvider(objectMapper, currentUser, settingProvider));
    }

    private static void SetPropertyOnTypeHierarchy(object target, string propertyName, object value)
    {
        var type = target.GetType();
        while (type != null)
        {
            var prop = type.GetProperty(
                propertyName, 
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (prop != null && prop.CanWrite)
            {
                prop.SetValue(target, value);
                return;
            }
            
            var fieldName = $"_{char.ToLowerInvariant(propertyName[0])}{propertyName.Substring(1)}";
            var field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)
                       ?? type.GetField(propertyName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(target, value);
                return;
            }
            type = type.BaseType;
        }
    }

    private static void SetFieldByType(object target, Type fieldType, object value)
    {
        var type = target.GetType();
        while (type != null)
        {
            var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (fieldType.IsAssignableFrom(field.FieldType)
                    && (value == null || field.FieldType.IsAssignableFrom(value.GetType())))
                {
                    field.SetValue(target, value);
                    return;
                }
            }
            type = type.BaseType;
        }
    }

    private sealed class TestServiceProvider : IServiceProvider
    {
        private readonly Volo.Abp.ObjectMapping.IObjectMapper _objectMapper;
        private readonly Volo.Abp.Users.ICurrentUser _currentUser;
        private readonly Volo.Abp.Settings.ISettingProvider _settingProvider;

        public TestServiceProvider(
            Volo.Abp.ObjectMapping.IObjectMapper objectMapper, 
            Volo.Abp.Users.ICurrentUser currentUser,
            Volo.Abp.Settings.ISettingProvider settingProvider)
        {
            _objectMapper = objectMapper;
            _currentUser = currentUser;
            _settingProvider = settingProvider;
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(Volo.Abp.ObjectMapping.IObjectMapper))
            {
                return _objectMapper;
            }

            if (serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof(Volo.Abp.ObjectMapping.IObjectMapper<>))
            {
                return _objectMapper;
            }

            if (serviceType == typeof(Volo.Abp.Users.ICurrentUser))
            {
                return _currentUser;
            }

            if (serviceType == typeof(Volo.Abp.Settings.ISettingProvider))
            {
                return _settingProvider;
            }

            return null;
        }
    }
}
