# EleonsoftProxy.Api.NotificatorSettingsApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**TenantManagementNotificatorSettingsGet**](NotificatorSettingsApi.md#tenantmanagementnotificatorsettingsget) | **GET** /api/TenantManagement/NotificatorSettings/Get |  |
| [**TenantManagementNotificatorSettingsSendCustomTestEmail**](NotificatorSettingsApi.md#tenantmanagementnotificatorsettingssendcustomtestemail) | **POST** /api/TenantManagement/NotificatorSettings/SendCustomTestEmail |  |
| [**TenantManagementNotificatorSettingsUpdate**](NotificatorSettingsApi.md#tenantmanagementnotificatorsettingsupdate) | **POST** /api/TenantManagement/NotificatorSettings/Update |  |
| [**TenantManagementNotificatorSettingsValidateTemplate**](NotificatorSettingsApi.md#tenantmanagementnotificatorsettingsvalidatetemplate) | **POST** /api/TenantManagement/NotificatorSettings/ValidateTemplate |  |

<a id="tenantmanagementnotificatorsettingsget"></a>
# **TenantManagementNotificatorSettingsGet**
> EleonsoftModuleCollectorNotificatorSettingsDto TenantManagementNotificatorSettingsGet ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementNotificatorSettingsGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new NotificatorSettingsApi(config);

            try
            {
                EleonsoftModuleCollectorNotificatorSettingsDto result = apiInstance.TenantManagementNotificatorSettingsGet();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling NotificatorSettingsApi.TenantManagementNotificatorSettingsGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementNotificatorSettingsGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleonsoftModuleCollectorNotificatorSettingsDto> response = apiInstance.TenantManagementNotificatorSettingsGetWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling NotificatorSettingsApi.TenantManagementNotificatorSettingsGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**EleonsoftModuleCollectorNotificatorSettingsDto**](EleonsoftModuleCollectorNotificatorSettingsDto.md)

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

<a id="tenantmanagementnotificatorsettingssendcustomtestemail"></a>
# **TenantManagementNotificatorSettingsSendCustomTestEmail**
> string TenantManagementNotificatorSettingsSendCustomTestEmail (ModuleCollectorSendTestEmailInputDto moduleCollectorSendTestEmailInputDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementNotificatorSettingsSendCustomTestEmailExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new NotificatorSettingsApi(config);
            var moduleCollectorSendTestEmailInputDto = new ModuleCollectorSendTestEmailInputDto(); // ModuleCollectorSendTestEmailInputDto |  (optional) 

            try
            {
                string result = apiInstance.TenantManagementNotificatorSettingsSendCustomTestEmail(moduleCollectorSendTestEmailInputDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling NotificatorSettingsApi.TenantManagementNotificatorSettingsSendCustomTestEmail: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementNotificatorSettingsSendCustomTestEmailWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<string> response = apiInstance.TenantManagementNotificatorSettingsSendCustomTestEmailWithHttpInfo(moduleCollectorSendTestEmailInputDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling NotificatorSettingsApi.TenantManagementNotificatorSettingsSendCustomTestEmailWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **moduleCollectorSendTestEmailInputDto** | [**ModuleCollectorSendTestEmailInputDto**](ModuleCollectorSendTestEmailInputDto.md) |  | [optional]  |

### Return type

**string**

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

<a id="tenantmanagementnotificatorsettingsupdate"></a>
# **TenantManagementNotificatorSettingsUpdate**
> void TenantManagementNotificatorSettingsUpdate (EleonsoftModuleCollectorNotificatorSettingsDto eleonsoftModuleCollectorNotificatorSettingsDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementNotificatorSettingsUpdateExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new NotificatorSettingsApi(config);
            var eleonsoftModuleCollectorNotificatorSettingsDto = new EleonsoftModuleCollectorNotificatorSettingsDto(); // EleonsoftModuleCollectorNotificatorSettingsDto |  (optional) 

            try
            {
                apiInstance.TenantManagementNotificatorSettingsUpdate(eleonsoftModuleCollectorNotificatorSettingsDto);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling NotificatorSettingsApi.TenantManagementNotificatorSettingsUpdate: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementNotificatorSettingsUpdateWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.TenantManagementNotificatorSettingsUpdateWithHttpInfo(eleonsoftModuleCollectorNotificatorSettingsDto);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling NotificatorSettingsApi.TenantManagementNotificatorSettingsUpdateWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **eleonsoftModuleCollectorNotificatorSettingsDto** | [**EleonsoftModuleCollectorNotificatorSettingsDto**](EleonsoftModuleCollectorNotificatorSettingsDto.md) |  | [optional]  |

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

<a id="tenantmanagementnotificatorsettingsvalidatetemplate"></a>
# **TenantManagementNotificatorSettingsValidateTemplate**
> List&lt;string&gt; TenantManagementNotificatorSettingsValidateTemplate (string templateType = null, string template = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementNotificatorSettingsValidateTemplateExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new NotificatorSettingsApi(config);
            var templateType = "templateType_example";  // string |  (optional) 
            var template = "template_example";  // string |  (optional) 

            try
            {
                List<string> result = apiInstance.TenantManagementNotificatorSettingsValidateTemplate(templateType, template);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling NotificatorSettingsApi.TenantManagementNotificatorSettingsValidateTemplate: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementNotificatorSettingsValidateTemplateWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<string>> response = apiInstance.TenantManagementNotificatorSettingsValidateTemplateWithHttpInfo(templateType, template);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling NotificatorSettingsApi.TenantManagementNotificatorSettingsValidateTemplateWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **templateType** | **string** |  | [optional]  |
| **template** | **string** |  | [optional]  |

### Return type

**List<string>**

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

