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

	//UI
	public CanvasGroup PauseUI;
	public CanvasGroup ConnectionUI;

	public SimpleTimer LearningTimer;

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
			bool is3Dfirst = true;
			if(i % 2 == 0){
				is3Dfirst = false;
			}
			Trial trial = new Trial(is3Dfirst);
			TrialList.Add(trial);
		}

		if(Config.doPracticeTrial){
			practiceTrial = new Trial(true);	//2 special objects for practice trial
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
			PauseUI.alpha = 1.0f;
			Time.timeScale = 0.0f;
		} 
		else {
			Time.timeScale = 1.0f;
			//exp.player.controls.Pause(false);
			//exp.player.controls.ShouldLockControls = false;
			PauseUI.alpha = 0.0f;
		}
	}


	//FILL THIS IN DEPENDING ON EXPERIMENT SPECIFICATIONS
	public IEnumerator RunExperiment(){
		if (!ExperimentSettings.isReplay) {
			exp.fullInstructionsPanel.TurnOffInstructions();

			exp.player.controls.ShouldLockControls = true;

			if(ExperimentSettings.isSystem2 || ExperimentSettings.isSyncbox){
				yield return StartCoroutine( WaitForEEGHardwareConnection() );
			}
			else{
				ConnectionUI.alpha = 0.0f;
			}

			//show instructions for exploring, wait for the action button
			trialLogger.LogInstructionEvent();
			yield return StartCoroutine (exp.instructionsController.ShowSingleInstruction (Config.initialInstructions1, true, true, false, Config.minInitialInstructionsTime));
			yield return StartCoroutine (exp.instructionsController.ShowSingleInstruction (Config.initialInstructions2, true, true, false, Config.minInitialInstructionsTime));

			//let player explore until the button is pressed again
			trialLogger.LogLearningExplorationEvent(true);
			exp.player.controls.ShouldLockControls = false;
			yield return StartCoroutine(WaitForLearningPhase());
			trialLogger.LogLearningExplorationEvent(false);
			//yield return StartCoroutine (exp.WaitForActionButton ());

			exp.player.controls.ShouldLockControls = true;
			yield return StartCoroutine(exp.instructionsController.ShowSingleInstruction("Great Job! Time to move on to the real task.", true, true, false, Config.minInitialInstructionsTime ));
			exp.player.controls.ShouldLockControls = false;

			//get the number of blocks so far -- floor half the number of trials recorded
			int totalTrialCount = ExperimentSettings.currentSubject.trials;
			numRealTrials = totalTrialCount;
			if (Config.doPracticeTrial) {
				if (numRealTrials >= 2) { //otherwise, leave numRealTrials at zero.
					numRealTrials -= 1; //-1 for practice trial
				}
			}

			string overheadIntroInstruction = "You will now be asked to go to various locations in the apartment to pick up things. You will also be asked to show your path on a map." +
				"\n\n Press (X) to begin.";
			yield return StartCoroutine(exp.fullInstructionsPanel.ShowSingleInstruction (overheadIntroInstruction, true, true, false, Config.minDefaultInstructionTime));

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

		LearningTimer.SetTimerMaxTime(Config.totalLearningTime);
		LearningTimer.StartTimer();
		while(LearningTimer.IsRunning){
			if(exp.learningLocatorController.GetActiveNumLocators() <= 0){
				exp.learningLocatorController.ReActivateAllLocators();
			}
			float timePassed = Config.totalLearningTime - LearningTimer.GetSecondsFloat();
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

		ConnectionUI.alpha = 1.0f;
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
		ConnectionUI.alpha = 0.0f;
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
		exp.player.controls.ShouldLockControls = false;
		
		//tell player where to go next
		exp.instructionsController.SetInstructionsTransparentOverlay();
		exp.instructionsController.DisplayText(goToLocationInstruction);
		
		
		//wait for player to hit target item
		yield return StartCoroutine (exp.player.WaitForItemCollision (currentTrial.desiredItemLocation));
		exp.instructionsController.TurnOffInstructions();
		
		trialLogger.LogTrialNavigation (false);

		trialLogger.LogFirstPersonTrial(false);
	}

	IEnumerator Do2DPhase(bool isStart){
		trialLogger.LogOverheadTrial(true);

		string goToLocationInstruction = "";


		if(isStart){
			//TODO: put the player cursor in the right start location!!
			goToLocationInstruction = "Please go to the " + currentTrial.desiredItemLocation.name + ".";
		}
		else{
			goToLocationInstruction = "Show us the path you took to the " + currentTrial.desiredItemLocation.name + ", and press (X) when finished.";
		}

		//unlock avatar controls
		exp.player.controls.ShouldLockControls = true;

		exp.overheadMap.TurnOn(true);
		trialLogger.LogTrialNavigation (true);
		exp.overheadMap.LockCursor(false);
		if(!isStart){
			yield return StartCoroutine(exp.instructionsController.ShowSingleInstruction("Place the cursor where you started, and then press (X).", false, true, false, 0.0f));
		}
		exp.overheadMap.mapCursor.StartPath();
		yield return StartCoroutine(exp.instructionsController.ShowSingleInstruction(goToLocationInstruction, false, true, false, 0.0f));
		exp.overheadMap.LockCursor (true);

		trialLogger.LogTrialNavigation (false);

		yield return StartCoroutine(exp.instructionsController.ShowSingleInstruction("Press (X) to continue to the next trial.", false, true, false, 0.0f));
		exp.overheadMap.TurnOn(false);

		trialLogger.LogOverheadTrial(false);
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
