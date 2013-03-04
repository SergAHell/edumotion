package com.companyname.appname;

/*
 * @author: keerthik
 * @company: activetheoryinc
 * /!\ NOTE: There is an OpenGL implementation bug in the
 * AVD (Emulated Android Virtual Device) system, hence
 * Unity apps will not work via emulator. Must be debugged
 * on device.
 */
import java.util.Timer;
import java.util.TimerTask;

import android.os.Bundle;
import android.os.Handler;
import android.util.Log;
import android.view.Menu;
import android.view.Window;
import android.view.WindowManager;
import android.widget.FrameLayout;

import com.activetheoryinc.sdk.lib.BGBodyReadingData;
import com.activetheoryinc.sdk.lib.BitGymMotion;
import com.activetheoryinc.sdk.lib.BitGymMotionActivity;
import com.activetheoryinc.sdk.lib.ReadingListener;
import com.unity3d.player.UnityPlayer;

public class UnityBitGymActivity extends BitGymMotionActivity {
	private UnityPlayer m_UnityPlayer;
	private Timer timer;
	private boolean _unityListenerRegistered = false;
	private ReadingListener<BGBodyReadingData> unityBodyReadingListener;
	
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        // It's a game: Full-screen
        requestWindowFeature(Window.FEATURE_NO_TITLE);
        getWindow().addFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN);
        
        // Create the UnityPlayer
        m_UnityPlayer = new UnityPlayer(this);
        int glesMode = m_UnityPlayer.getSettings().getInt("gles_mode", 1);
        boolean trueColor8888 = false;
        m_UnityPlayer.init(glesMode, trueColor8888);

        /*
         * Register this listener only when unity has at least one registered listener;
         * This means we need to implement a polling system to C++ to look for a Unity-set
         * variable's value to determine whether this needs to be added, as follows
         *  Unity requests listener -> make native C call StartTracking() -> sets global C
         *  variable 'unityHasListener' /==/ Java polls C via JNI for unityHasListener value -> 
         *  if true, registers unityBodyReadingListener
         * This has been implemented in BitGymSDK/Preview
         */
        
        unityBodyReadingListener = new ReadingListener<BGBodyReadingData>() {
    		public void OnNewReading(BGBodyReadingData reading) {
    			//Log.v("UnityMessage", reading!=null?reading.toString():"");
    			// Send the message to Unity for the BG manager to interpret
    			UnityPlayer.UnitySendMessage("nativeTrackerContainer", "BodyPositionReading", 
                    reading!=null?reading.toString():"");
    		}
        };
        
        setContentView(R.layout.activity_main);
        // Set up some bitgym stuff
        FrameLayout layout = (FrameLayout) findViewById(R.id.frameLayout2);
        mPreview.Hide();
        layout.addView( mPreview);

        // Set up some unity stuff
        FrameLayout unityLayout= (FrameLayout) findViewById(R.id.frameLayout1);    
        unityLayout.addView(m_UnityPlayer.getView());
        checkUnityListeners();
        // It's a game: don't sleep
        unityLayout.setKeepScreenOn(true);        
    }

    public void checkUnityListeners (){
    	timer = new Timer();
    	final Handler handler = new Handler ();
    	timer.scheduleAtFixedRate (new TimerTask (){
            public void run (){
            	handler.post (new Runnable (){
                    public void run (){
                    	// Check with C++ for unity listeners
		        		if (BitGymMotion.BGUnityHasListener() && !_unityListenerRegistered) {
		        			RegisterBodyReadingUpdateListener(unityBodyReadingListener);
		        			_unityListenerRegistered = true;
		        			Log.v("ListenerCheck", "Adding in Unity Listener");
		        		}
		        		if (!BitGymMotion.BGUnityHasListener() && _unityListenerRegistered) {
		        			UnregisterBodyReadingUpdateListener(unityBodyReadingListener);
		        			_unityListenerRegistered = false;
		        		}
                    }
            	});
            }
        }, 0, (long) 400.0);
    }

    public void onPause() {
    	super.onPause();
    	m_UnityPlayer.pause();
    }
    
    public void onResume() {
    	super.onResume();
    	m_UnityPlayer.resume();
    }
    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        //getMenuInflater().inflate(R.menu.activity_main, menu);
        return true;
    }
}
