using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	Experiment exp { get { return Experiment.Instance; } }

	public PlayerControls controls;
	public GameObject visuals;

	public List<Transform> roomsVisited;
	public Transform currentRoom; //should set initially in scene!

	List<Transform> gatesVisitedThisTrial;

	public bool shouldUseArrows;

	float arrowAngleThreshold = 0.0f; //minimum (absolute) angle between player and chest before arrows should be turned on
	bool rightArrowsOn = true;
	bool leftArrowsOn = true;
	bool overheadArrowOn = true;

	public GameObject rightArrows;
	EnableChildrenLogTrack rightArrowEnableLog;
	public GameObject leftArrows;
	EnableChildrenLogTrack leftArrowEnableLog;
	public GameObject overheadArrow;
	public GameObject overheadArrowVisuals;
	EnableChildrenLogTrack overheadArrowEnableLog;

	ObjectLogTrack myObjLogTrack;
	

	// Use this for initialization
	void Start () {
		rightArrowEnableLog = rightArrows.GetComponent<EnableChildrenLogTrack> ();
		leftArrowEnableLog = leftArrows.GetComponent<EnableChildrenLogTrack> ();
		overheadArrowEnableLog = overheadArrow.GetComponent<EnableChildrenLogTrack> ();

		TurnOnRightArrows (false);
		TurnOnLeftArrows (false);
		TurnOnOverheadArrow(false, transform);

		myObjLogTrack = GetComponent<ObjectLogTrack> ();

		roomsVisited = new List<Transform>();
		gatesVisitedThisTrial = new List<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
		if(shouldUseArrows){
			SetArrows ();
		}
	}

	public void TurnOnVisuals(bool isVisible){
		visuals.SetActive (isVisible);
	}

	List<Transform> GetUnvisitedRooms(){
		//Get unvisited rooms
		List<Transform> unvisitedRooms = new List<Transform>();
		for(int i = 0; i < exp.houseController.RoomLocators.Length; i++){
			if(!roomsVisited.Contains(exp.houseController.RoomLocators[i])){
				unvisitedRooms.Add(exp.houseController.RoomLocators[i]);
			}
		}

		return unvisitedRooms;
	}

	Transform GetClosestUnvisitedRoom(){

		//get the list of unvisited rooms
		List<Transform> unvisitedRooms = GetUnvisitedRooms();

		//if there are no unvisited rooms, return null
		if(unvisitedRooms.Count == 0){
			return null;
		}

		//get the closest unvisited room
		int minIndex = 0;
		float minDistance = -1;
		for(int i = 0; i < unvisitedRooms.Count; i++){
			float distance = (unvisitedRooms[i].transform.position - transform.position).magnitude;
			
			if(i == 0){
				minDistance = distance;
			}
			else if(distance < minDistance){
				minDistance = distance;
				minIndex = i;
			}
		}
		
		return unvisitedRooms[minIndex];
		
	}

	void SetArrows(){
		Transform closestRoom = GetClosestUnvisitedRoom();
		if ( closestRoom != null ) {
			Vector3 targetPos = closestRoom.transform.position;

			float angleBetweenPlayerAndTreasure = controls.GetYAngleBetweenFacingDirAndObjectXZ( targetPos );

			TurnOnOverheadArrow(true, closestRoom);
			Debug.Log("Closest room: " + closestRoom.name);

			//if the angle is bigger than the threshold, turn on the appropriate arrows
			/*if( Mathf.Abs(angleBetweenPlayerAndTreasure) > arrowAngleThreshold){
				if(angleBetweenPlayerAndTreasure < 0){
					TurnOnLeftArrows(true);
					TurnOnRightArrows(false);
				}
				else{
					TurnOnRightArrows(true);
					TurnOnLeftArrows(false);
				}
			}
			//if smaller than the threshold, turn off all arrows
			else{
				TurnOnLeftArrows(false);
				TurnOnRightArrows(false);
			}*/
		}
		else if(rightArrowsOn){
			TurnOnRightArrows(false);
		}
		else if(leftArrowsOn){
			TurnOnLeftArrows(false);
		}
		else if(overheadArrowOn){
			TurnOnOverheadArrow(false, transform);
		}
	}

	public void TurnOnOverheadArrow(bool shouldTurnOn, Transform transformToLookAt){
		//only toggle them if they aren't in the shouldTurnOn state
		if (overheadArrowOn == !shouldTurnOn) {
			UsefulFunctions.EnableChildren (overheadArrow.transform, shouldTurnOn);
			overheadArrowEnableLog.LogChildrenEnabled (shouldTurnOn);
			overheadArrowOn = shouldTurnOn;
		}
		if(transformToLookAt != null){
			overheadArrowVisuals.transform.LookAt(transformToLookAt);
		}
	}

	public void TurnOnRightArrows(bool shouldTurnOn){
		//only toggle them if they aren't in the shouldTurnOn state
		if (rightArrowsOn == !shouldTurnOn) {
			UsefulFunctions.EnableChildren (rightArrows.transform, shouldTurnOn);
			rightArrowEnableLog.LogChildrenEnabled (shouldTurnOn);
			rightArrowsOn = shouldTurnOn;
		}
	}

	public void TurnOnLeftArrows(bool shouldTurnOn){
		//only toggle them if they aren't in the shouldTurnOn state
		if (leftArrowsOn == !shouldTurnOn) {
			UsefulFunctions.EnableChildren (leftArrows.transform, shouldTurnOn);
			leftArrowEnableLog.LogChildrenEnabled (shouldTurnOn);
			leftArrowsOn = shouldTurnOn;
		}
	}

	void OnTriggerEnter(Collider collider){
		if(collider.tag == "RoomLocator"){
			if(!roomsVisited.Contains(collider.transform)){
				roomsVisited.Add(collider.transform);
			}
			Debug.Log("Room entered: " + collider.name);
			currentRoom = collider.transform;
		}
		else if(collider.tag == "Gate"){
			gatesVisitedThisTrial.Add(collider.transform);
			Debug.Log("Gate name: " + collider.name);
		}
	}

	public void ResetGatesVisited(){
		gatesVisitedThisTrial.Clear();
	}

	GameObject currentCollisionObject;
	void OnCollisionEnter(Collision collision){
		if(collision.gameObject.tag == "LearningLocator"){
			exp.scoreController.AddLocatorCollectedPoints();
		}

		currentCollisionObject = collision.gameObject;
		
		//log store collision
		if (collision.gameObject.tag == "ItemLocation"){
			myObjLogTrack.LogCollision (collision.gameObject.name);
		}
	}

	public GameObject GetCollisionObject(){
		return currentCollisionObject;
	}

	public IEnumerator WaitForItemCollision(ItemLocation item){
		Debug.Log("WAITING FOR COLLISION WITH: " + item.name);
		
		string lastCollisionName = "";
		//while(true){
		while (lastCollisionName != item.name) {
			if(currentCollisionObject != null){
				lastCollisionName = currentCollisionObject.name;
			}
			yield return 0;
		}
		
		Debug.Log ("FOUND ITEM");
		
	}


}
