// Type: ANT_Managed_Library.ANT_VersionInfo
// Assembly: ANT_NET, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Users\Alex\Desktop\SomaxisAfeDiagnosticMonitor_20120602_V1_0_1_1\SomaxisAfeDiagnosticMonitor_20120602_V1_0_1_1\ANT_NET.dll

using System.Reflection;
using System.Runtime.InteropServices;

namespace ANT_Managed_Library
{
  /// <summary>
  /// The information for this version of the ANT Managed Library
  /// 
  /// </summary>
  public static class ANT_VersionInfo
  {
    private static string applicationCode = "AMO";
    private static string versionSuffix = "";
    /// <summary>
    /// This string shows the date the library received its current version number
    /// 
    /// </summary>
    public static string versionNumberLastChangedOn = "Jun 17 2011";

    static ANT_VersionInfo()
    {
    }

    /// <summary>
    /// Returns the version information as a string
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// Managed Library Version String
    /// </returns>
    public static string getManagedLibraryVersion()
    {
      return ANT_VersionInfo.applicationCode + Assembly.GetExecutingAssembly().GetName().Version.ToString(4) + ANT_VersionInfo.versionSuffix;
    }

    [DllImport("ANT_WrappedLib.dll", CallingConvention = CallingConvention.Cdecl)]
    private extern static string getUnmanagedVersion();

    /// <summary>
    /// Gets the version string of the underlying unmanaged wrapper library, ANT_WrappedLib.dll
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// Unmanaged Wrapper Version String
    /// </returns>
    public static string getUnmanagedLibraryVersion()
    {
      return ANT_VersionInfo.getUnmanagedVersion();
    }
  }
}
