using Common.EventBus.Module.Options;
using Logging.Module;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VPortal.Infrastructure.Module.Domain.CliBinding;

namespace VPortal.GatewayClient.Host.EventBus
{
    public class NatsRuntimeManager : ISingletonDependency
    {
        private const string NatsRuntimePath = @"nats-server.exe";
        private readonly IVportalLogger<NatsRuntimeManager> logger;
        CancellationTokenSource cts = new ();
        Task? process = null;

        public NatsRuntimeManager(IVportalLogger<NatsRuntimeManager> logger)
        {
            this.logger = logger;
        }

        public async Task StartNatsRuntime(NatsOptions options)
        {
            if (!File.Exists(NatsRuntimePath))
            {
                throw new Exception("NATS runtime not found.");
            }

            var binding = new CliBinding(NatsRuntimePath, $"-a {options.Url} -p {options.Port}");
            process = Task.Run(async () =>
            {
                var bindingTask = binding.RunToCompletion();
                await foreach (var line in binding.ReadLines())
                {
                    if (cts.Token.IsCancellationRequested)
                    {
                        return;
                    }

                    logger.Log.LogInformation($"NATS CLI log: {line}");
                }

                await bindingTask;
            }, cts.Token);
        }

        public async Task StopNatsRuntime()
        {
            cts.Cancel();
        }
    }
}
