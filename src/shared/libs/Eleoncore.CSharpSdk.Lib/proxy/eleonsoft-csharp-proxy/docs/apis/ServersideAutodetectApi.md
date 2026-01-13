# EleonsoftProxy.Api.ServersideAutodetectApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**SitesManagementServersideAutodetectGetDetectedModules**](ServersideAutodetectApi.md#sitesmanagementserversideautodetectgetdetectedmodules) | **GET** /api/Infrastructure/ServersideAutodetect/GetDetectedModules |  |
| [**SitesManagementServersideAutodetectStartDetect**](ServersideAutodetectApi.md#sitesmanagementserversideautodetectstartdetect) | **GET** /api/Infrastructure/ServersideAutodetect/StartDetect |  |

<a id="sitesmanagementserversideautodetectgetdetectedmodules"></a>
# **SitesManagementServersideAutodetectGetDetectedModules**
> List&lt;SitesManagementApplicationModuleDto&gt; SitesManagementServersideAutodetectGetDetectedModules ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class SitesManagementServersideAutodetectGetDetectedModulesExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ServersideAutodetectApi(config);

            try
            {
                List<SitesManagementApplicationModuleDto> result = apiInstance.SitesManagementServersideAutodetectGetDetectedModules();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ServersideAutodetectApi.SitesManagementServersideAutodetectGetDetectedModules: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementServersideAutodetectGetDetectedModulesWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<SitesManagementApplicationModuleDto>> response = apiInstance.SitesManagementServersideAutodetectGetDetectedModulesWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ServersideAutodetectApi.SitesManagementServersideAutodetectGetDetectedModulesWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**List&lt;SitesManagementApplicationModuleDto&gt;**](SitesManagementApplicationModuleDto.md)

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

<a id="sitesmanagementserversideautodetectstartdetect"></a>
# **SitesManagementServersideAutodetectStartDetect**
> void SitesManagementServersideAutodetectStartDetect ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class SitesManagementServersideAutodetectStartDetectExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ServersideAutodetectApi(config);

            try
            {
                apiInstance.SitesManagementServersideAutodetectStartDetect();
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ServersideAutodetectApi.SitesManagementServersideAutodetectStartDetect: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementServersideAutodetectStartDetectWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.SitesManagementServersideAutodetectStartDetectWithHttpInfo();
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ServersideAutodetectApi.SitesManagementServersideAutodetectStartDetectWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

void (empty response body)

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

