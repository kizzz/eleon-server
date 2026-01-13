# EleonsoftProxy.Api.TenantLocalizationApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**LanguageManagementTenantLocalizationGetTenantLanguage**](TenantLocalizationApi.md#languagemanagementtenantlocalizationgettenantlanguage) | **GET** /api/LanguageManagement/TenantLocalization/GetTenantLanguage |  |
| [**LanguageManagementTenantLocalizationSetTenantLanguage**](TenantLocalizationApi.md#languagemanagementtenantlocalizationsettenantlanguage) | **POST** /api/LanguageManagement/TenantLocalization/SetTenantLanguage |  |

<a id="languagemanagementtenantlocalizationgettenantlanguage"></a>
# **LanguageManagementTenantLocalizationGetTenantLanguage**
> TenantManagementLanguageEntryDto LanguageManagementTenantLocalizationGetTenantLanguage ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class LanguageManagementTenantLocalizationGetTenantLanguageExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new TenantLocalizationApi(config);

            try
            {
                TenantManagementLanguageEntryDto result = apiInstance.LanguageManagementTenantLocalizationGetTenantLanguage();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TenantLocalizationApi.LanguageManagementTenantLocalizationGetTenantLanguage: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the LanguageManagementTenantLocalizationGetTenantLanguageWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<TenantManagementLanguageEntryDto> response = apiInstance.LanguageManagementTenantLocalizationGetTenantLanguageWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TenantLocalizationApi.LanguageManagementTenantLocalizationGetTenantLanguageWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**TenantManagementLanguageEntryDto**](TenantManagementLanguageEntryDto.md)

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

<a id="languagemanagementtenantlocalizationsettenantlanguage"></a>
# **LanguageManagementTenantLocalizationSetTenantLanguage**
> void LanguageManagementTenantLocalizationSetTenantLanguage (string cultureName = null, string uiCultureName = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class LanguageManagementTenantLocalizationSetTenantLanguageExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new TenantLocalizationApi(config);
            var cultureName = "cultureName_example";  // string |  (optional) 
            var uiCultureName = "uiCultureName_example";  // string |  (optional) 

            try
            {
                apiInstance.LanguageManagementTenantLocalizationSetTenantLanguage(cultureName, uiCultureName);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TenantLocalizationApi.LanguageManagementTenantLocalizationSetTenantLanguage: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the LanguageManagementTenantLocalizationSetTenantLanguageWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.LanguageManagementTenantLocalizationSetTenantLanguageWithHttpInfo(cultureName, uiCultureName);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TenantLocalizationApi.LanguageManagementTenantLocalizationSetTenantLanguageWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **cultureName** | **string** |  | [optional]  |
| **uiCultureName** | **string** |  | [optional]  |

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

