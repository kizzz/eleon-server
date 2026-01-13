using Common.EventBus.Module;
using Common.Module.Constants;
using Logging.Module;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using VPortal.Infrastructure.Module.Entities;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;
using VPortal.Lifecycle.Feature.Module.DomainServices;
using VPortal.Lifecycle.Feature.Module.Entities.Conditions;
using VPortal.Lifecycle.Feature.Module.Repositories.Conditions;
using Xunit;

namespace VPortal.Lifecycle.Feature.Module.Conditions;

public class ConditionDomainServiceTests : ModuleDomainTestBase<ModuleDomainTestModule>
{
    private readonly Mock<IVportalLogger<ConditionDomainService>> _mockLogger;
    private readonly Mock<IDistributedEventBus> _mockEventBus;
    private readonly Mock<IConditionRepository> _mockRepository;
    private readonly ConditionDomainService _conditionDomainService;

    public ConditionDomainServiceTests()
    {
        _mockLogger = new Mock<IVportalLogger<ConditionDomainService>>();
        _mockEventBus = new Mock<IDistributedEventBus>();
        _mockRepository = new Mock<IConditionRepository>();

        // Setup IResponseCapableEventBus interface BEFORE accessing .Object
        _mockEventBus.As<Common.EventBus.Module.IResponseCapableEventBus>();

        _conditionDomainService = new ConditionDomainService(
            _mockLogger.Object,
            _mockEventBus.Object,
            _mockRepository.Object);

        // Set LazyServiceProvider for GuidGenerator
        var lazyServiceProviderProp = typeof(Volo.Abp.Domain.Services.DomainService)
            .GetProperty("LazyServiceProvider", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (lazyServiceProviderProp != null && lazyServiceProviderProp.CanWrite)
        {
            var lazyServiceProvider = GetRequiredService<Volo.Abp.DependencyInjection.IAbpLazyServiceProvider>();
            lazyServiceProviderProp.SetValue(_conditionDomainService, lazyServiceProvider);
        }
    }

    [Fact]
    public async Task AddCondition_Should_Succeed_When_Valid()
    {
        // Arrange
        var conditionId = Guid.NewGuid();
        var createEntity = new ConditionEntity(conditionId)
        {
            RefId = Guid.NewGuid(),
            ConditionType = LifecycleConditionType.And,
            ConditionTargetType = LifecycleConditionTargetType.State,
            ConditionResultType = LifecycleConditionResultType.SkipOnSuccess,
            Rules = new List<RuleEntity>()
        };

        _mockRepository.Setup(r => r.InsertAsync(
                It.IsAny<ConditionEntity>(),
                It.IsAny<bool>(),
                It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync((ConditionEntity e, bool _, System.Threading.CancellationToken _) => e);

        // Act
        var result = await _conditionDomainService.AddCondition(createEntity);

        // Assert
        result.ShouldBeTrue();
        _mockRepository.Verify(r => r.InsertAsync(
            It.Is<ConditionEntity>(e => e.RefId == createEntity.RefId &&
                                       e.ConditionType == createEntity.ConditionType &&
                                       e.ConditionTargetType == createEntity.ConditionTargetType &&
                                       e.ConditionResultType == createEntity.ConditionResultType &&
                                       e.IsEnabled == true),
            It.IsAny<bool>(),
            It.IsAny<System.Threading.CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateCondition_Should_Succeed_When_Valid()
    {
        // Arrange
        var existingConditionId = Guid.NewGuid();
        var refId = Guid.NewGuid();

        var existingCondition = new ConditionEntity(existingConditionId)
        {
            RefId = refId,
            ConditionType = LifecycleConditionType.Or,
            ConditionTargetType = LifecycleConditionTargetType.Actor,
            ConditionResultType = LifecycleConditionResultType.ActivateOnSuccess
        };

        // UpdateEntity should have the same ID as existing condition (method uses createEntity.Id to get existing)
        var updateEntity = new ConditionEntity(existingConditionId)
        {
            RefId = refId,
            ConditionType = LifecycleConditionType.And,
            ConditionTargetType = LifecycleConditionTargetType.State,
            ConditionResultType = LifecycleConditionResultType.SkipOnSuccess,
            Rules = new List<RuleEntity>
            {
                new RuleEntity(Guid.NewGuid())
                {
                    Function = "TestFunction",
                    FunctionType = DocumentTemplateElementMapFunctionType.XQuery
                }
            }
        };

        _mockRepository.Setup(r => r.GetAsync(existingConditionId, It.IsAny<bool>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(existingCondition);

        _mockRepository.Setup(r => r.InsertAsync(
                It.IsAny<ConditionEntity>(),
                It.IsAny<bool>(),
                It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync((ConditionEntity e, bool _, System.Threading.CancellationToken _) => e);

        // Act
        var result = await _conditionDomainService.UpdateCondition(updateEntity);

        // Assert
        result.ShouldBeTrue();
        _mockRepository.Verify(r => r.GetAsync(existingConditionId, It.IsAny<bool>(), It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.InsertAsync(
            It.Is<ConditionEntity>(e => e.RefId == updateEntity.RefId &&
                                       e.ConditionType == updateEntity.ConditionType &&
                                       e.IsEnabled == true),
            It.IsAny<bool>(),
            It.IsAny<System.Threading.CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddRule_Should_Succeed_When_Condition_Exists()
    {
        // Arrange
        var conditionId = Guid.NewGuid();
        var ruleId = Guid.NewGuid();
        var condition = new ConditionEntity(conditionId)
        {
            RefId = Guid.NewGuid(),
            ConditionType = LifecycleConditionType.And,
            ConditionTargetType = LifecycleConditionTargetType.State,
            ConditionResultType = LifecycleConditionResultType.SkipOnSuccess,
            Rules = new List<RuleEntity>()
        };

        var newRule = new RuleEntity(ruleId)
        {
            Function = "TestFunction",
            FunctionType = DocumentTemplateElementMapFunctionType.XQuery
        };

        _mockRepository.Setup(r => r.GetAsync(conditionId, It.IsAny<bool>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(condition);

        _mockRepository.Setup(r => r.UpdateAsync(
                It.IsAny<ConditionEntity>(),
                It.IsAny<bool>(),
                It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync((ConditionEntity e, bool _, System.Threading.CancellationToken _) => e);

        // Act
        var result = await _conditionDomainService.AddRule(conditionId, newRule);

        // Assert
        result.ShouldBeTrue();
        condition.Rules.ShouldContain(r => r.Function == newRule.Function && r.IsEnabled == true);
        _mockRepository.Verify(r => r.UpdateAsync(condition, It.IsAny<bool>(), It.IsAny<System.Threading.CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateRule_Should_Succeed_When_Rule_Exists()
    {
        // Arrange
        var conditionId = Guid.NewGuid();
        var ruleId = Guid.NewGuid();
        var existingRule = new RuleEntity(ruleId)
        {
            Function = "OldFunction",
            FunctionType = DocumentTemplateElementMapFunctionType.XQuery,
            IsEnabled = true
        };

        var condition = new ConditionEntity(conditionId)
        {
            RefId = Guid.NewGuid(),
            ConditionType = LifecycleConditionType.And,
            ConditionTargetType = LifecycleConditionTargetType.State,
            ConditionResultType = LifecycleConditionResultType.SkipOnSuccess,
            Rules = new List<RuleEntity> { existingRule }
        };

        var updateRule = new RuleEntity(ruleId)
        {
            Function = "NewFunction",
            FunctionType = DocumentTemplateElementMapFunctionType.XQuery
        };

        _mockRepository.Setup(r => r.GetAsync(conditionId, It.IsAny<bool>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(condition);

        _mockRepository.Setup(r => r.UpdateAsync(
                It.IsAny<ConditionEntity>(),
                It.IsAny<bool>(),
                It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync((ConditionEntity e, bool _, System.Threading.CancellationToken _) => e);

        // Act
        var result = await _conditionDomainService.UpdateRule(conditionId, updateRule);

        // Assert
        result.ShouldBeTrue();
        existingRule.Function.ShouldBe("NewFunction");
        existingRule.IsEnabled.ShouldBeTrue();
        _mockRepository.Verify(r => r.UpdateAsync(condition, It.IsAny<bool>(), It.IsAny<System.Threading.CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoveCondition_Should_Succeed_When_Condition_Exists()
    {
        // Arrange
        var conditionId = Guid.NewGuid();

        _mockRepository.Setup(r => r.DeleteAsync(
                conditionId,
                It.IsAny<bool>(),
                It.IsAny<System.Threading.CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _conditionDomainService.RemoveCondition(conditionId);

        // Assert
        result.ShouldBeTrue();
        _mockRepository.Verify(r => r.DeleteAsync(conditionId, It.IsAny<bool>(), It.IsAny<System.Threading.CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoveRule_Should_Succeed_When_Rule_Exists()
    {
        // Arrange
        var conditionId = Guid.NewGuid();
        var ruleId = Guid.NewGuid();
        var ruleToRemove = new RuleEntity(ruleId)
        {
            Function = "TestFunction",
            FunctionType = DocumentTemplateElementMapFunctionType.XQuery
        };

        var condition = new ConditionEntity(conditionId)
        {
            RefId = Guid.NewGuid(),
            ConditionType = LifecycleConditionType.And,
            ConditionTargetType = LifecycleConditionTargetType.State,
            ConditionResultType = LifecycleConditionResultType.SkipOnSuccess,
            Rules = new List<RuleEntity> { ruleToRemove }
        };

        _mockRepository.Setup(r => r.GetAsync(conditionId, It.IsAny<bool>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(condition);

        _mockRepository.Setup(r => r.UpdateAsync(
                It.IsAny<ConditionEntity>(),
                It.IsAny<bool>(),
                It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync((ConditionEntity e, bool _, System.Threading.CancellationToken _) => e);

        // Act
        var result = await _conditionDomainService.RemoveRule(conditionId, ruleId);

        // Assert
        result.ShouldBeTrue();
        condition.Rules.ShouldNotContain(r => r.Id == ruleId);
        _mockRepository.Verify(r => r.UpdateAsync(condition, It.IsAny<bool>(), It.IsAny<System.Threading.CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoveRule_Should_Return_False_When_Rule_Not_Found()
    {
        // Arrange
        var conditionId = Guid.NewGuid();
        var ruleId = Guid.NewGuid();
        var condition = new ConditionEntity(conditionId)
        {
            RefId = Guid.NewGuid(),
            ConditionType = LifecycleConditionType.And,
            ConditionTargetType = LifecycleConditionTargetType.State,
            ConditionResultType = LifecycleConditionResultType.SkipOnSuccess,
            Rules = new List<RuleEntity>()
        };

        _mockRepository.Setup(r => r.GetAsync(conditionId, It.IsAny<bool>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(condition);

        // Act
        var result = await _conditionDomainService.RemoveRule(conditionId, ruleId);

        // Assert
        result.ShouldBeFalse();
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<ConditionEntity>(), It.IsAny<bool>(), It.IsAny<System.Threading.CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetCondition_Should_Return_Condition_When_Exists()
    {
        // Arrange
        var refId = Guid.NewGuid();
        var conditionId = Guid.NewGuid();
        var condition = new ConditionEntity(conditionId)
        {
            RefId = refId,
            ConditionType = LifecycleConditionType.And,
            ConditionTargetType = LifecycleConditionTargetType.State,
            ConditionResultType = LifecycleConditionResultType.SkipOnSuccess
        };

        _mockRepository.Setup(r => r.GetCondition(
                LifecycleConditionTargetType.State,
                refId))
            .ReturnsAsync(condition);

        // Act
        var result = await _conditionDomainService.GetCondition(LifecycleConditionTargetType.State, refId);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(conditionId);
        result.RefId.ShouldBe(refId);
        _mockRepository.Verify(r => r.GetCondition(LifecycleConditionTargetType.State, refId), Times.Once);
    }

    [Fact]
    public async Task GetCondition_Should_Return_Null_When_Not_Exists()
    {
        // Arrange
        var refId = Guid.NewGuid();

        _mockRepository.Setup(r => r.GetCondition(
                LifecycleConditionTargetType.State,
                refId))
            .ReturnsAsync((ConditionEntity)null);

        // Act
        var result = await _conditionDomainService.GetCondition(LifecycleConditionTargetType.State, refId);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task CheckRule_Should_Publish_Event_And_Return_False()
    {
        // Arrange
        var rule = new RuleEntity(Guid.NewGuid())
        {
            Function = "TestFunction",
            FunctionType = DocumentTemplateElementMapFunctionType.XQuery
        };
        var sourceXml = "<root><value>test</value></root>";

        _mockEventBus.Setup(e => e.PublishAsync(
                It.IsAny<LifecycleRuleCheckMsg>(),
                It.IsAny<bool>(),
                It.IsAny<bool>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _conditionDomainService.CheckRule(rule, sourceXml);

        // Assert
        result.ShouldBeFalse(); // Currently always returns false (TODO in implementation)
        _mockEventBus.Verify(e => e.PublishAsync(
            It.Is<LifecycleRuleCheckMsg>(m => m.Function == rule.Function &&
                                             m.FunctionType == rule.FunctionType &&
                                             m.SourceXml == sourceXml),
            It.IsAny<bool>(),
            It.IsAny<bool>()), Times.Once);
    }

    [Fact]
    public async Task ReplyCheckRule_Should_Complete_Without_Error()
    {
        // Arrange
        var id = "test-id";
        var ruleResult = true;

        // Act & Assert - Should not throw
        await _conditionDomainService.ReplyCheckRule(id, ruleResult);
    }

    [Fact]
    public async Task CheckShouldSkip_Should_Return_False_When_Condition_Not_Found()
    {
        // Arrange
        var documentObjectType = "TestDocument";
        var documentId = "doc-123";
        var refId = Guid.NewGuid();

        _mockRepository.Setup(r => r.GetCondition(
                LifecycleConditionTargetType.State,
                refId))
            .ReturnsAsync((ConditionEntity)null);

        // Act
        var result = await _conditionDomainService.CheckShouldSkip(
            documentObjectType,
            documentId,
            LifecycleConditionTargetType.State,
            refId);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public async Task CheckShouldSkip_Should_Return_False_When_No_Document_Version()
    {
        // Arrange
        var documentObjectType = "TestDocument";
        var documentId = "doc-123";
        var refId = Guid.NewGuid();
        var conditionId = Guid.NewGuid();

        var condition = new ConditionEntity(conditionId)
        {
            RefId = refId,
            ConditionType = LifecycleConditionType.And,
            ConditionTargetType = LifecycleConditionTargetType.State,
            ConditionResultType = LifecycleConditionResultType.SkipOnSuccess,
            Rules = new List<RuleEntity>()
        };

        _mockRepository.Setup(r => r.GetCondition(
                LifecycleConditionTargetType.State,
                refId))
            .ReturnsAsync(condition);

        // Setup RequestAsync via IResponseCapableEventBus interface
        _mockEventBus.As<Common.EventBus.Module.IResponseCapableEventBus>()
            .Setup(e => e.RequestAsync<AuditCurrentVersionGotMsg>(
                It.IsAny<object>(),
                It.IsAny<int>()))
            .ReturnsAsync(new AuditCurrentVersionGotMsg { CurrentVersion = null });

        // Act
        var result = await _conditionDomainService.CheckShouldSkip(
            documentObjectType,
            documentId,
            LifecycleConditionTargetType.State,
            refId);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public async Task CheckShouldSkip_Should_Handle_And_Condition_Type_With_Multiple_Rules()
    {
        // Arrange
        var documentObjectType = "TestDocument";
        var documentId = "doc-123";
        var refId = Guid.NewGuid();
        var conditionId = Guid.NewGuid();

        var condition = new ConditionEntity(conditionId)
        {
            RefId = refId,
            ConditionType = LifecycleConditionType.And,
            ConditionTargetType = LifecycleConditionTargetType.State,
            ConditionResultType = LifecycleConditionResultType.SkipOnSuccess,
            Rules = new List<RuleEntity>
            {
                new RuleEntity(Guid.NewGuid()) { Function = "Rule1", FunctionType = DocumentTemplateElementMapFunctionType.XQuery },
                new RuleEntity(Guid.NewGuid()) { Function = "Rule2", FunctionType = DocumentTemplateElementMapFunctionType.XQuery }
            }
        };

        _mockRepository.Setup(r => r.GetCondition(
                LifecycleConditionTargetType.State,
                refId))
            .ReturnsAsync(condition);

        var version = new DocumentVersionEntity { Version = "1" };
        _mockEventBus.As<Common.EventBus.Module.IResponseCapableEventBus>()
            .Setup(e => e.RequestAsync<AuditCurrentVersionGotMsg>(
                It.IsAny<object>(),
                It.IsAny<int>()))
            .ReturnsAsync(new AuditCurrentVersionGotMsg { CurrentVersion = version });

        var document = new AuditedDocumentEto { Data = "<root><value>test</value></root>" };
        _mockEventBus.As<Common.EventBus.Module.IResponseCapableEventBus>()
            .Setup(e => e.RequestAsync<AuditDocumentGotMsg>(
                It.IsAny<object>(),
                It.IsAny<int>()))
            .ReturnsAsync(new AuditDocumentGotMsg { AuditedDocument = document });

        _mockEventBus.Setup(e => e.PublishAsync(
                It.IsAny<LifecycleRuleCheckMsg>(),
                It.IsAny<bool>(),
                It.IsAny<bool>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _conditionDomainService.CheckShouldSkip(
            documentObjectType,
            documentId,
            LifecycleConditionTargetType.State,
            refId);

        // Assert
        // For And type, all rules must pass - since CheckRule always returns false, result should be false
        result.ShouldBeFalse();
        // Note: For And type, if first rule returns false, the method exits early (line 80-82)
        // So only the first rule is checked, not both
        _mockEventBus.Verify(e => e.PublishAsync(
            It.IsAny<LifecycleRuleCheckMsg>(),
            It.IsAny<bool>(),
            It.IsAny<bool>()), Times.Once); // Only first rule is checked due to early exit
    }

    [Fact]
    public async Task CheckShouldSkip_Should_Handle_Or_Condition_Type()
    {
        // Arrange
        var documentObjectType = "TestDocument";
        var documentId = "doc-123";
        var refId = Guid.NewGuid();
        var conditionId = Guid.NewGuid();

        var condition = new ConditionEntity(conditionId)
        {
            RefId = refId,
            ConditionType = LifecycleConditionType.Or,
            ConditionTargetType = LifecycleConditionTargetType.Actor,
            ConditionResultType = LifecycleConditionResultType.SkipOnSuccess,
            Rules = new List<RuleEntity>
            {
                new RuleEntity(Guid.NewGuid()) { Function = "Rule1", FunctionType = DocumentTemplateElementMapFunctionType.XQuery }
            }
        };

        _mockRepository.Setup(r => r.GetCondition(
                LifecycleConditionTargetType.Actor,
                refId))
            .ReturnsAsync(condition);

        var version = new DocumentVersionEntity { Version = "1" };
        _mockEventBus.As<Common.EventBus.Module.IResponseCapableEventBus>()
            .Setup(e => e.RequestAsync<AuditCurrentVersionGotMsg>(
                It.IsAny<object>(),
                It.IsAny<int>()))
            .ReturnsAsync(new AuditCurrentVersionGotMsg { CurrentVersion = version });

        var document = new AuditedDocumentEto { Data = "<root><value>test</value></root>" };
        _mockEventBus.As<Common.EventBus.Module.IResponseCapableEventBus>()
            .Setup(e => e.RequestAsync<AuditDocumentGotMsg>(
                It.IsAny<object>(),
                It.IsAny<int>()))
            .ReturnsAsync(new AuditDocumentGotMsg { AuditedDocument = document });

        _mockEventBus.Setup(e => e.PublishAsync(
                It.IsAny<LifecycleRuleCheckMsg>(),
                It.IsAny<bool>(),
                It.IsAny<bool>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _conditionDomainService.CheckShouldSkip(
            documentObjectType,
            documentId,
            LifecycleConditionTargetType.Actor,
            refId);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public async Task AddCondition_Should_Set_IsEnabled_To_True()
    {
        // Arrange
        var condition = new ConditionEntity(Guid.NewGuid())
        {
            RefId = Guid.NewGuid(),
            ConditionType = LifecycleConditionType.And,
            ConditionTargetType = LifecycleConditionTargetType.State,
            ConditionResultType = LifecycleConditionResultType.SkipOnSuccess,
            IsEnabled = false, // Explicitly set to false
            Rules = new List<RuleEntity>()
        };

        ConditionEntity insertedEntity = null;
        _mockRepository.Setup(r => r.InsertAsync(
                It.IsAny<ConditionEntity>(),
                It.IsAny<bool>(),
                It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync((ConditionEntity e, bool _, System.Threading.CancellationToken _) =>
            {
                insertedEntity = e;
                return e;
            });

        // Act
        var result = await _conditionDomainService.AddCondition(condition);

        // Assert
        result.ShouldBeTrue();
        // Verify the inserted entity has IsEnabled = true (method creates new entity with IsEnabled = true)
        insertedEntity.ShouldNotBeNull();
        insertedEntity.IsEnabled.ShouldBeTrue();
    }

    [Fact]
    public async Task UpdateCondition_Should_Handle_Exception_Gracefully()
    {
        // Arrange
        var conditionId = Guid.NewGuid();
        var updateEntity = new ConditionEntity(conditionId)
        {
            RefId = Guid.NewGuid(),
            ConditionType = LifecycleConditionType.And,
            ConditionTargetType = LifecycleConditionTargetType.State,
            ConditionResultType = LifecycleConditionResultType.SkipOnSuccess
        };

        _mockRepository.Setup(r => r.GetAsync(conditionId, It.IsAny<bool>(), It.IsAny<System.Threading.CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _conditionDomainService.UpdateCondition(updateEntity);

        // Assert
        result.ShouldBeFalse();
        // Logger.Capture is called with CallerMemberName parameter
        _mockLogger.Verify(l => l.Capture(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task AddRule_Should_Return_False_When_Condition_Not_Found()
    {
        // Arrange
        var conditionId = Guid.NewGuid();
        var newRule = new RuleEntity(Guid.NewGuid())
        {
            Function = "TestFunction",
            FunctionType = DocumentTemplateElementMapFunctionType.XQuery
        };

        _mockRepository.Setup(r => r.GetAsync(conditionId, It.IsAny<bool>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync((ConditionEntity)null);

        // Act
        var result = await _conditionDomainService.AddRule(conditionId, newRule);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public async Task UpdateRule_Should_Return_False_When_Rule_Not_Found()
    {
        // Arrange
        var conditionId = Guid.NewGuid();
        var ruleId = Guid.NewGuid();
        var condition = new ConditionEntity(conditionId)
        {
            RefId = Guid.NewGuid(),
            ConditionType = LifecycleConditionType.And,
            ConditionTargetType = LifecycleConditionTargetType.State,
            ConditionResultType = LifecycleConditionResultType.SkipOnSuccess,
            Rules = new List<RuleEntity>() // Empty rules
        };

        var updateRule = new RuleEntity(ruleId)
        {
            Function = "NewFunction",
            FunctionType = DocumentTemplateElementMapFunctionType.XQuery
        };

        _mockRepository.Setup(r => r.GetAsync(conditionId, It.IsAny<bool>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(condition);

        // Act
        var result = await _conditionDomainService.UpdateRule(conditionId, updateRule);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public async Task CheckShouldSkip_Should_Handle_ActivateOnSuccess_Result_Type()
    {
        // Arrange
        var documentObjectType = "TestDocument";
        var documentId = "doc-123";
        var refId = Guid.NewGuid();
        var conditionId = Guid.NewGuid();

        var condition = new ConditionEntity(conditionId)
        {
            RefId = refId,
            ConditionType = LifecycleConditionType.And,
            ConditionTargetType = LifecycleConditionTargetType.State,
            ConditionResultType = LifecycleConditionResultType.ActivateOnSuccess, // Different result type
            Rules = new List<RuleEntity>()
        };

        _mockRepository.Setup(r => r.GetCondition(
                LifecycleConditionTargetType.State,
                refId))
            .ReturnsAsync(condition);

        var version = new DocumentVersionEntity { Version = "1" };
        _mockEventBus.As<Common.EventBus.Module.IResponseCapableEventBus>()
            .Setup(e => e.RequestAsync<AuditCurrentVersionGotMsg>(
                It.IsAny<object>(),
                It.IsAny<int>()))
            .ReturnsAsync(new AuditCurrentVersionGotMsg { CurrentVersion = version });

        var document = new AuditedDocumentEto { Data = "<root><value>test</value></root>" };
        _mockEventBus.As<Common.EventBus.Module.IResponseCapableEventBus>()
            .Setup(e => e.RequestAsync<AuditDocumentGotMsg>(
                It.IsAny<object>(),
                It.IsAny<int>()))
            .ReturnsAsync(new AuditDocumentGotMsg { AuditedDocument = document });

        _mockEventBus.Setup(e => e.PublishAsync(
                It.IsAny<LifecycleRuleCheckMsg>(),
                It.IsAny<bool>(),
                It.IsAny<bool>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _conditionDomainService.CheckShouldSkip(
            documentObjectType,
            documentId,
            LifecycleConditionTargetType.State,
            refId);

        // Assert
        // ActivateOnSuccess means "activate on success" = don't skip (return false)
        // But with no rules, CheckRule always returns false, so result should be false
        // However, if the condition has no rules, it might return true (activate = don't skip)
        // Based on implementation: ActivateOnSuccess with no rules returns true (activate = don't skip)
        result.ShouldBeTrue(); // ActivateOnSuccess means activate (don't skip) = true
    }
}

