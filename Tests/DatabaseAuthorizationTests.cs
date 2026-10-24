using AUBE.Pages;
using Microsoft.AspNetCore.Authorization;
using Xunit;
using Xunit.Abstractions;

namespace AUBE.Tests;

public class DatabaseAuthorizationTests
{
    private readonly ITestOutputHelper _output;

    public DatabaseAuthorizationTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void DatabasePage_RequiresAuthorization()
    {
        _output.WriteLine(
            "===================\n"+
            "Authorization Test\n" +
            "===================");

        var authorizeAttribute = Attribute
            .GetCustomAttributes(typeof(DatabaseModel), inherit: true)
            .OfType<AuthorizeAttribute>()
            .FirstOrDefault();

        Assert.NotNull(authorizeAttribute);
    }
}