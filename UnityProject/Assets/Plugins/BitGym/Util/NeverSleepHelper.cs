using UnityEngine;
using System.Collections;

public class NeverSleepHelper : MonoBehaviour {

	void Start () {
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}
	
}
