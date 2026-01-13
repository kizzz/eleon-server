# EleonsoftProxy.Api.DashboardSettingApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**CoreDashboardSettingDeleteDashboardSettings**](DashboardSettingApi.md#coredashboardsettingdeletedashboardsettings) | **POST** /api/Infrastructure/DashboardSettings/DeleteDashboardSettings |  |
| [**CoreDashboardSettingGetDashboardSettings**](DashboardSettingApi.md#coredashboardsettinggetdashboardsettings) | **GET** /api/Infrastructure/DashboardSettings/GetDashboardSettings |  |
| [**CoreDashboardSettingUpdateSettings**](DashboardSettingApi.md#coredashboardsettingupdatesettings) | **POST** /api/Infrastructure/DashboardSettings/CreateOrUpdateSettings |  |

<a id="coredashboardsettingdeletedashboardsettings"></a>
# **CoreDashboardSettingDeleteDashboardSettings**
> string CoreDashboardSettingDeleteDashboardSettings (Guid dashboardSettingEntityId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreDashboardSettingDeleteDashboardSettingsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new DashboardSettingApi(config);
            var dashboardSettingEntityId = "dashboardSettingEntityId_example";  // Guid |  (optional) 

            try
            {
                string result = apiInstance.CoreDashboardSettingDeleteDashboardSettings(dashboardSettingEntityId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling DashboardSettingApi.CoreDashboardSettingDeleteDashboardSettings: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreDashboardSettingDeleteDashboardSettingsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<string> response = apiInstance.CoreDashboardSettingDeleteDashboardSettingsWithHttpInfo(dashboardSettingEntityId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling DashboardSettingApi.CoreDashboardSettingDeleteDashboardSettingsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **dashboardSettingEntityId** | **Guid** |  | [optional]  |

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

<a id="coredashboardsettinggetdashboardsettings"></a>
# **CoreDashboardSettingGetDashboardSettings**
> List&lt;CoreDashboardSettingDto&gt; CoreDashboardSettingGetDashboardSettings ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreDashboardSettingGetDashboardSettingsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new DashboardSettingApi(config);

            try
            {
                List<CoreDashboardSettingDto> result = apiInstance.CoreDashboardSettingGetDashboardSettings();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling DashboardSettingApi.CoreDashboardSettingGetDashboardSettings: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreDashboardSettingGetDashboardSettingsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<CoreDashboardSettingDto>> response = apiInstance.CoreDashboardSettingGetDashboardSettingsWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling DashboardSettingApi.CoreDashboardSettingGetDashboardSettingsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**List&lt;CoreDashboardSettingDto&gt;**](CoreDashboardSettingDto.md)

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

<a id="coredashboardsettingupdatesettings"></a>
# **CoreDashboardSettingUpdateSettings**
> string CoreDashboardSettingUpdateSettings (bool setAsDefault = null, List<CoreDashboardSettingDto> coreDashboardSettingDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreDashboardSettingUpdateSettingsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new DashboardSettingApi(config);
            var setAsDefault = true;  // bool |  (optional) 
            var coreDashboardSettingDto = new List<CoreDashboardSettingDto>(); // List<CoreDashboardSettingDto> |  (optional) 

            try
            {
                string result = apiInstance.CoreDashboardSettingUpdateSettings(setAsDefault, coreDashboardSettingDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling DashboardSettingApi.CoreDashboardSettingUpdateSettings: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreDashboardSettingUpdateSettingsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<string> response = apiInstance.CoreDashboardSettingUpdateSettingsWithHttpInfo(setAsDefault, coreDashboardSettingDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling DashboardSettingApi.CoreDashboardSettingUpdateSettingsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **setAsDefault** | **bool** |  | [optional]  |
| **coreDashboardSettingDto** | [**List&lt;CoreDashboardSettingDto&gt;**](CoreDashboardSettingDto.md) |  | [optional]  |

### Return type

**string**

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

