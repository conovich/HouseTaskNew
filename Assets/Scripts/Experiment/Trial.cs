using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//FOR USE IN TRIALCONTROLLER
public class Trial {
	Experiment exp { get { return Experiment.Instance; } }
	public ItemLocation desiredItemLocation;

	/*public Trial(){

	}*/

	public Trial(){
		desiredItemLocation = exp.houseController.ChooseItem();
	}
}