# EleonsoftProxy.Model.JobSchedulerTaskDto

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Id** | **Guid** |  | [optional] 
**IsActive** | **bool** |  | [optional] 
**Name** | **string** |  | [optional] 
**Description** | **string** |  | [optional] 
**CanRunManually** | **bool** |  | [optional] 
**RestartAfterFailIntervalSeconds** | **int** |  | [optional] 
**RestartAfterFailMaxAttempts** | **int** |  | [optional] 
**TimeoutSeconds** | **int** |  | [optional] 
**AllowForceStop** | **bool** |  | [optional] 
**LastRunTimeUtc** | **DateTime** |  | [optional] 
**NextRunTimeUtc** | **DateTime** |  | [optional] 
**Status** | **EleoncoreJobSchedulerTaskStatus** |  | [optional] 
**LastDurationSeconds** | **int** |  | [optional] 
**OnFailureRecepients** | **string** |  | [optional] 
**Executions** | [**List&lt;JobSchedulerTaskExecutionDto&gt;**](JobSchedulerTaskExecutionDto.md) |  | [optional] 

[[Back to Model list]](../../README.md#documentation-for-models) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to README]](../../README.md)

