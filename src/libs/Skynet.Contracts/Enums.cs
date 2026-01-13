using System.Text.Json.Serialization;

namespace Skynet.Contracts;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TaskStatus
{
    Created,
    Planning,
    Running,
    Waiting,
    Completed,
    Failed,
    Cancelled
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StepType
{
    UnaryOperation,
    WorkflowSubplan,
    NestedSubplan,
    CodexExecution
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StepStatus
{
    Pending,
    Running,
    Waiting,
    Completed,
    Failed,
    Skipped
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MemoryEventType
{
    UserGoal,
    UserMessage,
    AgentMessage,
    PlanCreated,
    PlanChanged,
    ToolResult,
    WorkflowStarted,
    WorkflowCompleted,
    CodexExecutionStarted,
    CodexExecutionCompleted,
    Error
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MemoryPageRole
{
    Working,
    Archive
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CodexSuccessCriteriaType
{
    File,
    Json,
    ExitCode
}
