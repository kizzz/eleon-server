# EleonsoftProxy.Api.FeaturePermissionListApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**TenantManagementFeaturePermissionListGet**](FeaturePermissionListApi.md#tenantmanagementfeaturepermissionlistget) | **GET** /api/CoreInfrastructure/FeaturePermissionList/GetAsync |  |

<a id="tenantmanagementfeaturepermissionlistget"></a>
# **TenantManagementFeaturePermissionListGet**
> TenantManagementFeaturePermissionListResultDto TenantManagementFeaturePermissionListGet (string providerName = null, string providerKey = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementFeaturePermissionListGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FeaturePermissionListApi(config);
            var providerName = "providerName_example";  // string |  (optional) 
            var providerKey = "providerKey_example";  // string |  (optional) 

            try
            {
                TenantManagementFeaturePermissionListResultDto result = apiInstance.TenantManagementFeaturePermissionListGet(providerName, providerKey);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FeaturePermissionListApi.TenantManagementFeaturePermissionListGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementFeaturePermissionListGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<TenantManagementFeaturePermissionListResultDto> response = apiInstance.TenantManagementFeaturePermissionListGetWithHttpInfo(providerName, providerKey);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FeaturePermissionListApi.TenantManagementFeaturePermissionListGetWithHttpInfo: " + e.Message);
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

[**TenantManagementFeaturePermissionListResultDto**](TenantManagementFeaturePermissionListResultDto.md)

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

