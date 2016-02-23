using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LearningLocatorController : MonoBehaviour {
	Experiment exp { get { return Experiment.Instance; } }

	//TODO: have locator controller keep track of the coins.

	int TotalNumLocators = 10;
	public GameObject locatorPrefab;

	List<GameObject> locatorList;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

	public int GetActiveNumLocators(){
		if (locatorList == null) {
			return 0;
		} else {
			return locatorList.Count;
		}
	}

	public void PlaceLocators(){
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
	}

	public void RemoveLocator(GameObject locator){
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
	}
}
