using UnityEngine;
using System.Collections;

public class LearningLocator : MonoBehaviour {
	Experiment exp { get { return Experiment.Instance; } } 

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(Collision collision){
		if(collision.gameObject.tag == "Player"){
			StartCoroutine(Die ());
		}
	}

	IEnumerator Die(){
		AudioSource myAudio = GetComponent<AudioSource>();
		myAudio.Play ();

		GetComponentInChildren<MeshRenderer>().enabled = false;
		GetComponent<Collider>().enabled = false;

		while(myAudio.isPlaying){
			yield return 0;
		}

		//this will also destroy the gameobject
		exp.learningLocatorController.RemoveLocator (gameObject);

	}
}
