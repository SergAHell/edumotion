using UnityEngine;
using System.Collections;

[ExecuteInEditMode]

public class Menu : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void OnGUI () {
		if(GUI.Button(new Rect(Screen.width - 200, Screen.height-80, 180, 60), "Calibrate")){
//			BitGymManager.StartCalibration();
		}
	}
}
