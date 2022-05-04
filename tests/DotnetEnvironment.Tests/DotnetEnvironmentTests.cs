using Xunit;

namespace Wittdennis.Tests;

public class DotnetEnvironmentTests
{
    [Theory]
    [InlineData("unit", "test")]
    [InlineData("test", "unit")]
    public void Environment_Should_PreferDotnetEnvironmentOverAspNetCoreEnvironment(string dotnetEnv, string aspnetEnv)
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", dotnetEnv, EnvironmentVariableTarget.Process);
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", aspnetEnv, EnvironmentVariableTarget.Process);

        Assert.Equal(dotnetEnv, DotnetEnvironment.Environment);
    }

    [Fact]
    public void Environment_Should_ReturnProduction_When_DotnetEnvironmentAndAspNetCoreEnvironmentIsNotSet()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", null!, EnvironmentVariableTarget.Process);
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", null!, EnvironmentVariableTarget.Process);

        Assert.Equal("Production", DotnetEnvironment.Environment);
    }

    [Theory]
    [InlineData("unit")]
    [InlineData("test")]
    public void Environment_Should_ReturnAspNetCoreEnvironment_When_DotnetEnvironmentIsNotSet(string aspnetEnv)
    {        
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", null!, EnvironmentVariableTarget.Process);
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", aspnetEnv, EnvironmentVariableTarget.Process);

        Assert.Equal(aspnetEnv, DotnetEnvironment.Environment);
    }

    [Theory]
    [InlineData("development")]
    [InlineData("DeveLopment")]
    public void IsDevelopment_Should_ReturnTrue_When_EnvironmentIsDevelopment(string env)
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", env, EnvironmentVariableTarget.Process);

        Assert.True(DotnetEnvironment.IsDevelopment);
    }
}