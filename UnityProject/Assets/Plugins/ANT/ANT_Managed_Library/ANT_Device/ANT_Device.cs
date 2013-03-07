// Type: ANT_Managed_Library.ANT_Device
// Assembly: ANT_NET, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Users\Alex\Desktop\SomaxisAfeDiagnosticMonitor_20120602_V1_0_1_1\SomaxisAfeDiagnosticMonitor_20120602_V1_0_1_1\ANT_NET.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace ANT_Managed_Library
{
  /// <summary>
  /// Control class for a given ANT device. An instance of this class is an open connection to the given ANT USB device.
  ///             Handles creating channels and device setup.
  /// 
  /// </summary>
  public class ANT_Device : IDisposable
  {
    private IntPtr unmanagedANTSerialPtr = IntPtr.Zero;
    private IntPtr unmanagedANTFramerPtr = IntPtr.Zero;
    private IntPtr unmanagedCancelBool = IntPtr.Zero;
    private byte USBDeviceNum = byte.MaxValue;
    private ANT_ReferenceLibrary.FramerType frameType;
    private ANT_ReferenceLibrary.PortType portType;
    private bool initializedUSB;
    private uint USBBaudRate;
    private ANT_DeviceCapabilities capabilities;
    private bool pollingOn;
    private int numDeviceChannels;
    private ANT_Channel[] antChannels;
    private Thread responsePoller;
    private ANT_Device.dDeviceResponseHandler m_deviceResponse;
    private ANT_Device.dSerialErrorHandler m_serialError;

    private bool? cancelTxFlag
    {
      get
      {
        if (this.unmanagedCancelBool == IntPtr.Zero)
          return new bool?();
        else
          return new bool?(Convert.ToBoolean(Marshal.ReadInt32(this.unmanagedCancelBool)));
      }
      set
      {
        if (!(this.unmanagedCancelBool != IntPtr.Zero))
          return;
        Marshal.WriteInt32(this.unmanagedCancelBool, Convert.ToInt32((object) value));
      }
    }

    /// <summary>
    /// The channel callback event. Triggered every time a message is received from the ANT device.
    ///             Examples include requested responses and setup status messages.
    /// 
    /// </summary>
    public event ANT_Device.dDeviceResponseHandler deviceResponse
    {
      [MethodImpl(MethodImplOptions.Synchronized)] add
      {
        this.m_deviceResponse = this.m_deviceResponse + value;
      }
      [MethodImpl(MethodImplOptions.Synchronized)] remove
      {
        this.m_deviceResponse = this.m_deviceResponse - value;
      }
    }

    /// <summary>
    /// This event is triggered when there is a serial communication error with the ANT Device.
    ///             If the error is critical all communication with the device is dead and the
    ///             device reference is sent in this function to allow the application
    ///             to know which device is dead and to dispose of it.
    /// 
    /// </summary>
    public event ANT_Device.dSerialErrorHandler serialError
    {
      [MethodImpl(MethodImplOptions.Synchronized)] add
      {
        this.m_serialError = this.m_serialError + value;
      }
      [MethodImpl(MethodImplOptions.Synchronized)] remove
      {
        this.m_serialError = this.m_serialError - value;
      }
    }

    /// <overloads>Opens a connection to an ANT device attached by USB.
    ///             Throws exception if a connection can not be established.
    ///             </overloads>
    /// <summary>
    /// Attempts to open a connection to an ANT device attached by USB using the given deviceNum and baud rate
    ///             Throws exception if a connection can not be established.
    /// 
    /// </summary>
    /// <param name="USBDeviceNum">The device number of the ANT USB device (the first connected device starts at 0 and so on)</param><param name="baudRate">The baud rate to connect at (AP2/AT3=57600, AP1=50000)</param>
    public ANT_Device(byte USBDeviceNum, uint baudRate)
      : this(ANT_ReferenceLibrary.PortType.USB, USBDeviceNum, baudRate, ANT_ReferenceLibrary.FramerType.basicANT)
    {
    }

    /// <overloads>Opens a connection to an ANT device attached by USB.
    ///             Throws exception if a connection can not be established.
    ///             </overloads>
    /// <summary>
    /// Attempts to open a connection to an ANT device attached by USB using the given deviceNum and baud rate
    ///             Throws exception if a connection can not be established.
    /// 
    /// </summary>
    /// <param name="portType">The type of connection to use when talking to the device</param><param name="USBDeviceNum">If port type is USB, device number of the ANT USB device.
    ///             If port type is COM this is the COM port number</param><param name="baudRate">The baud rate to connect at (USB: AP2/AT3=57600, AP1=50000)</param><param name="frameType">The framing method to use for the connection to the chip.
    ///             Needed for multimode chips and currently only supported on COM connections.</param>
    public ANT_Device(ANT_ReferenceLibrary.PortType portType, byte USBDeviceNum, uint baudRate, ANT_ReferenceLibrary.FramerType frameType)
    {
      try
      {
        ANT_Common.checkUnmanagedLibrary();
        this.startUp(USBDeviceNum, baudRate, frameType, portType, false);
      }
      catch
      {
        this.Dispose();
        throw;
      }
    }

    /// <summary>
    /// Automatically connects to first availiable ANTDevice.
    ///             Throws exception if a connection can not be established.
    ///             Will not auto find COM-connected devices.
    /// 
    /// </summary>
    public ANT_Device()
    {
      ANT_Common.checkUnmanagedLibrary();
      ulong num = (ulong) ANT_Common.getNumDetectedUSBDevices();
      if ((long) num == 0L)
      {
        string str = ": ensure an ANT device is connected to your system and try again";
        try
        {
          ANT_Common.checkUSBLibraries();
        }
        catch (ANT_Exception ex)
        {
          str = ": " + ex.Message.Remove(0, 21);
        }
        throw new ANT_Exception("No ANT devices detected" + str);
      }
      else
      {
        bool flag = true;
        for (byte USBDeviceNum = (byte) 0; (ulong) USBDeviceNum < num; ++USBDeviceNum)
        {
          if (flag)
          {
            try
            {
              this.startUp(USBDeviceNum, 57600U, ANT_ReferenceLibrary.FramerType.basicANT, ANT_ReferenceLibrary.PortType.USB, true);
              flag = false;
            }
            catch (Exception ex1)
            {
              try
              {
                this.startUp(USBDeviceNum, 50000U, ANT_ReferenceLibrary.FramerType.basicANT, ANT_ReferenceLibrary.PortType.USB, true);
                flag = false;
              }
              catch (Exception ex2)
              {
                flag = true;
              }
            }
          }
          else
            break;
        }
        if (!flag)
          return;
        this.Dispose();
        throw new ANT_Exception("Failed to connect to any ANT devices");
      }
    }

    /// <summary>
    /// Destructor closes all opened resources
    /// 
    /// </summary>
    ~ANT_Device()
    {
      this.shutdown();
    }

    [DllImport("ANT_WrappedLib", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_Init(byte ucUSBDeviceNum, uint usBaudrate, ref IntPtr returnSerialPtr, ref IntPtr returnFramerPtr, byte ucPortType, byte ucHCIFrameTpye);

    [DllImport("ANT_WrappedLib", CallingConvention = CallingConvention.Cdecl)]
    private extern static void ANT_Close(IntPtr SerialPtr, IntPtr FramerPtr);

    [DllImport("ANT_WrappedLib", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_USBReset(IntPtr SerialPtr);

    [DllImport("ANT_WrappedLib", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_ResetSystem(IntPtr FramerPtr, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib", CallingConvention = CallingConvention.Cdecl)]
    private extern static void ANT_SetCancelParameter(IntPtr FramerPtr, IntPtr pbCancel);

    [DllImport("ANT_WrappedLib", EntryPoint = "ANT_SetNetworkKey_RTO", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_SetNetworkKey(IntPtr FramerPtr, byte ucNetNumber, byte[] pucKey, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib", EntryPoint = "ANT_EnableLED_RTO", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_EnableLED(IntPtr FramerPtr, byte ucEnable, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib", EntryPoint = "ANT_SetUSBDescriptorString_RTO", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_SetUSBDescriptorString(IntPtr FramerPtr, byte ucStringNumber, byte[] pucDescString, byte ucStringSize, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_GetDeviceUSBPID(IntPtr FramerPtr, ref ushort usbPID);

    [DllImport("ANT_WrappedLib", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_GetDeviceUSBVID(IntPtr FramerPtr, ref ushort usbVID);

    [DllImport("ANT_WrappedLib", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_ConfigureSplitAdvancedBursts(IntPtr FramerPtr, int bEnabelSplitBursts);

    [DllImport("ANT_WrappedLib", EntryPoint = "ANT_ConfigureAdvancedBurst_RTO", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_ConfigureAdvancedBurst(IntPtr FramerPtr, int enable, byte ucMaxPacketLength, uint ulRequiredFields, uint ulOptionalFields, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib", EntryPoint = "ANT_ConfigureAdvancedBurst_ext_RTO", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_ConfigureAdvancedBurst_ext(IntPtr FramerPtr, int enable, byte ucMaxPacketLength, uint ulRequiredFields, uint ulOptionalFields, ushort usStallCount, byte ucRetryCount, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib", CallingConvention = CallingConvention.Cdecl)]
    private extern static uint ANT_GetDeviceSerialNumber(IntPtr SerialPtr);

    [DllImport("ANT_WrappedLib", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_GetDeviceUSBInfo(IntPtr FramerPtr, byte ucDeviceNum_, [MarshalAs(UnmanagedType.LPArray)] byte[] pucProductString_, [MarshalAs(UnmanagedType.LPArray)] byte[] pucSerialString_);

    [DllImport("ANT_WrappedLib", EntryPoint = "ANT_SetTransmitPower_RTO", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_SetTransmitPower(IntPtr FramerPtr, byte ucTransmitPower_, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_RequestMessage(IntPtr FramerPtr, byte ucANTChannel, byte ucMessageID, ref ANT_Device.ANTMessageItem ANT_MESSAGE_ITEM_response, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib", CallingConvention = CallingConvention.Cdecl)]
    private extern static ushort ANT_WaitForMessage(IntPtr FramerPtr, uint ulMilliseconds_);

    [DllImport("ANT_WrappedLib", CallingConvention = CallingConvention.Cdecl)]
    private extern static ushort ANT_GetMessage(IntPtr FramerPtr, ref ANT_Device.ANTMessage ANT_MESSAGE_response);

    [DllImport("ANT_WrappedLib", CallingConvention = CallingConvention.Cdecl)]
    private extern static byte ANT_GetChannelNumber(IntPtr FramerPtr, ref ANT_Device.ANTMessage pstANTMessage);

    [DllImport("ANT_WrappedLib", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_GetCapabilities(IntPtr FramerPtr, [MarshalAs(UnmanagedType.LPArray)] byte[] pucCapabilities_, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib", EntryPoint = "ANT_InitCWTestMode_RTO", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_InitCWTestMode(IntPtr FramerPtr, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib", EntryPoint = "ANT_SetCWTestMode_RTO", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_SetCWTestMode(IntPtr FramerPtr, byte ucTransmitPower_, byte ucRFChannel_, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib", EntryPoint = "ANT_OpenRxScanMode_RTO", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_OpenRxScanMode(IntPtr FramerPtr, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib", EntryPoint = "ANT_Script_Write_RTO", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_Script_Write(IntPtr FramerPtr, byte ucSize_, byte[] pucData_, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib", EntryPoint = "ANT_Script_Clear_RTO", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_Script_Clear(IntPtr FramerPtr, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib", EntryPoint = "ANT_Script_SetDefaultSector_RTO", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_Script_SetDefaultSector(IntPtr FramerPtr, byte ucSectNumber_, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib", EntryPoint = "ANT_Script_EndSector_RTO", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_Script_EndSector(IntPtr FramerPtr, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib", EntryPoint = "ANT_Script_Dump_RTO", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_Script_Dump(IntPtr FramerPtr, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib", EntryPoint = "ANT_Script_Lock_RTO", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_Script_Lock(IntPtr FramerPtr, uint ulResponseTimeout_);

    [DllImport("ANT_WrappedLib", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_RunScript(IntPtr FramerPtr, byte ucScriptNum_, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib", EntryPoint = "ANT_RxExtMesgsEnable_RTO", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_RxExtMesgsEnable(IntPtr FramerPtr, byte ucEnable_, uint ulResponseTimeout_);

    [DllImport("ANT_WrappedLib", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_CrystalEnable(IntPtr FramerPtr, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_WriteMessage(IntPtr FramerPtr, ANT_Device.ANTMessage pstANTMessage, ushort usMessageSize);

    [DllImport("ANT_WrappedLib", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_SetLibConfig(IntPtr FramerPtr, byte ucLibConfigFlags_, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ANT_UnlockInterface(IntPtr FramerPtr, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib", EntryPoint = "FIT_SetFEState_RTO", CallingConvention = CallingConvention.Cdecl)]
    private extern static int FIT_SetFEState(IntPtr FramerPtr, byte ucFEState_, uint ulResponseTime_);

    [DllImport("ANT_WrappedLib", EntryPoint = "FIT_AdjustPairingSettings_RTO", CallingConvention = CallingConvention.Cdecl)]
    private extern static int FIT_AdjustPairingSettings(IntPtr FramerPtr, byte ucSearchLv_, byte ucPairLv_, byte ucTrackLv_, uint ulResponseTime_);

    private void startUp(byte USBDeviceNum, uint baudRate, ANT_ReferenceLibrary.FramerType frameType, ANT_ReferenceLibrary.PortType portType, bool calledByAutoInit)
    {
      switch (ANT_Device.ANT_Init(USBDeviceNum, baudRate, ref this.unmanagedANTSerialPtr, ref this.unmanagedANTFramerPtr, (byte) portType, (byte) frameType))
      {
        case 0:
          this.initializedUSB = true;
          this.USBDeviceNum = USBDeviceNum;
          this.USBBaudRate = baudRate;
          this.frameType = frameType;
          this.portType = portType;
          ANT_Common.initDebugLogThread("Device" + (object) USBDeviceNum + "_Application");
          ANT_Common.ANT_DebugResetTime();
          ANT_Common.writeToDebugLog("ANT_NET.DLL " + ANT_VersionInfo.getManagedLibraryVersion() + " with ANT_WrappedLib.DLL " + ANT_VersionInfo.getUnmanagedLibraryVersion());
          try
          {
            this.responsePoller = new Thread(new ThreadStart(this.responsePollFunc));
            this.responsePoller.Name = this.ToString() + " Receive Thread";
            this.responsePoller.IsBackground = true;
            this.responsePoller.Start();
            try
            {
              this.getDeviceCapabilities(true, 200U);
            }
            catch (ANT_Exception ex)
            {
              throw new ANT_Exception(ex.Message.Remove(0, 22) + ", probably connecting at wrong baud rate");
            }
            if (ANT_Common.autoResetIsEnabled)
              ANT_Device.ANT_ResetSystem(this.unmanagedANTFramerPtr, 200U);
            this.numDeviceChannels = (int) this.capabilities.maxANTChannels;
            this.antChannels = new ANT_Channel[this.numDeviceChannels];
            this.unmanagedCancelBool = Marshal.AllocHGlobal(4);
            this.cancelTxFlag = new bool?(false);
            ANT_Device.ANT_SetCancelParameter(this.unmanagedANTFramerPtr, this.unmanagedCancelBool);
            break;
          }
          catch (Exception ex)
          {
            this.shutdown();
            throw ex;
          }
        case -3:
          if (!calledByAutoInit)
            ANT_Common.checkUSBLibraries();
          throw new ANT_Exception("Unable to initialize USB:" + (object) USBDeviceNum + " at Baud:" + (string) (object) baudRate + ", probably device not present or already in use, or drivers not installed");
        case -2:
          throw new ANT_Exception("Unrecognized type parameters");
        case -1:
          throw new ANT_Exception("Unexpected init library error. This is typically a problem with the c++ library");
        default:
          throw new ANT_Exception("Unrecognized error code received from c++ library");
      }
    }

    /// <summary>
    /// Dispose method for explicit resource cleanup. Same as shutdownDeviceInstance() but doesn't nullify reference.
    /// 
    /// </summary>
    public void Dispose()
    {
      this.shutdown();
      GC.SuppressFinalize((object) this);
    }

    private void shutdown()
    {
      try
      {
        lock (this)
        {
          if (!this.initializedUSB)
            return;
          if (this.antChannels != null)
          {
            this.NotifyDeviceEvent(ANT_Device.DeviceNotificationCode.Shutdown, (object) null);
            foreach (ANT_Channel item_0 in this.antChannels)
            {
              if (item_0 != null)
                item_0.Dispose();
            }
          }
          this.cancelTxFlag = new bool?(true);
          this.pollingOn = false;
          if (!this.responsePoller.Join(1500))
            this.responsePoller.Abort();
          if (this.capabilities != null && ANT_Common.autoResetIsEnabled)
            ANT_Device.ANT_ResetSystem(this.unmanagedANTFramerPtr, 0U);
          ANT_Device.ANT_Close(this.unmanagedANTSerialPtr, this.unmanagedANTFramerPtr);
          if (this.unmanagedCancelBool != IntPtr.Zero)
          {
            Marshal.FreeHGlobal(this.unmanagedCancelBool);
            this.unmanagedCancelBool = IntPtr.Zero;
          }
          this.initializedUSB = false;
        }
      }
      catch (Exception ex)
      {
      }
    }

    /// <summary>
    /// Shuts down all open resources and nullifies the given ANTDevice and all its channels
    /// 
    /// </summary>
    /// <param name="deviceToShutdown">ANTDevice to shutdown</param>
    public static void shutdownDeviceInstance(ref ANT_Device deviceToShutdown)
    {
      if (deviceToShutdown == null)
        return;
      deviceToShutdown.Dispose();
      deviceToShutdown = (ANT_Device) null;
    }

    internal void channelDisposed(byte channelNumber)
    {
      this.antChannels[(int) channelNumber] = (ANT_Channel) null;
    }

    /// <summary>
    /// Convert instance to a string including the USB device number the connection is on
    /// 
    /// </summary>
    public override string ToString()
    {
      return base.ToString() + " on USBdeviceNum: " + this.USBDeviceNum.ToString();
    }

    /// <summary>
    /// Returns the pointer to the underlying C++ ANT Framer used for messaging
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// Pointer to C++ ANT Framer
    /// </returns>
    internal IntPtr getFramerPtr()
    {
      if (!this.initializedUSB)
        throw new ObjectDisposedException("ANTDevice object has been disposed");
      else
        return this.unmanagedANTFramerPtr;
    }

    /// <summary>
    /// Returns the device number used when this instance was opened
    ///             Note: For some device types this number is not static and can change whenever new devices are enumerated in the system
    /// 
    /// </summary>
    public int getOpenedUSBDeviceNum()
    {
      return (int) this.USBDeviceNum;
    }

    /// <summary>
    /// Returns the baud rate used when this instance was opened
    /// 
    /// </summary>
    public uint getOpenedUSBBaudRate()
    {
      return this.USBBaudRate;
    }

    /// <summary>
    /// Returns the Frame Type used to open the device
    /// 
    /// </summary>
    public ANT_ReferenceLibrary.FramerType getOpenedFrameType()
    {
      return this.frameType;
    }

    /// <summary>
    /// Returns the Port Type used to open the device
    /// 
    /// </summary>
    public ANT_ReferenceLibrary.PortType getOpenedPortType()
    {
      return this.portType;
    }

    /// <summary>
    /// Returns the requested ANTChannel or throws an exception if channel doesn't exist.
    /// 
    /// </summary>
    /// <param name="num">Channel number requested</param>
    public ANT_Channel getChannel(int num)
    {
      if (!this.initializedUSB)
        throw new ObjectDisposedException("ANTDevice object has been disposed");
      if (num > this.antChannels.Length - 1 || num < 0)
        throw new ANT_Exception("Channel number invalid");
      if (this.antChannels[num] == null)
        this.antChannels[num] = new ANT_Channel(this, (byte) num);
      return this.antChannels[num];
    }

    /// <summary>
    /// Returns the number of ANTChannels owned by this device
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// Number of ant channels on device
    /// </returns>
    public int getNumChannels()
    {
      if (!this.initializedUSB)
        throw new ObjectDisposedException("ANTDevice object has been disposed");
      else
        return this.antChannels.Length;
    }

    private void NotifyDeviceEvent(ANT_Device.DeviceNotificationCode notification, object notificationInfo)
    {
      foreach (ANT_Channel antChannel in this.antChannels)
      {
        if (antChannel != null)
          antChannel.NotifyDeviceEvent(notification, notificationInfo);
      }
    }

    private void responsePollFunc()
    {
      this.pollingOn = true;
      ANT_Common.initDebugLogThread("Device" + (object) this.USBDeviceNum + "_ANTReceive");
      while (this.initializedUSB && this.pollingOn)
      {
        if ((int) ANT_Device.ANT_WaitForMessage(this.unmanagedANTFramerPtr, 100U) != 65534)
        {
          ANT_Device.ANTMessage ANT_MESSAGE_response = new ANT_Device.ANTMessage();
          ushort message = ANT_Device.ANT_GetMessage(this.unmanagedANTFramerPtr, ref ANT_MESSAGE_response);
          if ((int) message == (int) ushort.MaxValue)
          {
            bool isCritical = false;
            ANT_Device.serialErrorCode error;
            switch (ANT_MESSAGE_response.msgID)
            {
              case (byte) 1:
                error = ANT_Device.serialErrorCode.MessageLost_QueueOverflow;
                break;
              case (byte) 2:
                switch (ANT_MESSAGE_response.ucharBuf[0])
                {
                  case (byte) 1:
                    error = ANT_Device.serialErrorCode.DeviceConnectionLost;
                    isCritical = true;
                    break;
                  case (byte) 2:
                    error = ANT_Device.serialErrorCode.SerialWriteError;
                    break;
                  case (byte) 3:
                    error = ANT_Device.serialErrorCode.SerialReadError;
                    isCritical = true;
                    break;
                  case (byte) 4:
                    error = ANT_Device.serialErrorCode.MessageLost_CrcError;
                    break;
                  default:
                    error = ANT_Device.serialErrorCode.Unknown;
                    break;
                }
                break;
              case (byte) 3:
                error = ANT_Device.serialErrorCode.MessageLost_TooLarge;
                break;
              default:
                error = ANT_Device.serialErrorCode.Unknown;
                break;
            }
            if (isCritical)
              this.pollingOn = false;
            if (this.m_serialError != null)
              this.m_serialError(this, error, isCritical);
            if (isCritical)
              break;
          }
          else
          {
            bool flag = false;
            byte num1 = ANT_MESSAGE_response.msgID;
            if ((uint) num1 <= 80U)
            {
              switch (num1)
              {
                case (byte) 64:
                  if ((int) ANT_MESSAGE_response.ucharBuf[1] == 1)
                  {
                    flag = true;
                    goto label_26;
                  }
                  else
                    goto label_26;
                case (byte) 78:
                case (byte) 79:
                case (byte) 80:
                  break;
                default:
                  goto label_26;
              }
            }
            else
            {
              switch (num1)
              {
                case (byte) 93:
                case (byte) 94:
                case (byte) 95:
                case (byte) 114:
                  break;
                default:
                  goto label_26;
              }
            }
            flag = true;
label_26:
            if (flag)
            {
              byte num2 = (byte) ((uint) ANT_MESSAGE_response.ucharBuf[0] & 31U);
              if (this.antChannels != null && (int) num2 < this.antChannels.Length)
              {
                if (this.antChannels[(int) num2] != null)
                  this.antChannels[(int) num2].MessageReceived(ANT_MESSAGE_response, message);
              }
              else if (this.m_serialError != null)
                this.m_serialError(this, ANT_Device.serialErrorCode.MessageLost_InvalidChannel, false);
            }
            else
            {
              ANT_Response response = new ANT_Response((object) this, ANT_MESSAGE_response.ucharBuf[0], DateTime.Now, ANT_MESSAGE_response.msgID, Enumerable.ToArray<byte>(Enumerable.Take<byte>((IEnumerable<byte>) ANT_MESSAGE_response.ucharBuf, (int) message)));
              if (this.m_deviceResponse != null)
                this.m_deviceResponse(response);
            }
          }
        }
      }
    }

    /// <summary>
    /// Sets the cancel flag on all acknowledged and burst transfers in progress for the given amount of time.
    ///             When these transmissions see the flag they will abort their operation and return as cancelled.
    /// 
    /// </summary>
    /// <param name="cancelWaitTime">Time to set cancel flag for</param>
    public void cancelTransfers(int cancelWaitTime)
    {
      if (!this.initializedUSB)
        throw new ObjectDisposedException("ANTDevice object has been disposed");
      this.cancelTxFlag = new bool?(true);
      Thread.Sleep(cancelWaitTime);
      this.cancelTxFlag = new bool?(false);
    }

    /// <overloads>Returns the device capabilities</overloads>
    /// <summary>
    /// Returns the capabilities of this device.
    ///             Throws an exception if capabilities are not received.
    /// 
    /// </summary>
    /// <param name="forceNewCopy">Force function to send request message to device</param><param name="responseWaitTime">Time to wait for device success response</param>
    /// <returns>
    /// Last device capabilities received or a new copy if forceNewCopy is true
    /// </returns>
    public ANT_DeviceCapabilities getDeviceCapabilities(bool forceNewCopy, uint responseWaitTime)
    {
      if (!this.initializedUSB)
        throw new ObjectDisposedException("ANTDevice object has been disposed");
      if (!forceNewCopy)
      {
        if (this.capabilities != null)
          goto label_8;
      }
      try
      {
        ANT_DeviceCapabilities deviceCapabilities = (ANT_DeviceCapabilities) null;
        ANT_Response antResponse = this.requestMessageAndResponse(ANT_ReferenceLibrary.RequestMessageID.CAPABILITIES_0x54, responseWaitTime);
        if (antResponse != null && (int) antResponse.responseID == 84)
        {
          byte[] numArray = new byte[16 - antResponse.messageContents.Length];
          deviceCapabilities = new ANT_DeviceCapabilities(Enumerable.ToArray<byte>(Enumerable.Concat<byte>((IEnumerable<byte>) antResponse.messageContents, (IEnumerable<byte>) numArray)));
        }
        this.capabilities = deviceCapabilities;
      }
      catch (Exception ex)
      {
        throw new ANT_Exception("Retrieving Device Capabilities Failed");
      }
label_8:
      return this.capabilities;
    }

    /// <summary>
    /// Returns the device capabilities of this device.
    ///             Throws an exception if capabilities are not received.
    /// 
    /// </summary>
    public ANT_DeviceCapabilities getDeviceCapabilities()
    {
      return this.getDeviceCapabilities(false, 1500U);
    }

    /// <summary>
    /// Returns the device capabilities of this device.
    ///             Throws an exception if capabilities are not received.
    /// 
    /// </summary>
    public ANT_DeviceCapabilities getDeviceCapabilities(uint responseWaitTime)
    {
      return this.getDeviceCapabilities(false, responseWaitTime);
    }

    /// <overloads>Resets the USB device</overloads>
    /// <summary>
    /// Resets this USB device at the driver level
    /// 
    /// </summary>
    public void ResetUSB()
    {
      if (this.portType != ANT_ReferenceLibrary.PortType.USB)
        throw new ANT_Exception("Can't call ResetUSB on non-USB devices");
      if (!this.initializedUSB)
        throw new ObjectDisposedException("ANTDevice object has been disposed");
      ANT_Device.ANT_USBReset(this.unmanagedANTSerialPtr);
    }

    /// <overloads>Resets the device and all its channels</overloads>
    /// <summary>
    /// Reset this device and all associated channels
    /// 
    /// </summary>
    /// <param name="responseWaitTime">Time to wait for device success response</param>
    /// <returns>
    /// True on success. Note: Always returns true with a response time of 0
    /// </returns>
    public bool ResetSystem(uint responseWaitTime)
    {
      if (!this.initializedUSB)
        throw new ObjectDisposedException("ANTDevice object has been disposed");
      this.NotifyDeviceEvent(ANT_Device.DeviceNotificationCode.Reset, (object) null);
      return ANT_Device.ANT_ResetSystem(this.unmanagedANTFramerPtr, responseWaitTime) == 1;
    }

    /// <summary>
    /// Reset this device and all associated channels
    /// 
    /// </summary>
    public void ResetSystem()
    {
      this.ResetSystem(500U);
    }

    /// <overloads>Sets a network key</overloads>
    /// <summary>
    /// Set the network key for the given network
    ///             Throws exception if net number is invalid or network key is not 8-bytes in length
    /// 
    /// </summary>
    /// <param name="netNumber">The network number to set the key for</param><param name="networkKey">The 8-byte network key</param><param name="responseWaitTime">Time to wait for device success response</param>
    /// <returns>
    /// True on success. Note: Always returns true with a response time of 0
    /// </returns>
    public bool setNetworkKey(byte netNumber, byte[] networkKey, uint responseWaitTime)
    {
      if (!this.initializedUSB)
        throw new ObjectDisposedException("ANTDevice object has been disposed");
      if (this.capabilities != null && (int) netNumber >= (int) this.capabilities.maxNetworks)
        throw new ANT_Exception("Network number must be less than the maximum capable networks of the device");
      if (networkKey.Length != 8)
        throw new ANT_Exception("Network key must be 8 bytes");
      else
        return ANT_Device.ANT_SetNetworkKey(this.unmanagedANTFramerPtr, netNumber, networkKey, responseWaitTime) == 1;
    }

    /// <summary>
    /// Set the network key for the given network
    ///             Throws exception if net number is invalid or network key is not 8-bytes in length
    /// 
    /// </summary>
    /// <param name="netNumber">The network number to set the key for</param><param name="networkKey">The 8-byte network key</param>
    public void setNetworkKey(byte netNumber, byte[] networkKey)
    {
      this.setNetworkKey(netNumber, networkKey, 0U);
    }

    /// <overloads>Sets the transmit power for all channels</overloads>
    /// <summary>
    /// Set the transmit power for all channels of this device
    /// 
    /// </summary>
    /// <param name="transmitPower">Transmission power to set to</param><param name="responseWaitTime">Time to wait for device success response</param>
    /// <returns>
    /// True on success. Note: Always returns true with a response time of 0
    /// </returns>
    public bool setTransmitPowerForAllChannels(ANT_ReferenceLibrary.TransmitPower transmitPower, uint responseWaitTime)
    {
      if (!this.initializedUSB)
        throw new ObjectDisposedException("ANTDevice object has been disposed");
      else
        return ANT_Device.ANT_SetTransmitPower(this.unmanagedANTFramerPtr, (byte) transmitPower, responseWaitTime) == 1;
    }

    /// <summary>
    /// Set the transmit power for all channels of this device
    /// 
    /// </summary>
    /// <param name="transmitPower">Transmission power to set to</param>
    public void setTransmitPowerForAllChannels(ANT_ReferenceLibrary.TransmitPower transmitPower)
    {
      this.setTransmitPowerForAllChannels(transmitPower, 0U);
    }

    /// <summary>
    /// When enabled advanced burst messages will be split into standard burst packets when received.
    ///             This is disabled by default.
    /// 
    /// </summary>
    /// <param name="splitBursts">Whether to split advanced burst messages.</param>
    /// <returns>
    /// True on success.
    /// </returns>
    public bool configureAdvancedBurstSplitting(bool splitBursts)
    {
      if (!this.initializedUSB)
        throw new ObjectDisposedException("ANTDevice object has been disposed");
      else
        return ANT_Device.ANT_ConfigureSplitAdvancedBursts(this.unmanagedANTFramerPtr, splitBursts ? 1 : 0) == 1;
    }

    /// <summary>
    /// Configure advanced bursting for this device.
    /// 
    /// </summary>
    /// <param name="enable">Whether to enable advanced bursting messages</param><param name="maxPacketLength">Maximum packet length allowed for bursting messages (valid values are 1-3)</param><param name="requiredFields">Features that the application requires the device to use</param><param name="optionalFields">Features that the device should use if it supports them</param><param name="responseWaitTime">Time to wait for device success response</param>
    /// <returns>
    /// True on success. Note: Always returns true with a response time of 0
    /// </returns>
    public bool configureAdvancedBursting(bool enable, byte maxPacketLength, ANT_ReferenceLibrary.AdcancedBurstConfigFlags requiredFields, ANT_ReferenceLibrary.AdcancedBurstConfigFlags optionalFields, uint responseWaitTime)
    {
      if (!this.initializedUSB)
        throw new ObjectDisposedException("ANTDevice object has been disposed");
      else
        return ANT_Device.ANT_ConfigureAdvancedBurst(this.unmanagedANTFramerPtr, enable ? 1 : 0, maxPacketLength, (uint) requiredFields, (uint) optionalFields, responseWaitTime) == 1;
    }

    /// <summary>
    /// Configure advanced bursting for this device.
    /// 
    /// </summary>
    /// <param name="enable">Whether to enable advanced bursting messages</param><param name="maxPacketLength">Maximum packet length allowed for bursting messages (valid values are 1-3)</param><param name="requiredFields">Features that the application requires the device to use</param><param name="optionalFields">Features that the device should use if it supports them</param>
    public void configureAdvancedBursting(bool enable, byte maxPacketLength, ANT_ReferenceLibrary.AdcancedBurstConfigFlags requiredFields, ANT_ReferenceLibrary.AdcancedBurstConfigFlags optionalFields)
    {
      this.configureAdvancedBursting(enable, maxPacketLength, requiredFields, optionalFields, 0U);
    }

    /// <summary>
    /// Configure advanced bursting for this device including extended parameters.
    /// 
    /// </summary>
    /// <param name="enable">Whether to enable advanced bursting messages</param><param name="maxPacketLength">Maximum packet length allowed for bursting messages (valid values are 1-3)</param><param name="requiredFields">Features that the application requires the device to use</param><param name="optionalFields">Features that the device should use if it supports them</param><param name="stallCount">Maximum number of burst periods (~3.1ms) to stall for while waiting for the next message</param><param name="retryCount">Number of times (multiplied by 5) to retry burst</param><param name="responseWaitTime">Time to wait for device success response</param>
    /// <returns>
    /// True on success. Note: Always returns true with a response time of 0
    /// </returns>
    public bool configureAdvancedBursting_ext(bool enable, byte maxPacketLength, ANT_ReferenceLibrary.AdcancedBurstConfigFlags requiredFields, ANT_ReferenceLibrary.AdcancedBurstConfigFlags optionalFields, ushort stallCount, byte retryCount, uint responseWaitTime)
    {
      if (!this.initializedUSB)
        throw new ObjectDisposedException("ANTDevice object has been disposed");
      else
        return ANT_Device.ANT_ConfigureAdvancedBurst_ext(this.unmanagedANTFramerPtr, enable ? 1 : 0, maxPacketLength, (uint) requiredFields, (uint) optionalFields, stallCount, retryCount, responseWaitTime) == 1;
    }

    /// <summary>
    /// Configure advanced bursting for this device including extended parameters.
    /// 
    /// </summary>
    /// <param name="enable">Whether to enable advanced bursting messages</param><param name="maxPacketLength">Maximum packet length allowed for bursting messages (valid values are 1-3)</param><param name="requiredFields">Features that the application requires the device to use</param><param name="optionalFields">Features that the device should use if it supports them</param><param name="stallCount">Maximum number of burst periods (~3.1ms) to stall for while waiting for the next message</param><param name="retryCount">Number of times (multiplied by 5) to retry burst</param>
    public void configureAdvancedBursting_ext(bool enable, byte maxPacketLength, ANT_ReferenceLibrary.AdcancedBurstConfigFlags requiredFields, ANT_ReferenceLibrary.AdcancedBurstConfigFlags optionalFields, ushort stallCount, byte retryCount)
    {
      this.configureAdvancedBursting_ext(enable, maxPacketLength, requiredFields, optionalFields, stallCount, retryCount, 0U);
    }

    /// <overloads>Enables/Disables the device's LED</overloads>
    /// <summary>
    /// Enables/Disables the LED flashing when a transmission event occurs
    /// 
    /// </summary>
    /// <param name="IsEnabled">Desired state</param><param name="responseWaitTime">Time to wait for device success response</param>
    /// <returns>
    /// True on success. Note: Always returns true with a response time of 0
    /// </returns>
    public bool EnableLED(bool IsEnabled, uint responseWaitTime)
    {
      if (!this.initializedUSB)
        throw new ObjectDisposedException("ANTDevice object has been disposed");
      else
        return ANT_Device.ANT_EnableLED(this.unmanagedANTFramerPtr, Convert.ToByte(IsEnabled), responseWaitTime) == 1;
    }

    /// <summary>
    /// Enables/Disables the LED flashing when a transmission event occurs
    /// 
    /// </summary>
    /// <param name="IsEnabled">Desired state</param>
    public void EnableLED(bool IsEnabled)
    {
      this.EnableLED(IsEnabled, 0U);
    }

    /// <summary>
    /// Obtains the PID (Product ID) of the USB device.
    ///             Throws an exception if the PID is not received.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// PID of the USB device.
    /// </returns>
    public ushort getDeviceUSBPID()
    {
      if (!this.initializedUSB)
        throw new ObjectDisposedException("ANTDevice object has been disposed");
      ushort usbPID = (ushort) 0;
      if (ANT_Device.ANT_GetDeviceUSBPID(this.unmanagedANTFramerPtr, ref usbPID) != 1)
        throw new ANT_Exception("Retrieving Device USB PID failed");
      else
        return usbPID;
    }

    /// <summary>
    /// Obtains the VID (Vendor ID) of the USB device
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// VID of the USB device
    /// </returns>
    public ushort getDeviceUSBVID()
    {
      if (!this.initializedUSB)
        throw new ObjectDisposedException("ANTDevice object has been disposed");
      ushort usbVID = (ushort) 0;
      if (ANT_Device.ANT_GetDeviceUSBVID(this.unmanagedANTFramerPtr, ref usbVID) != 1)
        throw new ANT_Exception("Retrieving Device USB VID failed");
      else
        return usbVID;
    }

    /// <summary>
    /// Returns the USB device serial number.
    ///             This can be used to figure out the serial number if the option to use the USB device
    ///             serial number was selected.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// Client serial number
    /// </returns>
    public uint getSerialNumber()
    {
      if (!this.initializedUSB)
        throw new ObjectDisposedException("ANTDevice object has been disposed");
      else
        return ANT_Device.ANT_GetDeviceSerialNumber(this.unmanagedANTSerialPtr);
    }

    /// <overloads>Obtains the device USB Information</overloads>
    /// <summary>
    /// Obtains the USB information for the device
    ///             Throws an exception if no information is received
    /// 
    /// </summary>
    /// <param name="deviceNum">USB Device Number</param>
    /// <returns>
    /// USB Device Information
    /// </returns>
    public ANT_DeviceInfo getDeviceUSBInfo(byte deviceNum)
    {
      if (!this.initializedUSB)
        throw new ObjectDisposedException("ANTDevice object has been disposed");
      byte[] numArray1 = new byte[256];
      byte[] numArray2 = new byte[256];
      if (ANT_Device.ANT_GetDeviceUSBInfo(this.unmanagedANTFramerPtr, deviceNum, numArray1, numArray2) != 1)
        throw new ANT_Exception("Retrieving USB device information failed");
      else
        return new ANT_DeviceInfo(numArray1, numArray2);
    }

    /// <summary>
    /// Obtains the USB information for the device
    ///             Throws an exception if no information is received
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// USB Device Information
    /// </returns>
    public ANT_DeviceInfo getDeviceUSBInfo()
    {
      if (!this.initializedUSB)
        throw new ObjectDisposedException("ANTDevice object has been disposed");
      else
        return this.getDeviceUSBInfo(this.USBDeviceNum);
    }

    /// <overloads>Configure USB descriptor strings
    ///             IMPORTANT: The AP2-USB does not support re-writeable flash memory, and once a descriptor has
    ///             been set three times, it cannot be changed.  Use this command with caution.
    ///             </overloads>
    /// <summary>
    /// Configure USB descriptor strings.
    /// 
    /// </summary>
    /// <param name="stringNum">Descriptor string number</param><param name="descriptorString">Descriptor string</param><param name="stringSize">String length (max 32).  If a longer string is provided, it will be truncated.</param><param name="responseWaitTime">Time to wait for device success response</param>
    /// <returns>
    /// True on success. Note: Always returns true with a response time of 0
    /// </returns>
    public bool setUSBDescriptorString(byte stringNum, byte[] descriptorString, byte stringSize, uint responseWaitTime)
    {
      if (!this.initializedUSB)
        throw new ObjectDisposedException("ANTDevice object has been disposed");
      else
        return ANT_Device.ANT_SetUSBDescriptorString(this.unmanagedANTFramerPtr, stringNum, descriptorString, stringSize, responseWaitTime) == 1;
    }

    /// <summary>
    /// Configure USB descriptor strings
    /// 
    /// </summary>
    /// <param name="stringNum">Descriptor string number</param><param name="descriptorString">Descriptor string</param><param name="stringSize">String length (max 32).  If a longer string is provided, it will be truncated.</param>
    public void setUSBDescriptorString(byte stringNum, byte[] descriptorString, byte stringSize)
    {
      this.setUSBDescriptorString(stringNum, descriptorString, stringSize, 0U);
    }

    /// <summary>
    /// Configure USB descriptor strings
    /// 
    /// </summary>
    /// <param name="stringNum">Descriptor string number</param><param name="descriptorString">Descriptor string (max length 32).  If a longer string is provided, it will be truncated</param><param name="responseWaitTime">Time to wait for device success response</param>
    /// <returns>
    /// True on success. Note: Always returns true with a response time of 0
    /// </returns>
    public bool setUSBDescriptorString(byte stringNum, string descriptorString, uint responseWaitTime)
    {
      if ((int) stringNum == 0)
        throw new ANT_Exception("This overload of setUSBDescriptorString cannot be used to configure the VID/PID");
      byte[] bytes = Encoding.ASCII.GetBytes(descriptorString);
      Array.Resize<byte>(ref bytes, bytes.Length + 1);
      bytes[bytes.Length - 1] = (byte) 0;
      return this.setUSBDescriptorString(stringNum, bytes, (byte) bytes.Length, responseWaitTime);
    }

    /// <summary>
    /// Configure USB descriptor strings
    /// 
    /// </summary>
    /// <param name="stringNum">Descriptor string number</param><param name="descriptorString">Descriptor string (max length 32).  If a longer string is provided, it will be truncated</param>
    public void setUSBDescriptorString(byte stringNum, string descriptorString)
    {
      this.setUSBDescriptorString(stringNum, descriptorString, 0U);
    }

    /// <summary>
    /// Configures USB descriptor strings (VID and PID).
    /// 
    /// </summary>
    /// <param name="usbVID">USB Vendor ID</param><param name="usbPID">USB Product ID</param><param name="responseWaitTime">Time to wait for device success response</param>
    /// <returns>
    /// True on success. Note: Always returns true with a response time of 0
    /// </returns>
    public bool setUSBDescriptorString(ushort usbVID, ushort usbPID, uint responseWaitTime)
    {
      return this.setUSBDescriptorString((byte) 0, new byte[4]
      {
        (byte) ((uint) usbVID & (uint) byte.MaxValue),
        (byte) ((int) usbVID >> 8 & (int) byte.MaxValue),
        (byte) ((uint) usbPID & (uint) byte.MaxValue),
        (byte) ((int) usbPID >> 8 & (int) byte.MaxValue)
      }, (byte) 4, responseWaitTime);
    }

    /// <summary>
    /// Configures USB descriptor strings (VID and PID)
    /// 
    /// </summary>
    /// <param name="usbVID">USB Vendor ID</param><param name="usbPID">USB Product ID</param>
    public void setUSBDescriptorString(ushort usbVID, ushort usbPID)
    {
      this.setUSBDescriptorString(usbVID, usbPID, 0U);
    }

    /// <overloads>Requests a message from the device and returns the response</overloads>
    /// <summary>
    /// Request a message from device and returns the response.
    ///             Throws exception on timeout.
    /// 
    /// </summary>
    /// <param name="channelNum">Channel to send request on</param><param name="messageID">Request to send</param><param name="responseWaitTime">Time to wait for device success response</param>
    public ANT_Response requestMessageAndResponse(byte channelNum, ANT_ReferenceLibrary.RequestMessageID messageID, uint responseWaitTime)
    {
      if (!this.initializedUSB)
        throw new ObjectDisposedException("ANTDevice object has been disposed");
      ANT_Device.ANTMessageItem ANT_MESSAGE_ITEM_response = new ANT_Device.ANTMessageItem();
      if (ANT_Device.ANT_RequestMessage(this.unmanagedANTFramerPtr, channelNum, (byte) messageID, ref ANT_MESSAGE_ITEM_response, responseWaitTime) == 0)
        throw new ANT_Exception("Timed out waiting for requested message");
      else
        return new ANT_Response((object) this, channelNum, DateTime.Now, ANT_MESSAGE_ITEM_response.antMsgData.msgID, Enumerable.ToArray<byte>(Enumerable.Take<byte>((IEnumerable<byte>) ANT_MESSAGE_ITEM_response.antMsgData.ucharBuf, (int) ANT_MESSAGE_ITEM_response.dataSize)));
    }

    /// <summary>
    /// Request a message from device on channel 0 and returns the response.
    ///             Throws exception on timeout.
    /// 
    /// </summary>
    /// <param name="messageID">Request to send</param><param name="responseWaitTime">Time to wait for device success response</param>
    public ANT_Response requestMessageAndResponse(ANT_ReferenceLibrary.RequestMessageID messageID, uint responseWaitTime)
    {
      return this.requestMessageAndResponse((byte) 0, messageID, responseWaitTime);
    }

    /// <overloads>Requests a message from the device</overloads>
    /// <summary>
    /// Request a message from device
    /// 
    /// </summary>
    /// <param name="channelNum">Channel to send request on</param><param name="messageID">Request to send</param>
    public void requestMessage(byte channelNum, ANT_ReferenceLibrary.RequestMessageID messageID)
    {
      if (!this.initializedUSB)
        throw new ObjectDisposedException("ANTDevice object has been disposed");
      ANT_Device.ANTMessageItem ANT_MESSAGE_ITEM_response = new ANT_Device.ANTMessageItem();
      ANT_Device.ANT_RequestMessage(this.unmanagedANTFramerPtr, channelNum, (byte) messageID, ref ANT_MESSAGE_ITEM_response, 0U);
    }

    /// <summary>
    /// Request a message from device
    /// 
    /// </summary>
    /// <param name="messageID">Request to send</param>
    public void requestMessage(ANT_ReferenceLibrary.RequestMessageID messageID)
    {
      this.requestMessage((byte) 0, messageID);
    }

    /// <overloads>Set device in continuous scanning mode</overloads>
    /// <summary>
    /// Starts operation in continuous scanning mode.
    ///             This allows the device to receive all messages matching the configured channel ID mask in an asynchronous manner.
    ///             This feature is not available on all ANT devices.
    /// 
    /// </summary>
    /// <param name="responseWaitTime">Time to wait for device success response</param>
    /// <returns>
    /// True on success. Note: Always returns true with a response time of 0
    /// </returns>
    public bool openRxScanMode(uint responseWaitTime)
    {
      if (!this.initializedUSB)
        throw new ObjectDisposedException("ANTDevice object has been disposed");
      else
        return ANT_Device.ANT_OpenRxScanMode(this.unmanagedANTFramerPtr, responseWaitTime) == 1;
    }

    /// <summary>
    /// Starts operation in continuous scanning mode.
    ///             This allows the device to receive all messages matching the configured channel ID mask in an asynchronous manner.
    /// 
    /// </summary>
    public void openRxScanMode()
    {
      this.openRxScanMode(0U);
    }

    /// <overloads>Initializes and starts CW test mode</overloads>
    /// <summary>
    /// Initialize and start CW test mode. This mode is to test your implementation for RF frequency requirements.
    ///             The device will transmit an unmodulated carrier wave at the RF frequency of 2400Mhz + RFFreqOffset at the specified power level.
    ///             This mode can then only be exited by a system reset.
    ///             Note: When this function call returns false, the system will be reset automatically.
    /// 
    /// </summary>
    /// <param name="transmitPower">Transmission power to test at</param><param name="RFFreqOffset">Offset to add to 2400Mhz</param><param name="responseWaitTime">Time to wait for response, used for both initialization and start command</param>
    /// <returns>
    /// False if initialization or starting of CW test mode fails. On false, the system is automatically reset.
    /// </returns>
    /// 
    /// <remarks>
    /// This function encapsulates both ANT_InitCWTestMode and ANT_SetCWTestMode from the old library.
    ///             It will automatically reset the system if either call fails.
    ///             The given response time is used for both calls and the reset time is 500ms.
    ///             So max wait time = responseTime*2 + 500ms
    /// 
    /// </remarks>
    public bool startCWTest(ANT_ReferenceLibrary.TransmitPower transmitPower, byte RFFreqOffset, uint responseWaitTime)
    {
      if (!this.initializedUSB)
        throw new ObjectDisposedException("ANTDevice object has been disposed");
      bool flag = true;
      this.ResetSystem();
      if (ANT_Device.ANT_InitCWTestMode(this.unmanagedANTFramerPtr, responseWaitTime) != 1)
        flag = false;
      if (ANT_Device.ANT_SetCWTestMode(this.unmanagedANTFramerPtr, (byte) transmitPower, RFFreqOffset, responseWaitTime) != 1)
        flag = false;
      if (!flag && ANT_Common.autoResetIsEnabled)
        this.ResetSystem(500U);
      return flag;
    }

    /// <overloads>Enables extended message reception</overloads>
    /// <summary>
    /// Enables extended message receiving. When enabled, messages received will contain extended data.
    /// 
    /// </summary>
    /// <param name="IsEnabled">Desired State</param><param name="responseWaitTime">Time to wait for device success response</param>
    /// <returns>
    /// True on success. Note: Always returns true with a response time of 0
    /// </returns>
    public bool enableRxExtendedMessages(bool IsEnabled, uint responseWaitTime)
    {
      if (!this.initializedUSB)
        throw new ObjectDisposedException("ANTDevice object has been disposed");
      else
        return ANT_Device.ANT_RxExtMesgsEnable(this.unmanagedANTFramerPtr, Convert.ToByte(IsEnabled), responseWaitTime) == 1;
    }

    /// <summary>
    /// Enables extended message receiving. When enabled, messages received will contain extended data.
    /// 
    /// </summary>
    /// <param name="IsEnabled">Desired State</param>
    public void enableRxExtendedMessages(bool IsEnabled)
    {
      this.enableRxExtendedMessages(IsEnabled, 0U);
    }

    /// <overloads>Enables the use of external 32kHz crystal</overloads>
    /// <summary>
    /// If the use of an external 32kHz crystal input is desired, this message must be sent once, each time a startup message is received
    /// 
    /// </summary>
    /// <param name="responseWaitTime">Time to wait for device success response</param>
    /// <returns>
    /// True on success. Note: Always returns true with a response time of 0
    /// </returns>
    /// 
    /// <remarks>
    /// Enabling an external 32kHz crystal input as a low power clock source saves ~85uA while ANT is active when compared to using the internal clock source.
    /// </remarks>
    public bool crystalEnable(uint responseWaitTime)
    {
      if (!this.initializedUSB)
        throw new ObjectDisposedException("ANTDevice object has been disposed");
      else
        return ANT_Device.ANT_CrystalEnable(this.unmanagedANTFramerPtr, responseWaitTime) == 1;
    }

    /// <summary>
    /// If the use of an external 32kHz crystal input is desired, this message must be sent once, each time a startup message is received
    /// 
    /// </summary>
    /// 
    /// <remarks>
    /// Enabling an external 32kHz crystal input as a low power clock source saves ~85uA while ANT is active when compared to using the internal clock source.
    /// </remarks>
    public void crystalEnable()
    {
      this.crystalEnable(0U);
    }

    /// <summary>
    /// Writes a message to the device, this function allows sending manually formatted messages.
    /// 
    /// </summary>
    /// <param name="msgID">msgID to write</param><param name="msgData">data buffer to write</param>
    /// <returns>
    /// False if writing bytes to device fails
    /// </returns>
    public bool writeRawMessageToDevice(byte msgID, byte[] msgData)
    {
      if (!this.initializedUSB)
        throw new ObjectDisposedException("ANTDevice object has been disposed");
      ANT_Device.ANTMessage pstANTMessage = new ANT_Device.ANTMessage();
      pstANTMessage.msgID = msgID;
      int length = 41 - msgData.Length;
      if (length < 0)
        throw new ANT_Exception("msgData max length is " + (object) 41 + " bytes");
      pstANTMessage.ucharBuf = Enumerable.ToArray<byte>(Enumerable.Concat<byte>((IEnumerable<byte>) msgData, (IEnumerable<byte>) new byte[length]));
      return ANT_Device.ANT_WriteMessage(this.unmanagedANTFramerPtr, pstANTMessage, (ushort) msgData.Length) == 1;
    }

    /// <overloads>Configure the device ANT library, ie: to send extra msg info</overloads>
    /// <summary>
    /// Configure the device ANT library, ie: to send extra msg info
    /// 
    /// </summary>
    /// <param name="libConfigFlags">Config flags</param><param name="responseWaitTime">Time to wait for response</param>
    /// <returns>
    /// True on success. Note: Always returns true with a response time of 0
    /// </returns>
    public bool setLibConfig(ANT_ReferenceLibrary.LibConfigFlags libConfigFlags, uint responseWaitTime)
    {
      if (!this.initializedUSB)
        throw new ObjectDisposedException("ANTDevice object has been disposed");
      else
        return ANT_Device.ANT_SetLibConfig(this.unmanagedANTFramerPtr, (byte) libConfigFlags, responseWaitTime) == 1;
    }

    /// <summary>
    /// Configure the device ANT library, ie: to send extra msg info
    /// 
    /// </summary>
    /// <param name="libConfigFlags">Config flags</param>
    public void setLibConfig(ANT_ReferenceLibrary.LibConfigFlags libConfigFlags)
    {
      this.setLibConfig(libConfigFlags, 0U);
    }

    /// <overloads>Unlock Interface</overloads>
    /// <summary>
    /// Unlock Interface
    /// 
    /// </summary>
    /// <param name="responseWaitTime">Time to wait for response</param>
    /// <returns>
    /// True on success. Note: Always returns true with a response time of 0
    /// </returns>
    public bool unlockInterface(uint responseWaitTime)
    {
      if (!this.initializedUSB)
        throw new ObjectDisposedException("ANTDevice object has been disposed");
      else
        return ANT_Device.ANT_UnlockInterface(this.unmanagedANTFramerPtr, responseWaitTime) == 1;
    }

    /// <summary>
    /// Unlock Interface
    /// 
    /// </summary>
    public void unlockInterface()
    {
      this.unlockInterface(0U);
    }

    /// <overloads>Writes a SensRCore command to non-volatile memory</overloads>
    /// <summary>
    /// Writes a SensRcore command to non-volatile memory.
    ///             Throws exception if command string length &gt; 255, although commands will be much smaller
    /// 
    /// </summary>
    /// <param name="commandString">SensRcore command to write: [Cmd][CmdData0]...[CmdDataN], must be less than 256 bytes</param><param name="responseWaitTime">Time to wait for device success response</param>
    /// <returns>
    /// True on success. Note: Always returns true with a response time of 0
    /// </returns>
    public bool script_Write(byte[] commandString, uint responseWaitTime)
    {
      if (!this.initializedUSB)
        throw new ObjectDisposedException("ANTDevice object has been disposed");
      if (commandString.Length >= 256)
        throw new ANT_Exception("commandString max size is 255");
      else
        return ANT_Device.ANT_Script_Write(this.unmanagedANTFramerPtr, (byte) commandString.Length, commandString, responseWaitTime) == 1;
    }

    /// <summary>
    /// Writes a SensRcore command to non-volatile memory.
    ///             Throws exception if command string length &gt; 255.
    /// 
    /// </summary>
    /// <param name="commandString">SensRcore command to write: [Cmd][CmdData0]...[CmdDataN], must be less than 256 bytes</param>
    public void script_Write(byte[] commandString)
    {
      this.script_Write(commandString, 0U);
    }

    /// <overloads>Clears the NVM</overloads>
    /// <summary>
    /// Clears the non-volatile memory. NVM should be cleared before beginning write operations.
    /// 
    /// </summary>
    /// <param name="responseWaitTime">Time to wait for device success response</param>
    /// <returns>
    /// True on success. Note: Always returns true with a response time of 0
    /// </returns>
    public bool script_Clear(uint responseWaitTime)
    {
      if (!this.initializedUSB)
        throw new ObjectDisposedException("ANTDevice object has been disposed");
      else
        return ANT_Device.ANT_Script_Clear(this.unmanagedANTFramerPtr, responseWaitTime) == 1;
    }

    /// <summary>
    /// Clears the non-volatile memory. NVM should be cleared before beginning write operations.
    /// 
    /// </summary>
    public void script_Clear()
    {
      this.script_Clear(0U);
    }

    /// <overloads>Sets the default SensRCore sector</overloads>
    /// <summary>
    /// Set the default sector which will be executed after mandatory execution of sector 0.
    ///             This command has no effect if it is set to 0 or the Read Pins for Sector command appears in sector 0.
    /// 
    /// </summary>
    /// <param name="sectorNum">sector number to set as default</param><param name="responseWaitTime">Time to wait for device success response</param>
    /// <returns>
    /// True on success. Note: Always returns true with a response time of 0
    /// </returns>
    public bool script_setDefaultSector(byte sectorNum, uint responseWaitTime)
    {
      if (!this.initializedUSB)
        throw new ObjectDisposedException("ANTDevice object has been disposed");
      else
        return ANT_Device.ANT_Script_SetDefaultSector(this.unmanagedANTFramerPtr, sectorNum, responseWaitTime) == 1;
    }

    /// <summary>
    /// Set the default sector which will be executed after mandatory execution of sector 0.
    ///             This command has no effect if it is set to 0 or the Read Pins for Sector command appears in sector 0.
    /// 
    /// </summary>
    /// <param name="sectorNum">sector number to set as default</param>
    public void script_setDefaultSector(byte sectorNum)
    {
      this.script_setDefaultSector(sectorNum, 0U);
    }

    /// <overloads>Writes a sector break to NVM</overloads>
    /// <summary>
    /// Writes a sector break in the NVM image
    /// 
    /// </summary>
    /// <param name="responseWaitTime">Time to wait for device success response</param>
    /// <returns>
    /// True on success. Note: Always returns true with a response time of 0
    /// </returns>
    public bool script_endSector(uint responseWaitTime)
    {
      if (!this.initializedUSB)
        throw new ObjectDisposedException("ANTDevice object has been disposed");
      else
        return ANT_Device.ANT_Script_EndSector(this.unmanagedANTFramerPtr, responseWaitTime) == 1;
    }

    /// <summary>
    /// Writes a sector break in the NVM image
    /// 
    /// </summary>
    public void script_endSector()
    {
      this.script_endSector(0U);
    }

    /// <overloads>Request a dump of the device's script memory</overloads>
    /// <summary>
    /// Requests the device to return the current NVM contents through the device callback function.
    ///             The end of the dump is signified by a 0x57 NVM_Cmd msg, which contains 0x04 EndDump code followed by
    ///             a byte signifying how many instructions were read and returned.
    /// 
    /// </summary>
    /// <param name="responseWaitTime">Time to wait for device success response</param>
    /// <returns>
    /// True on success. Note: Always returns true with a response time of 0
    /// </returns>
    public bool script_requestNVMDump(uint responseWaitTime)
    {
      if (!this.initializedUSB)
        throw new ObjectDisposedException("ANTDevice object has been disposed");
      else
        return ANT_Device.ANT_Script_Dump(this.unmanagedANTFramerPtr, responseWaitTime) == 1;
    }

    /// <summary>
    /// Requests the device to return the current NVM contents through the device callback function.
    ///             The end of the dump is signified by a 0x57 NVM_Cmd msg, which contains 0x04 EndDump code followed by
    ///             a byte signifying how many instructions were read and returned.
    /// 
    /// </summary>
    public void script_requestNVMDump()
    {
      this.script_requestNVMDump(0U);
    }

    /// <overloads>Locks the NVM contents</overloads>
    /// <summary>
    /// Locks the NVM so that it can not be read by the dump function.
    ///             Can only be disabled by clearing the NVM.
    /// 
    /// </summary>
    /// <param name="responseWaitTime">Time to wait for device success response</param>
    /// <returns>
    /// True on success. Note: Always returns true with a response time of 0
    /// </returns>
    public bool script_lockNVM(uint responseWaitTime)
    {
      if (!this.initializedUSB)
        throw new ObjectDisposedException("ANTDevice object has been disposed");
      else
        return ANT_Device.ANT_Script_Lock(this.unmanagedANTFramerPtr, responseWaitTime) == 1;
    }

    /// <summary>
    /// Locks the NVM so that it can not be read by the dump function.
    ///             Can only be disabled by clearing the NVM.
    /// 
    /// </summary>
    public void script_lockNVM()
    {
      this.script_lockNVM(0U);
    }

    /// <overloads>Manually run a script</overloads>
    /// <summary>
    /// Manually run given script number
    /// 
    /// </summary>
    /// <param name="ScriptNumber">Script number to run</param><param name="responseWaitTime">Time to wait for device success response</param>
    /// <returns>
    /// True on success. Note: Always returns true with a response time of 0
    /// </returns>
    public bool script_Run(byte ScriptNumber, uint responseWaitTime)
    {
      if (!this.initializedUSB)
        throw new ObjectDisposedException("ANTDevice object has been disposed");
      else
        return ANT_Device.ANT_RunScript(this.unmanagedANTFramerPtr, ScriptNumber, responseWaitTime) == 1;
    }

    /// <summary>
    /// Manually run given script number
    /// 
    /// </summary>
    /// <param name="ScriptNumber">Script number to run</param>
    public void script_Run(byte ScriptNumber)
    {
      this.script_Run(ScriptNumber, 0U);
    }

    /// <overloads>Sets the equipment state</overloads>
    /// <summary>
    /// Sets the equipment state.
    ///              This command is specifically for use with the FIT1e module.
    /// 
    /// </summary>
    /// <param name="feState">Fitness equipment state</param><param name="responseWaitTime">Time to wait for device success response</param>
    /// <returns>
    /// True on success.  Note: Always returns true with a response time of 0
    /// </returns>
    public bool fitSetFEState(byte feState, uint responseWaitTime)
    {
      if (!this.initializedUSB)
        throw new ObjectDisposedException("ANTDevice object has been disposed");
      if (!this.capabilities.FIT)
        return false;
      else
        return ANT_Device.FIT_SetFEState(this.unmanagedANTFramerPtr, feState, responseWaitTime) == 1;
    }

    /// <summary>
    /// Sets the equipment state.
    ///             This command is specifically for use with the FIT1e module.
    /// 
    /// </summary>
    /// <param name="feState">Fitness equipment state</param>
    public void fitSetFEState(byte feState)
    {
      this.fitSetFEState(feState, 0U);
    }

    /// <summary>
    /// Adjusts the pairing distance settings.
    ///             This command is specifically for use with the FIT1e module.
    /// 
    /// </summary>
    /// <param name="searchLv">Minimum signal strength for a signal to be considered for pairing.</param><param name="pairLv">Signal strength required for the FIT1e to pair with an ANT+ HR strap or watch</param><param name="trackLv">An ANT+ device will unpair if the signal strength drops below this setting while in
    ///             READY state or within the first 30 secons of the IN_USE state</param><param name="responseWaitTime">Time to wait for device success response</param>
    /// <returns>
    /// True on success.  Note: Always returns true with a response time of 0
    /// </returns>
    public bool fitAdjustPairingSettings(byte searchLv, byte pairLv, byte trackLv, uint responseWaitTime)
    {
      if (!this.initializedUSB)
        throw new ObjectDisposedException("ANTDevice object has been disposed");
      if (!this.capabilities.FIT)
        return false;
      else
        return ANT_Device.FIT_AdjustPairingSettings(this.unmanagedANTFramerPtr, searchLv, pairLv, trackLv, responseWaitTime) == 1;
    }

    /// <summary>
    /// Adjusts the pairing distance settings.
    ///             This command is specifically for use with the FIT1e module.
    /// 
    /// </summary>
    /// <param name="searchLv">Minimum signal strength for a signal to be considered for pairing.</param><param name="pairLv">Signal strength required for the FIT1e to pair with an ANT+ HR strap or watch</param><param name="trackLv">An ANT+ device will unpair if the signal strength drops below this setting while in
    ///             READY state or within the first 30 secons of the IN_USE state</param>
    public void fitAdjustPairingSettings(byte searchLv, byte pairLv, byte trackLv)
    {
      this.fitAdjustPairingSettings(searchLv, pairLv, trackLv, 0U);
    }

    private struct ANTMessageItem
    {
      public byte dataSize;
      public ANT_Device.ANTMessage antMsgData;
    }

    /// <summary>
    /// ANTMessage struct as defined in unmanaged code for marshalling ant messages with unmanaged code
    /// 
    /// </summary>
    public struct ANTMessage
    {
      /// <summary>
      /// Message ID byte
      /// 
      /// </summary>
      public byte msgID;
      /// <summary>
      /// Data buffer
      /// 
      /// </summary>
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 41)]
      public byte[] ucharBuf;
    }

    /// <summary>
    /// Codes for the device notification event
    /// 
    /// </summary>
    public enum DeviceNotificationCode : byte
    {
      Reset = (byte) 1,
      Shutdown = (byte) 2,
    }

    /// <summary>
    /// Delegate for device response event
    /// 
    /// </summary>
    /// <param name="response">Message details received from device</param>
    public delegate void dDeviceResponseHandler(ANT_Response response);

    /// <summary>
    /// Function to handle ANT_Device serial errors
    /// 
    /// </summary>
    /// <param name="sender">The ANT_Device reporting the error</param><param name="error">The serial error that occured</param><param name="isCritical">If true, the communication with the device is lost and this device object should be disposed</param>
    public delegate void dSerialErrorHandler(ANT_Device sender, ANT_Device.serialErrorCode error, bool isCritical);

    /// <summary>
    /// ANT Device Serial Error Codes
    /// 
    /// </summary>
    public enum serialErrorCode
    {
      SerialWriteError,
      SerialReadError,
      DeviceConnectionLost,
      MessageLost_CrcError,
      MessageLost_QueueOverflow,
      MessageLost_TooLarge,
      MessageLost_InvalidChannel,
      Unknown,
    }
  }
}
