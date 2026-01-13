# EleonsoftProxy.Api.LanguageApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**LanguageManagementLanguageGetDefaultLanguage**](LanguageApi.md#languagemanagementlanguagegetdefaultlanguage) | **GET** /api/LanguageManagement/Language/GetDefaultLanguage |  |
| [**LanguageManagementLanguageGetLanguageList**](LanguageApi.md#languagemanagementlanguagegetlanguagelist) | **GET** /api/LanguageManagement/Language/GetLanguageList |  |
| [**LanguageManagementLanguageSetDefaultLanguage**](LanguageApi.md#languagemanagementlanguagesetdefaultlanguage) | **POST** /api/LanguageManagement/Language/SetDefaultLanguage |  |
| [**LanguageManagementLanguageSetLanguageEnabled**](LanguageApi.md#languagemanagementlanguagesetlanguageenabled) | **POST** /api/LanguageManagement/Language/SetLanguageEnabled |  |

<a id="languagemanagementlanguagegetdefaultlanguage"></a>
# **LanguageManagementLanguageGetDefaultLanguage**
> LanguageManagementLanguageInfoDto LanguageManagementLanguageGetDefaultLanguage ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class LanguageManagementLanguageGetDefaultLanguageExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new LanguageApi(config);

            try
            {
                LanguageManagementLanguageInfoDto result = apiInstance.LanguageManagementLanguageGetDefaultLanguage();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling LanguageApi.LanguageManagementLanguageGetDefaultLanguage: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the LanguageManagementLanguageGetDefaultLanguageWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<LanguageManagementLanguageInfoDto> response = apiInstance.LanguageManagementLanguageGetDefaultLanguageWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling LanguageApi.LanguageManagementLanguageGetDefaultLanguageWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**LanguageManagementLanguageInfoDto**](LanguageManagementLanguageInfoDto.md)

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

<a id="languagemanagementlanguagegetlanguagelist"></a>
# **LanguageManagementLanguageGetLanguageList**
> List&lt;LanguageManagementLanguageDto&gt; LanguageManagementLanguageGetLanguageList ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class LanguageManagementLanguageGetLanguageListExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new LanguageApi(config);

            try
            {
                List<LanguageManagementLanguageDto> result = apiInstance.LanguageManagementLanguageGetLanguageList();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling LanguageApi.LanguageManagementLanguageGetLanguageList: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the LanguageManagementLanguageGetLanguageListWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<LanguageManagementLanguageDto>> response = apiInstance.LanguageManagementLanguageGetLanguageListWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling LanguageApi.LanguageManagementLanguageGetLanguageListWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**List&lt;LanguageManagementLanguageDto&gt;**](LanguageManagementLanguageDto.md)

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

<a id="languagemanagementlanguagesetdefaultlanguage"></a>
# **LanguageManagementLanguageSetDefaultLanguage**
> bool LanguageManagementLanguageSetDefaultLanguage (Guid languageId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class LanguageManagementLanguageSetDefaultLanguageExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new LanguageApi(config);
            var languageId = "languageId_example";  // Guid |  (optional) 

            try
            {
                bool result = apiInstance.LanguageManagementLanguageSetDefaultLanguage(languageId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling LanguageApi.LanguageManagementLanguageSetDefaultLanguage: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the LanguageManagementLanguageSetDefaultLanguageWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.LanguageManagementLanguageSetDefaultLanguageWithHttpInfo(languageId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling LanguageApi.LanguageManagementLanguageSetDefaultLanguageWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **languageId** | **Guid** |  | [optional]  |

### Return type

**bool**

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

<a id="languagemanagementlanguagesetlanguageenabled"></a>
# **LanguageManagementLanguageSetLanguageEnabled**
> bool LanguageManagementLanguageSetLanguageEnabled (LanguageManagementSetLanguageEnabledDto languageManagementSetLanguageEnabledDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class LanguageManagementLanguageSetLanguageEnabledExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new LanguageApi(config);
            var languageManagementSetLanguageEnabledDto = new LanguageManagementSetLanguageEnabledDto(); // LanguageManagementSetLanguageEnabledDto |  (optional) 

            try
            {
                bool result = apiInstance.LanguageManagementLanguageSetLanguageEnabled(languageManagementSetLanguageEnabledDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling LanguageApi.LanguageManagementLanguageSetLanguageEnabled: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the LanguageManagementLanguageSetLanguageEnabledWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.LanguageManagementLanguageSetLanguageEnabledWithHttpInfo(languageManagementSetLanguageEnabledDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling LanguageApi.LanguageManagementLanguageSetLanguageEnabledWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **languageManagementSetLanguageEnabledDto** | [**LanguageManagementSetLanguageEnabledDto**](LanguageManagementSetLanguageEnabledDto.md) |  | [optional]  |

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

