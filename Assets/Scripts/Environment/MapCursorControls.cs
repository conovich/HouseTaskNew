using UnityEngine;
using System.Collections;

public class MapCursorControls : MonoBehaviour {

	Experiment exp { get { return Experiment.Instance; } }

	public bool ShouldLockControls = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (exp.currentState == Experiment.ExperimentState.inExperiment) {
			if(!ShouldLockControls){
				GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation; // TODO: on collision, don't allow a change in angular velocity?
				
				//sets velocities
				GetInput ();
			}
			else{
				GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
			}
		}
	}

	void GetInput(){
		//VERTICAL
		float verticalAxisInput = Input.GetAxis ("Vertical");
		//Debug.Log("vert input: " + verticalAxisInput);
		if ( Mathf.Abs(verticalAxisInput) > 0.0f) { //EPSILON should be accounted for in Input Settings "dead zone" parameter
			
			GetComponent<Rigidbody2D>().velocity = transform.up*verticalAxisInput*Config.cursorDriveSpeed; //since we are setting velocity based on input, no need for time.delta time component
			
		}
		else{
			GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		}
		
		//HORIZONTAL
		float horizontalAxisInput = Input.GetAxis ("Horizontal");
		if (Mathf.Abs (horizontalAxisInput) > 0.0f) { //EPSILON should be accounted for in Input Settings "dead zone" parameter
			GetComponent<Rigidbody2D>().velocity += ((Vector2)transform.right*horizontalAxisInput*Config.cursorDriveSpeed);
		}

		else if(Mathf.Abs(verticalAxisInput) <= 0.0f && Mathf.Abs(horizontalAxisInput) <= 0.0f){
			GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		}

	}
}
