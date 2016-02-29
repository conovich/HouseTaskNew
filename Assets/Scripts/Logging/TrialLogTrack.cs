using UnityEngine;
using System.Collections;

public class TrialLogTrack : LogTrack {


	bool firstLog = false;

	//log on late update so that everything for that frame gets set first
	void LateUpdate () {
		//just log the environment info on the first frame
		if (ExperimentSettings.isLogging && !firstLog) {
			LogEnvironmentDimensions ();
			firstLog = true;
		}
	}

	//gets called from trial controller instead of in update!
	public void LogTrialInfo(int trialNumber, Trial trial){
		if (ExperimentSettings.isLogging) {
			LogTrialNum (trialNumber);
			LogTrialTrajectory(trial.myTrajectory.startLoc.name, trial.myTrajectory.endLoc.name);
		}
	}

	//LOGGED ON THE START OF THE TRIAL.
	void LogTrialNum(int trialNumber){
		subjectLog.Log (GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "Trial Info" + separator + "NUM_TRIALS" + separator + trialNumber);
	}
	
	void LogTrialTrajectory(string startLocation, string endLocation){
		subjectLog.Log (GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "Trial Info" + separator + "TRIAL_TRAJECTORY_START" + separator + startLocation + separator + "TRIAL_TRAJECTORY_END" + separator + endLocation);
	}

	void LogTrialType(bool is3DFirst){
		if(is3DFirst){
			subjectLog.Log (GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "Trial Info" + separator + "TRIAL_TYPE" + separator + "3DFIRST");
		}
		else{
			subjectLog.Log (GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "Trial Info" + separator + "TRIAL_TYPE" + separator + "2DFIRST");
		}
	}


	//TODO: move to an experiment or an environment logger... just want to log this once at the beginning of the trials so there is a reference for all positions in the world.
	void LogEnvironmentDimensions(){
		//log center
		Vector3 envCenter = exp.environmentController.center;
		subjectLog.Log (GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "Experiment Info" + separator + "ENV_CENTER" + separator + envCenter.x + separator + envCenter.y + separator + envCenter.z);
	
		//log walls
		Vector3 wallPos = exp.environmentController.WallsXPos.position;
		subjectLog.Log (GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "Experiment Info" + separator + "ENV_WALL_XPOS" + separator + wallPos.x + separator + wallPos.y + separator + wallPos.z);

		wallPos = exp.environmentController.WallsXNeg.position;
		subjectLog.Log (GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "Experiment Info" + separator + "ENV_WALL_XNEG" + separator + wallPos.x + separator + wallPos.y + separator + wallPos.z);

		wallPos = exp.environmentController.WallsZPos.position;
		subjectLog.Log (GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "Experiment Info" + separator + "ENV_WALL_ZPOS" + separator + wallPos.x + separator + wallPos.y + separator + wallPos.z);

		wallPos = exp.environmentController.WallsZNeg.position;
		subjectLog.Log (GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "Experiment Info" + separator + "ENV_WALL_ZNEG" + separator + wallPos.x + separator + wallPos.y + separator + wallPos.z);
		Debug.Log ("LOGGED ENV");
	}


	//TODO: move to an experiment logger
	public void LogWaitForJitterStarted(float jitter){
		subjectLog.Log (GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), gameObject.name + separator + "RANDOM_JITTER_STARTED" + separator + jitter);
		Debug.Log ("JITTER STARTED LOGGED: " + jitter);
	}
	
	//TODO: move to an experiment logger
	public void LogWaitForJitterEnded(float jitter){
		subjectLog.Log (GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), gameObject.name + separator + "RANDOM_JITTER_ENDED" + separator + jitter);
		Debug.Log ("JITTER ENDED LOGGED: " + jitter);
	}




	public void LogAreYouSureResponse(bool response){
		//TODO: CHANGE THE "DOUBLE DOWN" TO ARE YOU SURE OR SOMETHING.
		subjectLog.Log (GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), gameObject.name + separator + "DOUBLE_DOWN_RESPONSE" + separator + response);
		Debug.Log ("DOUBLE DOWN LOGGED: " + response);
	}

	public void LogRememberResponse(bool response){
		subjectLog.Log (GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), gameObject.name + separator + "REMEMBER_RESPONSE" + separator + response);
		Debug.Log ("REMEMBER LOGGED: " + response);
	}

	//if the UI answer selector has moved TODO: move to an answer selector logger?
	public void LogAnswerPositionMoved(bool isYesPosition, bool isRememberResponse){ //either remember response or double down response
		string answerPosition = "NO";
		if (isYesPosition) {
			answerPosition = "YES";
		}

		if(isRememberResponse){
			subjectLog.Log (GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), gameObject.name + separator + "REMEMBER_ANSWER_MOVEMENT" + separator + answerPosition);
			Debug.Log ("REMEMBER MOVEMENT LOGGED: " + answerPosition);
		}
		else{
			//TODO: CHANGE THE "DOUBLE DOWN" TO ARE YOU SURE OR SOMETHING.
			subjectLog.Log (GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), gameObject.name + separator + "DOUBLE_DOWN_ANSWER_MOVEMENT" + separator + answerPosition);
			Debug.Log ("DOUBLE DOWN MOVEMENT LOGGED: " + answerPosition);
		}
	}


	//THE FOLLOWING ARE EVENTS
	public void LogPauseEvent(bool isPaused){
		subjectLog.Log (GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), gameObject.name + separator + "TASK_PAUSED" + separator + isPaused); //logged for replay
		subjectLog.Log (GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), "Trial Event" + separator + "TASK_PAUSED" + separator + isPaused); //logged for parsing events
		Debug.Log ("Logged pause event. isPaused: " + isPaused);
	}

	public void LogInstructionEvent(){
		if (ExperimentSettings.isLogging) {
			subjectLog.Log (GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), "Trial Event" + separator + "SHOWING_INSTRUCTIONS");
			Debug.Log ("Logged instruction event.");
		}
	}

	public void LogLearningExplorationEvent(bool isStarting){
		if (ExperimentSettings.isLogging) {
			if(isStarting){
				subjectLog.Log (GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), "Trial Event" + separator + "LEARNING_EXPLORATION_STARTED");
				Debug.Log ("Logged learning event.");
			}
			else{
				subjectLog.Log (GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), "Trial Event" + separator + "LEARNING_EXPLORATION_ENDED");
				Debug.Log ("Logged learning event.");
			}
		}
	}

	public void LogFirstPersonTrial(bool isStarting){
		if (ExperimentSettings.isLogging) {
			if(isStarting){
				subjectLog.Log (GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), "Trial Event" + separator + "3DTRIAL_STARTED");
				Debug.Log ("Logged 3D trial started event.");
			}
			else{
				subjectLog.Log (GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), "Trial Event" + separator + "3DTRIAL_ENDED");
				Debug.Log ("Logged 3D trial ended event.");
			}
		}
	}

	public void LogOverheadTrial(bool isStarting){
		if (ExperimentSettings.isLogging) {
			if(isStarting){
				subjectLog.Log (GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), "Trial Event" + separator + "2DTRIAL_STARTED");
				Debug.Log ("Logged 2D trial started event.");
			}
			else{
				subjectLog.Log (GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), "Trial Event" + separator + "2DTRIAL_ENDED");
				Debug.Log ("Logged 2D trial ended event.");
			}
		}
	}

	public void LogTrialNavigation(bool isStarting){
		if (ExperimentSettings.isLogging) {
			if(isStarting){
				subjectLog.Log (GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), "Trial Event" + separator + "TRIAL_NAVIGATION_STARTED");
				Debug.Log ("Logged nav started event.");
			}
			else{
				subjectLog.Log (GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), "Trial Event" + separator + "TRIAL_NAVIGATION_ENDED");
				Debug.Log ("Logged nav ended event.");
			}
		}
	}

	public void LogFeedback(bool isStarting){
		if (ExperimentSettings.isLogging) {
			if(isStarting){
				subjectLog.Log (GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), "Trial Event" + separator + "FEEDBACK_STARTED");
				Debug.Log ("Logged feedback started event.");
			}
			else{
				subjectLog.Log (GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), "Trial Event" + separator + "FEEDBACK_ENDED");
				Debug.Log ("Logged feedback started event.");
			}
		}
	}

}