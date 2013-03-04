// Type: ANT_Managed_Library.ANT_ChannelStatus
// Assembly: ANT_NET, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Users\Alex\Desktop\SomaxisAfeDiagnosticMonitor_20120602_V1_0_1_1\SomaxisAfeDiagnosticMonitor_20120602_V1_0_1_1\ANT_NET.dll

namespace ANT_Managed_Library
{
  /// <summary>
  /// Contains the information returned by a channel status request message
  /// 
  /// </summary>
  public struct ANT_ChannelStatus
  {
    /// <summary>
    /// Bits 0-1 of the status response
    /// 
    /// </summary>
    public ANT_ReferenceLibrary.BasicChannelStatusCode BasicStatus;
    /// <summary>
    /// Bits 2-3 of the status response. Invalid on AP1.
    /// 
    /// </summary>
    public byte networkNumber;
    /// <summary>
    /// Bits 4-7 of the status response. Not a valid channelType on AP1.
    /// 
    /// </summary>
    public ANT_ReferenceLibrary.ChannelType ChannelType;

    /// <summary>
    /// Creates and fills the ChannelStatus
    /// 
    /// </summary>
    /// <param name="BasicStatus"/><param name="networkNumber"/><param name="ChannelType"/>
    public ANT_ChannelStatus(ANT_ReferenceLibrary.BasicChannelStatusCode BasicStatus, byte networkNumber, ANT_ReferenceLibrary.ChannelType ChannelType)
    {
      this.BasicStatus = BasicStatus;
      this.networkNumber = networkNumber;
      this.ChannelType = ChannelType;
    }
  }
}
