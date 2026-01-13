# EleonsoftProxy.Api.UserOtpApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**OtpUserOtpGenerateOtp**](UserOtpApi.md#otpuserotpgenerateotp) | **POST** /api/Otp/UserOtp/Generate |  |
| [**OtpUserOtpIgnoreLastUserOtp**](UserOtpApi.md#otpuserotpignorelastuserotp) | **POST** /api/Otp/UserOtp/IgnoreLastUserOtp |  |
| [**OtpUserOtpValidateOtp**](UserOtpApi.md#otpuserotpvalidateotp) | **GET** /api/Otp/UserOtp/Validate |  |

<a id="otpuserotpgenerateotp"></a>
# **OtpUserOtpGenerateOtp**
> MessagingOtpGenerationResultDto OtpUserOtpGenerateOtp (string key = null, string message = null, List<MessagingOtpRecepientEto> messagingOtpRecepientEto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class OtpUserOtpGenerateOtpExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new UserOtpApi(config);
            var key = "key_example";  // string |  (optional) 
            var message = "message_example";  // string |  (optional) 
            var messagingOtpRecepientEto = new List<MessagingOtpRecepientEto>(); // List<MessagingOtpRecepientEto> |  (optional) 

            try
            {
                MessagingOtpGenerationResultDto result = apiInstance.OtpUserOtpGenerateOtp(key, message, messagingOtpRecepientEto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling UserOtpApi.OtpUserOtpGenerateOtp: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the OtpUserOtpGenerateOtpWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<MessagingOtpGenerationResultDto> response = apiInstance.OtpUserOtpGenerateOtpWithHttpInfo(key, message, messagingOtpRecepientEto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling UserOtpApi.OtpUserOtpGenerateOtpWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **key** | **string** |  | [optional]  |
| **message** | **string** |  | [optional]  |
| **messagingOtpRecepientEto** | [**List&lt;MessagingOtpRecepientEto&gt;**](MessagingOtpRecepientEto.md) |  | [optional]  |

### Return type

[**MessagingOtpGenerationResultDto**](MessagingOtpGenerationResultDto.md)

### Authorization

[oauth2](../README.md#oauth2)

### HTTP request headers

 - **Content-Type**: application/json, text/json, application/*+json
 - **Accept**: text/plain, application/json, text/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |
| **403** | Forbidden |  -  |
| **401** | Unauthorized |  -  |
| **400** | Bad Request |  -  |
| **404** | Not Found |  -  |
| **501** | Not Implemented |  -  |
| **500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="otpuserotpignorelastuserotp"></a>
# **OtpUserOtpIgnoreLastUserOtp**
> bool OtpUserOtpIgnoreLastUserOtp ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class OtpUserOtpIgnoreLastUserOtpExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new UserOtpApi(config);

            try
            {
                bool result = apiInstance.OtpUserOtpIgnoreLastUserOtp();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling UserOtpApi.OtpUserOtpIgnoreLastUserOtp: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the OtpUserOtpIgnoreLastUserOtpWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.OtpUserOtpIgnoreLastUserOtpWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling UserOtpApi.OtpUserOtpIgnoreLastUserOtpWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

**bool**

### Authorization

[oauth2](../README.md#oauth2)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/json, text/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |
| **403** | Forbidden |  -  |
| **401** | Unauthorized |  -  |
| **400** | Bad Request |  -  |
| **404** | Not Found |  -  |
| **501** | Not Implemented |  -  |
| **500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="otpuserotpvalidateotp"></a>
# **OtpUserOtpValidateOtp**
> MessagingOtpValidationResultEto OtpUserOtpValidateOtp (string key = null, string password = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class OtpUserOtpValidateOtpExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new UserOtpApi(config);
            var key = "key_example";  // string |  (optional) 
            var password = "password_example";  // string |  (optional) 

            try
            {
                MessagingOtpValidationResultEto result = apiInstance.OtpUserOtpValidateOtp(key, password);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling UserOtpApi.OtpUserOtpValidateOtp: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the OtpUserOtpValidateOtpWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<MessagingOtpValidationResultEto> response = apiInstance.OtpUserOtpValidateOtpWithHttpInfo(key, password);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling UserOtpApi.OtpUserOtpValidateOtpWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **key** | **string** |  | [optional]  |
| **password** | **string** |  | [optional]  |

### Return type

[**MessagingOtpValidationResultEto**](MessagingOtpValidationResultEto.md)

### Authorization

[oauth2](../README.md#oauth2)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/json, text/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |
| **403** | Forbidden |  -  |
| **401** | Unauthorized |  -  |
| **400** | Bad Request |  -  |
| **404** | Not Found |  -  |
| **501** | Not Implemented |  -  |
| **500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

