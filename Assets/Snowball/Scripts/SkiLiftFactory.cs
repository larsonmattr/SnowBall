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
	private double PathLength = 0.0;

	private string TOWER_CHILD = "Towers";
	private string CHAIR_CHILD = "Chairs";
	private string WAYPT_LEFT_CHILD = "WayPt_left";
	private string WAYPT_RIGHT_CHILD = "WayPt_right";

	// Calculate distance between two points.
	private double getDistance(Transform[] transforms, int i, int j) {
		if (i >= transforms.Length || j >= transforms.Length)
			return 0.0;

		Vector3 pt1 = transforms[i].position;
		Vector3 pt2 = transforms[j].position;

		return Vector3.Distance (pt1, pt2);
	}

	// Use this for initialization
	void Start () {

		Transform TowerTransform = transform.Find(TOWER_CHILD);

		// We will fill insert/append forward and reverse waypoints (part of the liftTower prefab)
		List<Transform> forwardPoints = new List<Transform>();
		List<Transform> reversePoints = new List<Transform>();

		// Determine how many GameObjects are in the trajectory
		foreach(Transform child in TowerTransform) {
			Transform left = child.Find (WAYPT_LEFT_CHILD);
			Transform right = child.Find (WAYPT_RIGHT_CHILD);

			if (left == null || right == null) {
				Debug.Log ("Unable to find the waypoint child nodes in the liftTower prefabs, please check.");
				continue;
			}

			forwardPoints.Add (right);
			reversePoints.Insert (0, left);
		}

		// TODO: eventually, add also the terminal loading zones which will have more points in a half-circule

		// combine the forward and reverse points into one list.
		List<Transform> allPoints = new List<Transform>();
		allPoints.AddRange (forwardPoints);
		allPoints.AddRange (reversePoints);
		Transform[] points = allPoints.ToArray ();

		Debug.Log ("All points = " + allPoints.Count);

		// calculate the distance between all the GameObjects and sum = PathLength.
		for (int i = 0; i < allPoints.Count; i++) {
			int j = i + 1;
			if (j >= allPoints.Count)
				j = 0;
			
			PathLength += getDistance(points, i, j);
		}
			
		// Determine the overall number of ski chairs to instantiate
		int numberOfChairs = (int) (PathLength / DistanceBetweenSkiChairs);

		// TODO: now instantiate all the chairs and put them at a sensible locations and facing the next waypoint.
		Debug.Log("Would instantiate " + numberOfChairs + " ski chairs");

		// To place the ski chairs.. I need to space them evenly between the chair lifts with the correct distance between the towers...
		if (allPoints.Count < 2) return;

		int lastNode = 0;
		int targetNode = 1;

		// initialization
		double lastChairDistance = 0; // origin
		double lastNodeDistance = 0; //origin
		double nextNodeDistance = getDistance(points, lastNode, targetNode);

		// Keep track of the distance of last chair, last node, and next node
		// If (DistanceBetweenChairs + lastNode < nextNode) then we can put it between the current two nodes.
		// Otherwise we need to increase the node indices.

		int towerIndex = 0;
		for (int i = 0; i < numberOfChairs; i++) {

			double checkDistance = nextNodeDistance - lastChairDistance;
			while (checkDistance < DistanceBetweenSkiChairs) {
				lastNode = targetNode;
				targetNode += 1;
				if (targetNode >= allPoints.Count)
					targetNode = 0;

				lastNodeDistance = nextNodeDistance;
				nextNodeDistance += getDistance(points, lastNode, targetNode);
				checkDistance = nextNodeDistance - lastChairDistance;
			}

			double remainder = DistanceBetweenSkiChairs - (lastNodeDistance - lastChairDistance);
			float percent = (float) ( (remainder) / (nextNodeDistance - lastNodeDistance) );
			lastChairDistance += DistanceBetweenSkiChairs;

			Vector3 position = Vector3.Lerp (points [lastNode].transform.position, points [targetNode].transform.position, percent);

			GameObject chair = GameObject.Instantiate (SkiChairPrefab);
			chair.transform.position = position;

			// Setup the list of points and it's target correctly.
			SkiChairWayPoints pts = chair.GetComponent(typeof(SkiChairWayPoints)) as SkiChairWayPoints;
			pts.waypoints = allPoints.ToArray();
			pts.waypointIndex = targetNode;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
