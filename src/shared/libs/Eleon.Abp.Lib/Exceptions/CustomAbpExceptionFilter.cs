using Logging.Module.ErrorHandling.Constants;
using Logging.Module.ErrorHandling.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Linq;
using Volo.Abp.AspNetCore.ExceptionHandling;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.ExceptionHandling;
using Volo.Abp.Authorization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ExceptionHandling;
using Volo.Abp.Http;

namespace EleonsoftAbp.Exceptions;

public class CustomAbpExceptionFilter : AbpExceptionFilter
{
  internal static bool UseObjectResult { get; set; } = true;
  protected override bool ShouldHandleException(ExceptionContext context)
  {
    var isTenantNotReslovedStatusCode = context.Exception.GetStatusCode() == EleonsoftStatusCodes.Default.TenantWasNotResoleved;
    if (isTenantNotReslovedStatusCode)
      return false;

    return base.ShouldHandleException(context);
  }

  protected override async Task HandleAndWrapException(ExceptionContext context)
  {
    // Skip logging for authorization exceptions - they are expected business logic, not errors
    if (context.Exception is AbpAuthorizationException)
    {
      // Get error info without logging
      var exceptionToErrorInfoConverter = context.GetRequiredService<IExceptionToErrorInfoConverter>();
      var remoteServiceErrorInfo = exceptionToErrorInfoConverter.Convert(context.Exception, options =>
      {
        options.SendExceptionsDetailsToClients = false;
        options.SendStackTraceToClients = false;
      });
      
      // Continue with error handling but skip notification for authorization exceptions
      if (!context.HttpContext.Response.HasStarted)
      {
        context.HttpContext.Response.Headers.Append(AbpHttpConsts.AbpErrorFormat, "true");
        context.HttpContext.Response.StatusCode = (int)context
            .GetRequiredService<IHttpExceptionStatusCodeFinder>()
            .GetStatusCode(context.HttpContext, context.Exception);
      }

      context.Result = await ResolveResult(context, remoteServiceErrorInfo);
      context.ExceptionHandled = true;
      return;
    }

    LogException(context, out var remoteServiceErrorInfo);

    await context.GetRequiredService<IExceptionNotifier>().NotifyAsync(new ExceptionNotificationContext(context.Exception));

    //if (context.Exception is AbpAuthorizationException)
    //{
    //    await context.HttpContext.RequestServices.GetRequiredService<IAbpAuthorizationExceptionHandler>()
    //        .HandleAsync(context.Exception.As<AbpAuthorizationException>(), context.HttpContext);
    //}
    //else
    //{
    if (!context.HttpContext.Response.HasStarted)
    {
      context.HttpContext.Response.Headers.Append(AbpHttpConsts.AbpErrorFormat, "true");

      if (int.TryParse(context.Exception.Data["StatusCode"]?.ToString(), out var statusCode))
      {
        context.HttpContext.Response.StatusCode = statusCode;
      }
      else
      {
        context.HttpContext.Response.StatusCode = (int)context
            .GetRequiredService<IHttpExceptionStatusCodeFinder>()
            .GetStatusCode(context.HttpContext, context.Exception);
      }
    }
    else
    {
      var logger = context.GetService<ILogger<AbpExceptionFilter>>(NullLogger<AbpExceptionFilter>.Instance)!;
      logger.LogWarning("HTTP response has already started, cannot set headers and status code!");
    }

    context.Result = await ResolveResult(context, remoteServiceErrorInfo);
    //}

    context.ExceptionHandled = true; //Handled!
  }

  protected virtual async Task<IActionResult> ResolveResult(ExceptionContext context, RemoteServiceErrorInfo? remoteServiceErrorInfo)
  {
    if (UseObjectResult)
    {
      return new ObjectResult(new RemoteServiceErrorResponse(remoteServiceErrorInfo)); // default abp implementation
    }

    return new ContentResult
    {
      Content = context.Exception.Message,
      ContentType = "text/plain",
      StatusCode = context.HttpContext.Response.StatusCode
    };
  }
}
