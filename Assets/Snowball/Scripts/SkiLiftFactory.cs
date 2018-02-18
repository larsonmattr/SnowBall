using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Put a SkiLift factory as a script in the top-level gameobject of a skilift hiearchy
 * Where there is a child object called 'Towers' that contains child prefabs of 'liftTower'
 * Where there is a child object called 'Chairs' that is empty but will hold all the instantiated chairs.
 * 
 * This script will determine the overall path the ski chairs will go.
 * This script will instantiate XX chairs, and then will evenly distribute the skiChairs around the loop of waypoints.
 */
public class SkiLiftFactory : MonoBehaviour {

	// How far apart should the skiChairs be distributed along the path.
	public double DistanceBetweenSkiChairs;

	// What prefab should be instantiated for skichairs.
	public GameObject SkiChairPrefab;

	// We will calculate the overall path length and from this the number of ski chairs.
	private double PathLength;

	private string TOWER_CHILD = "Towers";
	private string CHAIR_CHILD = "Chairs";

	// Use this for initialization
	void Start () {

		// TODO:
		GameObject TowerPrefab = new GameObject();
		//= gameObject.GetComponent;

		// We will fill insert/append forward and reverse waypoints (part of the liftTower prefab)
		GameObject[] forwardPoints;
		GameObject[] reversePoints;
		// Determine how many GameObjects are in the trajectory
		Transform t = TowerPrefab.transform;
		foreach(Transform child in t) {
			GameObject childObject = child.gameObject;
			// TODO: now find the two waypoints attached...

		}

		// TODO: now collate the forward and reverse points into one list.

		// TODO: Calculate the distance between all the GameObjects and sum = PathLength.

		// Determine the overall number of ski chairs to instantiate
		int numberOfChairs = (int) (PathLength / DistanceBetweenSkiChairs);

		// TODO: now instantiate all the chairs and put them at a sensible locations and facing the next waypoint.

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
