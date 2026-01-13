using System;
using System.Reflection;
using Eleon.TestsBase.Lib.TestHelpers;
using NSubstitute;
using Volo.Abp.Application.Services;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Settings;
using Volo.Abp.Users;

namespace Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.TestBase;

public abstract class AppServiceTestBase : DomainTestBase
{
    protected static ISettingProvider CreateTestSettingProvider(string value = "true")
    {
        var settingProvider = Substitute.For<ISettingProvider>();
        settingProvider.GetOrNullAsync(Arg.Any<string>())
            .Returns(callInfo => System.Threading.Tasks.Task.FromResult(value));
        return settingProvider;
    }

    protected static void SetAppServiceDependencies(
        ApplicationService service,
        IObjectMapper objectMapper,
        ICurrentUser currentUser,
        ISettingProvider settingProvider = null)
    {
        ServiceProviderTestHelpers.SetAppServiceDependencies(service, objectMapper, currentUser, settingProvider);
        if (settingProvider != null)
        {
            SetPropertyOnTypeHierarchy(service, "SettingProvider", settingProvider);
            SetFieldByType(service, typeof(ISettingProvider), settingProvider);
        }
    }

    private static void SetPropertyOnTypeHierarchy(object target, string propertyName, object value)
    {
        var type = target.GetType();
        while (type != null)
        {
            var prop = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
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
}
