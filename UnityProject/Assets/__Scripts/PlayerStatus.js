var totalPoints : int = 0;
var damage : int = 1;
var nLettersPickedUp : int = 0;


private var animation_state : Animation_State;
private var anim : Animation;
private var handlePickupAnimation = false;

function Start () {
	animation_state = FindObjectOfType(Animation_State);
	anim = GetComponent(Animation);
	if (!anim) {
		Debug.LogError("Missing player animation!");
	}
}

function Update () {
}

function AddPoints (pts : int) {
	totalPoints += pts;
}

function Damage (dam : int) {
	totalPoints -= dam;
}

function GetPoints() {
	return totalPoints;
}

function GetNumberLettersPickedUp(){
	return nLettersPickedUp;
}

function OnTriggerEnter (other : Collider) {
	Debug.Log("Player Collision triggered");
    if (other.gameObject.CompareTag ("Enemy")) {
		Damage(5);
		Destroy (other.gameObject);
	} else if (other.gameObject.CompareTag ("Pickup")) {
		Debug.Log("Pickup collision handler enter");
		nLettersPickedUp++;

		// Calc and add points
		var pickup = other.gameObject.GetComponent(Pickup);
		var pts = 1;
		if (pickup) {
			pts = pickup.GetPointValue();
		}
		AddPoints(pts);
		
		// Player anim
		animation_state.animState = 3;
		handlePickupAnimation = true;
		if (other.gameObject.name == "pickup_H") {
			HandleWin(other.gameObject);
		}
		
		// Clone letter and add to bottom of screen
		var clone = Instantiate(other.gameObject, new Vector3(-180f + (45f * GetNumberLettersPickedUp()), -40f, 140f), Quaternion.identity);
		clone.transform.Rotate(0,180,0);
		var rotateObj = clone.GetComponent(RotateObject);
		if (rotateObj) {
			Destroy(rotateObj);
		}
		
		other.renderer.enabled = false;
	}
}


function OnTriggerExit (other : Collider) {
	Debug.Log("Player Collision exit start");
	if (handlePickupAnimation) {
		animation_state.animState = 0;
		handlePickupAnimation = false;
		Destroy (other.gameObject);

	}
}

function HandleWin(go : GameObject) {
	animation_state.animState = 2;
}