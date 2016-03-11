using UnityEngine;
using System.Collections;

public class Config : MonoBehaviour {

	public enum Version
	{
		ExpName //TODO: change this for your experiment!
	}
	
	public static Version BuildVersion = Version.ExpName; //TODO: change this for your experiment!
	public static string VersionNumber = "2.03"; //TODO: change this for your experiment!
	
	public static bool isSyncbox = false;
	public static bool isSystem2 = false;
	
	//REPLAY
	public static int replayPadding = 6;

	//JUICE
	public static bool isJuice = true;
	public static bool isSoundtrack = false; //WON'T PLAY IF ISJUICE IS FALSE.


    //LOCATOR COLLECTION VARIABLES
	public static float totalLearningTime = 240.0f;
	public static int learningLocatorScore = 10;
	public static float learningTimeToPassUntilArrows = 180.0f;

	public static float max3DTrialTime = 30.0f;

	//stimulation variables
	public static bool shouldDoStim;	//TODO
	public static int stimFrequency;	//TODO
	public static float stimDuration;	//TODO
	public static bool shouldDoBreak;	//TODO

	public static int numTestTrials = 150;
	
	//practice settings
	public static int numTrialsPract = 1;
	public static bool doPracticeTrial = false;
	public static int numSpecialObjectsPract = 2;


	//SPECIFIC COIN TASK VARIABLES:
	public static float randomJitterMin = 0.0f;
	public static float randomJitterMax = 0.2f;



	//DEFAULT OBJECTS
	public static int numDefaultObjects = 5;

	public static float selectionDiameter = 20.0f;

	public static float objectToWallBuffer = 10.0f; //half of the selection diameter.
	public static float objectToObjectBuffer { get { return CalculateObjectToObjectBuffer(); } } //calculated base on min time to drive between objects!
	public static float specialObjectBufferMult = 0.0f; //the distance the object controller will try to keep between special objects. should be a multiple of objectToObjectBuffer

	public static float minDriveTimeBetweenObjects = 0.5f; //half a second driving between objects


	public static float rotateToSpecialObjectTime = 0.5f;
	public static float pauseAtTreasureTime = 1.5f;


	public static string initialInstructions1 = "Virtual loft apartment game" +
		"\n\nPart 1: Go explore the loft and collect all the coins." +
		"\n\nUse the left joystick to move around." +
		"\n\nPress (A) to start!";

	public static string overheadIntroInstruction = "Part 2: Now you’ll go to different locations in the loft." +
		"\n\nAfterwards, a map will appear and you’ll retrace the path you took." +
		"\n\nPress (A) to start.";

	public static float minObjselectionUITime = 0.5f;
	public static float minInitialInstructionsTime = 0.0f; //TODO: change back to 5.0f
	public static float minDefaultInstructionTime = 0.0f; //time each learning trial instruction should be displayed for
	public static float minScoreMapTime = 0.0f;


	//tilt variables
	public static bool isAvatarTilting = false;
	public static float turnAngleMult = 0.07f;

	//drive variables
	public static float driveSpeed = 3.5f;
	public static float cursorDriveSpeed = 0.001f;
	public static float autoDriveSpeed = 2.0f;

	//object buffer variables

	void Awake(){
		DontDestroyOnLoad(transform.gameObject);
	}

	void Start(){

	}

	public static int GetTotalNumTrials(){
		if (!doPracticeTrial) {
			return numTestTrials;
		} 
		else {
			return numTestTrials + numTrialsPract;
		}
	}

	public static float CalculateObjectToObjectBuffer(){
		float buffer = 0;

		if (Experiment.Instance != null) {

			buffer = driveSpeed * minDriveTimeBetweenObjects; //d = vt

			buffer += Experiment.Instance.objectController.GetMaxDefaultObjectColliderBoundXZ ();

			//Debug.Log ("BUFFER: " + buffer);

		}

		return buffer;
	}

}
