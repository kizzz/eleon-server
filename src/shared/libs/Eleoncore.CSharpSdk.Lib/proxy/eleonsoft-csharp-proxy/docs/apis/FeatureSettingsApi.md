# EleonsoftProxy.Api.FeatureSettingsApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**CoreFeatureSettingsGet**](FeatureSettingsApi.md#corefeaturesettingsget) | **GET** /api/Infrastructure/FeatureSettings/Get |  |
| [**CoreFeatureSettingsSet**](FeatureSettingsApi.md#corefeaturesettingsset) | **POST** /api/Infrastructure/FeatureSettings/Set |  |

<a id="corefeaturesettingsget"></a>
# **CoreFeatureSettingsGet**
> CoreFeatureSettingDto CoreFeatureSettingsGet (string group = null, string key = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreFeatureSettingsGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FeatureSettingsApi(config);
            var group = "group_example";  // string |  (optional) 
            var key = "key_example";  // string |  (optional) 

            try
            {
                CoreFeatureSettingDto result = apiInstance.CoreFeatureSettingsGet(group, key);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FeatureSettingsApi.CoreFeatureSettingsGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreFeatureSettingsGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<CoreFeatureSettingDto> response = apiInstance.CoreFeatureSettingsGetWithHttpInfo(group, key);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FeatureSettingsApi.CoreFeatureSettingsGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **group** | **string** |  | [optional]  |
| **key** | **string** |  | [optional]  |

### Return type

[**CoreFeatureSettingDto**](CoreFeatureSettingDto.md)

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

<a id="corefeaturesettingsset"></a>
# **CoreFeatureSettingsSet**
> List&lt;CoreFeatureSettingDto&gt; CoreFeatureSettingsSet (List<CoreSetFeatureSettingDto> coreSetFeatureSettingDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreFeatureSettingsSetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FeatureSettingsApi(config);
            var coreSetFeatureSettingDto = new List<CoreSetFeatureSettingDto>(); // List<CoreSetFeatureSettingDto> |  (optional) 

            try
            {
                List<CoreFeatureSettingDto> result = apiInstance.CoreFeatureSettingsSet(coreSetFeatureSettingDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FeatureSettingsApi.CoreFeatureSettingsSet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreFeatureSettingsSetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<CoreFeatureSettingDto>> response = apiInstance.CoreFeatureSettingsSetWithHttpInfo(coreSetFeatureSettingDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FeatureSettingsApi.CoreFeatureSettingsSetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **coreSetFeatureSettingDto** | [**List&lt;CoreSetFeatureSettingDto&gt;**](CoreSetFeatureSettingDto.md) |  | [optional]  |

### Return type

[**List&lt;CoreFeatureSettingDto&gt;**](CoreFeatureSettingDto.md)

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

