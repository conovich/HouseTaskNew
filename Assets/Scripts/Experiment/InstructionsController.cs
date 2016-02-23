using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class InstructionsController : MonoBehaviour {

	Experiment exp { get { return Experiment.Instance; } }

	public float timePerInstruction;

	public bool isFinished = false;

	//TextMesh _textMesh;
	public Text text; //TODO: rename this!!!
	public Text oculusText;
	public Color textColorDefault;
	public Color textColorOverlay;
	public Image background;
	public Image oculusBackground;
	public Color backgroundColorDefault;

	public GameObject ScoreInstructions; //turn these on and off as necessary during the trial.......

	List<string> _instructions;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void TurnOffInstructions(){
		SetInstructionsTransparentOverlay();
		SetInstructionsBlank();
	}

	void SetText(string newText){
		if(ExperimentSettings.isOculus){
			oculusText.text = newText;
		}
		else{
			text.text = newText;
		}
	}

	public void SetInstructionsBlank(){
		SetText ("");
	}

	public void SetInstructionsColorful(){
		//Debug.Log("set instructions dark");
		if(ExperimentSettings.isOculus){
			oculusBackground.color = backgroundColorDefault;
			oculusText.color = textColorDefault;
		}
		else{
			background.color = backgroundColorDefault;
			text.color = textColorDefault;
		}
	}
	
	public void SetInstructionsTransparentOverlay(){
		//Debug.Log("set instructions transparent overlay");
		if(ExperimentSettings.isOculus){
			oculusBackground.color = new Color(0,0,0,0);
			oculusText.color = textColorOverlay;
		}
		else{
			background.color = new Color(0,0,0,0);
			text.color = textColorOverlay;
		}
	}

	public void DisplayText(string line){
		SetText(line);
	}

	public IEnumerator ShowSingleInstruction(string line, bool isDark, bool waitForButton, bool addRandomPostJitter, float minDisplayTimeSeconds){
		if(isDark){
			SetInstructionsColorful();
		}
		else{
			SetInstructionsTransparentOverlay();
		}
		DisplayText(line);
		
		yield return new WaitForSeconds (minDisplayTimeSeconds);
		
		if (waitForButton) {
			yield return StartCoroutine (exp.WaitForActionButton ());
		}
		
		if (addRandomPostJitter) {
			yield return StartCoroutine(exp.WaitForJitter ( Config.randomJitterMin, Config.randomJitterMax ) );
		}
		
		TurnOffInstructions ();
	}
}
