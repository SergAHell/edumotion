-------------------------------------------------------------------
README for					____, _ _____ ,____ _  _ _    _
BitGym SDK (for Android)	|___| |   |   |  __ |__| |\  /|
Active Theory, Inc.		    |___| |   |   |____|  |  | \/ |
[www.bitgym.com]
-------------------------------------------------------------------
Updated September 11th, 2012
-------------------------------------------------------------------

Setup and Building
===================================================================
Basic Requirements
-------------------------------------------------------------------
* Install Android SDK
* Get the BitGym SDK package from 
	- http://www.bitgym.com/developer
* Extract BitGym SDK into known folder on the computer


Eclipse Java project
-------------------------------------------------------------------
* Create new project
* Import SDK project "from existing code" as library project
* Link as library project
* MainActivity extends BitGymActivity
	- RegisterBodyReadingUpdateListener
	- layout.addView(mPreview);


Unity Project - All
-------------------------------------------------------------------
* Install SDK .unitypackage. This should extract 
	- PostprocessBuildPlayer into Assets -> Editor
	- BitGym into Assets -> Plugins
* Copy out the AndroidBGProject into your Unity project folder
	- NOTE: This folder cannot be placed into the Assets folder because it contains
			Assembly-Csharp-firstpass.dll, etc. which is illegal inside Assets
* Execute Android Build via Unity to stage project
	- This should create UnityPlayer in your project folder 
	- Build .apk filename here is irrelevant
	

Unity Project - As Eclipse project (recommended)
-------------------------------------------------------------------
* Import UnityPlayer "from existing code" as library project
* Import SDK project "from existing code" as library project
* Import AndroidBGProject "from existing code"
* In AndroidBGProject, link UnityPlayer and SDK projects as library projects
	- Clean any broken library links the project may have
Per Build:
* Personalize and prepare your app (if changed after the first time)
	- Refactor package name to replace default com.companyname.appname
	- Edit the AndroidManifest as needed if you need any special permissions
* Build your app! Follow instructions here for additional details
	- http://developer.android.com/tools/building/index.html


Unity Project - Without Eclipse (alpha)
-------------------------------------------------------------------
* Inside your unity project -> Assets -> Editor, rename PostprocessBuildPlayer to PostprocessBuildPlayerEclipse (or anything else), and PostprocessBuildPlayerStandalone to PostprocessBuildPlayer
[ Basically, we want to use this other script as the builder ]
* Execute Android Build via Unity. Your .apk file is UnityBGAndroid.apk, NOT whatever name you gave when Unity requested a name when you hit build.
NOTE: 	1. Currently works only for Mac.
		2. Currently requires "As Eclipse project" steps to be carried out for first-time setup 

Implementation and Samples
===================================================================
Native Java Project
-------------------------------------------------------------------
Refer to: SampleMotionGame.java, in project BitGymMotionSDK Sample
* Extend BitGymMotionActivity with your activity that uses BitGym Motion controls
* Ensure your app layout includes a FrameLayout to house the camera preview surface. OnCreate, do
code:
        FrameLayout layout = (FrameLayout) findViewById(R.id.frameLayout2);
        layout.addView(mPreview);
  NOTE: The preview has to be added to a framelayout, to activate camera (inexplicable Android thing)
* TIP: Call the optional call 'mPreview.Hide();' to effectively hide the camera preview from sight 
* Define a reading listener in your activity, similar to
code:
        bodyReadingListener = new ReadingListener<BGBodyReadingData>() {
    		public void OnNewReading(BGBodyReadingData reading) {
            	// Use the data from 'reading' as input in your app, i.e.,
            	// functionUsingX(reading.x);
            	//Log.i("ExampleListener", reading.toString());
    		}
        };
* Call RegisterBodyListener whenever you want to start tracking people and executing the listener
code:
        RegisterBodyReadingUpdateListener(bodyReadingListener);
* Call UnregisterBodyListener whenever you want to no longer execute that listener
code:
        UnregisterBodyReadingUpdateListener(bodyReadingListener);
