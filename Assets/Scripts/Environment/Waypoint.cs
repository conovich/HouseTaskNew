using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Waypoint : MonoBehaviour {
	public List<Waypoint> neighbors;
	public List<int> neighborIDList;

	public int ID;
	public float DijkstraDistance = 0.0f;

	Experiment exp { get { return Experiment.Instance; } }

	// Use this for initialization
	void Start () {
		GetNeighbors ();
	}

	void GetNeighborIDs(){
		neighborIDList = new List<int> ();
		switch (ID) {
		case 0:
			neighborIDList.Add (1);
			neighborIDList.Add (2);
			neighborIDList.Add (10);
			neighborIDList.Add (9);
			neighborIDList.Add (16);
			neighborIDList.Add (17);
			break;
		case 1:
			neighborIDList.Add (0);
			neighborIDList.Add (10);
			break;
		case 2:
			neighborIDList.Add (0);
			neighborIDList.Add (10);
			neighborIDList.Add (11);
			break;
		case 3:
			neighborIDList.Add (11);
			neighborIDList.Add (12);
			neighborIDList.Add (4);
			neighborIDList.Add (6);
			neighborIDList.Add (24);
			break;
		case 4:
			neighborIDList.Add (3);
			neighborIDList.Add (12);
			neighborIDList.Add (13);
			neighborIDList.Add (5);
			break;
		case 5:
			neighborIDList.Add (13);
			neighborIDList.Add (4);
			neighborIDList.Add (21);
			neighborIDList.Add (6);
			neighborIDList.Add (19);
			break;
		case 6:
			neighborIDList.Add (3);
			neighborIDList.Add (24);
			neighborIDList.Add (19);
			neighborIDList.Add (21);
			neighborIDList.Add (5);
			break;
		case 7:
			neighborIDList.Add (25);
			neighborIDList.Add (19);
			neighborIDList.Add (20);
			break;
		case 8:
			neighborIDList.Add (15);
			neighborIDList.Add (25);
			neighborIDList.Add (16);
			neighborIDList.Add (18);
			break;
		case 9:
			neighborIDList.Add (0);
			neighborIDList.Add (3);
			neighborIDList.Add (24);
			neighborIDList.Add (15);
			neighborIDList.Add (16);
			neighborIDList.Add (17);
			break;
		case 10:
			neighborIDList.Add (0);
			neighborIDList.Add (2);
			neighborIDList.Add (1);
			break;
		case 11:
			neighborIDList.Add (12);
			neighborIDList.Add (3);
			neighborIDList.Add (2);
			break;
		case 12:
			neighborIDList.Add (11);
			neighborIDList.Add (3);
			neighborIDList.Add (4);
			break;
		case 13:
			neighborIDList.Add (4);
			neighborIDList.Add (5);
			neighborIDList.Add (14);
			break;
		case 14:
			neighborIDList.Add (13);
			neighborIDList.Add (23);
			break;
		case 15:
			neighborIDList.Add (25);
			neighborIDList.Add (24);
			neighborIDList.Add (9);
			neighborIDList.Add (8);
			break;
		case 16:
			neighborIDList.Add (18);
			neighborIDList.Add (17);
			neighborIDList.Add (9);
			neighborIDList.Add (0);
			neighborIDList.Add (8);
			break;
		case 17:
			neighborIDList.Add (18);
			neighborIDList.Add (16);
			neighborIDList.Add (0);
			neighborIDList.Add (9);
			break;
		case 18:
			neighborIDList.Add (16);
			neighborIDList.Add (8);
			break;
		case 19:
			neighborIDList.Add (6);
			neighborIDList.Add (7);
			neighborIDList.Add (5);
			neighborIDList.Add (20);
			neighborIDList.Add (21);
			break;
		case 20:
			neighborIDList.Add (19);
			neighborIDList.Add (22);
			neighborIDList.Add (21);
			neighborIDList.Add (7);
			break;
		case 21:
			neighborIDList.Add (5);
			neighborIDList.Add (2);
			neighborIDList.Add (20);
			neighborIDList.Add (19);
			break;
		case 22:
			neighborIDList.Add (21);
			neighborIDList.Add (20);
			break;
		case 23:
			neighborIDList.Add (14);
			break;
		case 24:
			neighborIDList.Add (15);
			neighborIDList.Add (3);
			neighborIDList.Add (6);
			neighborIDList.Add (9);
			break;
		case 25:
			neighborIDList.Add (15);
			neighborIDList.Add (7);
			neighborIDList.Add (8);
			break;

		}
	}

	void GetNeighbors(){
		neighbors = new List<Waypoint> ();
		GetNeighborIDs ();
		for (int i = 0; i < neighborIDList.Count; i++) {
			for (int j = 0; j < exp.houseController.HouseWaypoints.Count; j++) {
				if (exp.houseController.HouseWaypoints [j].ID == neighborIDList [i]) {
					neighbors.Add(exp.houseController.HouseWaypoints[j]);
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
