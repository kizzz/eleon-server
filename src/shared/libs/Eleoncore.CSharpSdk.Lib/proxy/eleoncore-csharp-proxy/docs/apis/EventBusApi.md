# EleoncoreProxy.Api.EventBusApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**GatewayManagementEventBusAddEventBus**](EventBusApi.md#gatewaymanagementeventbusaddeventbus) | **POST** /api/GatewayManagement/EventBuses/AddEventBus |  |
| [**GatewayManagementEventBusGetEventBusOptionsTemplates**](EventBusApi.md#gatewaymanagementeventbusgeteventbusoptionstemplates) | **GET** /api/GatewayManagement/EventBuses/GetEventBusOptionsTemplates |  |
| [**GatewayManagementEventBusGetEventBuses**](EventBusApi.md#gatewaymanagementeventbusgeteventbuses) | **GET** /api/GatewayManagement/EventBuses/GetEventBuses |  |

<a id="gatewaymanagementeventbusaddeventbus"></a>
# **GatewayManagementEventBusAddEventBus**
> void GatewayManagementEventBusAddEventBus (GatewayManagementEventBusDto gatewayManagementEventBusDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class GatewayManagementEventBusAddEventBusExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new EventBusApi(config);
            var gatewayManagementEventBusDto = new GatewayManagementEventBusDto(); // GatewayManagementEventBusDto |  (optional) 

            try
            {
                apiInstance.GatewayManagementEventBusAddEventBus(gatewayManagementEventBusDto);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventBusApi.GatewayManagementEventBusAddEventBus: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GatewayManagementEventBusAddEventBusWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.GatewayManagementEventBusAddEventBusWithHttpInfo(gatewayManagementEventBusDto);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling EventBusApi.GatewayManagementEventBusAddEventBusWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **gatewayManagementEventBusDto** | [**GatewayManagementEventBusDto**](GatewayManagementEventBusDto.md) |  | [optional]  |

### Return type

void (empty response body)

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

<a id="gatewaymanagementeventbusgeteventbusoptionstemplates"></a>
# **GatewayManagementEventBusGetEventBusOptionsTemplates**
> List&lt;GatewayManagementEventBusOptionsTemplateDto&gt; GatewayManagementEventBusGetEventBusOptionsTemplates ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class GatewayManagementEventBusGetEventBusOptionsTemplatesExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new EventBusApi(config);

            try
            {
                List<GatewayManagementEventBusOptionsTemplateDto> result = apiInstance.GatewayManagementEventBusGetEventBusOptionsTemplates();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventBusApi.GatewayManagementEventBusGetEventBusOptionsTemplates: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GatewayManagementEventBusGetEventBusOptionsTemplatesWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<GatewayManagementEventBusOptionsTemplateDto>> response = apiInstance.GatewayManagementEventBusGetEventBusOptionsTemplatesWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling EventBusApi.GatewayManagementEventBusGetEventBusOptionsTemplatesWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**List&lt;GatewayManagementEventBusOptionsTemplateDto&gt;**](GatewayManagementEventBusOptionsTemplateDto.md)

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

<a id="gatewaymanagementeventbusgeteventbuses"></a>
# **GatewayManagementEventBusGetEventBuses**
> List&lt;GatewayManagementEventBusDto&gt; GatewayManagementEventBusGetEventBuses ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class GatewayManagementEventBusGetEventBusesExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new EventBusApi(config);

            try
            {
                List<GatewayManagementEventBusDto> result = apiInstance.GatewayManagementEventBusGetEventBuses();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventBusApi.GatewayManagementEventBusGetEventBuses: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GatewayManagementEventBusGetEventBusesWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<GatewayManagementEventBusDto>> response = apiInstance.GatewayManagementEventBusGetEventBusesWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling EventBusApi.GatewayManagementEventBusGetEventBusesWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**List&lt;GatewayManagementEventBusDto&gt;**](GatewayManagementEventBusDto.md)

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

