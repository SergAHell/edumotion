// Type: ANT_Managed_Library.dRawChannelResponseHandler
// Assembly: ANT_NET, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Users\Alex\Desktop\SomaxisAfeDiagnosticMonitor_20120602_V1_0_1_1\SomaxisAfeDiagnosticMonitor_20120602_V1_0_1_1\ANT_NET.dll

namespace ANT_Managed_Library
{
  /// <summary>
  /// Delegate for Channel Response Event for forwarding the raw msg struct. If you are coding in C# use the other response event version.
  /// 
  /// </summary>
  /// <param name="message">Message bytes received from device</param><param name="messageSize">Length of data in message structure</param>
  public delegate void dRawChannelResponseHandler(ANT_Device.ANTMessage message, ushort messageSize);
}
