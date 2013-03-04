using System;
using UnityEngine;
using System.Runtime.InteropServices;
using ANT_Managed_Library;
using AppDomain;

public class SomaxisTrackerNative : MonoBehaviour, ISomaxisTracker
{            
	struct SomaxisTrackingProperties {
        public int playerCount; //should be 1-4
    };

	private byte[] KEY = new byte[8]
    {
      (byte)185,
      (byte)165,
      (byte)33,
      (byte)251,
      (byte)189,
      (byte)114,
      (byte)195,
      (byte)69
    };

	SomaxisTrackingProperties currentTrackingProperties;
	SomaxisManager owner;
	private bool nativeTransmitterIsRunning = false;
/*	
	private ulong _numMessages = 0UL;
	private uint _gBufferIndex = 0U;
	private uint _gNumTaps = 4U;
	private ushort[] _gSampleHistory = (ushort[])null;
	private uint _gSampleHistorySum = 0U;
	private ushort[] _gSampleAverageHistory = (ushort[])null;
	private uint[] _gSampleAverageDifferenceSquaredHistory = (uint[])null;
	private ushort[] _gPseudoStandardDeviationHistory = new ushort[4];
	 */
	private byte[] BroadcastBuffer = new byte[8];
	private const int CHARTLINE_ONE_SECOND_INTERVAL = 0;
	private const int CHARTLINE_AFE_MOVING_AVERAGE = 1;
	private const int NUM_POINTS_IN_WINDOW = 320;
	
	private ANT_Device antDevice;
	private ANT_Channel antfsBroadcastChannel;	
	
	private ushort BAUD_RATE = (ushort)57600U;
	private byte deviceNumber = (byte)0;
	private byte deviceType = (byte)0;
	private byte deviceRfFreq = (byte)0;
	private ushort periodDivisor = (ushort)500U;
	private ushort deviceChannel = (ushort)1;
	
	public SomaxisTrackerNative() {
		/*
		// Create application domain setup information
		AppDomainSetup domaininfo = new AppDomainSetup ();
		domaininfo.ConfigurationFile = System.Environment.CurrentDirectory + "myExe.exe.config";
		domaininfo.ApplicationBase = System.Environment.CurrentDirectory;

		//Create evidence for the new appdomain from evidence of the current application domain
		Evidence adevidence = AppDomain.CurrentDomain.Evidence;

		// Create appdomain
		AppDomain domain = AppDomain.CreateDomain ("Somaxis", adevidence, domaininfo);

		// Write out application domain information
		Console.WriteLine ("Host domain: " + AppDomain.CurrentDomain.FriendlyName);
		Console.WriteLine ("child domain: " + domain.FriendlyName);
		Console.WriteLine ();
		Console.WriteLine ("Configuration file is: " + domain.SetupInformation.ConfigurationFile);
		Console.WriteLine ("Application Base Directory is: " + domain.BaseDirectory);
		*/
	}
	
	public void SetDeviceChannel (ushort c)
	{
		this.deviceChannel = c;
	}
	
	public void SetDeviceType (byte t) {
		this.deviceType = t;
	}
	
      
	public void SetOwner(SomaxisManager mm)
	{
		owner = mm;
	}

	public void EnableTracking ()
	{
		Debug.Log ("enable tracking of ANT+ device");
		if (!nativeTransmitterIsRunning) {
			nativeTransmitterIsRunning = true;
			try {
				this.antDevice = new ANT_Device (ANT_ReferenceLibrary.PortType.USB, deviceNumber, this.BAUD_RATE, ANT_ReferenceLibrary.FramerType.basicANT);
				if (this.antDevice.getDeviceCapabilities ().ExtendedMessaging) {
					this.antDevice.setLibConfig (ANT_ReferenceLibrary.LibConfigFlags.MESG_OUT_INC_RSSI_0x40 
						| ANT_ReferenceLibrary.LibConfigFlags.MESG_OUT_INC_DEVICE_ID_0x80);
				}

				this.antDevice.deviceResponse += new ANT_Device.dDeviceResponseHandler (this.handleANTResponses);
				this.antfsBroadcastChannel = this.antDevice.getChannel (this.deviceChannel);
				this.antfsBroadcastChannel.channelResponse += new dChannelResponseHandler (this.handleBroadcastChannelResponses);
				this.configureBroadcastChannel ();
				this.antDevice.serialError += new ANT_Device.dSerialErrorHandler (this.handleSerialError);
			} catch (ANT_Exception ex) {
				Debug.LogError (ex.Message);
			}
		}
	}
	
	public void DisableTracking ()
	{
		nativeTransmitterIsRunning = false;
		lock (this) {
			if (this.antDevice != null) {
				this.antDevice.Dispose ();
				this.antDevice = (ANT_Device)null;
				this.antfsBroadcastChannel = (ANT_Channel)null;
				this.closeBroadcastChannel();
			}
		}
	}

	public bool IsTracking ()
	{
		return nativeTransmitterIsRunning;
	}

	public void DoUpdate ()
	{
	}

	public void SetPlayerCount (int playerCount)
	{
		currentTrackingProperties.playerCount = playerCount;
		SetTrackingProperties (currentTrackingProperties);
	}
	
	private void SetTrackingProperties (SomaxisTrackingProperties props)
	{
		this.currentTrackingProperties = props;
	}
	

    private void handleSerialError(ANT_Device sender, ANT_Device.serialErrorCode error, bool isCritical)
    {
      Debug.LogError("Error: Serial Failure");
      DisableTracking();
    }

    private void handleANTResponses (ANT_Response response)
	{
		if ((int)response.responseID == 64) {
			if (response.getChannelEventCode () == ANT_ReferenceLibrary.ANTEventID.RESPONSE_NO_ERROR_0x00)
				Debug.LogError ("SET " + ((object)response.getMessageID ()).ToString () + "... OK");
			else
				Debug.LogError ("Response: " + ((object)response.getChannelEventCode ()).ToString () 
					+ " to " + ((object)response.getMessageID ()).ToString());
		} else 
			Debug.LogError (((object)(ANT_ReferenceLibrary.ANTMessageID)response.responseID).ToString ());
			
	}
	/*
    private ushort _movingAverage(ushort aSample)
    {
      for (int index = 0; index < this._gSampleHistory.Length - 1; ++index)
        this._gSampleHistory[index] = this._gSampleHistory[index + 1];
      this._gSampleHistory[this._gSampleHistory.Length - 1] = aSample;
      uint num1 = 0U;
      foreach (ushort num2 in this._gSampleHistory)
        num1 += (uint) num2;
      return (ushort) ((ulong) num1 / (ulong) this._gSampleHistory.Length);
    }

    private ushort _variance(ushort aMean, ushort aSample)
    {
      try {
        return Convert.ToUInt16(Math.Sqrt(Math.Pow((double) ((int) aSample - (int) aMean), 2.0)));
      }
      catch
      {
        return (ushort) 0;
      }
    }	
	
	*/

    private void handleBroadcastChannelResponses (ANT_Response response)
	{
		switch ((ANT_ReferenceLibrary.ANTMessageID)response.responseID) {
		case ANT_ReferenceLibrary.ANTMessageID.RESPONSE_EVENT_0x40:
			switch (response.getChannelEventCode ()) {
			case ANT_ReferenceLibrary.ANTEventID.EVENT_RX_SEARCH_TIMEOUT_0x01:
				Debug.LogError ("Search timed out");
				return;
			case ANT_ReferenceLibrary.ANTEventID.EVENT_TX_0x03:
				this.antfsBroadcastChannel.sendBroadcastData (this.BroadcastBuffer);
				this.BroadcastBuffer [7] = (byte)((uint)this.BroadcastBuffer [7] + 1U);
				return;
			case ANT_ReferenceLibrary.ANTEventID.EVENT_CHANNEL_CLOSED_0x07:
				Debug.Log ("Channel closed, trying again");
				if (this.antfsBroadcastChannel.openChannel (this.deviceChannel))
					return;
				Debug.LogError ("Failed to open the channel");
				return;
			case ANT_ReferenceLibrary.ANTEventID.EVENT_CHANNEL_ACTIVE_0x0F:
				return;
			default:
				Debug.Log (string.Format ("{0}", (object)(int)response.getChannelEventCode ()));
				return;
			}
		case ANT_ReferenceLibrary.ANTMessageID.BROADCAST_DATA_0x4E:
			Debug.Log ("Channel = " + (object)response.antChannel);
			try {
				//this.Invoke ((Delegate)this.aListBoxMessageDelegate, (object)response.antChannel, 
				//	(object)response.getDataPayload ());
				this.PublishReading (response);
				break;
			} catch (Exception ex) {
				Debug.LogError (((object)ex.Message).ToString ());
				break;
			}
		case ANT_ReferenceLibrary.ANTMessageID.ACKNOWLEDGED_DATA_0x4F:
			if ((int)response.getDataPayload () [0] != 70 || ((int)response.getDataPayload () [5] != 0 
				|| (int)response.getDataPayload () [6] != 67 || (int)response.getDataPayload () [7] != 2))
				break;
			else
				break;
		case ANT_ReferenceLibrary.ANTMessageID.CHANNEL_ID_0x51:
			break;
		default:
			Debug.LogError ("HBCR:Unexpected message = " + (object)response.responseID);
			break;
		}
	}
	
	private void PublishReading (ANT_Response response)
	{
		Debug.Log ("Publishing BROADCAST_DATA_0x4E !!!");
		byte[] payload = response.getDataPayload ();
		
		// What the heck is going on here?!
		ushort[] dataArray1 = new ushort[4]
        {
          (ushort)(((int)payload [3] & (int)byte.MaxValue) << 2 | (int)payload [4] >> 6),
          (ushort)(((int)payload [4] & 63) << 4 | (int)payload [5] >> 4),
          (ushort)(((int)payload [5] & 15) << 6 | (int)payload [6] >> 2),
          (ushort)((uint)(((int)payload [6] & 3) << 8) | (uint)payload [7])
        };
		
		// Why are there 4 readings per sample?
		for (int index = 0; index < 4; index++) {
			/* logic to hanle sample history
			this._gSampleHistorySum -= (uint)this._gSampleHistory [this._gBufferIndex];
			this._gSampleHistory [this._gBufferIndex] = dataArray1 [index];
			this._gSampleHistorySum += (uint)this._gSampleHistory [this._gBufferIndex];
			this._gSampleAverageHistory [this._gBufferIndex] = (ushort)(this._gSampleHistorySum / this._gNumTaps);
			this._gSampleAverageDifferenceSquaredHistory [this._gBufferIndex] = 
				(uint)Math.Pow ((double)((int)this._gSampleHistory [this._gBufferIndex] 
					- (int)this._gSampleAverageHistory [this._gBufferIndex]), 2.0);
			uint num1 = 0U;
			foreach (uint num2 in this._gSampleAverageDifferenceSquaredHistory)
				num1 += num2;
			this._gPseudoStandardDeviationHistory [index] = (ushort)Math.Sqrt ((double)num1 / (double)(this._gNumTaps - 1U));
			this._gBufferIndex = ++this._gBufferIndex % this._gNumTaps;
			*/
			SomaxisReading reading = new SomaxisReading (payload [0], dataArray1 [index]);
			owner.PublishReading (reading, this);
		}		
	
	}

    private void configureBroadcastChannel ()
	{
		ANT_ChannelStatus antChannelStatus = this.antfsBroadcastChannel.requestStatus (this.deviceChannel);
		if (antChannelStatus.BasicStatus == ANT_ReferenceLibrary.BasicChannelStatusCode.TRACKING_0x3)
			return;
		if (antChannelStatus.BasicStatus == ANT_ReferenceLibrary.BasicChannelStatusCode.UNASSIGNED_0x0 
			&& !this.antfsBroadcastChannel.assignChannel (ANT_ReferenceLibrary.ChannelType.BASE_Slave_Receive_0x00, 
			this.deviceNumber, this.deviceChannel))
			Debug.LogError ("Error assigning channel");
		else if (!this.antDevice.setNetworkKey (this.deviceNumber, this.KEY, this.deviceChannel)) {
			Debug.LogError ("Unable to set ANT network key");
		} else {
			//if ((int)(ushort)this.antDevice.getSerialNumber () == 0);
			if (!this.antfsBroadcastChannel.setChannelID (this.deviceNumber, false, 
				this.deviceType, (byte)5, 500U)) {
				Debug.LogError ("Error configuring channel ID");
			} else if (!this.antfsBroadcastChannel.setChannelFreq (this.deviceRfFreq, 500U)) {
				Debug.LogError ("Error configuring radio frequency");
			} else if (!this.antfsBroadcastChannel.setChannelPeriod (this.periodDivisor, 500U)) {
				Debug.LogError ("Error configuring channel period");
			} else if (!this.antfsBroadcastChannel.openChannel (this.deviceChannel)) {
				Debug.LogError ("Failed to open the channel");
			} else {
				Debug.LogError ("ANT-FS Broadcast channel open");
			}
		}
	}

    private void closeBroadcastChannel()
    {
      if (!this.antfsBroadcastChannel.closeChannel(this.deviceChannel))
        Debug.LogError("Failed to close channel");
      else
        Debug.LogError("ANT-FS Broadcast channel closed");
    }
	
}
