using Eleon.ErrorHandling.Lib.Tests.TestHelpers;
using FluentAssertions;
using Logging.Module.ErrorHandling.Constants;
using Logging.Module.ErrorHandling.Mappers;
using Logging.Module.ErrorHandling.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Xunit;

namespace Eleon.ErrorHandling.Lib.Tests.Mappers;

public class DefaultExceptionMapperTests
{
    private readonly DefaultExceptionMapper _mapper;

    public DefaultExceptionMapperTests()
    {
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions();
        var optionsWrapper = ErrorHandlingTestHelpers.CreateOptions(options);
        _mapper = new DefaultExceptionMapper(optionsWrapper);
    }

    [Fact]
    public void Map_UnauthorizedAccessException_Should_Return_403_Forbidden()
    {
        // Arrange
        var exception = new UnauthorizedAccessException("Access denied");

        // Act
        var result = _mapper.Map(exception);

        // Assert
        result.HttpStatus.Should().Be(StatusCodes.Status403Forbidden);
        result.AppCode.Should().Be("ELEON-AUTH-403");
        result.Title.Should().Be("Forbidden");
    }

    [Fact]
    public void Map_ArgumentException_Should_Return_400_BadRequest()
    {
        // Arrange
        var exception = new ArgumentException("Invalid argument");

        // Act
        var result = _mapper.Map(exception);

        // Assert
        result.HttpStatus.Should().Be(StatusCodes.Status400BadRequest);
        result.AppCode.Should().Be("ELEON-REQ-400");
        result.Title.Should().Be("Bad Request");
    }

    [Fact]
    public void Map_ArgumentNullException_Should_Return_400_BadRequest()
    {
        // Arrange
        var exception = new ArgumentNullException("param");

        // Act
        var result = _mapper.Map(exception);

        // Assert
        result.HttpStatus.Should().Be(StatusCodes.Status400BadRequest);
        result.AppCode.Should().Be("ELEON-REQ-400");
    }

    [Fact]
    public void Map_ArgumentOutOfRangeException_Should_Return_400_BadRequest()
    {
        // Arrange
        var exception = new ArgumentOutOfRangeException("param", "Out of range");

        // Act
        var result = _mapper.Map(exception);

        // Assert
        result.HttpStatus.Should().Be(StatusCodes.Status400BadRequest);
        result.AppCode.Should().Be("ELEON-REQ-400");
    }

    [Fact]
    public void Map_InvalidOperationException_Should_Return_400_BadRequest()
    {
        // Arrange
        var exception = new InvalidOperationException("Invalid operation");

        // Act
        var result = _mapper.Map(exception);

        // Assert
        result.HttpStatus.Should().Be(StatusCodes.Status400BadRequest);
        result.AppCode.Should().Be("ELEON-REQ-400");
    }

    [Fact]
    public void Map_NotImplementedException_Should_Return_501_NotImplemented()
    {
        // Arrange
        var exception = new NotImplementedException();

        // Act
        var result = _mapper.Map(exception);

        // Assert
        result.HttpStatus.Should().Be(StatusCodes.Status501NotImplemented);
        result.AppCode.Should().Be("ELEON-SRV-501");
        result.Title.Should().Be("Not Implemented");
    }

    [Fact]
    public void Map_TimeoutException_Should_Return_504_GatewayTimeout()
    {
        // Arrange
        var exception = new TimeoutException("Request timeout");

        // Act
        var result = _mapper.Map(exception);

        // Assert
        result.HttpStatus.Should().Be(StatusCodes.Status504GatewayTimeout);
        result.AppCode.Should().Be("ELEON-SRV-504");
        result.Title.Should().Be("Gateway Timeout");
    }

    [Fact]
    public void Map_ExceptionWithProxyInternalError_Should_Return_500_WithProxyCode()
    {
        // Arrange
        var exception = ErrorHandlingTestHelpers.CreateExceptionWithStatusCode(
            EleonsoftStatusCodes.Proxy.ProxyInternalError);

        // Act
        var result = _mapper.Map(exception);

        // Assert
        result.HttpStatus.Should().Be(StatusCodes.Status500InternalServerError);
        result.AppCode.Should().Be("ELEON-PROXY-550");
        result.Title.Should().Be("Proxy Internal Error");
    }

    [Fact]
    public void Map_ExceptionWithDefaultServerError_Should_Return_500_WithServerCode()
    {
        // Arrange
        var exception = ErrorHandlingTestHelpers.CreateExceptionWithStatusCode(
            EleonsoftStatusCodes.Default.DefaultServerError);

        // Act
        var result = _mapper.Map(exception);

        // Assert
        result.HttpStatus.Should().Be(StatusCodes.Status500InternalServerError);
        result.AppCode.Should().Be("ELEON-SRV-600");
        result.Title.Should().Be("Internal Server Error");
    }

    [Fact]
    public void Map_ExceptionWithUnknownStatusCode_Should_Return_DefaultWithCode()
    {
        // Arrange
        var exception = ErrorHandlingTestHelpers.CreateExceptionWithStatusCode(999);

        // Act
        var result = _mapper.Map(exception);

        // Assert
        result.HttpStatus.Should().Be(500); // DefaultHttpStatus
        result.AppCode.Should().Be("ELEON-999");
        result.Title.Should().Be("Error");
    }

    [Fact]
    public void Map_NullException_Should_Return_Default()
    {
        // Act
        var result = _mapper.Map(null!);

        // Assert
        result.HttpStatus.Should().Be(500);
        result.AppCode.Should().Be("ELEON-0000");
        result.Title.Should().Be("Internal Server Error");
    }

    [Fact]
    public void Map_ExceptionWithNoStatusCode_Should_Return_Default()
    {
        // Arrange
        var exception = new Exception("Test exception");

        // Act
        var result = _mapper.Map(exception);

        // Assert
        result.HttpStatus.Should().Be(500);
        result.AppCode.Should().Be("ELEON-0000");
        result.Title.Should().Be("Internal Server Error");
    }

    [Fact]
    public void Map_Should_Use_Options_For_Default_Values()
    {
        // Arrange
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions(o =>
        {
            o.DefaultHttpStatus = 503;
            o.DefaultAppCode = "CUSTOM-0000";
        });
        var optionsWrapper = ErrorHandlingTestHelpers.CreateOptions(options);
        var mapper = new DefaultExceptionMapper(optionsWrapper);
        var exception = new Exception("Test");

        // Act
        var result = mapper.Map(exception);

        // Assert
        result.HttpStatus.Should().Be(503);
        result.AppCode.Should().Be("CUSTOM-0000");
    }
}
