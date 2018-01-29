using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkiController : MonoBehaviour {

	float rate = 0.0f;
	float maxRate = 30f;
	float convertFactor = 1f;
	float lastY = 0f;

	// The HMD angles are used for controlling the turns.
	public GameObject hmd;
	public GameObject leftSki;
	public GameObject rightSki;

	public Rigidbody body;

	// Use this for initialization
	void Start () {
		lastY = gameObject.transform.position.y;
		body = GetComponent(typeof(Rigidbody)) as Rigidbody;

		/*
		// A force that will apply in a direction
		Vector3 forward = gameObject.transform.forward;
		Vector3 force = forward * rate;
		body.velocity = force;
		*/
	}

	/**
	 *  Ski pole collisions will cause forward movements.
	 */
	public void HandleSkiPoleCollision() {
		rate += 5f;
	}

	// Update is called once per frame
	void Update() {
	}                                 

	void FixedUpdate () {
	
		// If we are moving, allow the ski(s) and player to rotate based on HMD roll.
		if (rate > 4f) {
			// Get the roll from euler angles.
			float z = hmd.transform.localRotation.eulerAngles.z;

			// Keep HMD roll rotation in range of [180 to -180]
			if (z > 180)
				z = z - 360f;
			else if (z < -180)
				z = z + 360f;
			
			float rotationFactor = -z * 0.5f;

			// Give each a default rotation
			Quaternion q = new Quaternion();
			leftSki.transform.localRotation = q;		
			rightSki.transform.localRotation = q;

			leftSki.transform.Rotate(new Vector3(0,rotationFactor, 0 ));
			rightSki.transform.Rotate(new Vector3(0,rotationFactor, 0));

			float rot = rotationFactor * Time.deltaTime;
			gameObject.transform.Rotate (new Vector3 (0, rot, 0));
		}

		// Movement should go to ski player's forward vector in World Space.
		Vector3 forward = gameObject.transform.forward;

		// Only move if above speed.
		if (rate > 4f) {

			// A force that will apply in a direction
			Vector3 force = forward * rate * 40f;
			body.AddForce (force);
		}

		// Descending the hill, convert potential energy into forward movements.
		rate += 9.8f * (lastY - gameObject.transform.position.y) * convertFactor;
		lastY = gameObject.transform.position.y;

		// Keep the rate from becoming too high.
		if (rate > maxRate)
			rate = maxRate;

	}
		
}
