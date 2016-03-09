using UnityEngine;
using System.Collections;

public class MapCursorControls : MonoBehaviour {

	Experiment exp { get { return Experiment.Instance; } }

	public bool ShouldLockControls = false;

	public Transform rightBound;
	public Transform leftBound;
	public Transform upBound;
	public Transform downBound;

	public bool isMoving = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (exp.currentState == Experiment.ExperimentState.inExperiment) {
			if(!ShouldLockControls){
				//GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX; // TODO: on collision, don't allow a change in angular velocity?
				
				//sets velocities
				GetInput ();
			}
			else{
				//GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
			}
		}
	}

	void CollisionEnter(Collision collision){

		Debug.Log (collision.gameObject.name);
	}

	void GetInput(){
		//float upMoveAmount = 0.0f;
		//float rightMoveAmount = 0.0f;

		Vector3 upVec = Vector3.zero;
		Vector3 rightVec = Vector3.zero;

		//VERTICAL
		float verticalAxisInput = Input.GetAxis ("Vertical");

		if ( Mathf.Abs(verticalAxisInput) > 0.0f) { //EPSILON should be accounted for in Input Settings "dead zone" parameter
			Vector3 dir = upBound.position - downBound.position;
			upVec = verticalAxisInput * dir.normalized * Config.cursorDriveSpeed;
		}

		//HORIZONTAL
		float horizontalAxisInput = Input.GetAxis ("Horizontal");
		if (Mathf.Abs (horizontalAxisInput) > 0.0f) { //EPSILON should be accounted for in Input Settings "dead zone" parameter
			Vector3 dir = rightBound.position - leftBound.position;
			rightVec = horizontalAxisInput * dir.normalized * Config.cursorDriveSpeed;
		}

		Move (upVec, rightVec);

		if (verticalAxisInput == 0 && horizontalAxisInput == 0) {
			isMoving = false;
		} else {
			isMoving = true;
		}
	}

	void Move(Vector3 upVec, Vector3 rightVec){
		RectTransform rt = GetComponent<RectTransform>();

		rt.position += upVec;
		/*if(transform.position.y > upBound.position.y){
			rt.position = new Vector3(rt.position.x, upBound.position.y, rt.position.z);
		}
		else if (transform.position.y < downBound.position.y){
			rt.position = new Vector3(rt.position.x, downBound.position.y, rt.position.z);
		}*/

		rt.position += rightVec;
		//if transform.position is between the two other points -- in x & z, on a line -- we're good
		/*if(!IsPointBetween(transform.position, rightBound.position, leftBound.position)){
			float leftDist = (transform.position - leftBound.position).magnitude;
			float rightDist = (transform.position - rightBound.position).magnitude;

			if(leftDist > rightDist){
				rt.position = new Vector3(rightBound.position.x, rt.position.y, rightBound.position.z);
			}
			else{
				rt.position = new Vector3(leftBound.position.x, rt.position.y, leftBound.position.z);
			}
		}*/

	}

	bool IsPointBetween(Vector3 point, Vector3 pointA, Vector3 pointB){
		float epsilon = 0.01f;

		float crossProd = (point.z - pointA.z) * (pointB.x - pointA.x) - (point.x - pointA.x) * (pointB.z - pointA.z);
		if(Mathf.Abs(crossProd) > epsilon){
			return false;
		}

		float dotProd = (point.x - pointA.x) * (pointB.x - pointA.x) + (point.z - pointA.z)*(pointB.z - pointA.z);
		if(dotProd < 0){
			return false;
		}

		float squaredlengthba = (pointB.x - pointA.x)*(pointB.x - pointA.x) + (pointB.z - pointA.z)*(pointB.z - pointA.z);
		if(dotProd > squaredlengthba){
			return false;
		}

			/*
			 * check if c is bw a & b
			crossproduct = (point.y - a.y) * (b.x - a.x) - (point.x - a.x) * (b.y - a.y)
				if abs(crossproduct) > epsilon : return False   # (or != 0 if using integers)
				
				dotproduct = (point.x - a.x) * (b.x - a.x) + (point.y - a.y)*(b.y - a.y)
				if dotproduct < 0 : return False
				
				squaredlengthba = (b.x - a.x)*(b.x - a.x) + (b.y - a.y)*(b.y - a.y)
				if dotproduct > squaredlengthba: return False*/

		return true;

	}
}
