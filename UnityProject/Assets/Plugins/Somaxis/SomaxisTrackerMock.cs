using UnityEngine;
using System;
		
public class SomaxisTrackerMock : MonoBehaviour, ISomaxisTracker
{
	public TextAsset eegCsv;
	private string[,] eegData;
	private int currIndex, dataLength;
		
	SomaxisManager owner;
	private bool isSetup = false;
	private int playerCount;
	
	private int yOffset = 0;
	private int MAX_AMPLITUDE = 1000;
		
	public void SetOwner (SomaxisManager mm)
	{
		owner = mm;
	}

	public void DisableTracking ()
	{
		isSetup = false;
		eegData = null;
		eegCsv = null;
		dataLength = 0;
	}
	
	public void EnableTracking ()
	{
		if (!isSetup) {
			eegCsv = Resources.Load ("ecg") as TextAsset;
			Debug.Log ("setup csv file: ");
			eegData = CSVReader.SplitCsvGrid (eegCsv.text);
			dataLength = eegData.Length;
			isSetup = true;
		}
	}
	
	public bool IsTracking ()
	{
		return isSetup;
	}
	
	public void SetPlayerCount (int _playerCount)
	{
		playerCount = _playerCount;
	}
		
	public void DoUpdate ()
	{
		float value = 0f;
		
		if (isSetup) {
		
			if (++currIndex == dataLength) {
				currIndex = 0;
			}
	
			try {
				value = (float.Parse (eegData [1, currIndex]) - yOffset) / MAX_AMPLITUDE;
				SomaxisReading reading = new SomaxisReading (int.Parse (eegData [0, currIndex]), value);
				owner.PublishReading (reading, this);
			} catch (Exception e) {
				Debug.LogError ("Could not generate csv reading data!");
			}
			
			if (currIndex % 120 == 0) {
				Debug.Log ("Csv reading data " + currIndex + ": " + eegData [0, currIndex]
					+ ", " + value);
			}
		}
	}
			
}
