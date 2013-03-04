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

#import <Foundation/Foundation.h>

@interface BGSensorObservable: NSObject {
    int observerCount;
    bool forcedStartMode;
    bool keepAliveForHandoff;
    NSTimer * handoffTimer;
    bool isTracking;
}

-(void) enableForcedTrackingMode;
-(void) disableForcedTrackingMode;
-(void) addObserver:(NSObject *)anObserver forKeyPath:(NSString *)keyPath options:(NSKeyValueObservingOptions)options context:(void *)context;
-(void) addObserver:(NSObject *)anObserver forKeyPath:(NSString *)keyPath;
-(void) removeObserver:(NSObject *)anObserver forKeyPath:(NSString *)keyPath;
-(void) keepAliveForHandoff:(double)seconds;
-(void) handoffTimerEnd;

-(void) checkIfShouldStopTracking;
-(void) checkIfShouldStartTracking;

//subclass methods that should be overridden
-(void) startTracking;
-(void) stopTracking;

@end