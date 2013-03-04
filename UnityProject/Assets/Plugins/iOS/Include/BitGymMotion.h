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

#ifndef BitGymMotion_h
#define BitGymMotion_h

#include "BGBodyReadingData.h"

extern "C" {

    //If true, the tracker and camera will run every frame so they can be polled for body position readings
    void SetIsBodyTrackingPolling(bool enabled);

    
    //playerNumber should be from 0-1
    BGBodyReadingData PollBodyTracker(int playerNumber); 

    struct BGBodyTrackingProperties {
        int playerCount; //should be 1-2
        //bool trackScaleAndRotation; choice not implemented yet - always on
    };

    void SetBodyTrackingProperties(BGBodyTrackingProperties props);

    struct BGBodyFeedbackProperties{
#warning not implemented yet
        bool greyscaleBackground;
        //future could have dot color, aspect ratio, and more
    };

    void StartOpenGLBodyFeedbackRendering(int textureId, BGBodyFeedbackProperties properties);
    void StopOpenGLBodyFeedbackRendering();
}
#endif 
