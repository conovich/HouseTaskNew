using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SimpleTimer : MonoBehaviour {

	public Text timerText;
	float secondsLeft = 0;
	float maxSeconds = 0;

	bool isRunning = false;
	public bool IsRunning { get { return isRunning; } } //public getter. don't want people setting isRunning outside of here.

	public delegate void ResetDelegate();
	public ResetDelegate myResetDelegate;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (isRunning) {
			UpdateTimer();
		}
	}

	void UpdateTimer(){
		secondsLeft -= Time.deltaTime;

		if(secondsLeft < 0.0f){
			secondsLeft = 0.0f;
			isRunning = false;
		}

		if(timerText != null){
			int displayMinutes = (int)Mathf.Floor (secondsLeft / 60.0f);
			int displaySeconds = (int)Mathf.Floor (secondsLeft - (displayMinutes * 60));
			if (displaySeconds < 10) {
				timerText.text = displayMinutes + ":0" + displaySeconds;
			} else {
				timerText.text = displayMinutes + ":" + displaySeconds;
			}
		}
	}

	public void SetTimerMaxTime(float numSeconds){
		maxSeconds = numSeconds;
	}

	public void StartTimer(){
		isRunning = true;
		secondsLeft = maxSeconds;
	}

	public void StopTimer(){
		isRunning = false;
	}

	public void ResetTimer(){
		myResetDelegate ();
		secondsLeft = 0;
	}

	public int GetSecondsInt(){
		return Mathf.FloorToInt(secondsLeft);
	}

	public float GetSecondsFloat(){
		return secondsLeft;
	}
}
