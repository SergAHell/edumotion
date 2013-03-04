#pragma strict

var buttonWidth:int = 200;
var buttonHeight:int = 60;
var spacing:int = 20;

var customSkin:GUISkin;

function Start () {

}

function Update () {

}

function OnGUI () {
/*	
	if (GUI.Button(Rect(Screen.width/2 - buttonWidth/2, Screen.height/2 - buttonHeight/2, buttonWidth, buttonHeight), "WREAK HAVOC!")) {
		print("Ready to wreak havoc!");
		Application.LoadLevel("InGame");
	}
*/
	GUI.skin = customSkin;
	
	var customSkinLabel = customSkin.GetStyle("Label");
	customSkinLabel.alignment = TextAnchor.UpperCenter;
	customSkinLabel.fontSize = 64;
//	GUI.Label(Rect(0, Screen.height/2 - 200, Screen.width, 300), "EDU IN-MOTION");
	
	GUILayout.BeginArea(Rect(Screen.width/2 - buttonWidth/2, Screen.height/2 - buttonHeight/2, buttonWidth, buttonHeight));
		if (GUILayout.Button("PLAY", GUILayout.Height(buttonHeight))) {
			Application.LoadLevel("SpaceScene1");
		}
/*
		GUILayout.Space(spacing);
		if (GUILayout.Button("Continue", GUILayout.Height(buttonHeight))) {
			print("continue");
		}
*/
	GUILayout.EndArea();
	
}