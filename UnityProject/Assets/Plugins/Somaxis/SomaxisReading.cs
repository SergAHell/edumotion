using UnityEngine;

public class SomaxisReading : ISomaxisReading
{
	private float amplitude;
	private float confidence;
	private float timestamp;
	private int playerCount;
	
	// sampleIndex is 0 to 49
	// amplitude is +/- 500
	public SomaxisReading (int sampleIndex, float amplitude)
	{
		this.amplitude = amplitude;
		this.timestamp = Time.time;
		this.confidence = 1.0f;
		this.playerCount = 1;
	}
	
	public float GetReading ()
	{
		return this.amplitude;
	}
	
	public float GetTimestamp ()
	{
		return this.timestamp;
	}
	public float GetConfidence ()
	{
		return this.confidence;
	}
	
	public int GetPlayer()
	{
		return this.playerCount;
	}
}
	
