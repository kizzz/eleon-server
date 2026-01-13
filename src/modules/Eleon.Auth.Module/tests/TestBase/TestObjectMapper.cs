using System;
using Volo.Abp.Account;
using Volo.Abp.Identity;
using Volo.Abp.ObjectMapping;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.TestBase;

internal sealed class TestObjectMapper : IObjectMapper
{
    public IAutoObjectMappingProvider AutoObjectMappingProvider => null;

    public TDestination Map<TSource, TDestination>(TSource source)
    {
        if (source == null)
        {
            return default;
        }

        if (source is IdentityUser user)
        {
            if (typeof(TDestination) == typeof(ProfileDto))
            {
                return (TDestination)(object)new ProfileDto
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Name = user.Name,
                    Surname = user.Surname
                };
            }

            if (typeof(TDestination) == typeof(IdentityUserDto))
            {
                return (TDestination)(object)new IdentityUserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email
                };
            }
        }

        return default;
    }

    public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
    {
        if (source == null)
        {
            return destination;
        }

        if (source is UpdateProfileDto update && destination is IdentityUser user)
        {
            if (!string.IsNullOrWhiteSpace(update.UserName))
            {
                typeof(IdentityUser).GetProperty("UserName")?.SetValue(user, update.UserName);
            }
            if (!string.IsNullOrWhiteSpace(update.Email))
            {
                typeof(IdentityUser).GetProperty("Email")?.SetValue(user, update.Email);
            }
            user.Name = update.Name;
            user.Surname = update.Surname;
            return destination;
        }

        if (source is IdentityUser identityUser && destination is IdentityUserDto identityUserDto)
        {
            identityUserDto.Id = identityUser.Id;
            identityUserDto.UserName = identityUser.UserName;
            identityUserDto.Email = identityUser.Email;
            return destination;
        }

        if (source is IdentityUser identityUserProfile && destination is ProfileDto profileDto)
        {
            profileDto.UserName = identityUserProfile.UserName;
            profileDto.Email = identityUserProfile.Email;
            profileDto.Name = identityUserProfile.Name;
            profileDto.Surname = identityUserProfile.Surname;
            return destination;
        }

        return destination;
    }
}
