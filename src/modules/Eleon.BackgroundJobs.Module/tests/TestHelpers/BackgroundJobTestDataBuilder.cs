using System;
using System.Collections.Generic;
using Common.Module.Constants;
using VPortal.BackgroundJobs.Module.Entities;

namespace BackgroundJobs.Module.TestHelpers;

public class BackgroundJobTestDataBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid? _tenantId = TestConstants.TenantIds.Host;
    private Guid? _parentJobId = null;
    private string _type = TestConstants.JobTypes.TestJob;
    private bool _isSystemInternal = false;
    private string _sourceId = TestConstants.Users.TestUser;
    private string _sourceType = "User";
    private BackgroundJobStatus _status = BackgroundJobStatus.New;
    private DateTime _scheduleExecutionDateUtc = TestConstants.Dates.UtcNow;
    private DateTime _lastExecutionDateUtc = TestConstants.Dates.UtcNow;
    private bool _isRetryAllowed = true;
    private string _initiator = TestConstants.Users.TestUser;
    private string _description = "Test job description";
    private string _startExecutionParams = "{}";
    private string _startExecutionExtraParams = string.Empty;
    private string _result = string.Empty;
    private DateTime? _jobFinishedUtc = null;
    private string _environmentId = string.Empty;
    private int _timeoutInMinutes = 60;
    private int _retryIntervalInMinutes = 5;
    private int _maxRetryAttempts = 3;
    private int _currentRetryAttempt = 0;
    private DateTime? _nextRetryTimeUtc = null;
    private string _onFailureRecepients = string.Empty;
    private List<BackgroundJobExecutionEntity> _executions = new();

    public BackgroundJobTestDataBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public BackgroundJobTestDataBuilder WithTenantId(Guid? tenantId)
    {
        _tenantId = tenantId;
        return this;
    }

    public BackgroundJobTestDataBuilder WithParentJobId(Guid? parentJobId)
    {
        _parentJobId = parentJobId;
        return this;
    }

    public BackgroundJobTestDataBuilder WithType(string type)
    {
        _type = type;
        return this;
    }

    public BackgroundJobTestDataBuilder AsSystemInternal()
    {
        _isSystemInternal = true;
        return this;
    }

    public BackgroundJobTestDataBuilder WithSource(string sourceId, string sourceType)
    {
        _sourceId = sourceId;
        _sourceType = sourceType;
        return this;
    }

    public BackgroundJobTestDataBuilder WithStatus(BackgroundJobStatus status)
    {
        _status = status;
        return this;
    }

    public BackgroundJobTestDataBuilder WithScheduleDate(DateTime scheduleDate)
    {
        _scheduleExecutionDateUtc = scheduleDate;
        return this;
    }

    public BackgroundJobTestDataBuilder WithLastExecutionDate(DateTime lastExecutionDate)
    {
        _lastExecutionDateUtc = lastExecutionDate;
        return this;
    }

    public BackgroundJobTestDataBuilder WithRetrySettings(int maxRetryAttempts, int retryIntervalInMinutes, bool isRetryAllowed = true)
    {
        _maxRetryAttempts = maxRetryAttempts;
        _retryIntervalInMinutes = retryIntervalInMinutes;
        _isRetryAllowed = isRetryAllowed;
        return this;
    }

    public BackgroundJobTestDataBuilder WithMaxRetryAttempts(int maxRetryAttempts)
    {
        _maxRetryAttempts = maxRetryAttempts;
        return this;
    }

    public BackgroundJobTestDataBuilder WithIsRetryAllowed(bool isRetryAllowed)
    {
        _isRetryAllowed = isRetryAllowed;
        return this;
    }

    public BackgroundJobTestDataBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public BackgroundJobTestDataBuilder WithTimeout(int timeoutInMinutes)
    {
        _timeoutInMinutes = timeoutInMinutes;
        return this;
    }

    public BackgroundJobTestDataBuilder WithCurrentRetryAttempt(int attempt)
    {
        _currentRetryAttempt = attempt;
        return this;
    }

    public BackgroundJobTestDataBuilder WithNextRetryTime(DateTime? nextRetryTime)
    {
        _nextRetryTimeUtc = nextRetryTime;
        return this;
    }

    public BackgroundJobTestDataBuilder WithExecutionParams(string startParams, string extraParams = null)
    {
        _startExecutionParams = startParams;
        _startExecutionExtraParams = extraParams ?? string.Empty;
        return this;
    }

    public BackgroundJobTestDataBuilder WithResult(string result)
    {
        _result = result ?? string.Empty;
        return this;
    }

    public BackgroundJobTestDataBuilder WithJobFinishedUtc(DateTime? finishedUtc)
    {
        _jobFinishedUtc = finishedUtc;
        return this;
    }

    public BackgroundJobTestDataBuilder WithOnFailureRecepients(string recepients)
    {
        _onFailureRecepients = recepients ?? string.Empty;
        return this;
    }

    public BackgroundJobTestDataBuilder WithExecution(BackgroundJobExecutionEntity execution)
    {
        _executions.Add(execution);
        return this;
    }

    public BackgroundJobTestDataBuilder WithExecutions(List<BackgroundJobExecutionEntity> executions)
    {
        _executions = executions ?? new List<BackgroundJobExecutionEntity>();
        return this;
    }

    public BackgroundJobEntity Build()
    {
        var job = new BackgroundJobEntity(_id)
        {
            TenantId = _tenantId,
            ParentJobId = _parentJobId,
            Type = _type,
            IsSystemInternal = _isSystemInternal,
            SourceId = _sourceId,
            SourceType = _sourceType,
            Status = _status,
            ScheduleExecutionDateUtc = _scheduleExecutionDateUtc,
            LastExecutionDateUtc = _lastExecutionDateUtc,
            IsRetryAllowed = _isRetryAllowed,
            Initiator = _initiator,
            Description = _description,
            StartExecutionParams = _startExecutionParams,
            StartExecutionExtraParams = _startExecutionExtraParams ?? string.Empty,
            Result = _result ?? string.Empty,
            JobFinishedUtc = _jobFinishedUtc,
            EnvironmentId = _environmentId ?? string.Empty,
            TimeoutInMinutes = _timeoutInMinutes,
            RetryIntervalInMinutes = _retryIntervalInMinutes,
            MaxRetryAttempts = _maxRetryAttempts,
            CurrentRetryAttempt = _currentRetryAttempt,
            NextRetryTimeUtc = _nextRetryTimeUtc,
            OnFailureRecepients = _onFailureRecepients ?? string.Empty
        };

        foreach (var execution in _executions)
        {
            job.Executions.Add(execution);
        }

        return job;
    }

    public static BackgroundJobTestDataBuilder Create() => new();

    public BackgroundJobTestDataBuilder WithEnvironmentId(string environmentId)
    {
        _environmentId = environmentId ?? string.Empty;
        return this;
    }
}

public class BackgroundJobExecutionTestDataBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid? _tenantId = TestConstants.TenantIds.Host;
    private DateTime _creationTime = TestConstants.Dates.UtcNow;
    private DateTime _executionStartTimeUtc = TestConstants.Dates.UtcNow;
    private DateTime? _executionEndTimeUtc = null;
    private BackgroundJobExecutionStatus _status = BackgroundJobExecutionStatus.Starting;
    private bool _isRetryExecution = false;
    private Guid? _retryUserInitiatorId = null;
    private string _startExecutionParams = "{}";
    private string _startExecutionExtraParams = string.Empty;
    private Guid _backgroundJobEntityId = TestConstants.JobIds.Job1;
    private List<BackgroundJobMessageEntity> _messages = new();
    private string _statusChangedBy = string.Empty;
    private bool _isStatusChangedManually = false;

    public BackgroundJobExecutionTestDataBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public BackgroundJobExecutionTestDataBuilder WithTenantId(Guid? tenantId)
    {
        _tenantId = tenantId;
        return this;
    }

    public BackgroundJobExecutionTestDataBuilder WithCreationTime(DateTime creationTime)
    {
        _creationTime = creationTime;
        return this;
    }

    public BackgroundJobExecutionTestDataBuilder WithExecutionStartTime(DateTime startTime)
    {
        _executionStartTimeUtc = startTime;
        return this;
    }

    public BackgroundJobExecutionTestDataBuilder WithExecutionEndTime(DateTime? endTime)
    {
        _executionEndTimeUtc = endTime;
        return this;
    }

    public BackgroundJobExecutionTestDataBuilder WithStatus(BackgroundJobExecutionStatus status)
    {
        _status = status;
        return this;
    }

    public BackgroundJobExecutionTestDataBuilder AsRetryExecution()
    {
        _isRetryExecution = true;
        return this;
    }

    public BackgroundJobExecutionTestDataBuilder WithRetryUserInitiatorId(Guid? userId)
    {
        _retryUserInitiatorId = userId;
        return this;
    }

    public BackgroundJobExecutionTestDataBuilder WithExecutionParams(string startParams, string extraParams = null)
    {
        _startExecutionParams = startParams;
        _startExecutionExtraParams = extraParams ?? string.Empty;
        return this;
    }

    public BackgroundJobExecutionTestDataBuilder WithBackgroundJobId(Guid jobId)
    {
        _backgroundJobEntityId = jobId;
        return this;
    }

    public BackgroundJobExecutionTestDataBuilder WithMessage(BackgroundJobMessageType type, string text, DateTime? dateTime = null)
    {
        _messages.Add(new BackgroundJobMessageEntity(Guid.NewGuid())
        {
            MessageType = type,
            TextMessage = text,
            CreationTime = dateTime ?? TestConstants.Dates.UtcNow
        });
        return this;
    }

    public BackgroundJobExecutionTestDataBuilder WithMessages(List<BackgroundJobMessageEntity> messages)
    {
        _messages = messages ?? new List<BackgroundJobMessageEntity>();
        return this;
    }

    public BackgroundJobExecutionTestDataBuilder WithStatusChangedBy(string changedBy, bool manually = false)
    {
        _statusChangedBy = changedBy ?? string.Empty;
        _isStatusChangedManually = manually;
        return this;
    }

    public BackgroundJobExecutionEntity Build()
    {
        var execution = new BackgroundJobExecutionEntity(_id)
        {
            TenantId = _tenantId,
            CreationTime = _creationTime,
            ExecutionStartTimeUtc = _executionStartTimeUtc,
            ExecutionEndTimeUtc = _executionEndTimeUtc,
            Status = _status,
            IsRetryExecution = _isRetryExecution,
            RetryUserInitiatorId = _retryUserInitiatorId,
            StartExecutionParams = _startExecutionParams,
            StartExecutionExtraParams = _startExecutionExtraParams ?? string.Empty,
            BackgroundJobEntityId = _backgroundJobEntityId,
            StatusChangedBy = _statusChangedBy ?? string.Empty,
            IsStatusChangedManually = _isStatusChangedManually
        };

        foreach (var message in _messages)
        {
            execution.Messages.Add(message);
        }

        return execution;
    }

    public static BackgroundJobExecutionTestDataBuilder Create() => new();
}

public class BackgroundJobMessageTestDataBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid? _tenantId = TestConstants.TenantIds.Host;
    private DateTime _creationTime = TestConstants.Dates.UtcNow;
    private string _textMessage = TestConstants.Messages.TestMessage;
    private BackgroundJobMessageType _messageType = BackgroundJobMessageType.Info;

    public BackgroundJobMessageTestDataBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public BackgroundJobMessageTestDataBuilder WithTenantId(Guid? tenantId)
    {
        _tenantId = tenantId;
        return this;
    }

    public BackgroundJobMessageTestDataBuilder WithCreationTime(DateTime creationTime)
    {
        _creationTime = creationTime;
        return this;
    }

    public BackgroundJobMessageTestDataBuilder WithText(string text)
    {
        _textMessage = text;
        return this;
    }

    public BackgroundJobMessageTestDataBuilder WithType(BackgroundJobMessageType type)
    {
        _messageType = type;
        return this;
    }

    public BackgroundJobMessageEntity Build()
    {
        return new BackgroundJobMessageEntity(_id)
        {
            TenantId = _tenantId,
            CreationTime = _creationTime,
            TextMessage = _textMessage,
            MessageType = _messageType
        };
    }

    public static BackgroundJobMessageTestDataBuilder Create() => new();
}
