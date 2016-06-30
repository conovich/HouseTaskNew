using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

	Experiment exp { get { return Experiment.Instance; } }

	public CanvasGroup PauseUI;
	public CanvasGroup ConnectionUI;
	public Text ConnectionText; //changed in TrialController from "connecting..." to "press start..." etc.

	public CanvasGroup Part1InstructionsUI;
	public CanvasGroup Part2InstructionsUI;
	public CanvasGroup GoToInstructions3D;
	public CanvasGroup GoToInstructions2D;
	public Text GoToLocation3D;
	public Text GoToLocation2D;

	//public Transform InSceneUIParent;
	//public Transform OculusInSceneUIPos;

	void Start(){
		/*if (ExperimentSettings.isOculus) {
			InSceneUIParent.position = OculusInSceneUIPos.position;
			InSceneUIParent.parent = OculusInSceneUIPos;
			InSceneUIParent.rotation = OculusInSceneUIPos.rotation;
		}*/
	}

	public IEnumerator TurnOnCanvasGroup(CanvasGroup groupToTurnOn, bool WaitForButtonPress, float timeToBeOn){
		groupToTurnOn.alpha = 1.0f;

		if (WaitForButtonPress) {
			yield return StartCoroutine (exp.WaitForActionButton ());
		} 
		else {
			float timePassed = 0.0f;
			while (timePassed < timeToBeOn) {
				timePassed += Time.deltaTime;
				yield return 0;
			}
		}

		groupToTurnOn.alpha = 0.0f;
	}
}
