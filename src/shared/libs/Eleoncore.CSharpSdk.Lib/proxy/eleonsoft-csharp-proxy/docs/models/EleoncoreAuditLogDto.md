# EleonsoftProxy.Model.EleoncoreAuditLogDto

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Id** | **Guid** |  | [optional] 
**ApplicationName** | **string** |  | [optional] 
**UserId** | **Guid** |  | [optional] 
**UserName** | **string** |  | [optional] 
**TenantId** | **Guid** |  | [optional] 
**TenantName** | **string** |  | [optional] 
**ImpersonatorUserId** | **Guid** |  | [optional] 
**ImpersonatorUserName** | **string** |  | [optional] 
**ImpersonatorTenantId** | **Guid** |  | [optional] 
**ImpersonatorTenantName** | **string** |  | [optional] 
**ExecutionTime** | **DateTime** |  | [optional] 
**ExecutionDuration** | **int** |  | [optional] 
**ClientIpAddress** | **string** |  | [optional] 
**ClientName** | **string** |  | [optional] 
**ClientId** | **string** |  | [optional] 
**CorrelationId** | **string** |  | [optional] 
**BrowserInfo** | **string** |  | [optional] 
**HttpMethod** | **string** |  | [optional] 
**Url** | **string** |  | [optional] 
**Exceptions** | **string** |  | [optional] 
**Comments** | **string** |  | [optional] 
**HttpStatusCode** | **int** |  | [optional] 
**EntityChanges** | [**List&lt;EleoncoreEntityChangeDto&gt;**](EleoncoreEntityChangeDto.md) |  | [optional] 
**Actions** | [**List&lt;EleoncoreAuditLogActionDto&gt;**](EleoncoreAuditLogActionDto.md) |  | [optional] 
**ExtraProperties** | **Dictionary&lt;string, string&gt;** |  | [optional] 

[[Back to Model list]](../../README.md#documentation-for-models) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to README]](../../README.md)

