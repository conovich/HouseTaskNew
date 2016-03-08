﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class HouseController : MonoBehaviour {

	public TextAsset LocationIDFile;
	public TextAsset LocationOrderFile;

	public List<ItemLocation> trajectoriesOrder;

	public Transform[] RoomLocators;

	public List<ItemLocation> ItemLocations { get { return GetItemLocations(); } } //should typically use this one -- does a null check for itemLocations, and fills in list if necessary.
	List<ItemLocation> itemLocations;



	void Awake(){
		ProcessLocationIDs ();
		InitTrajectoriesOrder ();
	}

	void ProcessLocationIDs(){
		string[] lines = LocationIDFile.ToString().Split('\n');
		foreach (string line in lines) {
			if(line != ""){
				string[] columns = line.Split('\t');

				int ID = int.Parse(columns[0]);
				string locName = columns[1];

				locName = locName.Replace("\r", "");

				ItemLocation currLoc = GetItemByName(locName);
				if(currLoc != null){
					currLoc.SetID(ID);
				}
			}
		}
	}

	void InitTrajectoriesOrder(){
		trajectoriesOrder = new List<ItemLocation> ();

		string[] lines = LocationOrderFile.ToString().Split('\n');

		if (ExperimentSettings.trajectoriesPath != "") {
			lines = File.ReadAllLines(ExperimentSettings.trajectoriesPath);
		}

		foreach (string line in lines) {
			if(line != ""){

				string[] locIDsPerSession = line.Split('\t');

				int sessionIndex = Experiment.sessionID;
				if(Experiment.sessionID >= locIDsPerSession.Length){
					sessionIndex = locIDsPerSession.Length - 1;
					Debug.Log("TRIAL TYPES: Not enough sessions to choose from!");
				}

				int locID = int.Parse(locIDsPerSession[sessionIndex]);
				
				ItemLocation currLoc = GetItemByID(locID);
				if(currLoc != null){
					trajectoriesOrder.Add(currLoc);
				}
			}
		}
	}

	ItemLocation GetItemByName(string name){
		for(int i = 0; i < ItemLocations.Count; i++){
			if(ItemLocations[i].name == name){
				return ItemLocations[i];
			}
		}
		return null;
	}

	ItemLocation GetItemByID(int ID){
		for(int i = 0; i < ItemLocations.Count; i++){
			if(ItemLocations[i].GetID() == ID){
				return ItemLocations[i];
			}
		}
		return null;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	List<ItemLocation> GetItemLocations(){
		if(itemLocations != null){
			return itemLocations;
		}
		else{
			itemLocations = new List<ItemLocation>();
			ItemLocation[] itemLocationArr = GetComponentsInChildren<ItemLocation>();
			for(int i = 0; i < itemLocationArr.Length; i++){
				itemLocations.Add(itemLocationArr[i]);
			}
			return itemLocations;
		}
	}

	int nextItemIndex = 1;
	public ItemLocation ChooseNextItem(){
		if (nextItemIndex < trajectoriesOrder.Count) {
			nextItemIndex++;
			return trajectoriesOrder [nextItemIndex - 1];
		}
		else{
			Debug.Log("No more item locations!");
			return null;
		}
	}

}
