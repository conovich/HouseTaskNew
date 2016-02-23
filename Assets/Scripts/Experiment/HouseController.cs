using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HouseController : MonoBehaviour {

	public Transform[] RoomLocators;
	public List<ItemLocation> ItemLocations { get { return GetItemLocations(); } }
	List<ItemLocation> itemLocations;
	List<ItemLocation> itemLocationsLeftToChoose;

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

	public ItemLocation ChooseItem(){
		if(itemLocationsLeftToChoose == null){
			itemLocationsLeftToChoose = GetItemLocations();
		}

		if(itemLocationsLeftToChoose.Count <= 0){
			itemLocationsLeftToChoose = GetItemLocations();
		}
		int randomIndex = Random.Range(0, itemLocationsLeftToChoose.Count);
		ItemLocation chosenItem = itemLocationsLeftToChoose[randomIndex];
		itemLocationsLeftToChoose.RemoveAt(randomIndex);
		return chosenItem;
		
	}
}
