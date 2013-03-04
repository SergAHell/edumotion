using UnityEngine;
using System.Collections;

public class Director : MonoBehaviour {
	
	public GameObject player1;
	public GameObject player2;
	private bool keyboardMode = false;
	
	void Start () {
		SetEnabledState(player2, false);
		BGMotionManager.GetInstance().SetPlayerCount(1);
		BGMotionManager.GetInstance().SetEditorWebcamMode();
		
		player1.GetComponent<CursorMovement>().SetCameraTransform(transform);
		player2.GetComponent<CursorMovement>().SetCameraTransform(transform);
	}
	
	void Update () {
	}
	
	void OnGUI ()
	{
		/* // BGMotion code
		if(GUI.Button(new Rect(20, 20, 100, 40), "1P")){
			BGMotionManager.GetInstance().SetPlayerCount(1);
			SetEnabledState(player2, false);
			SetEnabledState(player1, true);
		}
		if(GUI.Button(new Rect(130, 20, 100, 40), "2P")){
			BGMotionManager.GetInstance().SetPlayerCount(2);
			SetEnabledState(player2, true);
			SetEnabledState(player1, true);
		}

		if(Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor){
			if(GUI.Button(new Rect(240, 20, 140, 40), "Keyboard Toggle")){
				keyboardMode = (keyboardMode == true) ? false : true;
				if(keyboardMode)
					BGMotionManager.GetInstance().SetEditorKeyboardMode();
				else 
					BGMotionManager.GetInstance().SetEditorWebcamMode();
			}
		}
		
		
		if(BGMotionManager.GetInstance().GetPlayerCount() <= 0){
			if(GUI.Button(new Rect(400, 20, 100, 40), "Enable")){
				BGMotionManager.GetInstance().SetPlayerCount(1);
				SetEnabledState(player2, false);
				SetEnabledState(player1, true);
			}
		} else { 
			if(GUI.Button(new Rect(400, 20, 100, 40), "Disable")){
				BGMotionManager.GetInstance().SetPlayerCount(0);
				SetEnabledState(player2, false);
				SetEnabledState(player1, false);
			}
		}
		*/
		

	}
	
	public void SetEnabledState(GameObject target, bool enabledState) {
		CursorMovement cm = target.GetComponent<CursorMovement>() as CursorMovement;
		if(enabledState)
			BGMotionManager.GetInstance().AddBodyReadingListener(cm);
		else
			BGMotionManager.GetInstance().RemoveBodyReadingListener(cm);
			
		//Sets visibility for an object and all its children
		target.SetActiveRecursively(enabledState);
		Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
		foreach (Renderer r in renderers) {
			r.enabled = enabledState;
		}
	}
}
