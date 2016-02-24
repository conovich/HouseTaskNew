using UnityEngine;
using System.Collections;

//http://answers.unity3d.com/questions/742466/camp-fire-light-flicker-control.html

public class FlickeringLight : MonoBehaviour {

	float minFlickerIntensity = 2.0f;
	float maxFlickerIntensity = 3.5f;
	float flickerSpeed = 0.035f;
	
	private float randomizer = 0.0f;

	Light light;

	void Start(){
		light = GetComponent<Light>();
		StartCoroutine(Flicker());
	}


	IEnumerator Flicker(){
		while (true){
			if (randomizer == 0) {
				light.intensity = (Random.Range (minFlickerIntensity, maxFlickerIntensity));
				
			}
			else light.intensity = (Random.Range (minFlickerIntensity, maxFlickerIntensity));
			
			randomizer = Random.Range (0.0f, 1.1f);
			yield return new WaitForSeconds (flickerSpeed);
		}
	}
}
