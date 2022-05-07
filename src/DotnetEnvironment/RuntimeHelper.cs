using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Pseud0R4ndom;

internal class RuntimeHelper : IRuntimeHelper
{
    /// <summary>
    /// Gets the current running os platform
    /// </summary>
    /// <returns></returns>
    [ExcludeFromCodeCoverage]
    public OSPlatform GetOSPlatform()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return OSPlatform.Windows;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return OSPlatform.Linux;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return OSPlatform.OSX;
        }
        #if NETCOREAPP3_0_OR_GREATER
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
        {
            return OSPlatform.FreeBSD;
        }
        #endif
        else
        {            
            return OSPlatform.Create("Unknown");
        }
    }
}