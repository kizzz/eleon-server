using Logging.Module;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

public class AntiforgeryValidationMiddleware
{
  private readonly RequestDelegate _next;
  private readonly IAntiforgery _antiforgery;
  private readonly IVportalLogger<AntiforgeryValidationMiddleware> logger;

  public AntiforgeryValidationMiddleware(RequestDelegate next, IAntiforgery antiforgery,
      IVportalLogger<AntiforgeryValidationMiddleware> logger)
  {
    _next = next;
    _antiforgery = antiforgery;
    this.logger = logger;
  }


  public async Task InvokeAsync(HttpContext context)
  {
    // Only validate POST requests to specific paths (e.g., /Account/Login)
    if (context.Request.Method == HttpMethods.Post && context.Request.Path.StartsWithSegments("/Account/Login"))
    {
      try
      {
        await _antiforgery.ValidateRequestAsync(context);
      }
      catch (AntiforgeryValidationException ex)
      {
        System.Diagnostics.Debug.WriteLine("Antiforgery validation failed: " + ex.Message);
        throw;
      }
    }

    await _next(context);
  }
}
