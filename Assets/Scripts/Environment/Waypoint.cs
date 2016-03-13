using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Waypoint : MonoBehaviour {
	public List<Waypoint> neighbors;
	public List<int> neighborIDList;

	public int ID;
	public float DijkstraDistance = 0.0f;

	Experiment exp { get { return Experiment.Instance; } }

	void Awake(){
		SetID();
	}

	void SetID(){
		string IDString = gameObject.name;
		IDString = Regex.Replace(IDString, "[^0-9]", "");
		ID = int.Parse(IDString);
	}

	// Use this for initialization
	void Start () {
		GetNeighbors ();
	}

	void GetNeighborIDs(){
		neighborIDList = new List<int> ();
		switch (ID) {
		case 0:
			neighborIDList.Add (1);
			neighborIDList.Add (8);
			break;
		case 1:
			neighborIDList.Add (0);
			neighborIDList.Add (2);
			neighborIDList.Add (8);
			neighborIDList.Add (7);
			neighborIDList.Add (11);
			break;
		case 2:
			neighborIDList.Add (1);
			neighborIDList.Add (7);
			neighborIDList.Add (3);
			break;
		case 3:
			neighborIDList.Add (2);
			neighborIDList.Add (7);
			neighborIDList.Add (12);
			neighborIDList.Add (4);
			neighborIDList.Add (5);
			break;
		case 4:
			neighborIDList.Add (3);
			neighborIDList.Add (5);
			break;
		case 5:
			neighborIDList.Add (3);
			neighborIDList.Add (4);
			neighborIDList.Add (12);
			neighborIDList.Add (13);
			neighborIDList.Add (14);
			break;
		case 6:
			neighborIDList.Add (68);
			neighborIDList.Add (69);
			neighborIDList.Add (16);
			neighborIDList.Add (17);
			break;
		case 7:
			neighborIDList.Add (1);
			neighborIDList.Add (2);
			neighborIDList.Add (3);
			neighborIDList.Add (11);
			neighborIDList.Add (12);
			break;
		case 8:
			neighborIDList.Add (0);
			neighborIDList.Add (1);
			neighborIDList.Add (11);
			neighborIDList.Add (9);
			break;
		case 9:
			neighborIDList.Add (8);
			neighborIDList.Add (11);
			neighborIDList.Add (10);
			neighborIDList.Add (25);
			neighborIDList.Add (28);
			break;
		case 10:
			neighborIDList.Add (9);
			neighborIDList.Add (11);
			neighborIDList.Add (12);
			neighborIDList.Add (25);
			neighborIDList.Add (13);
			break;
		case 11:
			neighborIDList.Add (1);
			neighborIDList.Add (7);
			neighborIDList.Add (8);
			neighborIDList.Add (12);
			neighborIDList.Add (10);
			neighborIDList.Add (9);
			break;
		case 12:
			neighborIDList.Add (10);
			neighborIDList.Add (11);
			neighborIDList.Add (7);
			neighborIDList.Add (3);
			neighborIDList.Add (5);
			neighborIDList.Add (13);
			break;
		case 13:
			neighborIDList.Add (12);
			neighborIDList.Add (5);
			neighborIDList.Add (14);
			neighborIDList.Add (10);
			neighborIDList.Add (33);
			break;
		case 14:
			neighborIDList.Add (13);
			neighborIDList.Add (5);
			neighborIDList.Add (33);
			neighborIDList.Add (15);
			break;
		case 15:
			neighborIDList.Add (14);
			neighborIDList.Add (16);
			break;
		case 16:
			neighborIDList.Add (15);
			neighborIDList.Add (21);
			neighborIDList.Add (20);
			neighborIDList.Add (17);
			neighborIDList.Add (6);
			break;
		case 17:
			neighborIDList.Add (6);
			neighborIDList.Add (16);
			neighborIDList.Add (18);
			neighborIDList.Add (21);
			neighborIDList.Add (20);
			neighborIDList.Add (19);
			break;
		case 18:
			neighborIDList.Add (17);
			neighborIDList.Add (20);
			neighborIDList.Add (19);
			break;
		case 19:
			neighborIDList.Add (17);
			neighborIDList.Add (20);
			neighborIDList.Add (18);
			neighborIDList.Add (23);
			neighborIDList.Add (24);
			break;
		case 20:
			neighborIDList.Add (16);
			neighborIDList.Add (17);
			neighborIDList.Add (18);
			neighborIDList.Add (21);
			neighborIDList.Add (19);
			neighborIDList.Add (22);
			neighborIDList.Add (23);
			neighborIDList.Add (24);
			break;
		case 21:
			neighborIDList.Add (16);
			neighborIDList.Add (17);
			neighborIDList.Add (20);
			neighborIDList.Add (22);
			neighborIDList.Add (23);
			break;
		case 22:
			neighborIDList.Add (21);
			neighborIDList.Add (20);
			neighborIDList.Add (23);
			break;
		case 23:
			neighborIDList.Add (22);
			neighborIDList.Add (21);
			neighborIDList.Add (20);
			neighborIDList.Add (19);
			neighborIDList.Add (24);
			break;
		case 24:
			neighborIDList.Add (23);
			neighborIDList.Add (20);
			neighborIDList.Add (19);
			break;
		case 25:
			neighborIDList.Add (10);
			neighborIDList.Add (9);
			neighborIDList.Add (28);
			neighborIDList.Add (26);
			break;
		case 26:
			neighborIDList.Add (27);
			neighborIDList.Add (29);
			neighborIDList.Add (28);
			neighborIDList.Add (25);
			break;
		case 27:
			neighborIDList.Add (26);
			neighborIDList.Add (29);
			neighborIDList.Add (30);
			neighborIDList.Add (36);
			break;
		case 28:
			neighborIDList.Add (29);
			neighborIDList.Add (26);
			neighborIDList.Add (25);
			neighborIDList.Add (9);
			break;
		case 29:
			neighborIDList.Add (28);
			neighborIDList.Add (26);
			neighborIDList.Add (27);
			neighborIDList.Add (36);
			break;
		case 30:
			neighborIDList.Add (27);
			neighborIDList.Add (31);
			break;
		case 31:
			neighborIDList.Add (30);
			neighborIDList.Add (33);
			neighborIDList.Add (32);
			neighborIDList.Add (34);
			break;
		case 32:
			neighborIDList.Add (33);
			neighborIDList.Add (31);
			neighborIDList.Add (34);
			neighborIDList.Add (35);
			break;
		case 33:
			neighborIDList.Add (13);
			neighborIDList.Add (14);
			neighborIDList.Add (31);
			neighborIDList.Add (32);
			break;
		case 34:
			neighborIDList.Add (31);
			neighborIDList.Add (32);
			neighborIDList.Add (35);
			neighborIDList.Add (37);
			break;
		case 35:
			neighborIDList.Add (32);
			neighborIDList.Add (34);
			neighborIDList.Add (67);
			break;
		case 36:
			neighborIDList.Add (29);
			neighborIDList.Add (27);
			neighborIDList.Add (38);
			neighborIDList.Add (42);
			break;
		case 37:
			neighborIDList.Add (34);
			neighborIDList.Add (39);
			neighborIDList.Add (40);
			break;
		case 38:
			neighborIDList.Add (36);
			neighborIDList.Add (42);
			neighborIDList.Add (43);
			break;
		case 39:
			neighborIDList.Add (37);
			neighborIDList.Add (40);
			neighborIDList.Add (44);
			break;
		case 40:
			neighborIDList.Add (41);
			neighborIDList.Add (37);
			neighborIDList.Add (39);
			neighborIDList.Add (44);
			break;
		case 41:
			neighborIDList.Add (43);
			neighborIDList.Add (40);
			break;
		case 42:
			neighborIDList.Add (36);
			neighborIDList.Add (38);
			neighborIDList.Add (43);
			neighborIDList.Add (46);
			break;
		case 43:
			neighborIDList.Add (38);
			neighborIDList.Add (42);
			neighborIDList.Add (41);
			neighborIDList.Add (46);
			break;
		case 44:
			neighborIDList.Add (49);
			neighborIDList.Add (45);
			neighborIDList.Add (39);
			neighborIDList.Add (40);
			break;
		case 45:
			neighborIDList.Add (44);
			neighborIDList.Add (50);
			break;
		case 46:
			neighborIDList.Add (42);
			neighborIDList.Add (43);
			neighborIDList.Add (47);
			break;
		case 47:
			neighborIDList.Add (46);
			neighborIDList.Add (48);
			break;
		case 48:
			neighborIDList.Add (47);
			neighborIDList.Add (49);
			break;
		case 49:
			neighborIDList.Add (48);
			neighborIDList.Add (44);
			break;
		case 50:
			neighborIDList.Add (45);
			neighborIDList.Add (51);
			break;
		case 51:
			neighborIDList.Add (50);
			neighborIDList.Add (54);
			neighborIDList.Add (53);
			neighborIDList.Add (52);
			neighborIDList.Add (55);
			break;
		case 52:
			neighborIDList.Add (51);
			neighborIDList.Add (54);
			neighborIDList.Add (53);
			neighborIDList.Add (55);
			break;
		case 53:
			neighborIDList.Add (51);
			neighborIDList.Add (52);
			neighborIDList.Add (54);
			neighborIDList.Add (59);
			break;
		case 54:
			neighborIDList.Add (51);
			neighborIDList.Add (53);
			neighborIDList.Add (52);
			break;
		case 55:
			neighborIDList.Add (56);
			neighborIDList.Add (51);
			neighborIDList.Add (52);
			break;
		case 56:
			neighborIDList.Add (58);
			neighborIDList.Add (57);
			neighborIDList.Add (55);
			break;
		case 57:
			neighborIDList.Add (56);
			neighborIDList.Add (58);
			break;
		case 58:
			neighborIDList.Add (56);
			neighborIDList.Add (57);
			break;
		case 59:
			neighborIDList.Add (53);
			neighborIDList.Add (63);
			neighborIDList.Add (62);
			break;
		case 60:
			neighborIDList.Add (61);
			neighborIDList.Add (62);
			neighborIDList.Add (63);
			neighborIDList.Add (64);
			neighborIDList.Add (65);
			neighborIDList.Add (66);
			neighborIDList.Add (67);
			break;
		case 61:
			neighborIDList.Add (62);
			neighborIDList.Add (63);
			neighborIDList.Add (60);
			neighborIDList.Add (65);
			neighborIDList.Add (64);
			break;
		case 62:
			neighborIDList.Add (59);
			neighborIDList.Add (63);
			neighborIDList.Add (60);
			neighborIDList.Add (61);
			break;
		case 63:
			neighborIDList.Add (67);
			neighborIDList.Add (60);
			neighborIDList.Add (61);
			neighborIDList.Add (62);
			neighborIDList.Add (59);
			break;
		case 64:
			neighborIDList.Add (69);
			neighborIDList.Add (68);
			neighborIDList.Add (65);
			neighborIDList.Add (60);
			neighborIDList.Add (61);
			break;
		case 65:
			neighborIDList.Add (61);
			neighborIDList.Add (64);
			neighborIDList.Add (66);
			neighborIDList.Add (67);
			neighborIDList.Add (60);
			neighborIDList.Add (68);
			neighborIDList.Add (69);
			break;
		case 66:
			neighborIDList.Add (67);
			neighborIDList.Add (60);
			neighborIDList.Add (65);
			neighborIDList.Add (69);
			neighborIDList.Add (68);
			break;
		case 67:
			neighborIDList.Add (35);
			neighborIDList.Add (66);
			neighborIDList.Add (65);
			neighborIDList.Add (60);
			neighborIDList.Add (63);
			break;
		case 68:
			neighborIDList.Add (66);
			neighborIDList.Add (65);
			neighborIDList.Add (64);
			neighborIDList.Add (69);
			neighborIDList.Add (6);
			break;
		case 69:
			neighborIDList.Add (6);
			neighborIDList.Add (68);
			neighborIDList.Add (66);
			neighborIDList.Add (65);
			neighborIDList.Add (64);
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
