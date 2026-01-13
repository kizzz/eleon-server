using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Volo.Abp.Application.Dtos;
using Xunit;
using BackgroundJobs.Module.BackgroundJobs;
using BackgroundJobs.Module.Controllers;
using BackgroundJobs.Module.TestBase;
using BackgroundJobs.Module.TestHelpers;

namespace BackgroundJobs.Module.HttpApi.Controllers;

public class BackgroundJobControllerTests : ApplicationTestBase
{
    [Fact]
    public async Task CancelBackgroundJobAsync_CallsAppService()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;
        var appService = Substitute.For<IBackgroundJobAppService>();
        appService.CancelBackgroundJobAsync(jobId).Returns(true);
        
        var logger = CreateMockLogger<BackgroundJobController>();
        var controller = new BackgroundJobController(appService, logger);

        // Act
        var result = await controller.CancelBackgroundJobAsync(jobId);

        // Assert
        result.Should().BeTrue();
        await appService.Received(1).CancelBackgroundJobAsync(jobId);
    }

    [Fact]
    public async Task CompleteAsync_CallsAppService()
    {
        // Arrange
        var input = new BackgroundJobExecutionCompleteDto
        {
            BackgroundJobId = TestConstants.JobIds.Job1,
            ExecutionId = TestConstants.ExecutionIds.Execution1
        };
        var appService = Substitute.For<IBackgroundJobAppService>();
        var dto = new BackgroundJobDto { Id = input.BackgroundJobId };
        appService.CompleteAsync(input).Returns(dto);
        
        var logger = CreateMockLogger<BackgroundJobController>();
        var controller = new BackgroundJobController(appService, logger);

        // Act
        var result = await controller.CompleteAsync(input);

        // Assert
        result.Should().Be(dto);
        await appService.Received(1).CompleteAsync(input);
    }

    [Fact]
    public async Task CreateAsync_CallsAppService()
    {
        // Arrange
        var input = new CreateBackgroundJobDto
        {
            Type = TestConstants.JobTypes.TestJob
        };
        var appService = Substitute.For<IBackgroundJobAppService>();
        var dto = new BackgroundJobDto { Type = input.Type };
        appService.CreateAsync(input).Returns(dto);
        
        var logger = CreateMockLogger<BackgroundJobController>();
        var controller = new BackgroundJobController(appService, logger);

        // Act
        var result = await controller.CreateAsync(input);

        // Assert
        result.Should().Be(dto);
        await appService.Received(1).CreateAsync(input);
    }

    [Fact]
    public async Task GetBackgroundJobByIdAsync_CallsAppService()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;
        var appService = Substitute.For<IBackgroundJobAppService>();
        var dto = new FullBackgroundJobDto { Id = jobId };
        appService.GetBackgroundJobByIdAsync(jobId).Returns(dto);
        
        var logger = CreateMockLogger<BackgroundJobController>();
        var controller = new BackgroundJobController(appService, logger);

        // Act
        var result = await controller.GetBackgroundJobByIdAsync(jobId);

        // Assert
        result.Should().Be(dto);
        await appService.Received(1).GetBackgroundJobByIdAsync(jobId);
    }

    [Fact]
    public async Task GetBackgroundJobListAsync_CallsAppService()
    {
        // Arrange
        var input = new BackgroundJobListRequestDto();
        var appService = Substitute.For<IBackgroundJobAppService>();
        var dto = new PagedResultDto<BackgroundJobHeaderDto>(0, new List<BackgroundJobHeaderDto>());
        appService.GetBackgroundJobListAsync(input).Returns(dto);
        
        var logger = CreateMockLogger<BackgroundJobController>();
        var controller = new BackgroundJobController(appService, logger);

        // Act
        var result = await controller.GetBackgroundJobListAsync(input);

        // Assert
        result.Should().Be(dto);
        await appService.Received(1).GetBackgroundJobListAsync(input);
    }

    [Fact]
    public async Task MarkExecutionStartedAsync_CallsAppService()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;
        var executionId = TestConstants.ExecutionIds.Execution1;
        var appService = Substitute.For<IBackgroundJobAppService>();
        appService.MarkExecutionStartedAsync(jobId, executionId).Returns(true);
        
        var logger = CreateMockLogger<BackgroundJobController>();
        var controller = new BackgroundJobController(appService, logger);

        // Act
        var result = await controller.MarkExecutionStartedAsync(jobId, executionId);

        // Assert
        result.Should().BeTrue();
        await appService.Received(1).MarkExecutionStartedAsync(jobId, executionId);
    }

    [Fact]
    public async Task RetryBackgroundJobAsync_CallsAppService()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;
        var appService = Substitute.For<IBackgroundJobAppService>();
        appService.RetryBackgroundJobAsync(jobId).Returns(true);
        
        var logger = CreateMockLogger<BackgroundJobController>();
        var controller = new BackgroundJobController(appService, logger);

        // Act
        var result = await controller.RetryBackgroundJobAsync(jobId);

        // Assert
        result.Should().BeTrue();
        await appService.Received(1).RetryBackgroundJobAsync(jobId);
    }
}

