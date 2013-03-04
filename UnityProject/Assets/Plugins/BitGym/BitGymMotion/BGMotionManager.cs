using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;



public class BGMotionManager : MonoBehaviour, IBGBodyTrackerManager
{
	
	private HashSet<IBGBodyReadingListener> listeners = new HashSet<IBGBodyReadingListener> ();
	private bool useWebcamInEditor = true;
	private IBGBodyTracker currentTracker;
	private IBGBodyTracker bodyTrackerMono;
	private IBGBodyTracker bodyTrackerNative;
	private IBGBodyTracker bodyTrackerKeyboard;
	
	private static object _lock = new object(); //used for locking
	private static BGMotionManager instance;
	private static GameObject container;
	private static GameObject monoTrackerContainer;
	private static GameObject nativeTrackerContainer;
	private static int playerCount = 1;
	private static Texture2D feedbackTexture;
	
	
	public static BGMotionManager GetInstance () {
		if (instance == null) {
			print("getting lock");
			lock (_lock) {
				if (instance == null) {
					container = new GameObject ("BGMotionManager");
					instance = container.AddComponent (typeof(BGMotionManager)) as BGMotionManager;
					
					instance.bodyTrackerKeyboard = new BGBodyTrackerKeyboard (instance);
					
					monoTrackerContainer = new GameObject("monoTrackerContainer");
					print("made mono tracker");
					instance.bodyTrackerMono = monoTrackerContainer.AddComponent(typeof(BGBodyTrackerMono)) as BGBodyTrackerMono;
					((instance.bodyTrackerMono) as BGBodyTrackerMono).SetOwner(instance);
					
					nativeTrackerContainer = new GameObject("nativeTrackerContainer");
					instance.bodyTrackerNative = nativeTrackerContainer.AddComponent(typeof(BGBodyTrackerNative)) as BGBodyTrackerNative;
					((instance.bodyTrackerNative) as BGBodyTrackerNative).SetOwner(instance);
					
					feedbackTexture = new Texture2D(256, 256, TextureFormat.ARGB32, false);
					DontDestroyOnLoad (container);
					DontDestroyOnLoad (nativeTrackerContainer);
					DontDestroyOnLoad (monoTrackerContainer);
				}
			}
		}
		return instance;
	}
	
	private BGMotionManager ()
	{
		//print ("Creating BGMotionManager");
	}

	public void Start ()
	{
		//print ("BGMotionManager Start()");
		SetupTracker();
	}

	public void AddBodyReadingListener (IBGBodyReadingListener listener)
	{
		listeners.Add (listener);
	}

	public void RemoveBodyReadingListener (IBGBodyReadingListener listener)
	{
		listeners.Remove (listener);
	}

	public void SetEditorKeyboardMode ()
	{
		useWebcamInEditor = false;
		SetupTracker ();
	}

	public void SetEditorWebcamMode ()
	{
		useWebcamInEditor = true;
		SetupTracker ();
	}

	private void SetupTracker ()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android) {
			currentTracker = bodyTrackerNative;
			//bodyTrackerNative.EnableTracking ();
		} else if (useWebcamInEditor) {
			currentTracker = bodyTrackerMono;
			//bodyTrackerMono.EnableTracking ();
			bodyTrackerKeyboard.DisableTracking ();
		} else {
			currentTracker = bodyTrackerKeyboard;
			bodyTrackerMono.DisableTracking ();
			//bodyTrackerKeyboard.EnableTracking ();
		}
		currentTracker.SetPlayerCount(playerCount);
	}
	
	public IBGBodyTracker GetCurrentTracker() {
		return currentTracker;
	}
	
	public Texture2D GetFeedbackTexture(){
		return feedbackTexture;
	}
	
	void Update ()
	{
		if(listeners.Count == 0 && currentTracker.IsTracking()){
			currentTracker.DisableTracking();
		} else if (listeners.Count != 0 && !currentTracker.IsTracking()){
			currentTracker.EnableTracking();
		}
		currentTracker.DoUpdate();
	}

	void OnLevelWasLoaded(int level){
		listeners.Clear(); 
		print("cleared listeners on level load");
	}
	
	public void PublishReading (BGBodyReading reading, IBGBodyTracker tracker)
	{
		if (tracker != currentTracker) {
			print ("Warning: PublishReading cannot be called from a IBGBodyTracker who is not the current tracker.");
			return;
		}
		foreach (IBGBodyReadingListener listener in listeners) {
			listener.HandleReading (reading);
		}
	}
	
	public void SetPlayerCount(int _playerCount){
		if(_playerCount < 0 || _playerCount > 2){
			print("Player count must be either 1 or 2! " + _playerCount + " is not a valid setting. Not changing player count.");
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