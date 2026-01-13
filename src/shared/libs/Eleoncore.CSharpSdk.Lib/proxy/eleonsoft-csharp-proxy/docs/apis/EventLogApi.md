# EleonsoftProxy.Api.EventLogApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**CountriesEventLogAddInterceptionRule**](EventLogApi.md#countrieseventlogaddinterceptionrule) | **POST** /api/country/event-logs/AddInterceptionRule |  |
| [**CountriesEventLogGetEventLogList**](EventLogApi.md#countrieseventloggeteventloglist) | **GET** /api/country/event-logs/GetEventLogList |  |
| [**CountriesEventLogGetInterceptionRules**](EventLogApi.md#countrieseventloggetinterceptionrules) | **GET** /api/country/event-logs/GetInterceptionRules |  |
| [**CountriesEventLogRemoveInterceptionRule**](EventLogApi.md#countrieseventlogremoveinterceptionrule) | **POST** /api/country/event-logs/RemoveInterceptionRule |  |
| [**CountriesEventLogUpdateInterceptionRule**](EventLogApi.md#countrieseventlogupdateinterceptionrule) | **POST** /api/country/event-logs/UpdateInterceptionRule |  |

<a id="countrieseventlogaddinterceptionrule"></a>
# **CountriesEventLogAddInterceptionRule**
> void CountriesEventLogAddInterceptionRule (EleoncoreEventInterceptionRuleDto eleoncoreEventInterceptionRuleDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CountriesEventLogAddInterceptionRuleExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new EventLogApi(config);
            var eleoncoreEventInterceptionRuleDto = new EleoncoreEventInterceptionRuleDto(); // EleoncoreEventInterceptionRuleDto |  (optional) 

            try
            {
                apiInstance.CountriesEventLogAddInterceptionRule(eleoncoreEventInterceptionRuleDto);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventLogApi.CountriesEventLogAddInterceptionRule: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CountriesEventLogAddInterceptionRuleWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.CountriesEventLogAddInterceptionRuleWithHttpInfo(eleoncoreEventInterceptionRuleDto);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling EventLogApi.CountriesEventLogAddInterceptionRuleWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **eleoncoreEventInterceptionRuleDto** | [**EleoncoreEventInterceptionRuleDto**](EleoncoreEventInterceptionRuleDto.md) |  | [optional]  |

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

<a id="countrieseventloggeteventloglist"></a>
# **CountriesEventLogGetEventLogList**
> EleoncorePagedResultDtoOfEleoncoreEventLogDto CountriesEventLogGetEventLogList (bool filterByTenant = null, Guid tenantIdFilter = null, string moduleNameFilter = null, string eventNameFilter = null, int skip = null, int take = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CountriesEventLogGetEventLogListExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new EventLogApi(config);
            var filterByTenant = true;  // bool |  (optional) 
            var tenantIdFilter = "tenantIdFilter_example";  // Guid |  (optional) 
            var moduleNameFilter = "moduleNameFilter_example";  // string |  (optional) 
            var eventNameFilter = "eventNameFilter_example";  // string |  (optional) 
            var skip = 56;  // int |  (optional) 
            var take = 56;  // int |  (optional) 

            try
            {
                EleoncorePagedResultDtoOfEleoncoreEventLogDto result = apiInstance.CountriesEventLogGetEventLogList(filterByTenant, tenantIdFilter, moduleNameFilter, eventNameFilter, skip, take);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventLogApi.CountriesEventLogGetEventLogList: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CountriesEventLogGetEventLogListWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleoncorePagedResultDtoOfEleoncoreEventLogDto> response = apiInstance.CountriesEventLogGetEventLogListWithHttpInfo(filterByTenant, tenantIdFilter, moduleNameFilter, eventNameFilter, skip, take);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling EventLogApi.CountriesEventLogGetEventLogListWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **filterByTenant** | **bool** |  | [optional]  |
| **tenantIdFilter** | **Guid** |  | [optional]  |
| **moduleNameFilter** | **string** |  | [optional]  |
| **eventNameFilter** | **string** |  | [optional]  |
| **skip** | **int** |  | [optional]  |
| **take** | **int** |  | [optional]  |

### Return type

[**EleoncorePagedResultDtoOfEleoncoreEventLogDto**](EleoncorePagedResultDtoOfEleoncoreEventLogDto.md)

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

<a id="countrieseventloggetinterceptionrules"></a>
# **CountriesEventLogGetInterceptionRules**
> List&lt;EleoncoreEventInterceptionRuleDto&gt; CountriesEventLogGetInterceptionRules ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CountriesEventLogGetInterceptionRulesExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new EventLogApi(config);

            try
            {
                List<EleoncoreEventInterceptionRuleDto> result = apiInstance.CountriesEventLogGetInterceptionRules();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventLogApi.CountriesEventLogGetInterceptionRules: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CountriesEventLogGetInterceptionRulesWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<EleoncoreEventInterceptionRuleDto>> response = apiInstance.CountriesEventLogGetInterceptionRulesWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling EventLogApi.CountriesEventLogGetInterceptionRulesWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**List&lt;EleoncoreEventInterceptionRuleDto&gt;**](EleoncoreEventInterceptionRuleDto.md)

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

<a id="countrieseventlogremoveinterceptionrule"></a>
# **CountriesEventLogRemoveInterceptionRule**
> void CountriesEventLogRemoveInterceptionRule (Guid ruleId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CountriesEventLogRemoveInterceptionRuleExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new EventLogApi(config);
            var ruleId = "ruleId_example";  // Guid |  (optional) 

            try
            {
                apiInstance.CountriesEventLogRemoveInterceptionRule(ruleId);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventLogApi.CountriesEventLogRemoveInterceptionRule: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CountriesEventLogRemoveInterceptionRuleWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.CountriesEventLogRemoveInterceptionRuleWithHttpInfo(ruleId);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling EventLogApi.CountriesEventLogRemoveInterceptionRuleWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **ruleId** | **Guid** |  | [optional]  |

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

<a id="countrieseventlogupdateinterceptionrule"></a>
# **CountriesEventLogUpdateInterceptionRule**
> void CountriesEventLogUpdateInterceptionRule (EleoncoreEventInterceptionRuleDto eleoncoreEventInterceptionRuleDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CountriesEventLogUpdateInterceptionRuleExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new EventLogApi(config);
            var eleoncoreEventInterceptionRuleDto = new EleoncoreEventInterceptionRuleDto(); // EleoncoreEventInterceptionRuleDto |  (optional) 

            try
            {
                apiInstance.CountriesEventLogUpdateInterceptionRule(eleoncoreEventInterceptionRuleDto);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventLogApi.CountriesEventLogUpdateInterceptionRule: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CountriesEventLogUpdateInterceptionRuleWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.CountriesEventLogUpdateInterceptionRuleWithHttpInfo(eleoncoreEventInterceptionRuleDto);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling EventLogApi.CountriesEventLogUpdateInterceptionRuleWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **eleoncoreEventInterceptionRuleDto** | [**EleoncoreEventInterceptionRuleDto**](EleoncoreEventInterceptionRuleDto.md) |  | [optional]  |

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

