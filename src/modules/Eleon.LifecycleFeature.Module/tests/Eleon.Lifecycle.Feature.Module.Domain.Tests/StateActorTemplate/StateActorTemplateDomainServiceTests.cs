using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Module.Constants;
using Logging.Module;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Moq;
using Shouldly;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.Threading;
using VPortal.Lifecycle.Feature.Module.DomainServices;
using VPortal.Lifecycle.Feature.Module.Entities;
using VPortal.Lifecycle.Feature.Module.Entities.Templates;
using VPortal.Lifecycle.Feature.Module.Localization;
using VPortal.Lifecycle.Feature.Module.Repositories.Templates;
using Eleon.TestsBase.Lib.TestHelpers;
using Xunit;

namespace VPortal.Lifecycle.Feature.Module.StateActorTemplate;

public class StateActorTemplateDomainServiceTests : ModuleDomainTestBase<ModuleDomainTestModule>
{
  private readonly Mock<IVportalLogger<StateActorTemplateDomainService>> _mockLogger;
  private readonly Mock<IStatesGroupTemplatesRepository> _mockRepository;
  private readonly Mock<IStringLocalizer<LifecycleFeatureModuleResource>> _mockLocalizer;
  private readonly MockIdentityUserManager _userManager;
  private readonly MockIdentityRoleManager _roleManager;
  private readonly StateActorTemplateDomainService _service;

  private readonly Guid _testStateId = Guid.NewGuid();
  private readonly Guid _testUserId = Guid.NewGuid();

  public StateActorTemplateDomainServiceTests()
  {
    _mockLogger = new Mock<IVportalLogger<StateActorTemplateDomainService>>();
    _mockRepository = new Mock<IStatesGroupTemplatesRepository>();
    _mockLocalizer = new Mock<IStringLocalizer<LifecycleFeatureModuleResource>>();
    // Use mocked managers - no database needed
    _userManager = new MockIdentityUserManager();
    _roleManager = new MockIdentityRoleManager();

    _service = new StateActorTemplateDomainService(
      _mockLogger.Object,
      _mockRepository.Object,
      _mockLocalizer.Object,
      _userManager,
      GetRequiredService<IGuidGenerator>(),
      _roleManager
    );

    // Set LazyServiceProvider for GuidGenerator (DomainService base class property)
    var lazyServiceProviderProp = typeof(Volo.Abp.Domain.Services.DomainService).GetProperty(
      "LazyServiceProvider",
      System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance
    );
    if (lazyServiceProviderProp != null && lazyServiceProviderProp.CanWrite)
    {
      var lazyServiceProvider =
        GetRequiredService<Volo.Abp.DependencyInjection.IAbpLazyServiceProvider>();
      lazyServiceProviderProp.SetValue(_service, lazyServiceProvider);
    }
  }

  #region GetAllAsync Tests

  [Fact]
  public async Task GetAllAsync_Should_Set_DisplayName_For_Initiator()
  {
    // Arrange
    var actor = new StateActorTemplateEntity(Guid.NewGuid())
    {
      ActorType = LifecycleActorTypes.Initiator,
    };

    var state = new StateTemplateEntity(_testStateId)
    {
      Actors = new List<StateActorTemplateEntity> { actor },
    };

    _mockLocalizer
      .Setup(l => l["Initiator"])
      .Returns(new Microsoft.Extensions.Localization.LocalizedString("Initiator", "Initiator"));

    _mockRepository.Setup(r => r.GetState(_testStateId)).ReturnsAsync(state);

    // Act
    var result = await _service.GetAllAsync(_testStateId);

    // Assert
    result.Count.ShouldBe(1);
    actor.DisplayName.ShouldBe("Initiator");
  }

  [Fact]
  public async Task GetAllAsync_Should_Set_DisplayName_For_User()
  {
    // Arrange - Create user in database
    var user = new Volo.Abp.Identity.IdentityUser(
      _testUserId,
      "test@test.com",
      "test@test.com",
      null
    )
    {
      Name = "John",
      Surname = "Doe",
    };
    // Add user to mocked manager (no database needed)
    _userManager.AddUser(user);

    var actor = new StateActorTemplateEntity(Guid.NewGuid())
    {
      ActorType = LifecycleActorTypes.User,
      RefId = _testUserId.ToString(),
    };

    var state = new StateTemplateEntity(_testStateId)
    {
      Actors = new List<StateActorTemplateEntity> { actor },
    };

    _mockRepository.Setup(r => r.GetState(_testStateId)).ReturnsAsync(state);

    // Act
    var result = await _service.GetAllAsync(_testStateId);

    // Assert
    result.Count.ShouldBe(1);
    actor.DisplayName.ShouldBe("John Doe");
  }

  [Fact]
  public async Task GetAllAsync_Should_Set_DisplayName_For_Role()
  {
    // Arrange - Create role in database
    var roleName = "ManagerRole";
    var roleId = Guid.NewGuid();
    var role = new Volo.Abp.Identity.IdentityRole(roleId, roleName, null);
    // Add role to mocked manager (no database needed)
    _roleManager.AddRole(role);

    var actor = new StateActorTemplateEntity(Guid.NewGuid())
    {
      ActorType = LifecycleActorTypes.Role,
      RefId = roleName,
    };

    var state = new StateTemplateEntity(_testStateId)
    {
      Actors = new List<StateActorTemplateEntity> { actor },
    };

    _mockRepository.Setup(r => r.GetState(_testStateId)).ReturnsAsync(state);

    // Act
    var result = await _service.GetAllAsync(_testStateId);

    // Assert
    result.Count.ShouldBe(1);
    actor.DisplayName.ShouldBe(roleName);
  }

  [Fact]
  public async Task GetAllAsync_Should_Set_DisplayName_For_Beneficiary()
  {
    // Arrange
    var actor = new StateActorTemplateEntity(Guid.NewGuid())
    {
      ActorType = LifecycleActorTypes.Beneficiary,
    };

    var state = new StateTemplateEntity(_testStateId)
    {
      Actors = new List<StateActorTemplateEntity> { actor },
    };

    _mockLocalizer
      .Setup(l => l["Beneficiary"])
      .Returns(new Microsoft.Extensions.Localization.LocalizedString("Beneficiary", "Beneficiary"));

    _mockRepository.Setup(r => r.GetState(_testStateId)).ReturnsAsync(state);

    // Act
    var result = await _service.GetAllAsync(_testStateId);

    // Assert
    result.Count.ShouldBe(1);
    actor.DisplayName.ShouldBe("Beneficiary");
  }

  [Fact]
  public async Task GetAllAsync_Should_Handle_Multiple_Actor_Types()
  {
    // Arrange - Create user and role in database
    var testUser = new Volo.Abp.Identity.IdentityUser(
      _testUserId,
      "test@test.com",
      "test@test.com",
      null
    )
    {
      Name = "John",
      Surname = "Doe",
    };
    var roleName = "ManagerRole";
    var roleId = Guid.NewGuid();
    var testRole = new Volo.Abp.Identity.IdentityRole(roleId, roleName, null);
    // Add user and role to mocked managers (no database needed)
    _userManager.AddUser(testUser);
    _roleManager.AddRole(testRole);

    var initiator = new StateActorTemplateEntity(Guid.NewGuid())
    {
      ActorType = LifecycleActorTypes.Initiator,
    };
    var user = new StateActorTemplateEntity(Guid.NewGuid())
    {
      ActorType = LifecycleActorTypes.User,
      RefId = _testUserId.ToString(),
    };
    var role = new StateActorTemplateEntity(Guid.NewGuid())
    {
      ActorType = LifecycleActorTypes.Role,
      RefId = roleName,
    };

    var state = new StateTemplateEntity(_testStateId)
    {
      Actors = new List<StateActorTemplateEntity> { initiator, user, role },
    };

    _mockLocalizer
      .Setup(l => l["Initiator"])
      .Returns(new Microsoft.Extensions.Localization.LocalizedString("Initiator", "Initiator"));

    _mockRepository.Setup(r => r.GetState(_testStateId)).ReturnsAsync(state);

    // Act
    var result = await _service.GetAllAsync(_testStateId);

    // Assert
    result.Count.ShouldBe(3);
    initiator.DisplayName.ShouldBe("Initiator");
    user.DisplayName.ShouldBe("John Doe");
    role.DisplayName.ShouldBe(roleName);
  }

  #endregion

  #region Enable Tests

  [Fact]
  public async Task Enable_Should_Update_IsActive()
  {
    // Arrange
    var actorId = Guid.NewGuid();
    var actor = new StateActorTemplateEntity(actorId) { IsActive = false };

    _mockRepository.Setup(r => r.GetStateActor(actorId)).ReturnsAsync(actor);

    // Act
    var result = await _service.Enable(actorId, true);

    // Assert
    result.ShouldBeTrue();
    actor.IsActive.ShouldBeTrue();
  }

  #endregion

  #region Add Tests

  [Fact]
  public async Task Add_Should_Create_New_Actor_With_TaskLists()
  {
    // Arrange
    var state = new StateTemplateEntity(_testStateId)
    {
      Actors = new List<StateActorTemplateEntity>(),
    };

    var newActor = new StateActorTemplateEntity(Guid.NewGuid())
    {
      StateTemplateId = _testStateId,
      ActorName = "NewActor",
      ActorType = LifecycleActorTypes.User,
      RefId = _testUserId.ToString(),
      IsApprovalNeeded = true,
      TaskLists = new List<StateActorTaskListSettingTemplateEntity>
      {
        new StateActorTaskListSettingTemplateEntity(Guid.NewGuid())
        {
          DocumentObjectType = "TaskDoc",
          TaskListId = Guid.NewGuid(),
        },
      },
    };

    _mockRepository.Setup(r => r.GetState(_testStateId)).ReturnsAsync(state);

    // Act
    var result = await _service.Add(newActor);

    // Assert
    result.ShouldBeTrue();
    state.Actors.Count.ShouldBe(1);
    state.Actors[0].ActorName.ShouldBe("NewActor");
    state.Actors[0].TaskLists.Count.ShouldBe(1);
  }

  #endregion

  #region UpdateOrderIndexes Tests

  [Fact]
  public async Task UpdateOrderIndexes_Should_Update_Multiple_Actors()
  {
    // Arrange
    var actor1Id = Guid.NewGuid();
    var actor2Id = Guid.NewGuid();
    var actor1 = new StateActorTemplateEntity(actor1Id) { OrderIndex = 1 };
    var actor2 = new StateActorTemplateEntity(actor2Id) { OrderIndex = 2 };

    var order = new Dictionary<Guid, int> { { actor1Id, 3 }, { actor2Id, 1 } };

    _mockRepository.Setup(r => r.GetStateActor(actor1Id)).ReturnsAsync(actor1);
    _mockRepository.Setup(r => r.GetStateActor(actor2Id)).ReturnsAsync(actor2);

    // Act
    var result = await _service.UpdateOrderIndexes(order);

    // Assert
    result.ShouldBeTrue();
    actor1.OrderIndex.ShouldBe(3);
    actor2.OrderIndex.ShouldBe(1);
  }

  #endregion

  #region Update Tests

  [Fact]
  public async Task Update_Should_Update_All_Actor_Properties()
  {
    // Arrange
    var actorId = Guid.NewGuid();
    var existingActor = new StateActorTemplateEntity(actorId)
    {
      ActorName = "OldName",
      ActorType = LifecycleActorTypes.User,
      IsApprovalNeeded = false,
      TaskLists = new List<StateActorTaskListSettingTemplateEntity>
      {
        new StateActorTaskListSettingTemplateEntity(Guid.NewGuid()),
      },
    };

    var updatedActor = new StateActorTemplateEntity(actorId)
    {
      ActorName = "NewName",
      ActorType = LifecycleActorTypes.Role,
      IsApprovalNeeded = true,
      TaskLists = new List<StateActorTaskListSettingTemplateEntity>
      {
        new StateActorTaskListSettingTemplateEntity(Guid.NewGuid())
        {
          DocumentObjectType = "NewDoc",
          TaskListId = Guid.NewGuid(),
        },
      },
    };

    _mockRepository.Setup(r => r.GetStateActor(actorId)).ReturnsAsync(existingActor);

    // Act
    var result = await _service.Update(updatedActor);

    // Assert
    result.ShouldBeTrue();
    existingActor.ActorName.ShouldBe("NewName");
    existingActor.ActorType.ShouldBe(LifecycleActorTypes.Role);
    existingActor.IsApprovalNeeded.ShouldBeTrue();
    existingActor.TaskLists.Count.ShouldBe(1);
    existingActor.TaskLists[0].DocumentObjectType.ShouldBe("NewDoc");
  }

  [Fact]
  public async Task Update_Should_Clear_Old_TaskLists()
  {
    // Arrange
    var actorId = Guid.NewGuid();
    var existingActor = new StateActorTemplateEntity(actorId)
    {
      TaskLists = new List<StateActorTaskListSettingTemplateEntity>
      {
        new StateActorTaskListSettingTemplateEntity(Guid.NewGuid()),
        new StateActorTaskListSettingTemplateEntity(Guid.NewGuid()),
      },
    };

    var updatedActor = new StateActorTemplateEntity(actorId)
    {
      TaskLists = new List<StateActorTaskListSettingTemplateEntity>(),
    };

    _mockRepository.Setup(r => r.GetStateActor(actorId)).ReturnsAsync(existingActor);

    // Act
    var result = await _service.Update(updatedActor);

    // Assert
    result.ShouldBeTrue();
    existingActor.TaskLists.Count.ShouldBe(0);
  }

  #endregion

  #region Remove Tests

  [Fact]
  public async Task Remove_Should_Remove_Actor_From_State()
  {
    // Arrange
    var actorId = Guid.NewGuid();
    var actor = new StateActorTemplateEntity(actorId);
    var state = new StateTemplateEntity(_testStateId)
    {
      Actors = new List<StateActorTemplateEntity>
      {
        actor,
        new StateActorTemplateEntity(Guid.NewGuid()),
      },
    };

    _mockRepository.Setup(r => r.GetStateActor(actorId)).ReturnsAsync(actor);
    actor.StateTemplateEntity = state;

    // Act
    var result = await _service.Remove(actorId);

    // Assert
    result.ShouldBeTrue();
    state.Actors.Count.ShouldBe(1);
    state.Actors.ShouldNotContain(actor);
  }

  #endregion
}
