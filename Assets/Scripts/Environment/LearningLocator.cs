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
			StartCoroutine(OnPlayerCollision ());
		}
	}

	IEnumerator OnPlayerCollision(){
		AudioSource myAudio = GetComponent<AudioSource>();
		myAudio.Play ();

		Deactivate();

		while(myAudio.isPlaying){
			yield return 0;
		}

		exp.learningLocatorController.RemoveActiveLocator (this);


	}

	public void Deactivate(){
		GetComponentInChildren<MeshRenderer>().enabled = false;
		GetComponent<Collider>().enabled = false;
	}

	public void Activate(){
		GetComponentInChildren<MeshRenderer>().enabled = true;
		GetComponent<Collider>().enabled = true;
	}
}
