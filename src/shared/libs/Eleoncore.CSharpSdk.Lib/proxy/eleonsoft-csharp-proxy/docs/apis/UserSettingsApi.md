# EleonsoftProxy.Api.UserSettingsApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**CoreUserSettingsGetAppearanceSetting**](UserSettingsApi.md#coreusersettingsgetappearancesetting) | **GET** /api/CoreInfrastructure/UserSettings/GetAppearanceSetting |  |
| [**CoreUserSettingsGetCurrentUserSetting**](UserSettingsApi.md#coreusersettingsgetcurrentusersetting) | **GET** /api/CoreInfrastructure/UserSettings/GetCurrentUserSetting |  |
| [**CoreUserSettingsGetUserSetting**](UserSettingsApi.md#coreusersettingsgetusersetting) | **GET** /api/CoreInfrastructure/UserSettings/GetUserSetting |  |
| [**CoreUserSettingsGetUserSettingByUserId**](UserSettingsApi.md#coreusersettingsgetusersettingbyuserid) | **GET** /api/CoreInfrastructure/UserSettings/GetUserSettingsByUserId |  |
| [**CoreUserSettingsSetAppearanceSetting**](UserSettingsApi.md#coreusersettingssetappearancesetting) | **POST** /api/CoreInfrastructure/UserSettings/SetAppearanceSetting |  |
| [**CoreUserSettingsSetCurrentUserSetting**](UserSettingsApi.md#coreusersettingssetcurrentusersetting) | **POST** /api/CoreInfrastructure/UserSettings/SetCurrentUserSetting |  |
| [**CoreUserSettingsSetUserSetting**](UserSettingsApi.md#coreusersettingssetusersetting) | **POST** /api/CoreInfrastructure/UserSettings/SetUserSetting |  |
| [**CoreUserSettingsSetUserSettings**](UserSettingsApi.md#coreusersettingssetusersettings) | **POST** /api/CoreInfrastructure/UserSettings/SetUserSettings |  |

<a id="coreusersettingsgetappearancesetting"></a>
# **CoreUserSettingsGetAppearanceSetting**
> EleoncoreResultDtoOfEleoncoreString CoreUserSettingsGetAppearanceSetting (string appId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreUserSettingsGetAppearanceSettingExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new UserSettingsApi(config);
            var appId = "appId_example";  // string |  (optional) 

            try
            {
                EleoncoreResultDtoOfEleoncoreString result = apiInstance.CoreUserSettingsGetAppearanceSetting(appId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling UserSettingsApi.CoreUserSettingsGetAppearanceSetting: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreUserSettingsGetAppearanceSettingWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleoncoreResultDtoOfEleoncoreString> response = apiInstance.CoreUserSettingsGetAppearanceSettingWithHttpInfo(appId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling UserSettingsApi.CoreUserSettingsGetAppearanceSettingWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **appId** | **string** |  | [optional]  |

### Return type

[**EleoncoreResultDtoOfEleoncoreString**](EleoncoreResultDtoOfEleoncoreString.md)

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

<a id="coreusersettingsgetcurrentusersetting"></a>
# **CoreUserSettingsGetCurrentUserSetting**
> string CoreUserSettingsGetCurrentUserSetting (string name = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreUserSettingsGetCurrentUserSettingExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new UserSettingsApi(config);
            var name = "name_example";  // string |  (optional) 

            try
            {
                string result = apiInstance.CoreUserSettingsGetCurrentUserSetting(name);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling UserSettingsApi.CoreUserSettingsGetCurrentUserSetting: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreUserSettingsGetCurrentUserSettingWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<string> response = apiInstance.CoreUserSettingsGetCurrentUserSettingWithHttpInfo(name);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling UserSettingsApi.CoreUserSettingsGetCurrentUserSettingWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **name** | **string** |  | [optional]  |

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

<a id="coreusersettingsgetusersetting"></a>
# **CoreUserSettingsGetUserSetting**
> string CoreUserSettingsGetUserSetting (Guid userId = null, string name = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreUserSettingsGetUserSettingExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new UserSettingsApi(config);
            var userId = "userId_example";  // Guid |  (optional) 
            var name = "name_example";  // string |  (optional) 

            try
            {
                string result = apiInstance.CoreUserSettingsGetUserSetting(userId, name);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling UserSettingsApi.CoreUserSettingsGetUserSetting: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreUserSettingsGetUserSettingWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<string> response = apiInstance.CoreUserSettingsGetUserSettingWithHttpInfo(userId, name);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling UserSettingsApi.CoreUserSettingsGetUserSettingWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **userId** | **Guid** |  | [optional]  |
| **name** | **string** |  | [optional]  |

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

<a id="coreusersettingsgetusersettingbyuserid"></a>
# **CoreUserSettingsGetUserSettingByUserId**
> TenantManagementUserSettingDto CoreUserSettingsGetUserSettingByUserId (Guid userId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreUserSettingsGetUserSettingByUserIdExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new UserSettingsApi(config);
            var userId = "userId_example";  // Guid |  (optional) 

            try
            {
                TenantManagementUserSettingDto result = apiInstance.CoreUserSettingsGetUserSettingByUserId(userId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling UserSettingsApi.CoreUserSettingsGetUserSettingByUserId: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreUserSettingsGetUserSettingByUserIdWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<TenantManagementUserSettingDto> response = apiInstance.CoreUserSettingsGetUserSettingByUserIdWithHttpInfo(userId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling UserSettingsApi.CoreUserSettingsGetUserSettingByUserIdWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **userId** | **Guid** |  | [optional]  |

### Return type

[**TenantManagementUserSettingDto**](TenantManagementUserSettingDto.md)

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

<a id="coreusersettingssetappearancesetting"></a>
# **CoreUserSettingsSetAppearanceSetting**
> EleoncoreResultDtoOfEleoncoreString CoreUserSettingsSetAppearanceSetting (string appearanceSettingsDto = null, string appId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreUserSettingsSetAppearanceSettingExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new UserSettingsApi(config);
            var appearanceSettingsDto = "appearanceSettingsDto_example";  // string |  (optional) 
            var appId = "appId_example";  // string |  (optional) 

            try
            {
                EleoncoreResultDtoOfEleoncoreString result = apiInstance.CoreUserSettingsSetAppearanceSetting(appearanceSettingsDto, appId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling UserSettingsApi.CoreUserSettingsSetAppearanceSetting: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreUserSettingsSetAppearanceSettingWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleoncoreResultDtoOfEleoncoreString> response = apiInstance.CoreUserSettingsSetAppearanceSettingWithHttpInfo(appearanceSettingsDto, appId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling UserSettingsApi.CoreUserSettingsSetAppearanceSettingWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **appearanceSettingsDto** | **string** |  | [optional]  |
| **appId** | **string** |  | [optional]  |

### Return type

[**EleoncoreResultDtoOfEleoncoreString**](EleoncoreResultDtoOfEleoncoreString.md)

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

<a id="coreusersettingssetcurrentusersetting"></a>
# **CoreUserSettingsSetCurrentUserSetting**
> void CoreUserSettingsSetCurrentUserSetting (string name = null, string value = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreUserSettingsSetCurrentUserSettingExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new UserSettingsApi(config);
            var name = "name_example";  // string |  (optional) 
            var value = "value_example";  // string |  (optional) 

            try
            {
                apiInstance.CoreUserSettingsSetCurrentUserSetting(name, value);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling UserSettingsApi.CoreUserSettingsSetCurrentUserSetting: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreUserSettingsSetCurrentUserSettingWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.CoreUserSettingsSetCurrentUserSettingWithHttpInfo(name, value);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling UserSettingsApi.CoreUserSettingsSetCurrentUserSettingWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **name** | **string** |  | [optional]  |
| **value** | **string** |  | [optional]  |

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

<a id="coreusersettingssetusersetting"></a>
# **CoreUserSettingsSetUserSetting**
> void CoreUserSettingsSetUserSetting (Guid userId = null, string name = null, string value = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreUserSettingsSetUserSettingExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new UserSettingsApi(config);
            var userId = "userId_example";  // Guid |  (optional) 
            var name = "name_example";  // string |  (optional) 
            var value = "value_example";  // string |  (optional) 

            try
            {
                apiInstance.CoreUserSettingsSetUserSetting(userId, name, value);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling UserSettingsApi.CoreUserSettingsSetUserSetting: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreUserSettingsSetUserSettingWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.CoreUserSettingsSetUserSettingWithHttpInfo(userId, name, value);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling UserSettingsApi.CoreUserSettingsSetUserSettingWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **userId** | **Guid** |  | [optional]  |
| **name** | **string** |  | [optional]  |
| **value** | **string** |  | [optional]  |

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

<a id="coreusersettingssetusersettings"></a>
# **CoreUserSettingsSetUserSettings**
> TenantManagementUserSettingDto CoreUserSettingsSetUserSettings (TenantManagementUserSettingDto tenantManagementUserSettingDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreUserSettingsSetUserSettingsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new UserSettingsApi(config);
            var tenantManagementUserSettingDto = new TenantManagementUserSettingDto(); // TenantManagementUserSettingDto |  (optional) 

            try
            {
                TenantManagementUserSettingDto result = apiInstance.CoreUserSettingsSetUserSettings(tenantManagementUserSettingDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling UserSettingsApi.CoreUserSettingsSetUserSettings: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreUserSettingsSetUserSettingsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<TenantManagementUserSettingDto> response = apiInstance.CoreUserSettingsSetUserSettingsWithHttpInfo(tenantManagementUserSettingDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling UserSettingsApi.CoreUserSettingsSetUserSettingsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **tenantManagementUserSettingDto** | [**TenantManagementUserSettingDto**](TenantManagementUserSettingDto.md) |  | [optional]  |

### Return type

[**TenantManagementUserSettingDto**](TenantManagementUserSettingDto.md)

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

