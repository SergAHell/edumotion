using UnityEngine;

public class CursorMovement : MonoBehaviour, IBGBodyReadingListener{
	
	public int player = 0;
	private Vector3 startPosition;
	private Vector3 startScale;
	private Transform cameraTransform;
		
	void Start(){
		BGMotionManager.GetInstance().AddBodyReadingListener(this);
		startPosition = transform.position;
		startScale = transform.localScale;
	}
	 
	public void HandleReading(BGBodyReading reading) {
		if(player == reading.GetPlayer()){
			//first look at the camera, then apply our rotations
			transform.LookAt(cameraTransform.position * -1.0f);
			
			transform.position = startPosition + new Vector3(reading.GetX()*100.0f, reading.GetY()*100.0f, 0.0f);
			transform.localRotation = Quaternion.Euler(new Vector3(
				transform.localRotation.eulerAngles.x,
				transform.localRotation.eulerAngles.y,
				-reading.GetTilt()*Mathf.Rad2Deg
			));
			
			
			transform.localScale = startScale * (reading.GetScale() + 0.3f) * 4.0f;
				
		}
	}
	
	public void SetCameraTransform(Transform newCameraTransform)
	{
		cameraTransform = newCameraTransform;
	}
}