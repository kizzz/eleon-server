# EleoncoreProxy.Api.AbpApiDefinitionApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**AbpAbpApiDefinitionGet**](AbpApiDefinitionApi.md#abpabpapidefinitionget) | **GET** /api/abp/api-definition |  |

<a id="abpabpapidefinitionget"></a>
# **AbpAbpApiDefinitionGet**
> EleoncoreApplicationApiDescriptionModel AbpAbpApiDefinitionGet (bool includeTypes = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class AbpAbpApiDefinitionGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new AbpApiDefinitionApi(config);
            var includeTypes = true;  // bool |  (optional) 

            try
            {
                EleoncoreApplicationApiDescriptionModel result = apiInstance.AbpAbpApiDefinitionGet(includeTypes);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling AbpApiDefinitionApi.AbpAbpApiDefinitionGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the AbpAbpApiDefinitionGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleoncoreApplicationApiDescriptionModel> response = apiInstance.AbpAbpApiDefinitionGetWithHttpInfo(includeTypes);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling AbpApiDefinitionApi.AbpAbpApiDefinitionGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **includeTypes** | **bool** |  | [optional]  |

### Return type

[**EleoncoreApplicationApiDescriptionModel**](EleoncoreApplicationApiDescriptionModel.md)

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

