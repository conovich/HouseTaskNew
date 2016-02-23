using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CanvasGroup))]
[RequireComponent (typeof (CanvasGroupLogTrack))]
public class OverheadMap : MonoBehaviour {

	public bool isVisibleOnAwake = false;
	public MapCursor mapCursor;

	void Awake(){
		if(isVisibleOnAwake){
			TurnOn(true);
		}
		else{
			TurnOn(false);
		}
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
}
