# EleonsoftProxy.Api.FeaturesApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**TenantManagementFeaturesDelete**](FeaturesApi.md#tenantmanagementfeaturesdelete) | **POST** /api/TenantManagement/Features/DeleteAsync |  |
| [**TenantManagementFeaturesGet**](FeaturesApi.md#tenantmanagementfeaturesget) | **GET** /api/TenantManagement/Features/GetAsync |  |
| [**TenantManagementFeaturesUpdate**](FeaturesApi.md#tenantmanagementfeaturesupdate) | **POST** /api/TenantManagement/Features/UpdateAsync |  |

<a id="tenantmanagementfeaturesdelete"></a>
# **TenantManagementFeaturesDelete**
> void TenantManagementFeaturesDelete (string providerName = null, string providerKey = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementFeaturesDeleteExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FeaturesApi(config);
            var providerName = "providerName_example";  // string |  (optional) 
            var providerKey = "providerKey_example";  // string |  (optional) 

            try
            {
                apiInstance.TenantManagementFeaturesDelete(providerName, providerKey);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FeaturesApi.TenantManagementFeaturesDelete: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementFeaturesDeleteWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.TenantManagementFeaturesDeleteWithHttpInfo(providerName, providerKey);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FeaturesApi.TenantManagementFeaturesDeleteWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **providerName** | **string** |  | [optional]  |
| **providerKey** | **string** |  | [optional]  |

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

<a id="tenantmanagementfeaturesget"></a>
# **TenantManagementFeaturesGet**
> EleoncoreGetFeatureListResultDto TenantManagementFeaturesGet (string providerName = null, string providerKey = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementFeaturesGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FeaturesApi(config);
            var providerName = "providerName_example";  // string |  (optional) 
            var providerKey = "providerKey_example";  // string |  (optional) 

            try
            {
                EleoncoreGetFeatureListResultDto result = apiInstance.TenantManagementFeaturesGet(providerName, providerKey);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FeaturesApi.TenantManagementFeaturesGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementFeaturesGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleoncoreGetFeatureListResultDto> response = apiInstance.TenantManagementFeaturesGetWithHttpInfo(providerName, providerKey);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FeaturesApi.TenantManagementFeaturesGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **providerName** | **string** |  | [optional]  |
| **providerKey** | **string** |  | [optional]  |

### Return type

[**EleoncoreGetFeatureListResultDto**](EleoncoreGetFeatureListResultDto.md)

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

<a id="tenantmanagementfeaturesupdate"></a>
# **TenantManagementFeaturesUpdate**
> void TenantManagementFeaturesUpdate (string providerName = null, string providerKey = null, EleoncoreUpdateFeaturesDto eleoncoreUpdateFeaturesDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementFeaturesUpdateExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FeaturesApi(config);
            var providerName = "providerName_example";  // string |  (optional) 
            var providerKey = "providerKey_example";  // string |  (optional) 
            var eleoncoreUpdateFeaturesDto = new EleoncoreUpdateFeaturesDto(); // EleoncoreUpdateFeaturesDto |  (optional) 

            try
            {
                apiInstance.TenantManagementFeaturesUpdate(providerName, providerKey, eleoncoreUpdateFeaturesDto);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FeaturesApi.TenantManagementFeaturesUpdate: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementFeaturesUpdateWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.TenantManagementFeaturesUpdateWithHttpInfo(providerName, providerKey, eleoncoreUpdateFeaturesDto);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FeaturesApi.TenantManagementFeaturesUpdateWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **providerName** | **string** |  | [optional]  |
| **providerKey** | **string** |  | [optional]  |
| **eleoncoreUpdateFeaturesDto** | [**EleoncoreUpdateFeaturesDto**](EleoncoreUpdateFeaturesDto.md) |  | [optional]  |

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

