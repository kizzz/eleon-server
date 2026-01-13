using Common.Module.Constants;
using Eleon.Templating.Module.EntityFrameworkCore.Templates;
using Eleon.Templating.Module.Templates;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Eleon.Templating.Module.EntityFrameworkCore.Templates;

public class EfCoreTemplateRepositoryTests : ModuleEntityFrameworkCoreTestBase
{
    private readonly ITemplateRepository _templateRepository;

    public EfCoreTemplateRepositoryTests()
    {
        _templateRepository = GetRequiredService<ITemplateRepository>();
    }

    [Fact]
    public async Task GetByTypeAndIsSystemAsync_Should_Return_Filtered_Templates()
    {
        // Arrange - Create test templates
        var actionSystemTemplate = new Template(Guid.NewGuid())
        {
            Name = "ActionSystem",
            Type = TemplateType.Action,
            Format = TextFormat.Plaintext,
            TemplateContent = "System Action",
            RequiredPlaceholders = string.Empty,
            IsSystem = true,
            TemplateId = "action-system"
        };

        var actionUserTemplate = new Template(Guid.NewGuid())
        {
            Name = "ActionUser",
            Type = TemplateType.Action,
            Format = TextFormat.Plaintext,
            TemplateContent = "User Action",
            RequiredPlaceholders = string.Empty,
            IsSystem = false,
            TemplateId = "action-user"
        };

        var notificationSystemTemplate = new Template(Guid.NewGuid())
        {
            Name = "NotificationSystem",
            Type = TemplateType.Notification,
            Format = TextFormat.Scriban,
            TemplateContent = "System Notification",
            RequiredPlaceholders = string.Empty,
            IsSystem = true,
            TemplateId = "notification-system"
        };

        // Act
        await WithUnitOfWorkAsync(async () =>
        {
            await _templateRepository.InsertAsync(actionSystemTemplate);
            await _templateRepository.InsertAsync(actionUserTemplate);
            await _templateRepository.InsertAsync(notificationSystemTemplate);
        });

        List<Template>? systemActionTemplates = null;
        List<Template>? userActionTemplates = null;

        await WithUnitOfWorkAsync(async () =>
        {
            systemActionTemplates = await _templateRepository.GetByTypeAndIsSystemAsync(
                TemplateType.Action, true);
            userActionTemplates = await _templateRepository.GetByTypeAndIsSystemAsync(
                TemplateType.Action, false);
        });

        // Assert
        systemActionTemplates.ShouldNotBeNull();
        systemActionTemplates.ShouldContain(t => t.Name == "ActionSystem");
        systemActionTemplates.ShouldNotContain(t => t.Name == "ActionUser");

        userActionTemplates.ShouldNotBeNull();
        userActionTemplates.ShouldContain(t => t.Name == "ActionUser");
        userActionTemplates.ShouldNotContain(t => t.Name == "ActionSystem");
    }

    [Fact]
    public async Task Repository_Should_Support_Standard_Operations()
    {
        // Arrange
        var template = new Template(Guid.NewGuid())
        {
            Name = "TestTemplate",
            Type = TemplateType.Action,
            Format = TextFormat.Plaintext,
            TemplateContent = "Test Content",
            RequiredPlaceholders = string.Empty,
            IsSystem = false,
            TemplateId = "test-template"
        };

        // Act & Assert - Insert
        Template? inserted = null;
        await WithUnitOfWorkAsync(async () =>
        {
            inserted = await _templateRepository.InsertAsync(template);
        });

        inserted.ShouldNotBeNull();
        inserted.Id.ShouldBe(template.Id);

        // Act & Assert - Get
        Template? retrieved = null;
        await WithUnitOfWorkAsync(async () =>
        {
            retrieved = await _templateRepository.GetAsync(template.Id);
        });

        retrieved.ShouldNotBeNull();
        retrieved.Name.ShouldBe("TestTemplate");

        // Act & Assert - Update
        retrieved.Update(
            "UpdatedName",
            retrieved.Type,
            retrieved.Format,
            retrieved.TemplateContent,
            retrieved.RequiredPlaceholders,
            retrieved.TemplateId);
        Template? updated = null;
        await WithUnitOfWorkAsync(async () =>
        {
            updated = await _templateRepository.UpdateAsync(retrieved);
        });

        updated.ShouldNotBeNull();
        updated.Name.ShouldBe("UpdatedName");

        // Act & Assert - Delete
        await WithUnitOfWorkAsync(async () =>
        {
            await _templateRepository.DeleteAsync(updated);
        });

        Template? deleted = null;
        await WithUnitOfWorkAsync(async () =>
        {
            deleted = await _templateRepository.FindAsync(updated.Id);
        });

        deleted.ShouldBeNull();
    }
}
