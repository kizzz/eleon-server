using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Module.Constants;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages;
using Eleon.TestsBase.Lib.TestHelpers;
using Logging.Module;
using Messaging.Module.Messages;
using Microsoft.Extensions.Localization;
using Moq;
using Shouldly;
using Volo.Abp.EventBus.Distributed;
using VPortal.Lifecycle.Feature.Module.DomainServices;
using VPortal.Lifecycle.Feature.Module.Entities;
using VPortal.Lifecycle.Feature.Module.Entities.Templates;
using VPortal.Lifecycle.Feature.Module.Localization;
using VPortal.Lifecycle.Feature.Module.Repositories.Templates;
using Xunit;

namespace VPortal.Lifecycle.Feature.Module.StatesGroupTemplate;

public class StatesGroupTemplateDomainServiceTests : ModuleDomainTestBase<ModuleDomainTestModule>
{
  private readonly Mock<IVportalLogger<StatesGroupTemplateDomainService>> _mockLogger;
  private readonly Mock<IStatesGroupTemplatesRepository> _mockRepository;
  private readonly Mock<IDistributedEventBus> _mockEventBus;
  private readonly MockIdentityUserManager _mockIdentityUserManager;
  private readonly MockIdentityRoleManager _mockIdentityRoleManager;
  private readonly Mock<IStringLocalizer<LifecycleFeatureModuleResource>> _mockLocalizer;
  private readonly StatesGroupTemplateDomainService _service;

  private readonly string _testDocumentType = "TestDocument";
  private readonly Guid _testTenantId = Guid.NewGuid();

  public StatesGroupTemplateDomainServiceTests()
  {
    _mockLogger = new Mock<IVportalLogger<StatesGroupTemplateDomainService>>();
    _mockRepository = new Mock<IStatesGroupTemplatesRepository>();
    _mockEventBus = new Mock<IDistributedEventBus>();
    _mockIdentityUserManager = new MockIdentityUserManager();
    _mockIdentityRoleManager = new MockIdentityRoleManager();
    _mockLocalizer = new Mock<IStringLocalizer<LifecycleFeatureModuleResource>>();

    _service = new StatesGroupTemplateDomainService(
      _mockLogger.Object,
      _mockRepository.Object,
      _mockEventBus.Object,
      _mockIdentityUserManager,
      _mockLocalizer.Object,
      _mockIdentityRoleManager
    );

    // Set CurrentTenant for DomainService base class
    // Create a mock and ensure it returns the test tenant ID
    var mockCurrentTenant = new Mock<Volo.Abp.MultiTenancy.ICurrentTenant>();
    mockCurrentTenant.Setup(t => t.Id).Returns(_testTenantId);
    mockCurrentTenant.Setup(t => t.Name).Returns("TestTenant");
    mockCurrentTenant.Setup(t => t.IsAvailable).Returns(true);

    // Set CurrentTenant property via reflection
    var currentTenantProp = typeof(Volo.Abp.Domain.Services.DomainService).GetProperty(
      "CurrentTenant",
      System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance
    );
    if (currentTenantProp != null)
    {
      if (currentTenantProp.CanWrite)
      {
        currentTenantProp.SetValue(_service, mockCurrentTenant.Object);
      }
      else
      {
        // Property is read-only, try to set via backing field
        var field =
          typeof(Volo.Abp.Domain.Services.DomainService).GetField(
            "_currentTenant",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
          )
          ?? typeof(Volo.Abp.Domain.Services.DomainService).GetField(
            "currentTenant",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
          );
        if (field != null)
        {
          field.SetValue(_service, mockCurrentTenant.Object);
        }
      }
    }

    // Also set up LazyServiceProvider to provide ICurrentTenant if accessed via lazy loading
    var lazyServiceProvider =
      GetRequiredService<Volo.Abp.DependencyInjection.IAbpLazyServiceProvider>();
    var lazyServiceProviderProp = typeof(Volo.Abp.Domain.Services.DomainService).GetProperty(
      "LazyServiceProvider",
      System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance
    );
    if (lazyServiceProviderProp != null && lazyServiceProviderProp.CanWrite)
    {
      lazyServiceProviderProp.SetValue(_service, lazyServiceProvider);
    }
  }

  #region GetAsync Tests

  [Fact]
  public async Task GetAsync_Should_Return_Template_When_Found()
  {
    // Arrange
    var templateId = Guid.NewGuid();
    var template = new StatesGroupTemplateEntity(templateId)
    {
      DocumentObjectType = _testDocumentType,
      GroupName = "TestTemplate",
      IsActive = true,
    };

    _mockRepository.Setup(r => r.GetAsync(templateId)).ReturnsAsync(template);

    // Act
    var result = await _service.GetAsync(templateId);

    // Assert
    result.ShouldNotBeNull();
    result.Id.ShouldBe(templateId);
    result.GroupName.ShouldBe("TestTemplate");
    _mockRepository.Verify(r => r.GetAsync(templateId), Times.Once);
  }

  [Fact]
  public async Task GetAsync_Should_Return_Null_When_Not_Found()
  {
    // Arrange
    var templateId = Guid.NewGuid();

    _mockRepository
      .Setup(r => r.GetAsync(templateId))
      .ReturnsAsync((StatesGroupTemplateEntity)null);

    // Act
    var result = await _service.GetAsync(templateId);

    // Assert
    result.ShouldBeNull();
  }

  [Fact]
  public async Task GetAsync_Should_Handle_Exceptions_Gracefully()
  {
    // Arrange
    var templateId = Guid.NewGuid();

    _mockRepository.Setup(r => r.GetAsync(templateId)).ThrowsAsync(new Exception("Database error"));

    // Act
    var result = await _service.GetAsync(templateId);

    // Assert
    result.ShouldBeNull();
    // Capture has [CallerMemberName] parameter, so it will be called with the actual method name, not the test method name
    _mockLogger.Verify(l => l.Capture(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
  }

  #endregion

  #region GetListAsync Tests

  [Fact]
  public async Task GetListAsync_Should_Return_Paginated_List()
  {
    // Arrange
    var templates = new List<StatesGroupTemplateEntity>
    {
      new StatesGroupTemplateEntity(Guid.NewGuid())
      {
        DocumentObjectType = _testDocumentType,
        GroupName = "Template1",
      },
      new StatesGroupTemplateEntity(Guid.NewGuid())
      {
        DocumentObjectType = _testDocumentType,
        GroupName = "Template2",
      },
    };

    var expectedResult = new KeyValuePair<long, List<StatesGroupTemplateEntity>>(2, templates);

    _mockRepository
      .Setup(r => r.GetPaginatedListAsync(_testDocumentType, null, int.MaxValue, 0))
      .ReturnsAsync(expectedResult);

    // Act
    var result = await _service.GetListAsync(_testDocumentType);

    // Assert
    result.Key.ShouldBe(2);
    result.Value.Count.ShouldBe(2);
    _mockRepository.Verify(
      r => r.GetPaginatedListAsync(_testDocumentType, null, int.MaxValue, 0),
      Times.Once
    );
  }

  [Fact]
  public async Task GetListAsync_Should_Respect_Pagination_Parameters()
  {
    // Arrange
    var templates = new List<StatesGroupTemplateEntity>
    {
      new StatesGroupTemplateEntity(Guid.NewGuid())
      {
        DocumentObjectType = _testDocumentType,
        GroupName = "Template1",
      },
    };

    var expectedResult = new KeyValuePair<long, List<StatesGroupTemplateEntity>>(10, templates);

    _mockRepository
      .Setup(r => r.GetPaginatedListAsync(_testDocumentType, "GroupName", 5, 10))
      .ReturnsAsync(expectedResult);

    // Act
    var result = await _service.GetListAsync(_testDocumentType, "GroupName", 5, 10);

    // Assert
    result.Key.ShouldBe(10);
    result.Value.Count.ShouldBe(1);
    _mockRepository.Verify(
      r => r.GetPaginatedListAsync(_testDocumentType, "GroupName", 5, 10),
      Times.Once
    );
  }

  #endregion

  #region Add Tests

  [Fact]
  public async Task Add_Should_Insert_Template_And_Return_True()
  {
    // Arrange
    var template = new StatesGroupTemplateEntity(Guid.NewGuid())
    {
      DocumentObjectType = _testDocumentType,
      GroupName = "NewTemplate",
      IsActive = true,
    };

    _mockRepository
      .Setup(r => r.GetDocumentTypeGroupsCountAsync(_testDocumentType))
      .ReturnsAsync(5);

    _mockRepository.Setup(r => r.Add(template)).ReturnsAsync(true);

    // Act
    var result = await _service.Add(template);

    // Assert
    result.ShouldBeTrue();
    _mockRepository.Verify(r => r.Add(template), Times.Once);
  }

  [Fact]
  public async Task Add_Should_Handle_Exceptions_Gracefully()
  {
    // Arrange
    var template = new StatesGroupTemplateEntity(Guid.NewGuid())
    {
      DocumentObjectType = _testDocumentType,
      GroupName = "NewTemplate",
    };

    _mockRepository
      .Setup(r => r.GetDocumentTypeGroupsCountAsync(_testDocumentType))
      .ThrowsAsync(new Exception("Database error"));

    // Act
    var result = await _service.Add(template);

    // Assert
    result.ShouldBeFalse();
    // Note: Logger.Capture is called in catch block with CallerMemberName parameter
    _mockLogger.Verify(l => l.Capture(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
  }

  #endregion

  #region Remove Tests

  [Fact]
  public async Task Remove_Should_Delete_Template_And_Publish_Event()
  {
    // Arrange
    var templateId = Guid.NewGuid();

    // Ensure CurrentTenant is set correctly before the test
    var currentTenantProp = typeof(Volo.Abp.Domain.Services.DomainService).GetProperty(
      "CurrentTenant",
      System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance
    );
    if (currentTenantProp != null)
    {
      var currentTenantValue =
        currentTenantProp.GetValue(_service) as Volo.Abp.MultiTenancy.ICurrentTenant;
      if (currentTenantValue == null || currentTenantValue.Id != _testTenantId)
      {
        // CurrentTenant is not set correctly, set it now
        var mockCurrentTenant = new Mock<Volo.Abp.MultiTenancy.ICurrentTenant>();
        mockCurrentTenant.Setup(t => t.Id).Returns(_testTenantId);
        mockCurrentTenant.Setup(t => t.Name).Returns("TestTenant");
        mockCurrentTenant.Setup(t => t.IsAvailable).Returns(true);

        if (currentTenantProp.CanWrite)
        {
          currentTenantProp.SetValue(_service, mockCurrentTenant.Object);
        }
        else
        {
          // Property is read-only, try to set via backing field
          var field =
            typeof(Volo.Abp.Domain.Services.DomainService).GetField(
              "_currentTenant",
              System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
            )
            ?? typeof(Volo.Abp.Domain.Services.DomainService).GetField(
              "currentTenant",
              System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
            );
          if (field != null)
          {
            field.SetValue(_service, mockCurrentTenant.Object);
          }
        }

        // Verify the property was set
        var verifyValue =
          currentTenantProp.GetValue(_service) as Volo.Abp.MultiTenancy.ICurrentTenant;
        if (verifyValue == null || verifyValue.Id != _testTenantId)
        {
          // Property still not set - CurrentTenant might be accessed via LazyServiceProvider
          // This is expected to fail, but we'll continue to see what exception occurs
        }
      }
    }

    _mockRepository.Setup(r => r.DeleteAsync(templateId)).Returns(Task.CompletedTask);

    // Allow TenantId to be null in the message (CurrentTenant?.Id might be null)
    _mockEventBus
      .Setup(e =>
        e.PublishAsync(
          It.Is<LifecycleRemovedMsg>(m => m.LifecylceId == templateId),
          It.IsAny<bool>(),
          It.IsAny<bool>()
        )
      )
      .Returns(Task.CompletedTask);

    // Act
    var result = await _service.Remove(templateId);

    // Assert
    result.ShouldBeTrue();

    // If result is false, check what exception was logged
    if (!result)
    {
      _mockLogger.Verify(l => l.Capture(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
    }

    _mockRepository.Verify(r => r.DeleteAsync(templateId), Times.Once);
    _mockEventBus.Verify(
      e =>
        e.PublishAsync(
          It.Is<LifecycleRemovedMsg>(m => m.LifecylceId == templateId),
          It.IsAny<bool>(),
          It.IsAny<bool>()
        ),
      Times.Once
    );
  }

  [Fact]
  public async Task Remove_Should_Return_False_On_Exception()
  {
    // Arrange
    var templateId = Guid.NewGuid();

    _mockRepository
      .Setup(r => r.DeleteAsync(templateId))
      .ThrowsAsync(new Exception("Delete failed"));

    // Mock event bus to avoid exception when PublishAsync is called
    _mockEventBus
      .Setup(e => e.PublishAsync(It.IsAny<object>(), It.IsAny<bool>(), It.IsAny<bool>()))
      .Returns(Task.CompletedTask);

    // Act
    var result = await _service.Remove(templateId);

    // Assert
    result.ShouldBeFalse();
    // Note: Logger.Capture is called in catch block with CallerMemberName parameter
    _mockLogger.Verify(l => l.Capture(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
  }

  #endregion

  #region Enable Tests

  [Fact]
  public async Task Enable_Should_Update_IsActive_To_True()
  {
    // Arrange
    var templateId = Guid.NewGuid();
    var template = new StatesGroupTemplateEntity(templateId) { IsActive = false };

    _mockRepository.Setup(r => r.GetAsync(templateId)).ReturnsAsync(template);

    _mockRepository.Setup(r => r.UpdateAsync(template)).ReturnsAsync(template);

    // Act
    var result = await _service.Enable(templateId, true);

    // Assert
    result.ShouldBeTrue();
    template.IsActive.ShouldBeTrue();
    _mockRepository.Verify(r => r.UpdateAsync(template), Times.Once);
  }

  [Fact]
  public async Task Enable_Should_Update_IsActive_To_False()
  {
    // Arrange
    var templateId = Guid.NewGuid();
    var template = new StatesGroupTemplateEntity(templateId) { IsActive = true };

    _mockRepository.Setup(r => r.GetAsync(templateId)).ReturnsAsync(template);

    _mockRepository.Setup(r => r.UpdateAsync(template)).ReturnsAsync(template);

    // Act
    var result = await _service.Enable(templateId, false);

    // Assert
    result.ShouldBeTrue();
    template.IsActive.ShouldBeFalse();
  }

  #endregion

  #region Rename Tests

  [Fact]
  public async Task Rename_Should_Update_GroupName()
  {
    // Arrange
    var templateId = Guid.NewGuid();
    var newName = "RenamedTemplate";
    var template = new StatesGroupTemplateEntity(templateId) { GroupName = "OldName" };

    _mockRepository.Setup(r => r.GetAsync(templateId)).ReturnsAsync(template);

    _mockRepository.Setup(r => r.UpdateAsync(template)).ReturnsAsync(template);

    // Act
    var result = await _service.Rename(templateId, newName);

    // Assert
    result.ShouldBeTrue();
    template.GroupName.ShouldBe(newName);
    _mockRepository.Verify(r => r.UpdateAsync(template), Times.Once);
  }

  [Fact]
  public async Task Rename_Should_Handle_Empty_Name()
  {
    // Arrange
    var templateId = Guid.NewGuid();
    var template = new StatesGroupTemplateEntity(templateId) { GroupName = "OldName" };

    _mockRepository.Setup(r => r.GetAsync(templateId)).ReturnsAsync(template);

    _mockRepository.Setup(r => r.UpdateAsync(template)).ReturnsAsync(template);

    // Act
    var result = await _service.Rename(templateId, string.Empty);

    // Assert
    result.ShouldBeTrue();
    template.GroupName.ShouldBe(string.Empty);
  }

  #endregion

  #region Update Tests

  [Fact]
  public async Task Update_Should_Update_GroupName_And_IsActive()
  {
    // Arrange
    var templateId = Guid.NewGuid();
    var existingTemplate = new StatesGroupTemplateEntity(templateId)
    {
      GroupName = "OldName",
      IsActive = false,
    };

    var updatedTemplate = new StatesGroupTemplateEntity(templateId)
    {
      GroupName = "NewName",
      IsActive = true,
    };

    _mockRepository.Setup(r => r.GetAsync(templateId)).ReturnsAsync(existingTemplate);

    _mockRepository.Setup(r => r.UpdateAsync(existingTemplate)).ReturnsAsync(existingTemplate);

    // Act
    var result = await _service.Update(updatedTemplate);

    // Assert
    result.ShouldBeTrue();
    existingTemplate.GroupName.ShouldBe("NewName");
    existingTemplate.IsActive.ShouldBeTrue();
    _mockRepository.Verify(r => r.UpdateAsync(existingTemplate), Times.Once);
  }

  [Fact]
  public async Task Update_Should_Not_Update_States()
  {
    // Arrange
    var templateId = Guid.NewGuid();
    var existingTemplate = new StatesGroupTemplateEntity(templateId)
    {
      GroupName = "OldName",
      States = new List<StateTemplateEntity>
      {
        new StateTemplateEntity(Guid.NewGuid()) { StateName = "State1" },
      },
    };

    var updatedTemplate = new StatesGroupTemplateEntity(templateId)
    {
      GroupName = "NewName",
      States = new List<StateTemplateEntity>
      {
        new StateTemplateEntity(Guid.NewGuid()) { StateName = "State2" },
      },
    };

    _mockRepository.Setup(r => r.GetAsync(templateId)).ReturnsAsync(existingTemplate);

    _mockRepository.Setup(r => r.UpdateAsync(existingTemplate)).ReturnsAsync(existingTemplate);

    // Act
    var result = await _service.Update(updatedTemplate);

    // Assert
    result.ShouldBeTrue();
    // States should not be updated by this method
    existingTemplate.States.Count.ShouldBe(1);
    existingTemplate.States[0].StateName.ShouldBe("State1");
  }

  #endregion
}
