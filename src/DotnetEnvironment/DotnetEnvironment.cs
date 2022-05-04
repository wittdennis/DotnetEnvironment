namespace Wittdennis;

using System.Security;

public static class DotnetEnvironment
{
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
}
