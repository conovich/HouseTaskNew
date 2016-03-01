using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LearningLocatorController : MonoBehaviour {
	Experiment exp { get { return Experiment.Instance; } }

	public bool hasRegenerated = false;
	//public LearningLocator closestLocatorToPlayer;

	List<LearningLocator> activeLocators;
	LearningLocator[] locatorArray;

	// Use this for initialization
	void Start () {
		InitLocators();
	}

	void InitLocators(){
		activeLocators = new List<LearningLocator>();
		locatorArray = GetComponentsInChildren<LearningLocator>();
		for(int i = 0; i < locatorArray.Length; i++){
			activeLocators.Add(locatorArray[i]);
		}
	}

	/*void SetClosestLocatorToPlayer(){
		closestLocatorToPlayer = GetClosestLocator(exp.player.transform.position);
	}*/

	// Update is called once per frame
	void Update () {
		/*if(exp.player.shouldUseArrows){
			SetClosestLocatorToPlayer();
		}*/
	}

	public int GetActiveNumLocators(){
		if (activeLocators == null) {
			return 0;
		} else {
			return activeLocators.Count;
		}
	}

	public void RemoveActiveLocator(LearningLocator locator){
		activeLocators.Remove(locator);
		/*if(locator == closestLocatorToPlayer){
			SetClosestLocatorToPlayer();
		}*/
	}

	public void ReActivateAllLocators(){
		activeLocators.Clear();
		for(int i = 0; i < locatorArray.Length; i++){
			locatorArray[i].Activate();
			activeLocators.Add(locatorArray[i]);
		}
	}

	public void DeactivateAllLocators(){
		for(int i = 0; i < locatorArray.Length; i++){
			activeLocators.Clear();
			locatorArray[i].Deactivate();
		}
	}

	
	/*LearningLocator GetClosestLocator(Vector3 position){
		if(activeLocators.Count == 0){
			return null;
		}

		int minIndex = 0;
		float minDistance = -1;
		for(int i = 0; i < activeLocators.Count; i++){
			float distance = (activeLocators[i].transform.position - position).magnitude;

			if(i == 0){
				minDistance = distance;
			}
			else if(distance < minDistance){
				minDistance = distance;
				minIndex = i;
			}
		}
		
		return activeLocators[minIndex];
	
	}*/

/*	public void PlaceLocators(){
		if (locatorList == null) {
			locatorList = new List<GameObject> ();
		}
		//get distance for even coin distribution
		float locatorPathDistance = 0;
		List<float> roomDistances = new List<float>();
		for(int i = 1; i < exp.houseController.RoomLocators.Length; i++){
			Transform prevTransform = exp.houseController.RoomLocators[i-1];
			Transform currTransform = exp.houseController.RoomLocators[i];

			float roomToRoomDistance = (prevTransform.position - currTransform.position).magnitude;

			locatorPathDistance += roomToRoomDistance;

			roomDistances.Add(roomToRoomDistance);
		}


		Vector3 spawnPos = exp.houseController.RoomLocators[0].transform.position;
		int numLocatorsLeft = TotalNumLocators;
		for(int i = 0; i < roomDistances.Count; i++){
			int currNumLocators = (int)Mathf.CeilToInt((roomDistances[0] / locatorPathDistance) * TotalNumLocators);
			Vector3 currDir = (exp.houseController.RoomLocators[i+1].position - exp.houseController.RoomLocators[i].position);

			float pathIncrement = roomDistances[i] / (float)currNumLocators;
			for(int j = 0; j < currNumLocators; j++){

				if(j != 0){
					spawnPos += (pathIncrement*currDir.normalized);
				}

				numLocatorsLeft--;
			
				spawnPos = new Vector3(spawnPos.x, locatorPrefab.transform.position.y, spawnPos.z);
				GameObject newLocator = Instantiate(locatorPrefab,  spawnPos, locatorPrefab.transform.rotation) as GameObject;
				locatorList.Add(newLocator);
			}

		}

		Debug.Log(numLocatorsLeft);
	}*/

	/*public void RemoveLocator(GameObject locator){
		int locatorIndex = locatorList.IndexOf (locator);
		locatorList.RemoveAt (locatorIndex);
		Destroy(locator);
	}

	public void ClearLocators(){
		if (locatorList != null) {
			int numLocators = locatorList.Count;
			for (int i = 0; i < numLocators; i++) {
				GameObject locator = locatorList [0];
				locatorList.RemoveAt (0);
				Destroy (locator);
			}
		}
	}*/
}
