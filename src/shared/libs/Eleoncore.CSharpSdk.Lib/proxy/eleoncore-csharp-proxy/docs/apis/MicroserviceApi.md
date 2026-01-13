# EleoncoreProxy.Api.MicroserviceApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**SitesManagementMicroserviceCreate**](MicroserviceApi.md#sitesmanagementmicroservicecreate) | **POST** /api/SitesManagement/MicroserviceController/Create |  |
| [**SitesManagementMicroserviceGetMicroserviceList**](MicroserviceApi.md#sitesmanagementmicroservicegetmicroservicelist) | **GET** /api/SitesManagement/MicroserviceController/GetMicroserviceList |  |
| [**SitesManagementMicroserviceInitializeMicroservice**](MicroserviceApi.md#sitesmanagementmicroserviceinitializemicroservice) | **POST** /api/SitesManagement/MicroserviceController/InitializeMicroservice |  |

<a id="sitesmanagementmicroservicecreate"></a>
# **SitesManagementMicroserviceCreate**
> SitesManagementEleoncoreModuleDto SitesManagementMicroserviceCreate (SitesManagementEleoncoreModuleDto sitesManagementEleoncoreModuleDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementMicroserviceCreateExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new MicroserviceApi(config);
            var sitesManagementEleoncoreModuleDto = new SitesManagementEleoncoreModuleDto(); // SitesManagementEleoncoreModuleDto |  (optional) 

            try
            {
                SitesManagementEleoncoreModuleDto result = apiInstance.SitesManagementMicroserviceCreate(sitesManagementEleoncoreModuleDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling MicroserviceApi.SitesManagementMicroserviceCreate: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementMicroserviceCreateWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<SitesManagementEleoncoreModuleDto> response = apiInstance.SitesManagementMicroserviceCreateWithHttpInfo(sitesManagementEleoncoreModuleDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling MicroserviceApi.SitesManagementMicroserviceCreateWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **sitesManagementEleoncoreModuleDto** | [**SitesManagementEleoncoreModuleDto**](SitesManagementEleoncoreModuleDto.md) |  | [optional]  |

### Return type

[**SitesManagementEleoncoreModuleDto**](SitesManagementEleoncoreModuleDto.md)

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

<a id="sitesmanagementmicroservicegetmicroservicelist"></a>
# **SitesManagementMicroserviceGetMicroserviceList**
> List&lt;SitesManagementEleoncoreModuleDto&gt; SitesManagementMicroserviceGetMicroserviceList ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementMicroserviceGetMicroserviceListExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new MicroserviceApi(config);

            try
            {
                List<SitesManagementEleoncoreModuleDto> result = apiInstance.SitesManagementMicroserviceGetMicroserviceList();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling MicroserviceApi.SitesManagementMicroserviceGetMicroserviceList: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementMicroserviceGetMicroserviceListWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<SitesManagementEleoncoreModuleDto>> response = apiInstance.SitesManagementMicroserviceGetMicroserviceListWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling MicroserviceApi.SitesManagementMicroserviceGetMicroserviceListWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**List&lt;SitesManagementEleoncoreModuleDto&gt;**](SitesManagementEleoncoreModuleDto.md)

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

<a id="sitesmanagementmicroserviceinitializemicroservice"></a>
# **SitesManagementMicroserviceInitializeMicroservice**
> bool SitesManagementMicroserviceInitializeMicroservice (MessagingInitializeMicroserviceMsg messagingInitializeMicroserviceMsg = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementMicroserviceInitializeMicroserviceExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new MicroserviceApi(config);
            var messagingInitializeMicroserviceMsg = new MessagingInitializeMicroserviceMsg(); // MessagingInitializeMicroserviceMsg |  (optional) 

            try
            {
                bool result = apiInstance.SitesManagementMicroserviceInitializeMicroservice(messagingInitializeMicroserviceMsg);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling MicroserviceApi.SitesManagementMicroserviceInitializeMicroservice: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementMicroserviceInitializeMicroserviceWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.SitesManagementMicroserviceInitializeMicroserviceWithHttpInfo(messagingInitializeMicroserviceMsg);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling MicroserviceApi.SitesManagementMicroserviceInitializeMicroserviceWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **messagingInitializeMicroserviceMsg** | [**MessagingInitializeMicroserviceMsg**](MessagingInitializeMicroserviceMsg.md) |  | [optional]  |

### Return type

**bool**

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

