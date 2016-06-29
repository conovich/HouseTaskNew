using UnityEngine;
using System.Collections;
using System.IO;

public class Experiment : MonoBehaviour {

	//juice controller
	public JuiceController juiceController;

	//instructions
	public InstructionsController instructionsController;
	public InstructionsController fullInstructionsPanel;
	public CameraController cameraController;

	//logging
	private string subjectLogfile; //gets set based on the current subject in Awake()
	public Logger_Threading subjectLog;
	private string eegLogfile; //gets set based on the current subject in Awake()
	public Logger_Threading eegLog;
	string sessionDirectory;
	public static string sessionStartedFileName = "sessionStarted.txt";
	public static int sessionID;

	//session controller
	public TrialController trialController;

	//score controller
	public ScoreController scoreController;

	//UI controller
	public UIController uiController;

	//object controller
	public ObjectController objectController;

	//environment controller
	public EnvironmentController environmentController;

	//learning locator controller
	public LearningLocatorController learningLocatorController;

	//house controller
	public HouseController houseController;

	public OverheadMap overheadMap;

	//avatar
	public Player player;

	//public bool isOculus = false;

	//state enum
	public ExperimentState currentState = ExperimentState.inExperiment;

	public enum ExperimentState
	{
		inExperiment,
		inExperimentOver
	}

	//bools for whether we have started the state coroutines
	bool isRunningExperiment = false;


	//EXPERIMENT IS A SINGLETON
	private static Experiment _instance;

	public static Experiment Instance{
		get{
			return _instance;
		}
	}

	void Awake(){
		if (_instance != null) {
			Debug.Log("Instance already exists!");
			return;
		}
		_instance = this;

		juiceController.Init ();


		cameraController.SetInGame(); //don't use oculus for replay mode

		if (ExperimentSettings.isLogging) {
			InitLogging();
		}
		else if(ExperimentSettings.isReplay) {
			instructionsController.TurnOffInstructions();
		}

	}
	
	//TODO: move to logger_threading perhaps? *shrug*
	void InitLogging(){
		string subjectDirectory = ExperimentSettings.defaultLoggingPath + ExperimentSettings.currentSubject.name + "/";
		sessionDirectory = subjectDirectory + "session_0" + "/";
		
		sessionID = 0;
		string sessionIDString = "_0";
		
		if(!Directory.Exists(subjectDirectory)){
			Directory.CreateDirectory(subjectDirectory);
		}
		while (File.Exists(sessionDirectory + sessionStartedFileName)) {
			sessionID++;
			
			sessionIDString = "_" + sessionID.ToString();
			
			sessionDirectory = subjectDirectory + "session" + sessionIDString + "/";
		}
		
		//delete old files.
		if(Directory.Exists(sessionDirectory)){
			DirectoryInfo info = new DirectoryInfo(sessionDirectory);
			FileInfo[] fileInfo = info.GetFiles();
			for(int i = 0; i < fileInfo.Length; i++){
				File.Delete(fileInfo[i].ToString());
			}
		}
		else{ //if directory didn't exist, make it!
			Directory.CreateDirectory(sessionDirectory);
		}
		
		subjectLog.fileName = sessionDirectory + ExperimentSettings.currentSubject.name + "Log" + ".txt";
		eegLog.fileName = sessionDirectory + ExperimentSettings.currentSubject.name + "EEGLog" + ".txt";
	}


	//In order to increment the session, this file must be present. Otherwise, the session has not actually started.
	//This accounts for when we don't successfully connect to hardware -- wouldn't want new session folders.
	//Gets created in TrialController after any hardware has connected.
	public void CreateSessionStartedFile(){
		StreamWriter newSR = new StreamWriter (sessionDirectory + sessionStartedFileName);
	}



	// Use this for initialization
	void Start () {
		//Config_CoinTask.Init();
		//inGameInstructionsController.DisplayText("");
	}

	// Update is called once per frame
	void Update () {
		//Proceed with experiment if we're not in REPLAY mode
		if (!ExperimentSettings.isReplay) { //REPLAY IS HANDLED IN REPLAY.CS VIA LOG FILE PARSING
			if (currentState == ExperimentState.inExperiment && !isRunningExperiment) {
				Debug.Log("running experiment");
				StartCoroutine(BeginExperiment());
			}

		}
	}

	public IEnumerator RunOutOfTrials(){
		/*while(environmentMap.IsActive){
			yield return 0; //thus, should wait for the button press before ending the experiment
		}*/
		
		yield return StartCoroutine(instructionsController.ShowSingleInstruction("You have finished your trials! \nPress the button to proceed.", true, true, false, 0.0f));
		instructionsController.SetInstructionsColorful(); //want to keep a dark screen before transitioning to the end!
		instructionsController.DisplayText("...loading end screen...");
		EndExperiment();

		yield return 0;
	}


	public IEnumerator BeginExperiment(){
		isRunningExperiment = true;

		yield return StartCoroutine(trialController.RunExperiment());
		
		yield return StartCoroutine(RunOutOfTrials()); //calls EndExperiment()

		yield return 0;

	}

	public void EndExperiment(){
		Debug.Log ("Experiment Over");
		currentState = ExperimentState.inExperimentOver;
		isRunningExperiment = false;
		
		SceneController.Instance.LoadEndMenu();
	}

	public IEnumerator WaitForActionButton(){
		bool hasPressedButton = false;
		while(Input.GetAxis("Action Button") != 0f){
			yield return 0;
		}
		while(!hasPressedButton){
			if(Input.GetAxis("Action Button") == 1.0f){
				hasPressedButton = true;
			}
			yield return 0;
		}
	}

	public IEnumerator WaitForJitter(float minJitter, float maxJitter){
		float randomJitter = Random.Range(minJitter, maxJitter);
		trialController.GetComponent<TrialLogTrack>().LogWaitForJitterStarted(randomJitter);
		
		float currentTime = 0.0f;
		while (currentTime < randomJitter) {
			currentTime += Time.deltaTime;
			yield return 0;
		}

		trialController.GetComponent<TrialLogTrack>().LogWaitForJitterEnded(currentTime);
	}


	public void OnExit(){ //call in scene controller when switching to another scene!
		if (ExperimentSettings.isLogging) {
			subjectLog.close ();
			eegLog.close ();
		}
	}

	void OnApplicationQuit(){
		if (ExperimentSettings.isLogging) {
			subjectLog.close ();
			eegLog.close ();
		}
	}


}
