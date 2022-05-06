using System.Runtime.InteropServices;

using FakeItEasy;

using Xunit;

namespace Pseud0R4ndom.Tests;

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

    [Theory]
    [InlineData("VAR_NAME", "value")]
    [InlineData("VAR_NAME2", "Test")]
    [InlineData("VAR_NAME3", null!)]
    public void GetEnvironmentVariable_Should_ReturnEnvironmentVariable(string variable, string? value)
    {
        Environment.SetEnvironmentVariable(variable, value, EnvironmentVariableTarget.Process);

        Assert.Equal(value, DotnetEnvironment.GetEnvironmentVariable(variable));
    }

    [Theory]
    [InlineData("app1")]
    [InlineData("app2")]
    public void GetLogDirectory_Should_PlatformSpecificLogPath(string appName)
    {
        char seperator = Path.DirectorySeparatorChar;
        DotnetEnvironment.RuntimeHelper = A.Fake<IRuntimeHelper>();
        
        Environment.SetEnvironmentVariable("LocalAppData", @$"C:{seperator}Users{seperator}USER{seperator}AppData{seperator}Local", EnvironmentVariableTarget.Process);
        A.CallTo(() => DotnetEnvironment.RuntimeHelper.GetOSPlatform()).Returns(OSPlatform.Windows);        
        string logPath = DotnetEnvironment.GetLogDirectory(appName);
        Assert.Equal(@$"C:{seperator}Users{seperator}USER{seperator}AppData{seperator}Local{seperator}{appName}{seperator}log", logPath);

        Environment.SetEnvironmentVariable("HOME", $"{seperator}home{seperator}USER", EnvironmentVariableTarget.Process);
        A.CallTo(() => DotnetEnvironment.RuntimeHelper.GetOSPlatform()).Returns(OSPlatform.OSX);
        logPath = DotnetEnvironment.GetLogDirectory(appName);
        Assert.Equal($"{seperator}home{seperator}USER{seperator}{appName}{seperator}log", logPath);
    
        A.CallTo(() => DotnetEnvironment.RuntimeHelper.GetOSPlatform()).Returns(OSPlatform.Linux);
        logPath = DotnetEnvironment.GetLogDirectory(appName);
        Assert.Equal($"{seperator}var{seperator}log{seperator}{appName}", logPath);

        #if NETCOREAPP3_0_OR_GREATER
        A.CallTo(() => DotnetEnvironment.RuntimeHelper.GetOSPlatform()).Returns(OSPlatform.FreeBSD);
        logPath = DotnetEnvironment.GetLogDirectory(appName);
        Assert.Equal($"{seperator}var{seperator}log{seperator}{appName}", logPath);
        #endif
    }

    [Fact]
    public void GetLogDirectory_Should_ThrowDirectoryNotFoundException_When_LocalAppDataIsNotSetOnWindows()
    {
        DotnetEnvironment.RuntimeHelper = A.Fake<IRuntimeHelper>();
        
        Environment.SetEnvironmentVariable("LocalAppData", null!, EnvironmentVariableTarget.Process);
        A.CallTo(() => DotnetEnvironment.RuntimeHelper.GetOSPlatform()).Returns(OSPlatform.Windows);

        Assert.Throws<DirectoryNotFoundException>(() => DotnetEnvironment.GetLogDirectory("test"));
    }

    [Fact]
    public void GetLogDirectory_Should_ThrowDirectoryNotFoundException_When_HOMEIsNotSetOnOSX()
    {
        DotnetEnvironment.RuntimeHelper = A.Fake<IRuntimeHelper>();
        
        Environment.SetEnvironmentVariable("HOME", null!, EnvironmentVariableTarget.Process);
        A.CallTo(() => DotnetEnvironment.RuntimeHelper.GetOSPlatform()).Returns(OSPlatform.OSX);

        Assert.Throws<DirectoryNotFoundException>(() => DotnetEnvironment.GetLogDirectory("test"));
    }

    [Fact]
    public void GetLogDirectory_Should_ThrowDirectoryNotFoundException_When_OsPlatformCouldNotBeDetermined()
    {
        DotnetEnvironment.RuntimeHelper = A.Fake<IRuntimeHelper>();
        A.CallTo(() => DotnetEnvironment.RuntimeHelper.GetOSPlatform()).Returns(OSPlatform.Create("Test"));        

        Assert.Throws<DirectoryNotFoundException>(() => DotnetEnvironment.GetLogDirectory("test"));
    }

    [Fact]
    public void GetLogDirectory_Should_ReturnPathWithAssemblyName_When_MethodWithoutAppNameParameterIsUsed()
    {
        string appName = "testhost";
        char seperator = Path.DirectorySeparatorChar;
        DotnetEnvironment.RuntimeHelper = A.Fake<IRuntimeHelper>();
        
        Environment.SetEnvironmentVariable("LocalAppData", @$"C:{seperator}Users{seperator}USER{seperator}AppData{seperator}Local", EnvironmentVariableTarget.Process);
        A.CallTo(() => DotnetEnvironment.RuntimeHelper.GetOSPlatform()).Returns(OSPlatform.Windows);        
        string logPath = DotnetEnvironment.GetLogDirectory();
        Assert.Equal(@$"C:{seperator}Users{seperator}USER{seperator}AppData{seperator}Local{seperator}{appName}{seperator}log", logPath);

        Environment.SetEnvironmentVariable("HOME", $"{seperator}home{seperator}USER", EnvironmentVariableTarget.Process);
        A.CallTo(() => DotnetEnvironment.RuntimeHelper.GetOSPlatform()).Returns(OSPlatform.OSX);
        logPath = DotnetEnvironment.GetLogDirectory();
        Assert.Equal($"{seperator}home{seperator}USER{seperator}{appName}{seperator}log", logPath);
    
        A.CallTo(() => DotnetEnvironment.RuntimeHelper.GetOSPlatform()).Returns(OSPlatform.Linux);
        logPath = DotnetEnvironment.GetLogDirectory();
        Assert.Equal($"{seperator}var{seperator}log{seperator}{appName}", logPath);

        #if NETCOREAPP3_0_OR_GREATER
        A.CallTo(() => DotnetEnvironment.RuntimeHelper.GetOSPlatform()).Returns(OSPlatform.FreeBSD);
        logPath = DotnetEnvironment.GetLogDirectory();
        Assert.Equal($"{seperator}var{seperator}log{seperator}{appName}", logPath);
        #endif
    }

    [Fact]
    public void GetLogDirectory_Should_ThrowArgumentNullException_When_AppNameIsNull()
    {
        A.CallTo(() => DotnetEnvironment.RuntimeHelper.GetOSPlatform()).Returns(OSPlatform.Linux);
        Assert.Throws<ArgumentNullException>(() => DotnetEnvironment.GetLogDirectory(null!));
    }

    [Fact]
    public void RuntimeIdentifier_Should_CreateANewInstanceOfRuntimeHelper()
    {
        IRuntimeHelper runtimeHelper = DotnetEnvironment.RuntimeHelper;
        IRuntimeHelper runtimeHelper1 = DotnetEnvironment.RuntimeHelper;

        Assert.IsType<RuntimeHelper>(runtimeHelper);
        Assert.Same(runtimeHelper, runtimeHelper1);
    }
}