// Type: ANT_Managed_Library.dDeviceNotificationHandler
// Assembly: ANT_NET, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Users\Alex\Desktop\SomaxisAfeDiagnosticMonitor_20120602_V1_0_1_1\SomaxisAfeDiagnosticMonitor_20120602_V1_0_1_1\ANT_NET.dll

namespace ANT_Managed_Library
{
  /// <summary>
  /// Delegate for the DeviceNotification event
  /// 
  /// </summary>
  /// <param name="notification">The notification code for the current event</param><param name="notificationInfo">An object that optionally holds more information about the current event</param>
  public delegate void dDeviceNotificationHandler(ANT_Device.DeviceNotificationCode notification, object notificationInfo);
}
