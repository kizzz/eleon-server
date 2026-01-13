# EleonsoftProxy.Api.AuditApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ModuleCollectorAuditCreate**](AuditApi.md#modulecollectorauditcreate) | **POST** /api/Auditor/Audit/Create |  |
| [**ModuleCollectorAuditGet**](AuditApi.md#modulecollectorauditget) | **GET** /api/Auditor/Audit/Get |  |
| [**ModuleCollectorAuditGetCurrentVersion**](AuditApi.md#modulecollectorauditgetcurrentversion) | **GET** /api/Auditor/Audit/GetCurrentVersion |  |
| [**ModuleCollectorAuditIncrementAuditVersion**](AuditApi.md#modulecollectorauditincrementauditversion) | **POST** /api/Auditor/Audit/IncrementVersion |  |

<a id="modulecollectorauditcreate"></a>
# **ModuleCollectorAuditCreate**
> bool ModuleCollectorAuditCreate (ModuleCollectorCreateAuditDto moduleCollectorCreateAuditDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class ModuleCollectorAuditCreateExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new AuditApi(config);
            var moduleCollectorCreateAuditDto = new ModuleCollectorCreateAuditDto(); // ModuleCollectorCreateAuditDto |  (optional) 

            try
            {
                bool result = apiInstance.ModuleCollectorAuditCreate(moduleCollectorCreateAuditDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling AuditApi.ModuleCollectorAuditCreate: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ModuleCollectorAuditCreateWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.ModuleCollectorAuditCreateWithHttpInfo(moduleCollectorCreateAuditDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling AuditApi.ModuleCollectorAuditCreateWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **moduleCollectorCreateAuditDto** | [**ModuleCollectorCreateAuditDto**](ModuleCollectorCreateAuditDto.md) |  | [optional]  |

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

<a id="modulecollectorauditget"></a>
# **ModuleCollectorAuditGet**
> ModuleCollectorAuditDto ModuleCollectorAuditGet (string auditedDocumentObjectType = null, string auditedDocumentId = null, string version = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class ModuleCollectorAuditGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new AuditApi(config);
            var auditedDocumentObjectType = "auditedDocumentObjectType_example";  // string |  (optional) 
            var auditedDocumentId = "auditedDocumentId_example";  // string |  (optional) 
            var version = "version_example";  // string |  (optional) 

            try
            {
                ModuleCollectorAuditDto result = apiInstance.ModuleCollectorAuditGet(auditedDocumentObjectType, auditedDocumentId, version);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling AuditApi.ModuleCollectorAuditGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ModuleCollectorAuditGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<ModuleCollectorAuditDto> response = apiInstance.ModuleCollectorAuditGetWithHttpInfo(auditedDocumentObjectType, auditedDocumentId, version);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling AuditApi.ModuleCollectorAuditGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **auditedDocumentObjectType** | **string** |  | [optional]  |
| **auditedDocumentId** | **string** |  | [optional]  |
| **version** | **string** |  | [optional]  |

### Return type

[**ModuleCollectorAuditDto**](ModuleCollectorAuditDto.md)

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

<a id="modulecollectorauditgetcurrentversion"></a>
# **ModuleCollectorAuditGetCurrentVersion**
> EleoncoreDocumentVersionEntity ModuleCollectorAuditGetCurrentVersion (string refDocumentObjectType = null, string refDocumentId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class ModuleCollectorAuditGetCurrentVersionExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new AuditApi(config);
            var refDocumentObjectType = "refDocumentObjectType_example";  // string |  (optional) 
            var refDocumentId = "refDocumentId_example";  // string |  (optional) 

            try
            {
                EleoncoreDocumentVersionEntity result = apiInstance.ModuleCollectorAuditGetCurrentVersion(refDocumentObjectType, refDocumentId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling AuditApi.ModuleCollectorAuditGetCurrentVersion: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ModuleCollectorAuditGetCurrentVersionWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleoncoreDocumentVersionEntity> response = apiInstance.ModuleCollectorAuditGetCurrentVersionWithHttpInfo(refDocumentObjectType, refDocumentId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling AuditApi.ModuleCollectorAuditGetCurrentVersionWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **refDocumentObjectType** | **string** |  | [optional]  |
| **refDocumentId** | **string** |  | [optional]  |

### Return type

[**EleoncoreDocumentVersionEntity**](EleoncoreDocumentVersionEntity.md)

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

<a id="modulecollectorauditincrementauditversion"></a>
# **ModuleCollectorAuditIncrementAuditVersion**
> ModuleCollectorIncrementVersionResultDto ModuleCollectorAuditIncrementAuditVersion (ModuleCollectorIncrementVersionRequestDto moduleCollectorIncrementVersionRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class ModuleCollectorAuditIncrementAuditVersionExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new AuditApi(config);
            var moduleCollectorIncrementVersionRequestDto = new ModuleCollectorIncrementVersionRequestDto(); // ModuleCollectorIncrementVersionRequestDto |  (optional) 

            try
            {
                ModuleCollectorIncrementVersionResultDto result = apiInstance.ModuleCollectorAuditIncrementAuditVersion(moduleCollectorIncrementVersionRequestDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling AuditApi.ModuleCollectorAuditIncrementAuditVersion: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ModuleCollectorAuditIncrementAuditVersionWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<ModuleCollectorIncrementVersionResultDto> response = apiInstance.ModuleCollectorAuditIncrementAuditVersionWithHttpInfo(moduleCollectorIncrementVersionRequestDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling AuditApi.ModuleCollectorAuditIncrementAuditVersionWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **moduleCollectorIncrementVersionRequestDto** | [**ModuleCollectorIncrementVersionRequestDto**](ModuleCollectorIncrementVersionRequestDto.md) |  | [optional]  |

### Return type

[**ModuleCollectorIncrementVersionResultDto**](ModuleCollectorIncrementVersionResultDto.md)

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

