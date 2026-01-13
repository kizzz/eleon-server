using System;
using System.Threading.Tasks;
using Logging.Module;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Xunit;

namespace Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.HttpApi;

public class AntiforgeryValidationMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_PostLogin_ValidatesAntiforgery()
    {
        var antiforgery = Substitute.For<IAntiforgery>();
        antiforgery.ValidateRequestAsync(Arg.Any<HttpContext>())
            .Returns(Task.CompletedTask);
        var logger = Substitute.For<IVportalLogger<AntiforgeryValidationMiddleware>>();

        var invoked = false;
        RequestDelegate next = _ =>
        {
            invoked = true;
            return Task.CompletedTask;
        };

        var middleware = new AntiforgeryValidationMiddleware(next, antiforgery, logger);
        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Post;
        context.Request.Path = "/Account/Login";

        await middleware.InvokeAsync(context);

        await antiforgery.Received(1).ValidateRequestAsync(context);
        Assert.True(invoked);
    }

    [Fact]
    public async Task InvokeAsync_OtherPath_DoesNotValidate()
    {
        var antiforgery = Substitute.For<IAntiforgery>();
        var logger = Substitute.For<IVportalLogger<AntiforgeryValidationMiddleware>>();

        var invoked = false;
        RequestDelegate next = _ =>
        {
            invoked = true;
            return Task.CompletedTask;
        };

        var middleware = new AntiforgeryValidationMiddleware(next, antiforgery, logger);
        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Post;
        context.Request.Path = "/Account/Other";

        await middleware.InvokeAsync(context);

        await antiforgery.Received(0).ValidateRequestAsync(Arg.Any<HttpContext>());
        Assert.True(invoked);
    }

    [Fact]
    public async Task InvokeAsync_InvalidAntiforgery_Throws()
    {
        var antiforgery = Substitute.For<IAntiforgery>();
        antiforgery.ValidateRequestAsync(Arg.Any<HttpContext>())
            .Returns<Task>(_ => throw new AntiforgeryValidationException("invalid"));
        var logger = Substitute.For<IVportalLogger<AntiforgeryValidationMiddleware>>();

        var middleware = new AntiforgeryValidationMiddleware(_ => Task.CompletedTask, antiforgery, logger);
        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Post;
        context.Request.Path = "/Account/Login";

        await Assert.ThrowsAsync<AntiforgeryValidationException>(() => middleware.InvokeAsync(context));
    }
}
