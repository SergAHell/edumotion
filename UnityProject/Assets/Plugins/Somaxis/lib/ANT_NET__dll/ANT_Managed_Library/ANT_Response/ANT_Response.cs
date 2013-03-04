// Type: ANT_Managed_Library.ANT_Response
// Assembly: ANT_NET, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Users\Alex\Desktop\SomaxisAfeDiagnosticMonitor_20120602_V1_0_1_1\SomaxisAfeDiagnosticMonitor_20120602_V1_0_1_1\ANT_NET.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace ANT_Managed_Library
{
  /// <summary>
  /// Container for all the information passed from an ANT device callback function
  /// 
  /// </summary>
  public class ANT_Response
  {
    /// <summary>
    /// The object that created this response (ie: The corresponding ANTChannel or ANTDevice instance).
    /// 
    /// </summary>
    public object sender;
    /// <summary>
    /// The channel parameter received in the message. Note: For some messages this is not applicable.
    /// 
    /// </summary>
    public byte antChannel;
    /// <summary>
    /// The time the message was received.
    /// 
    /// </summary>
    public DateTime timeReceived;
    /// <summary>
    /// The MessageID of the response
    /// 
    /// </summary>
    public byte responseID;
    /// <summary>
    /// The raw contents of the response message
    /// 
    /// </summary>
    public byte[] messageContents;

    internal ANT_Response(object sender, byte antChannel, DateTime timeReceived, byte IDcode, byte[] messageContents)
    {
      this.sender = sender;
      this.antChannel = antChannel;
      this.timeReceived = timeReceived;
      this.responseID = IDcode;
      this.messageContents = messageContents;
    }

    /// <summary>
    /// Returns messageContents[2] cast to an ANTEventID. Throws an exception if this is not a channel event.
    /// 
    /// </summary>
    public ANT_ReferenceLibrary.ANTEventID getChannelEventCode()
    {
      if ((int) this.responseID != 64)
        throw new ANT_Exception("Response is not a channel event");
      else
        return (ANT_ReferenceLibrary.ANTEventID) this.messageContents[2];
    }

    /// <summary>
    /// Returns messageContents[1] cast to an ANTMessageID. Throws an exception if this is not a response event.
    /// 
    /// </summary>
    public ANT_ReferenceLibrary.ANTMessageID getMessageID()
    {
      if ((int) this.responseID != 64)
        throw new ANT_Exception("Response is not a response event");
      else
        return (ANT_ReferenceLibrary.ANTMessageID) this.messageContents[1];
    }

    /// <summary>
    /// Returns the 8-byte data payload of an ANT message. Throws an exception if this is not a received message.
    /// 
    /// </summary>
    /// 
    /// <returns/>
    public byte[] getDataPayload()
    {
      if (this.messageContents.Length == 9 && ((int) this.responseID == 78 || (int) this.responseID == 79 || ((int) this.responseID == 80 || (int) this.responseID == 114)))
        return Enumerable.ToArray<byte>(Enumerable.Skip<byte>((IEnumerable<byte>) this.messageContents, 1));
      else
        return this.splitExtMessage(ANT_Response.extMsgParts.DataPayload);
    }

    /// <summary>
    /// Returns the burst sequence number (upper three bits of channel number). Throws exception if this is not a burst event.
    /// 
    /// </summary>
    public byte getBurstSequenceNumber()
    {
      if ((int) this.responseID != 80 && (int) this.responseID != 95 && (int) this.responseID != 114)
        throw new ANT_Exception("Response is not a burst event");
      else
        return (byte) (((int) this.messageContents[0] & 224) >> 5);
    }

    /// <summary>
    /// Returns the channel ID portion of an extended message. Throws an exception if this is not an extended message.
    /// 
    /// </summary>
    public ANT_ChannelID getDeviceIDfromExt()
    {
      ANT_ChannelID antChannelId = new ANT_ChannelID();
      byte[] numArray = this.splitExtMessage(ANT_Response.extMsgParts.DeviceID);
      antChannelId.deviceNumber = (ushort) ((uint) numArray[0] + ((uint) numArray[1] << 8));
      antChannelId.pairingBit = Convert.ToBoolean((int) numArray[2] & 128);
      antChannelId.deviceTypeID = (byte) ((uint) numArray[2] & (uint) sbyte.MaxValue);
      antChannelId.transmissionTypeID = numArray[3];
      return antChannelId;
    }

    /// <summary>
    /// Returns true if this is an extended message, false otherwise
    /// 
    /// </summary>
    public bool isExtended()
    {
      return this.messageContents.Length >= 13 && (int) this.responseID != 114;
    }

    /// <summary>
    /// Splits and returns the requested part of an extended message. Throws an exception if this is not an extended message.
    /// 
    /// </summary>
    /// <param name="whichPart">The desired part of the message</param>
    private byte[] splitExtMessage(ANT_Response.extMsgParts whichPart)
    {
      if (!this.isExtended())
        throw new ANT_Exception("Response is not an extended message");
      byte[] numArray1;
      byte[] numArray2;
      if ((int) this.responseID == 93 || (int) this.responseID == 94 || (int) this.responseID == 95)
      {
        numArray1 = Enumerable.ToArray<byte>(Enumerable.Take<byte>(Enumerable.Skip<byte>((IEnumerable<byte>) this.messageContents, 1), 4));
        numArray2 = Enumerable.ToArray<byte>(Enumerable.Take<byte>(Enumerable.Skip<byte>((IEnumerable<byte>) this.messageContents, 5), 8));
      }
      else
      {
        if ((int) this.responseID != 78 && (int) this.responseID != 79 && (int) this.responseID != 80)
          throw new ANT_Exception("Response is not an extended message");
        numArray2 = Enumerable.ToArray<byte>(Enumerable.Take<byte>(Enumerable.Skip<byte>((IEnumerable<byte>) this.messageContents, 1), 8));
        if (((int) this.messageContents[9] & 128) == 0)
          throw new ANT_Exception("Response does not contain a channel ID");
        numArray1 = Enumerable.ToArray<byte>(Enumerable.Take<byte>(Enumerable.Skip<byte>((IEnumerable<byte>) this.messageContents, 10), 4));
      }
      switch (whichPart)
      {
        case ANT_Response.extMsgParts.DataPayload:
          return numArray2;
        case ANT_Response.extMsgParts.DeviceID:
          return numArray1;
        default:
          throw new ANT_Exception("Invalid extMsgPart");
      }
    }

    private enum extMsgParts
    {
      DataPayload,
      DeviceID,
    }
  }
}
