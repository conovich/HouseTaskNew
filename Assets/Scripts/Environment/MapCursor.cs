using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapCursor : MonoBehaviour {

	public MapCursorControls controls;
	public GameObject PathPiece;
	float timeUntilNextPathPiece = 0.5f;

	List<GameObject> pathPieces;

	// Use this for initialization
	void Start () {
		pathPieces = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {

	}

	bool shouldMakePath = false;
	public void StartPath(){
		shouldMakePath = true;
		StartCoroutine(PlacePathComponents());
	}

	IEnumerator PlacePathComponents(){
		while (shouldMakePath){
			if(GetComponent<Rigidbody2D>().velocity.magnitude != 0){
				GameObject newPathPiece = Instantiate(PathPiece, transform.position, Quaternion.identity) as GameObject;
				pathPieces.Add(newPathPiece);
				newPathPiece.transform.SetParent(transform.parent);

				yield return new WaitForSeconds(timeUntilNextPathPiece);
			}
			else{
				yield return 0;
			}
		}
	}

	public void EndPath(){
		shouldMakePath = false;
		ClearPath();
	}

	void ClearPath(){
		if(pathPieces != null){
			int numPathPieces = pathPieces.Count;
			for(int i = 0; i < numPathPieces; i++){
				GameObject pathPiece = pathPieces[0];
				pathPieces.RemoveAt(0);
				Destroy(pathPiece);
			}
		}
	}
}
