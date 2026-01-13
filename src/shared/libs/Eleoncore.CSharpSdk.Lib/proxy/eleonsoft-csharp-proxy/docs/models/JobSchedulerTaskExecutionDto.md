# EleonsoftProxy.Model.JobSchedulerTaskExecutionDto

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Id** | **Guid** |  | [optional] 
**Status** | **EleoncoreJobSchedulerTaskExecutionStatus** |  | [optional] 
**RunnedByUserId** | **Guid** |  | [optional] 
**RunnedByUserName** | **string** |  | [optional] 
**RunnedByTriggerId** | **Guid** |  | [optional] 
**RunnedByTriggerName** | **string** |  | [optional] 
**StartedAtUtc** | **DateTime** |  | [optional] 
**FinishedAtUtc** | **DateTime** |  | [optional] 
**TaskId** | **Guid** |  | [optional] 
**ActionExecutions** | [**List&lt;JobSchedulerActionExecutionDto&gt;**](JobSchedulerActionExecutionDto.md) |  | [optional] 

[[Back to Model list]](../../README.md#documentation-for-models) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to README]](../../README.md)

