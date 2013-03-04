@script ExecuteInEditMode()

private var playerStatus : PlayerStatus;
var textPos = Vector2(0,0);

public var customSkin:GUISkin;

function Update () {
}

function Awake () {
playerStatus = FindObjectOfType(PlayerStatus);
	
}

function OnGUI() {
	var points = playerStatus.GetPoints();
	
	GUI.skin = customSkin;
	
	var customSkinLabel = customSkin.GetStyle("Label");
	customSkinLabel.alignment = TextAnchor.UpperCenter;
	customSkinLabel.fontSize = 140;
	
//	GUI.Label(Rect(textPos.x,textPos.y,400,100), points.ToString() );

	// update score
	customSkinLabel.alignment = TextAnchor.UpperLeft;
	customSkinLabel.fontSize = 24;
	GUI.Label(Rect(100, 50, 300, 100), String.Format("SCORE: {0}", points));


}