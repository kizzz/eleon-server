// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PingControllerTests.cs" company="Eleon">
// Licensed under the MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Eleon.HttpApi.Tests
{
    using System.Net;
    using System.Threading.Tasks;
    using Eleon.HttpApi.Tests.Fixtures;
    using FluentAssertions;
    using Xunit;

    public sealed class PingControllerTests : IClassFixture<HostWebAppFactory>
    {
        private readonly HostWebAppFactory factory;

        public PingControllerTests(HostWebAppFactory factory)
        {
            this.factory = factory;
        }

        [Fact]
        public async Task Ping_returns_ok()
        {
            var client = factory.CreateClient();
            var response = await client.GetAsync("/api/ping");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}