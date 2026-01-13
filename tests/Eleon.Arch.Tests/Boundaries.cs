using System;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using NetArchTest.Rules;
using Xunit;

namespace Eleon.ArchTests
{
    public class Boundaries
    {
        [Fact]
        public void Applications_must_not_reference_Infrastructure()
        {
            var result = Types.InCurrentDomain()
                .That().ResideInNamespace("Eleon.Server.Apps", true)
                .Should().NotHaveDependencyOn("Eleon.Server.Modules.*.Infrastructure")
                .GetResult();

            result.IsSuccessful.Should().BeTrue($"Apps must not reference Infrastructure directly. Failing: {string.Join(", ", result.FailingTypeNames)}");
        }

        [Fact]
        public void Hosts_should_only_reference_Apps_and_Shared()
        {
            var result = Types.InCurrentDomain()
                .That().ResideInNamespace("Eleon.Server.Hosts", true)
                .Should().OnlyHaveDependenciesOn(
                    "System",
                    "Eleon.Server.Apps",
                    "Eleon.Shared",
                    "Eleon.Server.Libs"
                )
                .GetResult();

            result.IsSuccessful.Should().BeTrue($"Hosts must only depend on Apps/Shared/Libs. Failing: {string.Join(", ", result.FailingTypeNames)}");
        }

        [Fact]
        public void Modules_can_depend_on_Libs_and_Shared_only_outside_their_boundary()
        {
            var result = Types.InCurrentDomain()
                .That().ResideInNamespace("Eleon.Server.Modules", true)
                .Should().OnlyHaveDependenciesOn(
                    "System",
                    "Eleon.Server.Modules",
                    "Eleon.Server.Libs",
                    "Eleon.Shared"
                )
                .GetResult();

            result.IsSuccessful.Should().BeTrue($"Modules must limit cross-boundary deps. Failing: {string.Join(", ", result.FailingTypeNames)}");
        }
    }
}