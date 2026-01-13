using System;
using Common.Module.Constants;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using VPortal.FileManager.Module.Factories;
using VPortal.FileManager.Module.Repositories;
using VPortal.FileManager.Module.Repositories.Compression;
using VPortal.FileManager.Module.Tests.TestBase;
using Xunit;

namespace VPortal.FileManager.Module.Tests.Domain.Factories;

public class CompressionFactoryTests : DomainTestBase
{
    private readonly CompressionFactory _factory;
    private readonly IServiceProvider _serviceProvider;
    private readonly ZipCompressionRepository _zipRepository;

    public CompressionFactoryTests()
    {
        _serviceProvider = Substitute.For<IServiceProvider>();
        _zipRepository = Substitute.For<ZipCompressionRepository>(
            CreateMockLogger<ZipCompressionRepository>(),
            Substitute.For<VPortal.Infrastructure.Module.Domain.DomainServices.ZipDomainService>(
                CreateMockLogger<VPortal.Infrastructure.Module.Domain.DomainServices.ZipDomainService>()));

        _factory = new CompressionFactory(_serviceProvider);
    }

    #region Get Tests

    [Fact]
    public void Get_ZipType_ReturnsZipRepository()
    {
        // Arrange
        _serviceProvider.GetService(typeof(ZipCompressionRepository))
            .Returns(_zipRepository);

        // Act
        var result = _factory.Get(FileCompressionType.Zip);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(_zipRepository);
    }

    [Fact]
    public void Get_DefaultParameter_ReturnsZipRepository()
    {
        // Arrange
        _serviceProvider.GetService(typeof(ZipCompressionRepository))
            .Returns(_zipRepository);

        // Act
        var result = _factory.Get();

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(_zipRepository);
    }

    [Fact]
    public void Get_InvalidType_ReturnsNull()
    {
        // Arrange
        var invalidType = (FileCompressionType)999;

        // Act
        var result = _factory.Get(invalidType);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Get_ZipType_ServiceNotRegistered_ThrowsException()
    {
        // Arrange
        _serviceProvider.GetService(typeof(ZipCompressionRepository))
            .Returns((object)null);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            _factory.Get(FileCompressionType.Zip));
    }

    #endregion

    #region Concurrency Tests

    [Fact]
    public void Get_ConcurrentAccess_ThreadSafe()
    {
        // Arrange
        _serviceProvider.GetService(typeof(ZipCompressionRepository))
            .Returns(_zipRepository);

        // Act
        var results = new ICompressionRepository[10];
        for (int i = 0; i < 10; i++)
        {
            results[i] = _factory.Get(FileCompressionType.Zip);
        }

        // Assert
        results.Should().AllBeEquivalentTo(_zipRepository);
        results.Should().HaveCount(10);
    }

    #endregion
}
