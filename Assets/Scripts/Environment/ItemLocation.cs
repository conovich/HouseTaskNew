using UnityEngine;
using System.Collections;

public class ItemLocation : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Transform GetRoom(){
		return transform.parent;
	}
}
