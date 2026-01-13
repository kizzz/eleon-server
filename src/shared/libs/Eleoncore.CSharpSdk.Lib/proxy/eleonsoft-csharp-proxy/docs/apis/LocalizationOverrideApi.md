# EleonsoftProxy.Api.LocalizationOverrideApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**LanguageManagementLocalizationOverrideGetLocalization**](LocalizationOverrideApi.md#languagemanagementlocalizationoverridegetlocalization) | **GET** /api/LanguageManagement/LocalizationOverride/GetLocalization |  |
| [**LanguageManagementLocalizationOverrideGetLocalizationInformation**](LocalizationOverrideApi.md#languagemanagementlocalizationoverridegetlocalizationinformation) | **GET** /api/LanguageManagement/LocalizationOverride/GetLocalizationInformation |  |
| [**LanguageManagementLocalizationOverrideGetLocalizationStrings**](LocalizationOverrideApi.md#languagemanagementlocalizationoverridegetlocalizationstrings) | **POST** /api/LanguageManagement/LocalizationOverride/GetLocalizationStrings |  |
| [**LanguageManagementLocalizationOverrideOverrideLocalizationEntry**](LocalizationOverrideApi.md#languagemanagementlocalizationoverrideoverridelocalizationentry) | **POST** /api/LanguageManagement/LocalizationOverride/OverrideLocalizationEntry |  |

<a id="languagemanagementlocalizationoverridegetlocalization"></a>
# **LanguageManagementLocalizationOverrideGetLocalization**
> LanguageManagementLocalizationDto LanguageManagementLocalizationOverrideGetLocalization (string culture = null, List<string> localizationResources = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class LanguageManagementLocalizationOverrideGetLocalizationExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new LocalizationOverrideApi(config);
            var culture = "culture_example";  // string |  (optional) 
            var localizationResources = new List<string>(); // List<string> |  (optional) 

            try
            {
                LanguageManagementLocalizationDto result = apiInstance.LanguageManagementLocalizationOverrideGetLocalization(culture, localizationResources);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling LocalizationOverrideApi.LanguageManagementLocalizationOverrideGetLocalization: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the LanguageManagementLocalizationOverrideGetLocalizationWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<LanguageManagementLocalizationDto> response = apiInstance.LanguageManagementLocalizationOverrideGetLocalizationWithHttpInfo(culture, localizationResources);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling LocalizationOverrideApi.LanguageManagementLocalizationOverrideGetLocalizationWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **culture** | **string** |  | [optional]  |
| **localizationResources** | [**List&lt;string&gt;**](string.md) |  | [optional]  |

### Return type

[**LanguageManagementLocalizationDto**](LanguageManagementLocalizationDto.md)

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

<a id="languagemanagementlocalizationoverridegetlocalizationinformation"></a>
# **LanguageManagementLocalizationOverrideGetLocalizationInformation**
> LanguageManagementLocalizationInformationDto LanguageManagementLocalizationOverrideGetLocalizationInformation ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class LanguageManagementLocalizationOverrideGetLocalizationInformationExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new LocalizationOverrideApi(config);

            try
            {
                LanguageManagementLocalizationInformationDto result = apiInstance.LanguageManagementLocalizationOverrideGetLocalizationInformation();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling LocalizationOverrideApi.LanguageManagementLocalizationOverrideGetLocalizationInformation: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the LanguageManagementLocalizationOverrideGetLocalizationInformationWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<LanguageManagementLocalizationInformationDto> response = apiInstance.LanguageManagementLocalizationOverrideGetLocalizationInformationWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling LocalizationOverrideApi.LanguageManagementLocalizationOverrideGetLocalizationInformationWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**LanguageManagementLocalizationInformationDto**](LanguageManagementLocalizationInformationDto.md)

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

<a id="languagemanagementlocalizationoverridegetlocalizationstrings"></a>
# **LanguageManagementLocalizationOverrideGetLocalizationStrings**
> EleoncorePagedResultDtoOfLanguageManagementOverriddenLocalizationStringDto LanguageManagementLocalizationOverrideGetLocalizationStrings (LanguageManagementGetLocalizationStringsRequest languageManagementGetLocalizationStringsRequest = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class LanguageManagementLocalizationOverrideGetLocalizationStringsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new LocalizationOverrideApi(config);
            var languageManagementGetLocalizationStringsRequest = new LanguageManagementGetLocalizationStringsRequest(); // LanguageManagementGetLocalizationStringsRequest |  (optional) 

            try
            {
                EleoncorePagedResultDtoOfLanguageManagementOverriddenLocalizationStringDto result = apiInstance.LanguageManagementLocalizationOverrideGetLocalizationStrings(languageManagementGetLocalizationStringsRequest);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling LocalizationOverrideApi.LanguageManagementLocalizationOverrideGetLocalizationStrings: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the LanguageManagementLocalizationOverrideGetLocalizationStringsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleoncorePagedResultDtoOfLanguageManagementOverriddenLocalizationStringDto> response = apiInstance.LanguageManagementLocalizationOverrideGetLocalizationStringsWithHttpInfo(languageManagementGetLocalizationStringsRequest);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling LocalizationOverrideApi.LanguageManagementLocalizationOverrideGetLocalizationStringsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **languageManagementGetLocalizationStringsRequest** | [**LanguageManagementGetLocalizationStringsRequest**](LanguageManagementGetLocalizationStringsRequest.md) |  | [optional]  |

### Return type

[**EleoncorePagedResultDtoOfLanguageManagementOverriddenLocalizationStringDto**](EleoncorePagedResultDtoOfLanguageManagementOverriddenLocalizationStringDto.md)

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

<a id="languagemanagementlocalizationoverrideoverridelocalizationentry"></a>
# **LanguageManagementLocalizationOverrideOverrideLocalizationEntry**
> bool LanguageManagementLocalizationOverrideOverrideLocalizationEntry (LanguageManagementOverrideLocalizationEntryRequest languageManagementOverrideLocalizationEntryRequest = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class LanguageManagementLocalizationOverrideOverrideLocalizationEntryExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new LocalizationOverrideApi(config);
            var languageManagementOverrideLocalizationEntryRequest = new LanguageManagementOverrideLocalizationEntryRequest(); // LanguageManagementOverrideLocalizationEntryRequest |  (optional) 

            try
            {
                bool result = apiInstance.LanguageManagementLocalizationOverrideOverrideLocalizationEntry(languageManagementOverrideLocalizationEntryRequest);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling LocalizationOverrideApi.LanguageManagementLocalizationOverrideOverrideLocalizationEntry: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the LanguageManagementLocalizationOverrideOverrideLocalizationEntryWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.LanguageManagementLocalizationOverrideOverrideLocalizationEntryWithHttpInfo(languageManagementOverrideLocalizationEntryRequest);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling LocalizationOverrideApi.LanguageManagementLocalizationOverrideOverrideLocalizationEntryWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **languageManagementOverrideLocalizationEntryRequest** | [**LanguageManagementOverrideLocalizationEntryRequest**](LanguageManagementOverrideLocalizationEntryRequest.md) |  | [optional]  |

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

