using UnityEngine;
using System.Collections;

public class PlayerEventLogTrack : LogTrack {


	bool firstLog = false;

	//log on late update so that everything for that frame gets set first
	void LateUpdate () {
		//just log the environment info on the first frame
		if (ExperimentSettings.isLogging && !firstLog) {
			LogCurrentRoom ();
			firstLog = true;
		}
	}

	public void LogCurrentRoom(){
		if (ExperimentSettings.isLogging) {
			subjectLog.Log (GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), gameObject.name + separator + "CURRENT_ROOM" + separator + exp.player.currentRoom.name);
		}
	}

	public void LogGateCollision(GameObject gateEntered){
		if (ExperimentSettings.isLogging) {
			subjectLog.Log (GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), gameObject.name + separator + "GATE_ENTERED" + separator + gateEntered.name);
		}
	}

	public void LogPlayerCollision(GameObject collisionObj){
		if (ExperimentSettings.isLogging) {
			subjectLog.Log (GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), gameObject.name + separator + "PLAYER_COLLISION" + separator + collisionObj.name);
		}
	}

}