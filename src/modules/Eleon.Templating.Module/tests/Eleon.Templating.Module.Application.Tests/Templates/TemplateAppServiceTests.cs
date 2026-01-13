using Common.Module.Constants;
using Eleon.Templating.Module.Templates;
using Shouldly;
using Xunit;

namespace Eleon.Templating.Module.Templates;

public class TemplateAppServiceTests : ModuleApplicationTestBase<ModuleApplicationTestModule>
{
    private readonly ITemplateAppService _templateAppService;

    public TemplateAppServiceTests()
    {
        _templateAppService = GetRequiredService<ITemplateAppService>();
    }

    [Fact]
    public async Task CreateAsync_Should_Return_TemplateDto()
    {
        // Arrange
        var input = new CreateUpdateTemplateDto
        {
            Name = "TestTemplate",
            Type = TemplateType.Action,
            Format = TextFormat.Plaintext,
            TemplateContent = "Hello {name}",
            RequiredPlaceholders = "name",
            IsSystem = false,
            TemplateId = "test-template-id"
        };

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
            await _templateAppService.CreateAsync(input));

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe(input.Name);
        result.Type.ShouldBe(input.Type);
        result.Format.ShouldBe(input.Format);
        result.TemplateContent.ShouldBe(input.TemplateContent);
    }

    [Fact]
    public async Task GetListAsync_Should_Return_Paged_Results()
    {
        // Arrange
        var input = new GetTemplateListInput
        {
            MaxResultCount = 10,
            SkipCount = 0
        };

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
            await _templateAppService.GetListAsync(input));

        // Assert
        result.ShouldNotBeNull();
        result.Items.ShouldNotBeNull();
    }

    [Fact]
    public async Task ApplyTemplateAsync_Should_Return_Rendered_Template()
    {
        // Arrange - First create a template
        var createInput = new CreateUpdateTemplateDto
        {
            Name = "TestApplyTemplate",
            Type = TemplateType.Action,
            Format = TextFormat.Plaintext,
            TemplateContent = "Hello {name}",
            RequiredPlaceholders = string.Empty,
            IsSystem = false,
            TemplateId = "apply-template-id"
        };

        var applyInput = new ApplyTemplateInput
        {
            TemplateName = createInput.Name,
            TemplateType = createInput.Type,
            Placeholders = new Dictionary<string, string> { { "name", "World" } }
        };

        // Act
        TemplateDto? createdTemplate = null;
        string? result = null;

        await WithUnitOfWorkAsync(async () =>
        {
            createdTemplate = await _templateAppService.CreateAsync(createInput);
            result = await _templateAppService.ApplyTemplateAsync(applyInput);
        });

        // Assert
        createdTemplate.ShouldNotBeNull();
        result.ShouldNotBeNull();
        result.ShouldBe("Hello World");
    }
}
