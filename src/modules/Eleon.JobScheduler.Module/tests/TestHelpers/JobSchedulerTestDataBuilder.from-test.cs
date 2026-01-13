using System;
using System.Collections.Generic;
using Common.Module.Constants;
using VPortal.JobScheduler.Module.Entities;
using JobScheduler.Module.TestHelpers;

namespace JobScheduler.Module.TestHelpers;

public class TaskTestDataBuilderFromTest
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

    public TaskTestDataBuilderFromTest WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public TaskTestDataBuilderFromTest WithTenantId(Guid? tenantId)
    {
        _tenantId = tenantId;
        return this;
    }

    public TaskTestDataBuilderFromTest WithName(string name)
    {
        _name = name;
        return this;
    }

    public TaskTestDataBuilderFromTest WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public TaskTestDataBuilderFromTest WithIsActive(bool isActive)
    {
        _isActive = isActive;
        return this;
    }

    public TaskTestDataBuilderFromTest WithCanRunManually(bool canRunManually)
    {
        _canRunManually = canRunManually;
        return this;
    }

    public TaskTestDataBuilderFromTest WithAllowForceStop(bool allowForceStop)
    {
        _allowForceStop = allowForceStop;
        return this;
    }

    public TaskTestDataBuilderFromTest WithStatus(JobSchedulerTaskStatus status)
    {
        _status = status;
        return this;
    }

    public TaskTestDataBuilderFromTest WithTimeout(TimeSpan? timeout)
    {
        _timeout = timeout;
        return this;
    }

    public TaskTestDataBuilderFromTest WithRetrySettings(TimeSpan? interval, int maxAttempts)
    {
        _restartAfterFailInterval = interval;
        _restartAfterFailMaxAttempts = maxAttempts;
        return this;
    }

    public TaskTestDataBuilderFromTest WithCurrentRetryAttempt(int attempt)
    {
        _currentRetryAttempt = attempt;
        return this;
    }

    public TaskTestDataBuilderFromTest WithLastRunTime(DateTime? lastRunTime)
    {
        _lastRunTimeUtc = lastRunTime;
        return this;
    }

    public TaskTestDataBuilderFromTest WithNextRunTime(DateTime? nextRunTime)
    {
        _nextRunTimeUtc = nextRunTime;
        return this;
    }

    public TaskTestDataBuilderFromTest WithOnFailureRecepients(string recepients)
    {
        _onFailureRecepients = recepients ?? string.Empty;
        return this;
    }

    public TaskTestDataBuilderFromTest WithTrigger(TriggerEntity trigger)
    {
        _triggers.Add(trigger);
        return this;
    }

    public TaskTestDataBuilderFromTest WithAction(ActionEntity action)
    {
        _actions.Add(action);
        return this;
    }

    public TaskTestDataBuilderFromTest WithActions(List<ActionEntity> actions)
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

    public static TaskTestDataBuilderFromTest Create() => new();
}

public class ActionTestDataBuilderFromTest
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

    public ActionTestDataBuilderFromTest WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public ActionTestDataBuilderFromTest WithTenantId(Guid? tenantId)
    {
        _tenantId = tenantId;
        return this;
    }

    public ActionTestDataBuilderFromTest WithTaskId(Guid taskId)
    {
        _taskId = taskId;
        return this;
    }

    public ActionTestDataBuilderFromTest WithDisplayName(string displayName)
    {
        _displayName = displayName;
        return this;
    }

    public ActionTestDataBuilderFromTest WithEventName(string eventName)
    {
        _eventName = eventName;
        return this;
    }

    public ActionTestDataBuilderFromTest WithActionParams(string actionParams)
    {
        _actionParams = actionParams;
        return this;
    }

    public ActionTestDataBuilderFromTest WithActionExtraParams(string extraParams)
    {
        _actionExtraParams = extraParams ?? string.Empty;
        return this;
    }

    public ActionTestDataBuilderFromTest WithRetrySettings(TimeSpan? interval, int maxAttempts)
    {
        _retryInterval = interval;
        _maxRetryAttempts = maxAttempts;
        return this;
    }

    public ActionTestDataBuilderFromTest WithTimeoutInMinutes(int timeout)
    {
        _timeoutInMinutes = timeout;
        return this;
    }

    public ActionTestDataBuilderFromTest WithParamsFormat(TextFormat format)
    {
        _paramsFormat = format;
        return this;
    }

    public ActionTestDataBuilderFromTest WithOnFailureRecepients(string recepients)
    {
        _onFailureRecepients = recepients ?? string.Empty;
        return this;
    }

    public ActionTestDataBuilderFromTest WithParentAction(Guid parentActionId)
    {
        _parentActions.Add(new ActionParentEntity(_id, parentActionId));
        return this;
    }

    public ActionTestDataBuilderFromTest WithParentActions(List<ActionParentEntity> parentActions)
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

    public static ActionTestDataBuilderFromTest Create() => new();
}

public class TriggerTestDataBuilderFromTest
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

    public TriggerTestDataBuilderFromTest WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public TriggerTestDataBuilderFromTest WithTenantId(Guid? tenantId)
    {
        _tenantId = tenantId;
        return this;
    }

    public TriggerTestDataBuilderFromTest WithTaskId(Guid taskId)
    {
        _taskId = taskId;
        return this;
    }

    public TriggerTestDataBuilderFromTest WithName(string name)
    {
        _name = name;
        return this;
    }

    public TriggerTestDataBuilderFromTest WithIsEnabled(bool isEnabled)
    {
        _isEnabled = isEnabled;
        return this;
    }

    public TriggerTestDataBuilderFromTest WithLastRun(DateTime? lastRun)
    {
        _lastRun = lastRun;
        return this;
    }

    public TriggerTestDataBuilderFromTest WithStartUtc(DateTime startUtc)
    {
        _startUtc = startUtc;
        return this;
    }

    public TriggerTestDataBuilderFromTest WithExpireUtc(DateTime? expireUtc)
    {
        _expireUtc = expireUtc;
        return this;
    }

    public TriggerTestDataBuilderFromTest WithPeriodType(TimePeriodType periodType)
    {
        _periodType = periodType;
        return this;
    }

    public TriggerTestDataBuilderFromTest WithPeriod(int period)
    {
        _period = period;
        return this;
    }

    public TriggerTestDataBuilderFromTest WithDaysOfWeek(int daysOfWeek)
    {
        _daysOfWeek = daysOfWeek;
        return this;
    }

    public TriggerTestDataBuilderFromTest WithDaysOfMonth(long daysOfMonth)
    {
        _daysOfMonth = daysOfMonth;
        return this;
    }

    public TriggerTestDataBuilderFromTest WithMonths(int months)
    {
        _months = months;
        return this;
    }

    public TriggerTestDataBuilderFromTest WithRepeatTask(bool repeatTask)
    {
        _repeatTask = repeatTask;
        return this;
    }

    public TriggerTestDataBuilderFromTest WithRepeatInterval(int units, TimeUnit unitType)
    {
        _repeatIntervalUnits = units;
        _repeatIntervalUnitType = unitType;
        return this;
    }

    public TriggerTestDataBuilderFromTest WithRepeatDuration(int? units, TimeUnit unitType)
    {
        _repeatDurationUnits = units;
        _repeatDurationUnitType = unitType;
        return this;
    }

    public TriggerTestDataBuilderFromTest WithDaysOfWeekList(IList<int> daysOfWeekList)
    {
        _daysOfWeekList = daysOfWeekList;
        return this;
    }

    public TriggerTestDataBuilderFromTest WithDaysOfMonthList(IList<int> daysOfMonthList)
    {
        _daysOfMonthList = daysOfMonthList;
        return this;
    }

    public TriggerTestDataBuilderFromTest WithMonthsList(IList<int> monthsList)
    {
        _monthsList = monthsList;
        return this;
    }

    public TriggerTestDataBuilderFromTest WithDaysOfWeekOccurencesList(IList<int> occurencesList)
    {
        _daysOfWeekOccurencesList = occurencesList;
        return this;
    }

    public TriggerTestDataBuilderFromTest WithDaysOfWeekOccurencesLast(bool last)
    {
        _daysOfWeekOccurencesLast = last;
        return this;
    }

    public TriggerTestDataBuilderFromTest WithDaysOfMonthLast(bool last)
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

    public static TriggerTestDataBuilderFromTest Create() => new();
}

public class TaskExecutionTestDataBuilderFromTest
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

    public TaskExecutionTestDataBuilderFromTest WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public TaskExecutionTestDataBuilderFromTest WithTenantId(Guid? tenantId)
    {
        _tenantId = tenantId;
        return this;
    }

    public TaskExecutionTestDataBuilderFromTest WithTaskId(Guid taskId)
    {
        _taskId = taskId;
        return this;
    }

    public TaskExecutionTestDataBuilderFromTest WithStatus(JobSchedulerTaskExecutionStatus status)
    {
        _status = status;
        return this;
    }

    public TaskExecutionTestDataBuilderFromTest WithRunnedByUser(Guid? userId, string userName)
    {
        _runnedByUserId = userId;
        _runnedByUserName = userName;
        return this;
    }

    public TaskExecutionTestDataBuilderFromTest WithRunnedByTrigger(Guid? triggerId, string triggerName)
    {
        _runnedByTriggerId = triggerId;
        _runnedByTriggerName = triggerName;
        return this;
    }

    public TaskExecutionTestDataBuilderFromTest WithStartedAt(DateTime? startedAt)
    {
        _startedAtUtc = startedAt;
        return this;
    }

    public TaskExecutionTestDataBuilderFromTest WithFinishedAt(DateTime? finishedAt)
    {
        _finishedAtUtc = finishedAt;
        return this;
    }

    public TaskExecutionTestDataBuilderFromTest WithActionExecution(ActionExecutionEntity actionExecution)
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

    public static TaskExecutionTestDataBuilderFromTest Create() => new();
}

public class ActionExecutionTestDataBuilderFromTest
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

    public ActionExecutionTestDataBuilderFromTest WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public ActionExecutionTestDataBuilderFromTest WithTenantId(Guid? tenantId)
    {
        _tenantId = tenantId;
        return this;
    }

    public ActionExecutionTestDataBuilderFromTest WithTaskExecutionId(Guid taskExecutionId)
    {
        _taskExecutionId = taskExecutionId;
        return this;
    }

    public ActionExecutionTestDataBuilderFromTest WithActionName(string actionName)
    {
        _actionName = actionName;
        return this;
    }

    public ActionExecutionTestDataBuilderFromTest WithEventName(string eventName)
    {
        _eventName = eventName;
        return this;
    }

    public ActionExecutionTestDataBuilderFromTest WithStatus(JobSchedulerActionExecutionStatus status)
    {
        _status = status;
        return this;
    }

    public ActionExecutionTestDataBuilderFromTest WithStartedAt(DateTime? startedAt)
    {
        _startedAtUtc = startedAt;
        return this;
    }

    public ActionExecutionTestDataBuilderFromTest WithCompletedAt(DateTime? completedAt)
    {
        _completedAtUtc = completedAt;
        return this;
    }

    public ActionExecutionTestDataBuilderFromTest WithJobId(Guid? jobId)
    {
        _jobId = jobId;
        return this;
    }

    public ActionExecutionTestDataBuilderFromTest WithActionId(Guid? actionId)
    {
        _actionId = actionId;
        return this;
    }

    public ActionExecutionTestDataBuilderFromTest WithActionParams(string actionParams)
    {
        _actionParams = actionParams;
        return this;
    }

    public ActionExecutionTestDataBuilderFromTest WithActionExtraParams(string extraParams)
    {
        _actionExtraParams = extraParams ?? string.Empty;
        return this;
    }

    public ActionExecutionTestDataBuilderFromTest WithParentActionExecution(Guid parentActionExecutionId)
    {
        _parentActionExecutions.Add(new ActionExecutionParentEntity(_id, parentActionExecutionId));
        return this;
    }

    public ActionExecutionTestDataBuilderFromTest WithParentActionExecutions(List<ActionExecutionParentEntity> parentExecutions)
    {
        _parentActionExecutions = parentExecutions ?? new List<ActionExecutionParentEntity>();
        return this;
    }

    public ActionExecutionTestDataBuilderFromTest WithStatusChangedBy(string changedBy, bool manually = false)
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

    public static ActionExecutionTestDataBuilderFromTest Create() => new();
}
