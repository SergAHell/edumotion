using UnityEngine;
using System.Collections;


public interface IBGBodyTrackerManager {
	void SetPlayerCount(int count);
	int GetPlayerCount();
	Texture2D GetFeedbackTexture();
	void PublishReading(BGBodyReading reading, IBGBodyTracker tracker);
}
