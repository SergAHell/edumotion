// Type: ANT_Managed_Library.ANT_Exception
// Assembly: ANT_NET, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Users\Alex\Desktop\SomaxisAfeDiagnosticMonitor_20120602_V1_0_1_1\SomaxisAfeDiagnosticMonitor_20120602_V1_0_1_1\ANT_NET.dll

using System;

namespace ANT_Managed_Library
{
  /// <summary>
  /// An exception occuring in the ANT Managed Library
  /// 
  /// </summary>
  public class ANT_Exception : Exception
  {
    /// <summary>
    /// Prefixes given string with "ANTLibrary Exception: "
    /// 
    /// </summary>
    /// <param name="exceptionDetail">String to prefix</param>
    public ANT_Exception(string exceptionDetail)
      : base("ANTLibrary Exception: " + exceptionDetail)
    {
    }

    /// <summary>
    /// Prefixes given string with "ANTLibrary Exception: " and propates inner exception
    /// 
    /// </summary>
    /// <param name="exceptionDetail">String to prefix</param><param name="innerException">Inner exception</param>
    public ANT_Exception(string exceptionDetail, Exception innerException)
      : base("ANTLibrary Exception: " + exceptionDetail, innerException)
    {
    }

    /// <summary>
    /// Copy constructor
    /// 
    /// </summary>
    /// <param name="aex">ANTException to copy</param>
    public ANT_Exception(ANT_Exception aex)
      : base(aex.Message)
    {
    }
  }
}
