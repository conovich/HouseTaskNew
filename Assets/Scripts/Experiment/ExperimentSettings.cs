using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class ExperimentSettings : MonoBehaviour { //should be in main menu AND experiment


	Experiment exp { get { return Experiment.Instance; } }



	private static Subject _currentSubject;

	public static Subject currentSubject{ 
		get{ return _currentSubject; } 
		set{ 
			_currentSubject = value;
			//fileName = "TextFiles/" + _currentSubject.name + "Log.txt";
		}
	}



	//subject selection controller
	public SubjectSelectionController subjectSelectionController;

	//TOGGLES
	public static bool isOculus = false;
	public static bool isReplay = false;
	public static bool isLogging = true; //if not in replay mode, should log things! or can be toggled off in main menu.

	public Toggle oculusToggle; //only exists in main menu -- make sure to null check
	public Toggle loggingToggle; //only exists in main menu -- make sure to null check

	//EEG, STIM/SYNC TOGGLES
	public static bool isSystem2;
	public static bool isSyncbox;

	public Toggle system2Toggle;
	public Toggle syncboxToggle;


	public Text endCongratsText;
	public Text endScoreText;
	public Text endSessionText;


	public GameObject nonPilotOptions;
	public bool isPilot { get { return GetIsPilot (); } }

	//GET TRAJECTORIES PATH
	public static string trajectoriesPath = ""; //SET IN RESETDEFAULTPATHS();
	public Text trajectoriesPathDisplay;
	public InputField trajectoriesPathInputField;

	//LOGGING PATH
	public static string defaultLoggingPath = ""; //SET IN RESETDEFAULTPATHS();
	string ExpVersionFolder1 = "/EXP/"; //TODO: change for your experiment!
	public Text defaultLoggingPathDisplay;
	public InputField loggingPathInputField;


	//SINGLETON
	private static ExperimentSettings _instance;
	
	public static ExperimentSettings Instance{
		get{
			return _instance;
		}
	}
	
	void Awake(){
		
		if (_instance != null) {
			Debug.Log("Instance already exists!");
			Destroy(transform.gameObject);
			return;
		}
		_instance = this;

		InitLoggingPath ();
		InitMainMenuLabels ();
	}



	void ResetDefaultLoggingPaths(){
		if (Config.isSystem2) {
			defaultLoggingPath = "/Users/" + System.Environment.UserName + "/RAM_2.0/data";
		} else {
			defaultLoggingPath = "/Users/" + System.Environment.UserName + "/RAM/data";
		}
	}

	void InitLoggingPath(){
		ResetDefaultLoggingPaths ();
		
		if(Directory.Exists(defaultLoggingPath)){
			if (Config.BuildVersion == Config.Version.ExpName) {
				defaultLoggingPath += ExpVersionFolder1;
			} 
			
			if(!Directory.Exists(defaultLoggingPath)){ //if that TH folder doesn't exist, make it!
				Directory.CreateDirectory(defaultLoggingPath);
			}
		}
		else{
			defaultLoggingPath = "TextFiles/";
		}
		
		if (defaultLoggingPathDisplay != null) {
			defaultLoggingPathDisplay.text = defaultLoggingPath;
		}
	}
	
	
	public Text ExpNameVersion;
	public Text BuildType;
	void InitMainMenuLabels(){
		if (Application.loadedLevel == 0) {
			ExpNameVersion.text = Config.BuildVersion.ToString () + "/" + Config.VersionNumber;
			if (Config.isSyncbox) {
				BuildType.text = "Sync Box";
			} else if (Config.isSystem2) {
				BuildType.text = "System 2";
			} else {
				BuildType.text = "Demo";
			}
		}
	}




	// Use this for initialization
	void Start () {
		SetOculus();
		SetSystem2();
		SetSyncBox();
		if(Application.loadedLevelName == "EndMenu"){
			if(currentSubject != null){
				endCongratsText.text = "Congratulations " + currentSubject.name + "!";
				endScoreText.text = currentSubject.score.ToString();
				endSessionText.text = currentSubject.trials.ToString();
			}
			else{
				Debug.Log("Current subject is null!");
			}
		}
	}

	bool GetIsPilot(){
		if (nonPilotOptions.activeSelf) {
			return false;
		}
		return true;
	}
	
	public void ChangeLoggingPath(){
		if (Directory.Exists (loggingPathInputField.text)) {
			defaultLoggingPath = loggingPathInputField.text;
		}
		
		defaultLoggingPathDisplay.text = defaultLoggingPath;
	}

	public void ChangeTrajectoriesPath(){
		if (File.Exists(trajectoriesPathInputField.text)) {
			trajectoriesPath = trajectoriesPathInputField.text;
		}
		
		trajectoriesPathDisplay.text = trajectoriesPath;
	}

	public void SetReplayTrue(){
		isReplay = true;
		isLogging = false;
		loggingToggle.isOn = false;
	}


	public void SetReplayFalse(){
		isReplay = false;
		//shouldLog = true;
	}

	public void SetLogging(){
		if(isReplay){
			isLogging = false;
		}
		else{
			if(loggingToggle){
				isLogging = loggingToggle.isOn;
				Debug.Log("should log?: " + isLogging);
			}
		}

	}

	public void SetOculus(){
		if(oculusToggle){
			isOculus = oculusToggle.isOn;
		}
	}

	public void SetSystem2(){
		if(system2Toggle){
			isSystem2 = system2Toggle.isOn;
		}
	}

	public void SetSyncBox(){
		if(syncboxToggle){
			isSyncbox = syncboxToggle.isOn;
		}
	}
	
}
