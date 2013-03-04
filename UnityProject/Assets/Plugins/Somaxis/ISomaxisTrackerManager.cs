using UnityEngine;
using System.Collections;


public interface ISomaxisTrackerManager {
	void SetPlayerCount(int count);
	int GetPlayerCount();
	Texture2D GetFeedbackTexture();
	void PublishReading(SomaxisReading reading, ISomaxisTracker tracker);
}
