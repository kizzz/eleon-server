using System.Reflection;
using FluentAssertions;
using Xunit;

public class Boundaries
{
    private static Assembly Load(string name) => Assembly.Load(name);

    [Fact]
    public void Application_must_not_reference_EfCore_layers()
    {
        var app = Load("Eleon.Application");
        foreach (var refAsm in app.GetReferencedAssemblies())
        {
            refAsm.Name!.Should().NotContain("EfCore");
        }
    }

    [Fact]
    public void HttpApi_should_only_reference_Eleon_or_Contracts()
    {
        var http = Load("Eleon.HttpApi");
        foreach (var refAsm in http.GetReferencedAssemblies())
        {
            if (!refAsm.Name!.EndsWith(".Contracts"))
            {
                refAsm.Name!.Should().StartWith("Eleon");
            }
        }
    }
}
