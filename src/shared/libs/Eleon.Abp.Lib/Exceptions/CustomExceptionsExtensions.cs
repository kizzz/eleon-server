using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.ExceptionHandling;

namespace EleonsoftAbp.Exceptions;
public static class CustomExceptionsExtensions
{
  public static IServiceCollection AddEleonsoftExceptionsFilter(this IServiceCollection services, bool useObjectResult) // default true
  {
    CustomAbpExceptionFilter.UseObjectResult = useObjectResult;
    services.AddTransient<AbpExceptionFilter, CustomAbpExceptionFilter>();
    services.AddTransient<IAsyncExceptionFilter, CustomAbpExceptionFilter>();
    return services;
  }
}
