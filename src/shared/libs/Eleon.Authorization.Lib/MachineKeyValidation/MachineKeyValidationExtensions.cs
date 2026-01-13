using Microsoft.AspNetCore.Builder;

namespace Authorization.Module.MachineKeyValidation
{
  public static class MachineKeyValidationExtensions
  {
    public static void UseMachineKeyValidation(this IApplicationBuilder app)
    {
      app.UseMiddleware<MachineKeyValidationMiddleware>();
    }
  }
}
