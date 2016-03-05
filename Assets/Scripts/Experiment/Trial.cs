using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//FOR USE IN TRIALCONTROLLER
public class Trial {
	Experiment exp { get { return Experiment.Instance; } }
	public ItemLocation desiredItemLocation;
	public bool is3DFirst;

	public Trial(bool isTrial3DFirst){
		desiredItemLocation = exp.houseController.ChooseNextItem();
		is3DFirst = isTrial3DFirst;
	}
}