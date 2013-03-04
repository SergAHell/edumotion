// Type: ANT_Managed_Library.ANT_DeviceInfo
// Assembly: ANT_NET, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Users\Alex\Desktop\SomaxisAfeDiagnosticMonitor_20120602_V1_0_1_1\SomaxisAfeDiagnosticMonitor_20120602_V1_0_1_1\ANT_NET.dll

using System.Text;

namespace ANT_Managed_Library
{
  /// <summary>
  /// Container for all the USB Device information, returned from an ANTDevice
  /// 
  /// </summary>
  public class ANT_DeviceInfo
  {
    /// <summary>
    /// USB Device Product Description
    /// 
    /// </summary>
    public byte[] productDescription;
    /// <summary>
    /// USB Device Serial String
    /// 
    /// </summary>
    public byte[] serialString;

    internal ANT_DeviceInfo(byte[] productDescription, byte[] serialString)
    {
      this.productDescription = productDescription;
      this.serialString = serialString;
    }

    /// <summary>
    /// Returns a formatted, readable string for the product description
    /// 
    /// </summary>
    public string printProductDescription()
    {
      return this.printBytes(this.productDescription);
    }

    /// <summary>
    /// Returns a formatted, readable string for the serial string
    /// 
    /// </summary>
    public string printSerialString()
    {
      return this.printBytes(this.serialString);
    }

    private string printBytes(byte[] rawBytes)
    {
      string @string = Encoding.ASCII.GetString(rawBytes);
      return @string.Remove(@string.IndexOf(char.MinValue));
    }
  }
}
