using Common.Module.Constants;
using Logging.Module;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VPortal.Lifecycle.Feature.Module.DomainServices;
using VPortal.Lifecycle.Feature.Module.Entities;
using VPortal.Lifecycle.Feature.Module.Repositories.Templates;
using Xunit;

namespace VPortal.Lifecycle.Feature.Module.StateTemplate;

public class StateTemplateDomainServiceTests : ModuleDomainTestBase<ModuleDomainTestModule>
{
    private readonly Mock<IVportalLogger<StateTemplateDomainService>> _mockLogger;
    private readonly Mock<IStatesGroupTemplatesRepository> _mockRepository;
    private readonly StateTemplateDomainService _service;

    private readonly Guid _testGroupId = Guid.NewGuid();

    public StateTemplateDomainServiceTests()
    {
        _mockLogger = new Mock<IVportalLogger<StateTemplateDomainService>>();
        _mockRepository = new Mock<IStatesGroupTemplatesRepository>();

        _service = new StateTemplateDomainService(
            _mockLogger.Object,
            _mockRepository.Object);
    }

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_Should_Return_All_States_For_Group()
    {
        // Arrange
        var group = new StatesGroupTemplateEntity(_testGroupId)
        {
            States = new List<StateTemplateEntity>
            {
                new StateTemplateEntity(Guid.NewGuid()) { StateName = "State1", OrderIndex = 1 },
                new StateTemplateEntity(Guid.NewGuid()) { StateName = "State2", OrderIndex = 2 }
            }
        };

        _mockRepository.Setup(r => r.GetAsync(_testGroupId))
            .ReturnsAsync(group);

        // Act
        var result = await _service.GetAllAsync(_testGroupId);

        // Assert
        result.Count.ShouldBe(2);
        result[0].StateName.ShouldBe("State1");
        result[1].StateName.ShouldBe("State2");
    }

    #endregion

    #region UpdateOrderIndex Tests

    [Fact]
    public async Task UpdateOrderIndex_Should_Update_State_Order()
    {
        // Arrange
        var stateId = Guid.NewGuid();
        var newOrderIndex = 5;
        var state = new StateTemplateEntity(stateId)
        {
            OrderIndex = 1
        };

        _mockRepository.Setup(r => r.GetState(stateId))
            .ReturnsAsync(state);

        // Act
        var result = await _service.UpdateOrderIndex(stateId, newOrderIndex);

        // Assert
        result.ShouldBeTrue();
        state.OrderIndex.ShouldBe(newOrderIndex);
    }

    #endregion

    #region Enable Tests

    [Fact]
    public async Task Enable_Should_Update_IsActive_To_True()
    {
        // Arrange
        var stateId = Guid.NewGuid();
        var state = new StateTemplateEntity(stateId)
        {
            IsActive = false
        };

        _mockRepository.Setup(r => r.GetState(stateId))
            .ReturnsAsync(state);

        // Act
        var result = await _service.Enable(stateId, true);

        // Assert
        result.ShouldBeTrue();
        state.IsActive.ShouldBeTrue();
    }

    #endregion

    #region UpdateApprovalType Tests

    [Fact]
    public async Task UpdateApprovalType_Should_Update_Approval_Type()
    {
        // Arrange
        var stateId = Guid.NewGuid();
        var state = new StateTemplateEntity(stateId)
        {
            ApprovalType = LifecycleApprovalType.Regular
        };

        _mockRepository.Setup(r => r.GetState(stateId))
            .ReturnsAsync(state);

        // Act
        var result = await _service.UpdateApprovalType(stateId, LifecycleApprovalType.Parallel);

        // Assert
        result.ShouldBeTrue();
        state.ApprovalType.ShouldBe(LifecycleApprovalType.Parallel);
    }

    [Fact]
    public async Task UpdateApprovalType_Should_Support_All_Types()
    {
        // Arrange
        var stateId = Guid.NewGuid();
        var state = new StateTemplateEntity(stateId);

        _mockRepository.Setup(r => r.GetState(stateId))
            .ReturnsAsync(state);

        // Act & Assert - Regular
        var result1 = await _service.UpdateApprovalType(stateId, LifecycleApprovalType.Regular);
        result1.ShouldBeTrue();
        state.ApprovalType.ShouldBe(LifecycleApprovalType.Regular);

        // Parallel
        var result2 = await _service.UpdateApprovalType(stateId, LifecycleApprovalType.Parallel);
        result2.ShouldBeTrue();
        state.ApprovalType.ShouldBe(LifecycleApprovalType.Parallel);

        // AtLeast
        var result3 = await _service.UpdateApprovalType(stateId, LifecycleApprovalType.AtLeast);
        result3.ShouldBeTrue();
        state.ApprovalType.ShouldBe(LifecycleApprovalType.AtLeast);
    }

    #endregion

    #region UpdateName Tests

    [Fact]
    public async Task UpdateName_Should_Update_State_Name()
    {
        // Arrange
        var stateId = Guid.NewGuid();
        var newName = "UpdatedStateName";
        var state = new StateTemplateEntity(stateId)
        {
            StateName = "OldName"
        };

        _mockRepository.Setup(r => r.GetState(stateId))
            .ReturnsAsync(state);

        // Act
        var result = await _service.UpdateName(stateId, newName);

        // Assert
        result.ShouldBeTrue();
        state.StateName.ShouldBe(newName);
    }

    [Fact]
    public async Task UpdateName_Should_Throw_On_Exception()
    {
        // Arrange
        var stateId = Guid.NewGuid();

        _mockRepository.Setup(r => r.GetState(stateId))
            .ThrowsAsync(new Exception("State not found"));

        // Act & Assert
        await Should.ThrowAsync<Exception>(async () =>
            await _service.UpdateName(stateId, "NewName"));
    }

    #endregion

    #region Add Tests

    [Fact]
    public async Task Add_Should_Add_State_To_Group()
    {
        // Arrange
        var group = new StatesGroupTemplateEntity(_testGroupId)
        {
            States = new List<StateTemplateEntity>()
        };

        var newState = new StateTemplateEntity(Guid.NewGuid())
        {
            StatesGroupTemplateId = _testGroupId,
            StateName = "NewState"
        };

        _mockRepository.Setup(r => r.GetAsync(_testGroupId))
            .ReturnsAsync(group);

        // Act
        var result = await _service.Add(newState);

        // Assert
        result.ShouldBeTrue();
        group.States.Count.ShouldBe(1);
        group.States[0].StateName.ShouldBe("NewState");
    }

    #endregion

    #region UpdateOrderIndexes Tests

    [Fact]
    public async Task UpdateOrderIndexes_Should_Update_Multiple_States()
    {
        // Arrange
        var state1Id = Guid.NewGuid();
        var state2Id = Guid.NewGuid();
        var state1 = new StateTemplateEntity(state1Id) { OrderIndex = 1 };
        var state2 = new StateTemplateEntity(state2Id) { OrderIndex = 2 };

        var order = new Dictionary<Guid, int>
        {
            { state1Id, 3 },
            { state2Id, 1 }
        };

        _mockRepository.Setup(r => r.GetState(state1Id))
            .ReturnsAsync(state1);
        _mockRepository.Setup(r => r.GetState(state2Id))
            .ReturnsAsync(state2);

        // Act
        var result = await _service.UpdateOrderIndexes(order);

        // Assert
        result.ShouldBeTrue();
        state1.OrderIndex.ShouldBe(3);
        state2.OrderIndex.ShouldBe(1);
    }

    [Fact]
    public async Task UpdateOrderIndexes_Should_Handle_Null_States()
    {
        // Arrange
        var order = new Dictionary<Guid, int>
        {
            { Guid.NewGuid(), 1 }
        };

        _mockRepository.Setup(r => r.GetState(It.IsAny<Guid>()))
            .ReturnsAsync((StateTemplateEntity)null);

        // Act
        var result = await _service.UpdateOrderIndexes(order);

        // Assert
        result.ShouldBeTrue(); // Should not throw, just skip null states
    }

    #endregion

    #region Remove Tests

    [Fact]
    public async Task Remove_Should_Remove_State_From_Group()
    {
        // Arrange
        var stateId = Guid.NewGuid();
        var group = new StatesGroupTemplateEntity(_testGroupId)
        {
            States = new List<StateTemplateEntity>
            {
                new StateTemplateEntity(stateId) { StateName = "State1" },
                new StateTemplateEntity(Guid.NewGuid()) { StateName = "State2" }
            }
        };

        _mockRepository.Setup(r => r.GetAsync(_testGroupId))
            .ReturnsAsync(group);

        _mockRepository.Setup(r => r.UpdateAsync(group, true))
            .ReturnsAsync(group);

        // Act
        var result = await _service.Remove(_testGroupId, stateId);

        // Assert
        result.ShouldBeTrue();
        group.States.Count.ShouldBe(1);
        group.States[0].StateName.ShouldBe("State2");
        _mockRepository.Verify(r => r.UpdateAsync(group, true), Times.Once);
    }

    [Fact]
    public async Task Remove_Should_Return_False_When_State_Not_Found()
    {
        // Arrange
        var stateId = Guid.NewGuid();
        var group = new StatesGroupTemplateEntity(_testGroupId)
        {
            States = new List<StateTemplateEntity>()
        };

        _mockRepository.Setup(r => r.GetAsync(_testGroupId))
            .ReturnsAsync(group);

        // Act
        var result = await _service.Remove(_testGroupId, stateId);

        // Assert
        result.ShouldBeFalse();
    }

    #endregion
}

