using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (CanvasGroup))]
[RequireComponent (typeof (CanvasGroupLogTrack))]
public class OverheadMap : MonoBehaviour {

	public bool isVisibleOnAwake = false;
	public MapCursor mapCursor;

	public GameObject overheadItemLocationParent;
	ItemLocation[] overheadItemLocations;


	void Awake(){
		if(isVisibleOnAwake){
			TurnOn(true);
		}
		else{
			TurnOn(false);
		}

		GetOverheadLocations();
	}

	void GetOverheadLocations(){
		overheadItemLocations = overheadItemLocationParent.GetComponentsInChildren<ItemLocation>();
	}

	// Use this for initialization
	void Start () {
	
	}

	public void TurnOn(bool shouldTurnOn){
		if(shouldTurnOn){
			GetComponent<CanvasGroup>().alpha = 1.0f;
		}
		else{
			Debug.Log("TURNING OFF MAP");
			GetComponent<CanvasGroup>().alpha  = 0.0f;
			LockCursor(true);
			mapCursor.EndPath();
		}
	}

	public void LockCursor(bool shouldEnable){
		mapCursor.controls.ShouldLockControls = shouldEnable; //if we should lock controls, we should NOT enable them.
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	ItemLocation GetItemLocation(int locID){
		for(int i = 0; i < overheadItemLocations.Length; i++){
			if(overheadItemLocations[i].ID == locID){
				return overheadItemLocations[i];
			}
		}
		Debug.Log("No location with this ID!");
		return null;
	}

	public void MoveCursorToLocation(int locID){
		ItemLocation loc = GetItemLocation(locID);
		if(loc != null){
			mapCursor.transform.position = loc.transform.position;
		}
	}
}
