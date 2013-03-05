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
 * This file is the java bridge to the NDK C++ stuff.
 * Define native signatures for all the C++ function calls
 * you want to provide to BitGymSDK subscribers out of the
 * BitGymSDK.h/cpp listings
 */

package com.activetheoryinc.sdk.lib;

public class BitGymMotion {
	public static int AverageColor(byte[] data) {
		return data[0];
	}
	
	//Signature for JNI testing function
	public native static int BGAverageColor(byte[] data);
	
	// Signatures for Awesome BitGym Functions!
	public native static boolean BGUnityHasListener();
	public native static void BGSetPlayerCount(int newPlayerCount);
	public native static int BGGetPlayerCount();
	public native static void BGInitPerceptual(String readingClass, int _frameWidth, int _frameHeight);
	public native static void BGProcessVideoFrame(byte[] greyscaleImage, byte[] previousGreyscaleImage);
	public native static BGBodyReadingData BGGetBodyReadingData(int player);
	public native static void BGSetDeviceOrientation(int orient);
	public native static void BGRenderToTexture();
	public native static void BGStartFeedbackRender();
	static {
		System.loadLibrary("com_activetheoryinc_sdk_lib_BitGymMotion");
	}
}