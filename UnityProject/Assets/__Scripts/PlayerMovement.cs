using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	public Vector3 velocity;
	public void SetVelocity(Vector3 val)
	{
		velocity = val;
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(velocity);
	}
}