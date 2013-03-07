// Type: ANT_Managed_Library.ANT_ChannelID
// Assembly: ANT_NET, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Users\Alex\Desktop\SomaxisAfeDiagnosticMonitor_20120602_V1_0_1_1\SomaxisAfeDiagnosticMonitor_20120602_V1_0_1_1\ANT_NET.dll

namespace ANT_Managed_Library
{
  /// <summary>
  /// Structure containing the data composing a channel ID
  /// 
  /// </summary>
  public struct ANT_ChannelID
  {
    /// <summary>
    /// Device Number
    /// 
    /// </summary>
    public ushort deviceNumber;
    /// <summary>
    /// Pairing Bit
    /// 
    /// </summary>
    public bool pairingBit;
    /// <summary>
    /// Device Type ID
    /// 
    /// </summary>
    public byte deviceTypeID;
    /// <summary>
    /// Transmission Type ID
    /// 
    /// </summary>
    public byte transmissionTypeID;

    /// <summary>
    /// Initializes a new Channel ID Object
    /// 
    /// </summary>
    /// <param name="deviceNumber">Device Number</param><param name="deviceType">Device Type ID</param><param name="transmissionType">Transmission Type ID</param>
    public ANT_ChannelID(ushort deviceNumber, byte deviceType, byte transmissionType)
    {
      this.deviceNumber = deviceNumber;
      this.deviceTypeID = deviceType;
      this.transmissionTypeID = transmissionType;
      this.pairingBit = false;
    }
  }
}
