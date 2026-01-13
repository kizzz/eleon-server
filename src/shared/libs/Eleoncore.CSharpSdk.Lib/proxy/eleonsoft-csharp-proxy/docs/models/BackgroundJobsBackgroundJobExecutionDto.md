# EleonsoftProxy.Model.BackgroundJobsBackgroundJobExecutionDto

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Id** | **Guid** |  | [optional] 
**TenantId** | **Guid** |  | [optional] 
**CreationTime** | **DateTime** |  | [optional] 
**ExecutionStartTimeUtc** | **DateTime** |  | [optional] 
**ExecutionEndTimeUtc** | **DateTime** |  | [optional] 
**Status** | **EleoncoreBackgroundJobExecutionStatus** |  | [optional] 
**IsRetryExecution** | **bool** |  | [optional] 
**RetryUserInitiatorId** | **Guid** |  | [optional] 
**StartExecutionParams** | **string** |  | [optional] 
**StartExecutionExtraParams** | **string** |  | [optional] 
**BackgroundJobEntityId** | **Guid** |  | [optional] 
**Messages** | [**List&lt;BackgroundJobsBackgroundJobMessageDto&gt;**](BackgroundJobsBackgroundJobMessageDto.md) |  | [optional] 
**StatusChangedBy** | **string** |  | [optional] 
**IsStatusChangedManually** | **bool** |  | [optional] 

[[Back to Model list]](../../README.md#documentation-for-models) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to README]](../../README.md)

