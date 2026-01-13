using System.Threading.Tasks;
using Volo.Abp.ExceptionHandling;

namespace VPortal.Exceptions
{
  public class SentryExceptionSubscriber : ExceptionSubscriber
  {
    public override Task HandleAsync(ExceptionNotificationContext context)
    {
      return Task.CompletedTask;
      //if (context == null)
      //    return;

      //await Task.Run(() => SentrySdk.CaptureException(context.Exception));
    }
  }
}
