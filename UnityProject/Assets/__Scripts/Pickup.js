var pointsValue : int;

function Start() {
}

function Update () {
}

function OnTriggerEnter (col : Collider) {
}

function GetPointValue() {
	if (pointsValue == null) {
		pointsValue = 1;
	}
	
	return pointsValue;
}