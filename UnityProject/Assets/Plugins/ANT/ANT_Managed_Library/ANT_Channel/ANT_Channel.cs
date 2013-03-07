// Type: ANT_Managed_Library.ANT_Channel
// Assembly: ANT_NET, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Users\n8fern\Desktop\SomaxisDecompile\ANT_NET.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ANT_Managed_Library
{
  /// <summary>
  /// Control class for an individual ANT channel. Created and accessed through the ANTDevice class.
  /// 
  /// </summary>
  public class ANT_Channel : IANT_Channel, IDisposable
  {
    private IntPtr unmanagedANTFramerPointer = IntPtr.Zero;
    private readonly ANT_Device creatingDevice;
    private byte channelNumber;
    private bool disposed;
    private dRawChannelResponseHandler m_rawChannelResponse;
    private dDeviceNotificationHandler m_DeviceNotification;
    private dChannelResponseHandler m_channelResponse;

    /// <summary>
    /// The channel callback event for forwarding the raw msg struct. Triggered every time a message is received from the ANT device.
    ///             Examples include transmit and receive messages. If you are coding in C# use the other response event version.
    /// 
    /// </summary>
    public event dRawChannelResponseHandler rawChannelResponse
    {
      [MethodImpl(MethodImplOptions.Synchronized)] add
      {
        this.m_rawChannelResponse = this.m_rawChannelResponse + value;
      }
      [MethodImpl(MethodImplOptions.Synchronized)] remove
      {
        this.m_rawChannelResponse = this.m_rawChannelResponse - value;
      }
    }

    /// <summary>
    /// This event is fired whenever there are events on the device level that may impact the channel.
    ///             Events that currently occur (Event, value of notification info Object):
    ///                 Reset, null
    ///                 Shutdown, null
    /// 
    /// </summary>
    public event dDeviceNotificationHandler DeviceNotification
    {
      [MethodImpl(MethodImplOptions.Synchronized)] add
      {
        this.m_DeviceNotification = this.m_DeviceNotification + value;
      }
      [MethodImpl(MethodImplOptions.Synchronized)] remove
      {
        this.m_DeviceNotification = this.m_DeviceNotification - value;
      }
    }

    /// <summary>
    /// The channel callback event. Triggered every time a message is received from the ANT device.
    ///             Examples include transmit and receive messages.
    /// 
    /// </summary>
    public event dChannelResponseHandler channelResponse
    {
      [MethodImpl(MethodImplOptions.Synchronized)] add
      {
        this.m_channelResponse = this.m_channelResponse + value;
      }
      [MethodImpl(MethodImplOptions.Synchronized)] remove
      {
        this.m_channelResponse = this.m_channelResponse - value;
      }
    }

    internal ANT_Channel(ANT_Device creatingDevice, byte ucChannelNumber)
    {
      this.creatingDevice = creatingDevice;
      this.unmanagedANTFramerPointer = creatingDevice.getFramerPtr();
      this.channelNumber = ucChannelNumber;
    }

    [DllImport("ANT_WrappedLib.dll", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_GetChannelID(IntPtr FramerPtr, byte ucANTChannel_, ref ushort pusDeviceNumber_, ref byte pucDeviceType_, ref byte pucTransmitType_, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib.dll", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_GetChannelStatus(IntPtr FramerPtr, byte ucANTChannel_, ref byte pucStatus_, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib.dll", EntryPoint = "ANT_AssignChannel_RTO", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_AssignChannel(IntPtr FramerPtr, byte ucANTChannel, byte ucChanType, byte ucNetNumber, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib.dll", EntryPoint = "ANT_AssignChannelExt_RTO", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_AssignChannelExt(IntPtr FramerPtr, byte ucANTChannel, byte ucChanType, byte ucNetNumber, byte ucExtFlags, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib.dll", EntryPoint = "ANT_UnAssignChannel_RTO", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_UnAssignChannel(IntPtr FramerPtr, byte ucANTChannel, uint ulResponseTime);

    [DllImport("ANT_WrappedLib.dll", EntryPoint = "ANT_SetChannelId_RTO", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_SetChannelId(IntPtr FramerPtr, byte ucANTChannel, ushort usDeviceNumber, byte ucDeviceType, byte ucTransmissionType_, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib.dll", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_SetSerialNumChannelId_RTO(IntPtr FramerPtr, byte ucANTChannel_, byte ucDeviceType_, byte ucTransmissionType_, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib.dll", EntryPoint = "ANT_SetChannelPeriod_RTO", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_SetChannelPeriod(IntPtr FramerPtr, byte ucANTChannel_, ushort usMesgPeriod_, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib.dll", EntryPoint = "ANT_SetChannelRFFreq_RTO", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_SetChannelRFFreq(IntPtr FramerPtr, byte ucANTChannel_, byte ucRFFreq_, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib.dll", EntryPoint = "ANT_SetChannelTxPower_RTO", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_SetChannelTxPower(IntPtr FramerPtr, byte ucANTChannel_, byte ucTransmitPower_, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib.dll", EntryPoint = "ANT_SetChannelSearchTimeout_RTO", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_SetChannelSearchTimeout(IntPtr FramerPtr, byte ucANTChannel_, byte ucSearchTimeout_, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib.dll", EntryPoint = "ANT_OpenChannel_RTO", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_OpenChannel(IntPtr FramerPtr, byte ucANTChannel, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib.dll", EntryPoint = "ANT_CloseChannel_RTO", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_CloseChannel(IntPtr FramerPtr, byte ucANTChannel, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib.dll", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_SendBroadcastData(IntPtr FramerPtr, byte ucANTChannel, byte[] pucData);

    [DllImport("ANT_WrappedLib.dll", EntryPoint = "ANT_SendAcknowledgedData_RTO", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_SendAcknowledgedData(IntPtr FramerPtr, byte ucANTChannel, byte[] pucData, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib.dll", EntryPoint = "ANT_SendBurstTransfer_RTO", CallingConvention = CallingConvention.Cdecl)]
    private extern static byte ANT_SendBurstTransfer(IntPtr FramerPtr, byte ucANTChannel_, byte[] pucData_, uint usNumBytes, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib.dll", EntryPoint = "ANT_SendAdvancedBurstTransfer_RTO", CallingConvention = CallingConvention.Cdecl)]
    private extern static byte ANT_SendAdvancedBurstTransfer(IntPtr FramerPtr, byte ucANTChannel_, byte[] pucData_, uint usNumBytes, byte numStdPcktsPerSerialMsg_, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib.dll", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_SendExtBroadcastData(IntPtr FramerPtr, byte ucANTChannel, ushort usDeviceNumber, byte ucDeviceType, byte ucTransmissionType_, byte[] pucData);

    [DllImport("ANT_WrappedLib.dll", EntryPoint = "ANT_SendExtAcknowledgedData_RTO", CallingConvention = CallingConvention.Cdecl)]
    private extern static byte ANT_SendExtAcknowledgedData(IntPtr FramerPtr, byte ucANTChannel, ushort usDeviceNumber, byte ucDeviceType, byte ucTransmissionType_, byte[] pucData, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib.dll", EntryPoint = "ANT_SendExtBurstTransfer_RTO", CallingConvention = CallingConvention.Cdecl)]
    private extern static byte ANT_SendExtBurstTransfer(IntPtr FramerPtr, byte ucANTChannel_, ushort usDeviceNumber, byte ucDeviceType, byte ucTransmissionType_, byte[] pucData_, uint usNumBytes, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib.dll", EntryPoint = "ANT_SetLowPriorityChannelSearchTimeout_RTO", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_SetLowPriorityChannelSearchTimeout(IntPtr FramerPtr, byte ucANTChannel_, byte ucSearchTimeout_, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib.dll", EntryPoint = "ANT_AddChannelID_RTO", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_AddChannelID(IntPtr FramerPtr, byte ucANTChannel_, ushort usDeviceNumber_, byte ucDeviceType_, byte ucTransmissionType_, byte ucListIndex_, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib.dll", EntryPoint = "ANT_ConfigList_RTO", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_ConfigList(IntPtr FramerPtr, byte ucANTChannel_, byte ucListSize_, byte ucExclude_, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib.dll", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_SetProximitySearch(IntPtr FramerPtr, byte ucANTChannel_, byte ucSearchThreshold_, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib.dll", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_ConfigFrequencyAgility(IntPtr FramerPtr, byte ucANTChannel_, byte ucFreq1_, byte ucFreq2_, byte ucFreq3_, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib.dll", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_SetAGCConfig(IntPtr FramerPtr, byte ucANTChannel_, byte ucAGCConfigByte, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib.dll", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_SetSearchWaveform(IntPtr FramerPtr, byte ucANTChannel_, ushort usSearchWaveform_, uint ulResponseTime_);

    /// <summary>
    /// Returns the ANTDevice that this channel belongs to
    /// 
    /// </summary>
    public ANT_Device getParentDevice()
    {
      return this.creatingDevice;
    }

    /// <summary>
    /// Returns the underlying C++ ANT framer reference that this channel uses for messaging. Useful to pass to unmanaged C++ implementations.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// Pointer to the C++ ANT framer for messaging
    /// </returns>
    public IntPtr getUnmgdFramer()
    {
      return this.creatingDevice.getFramerPtr();
    }

    /// <summary>
    /// Returns the channel number of this instance
    /// 
    /// </summary>
    public byte getChannelNum()
    {
      return this.channelNumber;
    }

    internal void NotifyDeviceEvent(ANT_Device.DeviceNotificationCode notification, object notificationInfo)
    {
      if (this.m_DeviceNotification == null)
        return;
      this.m_DeviceNotification(notification, notificationInfo);
    }

    internal void MessageReceived(ANT_Device.ANTMessage newMessage, ushort messageSize)
    {
      if (this.m_channelResponse != null)
        this.m_channelResponse(new ANT_Response((object) this, this.channelNumber, DateTime.Now, newMessage.msgID, Enumerable.ToArray<byte>(Enumerable.Take<byte>((IEnumerable<byte>) newMessage.ucharBuf, (int) messageSize))));
      if (this.m_rawChannelResponse == null)
        return;
      this.m_rawChannelResponse(newMessage, messageSize);
    }

    /// <summary>
    /// Dispose this channel.
    /// 
    /// </summary>
    public void Dispose()
    {
      this.creatingDevice.channelDisposed(this.channelNumber);
      this.disposed = true;
      GC.SuppressFinalize((object) this);
    }

    /// <summary>
    /// Returns current channel status.
    ///             Throws exception on timeout.
    /// 
    /// </summary>
    /// <param name="responseWaitTime">Time to wait for device success response</param>
    public ANT_ChannelStatus requestStatus(uint responseWaitTime)
    {
      if (this.disposed)
        throw new ObjectDisposedException("This ANTChannel object has been disposed");
      byte pucStatus_ = (byte) 0;
      if (ANT_Channel.ANT_GetChannelStatus(this.unmanagedANTFramerPointer, this.channelNumber, ref pucStatus_, responseWaitTime) == 1)
        return new ANT_ChannelStatus((ANT_ReferenceLibrary.BasicChannelStatusCode) ((uint) pucStatus_ & 3U), (byte) (((int) pucStatus_ & 12) >> 2), (ANT_ReferenceLibrary.ChannelType) ((uint) pucStatus_ & 240U));
      else
        throw new ANT_Exception("Timed out waiting for requested message");
    }

    /// <summary>
    /// Returns the channel ID
    ///             Throws exception on timeout
    /// 
    /// </summary>
    /// <param name="responseWaitTime">Time to wait for device success response</param>
    /// <returns/>
    public ANT_ChannelID requestID(uint responseWaitTime)
    {
      if (this.disposed)
        throw new ObjectDisposedException("This ANTChannel object has been disposed");
      ushort pusDeviceNumber_ = (ushort) 0;
      byte pucDeviceType_ = (byte) 0;
      byte pucTransmitType_ = (byte) 0;
      if (ANT_Channel.ANT_GetChannelID(this.unmanagedANTFramerPointer, this.channelNumber, ref pusDeviceNumber_, ref pucDeviceType_, ref pucTransmitType_, responseWaitTime) == 1)
        return new ANT_ChannelID(pusDeviceNumber_, pucDeviceType_, pucTransmitType_);
      else
        throw new ANT_Exception("Timed out waiting for requested message");
    }

    /// <overloads>Assign channel</overloads>
    /// <summary>
    /// Assign an ANT channel along with its main parameters.
    ///             Throws exception if the network number is invalid.
    /// 
    /// </summary>
    /// <param name="channelTypeByte">Channel Type byte</param><param name="networkNumber">Network to assign to channel, must be less than device's max networks-1</param><param name="responseWaitTime">Time to wait for device success response</param>
    /// <returns>
    /// True on success. Note: Always returns true with a response time of 0
    /// </returns>
    public bool assignChannel(ANT_ReferenceLibrary.ChannelType channelTypeByte, byte networkNumber, uint responseWaitTime)
    {
      if (this.disposed)
        throw new ObjectDisposedException("This ANTChannel object has been disposed");
      if ((int) networkNumber > (int) this.creatingDevice.getDeviceCapabilities().maxNetworks - 1)
        throw new ANT_Exception("Network number must be less than device's max networks - 1");
      else
        return ANT_Channel.ANT_AssignChannel(this.unmanagedANTFramerPointer, this.channelNumber, (byte) channelTypeByte, networkNumber, responseWaitTime) == 1;
    }

    /// <summary>
    /// Assign an ANT channel.
    /// 
    /// </summary>
    /// <param name="channelTypeByte">Channel Type byte</param><param name="networkNumber">Network to assign to channel</param>
    public void assignChannel(ANT_ReferenceLibrary.ChannelType channelTypeByte, byte networkNumber)
    {
      this.assignChannel(channelTypeByte, networkNumber, 0U);
    }

    /// <overloads>Assign channel (extended)</overloads>
    /// <summary>
    /// Assign an ANT channel, using extended channel assignment
    ///             Throws exception if the network number is invalid.
    /// 
    /// </summary>
    /// <param name="channelTypeByte">Channel Type byte</param><param name="networkNumber">Network to assign to channel, must be less than device's max netwoks - 1</param><param name="extAssignByte">Extended assignment byte</param><param name="responseWaitTime">Time to wait for device success response</param>
    /// <returns>
    /// True on success. Note: Always returns true with a response time of 0
    /// </returns>
    public bool assignChannelExt(ANT_ReferenceLibrary.ChannelType channelTypeByte, byte networkNumber, ANT_ReferenceLibrary.ChannelTypeExtended extAssignByte, uint responseWaitTime)
    {
      if (this.disposed)
        throw new ObjectDisposedException("This ANTChannel object has been disposed");
      if ((int) networkNumber > (int) this.creatingDevice.getDeviceCapabilities().maxNetworks - 1)
        throw new ANT_Exception("Network number must be less than device's max networks - 1");
      else
        return ANT_Channel.ANT_AssignChannelExt(this.unmanagedANTFramerPointer, this.channelNumber, (byte) channelTypeByte, networkNumber, (byte) extAssignByte, responseWaitTime) == 1;
    }

    /// <summary>
    /// Assign an ANT channel, using extended channel assignment
    ///             Throws exception if the network number is invalid.
    /// 
    /// </summary>
    /// <param name="channelTypeByte">Channel Type byte</param><param name="networkNumber">Network to assign to channel, must be less than device's max netwoks - 1</param><param name="extAssignByte">Extended assignment byte</param>
    public void assignChannelExt(ANT_ReferenceLibrary.ChannelType channelTypeByte, byte networkNumber, ANT_ReferenceLibrary.ChannelTypeExtended extAssignByte)
    {
      this.assignChannelExt(channelTypeByte, networkNumber, extAssignByte, 0U);
    }

    /// <overloads>Unassign channel</overloads>
    /// <summary>
    /// Unassign this channel.
    /// 
    /// </summary>
    /// <param name="responseWaitTime">Time to wait for device success response</param>
    /// <returns>
    /// True on success. Note: Always returns true with a response time of 0
    /// </returns>
    public bool unassignChannel(uint responseWaitTime)
    {
      if (this.disposed)
        throw new ObjectDisposedException("This ANTChannel object has been disposed");
      else
        return ANT_Channel.ANT_UnAssignChannel(this.unmanagedANTFramerPointer, this.channelNumber, responseWaitTime) == 1;
    }

    /// <summary>
    /// Unassigns this channel.
    /// 
    /// </summary>
    public void unassignChannel()
    {
      this.unassignChannel(0U);
    }

    /// <overloads>Set the Channel ID</overloads>
    /// <summary>
    /// Set the Channel ID of this channel.
    ///             Throws exception if device type is &gt; 127.
    /// 
    /// </summary>
    /// <param name="deviceNumber">Device number to assign to channel. Set to 0 for receiver wild card matching</param><param name="pairingEnabled">Device pairing bit.</param><param name="deviceTypeID">Device type to assign to channel. Must be less than 128. Set to 0 for receiver wild card matching</param><param name="transmissionTypeID">Transmission type to assign to channel. Set to 0 for receiver wild card matching</param><param name="responseWaitTime">Time to wait for device success response</param>
    /// <returns>
    /// True on success. Note: Always returns true with a response time of 0
    /// </returns>
    public bool setChannelID(ushort deviceNumber, bool pairingEnabled, byte deviceTypeID, byte transmissionTypeID, uint responseWaitTime)
    {
      if (this.disposed)
        throw new ObjectDisposedException("This ANTChannel object has been disposed");
      if ((int) deviceTypeID > (int) sbyte.MaxValue)
        throw new ANT_Exception("Device Type ID is larger than 127");
      if (pairingEnabled)
          deviceTypeID |= (byte)byte.MinValue;
      return ANT_Channel.ANT_SetChannelId(this.unmanagedANTFramerPointer, this.channelNumber, deviceNumber, deviceTypeID, transmissionTypeID, responseWaitTime) == 1;
    }

    /// <summary>
    /// Set the Channel ID of this channel.
    ///             Throws exception if device type is &gt; 127.
    /// 
    /// </summary>
    /// <param name="deviceNumber">Device number to assign to channel. Set to 0 for receiver wild card matching</param><param name="pairingEnabled">Device pairing bit</param><param name="deviceTypeID">Device type to assign to channel. Set to 0 for receiver wild card matching</param><param name="transmissionTypeID">Transmission type to assign to channel. Set to 0 for receiver wild card matching</param>
    public void setChannelID(ushort deviceNumber, bool pairingEnabled, byte deviceTypeID, byte transmissionTypeID)
    {
      this.setChannelID(deviceNumber, pairingEnabled, deviceTypeID, transmissionTypeID, 0U);
    }

    /// <overloads>Sets the Channel ID, using serial number as device number</overloads>
    /// <summary>
    /// Identical to setChannelID, except last two bytes of serial number are used for device number.
    ///             Not available on all ANT devices.
    ///             Throws exception if device type is &gt; 127.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// True on success. Note: Always returns true with a response time of 0
    /// </returns>
    public bool setChannelID_UsingSerial(bool pairingEnabled, byte deviceTypeID, byte transmissionTypeID, uint waitResponseTime)
    {
      if (this.disposed)
        throw new ObjectDisposedException("This ANTChannel object has been disposed");
      if ((int) deviceTypeID > (int) sbyte.MaxValue)
        throw new ANT_Exception("Device Type ID is larger than 127");
      if (pairingEnabled)
          deviceTypeID |= 0;// (byte)sbyte.MinValue;
      return ANT_Channel.ANT_SetSerialNumChannelId_RTO(this.unmanagedANTFramerPointer, this.channelNumber, deviceTypeID, transmissionTypeID, waitResponseTime) == 1;
    }

    /// <summary>
    /// Identical to setChannelID, except last two bytes of serial number are used for device number.
    /// 
    /// </summary>
    public void setChannelID_UsingSerial(bool pairingEnabled, byte deviceTypeID, byte transmissionTypeID)
    {
      this.setChannelID_UsingSerial(pairingEnabled, deviceTypeID, transmissionTypeID, 0U);
    }

    /// <overloads>Sets channel message period</overloads>
    /// <summary>
    /// Set this channel's messaging period
    /// 
    /// </summary>
    /// <param name="messagePeriod_32768unitspersecond">Desired period in seconds * 32768</param><param name="responseWaitTime">Time to wait for device success response</param>
    /// <returns>
    /// True on success. Note: Always returns true with a response time of 0
    /// </returns>
    public bool setChannelPeriod(ushort messagePeriod_32768unitspersecond, uint responseWaitTime)
    {
      if (this.disposed)
        throw new ObjectDisposedException("This ANTChannel object has been disposed");
      else
        return ANT_Channel.ANT_SetChannelPeriod(this.unmanagedANTFramerPointer, this.channelNumber, messagePeriod_32768unitspersecond, responseWaitTime) == 1;
    }

    /// <summary>
    /// Set this channel's messaging period
    /// 
    /// </summary>
    /// <param name="messagePeriod_32768unitspersecond">Desired period in seconds * 32768</param>
    public void setChannelPeriod(ushort messagePeriod_32768unitspersecond)
    {
      this.setChannelPeriod(messagePeriod_32768unitspersecond, 0U);
    }

    /// <overloads>Sets channel RF Frequency</overloads>
    /// <summary>
    /// Set this channel's RF frequency, with the given offset from 2400Mhz.
    ///             Note: Changing this frequency may affect the ability to certify the product in certain areas of the world.
    /// 
    /// </summary>
    /// <param name="RFFreqOffset">Offset to add to 2400Mhz</param><param name="responseWaitTime">Time to wait for device success response</param>
    /// <returns>
    /// True on success. Note: Always returns true with a response time of 0
    /// </returns>
    public bool setChannelFreq(byte RFFreqOffset, uint responseWaitTime)
    {
      if (this.disposed)
        throw new ObjectDisposedException("This ANTChannel object has been disposed");
      else
        return ANT_Channel.ANT_SetChannelRFFreq(this.unmanagedANTFramerPointer, this.channelNumber, RFFreqOffset, responseWaitTime) == 1;
    }

    /// <summary>
    /// Set this channel's RF frequency, with the given offset from 2400Mhz.
    ///             Note: Changing this frequency may affect the ability to certify the product in certain areas of the world.
    /// 
    /// </summary>
    /// <param name="RFFreqOffset">Offset to add to 2400Mhz</param>
    public void setChannelFreq(byte RFFreqOffset)
    {
      this.setChannelFreq(RFFreqOffset, 0U);
    }

    /// <overloads>Sets the channel transmission power</overloads>
    /// <summary>
    /// Set the transmission power of this channel
    ///             Throws exception if device is not capable of per-channel transmit power.
    /// 
    /// </summary>
    /// <param name="transmitPower">Transmission power to set to</param><param name="responseWaitTime">Time to wait for device success response</param>
    /// <returns>
    /// True on success. Note: Always returns true with a response time of 0
    /// </returns>
    public bool setChannelTransmitPower(ANT_ReferenceLibrary.TransmitPower transmitPower, uint responseWaitTime)
    {
      if (this.disposed)
        throw new ObjectDisposedException("This ANTChannel object has been disposed");
      if (!this.creatingDevice.getDeviceCapabilities().perChannelTransmitPower)
        throw new ANT_Exception("Device not capable of per-channel transmit power");
      else
        return ANT_Channel.ANT_SetChannelTxPower(this.unmanagedANTFramerPointer, this.channelNumber, (byte) transmitPower, responseWaitTime) == 1;
    }

    /// <summary>
    /// Set the transmission power of this channel
    /// 
    /// </summary>
    /// <param name="transmitPower">Transmission power to set to</param>
    public void setChannelTransmitPower(ANT_ReferenceLibrary.TransmitPower transmitPower)
    {
      this.setChannelTransmitPower(transmitPower, 0U);
    }

    /// <overloads>Sets the channel search timeout</overloads>
    /// <summary>
    /// Set the search timeout
    /// 
    /// </summary>
    /// <param name="searchTimeout">timeout in 2.5 second units (in newer devices 255=infinite)</param><param name="responseWaitTime">Time to wait for device success response</param>
    /// <returns>
    /// True on success. Note: Always returns true with a response time of 0
    /// </returns>
    public bool setChannelSearchTimeout(byte searchTimeout, uint responseWaitTime)
    {
      if (this.disposed)
        throw new ObjectDisposedException("This ANTChannel object has been disposed");
      else
        return ANT_Channel.ANT_SetChannelSearchTimeout(this.unmanagedANTFramerPointer, this.channelNumber, searchTimeout, responseWaitTime) == 1;
    }

    /// <summary>
    /// Set the search timeout
    /// 
    /// </summary>
    /// <param name="searchTimeout">timeout in 2.5 second units (in newer devices 255=infinite)</param>
    public void setChannelSearchTimeout(byte searchTimeout)
    {
      this.setChannelSearchTimeout(searchTimeout, 0U);
    }

    /// <overloads>Opens the channel</overloads>
    /// <summary>
    /// Opens this channel
    /// 
    /// </summary>
    /// <param name="responseWaitTime">Time to wait for device success response</param>
    /// <returns>
    /// True on success. Note: Always returns true with a response time of 0
    /// </returns>
    public bool openChannel(uint responseWaitTime)
    {
      if (this.disposed)
        throw new ObjectDisposedException("This ANTChannel object has been disposed");
      else
        return ANT_Channel.ANT_OpenChannel(this.unmanagedANTFramerPointer, this.channelNumber, responseWaitTime) == 1;
    }

    /// <summary>
    /// Opens this channel
    /// 
    /// </summary>
    public void openChannel()
    {
      this.openChannel(0U);
    }

    /// <overloads>Sends broadcast message</overloads>
    /// <summary>
    /// Sends the given data on the broadcast transmission.
    ///             Throws exception if data &gt; 8-bytes in length
    /// 
    /// </summary>
    /// <param name="data">data to send (length 8 or less)</param>
    public bool sendBroadcastData(byte[] data)
    {
      if (this.disposed)
        throw new ObjectDisposedException("This ANTChannel object has been disposed");
      int length = 8 - data.Length;
      if (length < 0)
        throw new ANT_Exception("Send data must not be greater than 8 bytes");
      data = Enumerable.ToArray<byte>(Enumerable.Concat<byte>((IEnumerable<byte>) data, (IEnumerable<byte>) new byte[length]));
      return ANT_Channel.ANT_SendBroadcastData(this.unmanagedANTFramerPointer, this.channelNumber, data) == 1;
    }

    /// <overloads>Sends acknowledged message</overloads>
    /// <summary>
    /// Sends the given data as an acknowledged transmission. Returns: 0=fail, 1=pass, 2=timeout, 3=cancelled
    ///             Throws exception if data &gt; 8-bytes in length
    /// 
    /// </summary>
    /// <param name="data">data to send (length 8 or less)</param><param name="ackWaitTime">Time in ms to wait for acknowledgement</param>
    /// <returns>
    /// 0=fail, 1=pass, 2=timeout, 3=cancelled
    /// </returns>
    public ANT_ReferenceLibrary.MessagingReturnCode sendAcknowledgedData(byte[] data, uint ackWaitTime)
    {
      if (this.disposed)
        throw new ObjectDisposedException("This ANTChannel object has been disposed");
      int length = 8 - data.Length;
      if (length < 0)
        throw new ANT_Exception("Send data must not be greater than 8 bytes");
      data = Enumerable.ToArray<byte>(Enumerable.Concat<byte>((IEnumerable<byte>) data, (IEnumerable<byte>) new byte[length]));
      return (ANT_ReferenceLibrary.MessagingReturnCode) ANT_Channel.ANT_SendAcknowledgedData(this.unmanagedANTFramerPointer, this.channelNumber, data, ackWaitTime);
    }

    /// <summary>
    /// Sends the given data as an acknowledged transmission.
    ///             Throws exception if data &gt; 8-bytes in length
    /// 
    /// </summary>
    /// <param name="data">data to send (length 8 or less)</param>
    public void sendAcknowledgedData(byte[] data)
    {
      int num = (int) this.sendAcknowledgedData(data, 0U);
    }

    /// <overloads>Sends burst transfer</overloads>
    /// <summary>
    /// Sends the given data as a burst transmission. Returns: 0=fail, 1=pass, 2=timeout, 3=cancelled
    /// 
    /// </summary>
    /// <param name="data">data to send, can be any length</param><param name="completeWaitTime">Time in ms to wait for completion of transfer</param>
    /// <returns>
    /// 0=fail, 1=pass, 2=timeout, 3=cancelled
    /// </returns>
    public ANT_ReferenceLibrary.MessagingReturnCode sendBurstTransfer(byte[] data, uint completeWaitTime)
    {
      if (this.disposed)
        throw new ObjectDisposedException("This ANTChannel object has been disposed");
      int length = 8 - data.Length % 8;
      if (length != 8)
        data = Enumerable.ToArray<byte>(Enumerable.Concat<byte>((IEnumerable<byte>) data, (IEnumerable<byte>) new byte[length]));
      return (ANT_ReferenceLibrary.MessagingReturnCode) ANT_Channel.ANT_SendBurstTransfer(this.unmanagedANTFramerPointer, this.channelNumber, data, (uint) data.Length, completeWaitTime);
    }

    /// <summary>
    /// Sends the given data as a burst transmission.
    /// 
    /// </summary>
    /// <param name="data">data to send, can be any length</param>
    public void sendBurstTransfer(byte[] data)
    {
      int num = (int) this.sendBurstTransfer(data, 0U);
    }

    /// <summary>
    /// Sends the given data as an advanced burst transmission. Returns: 0=fail, 1=pass, 2=timeout, 3=cancelled
    /// 
    /// </summary>
    /// <param name="data">data to send, can be any length</param><param name="numStdPcktsPerSerialMsg">determines how many bytes of data are sent in each packet
    ///             over the serial port in multiples of packet size (the size of the packet payload at the
    ///             wireless level is determined by the pckt size set in the ConfigureAdvancedBurst command).</param><param name="completeWaitTime">Time in ms to wait for completion of transfer</param>
    /// <returns>
    /// 0=fail, 1=pass, 2=timeout, 3=cancelled, 4=invalid parameter
    /// </returns>
    public ANT_ReferenceLibrary.MessagingReturnCode sendAdvancedBurstTransfer(byte[] data, byte numStdPcktsPerSerialMsg, uint completeWaitTime)
    {
      if (this.disposed)
        throw new ObjectDisposedException("This ANTChannel object has been disposed");
      int length = 8 - data.Length % 8;
      if (length != 8)
        data = Enumerable.ToArray<byte>(Enumerable.Concat<byte>((IEnumerable<byte>) data, (IEnumerable<byte>) new byte[length]));
      return (ANT_ReferenceLibrary.MessagingReturnCode) ANT_Channel.ANT_SendAdvancedBurstTransfer(this.unmanagedANTFramerPointer, this.channelNumber, data, (uint) data.Length, numStdPcktsPerSerialMsg, completeWaitTime);
    }

    /// <summary>
    /// Sends the given data as an advanced burst transmission.
    /// 
    /// </summary>
    /// <param name="data">data to send, can be any length</param><param name="numStdPcktsPerSerialMsg">determines how many bytes of data are sent in each packet
    ///             over the serial port in multiples of packet size (the size of the packet payload at the
    ///             wireless level is determined by the pckt size set in the ConfigureAdvancedBurst command).</param>
    public void sendAdvancedBurstTransfer(byte[] data, byte numStdPcktsPerSerialMsg)
    {
      int num = (int) this.sendAdvancedBurstTransfer(data, numStdPcktsPerSerialMsg, 0U);
    }

    /// <overloads>Sends extended broadcast message</overloads>
    /// <summary>
    /// Sends the given data as an extended broadcast transmission.
    ///             Throws exception if data &gt; 8-bytes in length
    /// 
    /// </summary>
    /// <param name="deviceNumber">Device number of channel ID to send to</param><param name="deviceTypeID">Device type of channel ID to send to</param><param name="transmissionTypeID">Transmission type of channel ID to send to</param><param name="data">data to send (length 8 or less)</param>
    public bool sendExtBroadcastData(ushort deviceNumber, byte deviceTypeID, byte transmissionTypeID, byte[] data)
    {
      if (this.disposed)
        throw new ObjectDisposedException("This ANTChannel object has been disposed");
      int length = 8 - data.Length;
      if (length < 0)
        throw new ANT_Exception("Send data must not be greater than 8 bytes");
      data = Enumerable.ToArray<byte>(Enumerable.Concat<byte>((IEnumerable<byte>) data, (IEnumerable<byte>) new byte[length]));
      return ANT_Channel.ANT_SendExtBroadcastData(this.unmanagedANTFramerPointer, this.channelNumber, deviceNumber, deviceTypeID, transmissionTypeID, data) == 1;
    }

    /// <overloads>Sends extended acknowledged message</overloads>
    /// <summary>
    /// Sends the given data as an extended acknowledged transmission. Returns: 0=fail, 1=pass, 2=timeout, 3=cancelled
    ///             Throws exception if data &gt; 8-bytes in length
    /// 
    /// </summary>
    /// <param name="deviceNumber">Device number of channel ID to send to</param><param name="deviceTypeID">Device type of channel ID to send to</param><param name="transmissionTypeID">Transmission type of channel ID to send to</param><param name="data">data to send (length 8 or less)</param><param name="ackWaitTime">Time in ms to wait for acknowledgement</param>
    /// <returns>
    /// 0=fail, 1=pass, 2=timeout, 3=cancelled
    /// </returns>
    public ANT_ReferenceLibrary.MessagingReturnCode sendExtAcknowledgedData(ushort deviceNumber, byte deviceTypeID, byte transmissionTypeID, byte[] data, uint ackWaitTime)
    {
      if (this.disposed)
        throw new ObjectDisposedException("This ANTChannel object has been disposed");
      int length = 8 - data.Length;
      if (length < 0)
        throw new ANT_Exception("Send data must not be greater than 8 bytes");
      data = Enumerable.ToArray<byte>(Enumerable.Concat<byte>((IEnumerable<byte>) data, (IEnumerable<byte>) new byte[length]));
      return (ANT_ReferenceLibrary.MessagingReturnCode) ANT_Channel.ANT_SendExtAcknowledgedData(this.unmanagedANTFramerPointer, this.channelNumber, deviceNumber, deviceTypeID, transmissionTypeID, data, ackWaitTime);
    }

    /// <summary>
    /// Sends the given data as an extended acknowledged transmission.
    ///             Throws exception if data &gt; 8-bytes in length
    /// 
    /// </summary>
    /// <param name="deviceNumber">Device number of channel ID to send to</param><param name="deviceTypeID">Device type of channel ID to send to</param><param name="transmissionTypeID">Transmission type of channel ID to send to</param><param name="data">data to send (length 8 or less)</param>
    public void sendExtAcknowledgedData(ushort deviceNumber, byte deviceTypeID, byte transmissionTypeID, byte[] data)
    {
      int num = (int) this.sendExtAcknowledgedData(deviceNumber, deviceTypeID, transmissionTypeID, data, 0U);
    }

    /// <overloads>Sends extended burst data</overloads>
    /// <summary>
    /// Sends the given data as an extended burst transmission. Returns: 0=fail, 1=pass, 2=timeout, 3=cancelled
    /// 
    /// </summary>
    /// <param name="deviceNumber">Device number of channel ID to send to</param><param name="deviceTypeID">Device type of channel ID to send to</param><param name="transmissionTypeID">Transmission type of channel ID to send to</param><param name="data">data to send, can be any length</param><param name="completeWaitTime">Time in ms to wait for completion of transfer</param>
    /// <returns>
    /// 0=fail, 1=pass, 2=timeout, 3=cancelled
    /// </returns>
    public ANT_ReferenceLibrary.MessagingReturnCode sendExtBurstTransfer(ushort deviceNumber, byte deviceTypeID, byte transmissionTypeID, byte[] data, uint completeWaitTime)
    {
      if (this.disposed)
        throw new ObjectDisposedException("This ANTChannel object has been disposed");
      int length = 8 - data.Length % 8;
      if (length != 8)
        data = Enumerable.ToArray<byte>(Enumerable.Concat<byte>((IEnumerable<byte>) data, (IEnumerable<byte>) new byte[length]));
      return (ANT_ReferenceLibrary.MessagingReturnCode) ANT_Channel.ANT_SendExtBurstTransfer(this.unmanagedANTFramerPointer, this.channelNumber, deviceNumber, deviceTypeID, transmissionTypeID, data, (uint) data.Length, completeWaitTime);
    }

    /// <summary>
    /// Sends the given data as an extended burst transmission.
    /// 
    /// </summary>
    /// <param name="deviceNumber">Device number of channel ID to send to</param><param name="deviceTypeID">Device type of channel ID to send to</param><param name="transmissionTypeID">Transmission type of channel ID to send to</param><param name="data">data to send, can be any length</param>
    public void sendExtBurstTransfer(ushort deviceNumber, byte deviceTypeID, byte transmissionTypeID, byte[] data)
    {
      int num = (int) this.sendExtBurstTransfer(deviceNumber, deviceTypeID, transmissionTypeID, data, 0U);
    }

    /// <overloads>Closes the channel</overloads>
    /// <summary>
    /// Close this channel
    /// 
    /// </summary>
    /// <param name="responseWaitTime">Time to wait for device success response</param>
    /// <returns>
    /// True on success. Note: Always returns true with a response time of 0
    /// </returns>
    public bool closeChannel(uint responseWaitTime)
    {
      if (this.disposed)
        throw new ObjectDisposedException("This ANTChannel object has been disposed");
      else
        return ANT_Channel.ANT_CloseChannel(this.unmanagedANTFramerPointer, this.channelNumber, responseWaitTime) == 1;
    }

    /// <summary>
    /// Close this channel
    /// 
    /// </summary>
    public void closeChannel()
    {
      this.closeChannel(0U);
    }

    /// <overloads>Sets the channel low priority search timeout</overloads>
    /// <summary>
    /// Sets the search timeout for the channel's low-priority search, where it will not interrupt other open channels.
    ///             When this period expires the channel will drop to high-priority search.
    ///             This feature is not available in all ANT devices.
    /// 
    /// </summary>
    /// <param name="lowPriorityTimeout">Timeout period in 2.5 second units</param><param name="responseWaitTime">Time to wait for device success response</param>
    /// <returns>
    /// True on success. Note: Always returns true with a response time of 0
    /// </returns>
    public bool setLowPrioritySearchTimeout(byte lowPriorityTimeout, uint responseWaitTime)
    {
      if (this.disposed)
        throw new ObjectDisposedException("This ANTChannel object has been disposed");
      else
        return ANT_Channel.ANT_SetLowPriorityChannelSearchTimeout(this.unmanagedANTFramerPointer, this.channelNumber, lowPriorityTimeout, responseWaitTime) == 1;
    }

    /// <summary>
    /// Sets the timeout period for the channel's low-priority search, where it will not interrupt other open channels.
    ///             When this period expires the channel will drop to high-priority search.
    /// 
    /// </summary>
    /// <param name="lowPriorityTimeout">Timeout period in 2.5 second units</param>
    public void setLowPrioritySearchTimeout(byte lowPriorityTimeout)
    {
      this.setLowPrioritySearchTimeout(lowPriorityTimeout, 0U);
    }

    /// <overloads>Adds a channel ID to the device inclusion/exclusion list</overloads>
    /// <summary>
    /// Add the given channel ID to the channel's inclusion/exclusion list.
    ///             The channelID is then included or excluded from the wild card search depending on how the list is configured.
    ///             Throws exception if listIndex &gt; 3.
    /// 
    /// </summary>
    /// <param name="deviceNumber">deviceNumber of the channelID to add</param><param name="deviceTypeID">deviceType of the channelID to add</param><param name="transmissionTypeID">transmissionType of the channelID to add</param><param name="listIndex">position in inclusion/exclusion list to add channelID at (Max size of list is 4)</param><param name="responseWaitTime">Time to wait for device success response</param>
    /// <returns>
    /// True on success. Note: Always returns true with a response time of 0
    /// </returns>
    public bool includeExcludeList_addChannel(ushort deviceNumber, byte deviceTypeID, byte transmissionTypeID, byte listIndex, uint responseWaitTime)
    {
      if (this.disposed)
        throw new ObjectDisposedException("This ANTChannel object has been disposed");
      if ((int) listIndex > 3)
        throw new ANT_Exception("listIndex must be 0..3");
      else
        return ANT_Channel.ANT_AddChannelID(this.unmanagedANTFramerPointer, this.channelNumber, deviceNumber, deviceTypeID, transmissionTypeID, listIndex, responseWaitTime) == 1;
    }

    /// <summary>
    /// Add the given channel ID to the channel's inclusion/exclusion list.
    ///             The channelID is then included or excluded from the wild card search depending on how the list is configured.
    ///             Throws exception if listIndex &gt; 3.
    /// 
    /// </summary>
    /// <param name="deviceNumber">deviceNumber of the channelID to add</param><param name="deviceTypeID">deviceType of the channelID to add</param><param name="transmissionTypeID">transmissionType of the channelID to add</param><param name="listIndex">position in inclusion/exclusion list to add channelID at (0..3)</param>
    public void includeExcludeList_addChannel(ushort deviceNumber, byte deviceTypeID, byte transmissionTypeID, byte listIndex)
    {
      this.includeExcludeList_addChannel(deviceNumber, deviceTypeID, transmissionTypeID, listIndex, 0U);
    }

    /// <overloads>Configures the device inclusion/exclusion list</overloads>
    /// <summary>
    /// Configures the inclusion/exclusion list. If isExclusionList is true the channel IDs will be
    ///             excluded from any wild card search on this channel. Otherwise the IDs are the only IDs accepted in the search.
    ///             Throws exception if list size is greater than 4.
    /// 
    /// </summary>
    /// <param name="listSize">The desired size of the list, max size is 4, 0=none</param><param name="isExclusionList">True = exclusion list, False = inclusion list</param><param name="responseWaitTime">Time to wait for device success response</param>
    /// <returns>
    /// True on success. Note: Always returns true with a response time of 0
    /// </returns>
    public bool includeExcludeList_Configure(byte listSize, bool isExclusionList, uint responseWaitTime)
    {
      if (this.disposed)
        throw new ObjectDisposedException("This ANTChannel object has been disposed");
      if ((int) listSize > 4)
        throw new ANT_Exception("Inclusion Exclusion List has a maximum size of 4");
      else
        return ANT_Channel.ANT_ConfigList(this.unmanagedANTFramerPointer, this.channelNumber, listSize, Convert.ToByte(isExclusionList), responseWaitTime) == 1;
    }

    /// <summary>
    /// Configures the inclusion/exclusion list. If isExclusionList is true the channel IDs will be
    ///             excluded from any wild card search on this channel. Otherwise the IDs are the only IDs accepted in the search.
    ///             Throws exception if list size is greater than 4.
    /// 
    /// </summary>
    /// <param name="listSize">The desired size of the list, max size is 4, 0=none</param><param name="isExclusionList">True = exclusion list, False = inclusion list</param>
    public void includeExcludeList_Configure(byte listSize, bool isExclusionList)
    {
      this.includeExcludeList_Configure(listSize, isExclusionList, 0U);
    }

    /// <overloads>Configures proximity search</overloads>
    /// <summary>
    /// Enables a one time proximity requirement for searching.  Only ANT devices within the set proximity bin can be acquired.
    ///             Search threshold values are not correlated to specific distances as this will be dependent on the system design.
    ///             This feature is not available on all ANT devices.
    ///             Throws exception if given bin value is &gt; 10.
    /// 
    /// </summary>
    /// <param name="thresholdBin">Threshold bin. Value from 0-10 (0= disabled). A search threshold value of 1 (i.e. bin 1) will yield the smallest radius search and is generally recommended as there is less chance of connecting to the wrong device. </param><param name="responseWaitTime">Time to wait for device success response</param>
    /// <returns>
    /// True on success. Note: Always returns true with a response time of 0
    /// </returns>
    public bool setProximitySearch(byte thresholdBin, uint responseWaitTime)
    {
      if (this.disposed)
        throw new ObjectDisposedException("This ANTChannel object has been disposed");
      if ((int) thresholdBin > 10)
        throw new ANT_Exception("Threshold bin must be 0-10");
      else
        return ANT_Channel.ANT_SetProximitySearch(this.unmanagedANTFramerPointer, this.channelNumber, thresholdBin, responseWaitTime) == 1;
    }

    /// <summary>
    /// Enables a one time proximity requirement for searching.  Only ANT devices within the set proximity bin can be acquired.
    ///             Search threshold values are not correlated to specific distances as this will be dependent on the system design.
    ///             Throws exception if given bin value is &gt; 10.
    /// 
    /// </summary>
    /// <param name="thresholdBin">Threshold bin. Value from 0-10 (0= disabled). A search threshold value of 1 (i.e. bin 1) will yield the smallest radius search and is generally recommended as there is less chance of connecting to the wrong device. </param>
    public void setProximitySearch(byte thresholdBin)
    {
      this.setProximitySearch(thresholdBin, 0U);
    }

    /// <overloads>Configures the three operating RF frequencies for ANT frequency agility mode</overloads>
    /// <summary>
    /// This function configures the three operating RF frequencies for ANT frequency agility mode
    ///             and should be used with the ADV_FrequencyAgility_0x04 extended channel assignment flag.
    ///             Should not be used with shared, or Tx/Rx only channel types.
    ///             This feature is not available on all ANT devices.
    /// 
    /// </summary>
    /// <param name="freq1">Operating RF frequency 1</param><param name="freq2">Operating RF frequency 2</param><param name="freq3">Operating RF frequency 3</param><param name="responseWaitTime">Time to wait for device success response</param>
    /// <returns>
    /// True on success. Note: Always returns true with a response time of 0
    /// </returns>
    public bool configFrequencyAgility(byte freq1, byte freq2, byte freq3, uint responseWaitTime)
    {
      if (this.disposed)
        throw new ObjectDisposedException("This ANTChannel object has been disposed");
      else
        return ANT_Channel.ANT_ConfigFrequencyAgility(this.unmanagedANTFramerPointer, this.channelNumber, freq1, freq2, freq3, responseWaitTime) == 1;
    }

    /// <summary>
    /// This function configures the three operating RF frequencies for ANT frequency agility mode
    ///             and should be used with ADV_FrequencyAgility_0x04 channel assignment flag.
    ///             Should not be used with shared, or Tx/Rx only channel types.
    /// 
    /// </summary>
    /// <param name="freq1">Operating RF frequency 1</param><param name="freq2">Operating RF frequency 2</param><param name="freq3">Operating RF frequency 3</param>
    public void configFrequencyAgility(byte freq1, byte freq2, byte freq3)
    {
      this.configFrequencyAgility(freq1, freq2, freq3, 0U);
    }

    /// <overloads>Configures channel AGC</overloads>
    /// <summary>
    /// Configure channel AGC
    /// 
    /// </summary>
    /// <param name="AGCConfigByte">Configuration byte</param><param name="responseWaitTime">Time to wait for device success response</param>
    /// <returns>
    /// True on success. Note: Always returns true with a response time of 0
    /// </returns>
    public bool SetAGCConfig(byte AGCConfigByte, uint responseWaitTime)
    {
      if (this.disposed)
        throw new ObjectDisposedException("This ANTChannel object has been disposed");
      else
        return ANT_Channel.ANT_SetAGCConfig(this.unmanagedANTFramerPointer, this.channelNumber, AGCConfigByte, responseWaitTime) == 1;
    }

    /// <summary>
    /// Configure channel AGC
    /// 
    /// </summary>
    /// <param name="AGCConfigByte">Configuration byte</param>
    public void SetAGCConfig(byte AGCConfigByte)
    {
      this.SetAGCConfig(AGCConfigByte, 0U);
    }

    /// <overloads>Sets Search Waveform</overloads>
    /// <summary>
    /// Set Search Waveform
    /// 
    /// </summary>
    /// <param name="waveform">Waveform</param><param name="responseWaitTime">Time to wait for device success response</param>
    /// <returns>
    /// True on success. Note: Always returns true with a response time of 0
    /// </returns>
    public bool SetSearchWaveform(ushort waveform, uint responseWaitTime)
    {
      if (this.disposed)
        throw new ObjectDisposedException("This ANTChannel object has been disposed");
      else
        return ANT_Channel.ANT_SetSearchWaveform(this.unmanagedANTFramerPointer, this.channelNumber, waveform, responseWaitTime) == 1;
    }

    /// <summary>
    /// Set Search Waveform
    /// 
    /// </summary>
    /// <param name="waveform">Waveform</param>
    public void SetSearchWaveform(ushort waveform)
    {
      this.SetSearchWaveform(waveform, 0U);
    }
  }
}
