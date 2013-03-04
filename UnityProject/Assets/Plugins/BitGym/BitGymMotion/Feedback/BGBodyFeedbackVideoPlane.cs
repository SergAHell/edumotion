using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class BGBodyFeedbackVideoPlane : MonoBehaviour {

    private Texture2D m_Texture;
    private float fov;
		
#if UNITY_IPHONE		
    [DllImport ("__Internal")]
    public static extern void StartOpenGLBodyFeedbackRendering(int textureId);
    [DllImport ("__Internal")]
    public static extern void StopOpenGLBodyFeedbackRendering();
#else
#if UNITY_ANDROID
    [DllImport ("com_activetheoryinc_sdk_lib_BitGymMotion")]
    public static extern void StartOpenGLBodyFeedbackRendering(int textureId);
    [DllImport ("com_activetheoryinc_sdk_lib_BitGymMotion")]
    public static extern void StopOpenGLBodyFeedbackRendering();
	[DllImport ("com_activetheoryinc_sdk_lib_BitGymMotion")]
    public static extern void renderFeedbackToTexture();
#else
    public static void StartOpenGLBodyFeedbackRendering(int textureId) { }
    public static void StopOpenGLBodyFeedbackRendering() { }
#endif
#endif
	
	private Vector3 targetLocalRotation;
	private Vector3 landscapeRightRotation = new Vector3(-90.0f, 0.0f, 0.0f);
	private Vector3 landscapeLeftRotation = new Vector3(90.0f, 180.0f, 0.0f);
	
	private Vector3 androidLandscapeRightRotation = new Vector3(90.0f, 180.0f, 0.0f);
	private Vector3 androidLandscapeLeftRotation = new Vector3(-90.0f, 0.0f, 0.0f);
	private Transform myTransform;
    void Start () 
    {  	
    	myTransform = gameObject.transform;

		m_Texture = BGMotionManager.GetInstance().GetFeedbackTexture();
		m_Texture.filterMode = FilterMode.Point;
		renderer.sharedMaterial.mainTexture = m_Texture;
		
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
			
		} else {
			
			myTransform.localScale = new Vector3(-myTransform.localScale.x, myTransform.localScale.y, myTransform.localScale.z);
		}

    }
    
	private bool isRendering = false;
    void LateUpdate () 
    {
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
			//try to get video feed until we finally have it
			if(!isRendering && BGMotionManager.GetInstance().GetCurrentTracker().IsTracking()) {
				StartOpenGLBodyFeedbackRendering(m_Texture.GetNativeTextureID());
				Debug.Log("Native texture ID: " + m_Texture.GetNativeTextureID().ToString());
				isRendering = true;
			}
			#if UNITY_ANDROID
			if (Application.platform == RuntimePlatform.Android) renderFeedbackToTexture();
			#endif	
			renderer.material.mainTextureScale = new Vector2(1, 1); 
		} else {
			//renderer.material.mainTexture = m_Texture;
			renderer.sharedMaterial.mainTextureScale = new Vector2(0.625f, 0.46875f);
		}
		
		if (Screen.orientation == ScreenOrientation.LandscapeRight) {
			targetLocalRotation = (Application.platform == RuntimePlatform.Android)?androidLandscapeRightRotation:landscapeRightRotation; //
		} else {
			targetLocalRotation = (Application.platform == RuntimePlatform.Android)?androidLandscapeLeftRotation:landscapeLeftRotation; //
		}
		
		myTransform.localEulerAngles = targetLocalRotation;
    }
	
	void OnDisable(){
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
			StopOpenGLBodyFeedbackRendering();
			isRendering = false;
		}
	}
}
