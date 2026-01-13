# EleonsoftProxy.Api.ExternalLinkApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ExternalLinkExternalLinkDirectLogin**](ExternalLinkApi.md#externallinkexternallinkdirectlogin) | **POST** /api/external-links/DirectLoginAsync |  |
| [**ExternalLinkExternalLinkGetLoginInfo**](ExternalLinkApi.md#externallinkexternallinkgetlogininfo) | **POST** /api/external-links/GetLoginInfoAsync |  |
| [**ExternalLinkExternalLinkGetOtp**](ExternalLinkApi.md#externallinkexternallinkgetotp) | **POST** /api/external-links/GetOtp |  |
| [**ExternalLinkExternalLinkLoginWithOtp**](ExternalLinkApi.md#externallinkexternallinkloginwithotp) | **POST** /api/external-links/LoginWithOtp |  |

<a id="externallinkexternallinkdirectlogin"></a>
# **ExternalLinkExternalLinkDirectLogin**
> string ExternalLinkExternalLinkDirectLogin (string code = null, string password = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class ExternalLinkExternalLinkDirectLoginExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ExternalLinkApi(config);
            var code = "code_example";  // string |  (optional) 
            var password = "password_example";  // string |  (optional) 

            try
            {
                string result = apiInstance.ExternalLinkExternalLinkDirectLogin(code, password);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ExternalLinkApi.ExternalLinkExternalLinkDirectLogin: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ExternalLinkExternalLinkDirectLoginWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<string> response = apiInstance.ExternalLinkExternalLinkDirectLoginWithHttpInfo(code, password);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ExternalLinkApi.ExternalLinkExternalLinkDirectLoginWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **code** | **string** |  | [optional]  |
| **password** | **string** |  | [optional]  |

### Return type

**string**

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

<a id="externallinkexternallinkgetlogininfo"></a>
# **ExternalLinkExternalLinkGetLoginInfo**
> ExternalLinkExternalLinkLoginInfoDto ExternalLinkExternalLinkGetLoginInfo (string code = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class ExternalLinkExternalLinkGetLoginInfoExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ExternalLinkApi(config);
            var code = "code_example";  // string |  (optional) 

            try
            {
                ExternalLinkExternalLinkLoginInfoDto result = apiInstance.ExternalLinkExternalLinkGetLoginInfo(code);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ExternalLinkApi.ExternalLinkExternalLinkGetLoginInfo: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ExternalLinkExternalLinkGetLoginInfoWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<ExternalLinkExternalLinkLoginInfoDto> response = apiInstance.ExternalLinkExternalLinkGetLoginInfoWithHttpInfo(code);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ExternalLinkApi.ExternalLinkExternalLinkGetLoginInfoWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **code** | **string** |  | [optional]  |

### Return type

[**ExternalLinkExternalLinkLoginInfoDto**](ExternalLinkExternalLinkLoginInfoDto.md)

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

<a id="externallinkexternallinkgetotp"></a>
# **ExternalLinkExternalLinkGetOtp**
> string ExternalLinkExternalLinkGetOtp (string linkCode = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class ExternalLinkExternalLinkGetOtpExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ExternalLinkApi(config);
            var linkCode = "linkCode_example";  // string |  (optional) 

            try
            {
                string result = apiInstance.ExternalLinkExternalLinkGetOtp(linkCode);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ExternalLinkApi.ExternalLinkExternalLinkGetOtp: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ExternalLinkExternalLinkGetOtpWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<string> response = apiInstance.ExternalLinkExternalLinkGetOtpWithHttpInfo(linkCode);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ExternalLinkApi.ExternalLinkExternalLinkGetOtpWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **linkCode** | **string** |  | [optional]  |

### Return type

**string**

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

<a id="externallinkexternallinkloginwithotp"></a>
# **ExternalLinkExternalLinkLoginWithOtp**
> string ExternalLinkExternalLinkLoginWithOtp (string linkCode = null, string otp = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class ExternalLinkExternalLinkLoginWithOtpExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ExternalLinkApi(config);
            var linkCode = "linkCode_example";  // string |  (optional) 
            var otp = "otp_example";  // string |  (optional) 

            try
            {
                string result = apiInstance.ExternalLinkExternalLinkLoginWithOtp(linkCode, otp);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ExternalLinkApi.ExternalLinkExternalLinkLoginWithOtp: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ExternalLinkExternalLinkLoginWithOtpWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<string> response = apiInstance.ExternalLinkExternalLinkLoginWithOtpWithHttpInfo(linkCode, otp);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ExternalLinkApi.ExternalLinkExternalLinkLoginWithOtpWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **linkCode** | **string** |  | [optional]  |
| **otp** | **string** |  | [optional]  |

### Return type

**string**

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

