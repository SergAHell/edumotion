using UnityEngine;
using System.Collections;

public interface ISomaxisTracker
{
	void DisableTracking ();
	void EnableTracking ();
	void DoUpdate ();
	void SetPlayerCount(int playerCount);
	bool IsTracking();
}

