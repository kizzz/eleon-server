using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MassTransit.Logging.DiagnosticHeaders.Messaging;

namespace EleonsoftSdk.modules.EventBus.EventBus.LoopDetector;


// Loop Guard Configuration

// Configure<LoopGuardOptions>(options =>
//{
//    options.MaxHops = 4;
//    options.MaxChainEntries = 16;
//    options.DefaultTimeBudget = TimeSpan.FromSeconds(30);
//    options.TrackOnPublish = false;
//    options.ServiceId = configuration["ApplicationName"] ?? "Service"; // set per microservice
//});

//cfg.UsingRabbitMq((context, rabbitMq) =>
//            {
//                // var opt = context.GetRequiredService<LoopGuardOptions>();

//                //rabbitMq.UseConsumeFilter(typeof(LoopGuardConsumeFilter<>), context);
//                //rabbitMq.UseSendFilter(typeof(LoopGuardSendFilter<>), context);
//                //rabbitMq.UsePublishFilter(typeof(LoopGuardPublishFilter<>), context); // optional if TrackOnPublish=false

//                // Hardening add-ons (recommended):
//                rabbitMq.UseCircuitBreaker(cb =>
//                {
//                    cb.TrackingPeriod = TimeSpan.FromMinutes(1);
//                    cb.TripThreshold = 15;   // trip if 15% faults
//                    cb.ActiveThreshold = 20;   // after 20 messages
//                    cb.ResetInterval = TimeSpan.FromMinutes(5);
//                });

//rabbitMq.UseMessageRetry(r =>
//{
//    // Respect overall deadline to avoid retrying past the budget
//    r.Handle<Exception>();
//    r.Exponential(3, TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(2), TimeSpan.FromMilliseconds(200));
//});

//rabbitMq.UseKillSwitch(options =>
//{
//    options.ActivationThreshold = 10;
//    options.TripThreshold = 15;
//    options.RestartTimeout = TimeSpan.FromMinutes(1);
//});

//// If you use an outbox, enable it to avoid dupes fueling loops
//rabbitMq.UseInMemoryOutbox();
//}
