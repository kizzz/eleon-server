using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModule.HttpApi.Helpers;
public class EleoncoreSdkConfig
{
  public string BaseHost { get; set; }
  public string BasePath { get; set; }
  public string OAuthUrl { get; set; }
  public string OAuthToken { get; set; }
  public string ApiToken { get; set; }
  public string ApiAuthUrl { get; set; }
  public string ClientId { get; set; }
  public string ApiKey { get; set; }
  public string ApiKeySecret { get; set; }
  public string ClientSecret { get; set; }
  public bool UseApiAuthorization { get; set; }
  public bool UseOAuthAuthorization { get; set; }
  public bool IsApiAuthorizationDefault { get; set; }
  public bool IgnoreSslValidation { get; set; }
}

public static class EleoncoreSdkConfigExtensions
{
  public static EleoncoreSdkConfig FromConfiguration(this EleoncoreSdkConfig sdkConfig, IConfiguration configuration)
  {
    sdkConfig.BaseHost = configuration["EleoncoreSdk:BaseEleonsoftHost"];
    sdkConfig.BasePath = configuration["EleoncoreSdk:BasePath"];
    sdkConfig.OAuthUrl = configuration["EleoncoreSdk:OAuthUrl"];
    sdkConfig.UseOAuthAuthorization = bool.Parse(configuration["EleoncoreSdk:UseOAuthAuthorization"]);
    sdkConfig.ApiAuthUrl = configuration["EleoncoreSdk:ApiAuthUrl"];
    sdkConfig.ApiKey = configuration["EleoncoreSdk:ApiKey"];
    sdkConfig.ApiKeySecret = configuration["EleoncoreSdk:ApiKeySecret"];
    sdkConfig.UseApiAuthorization = bool.Parse(configuration["EleoncoreSdk:UseApiAuthorization"]);
    sdkConfig.ClientSecret = configuration["EleoncoreSdk:SecretKey"];
    sdkConfig.ClientId = configuration["EleoncoreSdk:AppKey"];
    sdkConfig.IgnoreSslValidation = configuration["EleoncoreSdk:IgnoreSslValidation"]?.ToLower() == "true";

    return sdkConfig;
  }
}
