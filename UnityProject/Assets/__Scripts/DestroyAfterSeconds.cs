using UnityEngine;
using System.Collections;

public class DestroyAfterSeconds : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine("DestroyAfter");
	}
	public float secondsToLive = 5f;
	IEnumerator DestroyAfter()
	{
		yield return new WaitForSeconds(secondsToLive);
		Destroy(gameObject);
	}
}
