using System;
using System.Collections.Generic;
using Common.Module.Constants;
using VPortal.JobScheduler.Module.Entities;
using JobScheduler.Module.TestHelpers;

namespace JobScheduler.Module.TestHelpers;

public class TaskTestDataBuilder
{
    private Guid _id = TestConstants.TaskIds.Task1;
    private Guid? _tenantId = TestConstants.TenantIds.Host;
    private bool _isActive = false;
    private string _name = TestConstants.TaskNames.TestTask;
    private string _description = "Test task description";
    private bool _canRunManually = false;
    private TimeSpan? _restartAfterFailInterval = null;
    private int _restartAfterFailMaxAttempts = 0;
    private int _currentRetryAttempt = 0;
    private TimeSpan? _timeout = TimeSpan.FromMinutes(60);
    private bool _allowForceStop = false;
    private DateTime? _lastRunTimeUtc = null;
    private DateTime? _nextRunTimeUtc = null;
    private JobSchedulerTaskStatus _status = JobSchedulerTaskStatus.Inactive;
    private string _onFailureRecepients = string.Empty;
    private List<TriggerEntity> _triggers = new();
    private List<TaskExecutionEntity> _executions = new();
    private List<ActionEntity> _actions = new();

    public TaskTestDataBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public TaskTestDataBuilder WithTenantId(Guid? tenantId)
    {
        _tenantId = tenantId;
        return this;
    }

    public TaskTestDataBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public TaskTestDataBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public TaskTestDataBuilder WithIsActive(bool isActive)
    {
        _isActive = isActive;
        return this;
    }

    public TaskTestDataBuilder WithCanRunManually(bool canRunManually)
    {
        _canRunManually = canRunManually;
        return this;
    }

    public TaskTestDataBuilder WithAllowForceStop(bool allowForceStop)
    {
        _allowForceStop = allowForceStop;
        return this;
    }

    public TaskTestDataBuilder WithStatus(JobSchedulerTaskStatus status)
    {
        _status = status;
        return this;
    }

    public TaskTestDataBuilder WithTimeout(TimeSpan? timeout)
    {
        _timeout = timeout;
        return this;
    }

    public TaskTestDataBuilder WithRetrySettings(TimeSpan? interval, int maxAttempts)
    {
        _restartAfterFailInterval = interval;
        _restartAfterFailMaxAttempts = maxAttempts;
        return this;
    }

    public TaskTestDataBuilder WithCurrentRetryAttempt(int attempt)
    {
        _currentRetryAttempt = attempt;
        return this;
    }

    public TaskTestDataBuilder WithLastRunTime(DateTime? lastRunTime)
    {
        _lastRunTimeUtc = lastRunTime;
        return this;
    }

    public TaskTestDataBuilder WithNextRunTime(DateTime? nextRunTime)
    {
        _nextRunTimeUtc = nextRunTime;
        return this;
    }

    public TaskTestDataBuilder WithOnFailureRecepients(string recepients)
    {
        _onFailureRecepients = recepients ?? string.Empty;
        return this;
    }

    public TaskTestDataBuilder WithTrigger(TriggerEntity trigger)
    {
        _triggers.Add(trigger);
        return this;
    }

    public TaskTestDataBuilder WithAction(ActionEntity action)
    {
        _actions.Add(action);
        return this;
    }

    public TaskTestDataBuilder WithActions(List<ActionEntity> actions)
    {
        _actions = actions ?? new List<ActionEntity>();
        return this;
    }

    public TaskEntity Build()
    {
        var task = new TaskEntity(_id)
        {
            TenantId = _tenantId,
            IsActive = _isActive,
            Name = _name,
            Description = _description,
            CanRunManually = _canRunManually,
            RestartAfterFailInterval = _restartAfterFailInterval,
            RestartAfterFailMaxAttempts = _restartAfterFailMaxAttempts,
            CurrentRetryAttempt = _currentRetryAttempt,
            Timeout = _timeout,
            AllowForceStop = _allowForceStop,
            LastRunTimeUtc = _lastRunTimeUtc,
            NextRunTimeUtc = _nextRunTimeUtc,
            Status = _status,
            OnFailureRecepients = _onFailureRecepients ?? string.Empty
        };

        // Ensure collections are initialized
        if (task.Triggers == null) task.Triggers = new List<TriggerEntity>();
        if (task.Executions == null) task.Executions = new List<TaskExecutionEntity>();
        if (task.Actions == null) task.Actions = new List<ActionEntity>();

        foreach (var trigger in _triggers)
        {
            task.Triggers.Add(trigger);
        }

        foreach (var execution in _executions)
        {
            task.Executions.Add(execution);
        }

        foreach (var action in _actions)
        {
            task.Actions.Add(action);
        }

        return task;
    }

    public static TaskTestDataBuilder Create() => new();
}

public class ActionTestDataBuilder
{
    private Guid _id = TestConstants.ActionIds.Action1;
    private Guid? _tenantId = TestConstants.TenantIds.Host;
    private Guid _taskId = TestConstants.TaskIds.Task1;
    private string _displayName = TestConstants.ActionNames.TestAction;
    private string _eventName = "TestEvent";
    private string _actionParams = "{}";
    private string _actionExtraParams = string.Empty;
    private TimeSpan? _retryInterval = null;
    private int _maxRetryAttempts = 0;
    private int _timeoutInMinutes = 60;
    private string _onFailureRecepients = string.Empty;
    private TextFormat _paramsFormat = TextFormat.Json;
    private List<ActionParentEntity> _parentActions = new();

    public ActionTestDataBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public ActionTestDataBuilder WithTenantId(Guid? tenantId)
    {
        _tenantId = tenantId;
        return this;
    }

    public ActionTestDataBuilder WithTaskId(Guid taskId)
    {
        _taskId = taskId;
        return this;
    }

    public ActionTestDataBuilder WithDisplayName(string displayName)
    {
        _displayName = displayName;
        return this;
    }

    public ActionTestDataBuilder WithEventName(string eventName)
    {
        _eventName = eventName;
        return this;
    }

    public ActionTestDataBuilder WithActionParams(string actionParams)
    {
        _actionParams = actionParams;
        return this;
    }

    public ActionTestDataBuilder WithActionExtraParams(string extraParams)
    {
        _actionExtraParams = extraParams ?? string.Empty;
        return this;
    }

    public ActionTestDataBuilder WithRetrySettings(TimeSpan? interval, int maxAttempts)
    {
        _retryInterval = interval;
        _maxRetryAttempts = maxAttempts;
        return this;
    }

    public ActionTestDataBuilder WithTimeoutInMinutes(int timeout)
    {
        _timeoutInMinutes = timeout;
        return this;
    }

    public ActionTestDataBuilder WithParamsFormat(TextFormat format)
    {
        _paramsFormat = format;
        return this;
    }

    public ActionTestDataBuilder WithOnFailureRecepients(string recepients)
    {
        _onFailureRecepients = recepients ?? string.Empty;
        return this;
    }

    public ActionTestDataBuilder WithParentAction(Guid parentActionId)
    {
        _parentActions.Add(new ActionParentEntity(_id, parentActionId));
        return this;
    }

    public ActionTestDataBuilder WithParentActions(List<ActionParentEntity> parentActions)
    {
        _parentActions = parentActions ?? new List<ActionParentEntity>();
        return this;
    }

    public ActionEntity Build()
    {
        return new ActionEntity(_id)
        {
            TenantId = _tenantId,
            TaskId = _taskId,
            DisplayName = _displayName,
            EventName = _eventName,
            ActionParams = _actionParams,
            ActionExtraParams = _actionExtraParams ?? string.Empty,
            RetryInterval = _retryInterval,
            MaxRetryAttempts = _maxRetryAttempts,
            TimeoutInMinutes = _timeoutInMinutes,
            OnFailureRecepients = _onFailureRecepients ?? string.Empty,
            ParamsFormat = _paramsFormat,
            ParentActions = _parentActions
        };
    }

    public static ActionTestDataBuilder Create() => new();
}

public class TriggerTestDataBuilder
{
    private Guid _id = TestConstants.TriggerIds.Trigger1;
    private Guid? _tenantId = TestConstants.TenantIds.Host;
    private Guid _taskId = TestConstants.TaskIds.Task1;
    private string _name = TestConstants.TriggerNames.DailyTrigger;
    private bool _isEnabled = true;
    private DateTime? _lastRun = null;
    private DateTime _startUtc = TestConstants.Dates.UtcNow;
    private DateTime? _expireUtc = null;
    private TimePeriodType _periodType = TimePeriodType.Daily;
    private int _period = 1;
    private int _daysOfWeek = 0;
    private int _daysOfWeekOccurences = 0;
    private long _daysOfMonth = 0;
    private int _months = 0;
    private bool _repeatTask = false;
    private int _repeatIntervalUnits = 0;
    private TimeUnit _repeatIntervalUnitType = TimeUnit.Minutes;
    private int? _repeatDurationUnits = null;
    private TimeUnit _repeatDurationUnitType = TimeUnit.Minutes;
    private IList<int> _daysOfWeekList = null;
    private IList<int> _daysOfMonthList = null;
    private IList<int> _monthsList = null;
    private IList<int> _daysOfWeekOccurencesList = null;
    private bool _daysOfWeekOccurencesLast = false;
    private bool _daysOfMonthLast = false;

    public TriggerTestDataBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public TriggerTestDataBuilder WithTenantId(Guid? tenantId)
    {
        _tenantId = tenantId;
        return this;
    }

    public TriggerTestDataBuilder WithTaskId(Guid taskId)
    {
        _taskId = taskId;
        return this;
    }

    public TriggerTestDataBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public TriggerTestDataBuilder WithIsEnabled(bool isEnabled)
    {
        _isEnabled = isEnabled;
        return this;
    }

    public TriggerTestDataBuilder WithLastRun(DateTime? lastRun)
    {
        _lastRun = lastRun;
        return this;
    }

    public TriggerTestDataBuilder WithStartUtc(DateTime startUtc)
    {
        _startUtc = startUtc;
        return this;
    }

    public TriggerTestDataBuilder WithExpireUtc(DateTime? expireUtc)
    {
        _expireUtc = expireUtc;
        return this;
    }

    public TriggerTestDataBuilder WithPeriodType(TimePeriodType periodType)
    {
        _periodType = periodType;
        return this;
    }

    public TriggerTestDataBuilder WithPeriod(int period)
    {
        _period = period;
        return this;
    }

    public TriggerTestDataBuilder WithDaysOfWeek(int daysOfWeek)
    {
        _daysOfWeek = daysOfWeek;
        return this;
    }

    public TriggerTestDataBuilder WithDaysOfMonth(long daysOfMonth)
    {
        _daysOfMonth = daysOfMonth;
        return this;
    }

    public TriggerTestDataBuilder WithMonths(int months)
    {
        _months = months;
        return this;
    }

    public TriggerTestDataBuilder WithRepeatTask(bool repeatTask)
    {
        _repeatTask = repeatTask;
        return this;
    }

    public TriggerTestDataBuilder WithRepeatInterval(int units, TimeUnit unitType)
    {
        _repeatIntervalUnits = units;
        _repeatIntervalUnitType = unitType;
        return this;
    }

    public TriggerTestDataBuilder WithRepeatDuration(int? units, TimeUnit unitType)
    {
        _repeatDurationUnits = units;
        _repeatDurationUnitType = unitType;
        return this;
    }

    public TriggerTestDataBuilder WithDaysOfWeekList(IList<int> daysOfWeekList)
    {
        _daysOfWeekList = daysOfWeekList;
        return this;
    }

    public TriggerTestDataBuilder WithDaysOfMonthList(IList<int> daysOfMonthList)
    {
        _daysOfMonthList = daysOfMonthList;
        return this;
    }

    public TriggerTestDataBuilder WithMonthsList(IList<int> monthsList)
    {
        _monthsList = monthsList;
        return this;
    }

    public TriggerTestDataBuilder WithDaysOfWeekOccurencesList(IList<int> occurencesList)
    {
        _daysOfWeekOccurencesList = occurencesList;
        return this;
    }

    public TriggerTestDataBuilder WithDaysOfWeekOccurencesLast(bool last)
    {
        _daysOfWeekOccurencesLast = last;
        return this;
    }

    public TriggerTestDataBuilder WithDaysOfMonthLast(bool last)
    {
        _daysOfMonthLast = last;
        return this;
    }

    public TriggerEntity Build()
    {
        var trigger = new TriggerEntity(_id)
        {
            TenantId = _tenantId,
            TaskId = _taskId,
            Name = _name,
            IsEnabled = _isEnabled,
            LastRun = _lastRun,
            StartUtc = _startUtc,
            ExpireUtc = _expireUtc,
            PeriodType = _periodType,
            Period = _period,
            DaysOfWeek = _daysOfWeek,
            DaysOfWeekOccurences = _daysOfWeekOccurences,
            DaysOfMonth = _daysOfMonth,
            Months = _months,
            RepeatTask = _repeatTask,
            RepeatIntervalUnits = _repeatIntervalUnits,
            RepeatIntervalUnitType = _repeatIntervalUnitType,
            RepeatDurationUnits = _repeatDurationUnits,
            RepeatDurationUnitType = _repeatDurationUnitType
        };

        // Set display-only properties
        trigger.DaysOfWeekList = _daysOfWeekList;
        trigger.DaysOfMonthList = _daysOfMonthList;
        trigger.MonthsList = _monthsList;
        trigger.DaysOfWeekOccurencesList = _daysOfWeekOccurencesList;
        trigger.DaysOfWeekOccurencesLast = _daysOfWeekOccurencesLast;
        trigger.DaysOfMonthLast = _daysOfMonthLast;

        return trigger;
    }

    public static TriggerTestDataBuilder Create() => new();
}

public class TaskExecutionTestDataBuilder
{
    private Guid _id = TestConstants.TaskExecutionIds.Execution1;
    private Guid? _tenantId = TestConstants.TenantIds.Host;
    private Guid _taskId = TestConstants.TaskIds.Task1;
    private JobSchedulerTaskExecutionStatus _status = JobSchedulerTaskExecutionStatus.Initializing;
    private Guid? _runnedByUserId = null;
    private string _runnedByUserName = null;
    private Guid? _runnedByTriggerId = null;
    private string _runnedByTriggerName = null;
    private DateTime? _startedAtUtc = null;
    private DateTime? _finishedAtUtc = null;
    private List<ActionExecutionEntity> _actionExecutions = new();

    public TaskExecutionTestDataBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public TaskExecutionTestDataBuilder WithTenantId(Guid? tenantId)
    {
        _tenantId = tenantId;
        return this;
    }

    public TaskExecutionTestDataBuilder WithTaskId(Guid taskId)
    {
        _taskId = taskId;
        return this;
    }

    public TaskExecutionTestDataBuilder WithStatus(JobSchedulerTaskExecutionStatus status)
    {
        _status = status;
        return this;
    }

    public TaskExecutionTestDataBuilder WithRunnedByUser(Guid? userId, string userName)
    {
        _runnedByUserId = userId;
        _runnedByUserName = userName;
        return this;
    }

    public TaskExecutionTestDataBuilder WithRunnedByTrigger(Guid? triggerId, string triggerName)
    {
        _runnedByTriggerId = triggerId;
        _runnedByTriggerName = triggerName;
        return this;
    }

    public TaskExecutionTestDataBuilder WithStartedAt(DateTime? startedAt)
    {
        _startedAtUtc = startedAt;
        return this;
    }

    public TaskExecutionTestDataBuilder WithFinishedAt(DateTime? finishedAt)
    {
        _finishedAtUtc = finishedAt;
        return this;
    }

    public TaskExecutionTestDataBuilder WithActionExecution(ActionExecutionEntity actionExecution)
    {
        _actionExecutions.Add(actionExecution);
        return this;
    }

    public TaskExecutionEntity Build()
    {
        var execution = new TaskExecutionEntity(_id, _taskId)
        {
            TenantId = _tenantId,
            Status = _status,
            RunnedByUserId = _runnedByUserId,
            RunnedByUserName = _runnedByUserName,
            RunnedByTriggerId = _runnedByTriggerId,
            RunnedByTriggerName = _runnedByTriggerName,
            StartedAtUtc = _startedAtUtc,
            FinishedAtUtc = _finishedAtUtc,
            ActionExecutions = new List<ActionExecutionEntity>()
        };

        foreach (var actionExecution in _actionExecutions)
        {
            execution.ActionExecutions.Add(actionExecution);
        }

        return execution;
    }

    public static TaskExecutionTestDataBuilder Create() => new();
}

public class ActionExecutionTestDataBuilder
{
    private Guid _id = TestConstants.ActionExecutionIds.ActionExecution1;
    private Guid? _tenantId = TestConstants.TenantIds.Host;
    private Guid _taskExecutionId = TestConstants.TaskExecutionIds.Execution1;
    private string _actionName = TestConstants.ActionNames.TestAction;
    private string _eventName = "TestEvent";
    private JobSchedulerActionExecutionStatus _status = JobSchedulerActionExecutionStatus.NotStarted;
    private DateTime? _startedAtUtc = null;
    private DateTime? _completedAtUtc = null;
    private Guid? _jobId = null;
    private Guid? _actionId = null;
    private string _actionParams = "{}";
    private string _actionExtraParams = string.Empty;
    private string _statusChangedBy = string.Empty;
    private bool _isStatusChangedManually = false;
    private List<ActionExecutionParentEntity> _parentActionExecutions = new();

    public ActionExecutionTestDataBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public ActionExecutionTestDataBuilder WithTenantId(Guid? tenantId)
    {
        _tenantId = tenantId;
        return this;
    }

    public ActionExecutionTestDataBuilder WithTaskExecutionId(Guid taskExecutionId)
    {
        _taskExecutionId = taskExecutionId;
        return this;
    }

    public ActionExecutionTestDataBuilder WithActionName(string actionName)
    {
        _actionName = actionName;
        return this;
    }

    public ActionExecutionTestDataBuilder WithEventName(string eventName)
    {
        _eventName = eventName;
        return this;
    }

    public ActionExecutionTestDataBuilder WithStatus(JobSchedulerActionExecutionStatus status)
    {
        _status = status;
        return this;
    }

    public ActionExecutionTestDataBuilder WithStartedAt(DateTime? startedAt)
    {
        _startedAtUtc = startedAt;
        return this;
    }

    public ActionExecutionTestDataBuilder WithCompletedAt(DateTime? completedAt)
    {
        _completedAtUtc = completedAt;
        return this;
    }

    public ActionExecutionTestDataBuilder WithJobId(Guid? jobId)
    {
        _jobId = jobId;
        return this;
    }

    public ActionExecutionTestDataBuilder WithActionId(Guid? actionId)
    {
        _actionId = actionId;
        return this;
    }

    public ActionExecutionTestDataBuilder WithActionParams(string actionParams)
    {
        _actionParams = actionParams;
        return this;
    }

    public ActionExecutionTestDataBuilder WithActionExtraParams(string extraParams)
    {
        _actionExtraParams = extraParams ?? string.Empty;
        return this;
    }

    public ActionExecutionTestDataBuilder WithParentActionExecution(Guid parentActionExecutionId)
    {
        _parentActionExecutions.Add(new ActionExecutionParentEntity(_id, parentActionExecutionId));
        return this;
    }

    public ActionExecutionTestDataBuilder WithParentActionExecutions(List<ActionExecutionParentEntity> parentExecutions)
    {
        _parentActionExecutions = parentExecutions ?? new List<ActionExecutionParentEntity>();
        return this;
    }

    public ActionExecutionTestDataBuilder WithStatusChangedBy(string changedBy, bool manually = false)
    {
        _statusChangedBy = changedBy ?? string.Empty;
        _isStatusChangedManually = manually;
        return this;
    }

    public ActionExecutionEntity Build()
    {
        return new ActionExecutionEntity(_id, _taskExecutionId)
        {
            TenantId = _tenantId,
            ActionName = _actionName,
            EventName = _eventName,
            Status = _status,
            StartedAtUtc = _startedAtUtc,
            CompletedAtUtc = _completedAtUtc,
            JobId = _jobId,
            ActionId = _actionId,
            ActionParams = _actionParams,
            ActionExtraParams = _actionExtraParams ?? string.Empty,
            StatusChangedBy = _statusChangedBy ?? string.Empty,
            IsStatusChangedManually = _isStatusChangedManually,
            ParentActionExecutions = _parentActionExecutions
        };
    }

    public static ActionExecutionTestDataBuilder Create() => new();
}
