using UnityEngine;
using System.Collections;

public class ItemLocation : MonoBehaviour {

	public int ID;
	public Transform playerSpotTransform;

	void Awake(){
		foreach (Transform child in transform){
			if(child.tag == "Player Spot"){
				playerSpotTransform = child;
				break;
			}
		}
	}

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
