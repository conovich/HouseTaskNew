using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

public class SyncboxControl : MonoBehaviour {
	Experiment exp { get { return Experiment.Instance; } }
	
	//DYNLIB FUNCTIONS
	[DllImport ("liblabjackusb")]
	private static extern float LJUSB_GetLibraryVersion( );
	
	
	[DllImport ("ASimplePlugin")]
	private static extern int PrintANumber();
	[DllImport ("ASimplePlugin")]
	private static extern float AddTwoFloats(float f1,float f2);
	[DllImport ("ASimplePlugin")]
	private static extern int OpenUSB();
	//private static extern IntPtr OpenUSB();
	[DllImport ("ASimplePlugin")]
	private static extern IntPtr CloseUSB();
	[DllImport ("ASimplePlugin")]
	private static extern IntPtr TurnLEDOn();
	[DllImport ("ASimplePlugin")]
	private static extern IntPtr TurnLEDOff();
	[DllImport ("ASimplePlugin")]
	private static extern long SyncPulse();
	[DllImport ("ASimplePlugin")]
	private static extern IntPtr StimPulse(float durationSeconds, float freqHz, bool doRelay);
	
	public bool ShouldSyncPulse = true;
	public float PulseOnSeconds;
	public float PulseOffSeconds;
	public TextMesh DownCircle;
	public Color DownColor;
	public Color UpColor;
	
	public bool isUSBOpen = false; //TODO: set to true.
	
	//bool isToggledOn = false;
	
	
	//SINGLETON
	private static SyncboxControl _instance;
	
	public static SyncboxControl Instance{
		get{
			return _instance;
		}
	}
	
	void Awake(){
		
		if (_instance != null) {
			UnityEngine.Debug.Log("Instance already exists!");
			Destroy(transform.gameObject);
			return;
		}
		_instance = this;
		
	}
	
	// Use this for initialization
	void Start () {
		//UnityEngine.Debug.Log (PrintANumber ());
		if(Config.isSyncbox){
			StartCoroutine(ConnectSyncbox());
		}
	}
	
	IEnumerator ConnectSyncbox(){
		while(!isUSBOpen){
			string usbOpenFeedback = "";//Marshal.PtrToStringAuto (OpenUSB());
			int usbOpenFeedbackInt = OpenUSB();
			//UnityEngine.Debug.Log(usbOpenFeedback);
			UnityEngine.Debug.Log("OPENED USB???: " + usbOpenFeedbackInt);
			//if(usbOpenFeedback != "didn't open USB..."){
			if(usbOpenFeedbackInt == 1){
				isUSBOpen = true;
			}
			
			yield return 0;
		}

		StartCoroutine (RunSyncPulseManual ());
	}
	
	// Update is called once per frame
	void Update () {
		/*if(ExperimentSettings_CoinTask.isSyncbox){
			if (!ShouldSyncPulse) {
				GetInput ();
			}
		}*/
		
		GetInput ();
	}
	
	void GetInput(){
		/*if (Input.GetKeyDown (KeyCode.DownArrow)) {
			SyncPulse();
		}*/
		/*else{
			ToggleOff ();
		}
		if(Input.GetKeyDown(KeyCode.S)){
			//DoSyncPulse();
			DoStimPulse();
		}*/
	}
	
	float syncPulseDuration = 0.05f;
	float syncPulseInterval = 1.0f;
	IEnumerator RunSyncPulse(){
		Stopwatch executionStopwatch = new Stopwatch ();
		
		while (ShouldSyncPulse) {
			executionStopwatch.Reset();

			SyncPulse(); //executes pulse, then waits for the rest of the 1 second interval
			
			executionStopwatch.Start();
			long syncPulseOnTime = SyncPulse();
			LogSYNCOn(syncPulseOnTime);
			while(executionStopwatch.ElapsedMilliseconds < 1500){
				yield return 0;
			}
			
			executionStopwatch.Stop();
		}
	}
	
	IEnumerator RunSyncPulseManual(){
		float jitterMin = 0.1f;
		float jitterMax = syncPulseInterval - syncPulseDuration;
		
		Stopwatch executionStopwatch = new Stopwatch ();
		
		while (ShouldSyncPulse) {
			executionStopwatch.Reset();
			
			
			float jitter = UnityEngine.Random.Range(jitterMin, jitterMax);//syncPulseInterval - syncPulseDuration);
			yield return StartCoroutine(WaitForShortTime(jitter));
			
			//UnityEngine.Debug.Log("time waited A: " + timeWaited);
			
			ToggleLEDOn();
			yield return StartCoroutine(WaitForShortTime(syncPulseDuration));
			//UnityEngine.Debug.Log("time waited B: " + timeWaited);
			ToggleLEDOff();
			
			float timeToWait = (syncPulseInterval - syncPulseDuration) - jitter;
			if(timeToWait < 0){
				timeToWait = 0;
			}
			
			yield return StartCoroutine(WaitForShortTime(timeToWait));
			//UnityEngine.Debug.Log("time waited C: " + timeWaited);
			
			executionStopwatch.Stop();
		}
	}
	
	//return microseconds it took to turn on LED
	void ToggleLEDOn(){
		
		TurnLEDOn ();
		LogSYNCOn (GameClock.SystemTime_Milliseconds);
	}
	
	void ToggleLEDOff(){
		
		TurnLEDOff();
		LogSYNCOff (GameClock.SystemTime_Milliseconds);
		
	}
	
	long GetMicroseconds(long ticks){
		long microseconds = ticks / (TimeSpan.TicksPerMillisecond / 1000);
		return microseconds;
	}
	
	//float timeWaited = 0.0f;
	IEnumerator WaitForShortTime(float jitter){
		float currentTime = 0.0f;
		while (currentTime < jitter) {
			currentTime += Time.deltaTime;
			yield return 0;
		}
		
		//timeWaited = currentTime;
	}

	
	void LogSYNCOn(long time){
		if (ExperimentSettings.isLogging) {
			exp.eegLog.Log (time, exp.eegLog.GetFrameCount(), "ON"); //NOTE: NOT USING FRAME IN THE FRAME SLOT
		}
	}
	
	void LogSYNCOff(long time){
		if (ExperimentSettings.isLogging) {
			exp.eegLog.Log (time, exp.eegLog.GetFrameCount(), "OFF"); //NOTE: NOT USING FRAME IN THE FRAME SLOT
		}
	}
	
	void LogSYNCStarted(long time, float duration){
		if (ExperimentSettings.isLogging) {
			exp.eegLog.Log (time, exp.eegLog.GetFrameCount (), "SYNC PULSE STARTED" + Logger_Threading.LogTextSeparator + duration);
		}
	}
	
	void LogSYNCPulseInfo(long time, float timeBeforePulseSeconds){
		if (ExperimentSettings.isLogging) {
			exp.eegLog.Log (time, exp.eegLog.GetFrameCount (), "SYNC PULSE INFO" + Logger_Threading.LogTextSeparator + timeBeforePulseSeconds*1000); //log milliseconds
		}
	}
	

	
	void OnApplicationQuit(){
		UnityEngine.Debug.Log(Marshal.PtrToStringAuto (CloseUSB()));
	}
	
}