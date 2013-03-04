using System;
using UnityEngine;
using System.Runtime.InteropServices;
                
public class BGBodyTrackerNative : MonoBehaviour, IBGBodyTracker
{            
	struct BGBodyTrackingProperties {
        public int playerCount; //should be 1-4
        //bool trackScaleAndRotation; choice not implemented yet - always on
    };
	
	BGBodyTrackingProperties currentTrackingProperties;
	
	BGMotionManager owner;

	private bool nativeTransmitterIsRunning = false;
      
	public void SetOwner(BGMotionManager mm)
	{
		owner = mm;
	}
                
    #if UNITY_IPHONE
    [DllImport ("__Internal")]
    #else
	[DllImport ("com_activetheoryinc_sdk_lib_BitGymMotion")]
	#endif
    private static extern void StartUnityPositionTransmitter ();
    
	#if UNITY_IPHONE
    [DllImport ("__Internal")]
    #else
	[DllImport ("com_activetheoryinc_sdk_lib_BitGymMotion")]
	#endif
    private static extern void StopUnityPositionTransmitter ();
        
	#if UNITY_IPHONE
    [DllImport ("__Internal")]
    #else
	[DllImport ("com_activetheoryinc_sdk_lib_BitGymMotion")]
	#endif
    private static extern void SetBodyTrackingProperties(BGBodyTrackingProperties properties);
	
	public void EnableTracking ()
	{
		StartUnityPositionTransmitter ();
		nativeTransmitterIsRunning = true;
	}
                
	public void DisableTracking ()
	{
		StopUnityPositionTransmitter ();
		nativeTransmitterIsRunning = false;
	}
                
	public bool IsTracking ()
	{
		return nativeTransmitterIsRunning;
	}

	public void DoUpdate ()
	{
	}
                
	public void BodyPositionReading (string message)
	{
		BGBodyReading reading = new BGBodyReading (message);
		owner.PublishReading (reading, this);
	}
	
	public void SetPlayerCount(int playerCount){
		currentTrackingProperties.playerCount = playerCount;
		SetBodyTrackingProperties(currentTrackingProperties);
	}
}
