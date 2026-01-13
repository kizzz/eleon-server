using Eleon.Telemetry.Lib.Domain.Tests.TestBase;
using Eleon.Telemetry.Lib.Domain.Tests.TestHelpers;
using FluentAssertions;
using SharedModule.modules.Otel.Module;

namespace Eleon.Telemetry.Lib.Domain.Tests.Options;

public class OtelOptionsTests : TelemetryDomainTestBase
{
    [Fact]
    public void Equals_WithSameValues_Should_ReturnTrue()
    {
        // Arrange
        var options1 = TelemetryTestDataBuilder.ValidOptions();
        var options2 = TelemetryTestDataBuilder.ValidOptions();

        // Act
        var result = options1.Equals(options2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentEndpoint_Should_ReturnFalse()
    {
        // Arrange
        var options1 = new TelemetryTestDataBuilder()
            .WithTracesEndpoint("http://localhost:4318/v1/traces")
            .Build();
        var options2 = new TelemetryTestDataBuilder()
            .WithTracesEndpoint("http://localhost:4319/v1/traces")
            .Build();

        // Act
        var result = options1.Equals(options2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_WithDifferentProtocol_Should_ReturnFalse()
    {
        // Arrange
        var options1 = new TelemetryTestDataBuilder()
            .WithTracesProtocol("grpc")
            .Build();
        var options2 = new TelemetryTestDataBuilder()
            .WithTracesProtocol("http")
            .Build();

        // Act
        var result = options1.Equals(options2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_WithDifferentBatchSetting_Should_ReturnFalse()
    {
        // Arrange
        var options1 = new TelemetryTestDataBuilder()
            .WithTracesBatch(true)
            .Build();
        var options2 = new TelemetryTestDataBuilder()
            .WithTracesBatch(false)
            .Build();

        // Act
        var result = options1.Equals(options2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_WithDifferentInstrumentationToggle_Should_ReturnFalse()
    {
        // Arrange
        var options1 = new TelemetryTestDataBuilder()
            .WithTracesInstrumentation(aspNetCore: true)
            .Build();
        var options2 = new TelemetryTestDataBuilder()
            .WithTracesInstrumentation(aspNetCore: false)
            .Build();

        // Act
        var result = options1.Equals(options2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_WithNull_Should_ReturnFalse()
    {
        // Arrange
        var options = TelemetryTestDataBuilder.ValidOptions();

        // Act
        var result = options.Equals((OtelOptions?)null);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_WithSameInstance_Should_ReturnTrue()
    {
        // Arrange
        var options = TelemetryTestDataBuilder.ValidOptions();

        // Act
        var result = options.Equals(options);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Equals_WithNullServiceName_Should_TreatNullAsEqual()
    {
        // Arrange
        var options1 = new OtelOptions { ServiceName = null };
        var options2 = new OtelOptions { ServiceName = null };

        // Act
        var result = options1.Equals(options2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Equals_WithNullAndEmptyServiceName_Should_TreatAsDifferent()
    {
        // Arrange
        var options1 = new OtelOptions { ServiceName = null };
        var options2 = new OtelOptions { ServiceName = "" };

        // Act
        var result = options1.Equals(options2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_WithSameValues_Should_ReturnSameHashCode()
    {
        // Arrange
        var options1 = TelemetryTestDataBuilder.ValidOptions();
        var options2 = TelemetryTestDataBuilder.ValidOptions();

        // Act
        var hash1 = options1.GetHashCode();
        var hash2 = options2.GetHashCode();

        // Assert
        hash1.Should().Be(hash2);
    }

    [Fact]
    public void GetHashCode_WithDifferentValues_Should_ReturnDifferentHashCode()
    {
        // Arrange
        var options1 = new TelemetryTestDataBuilder()
            .WithTracesEndpoint("http://localhost:4318/v1/traces")
            .Build();
        var options2 = new TelemetryTestDataBuilder()
            .WithTracesEndpoint("http://localhost:4319/v1/traces")
            .Build();

        // Act
        var hash1 = options1.GetHashCode();
        var hash2 = options2.GetHashCode();

        // Assert
        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void GetHashCode_Should_BeStableAcrossMultipleCalls()
    {
        // Arrange
        var options = TelemetryTestDataBuilder.ValidOptions();

        // Act
        var hash1 = options.GetHashCode();
        var hash2 = options.GetHashCode();
        var hash3 = options.GetHashCode();

        // Assert
        hash1.Should().Be(hash2);
        hash2.Should().Be(hash3);
    }

    [Fact]
    public void Equals_WithNestedOptionsDifference_Should_ReturnFalse()
    {
        // Arrange
        var options1 = new TelemetryTestDataBuilder()
            .WithMetricsEndpoint("http://localhost:4318/v1/metrics")
            .Build();
        var options2 = new TelemetryTestDataBuilder()
            .WithMetricsEndpoint("http://localhost:4319/v1/metrics")
            .Build();

        // Act
        var result = options1.Equals(options2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_WithLogsOptionsDifference_Should_ReturnFalse()
    {
        // Arrange
        var options1 = new TelemetryTestDataBuilder()
            .WithLogsOptions(includeScopes: true, includeFormattedMessage: true)
            .Build();
        var options2 = new TelemetryTestDataBuilder()
            .WithLogsOptions(includeScopes: false, includeFormattedMessage: true)
            .Build();

        // Act
        var result = options1.Equals(options2);

        // Assert
        result.Should().BeFalse();
    }
}
