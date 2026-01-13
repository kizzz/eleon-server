// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Layering.cs" company="Eleon">
// Licensed under the MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------



using System;
using System.Linq;
using NetArchTest.Rules;
using Xunit;

namespace Eleon.Arch.Tests;
public class Layering
{
    [Fact]
    public void Domain_should_not_depend_on_infrastructure_or_http()
    {
        var result = Types.InCurrentDomain()
            .That().ResideInNamespaceStartingWith("Eleon.Domain")
            .Should().NotHaveDependencyOnAny("Eleon.HttpApi", "Eleon.Persistence.EfCore")
            .GetResult();

        var failingTypes = result.FailingTypes ?? Enumerable.Empty<Type>();
        Assert.True(result.IsSuccessful, string.Join("\n", failingTypes.Select(t => t.FullName)));
    }
}