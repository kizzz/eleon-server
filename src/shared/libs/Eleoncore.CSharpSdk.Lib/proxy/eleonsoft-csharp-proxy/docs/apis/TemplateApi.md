# EleonsoftProxy.Api.TemplateApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**EleonTemplateApplyTemplate**](TemplateApi.md#eleontemplateapplytemplate) | **POST** /api/templating/templates/Apply |  |
| [**EleonTemplateCreate**](TemplateApi.md#eleontemplatecreate) | **POST** /api/templating/templates/Create |  |
| [**EleonTemplateDelete**](TemplateApi.md#eleontemplatedelete) | **DELETE** /api/templating/templates/Delete |  |
| [**EleonTemplateGet**](TemplateApi.md#eleontemplateget) | **GET** /api/templating/templates/GetById |  |
| [**EleonTemplateGetByNameAndType**](TemplateApi.md#eleontemplategetbynameandtype) | **GET** /api/templating/templates/GetByNameAndType |  |
| [**EleonTemplateGetList**](TemplateApi.md#eleontemplategetlist) | **GET** /api/templating/templates/GetList |  |
| [**EleonTemplateReset**](TemplateApi.md#eleontemplatereset) | **POST** /api/templating/templates/Reset |  |
| [**EleonTemplateUpdate**](TemplateApi.md#eleontemplateupdate) | **PUT** /api/templating/templates/Update |  |

<a id="eleontemplateapplytemplate"></a>
# **EleonTemplateApplyTemplate**
> string EleonTemplateApplyTemplate (EleonApplyTemplateInput eleonApplyTemplateInput = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class EleonTemplateApplyTemplateExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new TemplateApi(config);
            var eleonApplyTemplateInput = new EleonApplyTemplateInput(); // EleonApplyTemplateInput |  (optional) 

            try
            {
                string result = apiInstance.EleonTemplateApplyTemplate(eleonApplyTemplateInput);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TemplateApi.EleonTemplateApplyTemplate: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the EleonTemplateApplyTemplateWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<string> response = apiInstance.EleonTemplateApplyTemplateWithHttpInfo(eleonApplyTemplateInput);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TemplateApi.EleonTemplateApplyTemplateWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **eleonApplyTemplateInput** | [**EleonApplyTemplateInput**](EleonApplyTemplateInput.md) |  | [optional]  |

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

<a id="eleontemplatecreate"></a>
# **EleonTemplateCreate**
> EleonTemplateDto EleonTemplateCreate (EleonCreateUpdateTemplateDto eleonCreateUpdateTemplateDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class EleonTemplateCreateExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new TemplateApi(config);
            var eleonCreateUpdateTemplateDto = new EleonCreateUpdateTemplateDto(); // EleonCreateUpdateTemplateDto |  (optional) 

            try
            {
                EleonTemplateDto result = apiInstance.EleonTemplateCreate(eleonCreateUpdateTemplateDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TemplateApi.EleonTemplateCreate: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the EleonTemplateCreateWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleonTemplateDto> response = apiInstance.EleonTemplateCreateWithHttpInfo(eleonCreateUpdateTemplateDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TemplateApi.EleonTemplateCreateWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **eleonCreateUpdateTemplateDto** | [**EleonCreateUpdateTemplateDto**](EleonCreateUpdateTemplateDto.md) |  | [optional]  |

### Return type

[**EleonTemplateDto**](EleonTemplateDto.md)

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

<a id="eleontemplatedelete"></a>
# **EleonTemplateDelete**
> void EleonTemplateDelete (Guid id = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class EleonTemplateDeleteExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new TemplateApi(config);
            var id = "id_example";  // Guid |  (optional) 

            try
            {
                apiInstance.EleonTemplateDelete(id);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TemplateApi.EleonTemplateDelete: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the EleonTemplateDeleteWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.EleonTemplateDeleteWithHttpInfo(id);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TemplateApi.EleonTemplateDeleteWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **Guid** |  | [optional]  |

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

<a id="eleontemplateget"></a>
# **EleonTemplateGet**
> EleonTemplateDto EleonTemplateGet (Guid id = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class EleonTemplateGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new TemplateApi(config);
            var id = "id_example";  // Guid |  (optional) 

            try
            {
                EleonTemplateDto result = apiInstance.EleonTemplateGet(id);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TemplateApi.EleonTemplateGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the EleonTemplateGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleonTemplateDto> response = apiInstance.EleonTemplateGetWithHttpInfo(id);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TemplateApi.EleonTemplateGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **Guid** |  | [optional]  |

### Return type

[**EleonTemplateDto**](EleonTemplateDto.md)

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

<a id="eleontemplategetbynameandtype"></a>
# **EleonTemplateGetByNameAndType**
> EleonTemplateDto EleonTemplateGetByNameAndType (string name = null, EleonTemplateType type = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class EleonTemplateGetByNameAndTypeExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new TemplateApi(config);
            var name = "name_example";  // string |  (optional) 
            var type = (EleonTemplateType) "0";  // EleonTemplateType |  (optional) 

            try
            {
                EleonTemplateDto result = apiInstance.EleonTemplateGetByNameAndType(name, type);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TemplateApi.EleonTemplateGetByNameAndType: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the EleonTemplateGetByNameAndTypeWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleonTemplateDto> response = apiInstance.EleonTemplateGetByNameAndTypeWithHttpInfo(name, type);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TemplateApi.EleonTemplateGetByNameAndTypeWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **name** | **string** |  | [optional]  |
| **type** | **EleonTemplateType** |  | [optional]  |

### Return type

[**EleonTemplateDto**](EleonTemplateDto.md)

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

<a id="eleontemplategetlist"></a>
# **EleonTemplateGetList**
> EleoncorePagedResultDtoOfEleonMinimalTemplateDto EleonTemplateGetList (string searchQuery = null, EleonTemplateType type = null, EleoncoreTextFormat format = null, bool isSystem = null, string filter = null, string sorting = null, int skipCount = null, int maxResultCount = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class EleonTemplateGetListExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new TemplateApi(config);
            var searchQuery = "searchQuery_example";  // string |  (optional) 
            var type = (EleonTemplateType) "0";  // EleonTemplateType |  (optional) 
            var format = (EleoncoreTextFormat) "0";  // EleoncoreTextFormat |  (optional) 
            var isSystem = true;  // bool |  (optional) 
            var filter = "filter_example";  // string |  (optional) 
            var sorting = "sorting_example";  // string |  (optional) 
            var skipCount = 56;  // int |  (optional) 
            var maxResultCount = 56;  // int |  (optional) 

            try
            {
                EleoncorePagedResultDtoOfEleonMinimalTemplateDto result = apiInstance.EleonTemplateGetList(searchQuery, type, format, isSystem, filter, sorting, skipCount, maxResultCount);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TemplateApi.EleonTemplateGetList: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the EleonTemplateGetListWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleoncorePagedResultDtoOfEleonMinimalTemplateDto> response = apiInstance.EleonTemplateGetListWithHttpInfo(searchQuery, type, format, isSystem, filter, sorting, skipCount, maxResultCount);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TemplateApi.EleonTemplateGetListWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **searchQuery** | **string** |  | [optional]  |
| **type** | **EleonTemplateType** |  | [optional]  |
| **format** | **EleoncoreTextFormat** |  | [optional]  |
| **isSystem** | **bool** |  | [optional]  |
| **filter** | **string** |  | [optional]  |
| **sorting** | **string** |  | [optional]  |
| **skipCount** | **int** |  | [optional]  |
| **maxResultCount** | **int** |  | [optional]  |

### Return type

[**EleoncorePagedResultDtoOfEleonMinimalTemplateDto**](EleoncorePagedResultDtoOfEleonMinimalTemplateDto.md)

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

<a id="eleontemplatereset"></a>
# **EleonTemplateReset**
> EleonTemplateDto EleonTemplateReset (Guid id = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class EleonTemplateResetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new TemplateApi(config);
            var id = "id_example";  // Guid |  (optional) 

            try
            {
                EleonTemplateDto result = apiInstance.EleonTemplateReset(id);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TemplateApi.EleonTemplateReset: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the EleonTemplateResetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleonTemplateDto> response = apiInstance.EleonTemplateResetWithHttpInfo(id);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TemplateApi.EleonTemplateResetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **Guid** |  | [optional]  |

### Return type

[**EleonTemplateDto**](EleonTemplateDto.md)

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

<a id="eleontemplateupdate"></a>
# **EleonTemplateUpdate**
> EleonTemplateDto EleonTemplateUpdate (Guid id = null, EleonCreateUpdateTemplateDto eleonCreateUpdateTemplateDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class EleonTemplateUpdateExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new TemplateApi(config);
            var id = "id_example";  // Guid |  (optional) 
            var eleonCreateUpdateTemplateDto = new EleonCreateUpdateTemplateDto(); // EleonCreateUpdateTemplateDto |  (optional) 

            try
            {
                EleonTemplateDto result = apiInstance.EleonTemplateUpdate(id, eleonCreateUpdateTemplateDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TemplateApi.EleonTemplateUpdate: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the EleonTemplateUpdateWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleonTemplateDto> response = apiInstance.EleonTemplateUpdateWithHttpInfo(id, eleonCreateUpdateTemplateDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TemplateApi.EleonTemplateUpdateWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **Guid** |  | [optional]  |
| **eleonCreateUpdateTemplateDto** | [**EleonCreateUpdateTemplateDto**](EleonCreateUpdateTemplateDto.md) |  | [optional]  |

### Return type

[**EleonTemplateDto**](EleonTemplateDto.md)

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

