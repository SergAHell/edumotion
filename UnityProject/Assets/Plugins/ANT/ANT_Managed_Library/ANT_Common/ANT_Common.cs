// Type: ANT_Managed_Library.ANT_Common
// Assembly: ANT_NET, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Users\Alex\Desktop\SomaxisAfeDiagnosticMonitor_20120602_V1_0_1_1\SomaxisAfeDiagnosticMonitor_20120602_V1_0_1_1\ANT_NET.dll

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ANT_Managed_Library
{
  /// <summary>
  /// This is a static class that manages all the functions and variables common to the whole scope of the library.
  /// 
  /// </summary>
  public static class ANT_Common
  {
    /// <summary>
    /// Enables or disables all devices from resetting on startup, shutdown, and on CWTestMode Failure.
    ///             Default = true.
    /// 
    /// </summary>
    public static bool autoResetIsEnabled = true;
    internal const string ANT_UNMANAGED_WRAPPER = "ANT_WrappedLib.dll";
    internal const string ANT_SI_LIBRARY = "DSI_SiUSBXp_3_1.DLL";
    internal const string ANT_SI_LIBRARY2 = "DSI_CP210xManufacturing_3_1.dll";

    static ANT_Common()
    {
    }

    [DllImport("ANT_WrappedLib.dll", CallingConvention = CallingConvention.Cdecl)]
    private extern static uint ANT_GetNumDevices();

    [DllImport("ANT_WrappedLib.dll", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_EnableDebugLogging();

    [DllImport("ANT_WrappedLib.dll", CallingConvention = CallingConvention.Cdecl)]
    private extern static void ANT_DisableDebugLogging();

    [DllImport("ANT_WrappedLib.dll", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_SetDebugLogDirectory(string pcDirectory);

    [DllImport("ANT_WrappedLib.dll", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_DebugThreadInit(string pucName);

    [DllImport("ANT_WrappedLib.dll", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_DebugThreadWrite(string pcMessage);

    [DllImport("ANT_WrappedLib.dll", CallingConvention = CallingConvention.Cdecl)]
    internal extern static int ANT_DebugResetTime();

    /// <summary>
    /// Returns the number of ANT USB devices currently detected by the system.
    /// 
    /// </summary>
    public static uint getNumDetectedUSBDevices()
    {
      return ANT_Common.ANT_GetNumDevices();
    }

    /// <summary>
    /// Checks if the unmanaged library is present in the application's working directory.
    ///             Throws an exception if the library is missing.
    /// 
    /// </summary>
    public static void checkUnmanagedLibrary()
    {
      //if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ANT_WrappedLib.dll")))
        //throw new ANT_Exception("ANT_WrappedLib.dll not found in working directory");
    }

    /// <summary>
    /// Checks if device specific libraries are present in the application's working directory.
    ///             Throws an exception if any of these is missing.
    /// 
    /// </summary>
    public static void checkUSBLibraries()
    { /*
      string str = (string) null;
      if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DSI_SiUSBXp_3_1.DLL")))
        str = "DSI_SiUSBXp_3_1.DLL";
      if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DSI_CP210xManufacturing_3_1.dll")))
        str = str == null ? "DSI_CP210xManufacturing_3_1.dll" : str + ", DSI_CP210xManufacturing_3_1.dll";
      if (str != null)
        throw new ANT_Exception(str + " not found in working directory");
        */
    }

    /// <overloads>Enables debug files</overloads>
    /// <summary>
    /// Initializes and enables debug logs for all devices
    ///             Note:  For application specific logs to work properly
    ///             (e.g. ANT-FS logs), this needs to be called BEFORE
    ///             creating an ANT Device.
    /// 
    /// </summary>
    public static bool enableDebugLogs()
    {
      return ANT_Common.ANT_EnableDebugLogging() == 1;
    }

    /// <summary>
    /// Initializes and enables debug logs for all devices,
    ///             and stores the log in the specified path.
    ///             Note:  For application specific logs to work properly
    ///             (e.g. ANT-FS logs), this needs to be called BEFORE
    ///             creating an ANT Device.
    /// 
    /// </summary>
    /// <param name="debugPath">Debug log directory</param>
    public static void enableDebugLogs(string debugPath)
    {
      ANT_Common.enableDebugLogs();
      ANT_Common.setDebugLogDirectory(debugPath);
    }

    /// <summary>
    /// Disables and closes the debug logs
    /// 
    /// </summary>
    public static void disableDebugLogs()
    {
      ANT_Common.ANT_DisableDebugLogging();
    }

    /// <summary>
    /// Set the directory the log files are saved to.
    ///             This string will prefix the file name so must end with a slash or will be part of the name.
    ///             ie: directoryPath='c:\ANT\logs' will result in files being saved to the \ANT directory named logsdevice0.txt.
    ///             Throws an exception if directory does not exist.
    /// 
    /// </summary>
    /// <param name="directoryPath">Path to directory to save log files in. Default is the running directory.
    ///             This string will prefix the file name so must end with a slash or will be part of the name.
    ///             ie: directoryPath='c:\ANT\logs' will result in files being saved to the \ANT directory named logsdevice0.txt.
    ///             </param>
    public static bool setDebugLogDirectory(string directoryPath)
    {
      if (!Directory.Exists(directoryPath))
        throw new ANT_Exception("Path does not exist");
      else
        return ANT_Common.ANT_SetDebugLogDirectory(directoryPath) == 1;
    }

    /// <summary>
    /// Creates a debug log for the currently executing thread
    /// 
    /// </summary>
    /// <param name="name">Name of file (will result in ao_debug_name)</param>
    /// <returns>
    /// True if successful
    /// </returns>
    internal static bool initDebugLogThread(string name)
    {
      return ANT_Common.ANT_DebugThreadInit(name) == 1;
    }

    /// <summary>
    /// Adds an application specific message to the log for the current thread
    /// 
    /// </summary>
    /// <param name="message">Message to write to the log</param>
    /// <returns>
    /// True on success
    /// </returns>
    internal static bool writeToDebugLog(string message)
    {
      return ANT_Common.ANT_DebugThreadWrite(message) == 1;
    }
  }
}
