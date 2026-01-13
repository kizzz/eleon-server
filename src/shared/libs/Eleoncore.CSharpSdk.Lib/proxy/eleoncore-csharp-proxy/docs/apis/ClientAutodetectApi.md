# EleoncoreProxy.Api.ClientAutodetectApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**SitesManagementClientAutodetectGetDetectedProxy**](ClientAutodetectApi.md#sitesmanagementclientautodetectgetdetectedproxy) | **GET** /api/Infrastructure/ClientAutodetect/GetDetectedProxy |  |
| [**SitesManagementClientAutodetectGetDetectedWeb**](ClientAutodetectApi.md#sitesmanagementclientautodetectgetdetectedweb) | **GET** /api/Infrastructure/ClientAutodetect/GetDetectedWeb |  |

<a id="sitesmanagementclientautodetectgetdetectedproxy"></a>
# **SitesManagementClientAutodetectGetDetectedProxy**
> List&lt;SitesManagementApplicationModuleDto&gt; SitesManagementClientAutodetectGetDetectedProxy (Guid proxyId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementClientAutodetectGetDetectedProxyExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ClientAutodetectApi(config);
            var proxyId = "proxyId_example";  // Guid |  (optional) 

            try
            {
                List<SitesManagementApplicationModuleDto> result = apiInstance.SitesManagementClientAutodetectGetDetectedProxy(proxyId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ClientAutodetectApi.SitesManagementClientAutodetectGetDetectedProxy: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementClientAutodetectGetDetectedProxyWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<SitesManagementApplicationModuleDto>> response = apiInstance.SitesManagementClientAutodetectGetDetectedProxyWithHttpInfo(proxyId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ClientAutodetectApi.SitesManagementClientAutodetectGetDetectedProxyWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **proxyId** | **Guid** |  | [optional]  |

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

<a id="sitesmanagementclientautodetectgetdetectedweb"></a>
# **SitesManagementClientAutodetectGetDetectedWeb**
> List&lt;SitesManagementApplicationModuleDto&gt; SitesManagementClientAutodetectGetDetectedWeb (string url = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementClientAutodetectGetDetectedWebExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ClientAutodetectApi(config);
            var url = "url_example";  // string |  (optional) 

            try
            {
                List<SitesManagementApplicationModuleDto> result = apiInstance.SitesManagementClientAutodetectGetDetectedWeb(url);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ClientAutodetectApi.SitesManagementClientAutodetectGetDetectedWeb: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementClientAutodetectGetDetectedWebWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<SitesManagementApplicationModuleDto>> response = apiInstance.SitesManagementClientAutodetectGetDetectedWebWithHttpInfo(url);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ClientAutodetectApi.SitesManagementClientAutodetectGetDetectedWebWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **url** | **string** |  | [optional]  |

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

