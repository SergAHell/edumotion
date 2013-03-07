// Type: ANT_Managed_Library.ANT_ReferenceLibrary
// Assembly: ANT_NET, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Users\Alex\Desktop\SomaxisAfeDiagnosticMonitor_20120602_V1_0_1_1\SomaxisAfeDiagnosticMonitor_20120602_V1_0_1_1\ANT_NET.dll

using System;

namespace ANT_Managed_Library
{
  /// <summary>
  /// Contains all the ANT constants and enumerations for general use.
  ///             Note: Where desired, in functions where enumerations are required, a byte type can be
  ///             cast to the enumeration to feed the function raw byte values.
  ///             IE: <c>ANTDeviceInstance.RequestMessage((RequestMessageID)0x4E));</c> would compile.
  /// 
  /// </summary>
  public sealed class ANT_ReferenceLibrary
  {
    public const byte MAX_MESG_SIZE = (byte) 41;

    private ANT_ReferenceLibrary()
    {
      throw new Exception("This is a static class, do not create instances");
    }

    /// <summary>
    /// Possible port connection types.
    /// 
    /// </summary>
    public enum PortType : byte
    {
      USB,
      COM,
    }

    /// <summary>
    /// Possible framing modes.
    ///             Use FramerType.basicANT unless you know you need to use another.
    /// 
    /// </summary>
    public enum FramerType : byte
    {
      basicANT,
      HCI_TI,
      HCI_BRCM,
      HCI_TI_DIRECT,
      HCI_ST_ERICSSON,
    }

    /// <summary>
    /// Channel Type flags. A valid channel type is one BASE parameter (Slave XOR Receive)
    ///             combined by '|' (bitwise OR) with any desired ADV parameters
    /// 
    /// </summary>
    [Flags]
    public enum ChannelType : byte
    {
      BASE_Slave_Receive_0x00 = (byte) 0,
      BASE_Master_Transmit_0x10 = (byte) 16,
      ADV_Shared_0x20 = (byte) 32,
      ADV_TxRx_Only_or_RxAlwaysWildCard_0x40 = (byte) 64,
      ADV_Enable_RSSI_0x80 = (byte) 128,
    }

    [Flags]
    public enum ChannelTypeExtended : byte
    {
      ADV_AlwaysSearch_0x01 = (byte) 1,
      ADV_FrequencyAgility_0x04 = (byte) 4,
      ADV_IgnoreTransmissionType_0x02 = (byte) 2,
      ADV_AutoSharedSlave_0x08 = (byte) 8,
    }

    /// <summary>
    /// The int status codes returned by the acknowledged and broadcast messaging functions.
    /// 
    /// </summary>
    public enum MessagingReturnCode
    {
      Fail,
      Pass,
      Timeout,
      Cancelled,
      InvalidParams,
    }

    /// <summary>
    /// Basic Channel status message codes, the bottom two bits of the received status message
    /// 
    /// </summary>
    public enum BasicChannelStatusCode : byte
    {
      UNASSIGNED_0x0,
      ASSIGNED_0x1,
      SEARCHING_0x2,
      TRACKING_0x3,
    }

    /// <summary>
    /// Transmit Power offsets
    /// 
    /// </summary>
    public enum TransmitPower : byte
    {
      RADIO_TX_POWER_MINUS20DB_0x00,
      RADIO_TX_POWER_MINUS10DB_0x01,
      RADIO_TX_POWER_MINUS5DB_0x02,
      RADIO_TX_POWER_0DB_0x03,
    }

    /// <summary>
    /// Startup message
    /// 
    /// </summary>
    public enum StartupMessage : byte
    {
      RESET_POR_0x00 = (byte) 0,
      RESET_RST_0x01 = (byte) 1,
      RESET_WDT_0x02 = (byte) 2,
      RESET_CMD_0x20 = (byte) 32,
      RESET_SYNC_0x40 = (byte) 64,
      RESET_SUSPEND_0x80 = (byte) 128,
    }

    /// <summary>
    /// Message ID to request message.
    ///             Note: Where desired, raw byte values can be cast to the enum type. IE: <c>(RequestMessageID)0x4E</c> will compile.
    /// 
    /// </summary>
    public enum RequestMessageID : byte
    {
      VERSION_0x3E = (byte) 62,
      CHANNEL_ID_0x51 = (byte) 81,
      CHANNEL_STATUS_0x52 = (byte) 82,
      CAPABILITIES_0x54 = (byte) 84,
      SERIAL_NUMBER_0x61 = (byte) 97,
    }

    /// <summary>
    /// Command Codes for SensRcore operations
    /// 
    /// </summary>
    public enum SensRcoreScriptCommandCodes : byte
    {
      SCRIPT_CMD_FORMAT_0x00,
      SCRIPT_CMD_DUMP_0x01,
      SCRIPT_CMD_SET_DEFAULT_SECTOR_0x02,
      SCRIPT_CMD_END_SECTOR_0x03,
      SCRIPT_CMD_END_DUMP_0x04,
      SCRIPT_CMD_LOCK_0x05,
    }

    /// <summary>
    /// Flags for configuring device ANT library
    /// 
    /// </summary>
    [Flags]
    public enum LibConfigFlags
    {
      RADIO_CONFIG_ALWAYS_0x01 = 1,
      MESG_OUT_INC_TIME_STAMP_0x20 = 32,
      MESG_OUT_INC_RSSI_0x40 = 64,
      MESG_OUT_INC_DEVICE_ID_0x80 = 128,
    }

    /// <summary>
    /// Flags for configuring advanced bursting features.
    /// 
    /// </summary>
    [Flags]
    public enum AdcancedBurstConfigFlags : uint
    {
      FREQUENCY_HOP_ENABLE = 1U,
    }

    /// <summary>
    /// MessageIDs for reference
    /// 
    /// </summary>
    public enum ANTMessageID : byte
    {
      INVALID_0x00 = (byte) 0,
      EVENT_0x01 = (byte) 1,
      VERSION_0x3E = (byte) 62,
      RESPONSE_EVENT_0x40 = (byte) 64,
      UNASSIGN_CHANNEL_0x41 = (byte) 65,
      ASSIGN_CHANNEL_0x42 = (byte) 66,
      CHANNEL_MESG_PERIOD_0x43 = (byte) 67,
      CHANNEL_SEARCH_TIMEOUT_0x44 = (byte) 68,
      CHANNEL_RADIO_FREQ_0x45 = (byte) 69,
      NETWORK_KEY_0x46 = (byte) 70,
      RADIO_TX_POWER_0x47 = (byte) 71,
      RADIO_CW_MODE_0x48 = (byte) 72,
      SEARCH_WAVEFORM_0x49 = (byte) 73,
      SYSTEM_RESET_0x4A = (byte) 74,
      OPEN_CHANNEL_0x4B = (byte) 75,
      CLOSE_CHANNEL_0x4C = (byte) 76,
      REQUEST_0x4D = (byte) 77,
      BROADCAST_DATA_0x4E = (byte) 78,
      ACKNOWLEDGED_DATA_0x4F = (byte) 79,
      BURST_DATA_0x50 = (byte) 80,
      CHANNEL_ID_0x51 = (byte) 81,
      CHANNEL_STATUS_0x52 = (byte) 82,
      RADIO_CW_INIT_0x53 = (byte) 83,
      CAPABILITIES_0x54 = (byte) 84,
      STACKLIMIT_0x55 = (byte) 85,
      SCRIPT_DATA_0x56 = (byte) 86,
      SCRIPT_CMD_0x57 = (byte) 87,
      ID_LIST_ADD_0x59 = (byte) 89,
      ID_LIST_CONFIG_0x5A = (byte) 90,
      OPEN_RX_SCAN_0x5B = (byte) 91,
      EXT_CHANNEL_RADIO_FREQ_0x5C = (byte) 92,
      EXT_BROADCAST_DATA_0x5D = (byte) 93,
      EXT_ACKNOWLEDGED_DATA_0x5E = (byte) 94,
      EXT_BURST_DATA_0x5F = (byte) 95,
      CHANNEL_RADIO_TX_POWER_0x60 = (byte) 96,
      GET_SERIAL_NUM_0x61 = (byte) 97,
      GET_TEMP_CAL_0x62 = (byte) 98,
      SET_LP_SEARCH_TIMEOUT_0x63 = (byte) 99,
      SET_TX_SEARCH_ON_NEXT_0x64 = (byte) 100,
      SERIAL_NUM_SET_CHANNEL_ID_0x65 = (byte) 101,
      RX_EXT_MESGS_ENABLE_0x66 = (byte) 102,
      RADIO_CONFIG_ALWAYS_0x67 = (byte) 103,
      ENABLE_LED_FLASH_0x68 = (byte) 104,
      LED_OVERRIDE_0x69 = (byte) 105,
      AGC_CONFIG_0x6A = (byte) 106,
      CLOCK_DRIFT_DATA_0x6B = (byte) 107,
      RUN_SCRIPT_0x6C = (byte) 108,
      XTAL_ENABLE_0x6D = (byte) 109,
      ANTLIB_CONFIG_0x6E = (byte) 110,
      STARTUP_MESG_0x6F = (byte) 111,
      AUTO_FREQ_CONFIG_0x70 = (byte) 112,
      PROX_SEARCH_CONFIG_0x71 = (byte) 113,
      ADV_BURST_DATA_0x72 = (byte) 114,
      ADV_BURST_CONFIG_0x78 = (byte) 120,
      CUBE_CMD_0x80 = (byte) 128,
      PIN_DIODE_CONTROL_0x8E = (byte) 142,
      FIT1_SET_AGC_0x8F = (byte) 143,
      SET_CHANNEL_INPUT_MASK_0x90 = (byte) 144,
      FIT1_SET_EQUIP_STATE_0x91 = (byte) 145,
      SET_CHANNEL_DATA_TYPE_0x91 = (byte) 145,
      READ_PINS_FOR_SECT_0x92 = (byte) 146,
      TIMER_SELECT_0x93 = (byte) 147,
      ATOD_SETTINGS_0x94 = (byte) 148,
      SET_SHARED_ADDRESS_0x95 = (byte) 149,
      ATOD_EXTERNAL_ENABLE_0x96 = (byte) 150,
      ATOD_PIN_SETUP_0x97 = (byte) 151,
      SETUP_ALARM_0x98 = (byte) 152,
      ALARM_VARIABLE_MODIFY_TEST_0x99 = (byte) 153,
      PARTIAL_RESET_0x9A = (byte) 154,
      OVERWRITE_TEMP_CAL_0x9B = (byte) 155,
      SERIAL_PASSTHRU_SETTINGS_0x9C = (byte) 156,
      READ_SEGA_0xA0 = (byte) 160,
      SEGA_CMD_0xA1 = (byte) 161,
      SEGA_DATA_0xA2 = (byte) 162,
      SEGA_ERASE_0xA3 = (byte) 163,
      SEGA_WRITE_0xA4 = (byte) 164,
      SEGA_LOCK_0xA6 = (byte) 166,
      FLASH_PROTECTION_CHECK_0xA7 = (byte) 167,
      UARTREG_0xA8 = (byte) 168,
      MAN_TEMP_0xA9 = (byte) 169,
      BIST_0xAA = (byte) 170,
      SELFERASE_0xAB = (byte) 171,
      SET_MFG_BITS_0xAC = (byte) 172,
      UNLOCK_INTERFACE_0xAD = (byte) 173,
      SERIAL_ERROR_0xAE = (byte) 174,
      IO_STATE_0xB0 = (byte) 176,
      CFG_STATE_0xB1 = (byte) 177,
      BLOWFUSE_0xB2 = (byte) 178,
      MASTERIOCTRL_0xB3 = (byte) 179,
      PORT_GET_IO_STATE_0xB4 = (byte) 180,
      PORT_SET_IO_STATE_0xB5 = (byte) 181,
      RSSI_POWER_0xC0 = (byte) 192,
      RSSI_BROADCAST_DATA_0xC1 = (byte) 193,
      RSSI_ACKNOWLEDGED_DATA_0xC2 = (byte) 194,
      RSSI_BURST_DATA_0xC3 = (byte) 195,
      RSSI_SEARCH_THRESHOLD_0xC4 = (byte) 196,
      SLEEP_0xC5 = (byte) 197,
      SET_USB_INFO_0xC7 = (byte) 199,
      DEBUG_0xF0 = (byte) 240,
    }

    /// <summary>
    /// EventIDs for reference
    /// 
    /// </summary>
    public enum ANTEventID : byte
    {
      NO_EVENT_0x00 = (byte) 0,
      RESPONSE_NO_ERROR_0x00 = (byte) 0,
      EVENT_RX_SEARCH_TIMEOUT_0x01 = (byte) 1,
      EVENT_RX_FAIL_0x02 = (byte) 2,
      EVENT_TX_0x03 = (byte) 3,
      EVENT_TRANSFER_RX_FAILED_0x04 = (byte) 4,
      EVENT_TRANSFER_TX_COMPLETED_0x05 = (byte) 5,
      EVENT_TRANSFER_TX_FAILED_0x06 = (byte) 6,
      EVENT_CHANNEL_CLOSED_0x07 = (byte) 7,
      EVENT_RX_FAIL_GO_TO_SEARCH_0x08 = (byte) 8,
      EVENT_CHANNEL_COLLISION_0x09 = (byte) 9,
      EVENT_TRANSFER_TX_START_0x0A = (byte) 10,
      EVENT_CHANNEL_ACTIVE_0x0F = (byte) 15,
      EVENT_TRANSFER_TX_COMPLETED_RSSI_0x10 = (byte) 16,
      EVENT_TRANSFER_TX_NEXT_MESSAGE_0x11 = (byte) 17,
      CHANNEL_IN_WRONG_STATE_0x15 = (byte) 21,
      CHANNEL_NOT_OPENED_0x16 = (byte) 22,
      CHANNEL_ID_NOT_SET_0x18 = (byte) 24,
      CLOSE_ALL_CHANNELS_0x19 = (byte) 25,
      TRANSFER_IN_PROGRESS_0x1F = (byte) 31,
      TRANSFER_SEQUENCE_NUMBER_ERROR_0x20 = (byte) 32,
      TRANSFER_IN_ERROR_0x21 = (byte) 33,
      TRANSFER_BUSY_0x22 = (byte) 34,
      MESSAGE_SIZE_EXCEEDS_LIMIT_0x27 = (byte) 39,
      INVALID_MESSAGE_0x28 = (byte) 40,
      INVALID_NETWORK_NUMBER_0x29 = (byte) 41,
      INVALID_LIST_ID_0x30 = (byte) 48,
      INVALID_SCAN_TX_CHANNEL_0x31 = (byte) 49,
      INVALID_RSSI_THRESHOLD_0x32 = (byte) 50,
      INVALID_PARAMETER_PROVIDED_0x33 = (byte) 51,
      EVENT_QUE_OVERFLOW_0x35 = (byte) 53,
      EVENT_CLK_ERROR_0x36 = (byte) 54,
      SCRIPT_FULL_ERROR_0x40 = (byte) 64,
      SCRIPT_WRITE_ERROR_0x41 = (byte) 65,
      SCRIPT_INVALID_PAGE_ERROR_0x42 = (byte) 66,
      SCRIPT_LOCKED_ERROR_0x43 = (byte) 67,
      NO_RESPONSE_MESSAGE_0x50 = (byte) 80,
      RETURN_TO_MFG_0x51 = (byte) 81,
      FIT_ACTIVE_SEARCH_TIMEOUT_0x60 = (byte) 96,
      FIT_WATCH_PAIR_0x61 = (byte) 97,
      FIT_WATCH_UNPAIR_0x62 = (byte) 98,
      USB_STRING_WRITE_FAIL_0x70 = (byte) 112,
    }

    public enum USB_DescriptorString : byte
    {
      USB_DESCRIPTOR_VID_PID,
      USB_DESCRIPTOR_MANUFACTURER_STRING,
      USB_DESCRIPTOR_DEVICE_STRING,
      USB_DESCRIPTOR_SERIAL_STRING,
    }

    /// <summary>
    /// PIDs for reference
    /// 
    /// </summary>
    public enum USB_PID : ushort
    {
      ANT_INTERFACE_BOARD = (ushort) 16642,
      ANT_ARCT = (ushort) 16643,
    }
  }
}
