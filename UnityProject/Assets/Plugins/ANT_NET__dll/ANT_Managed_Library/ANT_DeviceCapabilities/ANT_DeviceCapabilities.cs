// Type: ANT_Managed_Library.ANT_DeviceCapabilities
// Assembly: ANT_NET, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Users\Alex\Desktop\SomaxisAfeDiagnosticMonitor_20120602_V1_0_1_1\SomaxisAfeDiagnosticMonitor_20120602_V1_0_1_1\ANT_NET.dll

using System.Text;

namespace ANT_Managed_Library
{
  /// <summary>
  /// Container for all the device capability information, returned from an ANTDevice
  /// 
  /// </summary>
  public class ANT_DeviceCapabilities
  {
    /// <summary>
    /// Number of channels available
    /// 
    /// </summary>
    public readonly byte maxANTChannels;
    /// <summary>
    /// Number of simultaneous networks allowed
    /// 
    /// </summary>
    public readonly byte maxNetworks;
    public readonly bool NoReceiveChannels;
    public readonly bool NoTransmitChannels;
    public readonly bool NoReceiveMessages;
    public readonly bool NoTransmitMessages;
    public readonly bool NoAckMessages;
    public readonly bool NoBurstMessages;
    public readonly bool PrivateNetworks;
    public readonly bool SerialNumber;
    public readonly bool perChannelTransmitPower;
    public readonly bool lowPrioritySearch;
    public readonly bool ScriptSupport;
    public readonly bool SearchList;
    public readonly bool OnboardLED;
    public readonly bool ExtendedMessaging;
    public readonly bool ScanModeSupport;
    public readonly bool ExtendedChannelAssignment;
    public readonly bool ProximitySearch;
    public readonly bool FS;
    public readonly bool FIT;
    public readonly bool AdvancedBurst;
    public readonly bool EventBuffering;
    public readonly bool EventFiltering;
    public readonly bool SearchScan;
    public readonly bool SearchSharing;
    public readonly bool RadioCoexConfig;
    /// <summary>
    /// Number of SensRcore data channels available
    /// 
    /// </summary>
    public readonly byte maxDataChannels;

    internal ANT_DeviceCapabilities(byte[] capabilitiesData)
    {
      if (capabilitiesData.Length != 16)
        throw new ANT_Exception("This function only decodes capabilities data of exactly length 16");
      this.maxANTChannels = capabilitiesData[0];
      this.maxNetworks = capabilitiesData[1];
      this.NoReceiveChannels = ((int) capabilitiesData[2] & 1) != 0;
      this.NoTransmitChannels = ((int) capabilitiesData[2] & 2) != 0;
      this.NoReceiveMessages = ((int) capabilitiesData[2] & 4) != 0;
      this.NoTransmitMessages = ((int) capabilitiesData[2] & 8) != 0;
      this.NoAckMessages = ((int) capabilitiesData[2] & 16) != 0;
      this.NoBurstMessages = ((int) capabilitiesData[2] & 32) != 0;
      this.PrivateNetworks = ((int) capabilitiesData[3] & 2) != 0;
      this.SerialNumber = ((int) capabilitiesData[3] & 8) != 0;
      this.perChannelTransmitPower = ((int) capabilitiesData[3] & 16) != 0;
      this.lowPrioritySearch = ((int) capabilitiesData[3] & 32) != 0;
      this.ScriptSupport = ((int) capabilitiesData[3] & 64) != 0;
      this.SearchList = ((int) capabilitiesData[3] & 128) != 0;
      this.OnboardLED = ((int) capabilitiesData[4] & 1) != 0;
      this.ExtendedMessaging = ((int) capabilitiesData[4] & 2) != 0;
      this.ScanModeSupport = ((int) capabilitiesData[4] & 4) != 0;
      this.ExtendedChannelAssignment = ((int) capabilitiesData[4] & 32) != 0;
      this.ProximitySearch = ((int) capabilitiesData[4] & 16) != 0;
      this.FS = ((int) capabilitiesData[4] & 64) != 0;
      this.FIT = ((int) capabilitiesData[4] & 128) != 0;
      this.maxDataChannels = capabilitiesData[5];
      this.AdvancedBurst = ((int) capabilitiesData[6] & 1) != 0;
      this.EventBuffering = ((int) capabilitiesData[6] & 2) != 0;
      this.EventFiltering = ((int) capabilitiesData[6] & 4) != 0;
      this.SearchScan = ((int) capabilitiesData[6] & 8) != 0;
      this.SearchSharing = ((int) capabilitiesData[6] & 16) != 0;
      this.RadioCoexConfig = ((int) capabilitiesData[6] & 32) != 0;
    }

    /// <summary>
    /// Prints a string containing a formatted, readable version of all the capabilities
    /// 
    /// </summary>
    public string printCapabilities()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("Max ANT Channels: " + (object) this.maxANTChannels);
      stringBuilder.AppendLine("Max Networks: " + (object) this.maxNetworks);
      stringBuilder.AppendLine("Max Data Channels: " + (object) this.maxDataChannels);
      stringBuilder.AppendLine("Capabilities:");
      if (this.NoReceiveChannels)
        stringBuilder.AppendLine("-No Receive Channels");
      if (this.NoTransmitChannels)
        stringBuilder.AppendLine("-No Transmit Channels");
      if (this.NoReceiveMessages)
        stringBuilder.AppendLine("-No Receive Messages");
      if (this.NoTransmitMessages)
        stringBuilder.AppendLine("-No Transmit Messages");
      if (this.NoAckMessages)
        stringBuilder.AppendLine("-No Acknowledged Messaging");
      if (this.NoBurstMessages)
        stringBuilder.AppendLine("-No Burst Messaging");
      if (this.PrivateNetworks)
        stringBuilder.AppendLine("-Private Networks");
      if (this.SerialNumber)
        stringBuilder.AppendLine("-Serial Number");
      if (this.perChannelTransmitPower)
        stringBuilder.AppendLine("-Per Channel Tx Power");
      if (this.lowPrioritySearch)
        stringBuilder.AppendLine("-Low Priority Search");
      if (this.ScriptSupport)
        stringBuilder.AppendLine("-Script Support");
      if (this.SearchList)
        stringBuilder.AppendLine("-Search List");
      if (this.OnboardLED)
        stringBuilder.AppendLine("-Onboard LED");
      if (this.ExtendedMessaging)
        stringBuilder.AppendLine("-Extended Messaging");
      if (this.ScanModeSupport)
        stringBuilder.AppendLine("-Scan Channel Support");
      if (this.ExtendedChannelAssignment)
        stringBuilder.AppendLine("-Ext Channel Assignment");
      if (this.ProximitySearch)
        stringBuilder.AppendLine("-Proximity Search");
      if (this.FS)
        stringBuilder.AppendLine("-FS");
      if (this.FIT)
        stringBuilder.AppendLine("-FIT");
      if (this.AdvancedBurst)
        stringBuilder.AppendLine("-Advanced Burst");
      if (this.EventBuffering)
        stringBuilder.AppendLine("-Event Buffering");
      if (this.EventFiltering)
        stringBuilder.AppendLine("-Event Filtering");
      if (this.SearchScan)
        stringBuilder.AppendLine("-Search Scan");
      if (this.SearchSharing)
        stringBuilder.AppendLine("-Search Sharing");
      if (this.RadioCoexConfig)
        stringBuilder.AppendLine("-Radio Coex Config");
      return ((object) stringBuilder).ToString();
    }

    /// <summary>
    /// Returns a formatted, readable string of all the capabilities
    /// 
    /// </summary>
    public override string ToString()
    {
      return this.printCapabilities();
    }

    /// <summary>
    /// Basic Capabilities Masks (3rd Byte)
    /// 
    /// </summary>
    public enum BasicCapabilitiesMasks : byte
    {
      NO_RX_CHANNELS = (byte) 1,
      NO_TX_CHANNELS = (byte) 2,
      NO_RX_MESSAGES = (byte) 4,
      NO_TX_MESSAGES = (byte) 8,
      NO_ACKD_MESSAGES = (byte) 16,
      NO_BURST_TRANSFER = (byte) 32,
    }

    /// <summary>
    /// Advanced Capabilities Masks 1 (4th Byte)
    /// 
    /// </summary>
    public enum AdvancedCapabilitiesMasks : byte
    {
      OVERUN_UNDERRUN = (byte) 1,
      NETWORK_CAPABLE = (byte) 2,
      AP1_VERSION_2 = (byte) 4,
      SERIAL_NUMBER_CAPABLE = (byte) 8,
      PER_CHANNEL_TX_POWER_CAPABLE = (byte) 16,
      LOW_PRIORITY_SEARCH_CAPABLE = (byte) 32,
      SCRIPT_CAPABLE = (byte) 64,
      SEARCH_LIST_CAPABLE = (byte) 128,
    }

    /// <summary>
    /// Advanced Capabilities Masks 2 (5th Byte)
    /// 
    /// </summary>
    public enum AdvancedCapabilities2Masks : byte
    {
      LED_CAPABLE = (byte) 1,
      EXT_MESSAGE_CAPABLE = (byte) 2,
      SCAN_MODE_CAPABLE = (byte) 4,
      RESERVED = (byte) 8,
      PROX_SEARCH_CAPABLE = (byte) 16,
      EXT_ASSIGN_CAPABLE = (byte) 32,
      FS_ANTFS_ENABLED = (byte) 64,
      FIT1_CAPABLE = (byte) 128,
    }

    /// <summary>
    /// Advanced Capabilities Masks 3 (7th Byte)
    /// 
    /// </summary>
    public enum AdvancedCapabilities3Masks : byte
    {
      ADVANCED_BURST_CAPABLE = (byte) 1,
      EVENT_BUFFERING_CAPABLE = (byte) 2,
      EVENT_FILTERING_CAPABLE = (byte) 4,
      SEARCH_SCAN_CAPABLE = (byte) 8,
      SEARCH_SHARING_CAPABLE = (byte) 16,
      RADIO_COEX_CONFIG_CAPABLE = (byte) 32,
    }
  }
}
