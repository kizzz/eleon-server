// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HubManifestSmokeTests.cs" company="Eleon">
// Licensed under the MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Eleon.SignalR.SmokeTests
{
    using System;
    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.SignalR.Client;
    using Xunit;

    public class HubManifestSmokeTests
    {
        [Fact(Skip = "Requires running Host at ASPNETCORE_URLS or http://localhost:5000")]
        public async Task ConnectsToAllHubsIfServerRunning()
        {
            var manifestPath = Path.Combine(".agents", "logs", "meta", "signalr.json");
            Assert.True(File.Exists(manifestPath), "signalr.json missing. Build the solution first.");
            var json = await File.ReadAllTextAsync(manifestPath).ConfigureAwait(false);
            var hubs = JsonSerializer.Deserialize<HubEntry[]>(json) ?? Array.Empty<HubEntry>();
            var baseUrl = Environment.GetEnvironmentVariable("E2E_BASE_URL") ?? "http://localhost:5000";

            foreach (var hub in hubs)
            {
                var url = $"{baseUrl}/{hub.Hub}";
                var connection = new HubConnectionBuilder().WithUrl(url).Build();
                await connection.StartAsync().ConfigureAwait(false);
                await connection.StopAsync().ConfigureAwait(false);
            }
        }

        public record HubEntry(string Hub, Method[] Methods);
        public record Method(string Name, Param[] Parameters);
        public record Param(string Name, string Type);
    }
}
