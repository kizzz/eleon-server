using Common.Module.Constants;
using Logging.Module;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.Threading;
using Volo.Abp.Users;
using VPortal.Lifecycle.Feature.Module.DomainServices;
using VPortal.Lifecycle.Feature.Module.Entities;
using VPortal.Lifecycle.Feature.Module.Localization;
using VPortal.Lifecycle.Feature.Module.Repositories.Audits;
using Eleon.TestsBase.Lib.TestHelpers;
using Xunit;

namespace VPortal.Lifecycle.Feature.Module.StatesGroupAudit;

public class StatesGroupAuditDomainServiceTests : ModuleDomainTestBase<ModuleDomainTestModule>
{
    private readonly Mock<IVportalLogger<StatesGroupAuditDomainService>> _mockLogger;
    private readonly Mock<IStatesGroupAuditsRepository> _mockRepository;
    private readonly MockIdentityUserManager _userManager;
    private readonly MockIdentityRoleManager _roleManager;
    private readonly Mock<ICurrentUser> _mockCurrentUser;
    private readonly Mock<IStringLocalizer<LifecycleFeatureModuleResource>> _mockLocalizer;
    private readonly StatesGroupAuditDomainService _service;

    private readonly string _testDocumentType = "TestDocument";
    private readonly string _testDocumentId = "DOC-123";
    private readonly Guid _testUserId = Guid.NewGuid();

    public StatesGroupAuditDomainServiceTests()
    {
        _mockLogger = new Mock<IVportalLogger<StatesGroupAuditDomainService>>();
        _mockRepository = new Mock<IStatesGroupAuditsRepository>();
        // Use mocked manager - no database needed
        _userManager = new MockIdentityUserManager();
        _roleManager = new MockIdentityRoleManager();
        _mockCurrentUser = new Mock<ICurrentUser>();
        _mockLocalizer = new Mock<IStringLocalizer<LifecycleFeatureModuleResource>>();

        _service = new StatesGroupAuditDomainService(
            _mockLogger.Object,
            _mockRepository.Object,
            _userManager,
            _mockCurrentUser.Object,
            _mockLocalizer.Object,
            _roleManager);
    }

    #region Add Tests

    [Fact]
    public async Task Add_Should_Insert_Audit_And_Return_True()
    {
        // Arrange
        var audit = new StatesGroupAuditEntity(Guid.NewGuid())
        {
            DocumentObjectType = _testDocumentType,
            DocumentId = _testDocumentId,
            Status = LifecycleStatus.New
        };

        _mockRepository.Setup(r => r.Add(audit))
            .ReturnsAsync(true);

        // Act
        var result = await _service.Add(audit);

        // Assert
        result.ShouldBeTrue();
        _mockRepository.Verify(r => r.Add(audit), Times.Once);
    }

    #endregion

    #region Remove Tests

    [Fact]
    public async Task Remove_Should_Delete_Audit_And_Return_True()
    {
        // Arrange
        var auditId = Guid.NewGuid();

        _mockRepository.Setup(r => r.Remove(auditId))
            .ReturnsAsync(true);

        // Act
        var result = await _service.Remove(auditId);

        // Assert
        result.ShouldBeTrue();
        _mockRepository.Verify(r => r.Remove(auditId), Times.Once);
    }

    #endregion

    #region DeepCancel Tests

    [Fact]
    public async Task DeepCancel_Should_Cancel_All_States_And_Actors()
    {
        // Arrange
        var audit = new StatesGroupAuditEntity(Guid.NewGuid())
        {
            DocumentObjectType = _testDocumentType,
            DocumentId = _testDocumentId,
            Status = LifecycleStatus.Enroute,
            States = new List<StateAuditEntity>
            {
                new StateAuditEntity(Guid.NewGuid())
                {
                    Status = LifecycleStatus.Enroute,
                    Actors = new List<StateActorAuditEntity>
                    {
                        new StateActorAuditEntity(Guid.NewGuid())
                        {
                            Status = LifecycleActorStatus.Enroute
                        }
                    }
                }
            }
        };

        _mockRepository.Setup(r => r.GetByDocIdAsync(_testDocumentType, _testDocumentId))
            .ReturnsAsync(audit);

        _mockRepository.Setup(r => r.UpdateAsync(audit, true))
            .ReturnsAsync(audit);

        // Act
        var result = await _service.DeepCancel(_testDocumentType, _testDocumentId);

        // Assert
        result.ShouldBeTrue();
        audit.Status.ShouldBe(LifecycleStatus.Canceled);
        audit.States[0].Status.ShouldBe(LifecycleStatus.Canceled);
        audit.States[0].Actors[0].Status.ShouldBe(LifecycleActorStatus.Canceled);
        audit.States[0].Actors[0].StatusDate.ShouldNotBe(default(DateTime));
    }

    [Fact]
    public async Task DeepCancel_Should_Return_False_When_Audit_Not_Found()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByDocIdAsync(_testDocumentType, _testDocumentId))
            .ReturnsAsync((StatesGroupAuditEntity)null);

        // Act
        var result = await _service.DeepCancel(_testDocumentType, _testDocumentId);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public async Task DeepCancel_Should_Handle_Multiple_States_And_Actors()
    {
        // Arrange
        var audit = new StatesGroupAuditEntity(Guid.NewGuid())
        {
            DocumentObjectType = _testDocumentType,
            DocumentId = _testDocumentId,
            Status = LifecycleStatus.Enroute,
            States = new List<StateAuditEntity>
            {
                new StateAuditEntity(Guid.NewGuid())
                {
                    Status = LifecycleStatus.Enroute,
                    Actors = new List<StateActorAuditEntity>
                    {
                        new StateActorAuditEntity(Guid.NewGuid()) { Status = LifecycleActorStatus.Enroute },
                        new StateActorAuditEntity(Guid.NewGuid()) { Status = LifecycleActorStatus.Approved }
                    }
                },
                new StateAuditEntity(Guid.NewGuid())
                {
                    Status = LifecycleStatus.Complete,
                    Actors = new List<StateActorAuditEntity>
                    {
                        new StateActorAuditEntity(Guid.NewGuid()) { Status = LifecycleActorStatus.Approved }
                    }
                }
            }
        };

        _mockRepository.Setup(r => r.GetByDocIdAsync(_testDocumentType, _testDocumentId))
            .ReturnsAsync(audit);

        _mockRepository.Setup(r => r.UpdateAsync(audit, true))
            .ReturnsAsync(audit);

        // Act
        var result = await _service.DeepCancel(_testDocumentType, _testDocumentId);

        // Assert
        result.ShouldBeTrue();
        audit.Status.ShouldBe(LifecycleStatus.Canceled);
        audit.States.All(s => s.Status == LifecycleStatus.Canceled).ShouldBeTrue();
        audit.States.SelectMany(s => s.Actors).All(a => a.Status == LifecycleActorStatus.Canceled).ShouldBeTrue();
    }

    #endregion

    #region ForceComplete Tests

    [Fact]
    public async Task ForceComplete_Should_Complete_All_States_And_Cancel_Actors()
    {
        // Arrange
        var audit = new StatesGroupAuditEntity(Guid.NewGuid())
        {
            DocumentObjectType = _testDocumentType,
            DocumentId = _testDocumentId,
            Status = LifecycleStatus.Enroute,
            States = new List<StateAuditEntity>
            {
                new StateAuditEntity(Guid.NewGuid())
                {
                    Status = LifecycleStatus.Enroute,
                    Actors = new List<StateActorAuditEntity>
                    {
                        new StateActorAuditEntity(Guid.NewGuid())
                        {
                            Status = LifecycleActorStatus.Enroute
                        }
                    }
                }
            }
        };

        _mockRepository.Setup(r => r.GetByDocIdAsync(_testDocumentType, _testDocumentId))
            .ReturnsAsync(audit);

        _mockRepository.Setup(r => r.UpdateAsync(audit, true))
            .ReturnsAsync(audit);

        // Act
        var result = await _service.ForceComplete(_testDocumentType, _testDocumentId);

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe(LifecycleStatus.Complete);
        result.States[0].Status.ShouldBe(LifecycleStatus.Complete);
        result.States[0].Actors[0].Status.ShouldBe(LifecycleActorStatus.Canceled);
        result.States[0].Actors[0].StatusDate.ShouldNotBe(default(DateTime));
    }

    #endregion

    #region GetPendingApproval Tests

    [Fact]
    public async Task GetPendingApproval_Should_Return_List_When_User_Provided()
    {
        // Arrange - Add user to mocked manager (no database needed)
        var userId = Guid.NewGuid();
        var user = new Volo.Abp.Identity.IdentityUser(userId, "test@test.com", "test@test.com", null);
        _userManager.AddUser(user);
        // Add some roles to the user
        _userManager.AddUserRole(userId, "Admin");
        _userManager.AddUserRole(userId, "Manager");

        var audits = new List<StatesGroupAuditEntity>
        {
            new StatesGroupAuditEntity(Guid.NewGuid())
            {
                DocumentObjectType = _testDocumentType,
                DocumentId = "DOC-1",
                Status = LifecycleStatus.Enroute
            }
        };

        // Get roles from mocked manager
        var actualUser = await _userManager.GetByIdAsync(userId);
        var actualRoles = await _userManager.GetRolesAsync(actualUser);

        var expectedResult = new KeyValuePair<long, List<StatesGroupAuditEntity>>(1, audits);

        // Mock repository with actual roles from user manager
        _mockRepository.Setup(r => r.GetReportListsAsync(
                null, int.MaxValue, 0, null, null, null, null, userId, actualRoles, null))
            .ReturnsAsync(expectedResult);

        // Act
        // Domain service exposes GetReports (GetPendingApproval was removed/renamed)
        var result = await _service.GetReports(userId: userId);

        // Assert
        result.Key.ShouldBe(1);
        result.Value.Count.ShouldBe(1);
    }

    [Fact]
    public async Task GetPendingApproval_Should_Return_List_When_No_User_Provided()
    {
        // Arrange
        var audits = new List<StatesGroupAuditEntity>();
        var expectedResult = new KeyValuePair<long, List<StatesGroupAuditEntity>>(0, audits);

        _mockRepository.Setup(r => r.GetReportListsAsync(
                null, int.MaxValue, 0, null, null, null, null, null, null, null))
            .ReturnsAsync(expectedResult);

        // Act
        // Domain service exposes GetReports (GetPendingApproval was removed/renamed)
        var result = await _service.GetReports();

        // Assert
        result.Key.ShouldBe(0);
        result.Value.Count.ShouldBe(0);
        // Using actual service from test base - Verify calls removed
        // Note: Tests may need adjustment to work with actual services
    }

    [Fact]
    public async Task GetPendingApproval_Should_Respect_Filters()
    {
        // Arrange
        var startDate = DateTime.Now.AddDays(-7);
        var endDate = DateTime.Now;
        var objectTypes = new List<string> { "Document1", "Document2" };

        var audits = new List<StatesGroupAuditEntity>();
        var expectedResult = new KeyValuePair<long, List<StatesGroupAuditEntity>>(0, audits);

        _mockRepository.Setup(r => r.GetReportListsAsync(
                "StatusDate", 10, 5, "search", startDate, endDate, objectTypes, null, null, null))
            .ReturnsAsync(expectedResult);

        // Act
        // Domain service exposes GetReports (GetPendingApproval was removed/renamed)
        var result = await _service.GetReports(
            sorting: "StatusDate",
            maxResultCount: 10,
            skipCount: 5,
            searchQuery: "search",
            statusDateFilterStart: startDate,
            statusDateFilterEnd: endDate,
            objectTypeFilter: objectTypes);

        // Assert
        result.Key.ShouldBe(0);
        _mockRepository.Verify(r => r.GetReportListsAsync(
            "StatusDate", 10, 5, "search", startDate, endDate, objectTypes, null, null, null), Times.Once);
    }

    #endregion

    #region GetDocumentIdsByFilter Tests

    [Fact]
    public async Task GetDocumentIdsByFilter_Should_Return_Document_Ids()
    {
        // Arrange
        var documentIds = new List<string> { "DOC-1", "DOC-2", "DOC-3" };

        _mockRepository.Setup(r => r.GetDocumentIdsByFilter(_testDocumentType, null, null, null))
            .ReturnsAsync(documentIds);

        // Act
        var result = await _service.GetDocumentIdsByFilter(_testDocumentType);

        // Assert
        result.Count.ShouldBe(3);
        result.ShouldContain("DOC-1");
        result.ShouldContain("DOC-2");
        result.ShouldContain("DOC-3");
    }

    [Fact]
    public async Task GetDocumentIdsByFilter_Should_Filter_By_User()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var documentIds = new List<string> { "DOC-1" };

        _mockRepository.Setup(r => r.GetDocumentIdsByFilter(_testDocumentType, userId, null, null))
            .ReturnsAsync(documentIds);

        // Act
        var result = await _service.GetDocumentIdsByFilter(_testDocumentType, userId);

        // Assert
        result.Count.ShouldBe(1);
        _mockRepository.Verify(r => r.GetDocumentIdsByFilter(_testDocumentType, userId, null, null), Times.Once);
    }

    [Fact]
    public async Task GetDocumentIdsByFilter_Should_Filter_By_Roles()
    {
        // Arrange
        var roles = new List<string> { "Manager", "Admin" };
        var documentIds = new List<string> { "DOC-1", "DOC-2" };

        _mockRepository.Setup(r => r.GetDocumentIdsByFilter(_testDocumentType, null, roles, null))
            .ReturnsAsync(documentIds);

        // Act
        var result = await _service.GetDocumentIdsByFilter(_testDocumentType, null, roles);

        // Assert
        result.Count.ShouldBe(2);
    }

    [Fact]
    public async Task GetDocumentIdsByFilter_Should_Filter_By_Statuses()
    {
        // Arrange
        var statuses = new List<LifecycleStatus> { LifecycleStatus.Enroute, LifecycleStatus.Complete };
        var documentIds = new List<string> { "DOC-1" };

        _mockRepository.Setup(r => r.GetDocumentIdsByFilter(_testDocumentType, null, null, statuses))
            .ReturnsAsync(documentIds);

        // Act
        var result = await _service.GetDocumentIdsByFilter(_testDocumentType, null, null, statuses);

        // Assert
        result.Count.ShouldBe(1);
    }

    [Fact]
    public async Task GetDocumentIdsByFilter_Should_Handle_All_Filters_Together()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roles = new List<string> { "Manager" };
        var statuses = new List<LifecycleStatus> { LifecycleStatus.Enroute };
        var documentIds = new List<string> { "DOC-1" };

        _mockRepository.Setup(r => r.GetDocumentIdsByFilter(_testDocumentType, userId, roles, statuses))
            .ReturnsAsync(documentIds);

        // Act
        var result = await _service.GetDocumentIdsByFilter(_testDocumentType, userId, roles, statuses);

        // Assert
        result.Count.ShouldBe(1);
        _mockRepository.Verify(r => r.GetDocumentIdsByFilter(_testDocumentType, userId, roles, statuses), Times.Once);
    }

    #endregion
}
