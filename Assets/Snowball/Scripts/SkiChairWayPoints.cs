using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Base on this : https://forum.unity.com/threads/a-waypoint-script-explained-in-super-detail.54678/
 * Smooth LERP between points, with smooth rotations to face new waypoints.
 * Also, need a circular rotation near the end, but that could just be waypoints.
 * 
 * This will continuously move an object along a cycle of waypoints.
 */
public class SkiChairWayPoints : MonoBehaviour {

	//This variable is an array. []< that is an array container if you didnt know. It holds all the Waypoint Objects that you assign in the inspector.
	public Transform[] waypoints;
	public bool isMoving = true;
	public float rotationDamping = 6.0f; // speed to rotate to face next target
	public float rateMovement = 6.0f;


	//This variable will store the "active" target object (the waypoint to move to).
	public int waypointIndex = 0;

	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

		if (isMoving) {
			Transform target = waypoints[waypointIndex];

			// Smooth rotate towards next waypoint
			//Look at the active waypoint.
			Quaternion rotation = Quaternion.LookRotation(target.position - transform.position);
			//Make the rotation nice and smooth.
			transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationDamping);
			 
			// Smooth translate towards the next waypoint.
			Vector3 direction = target.position - transform.position;
			float d = direction.magnitude;
			direction.Normalize();
			float deltaMove = rateMovement * Time.deltaTime;
			// Don't overshoot the target.
			if (deltaMove > d)
				deltaMove = d; 
			transform.position = transform.position + deltaMove * direction;

			// Check if we reached the target this iteration, if so go to the next target.
			if (d <= deltaMove) {
				waypointIndex += 1;
				if (waypointIndex >= waypoints.Length)
					waypointIndex = 0;
			}

		}
	}
}
