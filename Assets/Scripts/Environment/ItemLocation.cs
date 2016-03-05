using UnityEngine;
using System.Collections;

public class ItemLocation : MonoBehaviour {

	int ID;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Transform GetRoom(){
		return transform.parent;
	}

	public void SetID(int newID){
		ID = newID;
	}

	public int GetID(){
		return ID;
	}
}
