using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class HouseController : MonoBehaviour {

	public TextAsset LocationIDFile;
	public TextAsset LocationOrderFile;

	public List<ItemLocation> trajectoriesOrder;

	public Transform[] RoomLocators;

	public List<ItemLocation> ItemLocations { get { return GetItemLocations(); } } //should typically use this one -- does a null check for itemLocations, and fills in list if necessary.
	List<ItemLocation> itemLocations;

	public List<Gate> HouseGates;
	public List<Waypoint> HouseWaypoints;


	void Awake(){
		ProcessLocationIDs ();
		InitTrajectoriesOrder ();
		GetHouseGates();
		GetWaypoints ();
	}

	void ProcessLocationIDs(){
		string[] lines = LocationIDFile.ToString().Split('\n');
		foreach (string line in lines) {
			if(line != ""){
				string[] columns = line.Split('\t');

				int ID = int.Parse(columns[0]);
				string locName = columns[1];

				locName = locName.Replace("\r", "");

				/*ItemLocation currLoc = GetItemByName(locName);
				if(currLoc != null){
					currLoc.SetID(ID);
				}*/
				ItemLocation currLoc = GetItemByID(ID);
				if(currLoc != null){
					currLoc.name = locName;
				}
			}
		}
	}

	void InitTrajectoriesOrder(){
		trajectoriesOrder = new List<ItemLocation> ();

		string[] lines = LocationOrderFile.ToString().Split('\n');

		if (ExperimentSettings.trajectoriesPath != "") {
			lines = File.ReadAllLines(ExperimentSettings.trajectoriesPath);
		}

		foreach (string line in lines) {
			if(line != ""){

				string cleanLine = line.Replace("\r", "");
				string[] locIDsPerSession = cleanLine.Split('\t');

				int sessionIndex = Experiment.sessionID;
				if(Experiment.sessionID >= locIDsPerSession.Length){
					sessionIndex = locIDsPerSession.Length - 1;
					Debug.Log("TRIAL TYPES: Not enough sessions to choose from!");
				}


				string locIDString = locIDsPerSession[sessionIndex];
				locIDString = locIDString.Replace("\r", "");
				int locID = int.Parse(locIDString);
				
				ItemLocation currLoc = GetItemByID(locID);
				if(currLoc != null){
					trajectoriesOrder.Add(currLoc);
				}
			}
		}
	}

	ItemLocation GetItemByName(string name){
		for(int i = 0; i < ItemLocations.Count; i++){
			if(ItemLocations[i].name == name){
				return ItemLocations[i];
			}
		}
		return null;
	}

	ItemLocation GetItemByID(int ID){
		for(int i = 0; i < ItemLocations.Count; i++){
			if(ItemLocations[i].GetID() == ID){
				return ItemLocations[i];
			}
		}
		return null;
	}

	void GetHouseGates(){
		HouseGates = new List<Gate>();
		GameObject[] gateObjArray = GameObject.FindGameObjectsWithTag("Gate");
		for(int i = 0; i < gateObjArray.Length; i++){
			HouseGates.Add(gateObjArray[i].GetComponent<Gate>());
		}
	}

	void GetWaypoints(){
		HouseWaypoints = new List<Waypoint>();
		GameObject[] waypointObjArray = GameObject.FindGameObjectsWithTag("Waypoint");
		for(int i = 0; i < waypointObjArray.Length; i++){
			HouseWaypoints.Add(waypointObjArray[i].GetComponent<Waypoint>());
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	List<ItemLocation> GetItemLocations(){
		if(itemLocations != null){
			return itemLocations;
		}
		else{
			itemLocations = new List<ItemLocation>();
			ItemLocation[] itemLocationArr = GetComponentsInChildren<ItemLocation>();
			for(int i = 0; i < itemLocationArr.Length; i++){
				itemLocations.Add(itemLocationArr[i]);
			}
			return itemLocations;
		}
	}

	int nextItemIndex = 1;
	public ItemLocation ChooseNextItem(){
		if (nextItemIndex < trajectoriesOrder.Count) {
			nextItemIndex++;
			return trajectoriesOrder [nextItemIndex - 1];
		}
		else{
			Debug.Log("No more item locations!");
			return null;
		}
	}


	//Dijkstra's shortest path for waypoints!
	//https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm
	public List<Transform> GetShortestWaypointPath(Vector3 startPosition, Vector3 endPosition){

		List<Transform> waypointPath = new List<Transform> ();

		//1. Find starting node, ending node, set all other node distances to infinity (incl. end node)
		Waypoint startPoint = HouseWaypoints [0];
		startPoint.DijkstraDistance = 0;
		float minDistanceToStart = (startPosition - startPoint.transform.position).magnitude;

		Waypoint endPoint = HouseWaypoints [0]; //don't set endpoint distance to -inf until later (though it should get set in looking for the start node anyway...)
		float minDistanceToEnd = (endPosition - endPoint.transform.position).magnitude;

		for (int i = 1; i < HouseWaypoints.Count; i++) { //already used the 0 index to initialize, so start at index 1
			Waypoint currPoint = HouseWaypoints[i];
			float startToCurrPointDist = (startPosition - currPoint.transform.position).magnitude;
			float endToCurrPointDist = (endPosition - currPoint.transform.position).magnitude;

			//min distance check
			if(startToCurrPointDist < minDistanceToStart){
				startPoint.DijkstraDistance = Mathf.Infinity;	//set old start point distance to infinity
				currPoint.DijkstraDistance = 0;		//set new start point distance to 0
				startPoint = currPoint;				//set new start point
				minDistanceToStart = startToCurrPointDist;	//set new min distance
			}
			else{
				HouseWaypoints[i].DijkstraDistance = Mathf.Infinity;	//if it's not the shortest distance, set it's distance to infinity
			}

			//max distance check
			if(endToCurrPointDist < minDistanceToEnd){
				endPoint = currPoint;						//set new end point
				minDistanceToEnd = endToCurrPointDist;	//set new max distance
			}
		}

		if (endPoint != startPoint) {
			endPoint.DijkstraDistance = Mathf.Infinity;
		}

		//2. Set the initial node as current. Mark all other nodes unvisited. Create a set of all the unvisited nodes called the unvisited set.
		Waypoint currNode = startPoint;
		List<Waypoint> unvisitedPoints = new List<Waypoint> ();
		for (int i = 0; i < HouseWaypoints.Count; i++) {
			if(HouseWaypoints[i] != startPoint){
				unvisitedPoints.Add(HouseWaypoints[i]);
			}
		}


		while (unvisitedPoints.Count > 0) {

			//3. For the current node, consider all of its unvisited neighbors and calculate their tentative distances.
			//Compare the newly calculated tentative distance to the current assigned value and assign the smaller one.
			//For example, if the current node A is marked with a distance of 6, and the edge connecting it with a neighbor B has length 2,
			//...then the distance to B (through A) will be 6 + 2 = 8. If B was previously marked with a distance greater than 8 then change it to 8.
			//Otherwise, keep the current value.
			float neighborDistance = 0;
			for (int i = 0; i < currNode.neighbors.Count; i++) {
				Waypoint currNeighbor = currNode.neighbors [i];
				neighborDistance = (currNode.transform.position - currNeighbor.transform.position).magnitude;
				if (currNeighbor.DijkstraDistance > neighborDistance + currNode.DijkstraDistance) {
					currNeighbor.DijkstraDistance = neighborDistance + currNode.DijkstraDistance;
				}
			}

			//4. When we are done considering all of the neighbors of the current node, mark the current node as visited and remove it
			//from the unvisited set. A visited node will never be checked again.
			unvisitedPoints.Remove (currNode);

			//5. If the destination node has been marked visited (when planning a route between two specific nodes)
			//or if the smallest tentative distance among the nodes in the unvisited set is infinity (when planning a complete traversal;
			//occurs when there is no connection between the initial node and remaining unvisited nodes), then stop. The algorithm has finished.
			if(currNode == endPoint){
				break;
			}

			//6. Otherwise, select the unvisited node that is marked with the smallest tentative distance, set it as the new "current node", and go back to step 3.
			float smallestDist = Mathf.Infinity;
			int smallestDistIndex = -1;
			for(int i = 0; i < unvisitedPoints.Count; i++){
				if(unvisitedPoints[i].DijkstraDistance < smallestDist){
					smallestDist = unvisitedPoints[i].DijkstraDistance;
					smallestDistIndex = i;
				}
			}

			currNode = unvisitedPoints[smallestDistIndex];
		}

		//7. Get the path!
		//currNode  at the start of this loop currently = endPoint
		while(currNode != startPoint){
			waypointPath.Add(currNode.transform);

			//go through the neighbors, pick the one with the smallest distance
			float smallestDist = currNode.DijkstraDistance;	//distance should definitely be smaller than the current node
			int smallestDistIndex = -1;
			for(int i = 0; i < currNode.neighbors.Count; i++){
				if(currNode.neighbors[i].DijkstraDistance < smallestDist){
					smallestDist = currNode.neighbors[i].DijkstraDistance;
					smallestDistIndex = i;
				}
			}

			if (smallestDistIndex == -1) {
				Debug.Log("NO SMALLEST DISTANCE?! " + currNode.name + " " + currNode.DijkstraDistance);
				for (int i = 0; i < currNode.neighbors.Count; i++) {
					Debug.Log ("neighbor: " + currNode.neighbors [i].name + " " + currNode.neighbors [i].DijkstraDistance);
				}
			}
			currNode = currNode.neighbors[smallestDistIndex];
		}

		//decide whether or not to add the start point:
		//if the start point is between the player(start position) and the next point, add it.
		Vector3 nextPointPosition = endPosition;
		if (waypointPath.Count > 0) {
			nextPointPosition = waypointPath[waypointPath.Count - 1].transform.position;
		}
		if(!CheckBetweenPoints(startPosition, startPoint.transform.position, nextPointPosition)){
			waypointPath.Add(startPoint.transform);
		}


		//since the current order is from the end point to the start point, reverse the list
		waypointPath.Reverse ();



		return waypointPath;
	}

	//THIS FUNCTION IS MERELY CHECKING IF THE BETWEEN POINT IS BETWEEN THE COORDINATES OF THE OTHER TWO POINTS.
	//WE ARE *NOT* CHECKING IF THE BETWEEN POINT LIES PERFECTLY ON THE LINE A-B.
	bool CheckBetweenPoints(Vector3 betweenPoint, Vector3 endPtA, Vector3 endPtB){
		//CASE 1: if points are the same, return true
		if (endPtA.x == endPtB.x && endPtA.z == endPtB.z) {
			return true;
		}

		//CASES 2&3: both endpoints share an x coord
		if (endPtA.x == endPtB.x) {
			if (endPtA.z < endPtB.z) {
				if (betweenPoint.z < endPtB.z && betweenPoint.z > endPtA.z) {
					return true;
				}
			} else if (endPtA.z > endPtB.z) {
				if (betweenPoint.z > endPtB.z && betweenPoint.z < endPtA.z) {
					return true;
				}
			} else {
				return false;
			}
		}
		//CASES 2&3: both endpoints share z coord
		else if (endPtA.z == endPtB.z) {
			if (endPtA.x < endPtB.x) {
				if (betweenPoint.x < endPtB.x && betweenPoint.x > endPtA.x) {
					return true;
				}
			} else if (endPtA.x > endPtB.x) {
				if (betweenPoint.x > endPtB.x && betweenPoint.x < endPtA.x) {
					return true;
				}
			} else {
				return false;
			}
		}

		//CASES 3&4: endpoints to not share either coordinate, A.x < B.x
		else if (endPtA.x < endPtB.x) {
			if (betweenPoint.x > endPtA.x && betweenPoint.x < endPtB.x) {
				//A.z < B.z
				if (endPtA.z < endPtB.z) {
					if (betweenPoint.z > endPtA.z && betweenPoint.z < endPtB.z) {
						return true;
					}
				}
				//A.z > B.z
				else if (endPtA.z > endPtB.z) {
					if (betweenPoint.z < endPtA.z && betweenPoint.z > endPtB.z) {
						return true;
					}
				}
			} else {
				return false;
			}
		}

		//CASES 5&6: endpoints to not share either coordinate, A.x > B.x
		else if (endPtA.x > endPtB.x) {
			if (betweenPoint.x < endPtA.x && betweenPoint.x > endPtB.x) {
				//A.z < B.z
				if (endPtA.z < endPtB.z) {
					if (betweenPoint.z > endPtA.z && betweenPoint.z < endPtB.z) {
						return true;
					}
				}
				//A.z > B.z
				else if (endPtA.z > endPtB.z) {
					if (betweenPoint.z < endPtA.z && betweenPoint.z > endPtB.z) {
						return true;
					}
				}
			} else {
				return false;
			}
		}

		return false;

	}

}
