using UnityEngine;
using System.Collections;

public class MapCursorControls : MonoBehaviour {

	Experiment exp { get { return Experiment.Instance; } }

	public bool ShouldLockControls = false;

	public GameObject rightDirObject;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (exp.currentState == Experiment.ExperimentState.inExperiment) {
			if(!ShouldLockControls){
				GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX; // TODO: on collision, don't allow a change in angular velocity?
				
				//sets velocities
				GetInput ();
			}
			else{
				GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
			}
		}
	}

	void CollisionEnter(Collision collision){

		Debug.Log (collision.gameObject.name);
	}

	void GetInput(){
		//VERTICAL
		float verticalAxisInput = Input.GetAxis ("Vertical");
		//Debug.Log("vert input: " + verticalAxisInput);
		if ( Mathf.Abs(verticalAxisInput) > 0.0f) { //EPSILON should be accounted for in Input Settings "dead zone" parameter
			
			GetComponent<Rigidbody>().velocity = transform.up*verticalAxisInput*Config.cursorDriveSpeed; //since we are setting velocity based on input, no need for time.delta time component
			
		}
		else{
			GetComponent<Rigidbody>().velocity = Vector2.zero;
		}
		
		//HORIZONTAL
		float horizontalAxisInput = Input.GetAxis ("Horizontal");
		if (Mathf.Abs (horizontalAxisInput) > 0.0f) { //EPSILON should be accounted for in Input Settings "dead zone" parameter
			Vector3 rightDir = rightDirObject.transform.position - transform.position;
			rightDir = rightDir.normalized;
			GetComponent<Rigidbody>().velocity += rightDir*horizontalAxisInput*Config.cursorDriveSpeed;
		}

		else if(Mathf.Abs(verticalAxisInput) <= 0.0f && Mathf.Abs(horizontalAxisInput) <= 0.0f){
			GetComponent<Rigidbody>().velocity = Vector2.zero;
			Debug.Log("SETTING TO ZERO");
		}

		Rigidbody r = GetComponent<Rigidbody> ();

		Debug.Log ("VERT AXIS: " + verticalAxisInput + " " + GetComponent<Rigidbody>().velocity);

		Debug.Log ("HORIZ AXIS: " + horizontalAxisInput + " " + GetComponent<Rigidbody>().velocity);


	}
}
