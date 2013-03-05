/*
 Copyright 2011-2012 Active Theory Inc. All rights reserved.
 
 Redistribution and use in source and binary forms, with or without
 modification, is not permitted.
 
 THIS SOFTWARE IS PROVIDED BY THE ACTIVE THEORY INC``AS IS'' AND ANY EXPRESS OR
 IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
 MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO
 EVENT SHALL ACTIVE THEORY INC OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
 INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
 BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
 LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE
 OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
 ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
package com.activetheoryinc.sdk.lib;

/*
 */
import java.lang.reflect.Field;
import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;

import android.hardware.Camera;
import android.util.Log;

public class BitGymCameraController {
	/**
	 * Open the camera.  First attempt to find and open the front-facing camera.
	 * If that attempt fails, then fall back to whatever camera is available.
	 * 
	 * @return a Camera object
	 */
	// Singleton mode
	private static BitGymCameraController instance = null;
	private BitGymCameraController() {}
	public static BitGymCameraController GetInstance() {
		if (instance == null)
			instance = new BitGymCameraController();
		return instance;
	}
	
	private Camera camera = null;
	
	public Camera openFrontFacingCamera() {
	    String TAG = "BitGymCamera";
	    // Look for front-facing camera, using the Gingerbread API.
	    // Java reflection is used for backwards compatibility with pre-Gingerbread APIs.
	    try {
	        Class<?> cameraClass = Class.forName("android.hardware.Camera");
	        Object cameraInfo = null;
	        Field field = null;
	        int cameraCount = 0;
	        Method getNumberOfCamerasMethod = cameraClass.getMethod( "getNumberOfCameras" );
	        if ( getNumberOfCamerasMethod != null ) {
	            cameraCount = (Integer) getNumberOfCamerasMethod.invoke( null, (Object[]) null );
	        }
	        Class<?> cameraInfoClass = Class.forName("android.hardware.Camera$CameraInfo");
	        if ( cameraInfoClass != null ) {
	            cameraInfo = cameraInfoClass.newInstance();
	        }
	        if ( cameraInfo != null ) {
	            field = cameraInfo.getClass().getField( "facing" );
	        }
	        Method getCameraInfoMethod = cameraClass.getMethod( "getCameraInfo", Integer.TYPE, cameraInfoClass );
	        if ( getCameraInfoMethod != null && cameraInfoClass != null && field != null ) {
	            for ( int camIdx = 0; camIdx < cameraCount; camIdx++ ) {
	                getCameraInfoMethod.invoke( null, camIdx, cameraInfo );
	                int facing = field.getInt( cameraInfo );
	                if ( facing == 1 ) { // Camera.CameraInfo.CAMERA_FACING_FRONT
	                    try {
	                        Method cameraOpenMethod = cameraClass.getMethod( "open", Integer.TYPE );
	                        if ( cameraOpenMethod != null ) {
	                            camera = (Camera) cameraOpenMethod.invoke( null, camIdx );
	                        }
	                    } catch (RuntimeException e) {
	                        Log.e(TAG, "Camera failed to open: " + e.getLocalizedMessage());
	                    }
	                }
	            }
	        }
	    }
	    // Ignore the bevy of checked exceptions the Java Reflection API throws - if it fails, who cares.
	    
	    catch ( ClassNotFoundException e        ) {Log.e(TAG, "ClassNotFoundException" + e.getLocalizedMessage());}
	    catch ( NoSuchMethodException e         ) {Log.e(TAG, "NoSuchMethodException" + e.getLocalizedMessage());}
	    catch ( NoSuchFieldException e          ) {Log.e(TAG, "NoSuchFieldException" + e.getLocalizedMessage());}
	    catch ( IllegalAccessException e        ) {Log.e(TAG, "IllegalAccessException" + e.getLocalizedMessage());}
	    catch ( InvocationTargetException e     ) {Log.e(TAG, "InvocationTargetException" + e.getLocalizedMessage());}
	    catch ( InstantiationException e        ) {Log.e(TAG, "InstantiationException" + e.getLocalizedMessage());}
	    catch ( SecurityException e             ) {Log.e(TAG, "SecurityException" + e.getLocalizedMessage());}
	 
	    if ( camera == null ) {
	        // Try using the pre-Gingerbread APIs to open the camera.
	        try {
	            camera = Camera.open();
	        } catch (RuntimeException e) {
	            Log.e(TAG, "Camera failed to open: " + e.getLocalizedMessage());
	        }
	    }
	    
	    return camera;
	}
	

	public Camera GetCamera() {
		return camera;
	}
	
	public void CloseCamera() {
		if (camera != null) camera.release();
		camera = null;
	}
	public static void Close() {
		if (instance!= null) GetInstance().CloseCamera();
		instance = null;
	}
	
}


