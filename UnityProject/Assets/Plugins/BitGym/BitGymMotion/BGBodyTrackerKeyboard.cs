using UnityEngine;
		
public class BGBodyTrackerKeyboard : IBGBodyTracker
{
		
	BGMotionManager owner;
	private bool isSetup = false;
	private int playerCount;
	
	private PlayerBodyKeyboard p1, p2, p3, p4;
		
	public BGBodyTrackerKeyboard (BGMotionManager mm) {
		owner = mm;
	}
	
	private void setup ()
	{
		p1 = new PlayerBodyKeyboard ("a", "d", "s", "w", "q", "e", "z", "x", 0);
		p2 = new PlayerBodyKeyboard ("f", "h", "g", "t", "r", "y", "v", "b", 1);
		p3 = new PlayerBodyKeyboard ("j", "l", "k", "i", "u", "o", "m", ",", 2);
		p4 = new PlayerBodyKeyboard ("left", "right", "down", "up", ".", "/", ";", "'", 3);
	}
	
	public void DisableTracking () {
		
	}
	
	public void EnableTracking () {
		
	}
	
	public bool IsTracking () {
		return isSetup;
	}
	
	public void SetPlayerCount(int _playerCount){
		playerCount = _playerCount;
	}
	
		
	public void DoUpdate ()
	{
		if (!isSetup) {
			setup ();
			isSetup = true;
		}
		//use WASD to emulate human movement.
		owner.PublishReading (p1.Emulate (), this);
		if(playerCount > 1) owner.PublishReading (p2.Emulate (), this);
		if(playerCount > 2) owner.PublishReading (p3.Emulate (), this);
		if(playerCount > 3) owner.PublishReading (p4.Emulate (), this);
	}
		
	private class PlayerBodyKeyboard
	{
		private string left, right, duck, jump, leanLeft, leanRight, forward, backward;
		private int player;
		private bool isJumping;
		private float emulatedXPosition = 0.0f;
		private float emulatedYPosition = 0.0f;
		private float emulatedYVelocity = 0.0f;
		private float emulatedTilt = 0.0f;
		private float emulatedScale = 0.5f;
		private float timeToCrossHalfOfStage = 0.4f;
		private float timeToLeanCompletely = 0.6f;
		private float lastJumpStartTime;
		private float lastUpdateTime;
	
		public PlayerBodyKeyboard (string kleft, string kright, string kduck, string kjump, 
										string kleanLeft, string kleanRight, string kforward, string kbackward, int kplayer)
		{
			left = kleft;
			right = kright;
			duck = kduck;
			jump = kjump;
			leanLeft = kleanLeft;
			leanRight = kleanRight;
			player = kplayer;
			forward = kforward;
			backward = kbackward;
			lastUpdateTime = Time.time;
		}
			
		public BGBodyReading Emulate ()
		{
			float dt = Time.time - lastUpdateTime;
			lastUpdateTime = Time.time;
			// I moves left in a sticky way
			// K moves right in a sticky way
			// J creates a ducking motion
			// L creates a jumping motion
			// U leans left
			// O leans right
			if (Input.GetKey (left)) {
				emulatedXPosition -= dt * (1.0f / timeToCrossHalfOfStage);
				if (emulatedXPosition < -1.0f)
					emulatedXPosition = -1.0f;
			}
			if (Input.GetKey (right)) {
				emulatedXPosition += dt * (1.0f / timeToCrossHalfOfStage);
				if (emulatedXPosition > 1.0f)
					emulatedXPosition = 1.0f;
			}
			if (Input.GetKey (duck)) {
				emulatedYPosition = Mathf.Lerp (emulatedYPosition, -0.5f, 0.4f);
			}
			if (Input.GetKey (leanRight)) {
				emulatedTilt -= dt * (1.31f / timeToLeanCompletely);
				if (emulatedTilt < -1.31f)
					emulatedTilt = -1.31f;
				//				emulatedTilt = Mathf.Lerp (emulatedTilt, -1.31f, 0.4f);
			}
			if (Input.GetKey (leanLeft)) {
				emulatedTilt += dt * (1.31f / timeToLeanCompletely);
				if (emulatedTilt > 1.31f)
					emulatedTilt = 1.31f;
				//emulatedTilt = Mathf.Lerp (emulatedTilt, 1.31f, 0.4f);
			}
			if (Input.GetKey (forward)) {
				emulatedScale -= dt * (0.5f / timeToCrossHalfOfStage);
				if (emulatedScale < 0.25f)
					emulatedScale = 0.25f;
			}
			if (Input.GetKey (backward)) {
				emulatedScale += dt * (0.5f / timeToCrossHalfOfStage);
				if (emulatedScale > 1.0f)
					emulatedScale = 1.0f;
			}
			if (Input.GetKey (jump)) {
				if (isJumping == false) {
					lastJumpStartTime = Time.time;
					isJumping = true;
				}
				if (Time.time - lastJumpStartTime < 0.08f) {
					//pre-jump crouch
					emulatedYPosition = Mathf.Lerp (emulatedYPosition, -0.22f, 0.1f);
				} else if (emulatedYPosition < 0.0f) {
					emulatedYVelocity = 4.5f;
				}
			} else {
				isJumping = false;
			}
			//jumping phyics
			float crouchLandingPos = -0.2f;
			if (emulatedYVelocity > 0.0f || emulatedYPosition > 0.0f) {
				emulatedYVelocity -= 16.0f * dt;
				emulatedYPosition += emulatedYVelocity * dt;
				if (emulatedYPosition < crouchLandingPos)
					emulatedYPosition = crouchLandingPos;
			}
			if (emulatedYPosition < 0.0f) {
				emulatedYPosition = Mathf.Lerp (emulatedYPosition, 0.0f, 0.2f);	
			}
		
		
			return new BGBodyReading (emulatedXPosition, emulatedYPosition, emulatedTilt, emulatedScale, 1.0f, Time.time, player);
		}
	}	
}
