using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapCursor : MonoBehaviour {

	public MapCursorControls controls;
	public GameObject PathPiecePrefab;
	float timeUntilNextPathPiece = 0.2f;

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

			if(controls.isMoving){
				GameObject newPathPiece = Instantiate(PathPiecePrefab, transform.position, transform.rotation) as GameObject;
				pathPieces.Add(newPathPiece);
				newPathPiece.transform.SetParent(transform.parent);
				newPathPiece.transform.localScale = PathPiecePrefab.transform.localScale;

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
