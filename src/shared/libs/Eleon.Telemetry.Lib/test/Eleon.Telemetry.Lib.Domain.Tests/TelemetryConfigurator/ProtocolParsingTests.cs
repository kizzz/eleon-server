using Eleon.Telemetry.Lib.Domain.Tests.TestBase;
using Eleon.Telemetry.Lib.Domain.Tests.TestHelpers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OpenTelemetry.Exporter;
using SharedModule.modules.Otel.Module;
using System;
using System.Reflection;

namespace Eleon.Telemetry.Lib.Domain.Tests.TelemetryConfiguratorTests;

/// <summary>
/// Tests for protocol parsing behavior in TelemetryConfigurator.
/// Uses reflection to test the private ResolveProtocol method.
/// </summary>
public class ProtocolParsingTests : TelemetryDomainTestBase
{
    private static readonly MethodInfo? ResolveProtocolMethod = typeof(TelemetryConfigurator)
        .GetMethod("ResolveProtocol", BindingFlags.NonPublic | BindingFlags.Static);

    [Fact]
    public void ResolveProtocol_WithNull_Should_DefaultToGrpc()
    {
        // Arrange & Act
        var result = InvokeResolveProtocol(null);

        // Assert
        result.Should().Be(OtlpExportProtocol.Grpc);
    }

    [Fact]
    public void ResolveProtocol_WithEmpty_Should_DefaultToGrpc()
    {
        // Arrange & Act
        var result = InvokeResolveProtocol("");

        // Assert
        result.Should().Be(OtlpExportProtocol.Grpc);
    }

    [Fact]
    public void ResolveProtocol_WithWhitespace_Should_DefaultToGrpc()
    {
        // Arrange & Act
        var result = InvokeResolveProtocol("   ");

        // Assert
        result.Should().Be(OtlpExportProtocol.Grpc);
    }

    [Fact]
    public void ResolveProtocol_WithGrpc_Should_ReturnGrpc()
    {
        // Arrange & Act
        var result = InvokeResolveProtocol("grpc");

        // Assert
        result.Should().Be(OtlpExportProtocol.Grpc);
    }

    [Fact]
    public void ResolveProtocol_WithHttp_Should_ReturnHttpProtobuf()
    {
        // Arrange & Act
        var result = InvokeResolveProtocol("http");

        // Assert
        result.Should().Be(OtlpExportProtocol.HttpProtobuf);
    }

    [Fact]
    public void ResolveProtocol_WithGrpcUpperCase_Should_BeCaseInsensitive()
    {
        // Arrange & Act
        var result = InvokeResolveProtocol("GRPC");

        // Assert
        result.Should().Be(OtlpExportProtocol.Grpc);
    }

    [Fact]
    public void ResolveProtocol_WithHttpUpperCase_Should_BeCaseInsensitive()
    {
        // Arrange & Act
        var result = InvokeResolveProtocol("HTTP");

        // Assert
        result.Should().Be(OtlpExportProtocol.HttpProtobuf);
    }

    [Fact]
    public void ResolveProtocol_WithGrpcMixedCase_Should_BeCaseInsensitive()
    {
        // Arrange & Act
        var result = InvokeResolveProtocol("Grpc");

        // Assert
        result.Should().Be(OtlpExportProtocol.Grpc);
    }

    [Fact]
    public void ResolveProtocol_WithHttpMixedCase_Should_BeCaseInsensitive()
    {
        // Arrange & Act
        var result = InvokeResolveProtocol("Http");

        // Assert
        result.Should().Be(OtlpExportProtocol.HttpProtobuf);
    }

    [Fact]
    public void ResolveProtocol_WithGrpcWithWhitespace_Should_Trim()
    {
        // Arrange & Act
        var result = InvokeResolveProtocol(" grpc ");

        // Assert
        result.Should().Be(OtlpExportProtocol.Grpc);
    }

    [Fact]
    public void ResolveProtocol_WithHttpWithWhitespace_Should_Trim()
    {
        // Arrange & Act
        var result = InvokeResolveProtocol(" http ");

        // Assert
        result.Should().Be(OtlpExportProtocol.HttpProtobuf);
    }

    [Fact]
    public void ResolveProtocol_WithUnknownProtocol_Should_DefaultToGrpc()
    {
        // Arrange & Act
        var result = InvokeResolveProtocol("unknown");

        // Assert
        result.Should().Be(OtlpExportProtocol.Grpc);
    }

    private static OtlpExportProtocol InvokeResolveProtocol(string? protocol)
    {
        if (ResolveProtocolMethod == null)
        {
            throw new InvalidOperationException("ResolveProtocol method not found via reflection");
        }

        var result = ResolveProtocolMethod.Invoke(null, new object?[] { protocol });
        return (OtlpExportProtocol)result!;
    }
}
