using Common.Module.Constants;
using Shouldly;
using System;
using System.Threading.Tasks;
using Volo.Abp.Guids;
using VPortal.Lifecycle.Feature.Module.Entities.Conditions;
using VPortal.Lifecycle.Feature.Module.Repositories.Conditions;
using Xunit;

namespace VPortal.Lifecycle.Feature.Module.EntityFrameworkCore.Repositories;

public class ConditionRepositoryTests : ModuleEntityFrameworkCoreTestBase
{
    private readonly IConditionRepository _conditionRepository;
    private readonly IGuidGenerator _guidGenerator;

    public ConditionRepositoryTests()
    {
        _conditionRepository = GetRequiredService<IConditionRepository>();
        _guidGenerator = GetRequiredService<IGuidGenerator>();
    }

    [Fact]
    public async Task InsertAsync_Should_Succeed()
    {
        // Arrange
        var condition = new ConditionEntity(_guidGenerator.Create())
        {
            RefId = Guid.NewGuid(),
            ConditionType = LifecycleConditionType.And,
            ConditionTargetType = LifecycleConditionTargetType.State,
            ConditionResultType = LifecycleConditionResultType.SkipOnSuccess,
            IsEnabled = true,
            Rules = new System.Collections.Generic.List<RuleEntity>()
        };

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
            await _conditionRepository.InsertAsync(condition));

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(condition.Id);
    }

    [Fact]
    public async Task GetCondition_Should_Return_Condition_When_Exists()
    {
        // Arrange
        var refId = Guid.NewGuid();
        var condition = new ConditionEntity(_guidGenerator.Create())
        {
            RefId = refId,
            ConditionType = LifecycleConditionType.And,
            ConditionTargetType = LifecycleConditionTargetType.State,
            ConditionResultType = LifecycleConditionResultType.SkipOnSuccess,
            IsEnabled = true,
            Rules = new System.Collections.Generic.List<RuleEntity>()
        };

        await WithUnitOfWorkAsync(async () =>
            await _conditionRepository.InsertAsync(condition));

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
            await _conditionRepository.GetCondition(LifecycleConditionTargetType.State, refId));

        // Assert
        result.ShouldNotBeNull();
        result.RefId.ShouldBe(refId);
        result.ConditionTargetType.ShouldBe(LifecycleConditionTargetType.State);
    }

    [Fact]
    public async Task GetCondition_Should_Return_Null_When_Not_Exists()
    {
        // Arrange
        var refId = Guid.NewGuid();

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
            await _conditionRepository.GetCondition(LifecycleConditionTargetType.State, refId));

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task DeleteAsync_Should_Remove_Condition()
    {
        // Arrange
        var condition = new ConditionEntity(_guidGenerator.Create())
        {
            RefId = Guid.NewGuid(),
            ConditionType = LifecycleConditionType.And,
            ConditionTargetType = LifecycleConditionTargetType.State,
            ConditionResultType = LifecycleConditionResultType.SkipOnSuccess,
            IsEnabled = true,
            Rules = new System.Collections.Generic.List<RuleEntity>()
        };

        await WithUnitOfWorkAsync(async () =>
            await _conditionRepository.InsertAsync(condition));

        // Act
        await WithUnitOfWorkAsync(async () =>
            await _conditionRepository.DeleteAsync(condition));

        // Assert
        var result = await WithUnitOfWorkAsync(async () =>
            await _conditionRepository.GetCondition(LifecycleConditionTargetType.State, condition.RefId));
        result.ShouldBeNull();
    }

    [Fact]
    public async Task GetCondition_Should_Return_Null_For_Different_Target_Type()
    {
        // Arrange
        var refId = Guid.NewGuid();
        var condition = new ConditionEntity(_guidGenerator.Create())
        {
            RefId = refId,
            ConditionType = LifecycleConditionType.And,
            ConditionTargetType = LifecycleConditionTargetType.State,
            ConditionResultType = LifecycleConditionResultType.SkipOnSuccess,
            IsEnabled = true,
            Rules = new System.Collections.Generic.List<RuleEntity>()
        };

        await WithUnitOfWorkAsync(async () =>
            await _conditionRepository.InsertAsync(condition));

        // Act - Query for Actor type, but condition is State type
        var result = await WithUnitOfWorkAsync(async () =>
            await _conditionRepository.GetCondition(LifecycleConditionTargetType.Actor, refId));

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task GetCondition_Should_Return_Null_For_Different_RefId()
    {
        // Arrange
        var refId1 = Guid.NewGuid();
        var refId2 = Guid.NewGuid();
        var condition = new ConditionEntity(_guidGenerator.Create())
        {
            RefId = refId1,
            ConditionType = LifecycleConditionType.And,
            ConditionTargetType = LifecycleConditionTargetType.State,
            ConditionResultType = LifecycleConditionResultType.SkipOnSuccess,
            IsEnabled = true,
            Rules = new System.Collections.Generic.List<RuleEntity>()
        };

        await WithUnitOfWorkAsync(async () =>
            await _conditionRepository.InsertAsync(condition));

        // Act - Query for different RefId
        var result = await WithUnitOfWorkAsync(async () =>
            await _conditionRepository.GetCondition(LifecycleConditionTargetType.State, refId2));

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task InsertAsync_Should_Persist_Rules()
    {
        // Arrange
        var rule1Id = _guidGenerator.Create();
        var rule2Id = _guidGenerator.Create();
        var condition = new ConditionEntity(_guidGenerator.Create())
        {
            RefId = Guid.NewGuid(),
            ConditionType = LifecycleConditionType.And,
            ConditionTargetType = LifecycleConditionTargetType.State,
            ConditionResultType = LifecycleConditionResultType.SkipOnSuccess,
            IsEnabled = true,
            Rules = new System.Collections.Generic.List<RuleEntity>
            {
                new RuleEntity(rule1Id)
                {
                    Function = "Rule1",
                    FunctionType = DocumentTemplateElementMapFunctionType.XQuery,
                    IsEnabled = true
                },
                new RuleEntity(rule2Id)
                {
                    Function = "Rule2",
                    FunctionType = DocumentTemplateElementMapFunctionType.XQuery,
                    IsEnabled = true
                }
            }
        };

        // Act
        var inserted = await WithUnitOfWorkAsync(async () =>
            await _conditionRepository.InsertAsync(condition));

        // Assert
        inserted.ShouldNotBeNull();
        inserted.Rules.Count.ShouldBe(2);
        inserted.Rules.Any(r => r.Function == "Rule1").ShouldBeTrue();
        inserted.Rules.Any(r => r.Function == "Rule2").ShouldBeTrue();
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Condition_Properties()
    {
        // Arrange
        var condition = new ConditionEntity(_guidGenerator.Create())
        {
            RefId = Guid.NewGuid(),
            ConditionType = LifecycleConditionType.And,
            ConditionTargetType = LifecycleConditionTargetType.State,
            ConditionResultType = LifecycleConditionResultType.SkipOnSuccess,
            IsEnabled = true,
            Rules = new System.Collections.Generic.List<RuleEntity>()
        };

        await WithUnitOfWorkAsync(async () =>
            await _conditionRepository.InsertAsync(condition));

        // Modify condition
        condition.ConditionType = LifecycleConditionType.Or;
        condition.IsEnabled = false;

        // Act
        var updated = await WithUnitOfWorkAsync(async () =>
            await _conditionRepository.UpdateAsync(condition));

        // Assert
        updated.ShouldNotBeNull();
        updated.ConditionType.ShouldBe(LifecycleConditionType.Or);
        updated.IsEnabled.ShouldBeFalse();
    }

    [Fact]
    public async Task GetCondition_Should_Handle_Multiple_Conditions_For_Same_Target_Type()
    {
        // Arrange
        var refId1 = Guid.NewGuid();
        var refId2 = Guid.NewGuid();

        var condition1 = new ConditionEntity(_guidGenerator.Create())
        {
            RefId = refId1,
            ConditionType = LifecycleConditionType.And,
            ConditionTargetType = LifecycleConditionTargetType.State,
            ConditionResultType = LifecycleConditionResultType.SkipOnSuccess,
            IsEnabled = true,
            Rules = new System.Collections.Generic.List<RuleEntity>()
        };

        var condition2 = new ConditionEntity(_guidGenerator.Create())
        {
            RefId = refId2,
            ConditionType = LifecycleConditionType.Or,
            ConditionTargetType = LifecycleConditionTargetType.State,
            ConditionResultType = LifecycleConditionResultType.ActivateOnSuccess,
            IsEnabled = true,
            Rules = new System.Collections.Generic.List<RuleEntity>()
        };

        await WithUnitOfWorkAsync(async () =>
        {
            await _conditionRepository.InsertAsync(condition1);
            await _conditionRepository.InsertAsync(condition2);
        });

        // Act
        var result1 = await WithUnitOfWorkAsync(async () =>
            await _conditionRepository.GetCondition(LifecycleConditionTargetType.State, refId1));
        var result2 = await WithUnitOfWorkAsync(async () =>
            await _conditionRepository.GetCondition(LifecycleConditionTargetType.State, refId2));

        // Assert
        result1.ShouldNotBeNull();
        result1.RefId.ShouldBe(refId1);
        result1.ConditionType.ShouldBe(LifecycleConditionType.And);

        result2.ShouldNotBeNull();
        result2.RefId.ShouldBe(refId2);
        result2.ConditionType.ShouldBe(LifecycleConditionType.Or);
    }
}

