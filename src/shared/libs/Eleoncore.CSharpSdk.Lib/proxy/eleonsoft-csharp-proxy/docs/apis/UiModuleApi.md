# EleonsoftProxy.Api.UiModuleApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**SitesManagementUiModuleCreate**](UiModuleApi.md#sitesmanagementuimodulecreate) | **POST** /api/CoreInfrastructure/UiModules/CreateAsync |  |
| [**SitesManagementUiModuleDelete**](UiModuleApi.md#sitesmanagementuimoduledelete) | **DELETE** /api/CoreInfrastructure/UiModules/{id} |  |
| [**SitesManagementUiModuleGet**](UiModuleApi.md#sitesmanagementuimoduleget) | **GET** /api/CoreInfrastructure/UiModules/{id} |  |
| [**SitesManagementUiModuleGetAll**](UiModuleApi.md#sitesmanagementuimodulegetall) | **GET** /api/CoreInfrastructure/UiModules/GetAllAsync |  |
| [**SitesManagementUiModuleGetEnabledModules**](UiModuleApi.md#sitesmanagementuimodulegetenabledmodules) | **GET** /api/CoreInfrastructure/UiModules/enabled-modules |  |
| [**SitesManagementUiModuleGetModulesByApplication**](UiModuleApi.md#sitesmanagementuimodulegetmodulesbyapplication) | **GET** /api/CoreInfrastructure/UiModules/application/{applicationId} |  |
| [**SitesManagementUiModuleUpdate**](UiModuleApi.md#sitesmanagementuimoduleupdate) | **PUT** /api/CoreInfrastructure/UiModules/{id} |  |

<a id="sitesmanagementuimodulecreate"></a>
# **SitesManagementUiModuleCreate**
> SitesManagementEleoncoreModuleDto SitesManagementUiModuleCreate (SitesManagementEleoncoreModuleDto sitesManagementEleoncoreModuleDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class SitesManagementUiModuleCreateExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new UiModuleApi(config);
            var sitesManagementEleoncoreModuleDto = new SitesManagementEleoncoreModuleDto(); // SitesManagementEleoncoreModuleDto |  (optional) 

            try
            {
                SitesManagementEleoncoreModuleDto result = apiInstance.SitesManagementUiModuleCreate(sitesManagementEleoncoreModuleDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling UiModuleApi.SitesManagementUiModuleCreate: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementUiModuleCreateWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<SitesManagementEleoncoreModuleDto> response = apiInstance.SitesManagementUiModuleCreateWithHttpInfo(sitesManagementEleoncoreModuleDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling UiModuleApi.SitesManagementUiModuleCreateWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **sitesManagementEleoncoreModuleDto** | [**SitesManagementEleoncoreModuleDto**](SitesManagementEleoncoreModuleDto.md) |  | [optional]  |

### Return type

[**SitesManagementEleoncoreModuleDto**](SitesManagementEleoncoreModuleDto.md)

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

<a id="sitesmanagementuimoduledelete"></a>
# **SitesManagementUiModuleDelete**
> void SitesManagementUiModuleDelete (Guid id)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class SitesManagementUiModuleDeleteExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new UiModuleApi(config);
            var id = "id_example";  // Guid | 

            try
            {
                apiInstance.SitesManagementUiModuleDelete(id);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling UiModuleApi.SitesManagementUiModuleDelete: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementUiModuleDeleteWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.SitesManagementUiModuleDeleteWithHttpInfo(id);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling UiModuleApi.SitesManagementUiModuleDeleteWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **Guid** |  |  |

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

<a id="sitesmanagementuimoduleget"></a>
# **SitesManagementUiModuleGet**
> SitesManagementEleoncoreModuleDto SitesManagementUiModuleGet (Guid id)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class SitesManagementUiModuleGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new UiModuleApi(config);
            var id = "id_example";  // Guid | 

            try
            {
                SitesManagementEleoncoreModuleDto result = apiInstance.SitesManagementUiModuleGet(id);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling UiModuleApi.SitesManagementUiModuleGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementUiModuleGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<SitesManagementEleoncoreModuleDto> response = apiInstance.SitesManagementUiModuleGetWithHttpInfo(id);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling UiModuleApi.SitesManagementUiModuleGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **Guid** |  |  |

### Return type

[**SitesManagementEleoncoreModuleDto**](SitesManagementEleoncoreModuleDto.md)

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

<a id="sitesmanagementuimodulegetall"></a>
# **SitesManagementUiModuleGetAll**
> List&lt;SitesManagementEleoncoreModuleDto&gt; SitesManagementUiModuleGetAll ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class SitesManagementUiModuleGetAllExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new UiModuleApi(config);

            try
            {
                List<SitesManagementEleoncoreModuleDto> result = apiInstance.SitesManagementUiModuleGetAll();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling UiModuleApi.SitesManagementUiModuleGetAll: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementUiModuleGetAllWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<SitesManagementEleoncoreModuleDto>> response = apiInstance.SitesManagementUiModuleGetAllWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling UiModuleApi.SitesManagementUiModuleGetAllWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**List&lt;SitesManagementEleoncoreModuleDto&gt;**](SitesManagementEleoncoreModuleDto.md)

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

<a id="sitesmanagementuimodulegetenabledmodules"></a>
# **SitesManagementUiModuleGetEnabledModules**
> List&lt;SitesManagementEleoncoreModuleDto&gt; SitesManagementUiModuleGetEnabledModules ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class SitesManagementUiModuleGetEnabledModulesExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new UiModuleApi(config);

            try
            {
                List<SitesManagementEleoncoreModuleDto> result = apiInstance.SitesManagementUiModuleGetEnabledModules();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling UiModuleApi.SitesManagementUiModuleGetEnabledModules: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementUiModuleGetEnabledModulesWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<SitesManagementEleoncoreModuleDto>> response = apiInstance.SitesManagementUiModuleGetEnabledModulesWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling UiModuleApi.SitesManagementUiModuleGetEnabledModulesWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**List&lt;SitesManagementEleoncoreModuleDto&gt;**](SitesManagementEleoncoreModuleDto.md)

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

<a id="sitesmanagementuimodulegetmodulesbyapplication"></a>
# **SitesManagementUiModuleGetModulesByApplication**
> List&lt;SitesManagementEleoncoreModuleDto&gt; SitesManagementUiModuleGetModulesByApplication (Guid applicationId)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class SitesManagementUiModuleGetModulesByApplicationExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new UiModuleApi(config);
            var applicationId = "applicationId_example";  // Guid | 

            try
            {
                List<SitesManagementEleoncoreModuleDto> result = apiInstance.SitesManagementUiModuleGetModulesByApplication(applicationId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling UiModuleApi.SitesManagementUiModuleGetModulesByApplication: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementUiModuleGetModulesByApplicationWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<SitesManagementEleoncoreModuleDto>> response = apiInstance.SitesManagementUiModuleGetModulesByApplicationWithHttpInfo(applicationId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling UiModuleApi.SitesManagementUiModuleGetModulesByApplicationWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **applicationId** | **Guid** |  |  |

### Return type

[**List&lt;SitesManagementEleoncoreModuleDto&gt;**](SitesManagementEleoncoreModuleDto.md)

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

<a id="sitesmanagementuimoduleupdate"></a>
# **SitesManagementUiModuleUpdate**
> SitesManagementEleoncoreModuleDto SitesManagementUiModuleUpdate (Guid id, SitesManagementEleoncoreModuleDto sitesManagementEleoncoreModuleDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class SitesManagementUiModuleUpdateExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new UiModuleApi(config);
            var id = "id_example";  // Guid | 
            var sitesManagementEleoncoreModuleDto = new SitesManagementEleoncoreModuleDto(); // SitesManagementEleoncoreModuleDto |  (optional) 

            try
            {
                SitesManagementEleoncoreModuleDto result = apiInstance.SitesManagementUiModuleUpdate(id, sitesManagementEleoncoreModuleDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling UiModuleApi.SitesManagementUiModuleUpdate: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementUiModuleUpdateWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<SitesManagementEleoncoreModuleDto> response = apiInstance.SitesManagementUiModuleUpdateWithHttpInfo(id, sitesManagementEleoncoreModuleDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling UiModuleApi.SitesManagementUiModuleUpdateWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **Guid** |  |  |
| **sitesManagementEleoncoreModuleDto** | [**SitesManagementEleoncoreModuleDto**](SitesManagementEleoncoreModuleDto.md) |  | [optional]  |

### Return type

[**SitesManagementEleoncoreModuleDto**](SitesManagementEleoncoreModuleDto.md)

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

