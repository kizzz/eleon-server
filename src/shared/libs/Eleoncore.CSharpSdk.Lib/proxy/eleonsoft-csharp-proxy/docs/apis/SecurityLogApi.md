# EleonsoftProxy.Api.SecurityLogApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**InfrastructureSecurityLogGetSecurityLogById**](SecurityLogApi.md#infrastructuresecurityloggetsecuritylogbyid) | **GET** /api/SecurityLog/SecurityLogs/GetSecurityLogById |  |
| [**InfrastructureSecurityLogGetSecurityLogList**](SecurityLogApi.md#infrastructuresecurityloggetsecurityloglist) | **POST** /api/SecurityLog/SecurityLogs/GetSecurityLogList |  |

<a id="infrastructuresecurityloggetsecuritylogbyid"></a>
# **InfrastructureSecurityLogGetSecurityLogById**
> EleoncoreFullSecurityLogDto InfrastructureSecurityLogGetSecurityLogById (Guid id = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class InfrastructureSecurityLogGetSecurityLogByIdExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new SecurityLogApi(config);
            var id = "id_example";  // Guid |  (optional) 

            try
            {
                EleoncoreFullSecurityLogDto result = apiInstance.InfrastructureSecurityLogGetSecurityLogById(id);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling SecurityLogApi.InfrastructureSecurityLogGetSecurityLogById: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the InfrastructureSecurityLogGetSecurityLogByIdWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleoncoreFullSecurityLogDto> response = apiInstance.InfrastructureSecurityLogGetSecurityLogByIdWithHttpInfo(id);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling SecurityLogApi.InfrastructureSecurityLogGetSecurityLogByIdWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **Guid** |  | [optional]  |

### Return type

[**EleoncoreFullSecurityLogDto**](EleoncoreFullSecurityLogDto.md)

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

<a id="infrastructuresecurityloggetsecurityloglist"></a>
# **InfrastructureSecurityLogGetSecurityLogList**
> EleoncorePagedResultDtoOfEleoncoreSecurityLogDto InfrastructureSecurityLogGetSecurityLogList (EleoncoreSecurityLogListRequestDto eleoncoreSecurityLogListRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class InfrastructureSecurityLogGetSecurityLogListExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new SecurityLogApi(config);
            var eleoncoreSecurityLogListRequestDto = new EleoncoreSecurityLogListRequestDto(); // EleoncoreSecurityLogListRequestDto |  (optional) 

            try
            {
                EleoncorePagedResultDtoOfEleoncoreSecurityLogDto result = apiInstance.InfrastructureSecurityLogGetSecurityLogList(eleoncoreSecurityLogListRequestDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling SecurityLogApi.InfrastructureSecurityLogGetSecurityLogList: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the InfrastructureSecurityLogGetSecurityLogListWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleoncorePagedResultDtoOfEleoncoreSecurityLogDto> response = apiInstance.InfrastructureSecurityLogGetSecurityLogListWithHttpInfo(eleoncoreSecurityLogListRequestDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling SecurityLogApi.InfrastructureSecurityLogGetSecurityLogListWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **eleoncoreSecurityLogListRequestDto** | [**EleoncoreSecurityLogListRequestDto**](EleoncoreSecurityLogListRequestDto.md) |  | [optional]  |

### Return type

[**EleoncorePagedResultDtoOfEleoncoreSecurityLogDto**](EleoncorePagedResultDtoOfEleoncoreSecurityLogDto.md)

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

