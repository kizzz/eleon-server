# EleonsoftProxy.Api.HealthCheckApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**EleonsoftModuleCollectorHealthCheckAddReport**](HealthCheckApi.md#eleonsoftmodulecollectorhealthcheckaddreport) | **POST** /api/HealthChecks/HealthCheck/AddReport |  |
| [**EleonsoftModuleCollectorHealthCheckAddReportBulk**](HealthCheckApi.md#eleonsoftmodulecollectorhealthcheckaddreportbulk) | **POST** /api/HealthChecks/HealthCheck/AddReportBulk |  |
| [**EleonsoftModuleCollectorHealthCheckCreate**](HealthCheckApi.md#eleonsoftmodulecollectorhealthcheckcreate) | **POST** /api/HealthChecks/HealthCheck/Create |  |
| [**EleonsoftModuleCollectorHealthCheckGetById**](HealthCheckApi.md#eleonsoftmodulecollectorhealthcheckgetbyid) | **GET** /api/HealthChecks/HealthCheck/GetById |  |
| [**EleonsoftModuleCollectorHealthCheckGetList**](HealthCheckApi.md#eleonsoftmodulecollectorhealthcheckgetlist) | **GET** /api/HealthChecks/HealthCheck/GetList |  |
| [**EleonsoftModuleCollectorHealthCheckSend**](HealthCheckApi.md#eleonsoftmodulecollectorhealthchecksend) | **POST** /api/HealthChecks/HealthCheck/Send |  |

<a id="eleonsoftmodulecollectorhealthcheckaddreport"></a>
# **EleonsoftModuleCollectorHealthCheckAddReport**
> EleonsoftModuleCollectorHealthCheckReportDto EleonsoftModuleCollectorHealthCheckAddReport (EleonsoftModuleCollectorAddHealthCheckReportDto eleonsoftModuleCollectorAddHealthCheckReportDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class EleonsoftModuleCollectorHealthCheckAddReportExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new HealthCheckApi(config);
            var eleonsoftModuleCollectorAddHealthCheckReportDto = new EleonsoftModuleCollectorAddHealthCheckReportDto(); // EleonsoftModuleCollectorAddHealthCheckReportDto |  (optional) 

            try
            {
                EleonsoftModuleCollectorHealthCheckReportDto result = apiInstance.EleonsoftModuleCollectorHealthCheckAddReport(eleonsoftModuleCollectorAddHealthCheckReportDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling HealthCheckApi.EleonsoftModuleCollectorHealthCheckAddReport: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the EleonsoftModuleCollectorHealthCheckAddReportWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleonsoftModuleCollectorHealthCheckReportDto> response = apiInstance.EleonsoftModuleCollectorHealthCheckAddReportWithHttpInfo(eleonsoftModuleCollectorAddHealthCheckReportDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling HealthCheckApi.EleonsoftModuleCollectorHealthCheckAddReportWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **eleonsoftModuleCollectorAddHealthCheckReportDto** | [**EleonsoftModuleCollectorAddHealthCheckReportDto**](EleonsoftModuleCollectorAddHealthCheckReportDto.md) |  | [optional]  |

### Return type

[**EleonsoftModuleCollectorHealthCheckReportDto**](EleonsoftModuleCollectorHealthCheckReportDto.md)

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

<a id="eleonsoftmodulecollectorhealthcheckaddreportbulk"></a>
# **EleonsoftModuleCollectorHealthCheckAddReportBulk**
> bool EleonsoftModuleCollectorHealthCheckAddReportBulk (EleonsoftModuleCollectorAddHealthCheckReportBulkDto eleonsoftModuleCollectorAddHealthCheckReportBulkDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class EleonsoftModuleCollectorHealthCheckAddReportBulkExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new HealthCheckApi(config);
            var eleonsoftModuleCollectorAddHealthCheckReportBulkDto = new EleonsoftModuleCollectorAddHealthCheckReportBulkDto(); // EleonsoftModuleCollectorAddHealthCheckReportBulkDto |  (optional) 

            try
            {
                bool result = apiInstance.EleonsoftModuleCollectorHealthCheckAddReportBulk(eleonsoftModuleCollectorAddHealthCheckReportBulkDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling HealthCheckApi.EleonsoftModuleCollectorHealthCheckAddReportBulk: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the EleonsoftModuleCollectorHealthCheckAddReportBulkWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.EleonsoftModuleCollectorHealthCheckAddReportBulkWithHttpInfo(eleonsoftModuleCollectorAddHealthCheckReportBulkDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling HealthCheckApi.EleonsoftModuleCollectorHealthCheckAddReportBulkWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **eleonsoftModuleCollectorAddHealthCheckReportBulkDto** | [**EleonsoftModuleCollectorAddHealthCheckReportBulkDto**](EleonsoftModuleCollectorAddHealthCheckReportBulkDto.md) |  | [optional]  |

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

<a id="eleonsoftmodulecollectorhealthcheckcreate"></a>
# **EleonsoftModuleCollectorHealthCheckCreate**
> EleonsoftModuleCollectorHealthCheckDto EleonsoftModuleCollectorHealthCheckCreate (EleonsoftModuleCollectorCreateHealthCheckDto eleonsoftModuleCollectorCreateHealthCheckDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class EleonsoftModuleCollectorHealthCheckCreateExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new HealthCheckApi(config);
            var eleonsoftModuleCollectorCreateHealthCheckDto = new EleonsoftModuleCollectorCreateHealthCheckDto(); // EleonsoftModuleCollectorCreateHealthCheckDto |  (optional) 

            try
            {
                EleonsoftModuleCollectorHealthCheckDto result = apiInstance.EleonsoftModuleCollectorHealthCheckCreate(eleonsoftModuleCollectorCreateHealthCheckDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling HealthCheckApi.EleonsoftModuleCollectorHealthCheckCreate: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the EleonsoftModuleCollectorHealthCheckCreateWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleonsoftModuleCollectorHealthCheckDto> response = apiInstance.EleonsoftModuleCollectorHealthCheckCreateWithHttpInfo(eleonsoftModuleCollectorCreateHealthCheckDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling HealthCheckApi.EleonsoftModuleCollectorHealthCheckCreateWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **eleonsoftModuleCollectorCreateHealthCheckDto** | [**EleonsoftModuleCollectorCreateHealthCheckDto**](EleonsoftModuleCollectorCreateHealthCheckDto.md) |  | [optional]  |

### Return type

[**EleonsoftModuleCollectorHealthCheckDto**](EleonsoftModuleCollectorHealthCheckDto.md)

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

<a id="eleonsoftmodulecollectorhealthcheckgetbyid"></a>
# **EleonsoftModuleCollectorHealthCheckGetById**
> EleonsoftModuleCollectorFullHealthCheckDto EleonsoftModuleCollectorHealthCheckGetById (Guid id = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class EleonsoftModuleCollectorHealthCheckGetByIdExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new HealthCheckApi(config);
            var id = "id_example";  // Guid |  (optional) 

            try
            {
                EleonsoftModuleCollectorFullHealthCheckDto result = apiInstance.EleonsoftModuleCollectorHealthCheckGetById(id);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling HealthCheckApi.EleonsoftModuleCollectorHealthCheckGetById: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the EleonsoftModuleCollectorHealthCheckGetByIdWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleonsoftModuleCollectorFullHealthCheckDto> response = apiInstance.EleonsoftModuleCollectorHealthCheckGetByIdWithHttpInfo(id);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling HealthCheckApi.EleonsoftModuleCollectorHealthCheckGetByIdWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **Guid** |  | [optional]  |

### Return type

[**EleonsoftModuleCollectorFullHealthCheckDto**](EleonsoftModuleCollectorFullHealthCheckDto.md)

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

<a id="eleonsoftmodulecollectorhealthcheckgetlist"></a>
# **EleonsoftModuleCollectorHealthCheckGetList**
> EleoncorePagedResultDtoOfEleonsoftModuleCollectorHealthCheckDto EleonsoftModuleCollectorHealthCheckGetList (string type = null, string initiator = null, DateTime minTime = null, DateTime maxTime = null, string sorting = null, int skipCount = null, int maxResultCount = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class EleonsoftModuleCollectorHealthCheckGetListExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new HealthCheckApi(config);
            var type = "type_example";  // string |  (optional) 
            var initiator = "initiator_example";  // string |  (optional) 
            var minTime = DateTime.Parse("2013-10-20T19:20:30+01:00");  // DateTime |  (optional) 
            var maxTime = DateTime.Parse("2013-10-20T19:20:30+01:00");  // DateTime |  (optional) 
            var sorting = "sorting_example";  // string |  (optional) 
            var skipCount = 56;  // int |  (optional) 
            var maxResultCount = 56;  // int |  (optional) 

            try
            {
                EleoncorePagedResultDtoOfEleonsoftModuleCollectorHealthCheckDto result = apiInstance.EleonsoftModuleCollectorHealthCheckGetList(type, initiator, minTime, maxTime, sorting, skipCount, maxResultCount);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling HealthCheckApi.EleonsoftModuleCollectorHealthCheckGetList: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the EleonsoftModuleCollectorHealthCheckGetListWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleoncorePagedResultDtoOfEleonsoftModuleCollectorHealthCheckDto> response = apiInstance.EleonsoftModuleCollectorHealthCheckGetListWithHttpInfo(type, initiator, minTime, maxTime, sorting, skipCount, maxResultCount);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling HealthCheckApi.EleonsoftModuleCollectorHealthCheckGetListWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **type** | **string** |  | [optional]  |
| **initiator** | **string** |  | [optional]  |
| **minTime** | **DateTime** |  | [optional]  |
| **maxTime** | **DateTime** |  | [optional]  |
| **sorting** | **string** |  | [optional]  |
| **skipCount** | **int** |  | [optional]  |
| **maxResultCount** | **int** |  | [optional]  |

### Return type

[**EleoncorePagedResultDtoOfEleonsoftModuleCollectorHealthCheckDto**](EleoncorePagedResultDtoOfEleonsoftModuleCollectorHealthCheckDto.md)

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

<a id="eleonsoftmodulecollectorhealthchecksend"></a>
# **EleonsoftModuleCollectorHealthCheckSend**
> EleonsoftModuleCollectorHealthCheckDto EleonsoftModuleCollectorHealthCheckSend (EleonsoftModuleCollectorSendHealthCheckDto eleonsoftModuleCollectorSendHealthCheckDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class EleonsoftModuleCollectorHealthCheckSendExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new HealthCheckApi(config);
            var eleonsoftModuleCollectorSendHealthCheckDto = new EleonsoftModuleCollectorSendHealthCheckDto(); // EleonsoftModuleCollectorSendHealthCheckDto |  (optional) 

            try
            {
                EleonsoftModuleCollectorHealthCheckDto result = apiInstance.EleonsoftModuleCollectorHealthCheckSend(eleonsoftModuleCollectorSendHealthCheckDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling HealthCheckApi.EleonsoftModuleCollectorHealthCheckSend: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the EleonsoftModuleCollectorHealthCheckSendWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleonsoftModuleCollectorHealthCheckDto> response = apiInstance.EleonsoftModuleCollectorHealthCheckSendWithHttpInfo(eleonsoftModuleCollectorSendHealthCheckDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling HealthCheckApi.EleonsoftModuleCollectorHealthCheckSendWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **eleonsoftModuleCollectorSendHealthCheckDto** | [**EleonsoftModuleCollectorSendHealthCheckDto**](EleonsoftModuleCollectorSendHealthCheckDto.md) |  | [optional]  |

### Return type

[**EleonsoftModuleCollectorHealthCheckDto**](EleonsoftModuleCollectorHealthCheckDto.md)

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

