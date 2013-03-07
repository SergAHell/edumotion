using UnityEngine;
using System.Collections;
using Exocortex.DSP;

public class SomaxisGraph : MonoBehaviour, ISomaxisReadingListener {
	public int player = 0;
	public int resolution = 64;
	//public float particleSize = 0.1f;
	
	private int currentResolution;
	private float[] rawData;
	private float[] rawTimestamps;
	private float[] filteredData;
	private LineRenderer lr;
	
	private ParticleSystem.Particle[] points;
	//private int cIndex = 0;
	private float increment;
	
	// in Hz
	public int minFreqHz;
	public int maxFreqHz;
	public bool showFourier;
	
	// Fourier vars
	private float lastDt;
	//private ComplexF[] fft;
	
	public void Awake ()
	{
		Debug.Log ("Start SomaxisGraph");
		SomaxisManager.GetInstance ().AddReadingListener (this);
		CreatePoints ();
		lr = GetComponent<LineRenderer> ();
		//lr.SetWidth (0.2f, 0.2f);
	}
	
	public void setResolution (int res)
	{
		resolution = res;
	}
	
	private void CreatePoints ()
	{
		if (resolution < 2) {
			resolution = 2;
		} else if (resolution > 16384) {
			resolution = 16384;
		}
		currentResolution = resolution;

		rawData = new float[currentResolution];
		filteredData = new float[currentResolution];
		rawTimestamps = new float[currentResolution];
		if (showFourier == null) {
			showFourier = false;
		}
		//lr.SetVertexCount(currentResolution - 1); // this does not override the UI...?
		
		//points = new ParticleSystem.Particle[currentResolution];
		increment = 1f / (currentResolution - 1);
		
		for (int i = 0; i < currentResolution; i++) {
			rawData [i] = 0f;
			filteredData [i] = 0f;
			rawTimestamps [i] = 0f;
		}
	}
	
	public void Update ()
	{
		//Debug.Log ("Update particles");
		//string foo = "";
		
		if (currentResolution != resolution) {
			CreatePoints ();
		}
		
		FilterSignal ();
		
		for (int i = 1; i < currentResolution; i++) {
			int j = i-1;
			float x = i * increment;
			//lr.SetColors (new Color (x, .5f, filteredData [j], 0.9f), new Color (x, .5f, filteredData [i], 0.9f));
			lr.SetPosition (j, new Vector3 (x, filteredData [j], 0f));
			
			//foo += "," + rawData[i].ToString();
		}
		
		//lastDt = Time.deltaTime;
		
		//Debug.Log ("Data: " + foo);
	}

	public void HandleReading (SomaxisReading reading)
	{
		if (player == reading.GetPlayer ()) {
			
			for (int i=1; i<currentResolution; i++) {
				// shift arrays
				rawData [i - 1] = rawData [i]; 
				rawTimestamps [i - 1] = rawTimestamps [i];
			}

			rawData [currentResolution - 1] = reading.GetReading ();
			rawTimestamps [currentResolution - 1] = reading.GetTimestamp ();
			//fftTime = Time.deltaTime - lastDt;
		}
	}
	
	private void FilterSignal ()
	{
		if (minFreqHz == null || maxFreqHz == null) {
			Debug.LogWarning ("Cutoff min+max freqs. not set in SomaxisGraph. Won't filter signal.");
			return;
		}
		if (rawTimestamps [0] == 0f) {
			// We can't do filtering until we have enough samples
			return;
		}
		
		ComplexF[] fft = new ComplexF[currentResolution];
		// sampling freq
		float dt = rawTimestamps [currentResolution - 1] - rawTimestamps [0];
		
		for (int i=0; i<currentResolution; i++) {
			fft [i].Re = rawData [i];
		}

		Exocortex.DSP.Fourier.FFT (fft, fft.Length, FourierDirection.Forward);
		//BandFilter (fft, dt, minFreqHz, maxFreqHz);
		
		for (int i=0; i<currentResolution; i++) {
			//filteredData [i] = fft [i].Re; // the FT freq. domain
			filteredData [i] = rawData[i]; // use this to see the graph of the raw data
		}
	}
	
	private void BandFilter (ComplexF[] fft, float dt, float minFreq, float maxFreq)
	{
		float Fs = fft.Length / dt;
		
		// High pass filter
		float RC = 1 / (6.28318530718f * minFreq); // 2*pi = 6.28318530718
		float alpha = RC / (RC + dt);
		
		for (int i=1; i<fft.Length; i++) {
			fft[i].Re = (alpha * fft[i-1].Re) + (alpha * dt);
		}
			
		

		// Low pass filter
//		RC = 1 / (6.28318530718f * maxFreq); // 2*pi = 6.28318530718
//		alpha = RC / (RC + dt);
	}

}
