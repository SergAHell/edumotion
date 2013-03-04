// Just a simple rotating script.
var rotateSpeed = 1.0;

function Update () {
	transform.Rotate(0, rotateSpeed * Time.deltaTime, 0, Space.World);
}