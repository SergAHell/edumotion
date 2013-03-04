using UnityEngine;
using System.Collections;
using System;

public class BGBodyTrackerMono :  MonoBehaviour, IBGBodyTracker
{

	IBGBodyTrackerManager owner;
	bool monoBodyTrackerIsRunning = false;
	private bool isSetup;
	PerceptualHub perceptualHub;
	const int frameWidth = 160;
	const int frameHeight = 120;
	const int desiredCamWidth = 160;
	const int desiredCamHeight = 160;
	int camWidth;
	int camHeight;
	
	public void SetOwner(IBGBodyTrackerManager mm)
	{
		owner = mm;
		isSetup = false;
	}
       
    //we have this setup pattern because we can't construct a perceptual hub in the singleton ctor in MotionManager which usually constructs this class. 
	public void DoSetup ()
	{
		perceptualHub = new PerceptualHub (frameWidth, frameHeight, owner.GetFeedbackTexture());     
		perceptualHub.SetPlayerCount(owner.GetPlayerCount());
		isSetup = true;
		frameBuff1 = new byte[frameWidth*frameHeight];
		frameBuff2 = new byte[frameWidth*frameHeight];
		lastFrame = frameBuff1;
		thisFrame = frameBuff2;
	}
       
    public bool IsTracking(){
    	return monoBodyTrackerIsRunning;
    }
    
	public void EnableTracking ()
	{
		if(!monoBodyTrackerIsRunning){
			monoBodyTrackerIsRunning = true;
			StartCoroutine("StartWebCam");
		}
	}
        
	public void DisableTracking ()
	{
		StopWebCam ();
		monoBodyTrackerIsRunning = false;
	}
	
    //this is to turn off the camera on level load because the webcam texture 
	//doesn't seem to work between level loads. it will be turned back on automagically 
	//if there are any listeners!

    void OnDisable() {
		print("disable");
		if(monoBodyTrackerIsRunning){
			print("disabled, tearing down webcamTexture");
			DisableTracking(); 
		}
	}

	
	public void SetPlayerCount(int playerCount){
		if(perceptualHub != null)
			perceptualHub.SetPlayerCount(playerCount);
		//if no hub instantiated yet, we'll set the player count in DoSetup()
	}
	

	
	private BGBodyReading lastBodyReading;
	private WebCamTexture webcamTexture;
	private int frameCount = 0;
	private Color32[] camFrame;
	private byte[] lastFrame, thisFrame;
	private byte[] frameBuff1, frameBuff2;
	float fps = 0f;
	float lastFrameTime = 0f;
        
	private IEnumerator StartWebCam ()
	{
		yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
		if (Application.HasUserAuthorization (UserAuthorization.WebCam)) {
			Debug.Log ("WebCam Authorized");
			webcamTexture = new WebCamTexture (desiredCamWidth, desiredCamHeight, 24);
			webcamTexture.Play ();
			camWidth = webcamTexture.width;
			camHeight = webcamTexture.height;
			print("Created webcamtexture with desired dimensions: " + desiredCamWidth + ","+desiredCamHeight + " and got texture with dimensions: " + camWidth + "," + camHeight);
		} else {
			Debug.Log ("WebCam permissions not authorized!");
		}
	}
        
	private void StopWebCam ()
	{
		if (webcamTexture != null) {
			webcamTexture.Stop ();
			webcamTexture = null;
		}
	}
        
	public void DoUpdate ()
	{
		if (!isSetup)
			DoSetup ();
		if (webcamTexture != null && webcamTexture.didUpdateThisFrame) {
			frameCount++;
			if (frameCount > 1) {
				//lastFrame = thisFrame;
				fps = 0.9f * fps + 0.1f / (Time.time - lastFrameTime);
			}                
			lastFrameTime = Time.time;
			// Copy new frame from camera with transform
			camFrame = webcamTexture.GetPixels32 ();
			float dx = (float)camWidth/(float)frameWidth;
			float dy = (float)camHeight/(float)frameHeight;

			for (int x = 0; x < frameWidth; x++) {
				for (int y = 0; y < frameHeight; y++) {
					int i = x + y * frameWidth;
					int loc = (int)(x * dx) + (int)(y * dy) * camWidth;
					thisFrame [i] = camFrame[loc].g;
				}
			}
			if (lastFrame != null) {
				perceptualHub.ProcessVideoFrame (thisFrame, lastFrame);
				// Swap buffers
				lastFrame = (lastFrame == frameBuff1)?frameBuff2:frameBuff1;
				thisFrame = (thisFrame == frameBuff1)?frameBuff2:frameBuff1;
				for (int i = 0; i < perceptualHub.GetPlayerCount(); i++) {
					owner.PublishReading (perceptualHub.GetBodyReadingForPlayer(i), this);
				}
			}
		}
	}
       
	
}