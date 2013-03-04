using UnityEngine;
using System.Collections;

public enum JumpState {
	STANDING,
	CROUCHING,
	JUMPING
};

//A class that can be fed user position information and will spit out if the user is jumping, ducking, or standing.
//Note that it depends on on the player exercising between jump and duck events which sets the general bounds of movement.
public class BGJumpTracker {
	public JumpState jumpState;
	
	private float lowPassY;
	private float oldPosTimestamp = 0;
	private float oldYPos = -10;
	private float yVelocity = 0;
	
	private float jumpVelThreshold = 2.0f, crouchThreshold = 0.1f;
	
	public void TrackUserYPosition(float yPosition, float timestamp) {
		if (oldYPos != -10)
			yVelocity = (yPosition - oldYPos)/(timestamp - oldPosTimestamp);
		float x = Mathf.Pow(0.2f, timestamp - oldPosTimestamp);
		// TODO: Consider a system of adjusting this based on crouch or jump
		lowPassY = x*lowPassY + (1-x)*(yPosition);
		oldYPos = yPosition;
		oldPosTimestamp = timestamp;

		jumpState = JumpState.STANDING;
		if (yVelocity > jumpVelThreshold) jumpState = JumpState.JUMPING; 
		if (yPosition < lowPassY - crouchThreshold) jumpState = JumpState.CROUCHING;
	}
}


