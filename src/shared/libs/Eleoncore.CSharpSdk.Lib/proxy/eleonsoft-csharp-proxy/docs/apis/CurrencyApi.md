# EleonsoftProxy.Api.CurrencyApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ModuleCollectorCurrencyGetCurrencies**](CurrencyApi.md#modulecollectorcurrencygetcurrencies) | **GET** /api/infrastructure/currency/GetCurrencies |  |
| [**ModuleCollectorCurrencyGetCurrencyRate**](CurrencyApi.md#modulecollectorcurrencygetcurrencyrate) | **GET** /api/infrastructure/currency/GetCurrencyRate |  |
| [**ModuleCollectorCurrencyGetCurrencyRates**](CurrencyApi.md#modulecollectorcurrencygetcurrencyrates) | **GET** /api/infrastructure/currency/GetCurrencyRates |  |
| [**ModuleCollectorCurrencyGetSystemCurrency**](CurrencyApi.md#modulecollectorcurrencygetsystemcurrency) | **GET** /api/infrastructure/currency/GetSystemCurrency |  |
| [**ModuleCollectorCurrencySetSystemCurrency**](CurrencyApi.md#modulecollectorcurrencysetsystemcurrency) | **POST** /api/infrastructure/currency/SetSystemCurrency |  |

<a id="modulecollectorcurrencygetcurrencies"></a>
# **ModuleCollectorCurrencyGetCurrencies**
> List&lt;ModuleCollectorCurrencyDto&gt; ModuleCollectorCurrencyGetCurrencies ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class ModuleCollectorCurrencyGetCurrenciesExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new CurrencyApi(config);

            try
            {
                List<ModuleCollectorCurrencyDto> result = apiInstance.ModuleCollectorCurrencyGetCurrencies();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling CurrencyApi.ModuleCollectorCurrencyGetCurrencies: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ModuleCollectorCurrencyGetCurrenciesWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<ModuleCollectorCurrencyDto>> response = apiInstance.ModuleCollectorCurrencyGetCurrenciesWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling CurrencyApi.ModuleCollectorCurrencyGetCurrenciesWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**List&lt;ModuleCollectorCurrencyDto&gt;**](ModuleCollectorCurrencyDto.md)

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

<a id="modulecollectorcurrencygetcurrencyrate"></a>
# **ModuleCollectorCurrencyGetCurrencyRate**
> ModuleCollectorCurrencyRateDto ModuleCollectorCurrencyGetCurrencyRate (string from = null, string to = null, DateTime rateDate = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class ModuleCollectorCurrencyGetCurrencyRateExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new CurrencyApi(config);
            var from = "from_example";  // string |  (optional) 
            var to = "to_example";  // string |  (optional) 
            var rateDate = DateTime.Parse("2013-10-20T19:20:30+01:00");  // DateTime |  (optional) 

            try
            {
                ModuleCollectorCurrencyRateDto result = apiInstance.ModuleCollectorCurrencyGetCurrencyRate(from, to, rateDate);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling CurrencyApi.ModuleCollectorCurrencyGetCurrencyRate: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ModuleCollectorCurrencyGetCurrencyRateWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<ModuleCollectorCurrencyRateDto> response = apiInstance.ModuleCollectorCurrencyGetCurrencyRateWithHttpInfo(from, to, rateDate);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling CurrencyApi.ModuleCollectorCurrencyGetCurrencyRateWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **from** | **string** |  | [optional]  |
| **to** | **string** |  | [optional]  |
| **rateDate** | **DateTime** |  | [optional]  |

### Return type

[**ModuleCollectorCurrencyRateDto**](ModuleCollectorCurrencyRateDto.md)

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

<a id="modulecollectorcurrencygetcurrencyrates"></a>
# **ModuleCollectorCurrencyGetCurrencyRates**
> List&lt;ModuleCollectorCurrencyRateDto&gt; ModuleCollectorCurrencyGetCurrencyRates (string from = null, DateTime rateDate = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class ModuleCollectorCurrencyGetCurrencyRatesExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new CurrencyApi(config);
            var from = "from_example";  // string |  (optional) 
            var rateDate = DateTime.Parse("2013-10-20T19:20:30+01:00");  // DateTime |  (optional) 

            try
            {
                List<ModuleCollectorCurrencyRateDto> result = apiInstance.ModuleCollectorCurrencyGetCurrencyRates(from, rateDate);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling CurrencyApi.ModuleCollectorCurrencyGetCurrencyRates: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ModuleCollectorCurrencyGetCurrencyRatesWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<ModuleCollectorCurrencyRateDto>> response = apiInstance.ModuleCollectorCurrencyGetCurrencyRatesWithHttpInfo(from, rateDate);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling CurrencyApi.ModuleCollectorCurrencyGetCurrencyRatesWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **from** | **string** |  | [optional]  |
| **rateDate** | **DateTime** |  | [optional]  |

### Return type

[**List&lt;ModuleCollectorCurrencyRateDto&gt;**](ModuleCollectorCurrencyRateDto.md)

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

<a id="modulecollectorcurrencygetsystemcurrency"></a>
# **ModuleCollectorCurrencyGetSystemCurrency**
> ModuleCollectorCurrencyDto ModuleCollectorCurrencyGetSystemCurrency ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class ModuleCollectorCurrencyGetSystemCurrencyExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new CurrencyApi(config);

            try
            {
                ModuleCollectorCurrencyDto result = apiInstance.ModuleCollectorCurrencyGetSystemCurrency();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling CurrencyApi.ModuleCollectorCurrencyGetSystemCurrency: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ModuleCollectorCurrencyGetSystemCurrencyWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<ModuleCollectorCurrencyDto> response = apiInstance.ModuleCollectorCurrencyGetSystemCurrencyWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling CurrencyApi.ModuleCollectorCurrencyGetSystemCurrencyWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**ModuleCollectorCurrencyDto**](ModuleCollectorCurrencyDto.md)

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

<a id="modulecollectorcurrencysetsystemcurrency"></a>
# **ModuleCollectorCurrencySetSystemCurrency**
> bool ModuleCollectorCurrencySetSystemCurrency (string code = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class ModuleCollectorCurrencySetSystemCurrencyExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new CurrencyApi(config);
            var code = "code_example";  // string |  (optional) 

            try
            {
                bool result = apiInstance.ModuleCollectorCurrencySetSystemCurrency(code);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling CurrencyApi.ModuleCollectorCurrencySetSystemCurrency: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ModuleCollectorCurrencySetSystemCurrencyWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.ModuleCollectorCurrencySetSystemCurrencyWithHttpInfo(code);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling CurrencyApi.ModuleCollectorCurrencySetSystemCurrencyWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **code** | **string** |  | [optional]  |

### Return type

**bool**

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

