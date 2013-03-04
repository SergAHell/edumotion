using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;



public class SomaxisManager : MonoBehaviour, ISomaxisTrackerManager
{
	
	private HashSet<ISomaxisReadingListener> listeners = new HashSet<ISomaxisReadingListener> ();
	private bool useWebcamInEditor = true;
	private ISomaxisTracker currentTracker;
	private ISomaxisTracker trackerMock;
	private ISomaxisTracker trackerNative;
	
	private static object _lock = new object(); //used for locking
	private static SomaxisManager instance;
	private static GameObject container;
	private static GameObject mockTrackerContainer;
	private static GameObject nativeTrackerContainer;
	private static int playerCount = 1;
	private static Texture2D feedbackTexture;
	
	public static bool isMockOn = true;
	
	public static SomaxisManager GetInstance () {
		if (isMockOn == null) isMockOn = false;
		if (instance == null) {
			Debug.Log("getting lock");
			lock (_lock) {
				if (instance == null) {
					container = new GameObject ("SomaxisManager");
					instance = container.AddComponent (typeof(SomaxisManager)) as SomaxisManager;
					
					mockTrackerContainer = new GameObject("mockTrackerContainer");
					Debug.Log("made mock tracker");
					instance.trackerMock = mockTrackerContainer.AddComponent(typeof(SomaxisTrackerMock)) as SomaxisTrackerMock;
					((instance.trackerMock) as SomaxisTrackerMock).SetOwner(instance);
					
					nativeTrackerContainer = new GameObject("nativeTrackerContainer");
					instance.trackerNative = nativeTrackerContainer.AddComponent(typeof(SomaxisTrackerNative)) as SomaxisTrackerNative;
					((instance.trackerNative) as SomaxisTrackerNative).SetOwner(instance);
					
					feedbackTexture = new Texture2D(256, 256, TextureFormat.ARGB32, false);
					DontDestroyOnLoad (container);
					DontDestroyOnLoad (nativeTrackerContainer);
					DontDestroyOnLoad (mockTrackerContainer);
				}
			}
		}
		return instance;
	}
	
	public void Start ()
	{
		Debug.Log ("SomaxisManager Start()");
		SetupTracker();
	}

	public void AddReadingListener (ISomaxisReadingListener listener)
	{
		listeners.Add (listener);
	}

	public void RemoveReadingListener (ISomaxisReadingListener listener)
	{
		listeners.Remove (listener);
	}

	private void SetupTracker ()
	{
		//if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android) {
		if (!isMockOn) {
			currentTracker = trackerNative;
			trackerMock.DisableTracking ();
			trackerNative.EnableTracking ();
		} else {
			currentTracker = trackerMock;
			trackerMock.EnableTracking ();
			trackerNative.DisableTracking ();
		}
		currentTracker.SetPlayerCount (playerCount);
	}
	
	public ISomaxisTracker GetCurrentTracker() {
		return currentTracker;
	}
	
	public Texture2D GetFeedbackTexture(){
		return feedbackTexture;
	}
	
	void Update ()
	{
		if (listeners.Count == 0 && currentTracker.IsTracking ()) { 
			currentTracker.DisableTracking ();
		} else if (listeners.Count != 0 && !currentTracker.IsTracking ()) { 
			currentTracker.EnableTracking ();
		}
		currentTracker.DoUpdate ();
		
		if (Input.GetKeyDown (KeyCode.M)) {
			isMockOn = !isMockOn;
			Debug.Log ("Mock is " + (isMockOn ? "On" : "Off"));
			SetupTracker ();
		}

	}

	void OnLevelWasLoaded(int level){
		listeners.Clear(); 
		print("cleared listeners on level load");
	}
	
	public void PublishReading (SomaxisReading reading, ISomaxisTracker tracker)
	{
		if (tracker != currentTracker) {
			Debug.LogWarning ("Warning: PublishReading cannot be called from a ISomaxisTracker who is not the current tracker.");
			return;
		}
		foreach (ISomaxisReadingListener listener in listeners) {
			listener.HandleReading (reading);
		}
	}
	
	public void SetPlayerCount(int _playerCount){
		if(_playerCount < 0 || _playerCount > 2){
			Debug.LogError("Player count must be either 1 or 2! " + _playerCount + " is not a valid setting. Not changing player count.");
			return;
		}
		playerCount = _playerCount;
		if(currentTracker!=null)
			currentTracker.SetPlayerCount(playerCount);
	}
	public int GetPlayerCount(){
		return playerCount;
	}
	
}