using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using EleonsoftProxy.Api;
using EleonsoftProxy.Model;
using System.Text.Json;
using SharedModule.modules.Logging.Module.SystemLog;

namespace Eleoncore.SDK.CoreEvents;

internal class QueueScheduler
{
  private readonly IServiceScopeFactory _serviceScopeFactory;

  public QueueScheduler(
      IServiceScopeFactory serviceScopeFactory
      )
  {
    _serviceScopeFactory = serviceScopeFactory;
  }

  public async Task ScheduleAsync(string queueName, int receiveMessagesCount, int messagesLimit, string forwarding)
  {
    using var scope = _serviceScopeFactory.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<QueueScheduler>>();
    var eventApi = scope.ServiceProvider.GetRequiredService<IEventApi>();
    var queueApi = scope.ServiceProvider.GetRequiredService<IQueueApi>();
    var handlers = scope.ServiceProvider.GetRequiredService<IEnumerable<IMessageHandler>>();

    logger.LogDebug("Event handling starts");

    try
    {
      eventApi.UseApiAuth();
      queueApi.UseApiAuth();

      var response = await eventApi.EventManagementModuleEventReceiveManyAsync(queueName, receiveMessagesCount);

      var data = response.Ok();

      if (data == null)
      {
        logger.LogError("Failed to receive messages");
        return;
      }

      foreach (var message in data.Messages ?? [])
      {
        logger.LogDebug("Handling massage - Name: {0} Data: {1}", message.Name, message.Message);
        foreach (var handler in handlers)
        {
          logger.LogDebug("Starting handle message with handler: {0}", handler.GetType().Name);
          try
          {
            await handler.HandleAsync(message);
          }
          catch (Exception ex)
          {
            logger.LogError(ex, "An error occurred while executing message handler");
            try
            {
              await eventApi.EventManagementModuleEventPublishErrorAsync(JsonSerializer.Serialize(new { FailedMessage = message, Execption = ex }));
            }
            catch (Exception)
            {
              logger.LogError("Failed to handle message");
            }
          }
          finally
          {
            logger.LogDebug("Finished handle message with handler: {0}", handler.GetType().Name);
          }
        }
      }

      try
      {
        if (data.QueueStatus == "NotFound")
        {
          logger.LogWarning("Queue {queueName} not found. Executing ensure created", queueName);
          var ensureCreatedResponse = await queueApi.EventManagementModuleQueueEnsureCreatedAsync(new EventManagementModuleCreateQueueRequestDto
          {
            Name = queueName,
            Forwarding = forwarding,
            MessagesLimit = messagesLimit,
            DisplayName = queueName,
          });

          if (ensureCreatedResponse.IsSuccessStatusCode)
          {
            logger.LogDebug("Queue {queueName} created successfully", queueName);
          }
          else
          {
            logger.LogError("Failed to create queue {queueName}. Status code: {statusCode} Respone phrase: {responePhrase}", queueName, ensureCreatedResponse.StatusCode, ensureCreatedResponse.ReasonPhrase);
          }
        }
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "An exception was thrown while creating queue");
      }

      if ((data.Messages?.Count ?? 0) < receiveMessagesCount && (data.MessagesLeft ?? 0) > 0)
      {
        logger.LogError("Queue {queueName} is invalid: {0} messages received, expected {1}", queueName, data.Messages?.Count ?? 0, receiveMessagesCount);
      }
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "An exception was thrown while handling messages");
      throw;
    }
    finally
    {
      logger.LogDebug("Event handling finished");
    }
  }
}
