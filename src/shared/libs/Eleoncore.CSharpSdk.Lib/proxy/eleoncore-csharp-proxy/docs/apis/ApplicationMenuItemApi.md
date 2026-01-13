# EleoncoreProxy.Api.ApplicationMenuItemApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**SitesManagementApplicationMenuItemGetListByApplicationId**](ApplicationMenuItemApi.md#sitesmanagementapplicationmenuitemgetlistbyapplicationid) | **GET** /api/CoreInfrastructure/ApplicationMenuItems/GetListByApplicationId |  |
| [**SitesManagementApplicationMenuItemGetListByApplicationIdAndMenuType**](ApplicationMenuItemApi.md#sitesmanagementapplicationmenuitemgetlistbyapplicationidandmenutype) | **POST** /api/CoreInfrastructure/ApplicationMenuItems/GetListByApplicationIdAndMenuType |  |
| [**SitesManagementApplicationMenuItemUpdate**](ApplicationMenuItemApi.md#sitesmanagementapplicationmenuitemupdate) | **POST** /api/CoreInfrastructure/ApplicationMenuItems/Update |  |

<a id="sitesmanagementapplicationmenuitemgetlistbyapplicationid"></a>
# **SitesManagementApplicationMenuItemGetListByApplicationId**
> List&lt;SitesManagementApplicationMenuItemDto&gt; SitesManagementApplicationMenuItemGetListByApplicationId (Guid applicationId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementApplicationMenuItemGetListByApplicationIdExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ApplicationMenuItemApi(config);
            var applicationId = "applicationId_example";  // Guid |  (optional) 

            try
            {
                List<SitesManagementApplicationMenuItemDto> result = apiInstance.SitesManagementApplicationMenuItemGetListByApplicationId(applicationId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ApplicationMenuItemApi.SitesManagementApplicationMenuItemGetListByApplicationId: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementApplicationMenuItemGetListByApplicationIdWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<SitesManagementApplicationMenuItemDto>> response = apiInstance.SitesManagementApplicationMenuItemGetListByApplicationIdWithHttpInfo(applicationId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ApplicationMenuItemApi.SitesManagementApplicationMenuItemGetListByApplicationIdWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **applicationId** | **Guid** |  | [optional]  |

### Return type

[**List&lt;SitesManagementApplicationMenuItemDto&gt;**](SitesManagementApplicationMenuItemDto.md)

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

<a id="sitesmanagementapplicationmenuitemgetlistbyapplicationidandmenutype"></a>
# **SitesManagementApplicationMenuItemGetListByApplicationIdAndMenuType**
> List&lt;SitesManagementApplicationMenuItemDto&gt; SitesManagementApplicationMenuItemGetListByApplicationIdAndMenuType (Guid applicationId = null, SitesManagementMenuType menuType = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementApplicationMenuItemGetListByApplicationIdAndMenuTypeExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ApplicationMenuItemApi(config);
            var applicationId = "applicationId_example";  // Guid |  (optional) 
            var menuType = (SitesManagementMenuType) "0";  // SitesManagementMenuType |  (optional) 

            try
            {
                List<SitesManagementApplicationMenuItemDto> result = apiInstance.SitesManagementApplicationMenuItemGetListByApplicationIdAndMenuType(applicationId, menuType);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ApplicationMenuItemApi.SitesManagementApplicationMenuItemGetListByApplicationIdAndMenuType: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementApplicationMenuItemGetListByApplicationIdAndMenuTypeWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<SitesManagementApplicationMenuItemDto>> response = apiInstance.SitesManagementApplicationMenuItemGetListByApplicationIdAndMenuTypeWithHttpInfo(applicationId, menuType);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ApplicationMenuItemApi.SitesManagementApplicationMenuItemGetListByApplicationIdAndMenuTypeWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **applicationId** | **Guid** |  | [optional]  |
| **menuType** | **SitesManagementMenuType** |  | [optional]  |

### Return type

[**List&lt;SitesManagementApplicationMenuItemDto&gt;**](SitesManagementApplicationMenuItemDto.md)

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

<a id="sitesmanagementapplicationmenuitemupdate"></a>
# **SitesManagementApplicationMenuItemUpdate**
> List&lt;SitesManagementApplicationMenuItemDto&gt; SitesManagementApplicationMenuItemUpdate (Guid applicationId = null, List<SitesManagementApplicationMenuItemDto> sitesManagementApplicationMenuItemDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementApplicationMenuItemUpdateExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ApplicationMenuItemApi(config);
            var applicationId = "applicationId_example";  // Guid |  (optional) 
            var sitesManagementApplicationMenuItemDto = new List<SitesManagementApplicationMenuItemDto>(); // List<SitesManagementApplicationMenuItemDto> |  (optional) 

            try
            {
                List<SitesManagementApplicationMenuItemDto> result = apiInstance.SitesManagementApplicationMenuItemUpdate(applicationId, sitesManagementApplicationMenuItemDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ApplicationMenuItemApi.SitesManagementApplicationMenuItemUpdate: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementApplicationMenuItemUpdateWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<SitesManagementApplicationMenuItemDto>> response = apiInstance.SitesManagementApplicationMenuItemUpdateWithHttpInfo(applicationId, sitesManagementApplicationMenuItemDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ApplicationMenuItemApi.SitesManagementApplicationMenuItemUpdateWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **applicationId** | **Guid** |  | [optional]  |
| **sitesManagementApplicationMenuItemDto** | [**List&lt;SitesManagementApplicationMenuItemDto&gt;**](SitesManagementApplicationMenuItemDto.md) |  | [optional]  |

### Return type

[**List&lt;SitesManagementApplicationMenuItemDto&gt;**](SitesManagementApplicationMenuItemDto.md)

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

