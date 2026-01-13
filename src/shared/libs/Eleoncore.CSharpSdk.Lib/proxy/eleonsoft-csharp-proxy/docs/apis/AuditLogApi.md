# EleonsoftProxy.Api.AuditLogApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**InfrastructureAuditLogAddAudit**](AuditLogApi.md#infrastructureauditlogaddaudit) | **POST** /api/auditLog/auditLogs/AddAudit |  |
| [**InfrastructureAuditLogGetAuditLogById**](AuditLogApi.md#infrastructureauditloggetauditlogbyid) | **GET** /api/auditLog/auditLogs/GetAuditLogById |  |
| [**InfrastructureAuditLogGetAuditLogList**](AuditLogApi.md#infrastructureauditloggetauditloglist) | **POST** /api/auditLog/auditLogs/GetAuditLogList |  |
| [**InfrastructureAuditLogGetEntityChangeById**](AuditLogApi.md#infrastructureauditloggetentitychangebyid) | **GET** /api/auditLog/auditLogs/GetEntityChangeById |  |
| [**InfrastructureAuditLogGetEntityChangeList**](AuditLogApi.md#infrastructureauditloggetentitychangelist) | **POST** /api/auditLog/auditLogs/GetEntityChangeList |  |

<a id="infrastructureauditlogaddaudit"></a>
# **InfrastructureAuditLogAddAudit**
> bool InfrastructureAuditLogAddAudit (EleoncoreAuditLogDto eleoncoreAuditLogDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class InfrastructureAuditLogAddAuditExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new AuditLogApi(config);
            var eleoncoreAuditLogDto = new EleoncoreAuditLogDto(); // EleoncoreAuditLogDto |  (optional) 

            try
            {
                bool result = apiInstance.InfrastructureAuditLogAddAudit(eleoncoreAuditLogDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling AuditLogApi.InfrastructureAuditLogAddAudit: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the InfrastructureAuditLogAddAuditWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.InfrastructureAuditLogAddAuditWithHttpInfo(eleoncoreAuditLogDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling AuditLogApi.InfrastructureAuditLogAddAuditWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **eleoncoreAuditLogDto** | [**EleoncoreAuditLogDto**](EleoncoreAuditLogDto.md) |  | [optional]  |

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

<a id="infrastructureauditloggetauditlogbyid"></a>
# **InfrastructureAuditLogGetAuditLogById**
> EleoncoreAuditLogDto InfrastructureAuditLogGetAuditLogById (Guid id = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class InfrastructureAuditLogGetAuditLogByIdExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new AuditLogApi(config);
            var id = "id_example";  // Guid |  (optional) 

            try
            {
                EleoncoreAuditLogDto result = apiInstance.InfrastructureAuditLogGetAuditLogById(id);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling AuditLogApi.InfrastructureAuditLogGetAuditLogById: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the InfrastructureAuditLogGetAuditLogByIdWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleoncoreAuditLogDto> response = apiInstance.InfrastructureAuditLogGetAuditLogByIdWithHttpInfo(id);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling AuditLogApi.InfrastructureAuditLogGetAuditLogByIdWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **Guid** |  | [optional]  |

### Return type

[**EleoncoreAuditLogDto**](EleoncoreAuditLogDto.md)

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

<a id="infrastructureauditloggetauditloglist"></a>
# **InfrastructureAuditLogGetAuditLogList**
> EleoncorePagedResultDtoOfEleoncoreAuditLogHeaderDto InfrastructureAuditLogGetAuditLogList (EleoncoreAuditLogListRequestDto eleoncoreAuditLogListRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class InfrastructureAuditLogGetAuditLogListExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new AuditLogApi(config);
            var eleoncoreAuditLogListRequestDto = new EleoncoreAuditLogListRequestDto(); // EleoncoreAuditLogListRequestDto |  (optional) 

            try
            {
                EleoncorePagedResultDtoOfEleoncoreAuditLogHeaderDto result = apiInstance.InfrastructureAuditLogGetAuditLogList(eleoncoreAuditLogListRequestDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling AuditLogApi.InfrastructureAuditLogGetAuditLogList: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the InfrastructureAuditLogGetAuditLogListWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleoncorePagedResultDtoOfEleoncoreAuditLogHeaderDto> response = apiInstance.InfrastructureAuditLogGetAuditLogListWithHttpInfo(eleoncoreAuditLogListRequestDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling AuditLogApi.InfrastructureAuditLogGetAuditLogListWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **eleoncoreAuditLogListRequestDto** | [**EleoncoreAuditLogListRequestDto**](EleoncoreAuditLogListRequestDto.md) |  | [optional]  |

### Return type

[**EleoncorePagedResultDtoOfEleoncoreAuditLogHeaderDto**](EleoncorePagedResultDtoOfEleoncoreAuditLogHeaderDto.md)

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

<a id="infrastructureauditloggetentitychangebyid"></a>
# **InfrastructureAuditLogGetEntityChangeById**
> EleoncoreEntityChangeDto InfrastructureAuditLogGetEntityChangeById (Guid id = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class InfrastructureAuditLogGetEntityChangeByIdExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new AuditLogApi(config);
            var id = "id_example";  // Guid |  (optional) 

            try
            {
                EleoncoreEntityChangeDto result = apiInstance.InfrastructureAuditLogGetEntityChangeById(id);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling AuditLogApi.InfrastructureAuditLogGetEntityChangeById: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the InfrastructureAuditLogGetEntityChangeByIdWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleoncoreEntityChangeDto> response = apiInstance.InfrastructureAuditLogGetEntityChangeByIdWithHttpInfo(id);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling AuditLogApi.InfrastructureAuditLogGetEntityChangeByIdWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **Guid** |  | [optional]  |

### Return type

[**EleoncoreEntityChangeDto**](EleoncoreEntityChangeDto.md)

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

<a id="infrastructureauditloggetentitychangelist"></a>
# **InfrastructureAuditLogGetEntityChangeList**
> EleoncorePagedResultDtoOfEleoncoreEntityChangeDto InfrastructureAuditLogGetEntityChangeList (EleoncoreEntityChangeListRequestDto eleoncoreEntityChangeListRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class InfrastructureAuditLogGetEntityChangeListExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new AuditLogApi(config);
            var eleoncoreEntityChangeListRequestDto = new EleoncoreEntityChangeListRequestDto(); // EleoncoreEntityChangeListRequestDto |  (optional) 

            try
            {
                EleoncorePagedResultDtoOfEleoncoreEntityChangeDto result = apiInstance.InfrastructureAuditLogGetEntityChangeList(eleoncoreEntityChangeListRequestDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling AuditLogApi.InfrastructureAuditLogGetEntityChangeList: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the InfrastructureAuditLogGetEntityChangeListWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleoncorePagedResultDtoOfEleoncoreEntityChangeDto> response = apiInstance.InfrastructureAuditLogGetEntityChangeListWithHttpInfo(eleoncoreEntityChangeListRequestDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling AuditLogApi.InfrastructureAuditLogGetEntityChangeListWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **eleoncoreEntityChangeListRequestDto** | [**EleoncoreEntityChangeListRequestDto**](EleoncoreEntityChangeListRequestDto.md) |  | [optional]  |

### Return type

[**EleoncorePagedResultDtoOfEleoncoreEntityChangeDto**](EleoncorePagedResultDtoOfEleoncoreEntityChangeDto.md)

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

