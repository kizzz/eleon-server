# EleonsoftProxy.Api.UserProfilePictureApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**TenantManagementUserProfilePictureSetUserProfilePicture**](UserProfilePictureApi.md#tenantmanagementuserprofilepicturesetuserprofilepicture) | **POST** /api/CoreInfrastructure/UserProfilePicture/SetUserProfilePicture |  |

<a id="tenantmanagementuserprofilepicturesetuserprofilepicture"></a>
# **TenantManagementUserProfilePictureSetUserProfilePicture**
> bool TenantManagementUserProfilePictureSetUserProfilePicture (TenantManagementSetUserProfilePictureRequest tenantManagementSetUserProfilePictureRequest = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementUserProfilePictureSetUserProfilePictureExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new UserProfilePictureApi(config);
            var tenantManagementSetUserProfilePictureRequest = new TenantManagementSetUserProfilePictureRequest(); // TenantManagementSetUserProfilePictureRequest |  (optional) 

            try
            {
                bool result = apiInstance.TenantManagementUserProfilePictureSetUserProfilePicture(tenantManagementSetUserProfilePictureRequest);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling UserProfilePictureApi.TenantManagementUserProfilePictureSetUserProfilePicture: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementUserProfilePictureSetUserProfilePictureWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.TenantManagementUserProfilePictureSetUserProfilePictureWithHttpInfo(tenantManagementSetUserProfilePictureRequest);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling UserProfilePictureApi.TenantManagementUserProfilePictureSetUserProfilePictureWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **tenantManagementSetUserProfilePictureRequest** | [**TenantManagementSetUserProfilePictureRequest**](TenantManagementSetUserProfilePictureRequest.md) |  | [optional]  |

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

