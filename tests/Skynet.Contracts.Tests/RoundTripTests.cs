using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using Skynet.Contracts;
using Xunit;

namespace Skynet.Contracts.Tests;

public static class SerializationTestData
{
    public static readonly JsonSerializerOptions SerializerOptions = CreateSerializerOptions();

    private static JsonSerializerOptions CreateSerializerOptions()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    }

    public static T RoundTrip<T>(T value)
    {
        var json = JsonSerializer.Serialize(value, SerializerOptions);
        return JsonSerializer.Deserialize<T>(json, SerializerOptions)!;
    }
}

public class RoundTripTests
{
    [Fact]
    public void AiTask_RoundTripsThroughJson()
    {
        var snapshot = new DateTimeOffset(2024, 11, 15, 10, 30, 0, TimeSpan.Zero);

        var original = new AiTask
        {
            TaskId = Guid.Parse("6cbd9d88-4c5f-4b1c-8f43-8205de50d2dc"),
            TenantId = "tenant-alpha",
            UserId = "user-42",
            Goal = "Refactor the planning engine",
            Status = TaskStatus.Planning,
            CurrentStepPath = "1.1",
            PlanVersion = 3,
            AgentThreadId = "thread-abc",
            MemorySessionId = "mem-123",
            CreatedAt = snapshot,
            UpdatedAt = snapshot.AddHours(2)
        };

        var clone = SerializationTestData.RoundTrip(original);

        clone.Should().BeEquivalentTo(original, options => options.WithStrictOrdering());
    }

    [Fact]
    public void AiPlan_RoundTripsThroughJson()
    {
        var step = CreatePlanStep();

        var original = new AiPlan
        {
            PlanId = Guid.Parse("7d86b038-7be6-4a9c-8ebc-24c8908778ee"),
            TaskId = Guid.Parse("9deaf5e0-4af1-4e28-bb42-42d46dc3b427"),
            Version = 4,
            Steps = new List<AiPlanStep> { step }
        };

        var clone = SerializationTestData.RoundTrip(original);

        clone.Should().BeEquivalentTo(original, options => options.WithStrictOrdering());
    }

    [Fact]
    public void AiPlanStep_RoundTripsThroughJson()
    {
        var original = CreatePlanStep();

        var clone = SerializationTestData.RoundTrip(original);

        clone.Should().BeEquivalentTo(original, options => options.WithStrictOrdering());
    }

    [Fact]
    public void MemorySession_RoundTripsThroughJson()
    {
        var original = new MemorySession
        {
            SessionId = "session-001",
            TenantId = "tenant-alpha",
            TaskId = Guid.Parse("d138c124-3ff1-4124-82fd-86aa0528c12c"),
            AgentThreadId = "thread-xyz",
            ActivePageId = "page-live"
        };

        var clone = SerializationTestData.RoundTrip(original);

        clone.Should().BeEquivalentTo(original, options => options.WithStrictOrdering());
    }

    [Fact]
    public void MemoryEvent_RoundTripsThroughJson()
    {
        var payload = JsonSerializer.SerializeToElement(new { summary = "ok" });
        var original = new MemoryEvent
        {
            EventId = "evt-001",
            SessionId = "session-001",
            Type = MemoryEventType.PlanCreated,
            Role = "planner",
            Content = "Created plan",
            Payload = payload,
            CreatedAt = new DateTimeOffset(2024, 11, 15, 10, 45, 0, TimeSpan.Zero)
        };

        var clone = SerializationTestData.RoundTrip(original);

        clone.Should().BeEquivalentTo(original, options => options.WithStrictOrdering().Excluding(e => e.Payload));

        var expectedPayload = original.Payload is JsonElement expected ? expected.GetRawText() : null;
        var actualPayload = clone.Payload is JsonElement actual ? actual.GetRawText() : null;

        actualPayload.Should().Be(expectedPayload);
    }

    [Fact]
    public void MemoryPage_RoundTripsThroughJson()
    {
        var original = new MemoryPage
        {
            PageId = "page-001",
            SessionId = "session-001",
            Role = MemoryPageRole.Working,
            Summary = "Conversation so far",
            TokensApprox = 4096,
            CreatedAt = new DateTimeOffset(2024, 11, 15, 11, 0, 0, TimeSpan.Zero),
            LastUsedAt = new DateTimeOffset(2024, 11, 15, 12, 0, 0, TimeSpan.Zero)
        };

        var clone = SerializationTestData.RoundTrip(original);

        clone.Should().BeEquivalentTo(original, options => options.WithStrictOrdering());
    }

    [Fact]
    public void Enums_MatchSpecification()
    {
        Enum.GetValues<TaskStatus>().Should().Equal(new[]
        {
            TaskStatus.Created,
            TaskStatus.Planning,
            TaskStatus.Running,
            TaskStatus.Waiting,
            TaskStatus.Completed,
            TaskStatus.Failed,
            TaskStatus.Cancelled
        });

        Enum.GetValues<StepType>().Should().Equal(new[]
        {
            StepType.UnaryOperation,
            StepType.WorkflowSubplan,
            StepType.NestedSubplan,
            StepType.CodexExecution
        });

        Enum.GetValues<StepStatus>().Should().Equal(new[]
        {
            StepStatus.Pending,
            StepStatus.Running,
            StepStatus.Waiting,
            StepStatus.Completed,
            StepStatus.Failed,
            StepStatus.Skipped
        });

        Enum.GetValues<MemoryEventType>().Should().Equal(new[]
        {
            MemoryEventType.UserGoal,
            MemoryEventType.UserMessage,
            MemoryEventType.AgentMessage,
            MemoryEventType.PlanCreated,
            MemoryEventType.PlanChanged,
            MemoryEventType.ToolResult,
            MemoryEventType.WorkflowStarted,
            MemoryEventType.WorkflowCompleted,
            MemoryEventType.CodexExecutionStarted,
            MemoryEventType.CodexExecutionCompleted,
            MemoryEventType.Error
        });

        Enum.GetValues<MemoryPageRole>().Should().Equal(new[]
        {
            MemoryPageRole.Working,
            MemoryPageRole.Archive
        });

        Enum.GetValues<CodexSuccessCriteriaType>().Should().Equal(new[]
        {
            CodexSuccessCriteriaType.File,
            CodexSuccessCriteriaType.Json,
            CodexSuccessCriteriaType.ExitCode
        });
    }

    private static AiPlanStep CreatePlanStep()
    {
        return new AiPlanStep
        {
            StepId = "1",
            Title = "Collect telemetry",
            Description = "Gather logs from Memo",
            StepType = StepType.UnaryOperation,
            Status = StepStatus.Running,
            AttemptCount = 1,
            GoalHash = "abc123",
            Operation = new OperationDescriptor
            {
                ToolName = "memo.fetch_context",
                ArgumentsTemplate = "{\"sessionId\":\"session-001\"}",
                ArgumentsSchemaRef = "schemas/memo/context"
            },
            Workflow = new WorkflowDescriptor
            {
                WorkflowName = "memo-sync",
                TemplateName = "default",
                InputTemplate = "{}",
                CorrelationKey = "wf-001"
            },
            Codex = new CodexExecutionDescriptor
            {
                WorkspaceTemplate = "default",
                MissionFile = "mission.md",
                MissionContentTemplate = "Solve {{goal}}",
                TimeoutSeconds = 1800,
                SuccessCriteria = new CodexSuccessCriteria
                {
                    Type = CodexSuccessCriteriaType.File,
                    Path = "codex-result.json"
                },
                ExtraToolsProfile = "full"
            },
            Substeps = new List<AiPlanStep>
            {
                new()
                {
                    StepId = "1.1",
                    Title = "Call tool",
                    Description = "Invoke memo",
                    StepType = StepType.UnaryOperation,
                    Status = StepStatus.Pending,
                    AttemptCount = 0,
                    Operation = new OperationDescriptor
                    {
                        ToolName = "memo.fetch_context",
                        ArgumentsTemplate = "{}"
                    },
                    Substeps = Array.Empty<AiPlanStep>()
                }
            }
        };
    }
}
