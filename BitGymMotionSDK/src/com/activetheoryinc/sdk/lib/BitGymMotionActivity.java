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

/*
 * @author: keerthik
 * This activity is an abstract activity that any activity using
 * BitGym tech will want to inherit from. This activity will be
 * the only BitGym-inherited activity that can run at a given time.
 * It will be holding the lock on the camera and be managing a bunch
 * of listeners. Apps with multi-activity-architecture should use
 * one activity that inherits from BitGymActivity and is always kept
 * live: this means 
 */

package com.activetheoryinc.sdk.lib;

import java.util.ArrayList;

import javax.microedition.khronos.egl.EGLConfig;
import javax.microedition.khronos.opengles.GL10;
import javax.microedition.khronos.opengles.GL11;
import javax.microedition.khronos.opengles.GL11Ext;

import android.app.Activity;
import android.content.res.Configuration;
import android.graphics.ImageFormat;
import android.hardware.Camera;
import android.hardware.Camera.PreviewCallback;
import android.opengl.GLSurfaceView;
import android.opengl.GLU;
import android.os.Bundle;
import android.util.Log;
import android.view.Display;


// ----------------------------------------------------------------------

public abstract class BitGymMotionActivity extends Activity {
    protected BitGymPreview mPreview;
    protected GLSurfaceView mFeedback;
    protected int filename;
    
    Camera mCamera;
    byte[] cameraBuffer;
    byte[] previewBuffer1, previewBuffer2;
    byte[] oldBuffer, currentBuffer;
    private boolean _locked = false;
    private final static int BGWidth = 160;
    private final static int BGHeight = 120;
    private static int camWidth;
    private static int camHeight;
    ArrayList<ReadingListener<BGBodyReadingData>> listeners = new ArrayList<ReadingListener<BGBodyReadingData>> ();
    
    protected ReadingListener<BGBodyReadingData> bodyReadingListener;
        
    public void RegisterBodyReadingUpdateListener (ReadingListener<BGBodyReadingData> listener) {
    	Log.v("BitGymListeners", "Registering a listener");
    	if (listeners.isEmpty()) StartVisualTracking();
    	this.listeners.add(listener);
    }

    public void UnregisterBodyReadingUpdateListener (ReadingListener<BGBodyReadingData> listener) {
    	this.listeners.remove(listener);
    	if(listeners.isEmpty()) StopVisualTracking();
    }
            
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        
        // Initialize the BitGym tracking system
        BitGymMotion.BGInitPerceptual(	"com/activetheoryinc/sdk/lib/BGBodyReadingData",
				BGWidth, BGHeight);
		
        // Game purpose keep screen awake
        mPreview = new BitGymPreview(this);

        mFeedback = new GLSurfaceView(this);
        mFeedback.setRenderer(new FeedbackRenderer());
        mFeedback.setRenderMode(GLSurfaceView.RENDERMODE_WHEN_DIRTY);
         
        cameraBuffer = null;
    }
    
    public void SetPlayerCount(int pcount) {
    	BitGymMotion.BGSetPlayerCount(pcount);
    }
    
    private void StartVisualTracking() {
    	mCamera = BitGymCameraController.GetInstance().openFrontFacingCamera();
        if (mCamera != null) {
	        mPreview.switchCamera(mCamera);
	        camWidth = mCamera.getParameters().getPreviewSize().width;
	        camHeight = mCamera.getParameters().getPreviewSize().height;
        	// Doing the fancy memory-efficient buffering
        	if (cameraBuffer == null) {
        		cameraBuffer = new byte[(mCamera.getParameters().getPreviewSize().height *
        					mCamera.getParameters().getPreviewSize().width *
        					ImageFormat.getBitsPerPixel(mCamera.getParameters().getPreviewFormat()))/8];
	        	previewBuffer1 = new byte[BGWidth*BGHeight];
	        	previewBuffer2 = new byte[BGWidth*BGHeight];
        	}
        	oldBuffer = null;
        	currentBuffer = previewBuffer1;
	        mCamera.addCallbackBuffer(cameraBuffer);
	        
	        mCamera.setPreviewCallbackWithBuffer(new PreviewCallback() {
	            public void onPreviewFrame(byte[] data, Camera camera) {
	            	/* TODO: Fix silent death 
	            	 * This operation causes 'silent death' on some devices
	            	 * Symptoms: 
	            	 * * Application closes after about 2 camera frames are processed 
	            	 * * There is no stack trace or force close warning
	            	 * Potentially Problematic Platforms:
	            	 * * HTC One X
	            	 * * Nexus One
	            	 * * Samsung Galaxy Tab P1000
	            	 * Known Functional Platforms:
	            	 * * Nexus7
	            	 * * Samsung Galaxy Nexus
	            	 * * Motorola Photon 4G
	            	 * * HTC Incredible 2
	            	 * * Samsung Galaxy Note
	            	 */
	            	
	            	if (camera!=null) {
	                	// Do some cool downsampling and Y-plane extraction
                		float dx = (float)camWidth/(float)BGWidth;
            			float dy = (float)camHeight/(float)BGHeight;

            			for (int x = 0; x < BGWidth; x++) {
            				for (int y = 0; y < BGHeight; y++) {
            					int i = x + y * BGWidth;
            					int loc = (int)(x * dx) + (int)(y * dy) * camWidth;
            					currentBuffer [i] = data [loc];
            				}
            			}
            			
	                	// generate the new reading or update it or whatever
	                	if (oldBuffer != null && !_locked) {
	                		_locked = true;
	                		BitGymMotion.BGProcessVideoFrame(currentBuffer, oldBuffer);
	                		_locked = false;
	                	}
	                	mFeedback.requestRender();
	                	// This is looking for device orientation, not app orientation
	                	// 0 - Landscape cam up, 1 - Portrait cam left, 2 - Landscape cam down, 3 - Portrait cam right
	                	int _orientation = getScreenOrientation(); 
	                	BitGymMotion.BGSetDeviceOrientation(_orientation);
	                	
	                	for (int i = 0; i < BitGymMotion.BGGetPlayerCount(); i++) {
	                		BGBodyReadingData reading = BitGymMotion.BGGetBodyReadingData(i);
		                	// TODO: Do some smart thing to decide whether to actually do a callback or not
		                	
		                	if (!_locked) {
		                		// TODO: This portion should be thread managed, not UI-blocking
		                		_locked = true;
		                		if (!BitGymMotionActivity.this.listeners.isEmpty()) {
		                			for (ReadingListener<BGBodyReadingData> listener : BitGymMotionActivity.this.listeners)
		                				listener.OnNewReading(reading);
		                		}
		                		_locked = false;
		                		// Buffer management
		                		oldBuffer = currentBuffer;
		                		currentBuffer = (currentBuffer == previewBuffer1)? previewBuffer2:previewBuffer1;
		                	}
	                	}
	                	if (mCamera!=null) camera.addCallbackBuffer(cameraBuffer);
	                }
	                
	            }  
	        });
        } else Log.e("BitGym", "Creation failed!");

    }

    private void StopVisualTracking() {
        if (mCamera != null) {
            mPreview.EndPreview();
            BitGymCameraController.Close();
            mCamera = null;
        }
    }

    @Override
    protected void onResume() {
        super.onResume();
        mFeedback.onResume();
        // Open the default i.e. the first rear facing camera.
        if (!listeners.isEmpty() && mCamera == null) StartVisualTracking();
    }

    @Override
    protected void onPause() {
    	super.onPause();
        mFeedback.onPause();
        // Because the Camera object is a shared resource, it's very
        // important to release it when the activity is paused.
    	StopVisualTracking();
    }
    
    @SuppressWarnings("deprecation")
	public int getScreenOrientation()
    {
        Display getOrient = getWindowManager().getDefaultDisplay();
        int orientation = Configuration.ORIENTATION_UNDEFINED;
        if(getOrient.getWidth() < getOrient.getHeight()){
            orientation = Configuration.ORIENTATION_PORTRAIT;
        }else { 
             orientation = Configuration.ORIENTATION_LANDSCAPE;
        }
        return orientation;
    }
}

class FeedbackRenderer implements GLSurfaceView.Renderer {
	
	private final int[] cropParams;
	
	public FeedbackRenderer() {
		cropParams = new int[4];
        
	}
	public void onSurfaceCreated(GL10 gl, EGLConfig config) {
		gl.glHint(GL10.GL_PERSPECTIVE_CORRECTION_HINT, GL10.GL_FASTEST);
        gl.glEnable(GL10.GL_TEXTURE_2D);
        
        gl.glClear(GL10.GL_COLOR_BUFFER_BIT);
        
		// Start Open GL rendering
		BitGymMotion.BGStartFeedbackRender();
        
        cropParams[0] = 0;
        cropParams[1] = 120;
        cropParams[2] = 160;
        cropParams[3] = -120;
        ((GL11)gl).glTexParameteriv(GL10.GL_TEXTURE_2D, GL11Ext.GL_TEXTURE_CROP_RECT_OES, cropParams, 0);
        Log.v("BitGymFeedback","Feedback Parameters set");
        ((GL11Ext)gl).glDrawTexfOES(100, 20, 0f, 640, 480);
		
	}

	public void onDrawFrame(GL10 gl) {
		gl.glClear(GL10.GL_COLOR_BUFFER_BIT | GL10.GL_DEPTH_BUFFER_BIT);
		// Pipe the feedback bitmap onto this textureId
		BitGymMotion.BGRenderToTexture();
		((GL11Ext)gl).glDrawTexfOES(100, 20, 0f, 640, 480);
	}

	public void onSurfaceChanged(GL10 gl, int width, int height) {
		gl.glViewport(0, 0, width, height);
		gl.glMatrixMode(GL10.GL_PROJECTION);
		gl.glLoadIdentity();
		GLU.gluOrtho2D(gl, 0, width, 0, height);
	}	
}
