// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayeringRules.cs" company="Eleon">
// Licensed under the MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Eleon.Architecture.Tests
{
    using System;
    using System.Linq;
    using NetArchTest.Rules;
    using Xunit;

    public class LayeringRules
    {
        [Fact]
        public void Domain_should_not_depend_on_frameworks()
        {
            var result = Types.InCurrentDomain()
                .That().ResideInNamespaceMatching("^Eleon\\.Domain($|\\.)")
                .ShouldNot().HaveDependencyOnAny("Microsoft.EntityFrameworkCore", "Microsoft.AspNetCore")
                .GetResult();

            var failingTypes = result.FailingTypes ?? Enumerable.Empty<Type>();
            Assert.True(result.IsSuccessful, string.Join("\n", failingTypes.Select(t => t.FullName)));
        }
    }
}