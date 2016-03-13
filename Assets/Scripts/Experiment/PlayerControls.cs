using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerControls : MonoBehaviour{

	Experiment exp  { get { return Experiment.Instance; } }


	public bool ShouldLockControls = false;

	public Transform TiltableTransform;
	public Transform startTransform;
	public Transform TwoDPhaseTransform;

	float RotationSpeed = 30.0f;
	
	//float maxTimeToMove = 3.75f; //seconds to move across the furthest field distance
	//float minTimeToMove = 1.5f; //seconds to move across the closest field distance

	// Use this for initialization
	void Start () {
		//when in replay, we don't want physics collision interfering with anything
		if(ExperimentSettings.isReplay){
			GetComponent<Collider>().enabled = false;
		}
		else{
			GetComponent<Collider>().enabled = true;
		}
	}

	
	public void GoToStartPosition(){
		transform.position = startTransform.position;
		transform.rotation = startTransform.rotation;
	}
	
	// Update is called once per frame
	void Update () {

		if (exp.currentState == Experiment.ExperimentState.inExperiment) {
			if(!ShouldLockControls){
				GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationY; // TODO: on collision, don't allow a change in angular velocity?

				//sets velocities
				GetInput ();
			}
			else{
				GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
				SetTilt(0.0f, 1.0f);
			}
		}
	}

	public Camera myCamera;
	public Transform movementTransform;
	bool wasNotMoving = false;
	void GetInput()
	{
		//VERTICAL
		float verticalAxisInput = Input.GetAxis ("Vertical");
		if ( Mathf.Abs(verticalAxisInput) > 0.0f) { //EPSILON should be accounted for in Input Settings "dead zone" parameter
			Vector3 movementDir = transform.forward;
			float angleDifference = 0;
			if(wasNotMoving){
				//align forward direction with main camera forward direction!
				//myCamera.transform.parent = transform.parent;
				Quaternion cameraRot = myCamera.transform.rotation;
				angleDifference = movementTransform.rotation.eulerAngles.y - cameraRot.eulerAngles.y;
				movementTransform.RotateAround(movementTransform.position, Vector3.up, -angleDifference);
				movementDir = movementTransform.forward;
				//transform.RotateAround(transform.position, Vector3.up, angleDifference);
				//myCamera.transform.parent = transform;
				//myCamera.transform.RotateAround(transform.position, Vector3.up, angleDifference);
			}
			wasNotMoving = false;
			GetComponent<Rigidbody>().velocity = myCamera.transform.forward*verticalAxisInput*Config.driveSpeed; //since we are setting velocity based on input, no need for time.delta time component
			//transform.RotateAround(transform.position, Vector3.up, angleDifference);
		}
		else{
			wasNotMoving = true;
			GetComponent<Rigidbody>().velocity = Vector3.zero;
		}

		//HORIZONTAL
		float horizontalAxisInput = Input.GetAxis ("Horizontal");
		if (Mathf.Abs (horizontalAxisInput) > 0.0f) { //EPSILON should be accounted for in Input Settings "dead zone" parameter

			float percent = horizontalAxisInput / 1.0f;
			Turn (percent * RotationSpeed * Time.deltaTime); //framerate independent!
		} 
		else {
			if(!TrialController.isPaused){

				//resets the player back to center if the game gets paused on a tilt
				//NOTE: after pause is glitchy on keyboard --> unity seems to be retaining some of the horizontal axis input despite there being none. fine with controller though.

				float zTiltBack = 0.2f;
				float zTiltEpsilon = 2.0f * zTiltBack;
				float currentZRot = TiltableTransform.rotation.eulerAngles.z;
				if(currentZRot > 180.0f){
					currentZRot = -1.0f*(360.0f - currentZRot);
				}

				if(currentZRot > zTiltEpsilon){
					TiltableTransform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, currentZRot - zTiltBack);
				}
				else if (currentZRot < -zTiltEpsilon){
					TiltableTransform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, currentZRot + zTiltBack);
				}
				else{
					SetTilt(0.0f, 1.0f);
				}
			}
		}

	}

	void Move( float amount ){
		transform.position += transform.forward * amount;
	}
	
	void Turn( float amount ){
		transform.RotateAround (transform.position, Vector3.up, amount );
		SetTilt (amount, Time.deltaTime);
	}

	//based on amount difference of y rotation, tilt in z axis
	void SetTilt(float amountTurned, float turnTime){
		if (!TrialController.isPaused) {
			if (Config.isAvatarTilting) {
				float turnRate = 0.0f;
				if (turnTime != 0.0f) {
					turnRate = amountTurned / turnTime;
				}
			
				float tiltAngle = turnRate * Config.turnAngleMult;
			
				tiltAngle *= -1; //tilt in opposite direction of the difference
				TiltableTransform.rotation = Quaternion.Euler (transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, tiltAngle);	
			}
		}
	}

	public void Rotate(float degrees){
		transform.RotateAround(transform.position, Vector3.up, degrees);
	}

	public IEnumerator MoveToTargetItemThroughWaypoints(ItemLocation target){
		List<Transform> shortestWaypointPath = exp.houseController.GetShortestWaypointPath(transform.position, target.playerSpotTransform.position);
		Quaternion targetRotation;
		float distance = 0.0f;
		float travelTime = 0.0f;

		Vector3 targetPos = transform.position;
		for(int i = 0; i < shortestWaypointPath.Count; i++){
			Transform targetWaypointTransform = shortestWaypointPath[i];
			bool shouldFinishTurn = false;
			if(i != shortestWaypointPath.Count - 1){
				targetRotation = UsefulFunctions.GetDesiredRotation(transform, targetWaypointTransform);
			}
			else{
				shouldFinishTurn = true;
				targetRotation = UsefulFunctions.GetDesiredRotation(targetWaypointTransform, target.transform);
			}
			distance = UsefulFunctions.GetDistance(transform.position, targetWaypointTransform.position);
			travelTime = distance / Config.autoDriveSpeed;

			targetPos = new Vector3(targetWaypointTransform.position.x, transform.position.y, targetWaypointTransform.position.z);
			yield return StartCoroutine(SmoothMoveTo(targetPos, targetRotation, travelTime, true, shouldFinishTurn));
		}
		/*//move to final target player spot
		targetRotation = UsefulFunctions.GetDesiredRotation(target.playerSpotTransform, target.transform); //we want to be looking at the item FROM the player spot

		targetPos = new Vector3(target.playerSpotTransform.position.x, transform.position.y, target.playerSpotTransform.position.z);
		distance = UsefulFunctions.GetDistance(transform.position, targetPos);

		travelTime = distance / Config.autoDriveSpeed;
		yield return StartCoroutine(SmoothMoveTo(targetPos, targetRotation, travelTime, true));*/
	}

	public IEnumerator SmoothMoveTo(Vector3 targetPosition, Quaternion targetRotation, float timeToTravel, bool usePlayerTurnSpeed, bool finishTurn){

		SetTilt (0.0f, 1.0f);

		//stop collisions
		GetComponent<Collider> ().enabled = false;


		Quaternion origRotation = transform.rotation;
		Vector3 origPosition = transform.position;

		//float travelDistance = (origPosition - targetPosition).magnitude;

		//get total time to rotate at the player's speed
		//Vector3 rotatedForwardDir = UsefulFunctions.GetRotatedForwardDir(transform, targetRotation);
		float angleToRotate = UsefulFunctions.GetSmallestYAngleDifference(transform.rotation, targetRotation);
		float totalTimeToRotate = angleToRotate / RotationSpeed;  //speed is in degrees per second

		float percentageRotationTime = 0.0f;

		float tElapsed = 0.0f;

		//DEBUG
		float totalTimeElapsed = 0.0f;

		while(tElapsed < timeToTravel){
			totalTimeElapsed += Time.deltaTime;

			//tElapsed += (Time.deltaTime * moveAndRotateRate);

			tElapsed += Time.deltaTime;

			float percentageMoveTime = tElapsed / timeToTravel;
			if(totalTimeToRotate != 0){
				percentageRotationTime = tElapsed / totalTimeToRotate;
			}
			if(!usePlayerTurnSpeed){ //if we're not using the player speed, just finish the rotation by the end of the move time
				percentageRotationTime = percentageMoveTime;
			}

			//float amountToRotate = RotationSpeed * Time.deltaTime; //framerate independent!

			//will spherically interpolate the rotation for config.spinTime seconds
			transform.rotation = Quaternion.Slerp(origRotation, targetRotation, percentageRotationTime); //SLERP ALWAYS TAKES THE SHORTEST PATH.
			transform.position = Vector3.Lerp(origPosition, targetPosition, percentageMoveTime);


			yield return 0;
		}

		if(usePlayerTurnSpeed){ //if we're using the player's turn speed...
			while(tElapsed < totalTimeToRotate && finishTurn){ //...and the total rotation time hasn't passed yet... finish rotating!
				tElapsed += Time.deltaTime;
				percentageRotationTime = tElapsed / totalTimeToRotate;
				transform.rotation = Quaternion.Slerp(origRotation, targetRotation, percentageRotationTime); //SLERP ALWAYS TAKES THE SHORTEST PATH.
				yield return 0;
			}
		}
		
		//Debug.Log ("TOTAL TIME ELAPSED FOR SMOOTH MOVE: " + totalTimeElapsed);

		if(finishTurn || !usePlayerTurnSpeed){
			transform.rotation = targetRotation;
		}
		transform.position = targetPosition;

		//enable collisions again
		GetComponent<Collider> ().enabled = true;

		yield return 0;
	}

	public IEnumerator RotateTowardSpecialObject(GameObject target){
		Quaternion origRotation = transform.rotation;
		Quaternion desiredRotation = UsefulFunctions.GetDesiredRotation(transform, target.transform);
		
		float angleDifference = origRotation.eulerAngles.y - desiredRotation.eulerAngles.y;
		angleDifference = Mathf.Abs (angleDifference);
		if (angleDifference > 180.0f) {
			angleDifference = 360.0f - angleDifference;
		}


		float rotationSpeed = 0.03f;
		float totalTimeToRotate = angleDifference * rotationSpeed;

		//rotate to look at target
		transform.rotation = origRotation;

		float tElapsed = 0.0f;
		while (tElapsed < totalTimeToRotate){
			tElapsed += (Time.deltaTime );
			float turnPercent = tElapsed / totalTimeToRotate;

			float beforeRotY = transform.rotation.eulerAngles.y; //y angle before the rotation

			//will spherically interpolate the rotation
			transform.rotation = Quaternion.Slerp(origRotation, desiredRotation, turnPercent); //SLERP ALWAYS TAKES THE SHORTEST PATH.

			float angleRotated = transform.rotation.eulerAngles.y - beforeRotY;
			SetTilt(angleRotated, Time.deltaTime);

			yield return 0;
		}
		
		
		
		transform.rotation = desiredRotation;
		
		//Debug.Log ("TIME ELAPSED WHILE ROTATING: " + tElapsed);
	}

	//returns the angle between the facing angle of the player and an XZ position
	public float GetYAngleBetweenFacingDirAndObjectXZ ( Vector2 objectPos ){

		Quaternion origRotation = transform.rotation;
		Vector3 origPosition = transform.position;

		float origYRot = origRotation.eulerAngles.y;

		transform.position = new Vector3( objectPos.x, origPosition.y, objectPos.y );
		transform.RotateAround(origPosition, Vector3.up, -origYRot);

		Vector3 rotatedObjPos = transform.position;


		//put player back in orig position
		transform.position = origPosition;

		transform.LookAt (rotatedObjPos);


		float yAngle = transform.rotation.eulerAngles.y;

		if(yAngle > 180.0f){
			yAngle = 360.0f - yAngle; //looking for shortest angle no matter the angle
			yAngle *= -1; //give it a signed value
		}

		transform.rotation = origRotation;

		return yAngle;

	}


	
}
