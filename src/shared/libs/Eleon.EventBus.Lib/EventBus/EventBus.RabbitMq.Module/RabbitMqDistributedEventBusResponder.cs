using Common.EventBus.Module;
using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Volo.Abp.RabbitMQ;

namespace EventBus.RabbitMqOverrides.Module
{
  public class RabbitMqDistributedEventBusResponder : IDistributedEventBusResponder
  {
    private readonly IChannel channel;
    private readonly BasicDeliverEventArgs request;
    private readonly IRabbitMqSerializer serializer;

    public RabbitMqDistributedEventBusResponder(
        IChannel channel,
        BasicDeliverEventArgs request,
        IRabbitMqSerializer serializer)
    {
      this.channel = channel;
      this.request = request;
      this.serializer = serializer;
    }

    public async Task RespondAsync(object eventData)
    {
      string replyToRoutingKey = request.BasicProperties.ReplyTo;
      if (replyToRoutingKey.IsNullOrEmpty())
      {
        throw new Exception("Unable to respond as the other side did not specify the reply-to queue on request.");
      }

      var responseData = serializer.Serialize(eventData);
      await channel.BasicPublishAsync(
          RabbitMqReplyConsts.ReplyExchangeName,
          replyToRoutingKey,
          responseData);
    }
  }
}
