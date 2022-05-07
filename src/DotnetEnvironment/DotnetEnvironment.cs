using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DotnetEnvironment.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Pseud0R4ndom;

using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

/// <summary>
/// Provides information about the current environment and platform. This class cannot be inherited.
/// </summary>
public static class DotnetEnvironment
{
    private static IRuntimeHelper? s_runtimeHelper;

    /// <summary>
    /// Helper for OS dependency
    /// </summary>
    internal static IRuntimeHelper RuntimeHelper
    {
        get
        {                     
            if (s_runtimeHelper == null)
            {
                s_runtimeHelper = new RuntimeHelper();
            }
            return s_runtimeHelper;
        }
        set
        {
            s_runtimeHelper = value;
        }
    }

    /// <summary>
    /// Gets the current app environment
    /// </summary>
    public static string Environment 
        => System.Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

    /// <summary>
    /// Gets a bool value showing if the current app runs in development environment
    /// </summary>
    public static bool IsDevelopment
        => Environment.Equals("development", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Retrieves the value of an environment variable from the current process.
    /// </summary>
    /// <param name="variable">The name of the environment variable.</param>
    /// <returns>The value of the environment variable specified by <paramref name="variable" />, or <c>null</c> if the environment variable is not found.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="variable" /> is null.</exception>
    /// <exception cref="SecurityException">The caller does not have the required permission to perform this operation.</exception>
    public static string? GetEnvironmentVariable(string variable)
        => System.Environment.GetEnvironmentVariable(variable);

    /// <summary>
    /// Gets the directory where the application can store its log files
    /// </summary>
    /// <returns></returns>
    /// <exception cref="DirectoryNotFoundException">When a log location could not be determined</exception>
    /// <exception cref="NullReferenceException">When the entry assembly could not be determined. See remarks for more info</exception>
    /// <remarks>    
    /// This method uses <see href="https://docs.microsoft.com/dotnet/api/system.reflection.assembly.getentryassembly#remarks">Assembly.GetEntryAssembly()</see> internally and thus has the same limitations. 
    /// It is adviced to use the <see cref="GetLogDirectory(string)" instead.
    /// </remarks>
    public static string GetLogDirectory()     
        => GetLogDirectory(Assembly.GetEntryAssembly()!.GetName().Name!);    

    /// <summary>
    /// Gets the directory where the application can store its log files
    /// </summary>
    /// <param name="appName">Name of the app. Will be used for the full path</param>
    /// <returns></returns>
    /// <exception cref="DirectoryNotFoundException">When a log location could not be determined</exception>
    /// <exception cref="ArgumentException">When <paramref name="appName" /> is an absolute path</exception>
    /// <exception cref="ArgumentNullException">When <paramref name="appName" /> is null or empty</exception>
    public static string GetLogDirectory(string appName)
    {
        if (string.IsNullOrWhiteSpace(appName))
        {
            throw new ArgumentNullException(nameof(appName));
        }
        appName = appName.Trim().TrimStart('\\', '/');
        if (Path.IsPathRooted(appName))
        {
            throw new ArgumentException($"{nameof(appName)} can't be an absolute path", nameof(appName));
        }

        string path;
        if (RuntimeHelper.GetOSPlatform() == OSPlatform.Windows)
        {
            string? localAppData = GetEnvironmentVariable("LocalAppData");
            path = CombineLogPath(localAppData, @$"{appName}{Path.DirectorySeparatorChar}log");
        }
        else if (RuntimeHelper.GetOSPlatform() == OSPlatform.OSX)
        {
            string? homeFolder = GetEnvironmentVariable("HOME");
            path = CombineLogPath(homeFolder, $"{appName}{Path.DirectorySeparatorChar}log");
        }
        else if (RuntimeHelper.GetOSPlatform() == OSPlatform.Linux 
#if NETCOREAPP3_0_OR_GREATER
                || RuntimeHelper.GetOSPlatform() == OSPlatform.FreeBSD
#endif
        )
        {
            path = CombineLogPath($"{Path.DirectorySeparatorChar}var{Path.DirectorySeparatorChar}log", $"{appName}");
        }
        else
        {
            throw new DirectoryNotFoundException("Log directory could not be determined");
        }

        return path;
    }

    private static string CombineLogPath(string? prefix, string suffix)
    {
        if (string.IsNullOrWhiteSpace(prefix))
        {
            throw new DirectoryNotFoundException("Log directory could not be determined");
        }

#if NETCOREAPP
        return Path.Join(prefix, suffix);
#else
        return Path.Combine(prefix, suffix);
#endif
    }
}
