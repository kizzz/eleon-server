# EleonsoftProxy.Model.BackgroundJobsBackgroundJobHeaderDto

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Id** | **Guid** |  | [optional] 
**ParentJobId** | **Guid** |  | [optional] 
**CreationTime** | **DateTime** |  | [optional] 
**Type** | **string** |  | [optional] 
**Initiator** | **string** |  | [optional] 
**Status** | **EleoncoreBackgroundJobStatus** |  | [optional] 
**ScheduleExecutionDateUtc** | **DateTime** |  | [optional] 
**JobFinishedUtc** | **DateTime** |  | [optional] 
**LastExecutionDateUtc** | **DateTime** |  | [optional] 
**IsRetryAllowed** | **bool** |  | [optional] 
**Description** | **string** |  | [optional] 
**SourceId** | **string** |  | [optional] 
**SourceType** | **string** |  | [optional] 
**TimeoutInMinutes** | **int** |  | [optional] 
**RetryIntervalInMinutes** | **int** |  | [optional] 
**MaxRetryAttempts** | **int** |  | [optional] 
**CurrentRetryAttempt** | **int** |  | [optional] 
**NextRetryTimeUtc** | **DateTime** |  | [optional] 
**OnFailureRecepients** | **string** |  | [optional] 

[[Back to Model list]](../../README.md#documentation-for-models) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to README]](../../README.md)

