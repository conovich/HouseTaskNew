using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//FOR USE IN TRIALCONTROLLER
public class Trial {
	Experiment exp { get { return Experiment.Instance; } }
	public ItemLocation desiredItemLocation;
	public bool is3DFirst;
	public Trajectory myTrajectory;

	public class Trajectory{
		public ItemLocation startLoc;
		public ItemLocation endLoc;

		Trajectory(ItemLocation newStart, ItemLocation newEnd){
			startLoc = newStart;
			endLoc = newEnd;
		}
	}

	/*public Trial(){

	}*/

	public Trial(bool isTrial3DFirst){
		desiredItemLocation = exp.houseController.ChooseItem();
		is3DFirst = isTrial3DFirst;
	}
}