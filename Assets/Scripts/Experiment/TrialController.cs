using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class TrialController : MonoBehaviour {
	Experiment exp { get { return Experiment.Instance; } }

	//hardware connection
	bool isConnectingToHardware = false;

	//paused?!
	public static bool isPaused = false;

	//2D vs 3D trials
	public TextAsset TrialTypeFile;

	public SimpleTimer GameTimer;

	//public CanvasGroup initialInstructions;
	//public CanvasGroup trialPhaseInstructions;

	TrialLogTrack trialLogger;

	bool isPracticeTrial = false;
	int numRealTrials = 0; //used for logging trial ID's

	int numStoresVisited = 0;

	Trial currentTrial;
	Trial practiceTrial;

	List<Trial> TrialList;

	void Start(){
		GenerateTrials ();
		trialLogger = GetComponent<TrialLogTrack> ();
	}

	void GenerateTrials(){
		TrialList = new List<Trial> ();

		for(int i = 0; i < Config.numTestTrials; i++){
			Trial trial = new Trial(true); //will set the actual trial types later in ProcessTrialTypes
			TrialList.Add(trial);
		}

		if(Config.doPracticeTrial){
			practiceTrial = new Trial(true);	//2 special objects for practice trial
		}

		//ProcessTrialTypes ();

	}

	void ProcessTrialTypes(){
		string[] lines = TrialTypeFile.ToString().Split('\n');

		for (int i = 0; i < TrialList.Count; i++) {
			if(i < lines.Length){
				string line = lines[i];
				if(line != ""){
					string[] typesPerSession = line.Split('\t');
					int sessionIndex = Experiment.sessionID;
					if(Experiment.sessionID >= typesPerSession.Length){
						sessionIndex = typesPerSession.Length - 1;
						Debug.Log("TRIAL TYPES: Not enough sessions to choose from!");
					}

					int currSessionType = int.Parse(typesPerSession[sessionIndex]);
					if(currSessionType == 1){
						TrialList[i].is3DFirst = true;
					}
					else if(currSessionType == 2){
						TrialList[i].is3DFirst = false;
					}
				}
			}
			else{
				Debug.Log("Not enough lines in this file!");
			}
		}
	}

	Trial GetNextTrial(){
		Debug.Log("Picking a trial.");

		if (TrialList.Count > 0) {
			Trial nextTrial = TrialList[0];

			TrialList.RemoveAt (0);
			return nextTrial;
		}
		else{
			Debug.Log("No more trials left!");
			return null;
		}
	}


	
	void Update(){
		if(!isConnectingToHardware){
			GetPauseInput ();
		}
	}

	bool isPauseButtonPressed = false;
	void GetPauseInput(){
		//if (Input.GetAxis ("Pause Button") > 0) {
		if(Input.GetKeyDown(KeyCode.B) || Input.GetKey(KeyCode.JoystickButton2)){ //B JOYSTICK BUTTON TODO: move to input manager.
			Debug.Log("PAUSE BUTTON PRESSED");
			if(!isPauseButtonPressed){
				isPauseButtonPressed = true;
				Debug.Log ("PAUSE OR UNPAUSE");
				TogglePause (); //pause
			}
		} 
		else{
			isPauseButtonPressed = false;
		}
	}

	public void TogglePause(){
		isPaused = !isPaused;
		trialLogger.LogPauseEvent (isPaused);

		if (isPaused) {
			//exp.player.controls.Pause(true);
			exp.uiController.PauseUI.alpha = 1.0f;
			Time.timeScale = 0.0f;
		} 
		else {
			Time.timeScale = 1.0f;
			//exp.player.controls.Pause(false);
			//exp.player.LockControls(false);
			exp.uiController.PauseUI.alpha = 0.0f;
		}
	}


	//FILL THIS IN DEPENDING ON EXPERIMENT SPECIFICATIONS
	public IEnumerator RunExperiment(){
		if (!ExperimentSettings.isReplay) {
			exp.fullInstructionsPanel.TurnOffInstructions();
			exp.instructionsController.TurnOffInstructions ();

			exp.player.LockControls(true);

			if(ExperimentSettings.isSystem2 || ExperimentSettings.isSyncbox){
				yield return StartCoroutine( WaitForEEGHardwareConnection() );
			}
			else{
				exp.uiController.ConnectionUI.alpha = 0.0f;
			}

			//show instructions for exploring, wait for the action button
			trialLogger.LogInstructionEvent();

			//yield return StartCoroutine(exp.fullInstructionsPanel.ShowSingleInstruction (Config.initialInstructions1, true, true, false, Config.minDefaultInstructionTime));
			exp.fullInstructionsPanel.SetInstructionsColorful();
			yield return StartCoroutine(exp.uiController.TurnOnCanvasGroup(exp.uiController.Part1InstructionsUI, true, 0.0f));
			exp.fullInstructionsPanel.SetInstructionsTransparentOverlay ();

			//let player explore until the button is pressed again
			trialLogger.LogLearningExplorationEvent(true);
			exp.player.LockControls(false);
			yield return StartCoroutine(WaitForLearningPhase());
			trialLogger.LogLearningExplorationEvent(false);
			//yield return StartCoroutine (exp.WaitForActionButton ());

			exp.player.LockControls(true);

			//get the number of blocks so far -- floor half the number of trials recorded
			int totalTrialCount = ExperimentSettings.currentSubject.trials;
			numRealTrials = totalTrialCount;
			if (Config.doPracticeTrial) {
				if (numRealTrials >= 2) { //otherwise, leave numRealTrials at zero.
					numRealTrials -= 1; //-1 for practice trial
				}
			}


			//yield return StartCoroutine(exp.fullInstructionsPanel.ShowSingleInstruction (Config.overheadIntroInstruction, true, true, false, Config.minDefaultInstructionTime));
			exp.fullInstructionsPanel.SetInstructionsColorful();
			yield return StartCoroutine(exp.uiController.TurnOnCanvasGroup(exp.uiController.Part2InstructionsUI, true, 0.0f));
			exp.fullInstructionsPanel.SetInstructionsTransparentOverlay ();

			exp.player.controls.GoToStartPosition();

			//run practice trial(s)
			if(Config.doPracticeTrial){
				isPracticeTrial = true;
			}
			
			if (isPracticeTrial) {

				yield return StartCoroutine (RunTrial ( practiceTrial ));

				Debug.Log ("PRACTICE TRIALS COMPLETED");
				totalTrialCount += 1;
				isPracticeTrial = false;
			}


			//RUN THE REST OF THE TRIALS
			for(int i = 0; i < Config.numTestTrials; i++){
				Debug.Log("NUM TRIALS LEFT: " + TrialList.Count);
				Trial nextTrial = GetNextTrial();
				yield return StartCoroutine (RunTrial ( nextTrial ));

				totalTrialCount += 1;
				Debug.Log ("TRIALS COMPLETED: " + totalTrialCount);
			}

			yield return 0;
		}
		
	}

	IEnumerator WaitForLearningPhase(){
		Debug.Log("Waiting for learning phase!");

		exp.player.controls.GoToStartPosition();

		GameTimer.ResetTimer(Config.totalLearningTime);
		GameTimer.StartTimer();
		while(GameTimer.IsRunning){
			if(exp.learningLocatorController.GetActiveNumLocators() <= 0){
				exp.learningLocatorController.ReActivateAllLocators();
			}
			float timePassed = Config.totalLearningTime - GameTimer.GetSecondsFloat();
			if(timePassed >= Config.learningTimeToPassUntilArrows && !exp.learningLocatorController.hasRegenerated){
				exp.player.shouldUseArrows = true;
			}
			yield return 0;
		}

		exp.player.shouldUseArrows = false;
		exp.player.TurnOnOverheadArrow(false, null);

		//tell locator controller to delete the rest of the coins
		exp.learningLocatorController.DeactivateAllLocators();
	}

	IEnumerator WaitForEEGHardwareConnection(){
		isConnectingToHardware = true;

		exp.uiController.ConnectionUI.alpha = 1.0f;
		if(ExperimentSettings.isSystem2){
			while(!TCPServer.Instance.isConnected || !TCPServer.Instance.canStartGame){
				Debug.Log("Waiting for system 2 connection...");
				yield return 0;
			}
		}
		else if (ExperimentSettings.isSyncbox){
			while(!SyncboxControl.Instance.isUSBOpen){
				Debug.Log("Waiting for sync box to open...");
				yield return 0;
			}
		}
		exp.uiController.ConnectionUI.alpha = 0.0f;
		isConnectingToHardware = false;
	}
	

	//INDIVIDUAL TRIALS -- implement for repeating the same thing over and over again
	//could also create other IEnumerators for other types of trials
	IEnumerator RunTrial(Trial trial){

		exp.player.ResetGatesVisited();

		currentTrial = trial;

		if (isPracticeTrial) {
			trialLogger.LogTrialInfo (-1, currentTrial);
			Debug.Log("Logged practice trial.");
		} 
		else {
			trialLogger.LogTrialInfo (numRealTrials, currentTrial);
			numRealTrials++;
			Debug.Log("Logged trial #: " + numRealTrials);
		}

		if(trial.is3DFirst){
			yield return StartCoroutine(Do3DPhase(true));
			yield return StartCoroutine(Do2DPhase(false));
		}
		else{
			yield return StartCoroutine(Do2DPhase(true));
			yield return StartCoroutine(Do3DPhase(false));
		}

		//increment subject's trial count
		ExperimentSettings.currentSubject.IncrementTrial ();
	}

	IEnumerator Do3DPhase(bool isStart){
		trialLogger.LogFirstPersonTrial(true);
		string goToLocationInstruction = "";

		//TODO: move player to start location!
		//exp.player.transform.position = currentTrial.myTrajectory.startLoc.transform.position;

		if(isStart){
			goToLocationInstruction = "Please go to the " + currentTrial.desiredItemLocation.name + ".";
		}
		else{
			goToLocationInstruction = "Follow the same path to the " + currentTrial.desiredItemLocation.name + " that you took on the overhead map.";
		}
			
		//START NAVIGATION
		trialLogger.LogTrialNavigation (true);

		//unlock avatar controls
		exp.player.LockControls(false);
		
		//tell player where to go next
		exp.instructionsController.SetInstructionsTransparentOverlay();
		exp.instructionsController.DisplayText(goToLocationInstruction);
		
		
		//wait for player to hit target item
		GameTimer.ResetTimer(Config.max3DTrialTime);
		GameTimer.StartTimer();
		yield return StartCoroutine (exp.player.WaitForItemCollision (currentTrial.desiredItemLocation, GameTimer));
		GameTimer.StopTimer();

		if (GameTimer.GetSecondsFloat() <= 0.0f) { //if the time ran out before you got to the target... auto drive there!
			yield return StartCoroutine (exp.player.controls.MoveToTargetItemThroughWaypoints (currentTrial.desiredItemLocation));
		}

		exp.instructionsController.TurnOffInstructions();
		
		trialLogger.LogTrialNavigation (false);

		trialLogger.LogFirstPersonTrial(false);
	}

	IEnumerator Do2DPhase(bool isStart){
		trialLogger.LogOverheadTrial(true);

		string goToLocationInstruction = "";

		if(isStart){
			//TODO: put the player cursor in the right start location!!
			goToLocationInstruction = "Please go to the " + currentTrial.desiredItemLocation.name + " and press (A) when finished.";
		}
		else{
			goToLocationInstruction = "Retrace the path you just took to the " + currentTrial.desiredItemLocation.name + ". \n\nPress (A) when finished.";
		}

		//lock avatar controls
		exp.player.LockControls(true);
		Vector3 playerOrigPos = exp.player.transform.position;
		exp.player.transform.position = exp.player.controls.TwoDPhaseTransform.position;

		exp.overheadMap.TurnOn(true);
		trialLogger.LogTrialNavigation (true);

		if (!isStart) {
			//exp.overheadMap.LockCursor(false);
			//yield return StartCoroutine (exp.instructionsController.ShowSingleInstruction ("Place the cursor where you started, and then press (X).", false, true, false, 0.0f));
		} else {
			//exp.overheadMap.LockCursor(true);
			//yield return StartCoroutine (exp.instructionsController.ShowSingleInstruction ("To begin the trial, press (X).", false, true, false, 0.0f));
			//exp.overheadMap.LockCursor(false);
		}
		//exp.overheadMap.mapCursor.StartPath();
		//yield return StartCoroutine(exp.instructionsController.ShowSingleInstruction(goToLocationInstruction, true, false, false, 3.0f));
		exp.instructionsController.SetInstructionsColorful();
		exp.instructionsController.DisplayText(goToLocationInstruction);
		yield return new WaitForSeconds(3.0f);
		exp.overheadMap.LockCursor(false);
		while(!exp.overheadMap.mapCursor.controls.isMoving){ //wait for input 
			yield return 0;
		}

		//once player starts moving cursor, turn off instructions
		exp.instructionsController.SetInstructionsBlank();
		exp.instructionsController.SetInstructionsTransparentOverlay();

		//wait for button press to indicate travel is over
		yield return StartCoroutine(exp.WaitForActionButton());

		//lock cursor movement, log navigation over
		exp.overheadMap.LockCursor (true);
		trialLogger.LogTrialNavigation (false);

		//yield return StartCoroutine(exp.instructionsController.ShowSingleInstruction("Press (X) to continue to the next trial.", false, true, false, 0.0f));
		exp.overheadMap.TurnOn(false);

		trialLogger.LogOverheadTrial(false);

		//TODO: change this if we change the trial 2D vs 3D first stuff...
		exp.player.controls.Rotate(180.0f);
		exp.player.transform.position = playerOrigPos;

		//move cursor to correct location...
		exp.overheadMap.MoveCursorToLocation(currentTrial.desiredItemLocation.ID);
	}

	IEnumerator ShowFeedback(List<int> specialObjectOrder, List<Vector3> chosenPositions, List<bool> rememberResponses, List<bool> areYouSureResponses){

		yield return 0;
	}
	
	void DestroyGameObjectList(List<GameObject> gameObjectList){
		int numObjects = gameObjectList.Count;
		for (int i = 0; i < numObjects; i++) {
			Destroy (gameObjectList [i]);
		}
		gameObjectList.Clear ();
	}
	
}
