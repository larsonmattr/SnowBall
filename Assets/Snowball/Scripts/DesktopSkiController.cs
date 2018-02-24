using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesktopSkiController : MonoBehaviour {

	float convertFactor = 1000f;
	float pushFactor = 10000f;
	float frictionFactor = 100f;

	// The HMD angles are used for controlling the turns.
	public GameObject leftSki;
	public GameObject rightSki;
	public Rigidbody body;

	private float lastY = 0f;

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
		//rate += 5f;
		Debug.Log("Push!");

		// Movement should go to ski player's forward vector in World Space.
		Vector3 forward = gameObject.transform.forward;
		Vector3 force = forward * pushFactor;
		body.AddForce (force);
	}

	/**
	 * Calculate the height above the terrain
	 */
	private float getHeightOverTerrain() {
		RaycastHit hit;
		float heightAboveGround = 0;

		Transform t = gameObject.transform;

		Vector3 p = (leftSki.transform.position + rightSki.transform.position) / 2f;

		if (Physics.Raycast (p, t.TransformDirection (Vector3.down), out hit)) {
			heightAboveGround = hit.distance;
		}
		return heightAboveGround;
	}

	// Update is called once per frame
	void Update() {
		if (Input.GetKey ("w")) {
			print ("w key is held down");
			HandleSkiPoleCollision ();
		}

		if (Input.GetKey("s"))
			print("s key is held down");

		if (Input.GetKey("a"))
			print("a key is held down");

		if (Input.GetKey("d"))
			print("d key is held down");
	}                                 

	void FixedUpdate () {

		// TODO: replace the height check to see if on the ground, with 
		// collision detection checking against the hit with "Terrain"

		// Ski moves should only rotate if we are on the ground.
		float height = getHeightOverTerrain ();
		// Debug.Log ("height is " + height);
	
		// If we are moving, allow the ski(s) and player to rotate based on HMD roll.
		// This may need to be changed to Lerp to the correct orientation, not snap.
		float speed = body.velocity.magnitude;
		//Debug.Log ("speed is " + speed);

		/*
		if (speed > 4f) {
			// Get the roll from euler angles.
			float z = hmd.transform.localRotation.eulerAngles.z;

			// Keep HMD roll rotation in range of [180 to -180]
			if (z > 180)
				z = z - 360f;
			else if (z < -180)
				z = z + 360f;
			
			float rotationFactor = -z * 2f;

			// Give each a default rotation
			Quaternion q = new Quaternion();
			leftSki.transform.localRotation = q;		
			rightSki.transform.localRotation = q;

			leftSki.transform.Rotate(new Vector3(0,rotationFactor, 0 ));
			rightSki.transform.Rotate(new Vector3(0,rotationFactor, 0));

			// If close to ground, allow to turn the player's direction.
			if (height < 0.5f) {
				float rot = rotationFactor * Time.deltaTime;
				gameObject.transform.Rotate (new Vector3 (0, rot, 0));
			}
		}
		*/

		// Movement should go to ski player's forward vector in World Space.
		Vector3 forward = gameObject.transform.forward;

		// Only move if above speed.
		if (height < 0.5f) {

			// Descending the hill, convert potential energy into forward movements.
			float potentialToKinetic = 9.8f * (lastY - gameObject.transform.position.y) * convertFactor;

			// A force that will apply in a direction
			Vector3 force = forward * potentialToKinetic;
			body.AddForce (force);

			// TODO: sideways movements giving friction forces...
			/*
			// Apply a sideways force opposite vector when moving in direction at tangent to skis.
			// This should also kick up snow as an effect.
			Vector3 sidewaysFriction = getSlideFriction(forward, body.velocity);
			body.AddRelativeForce (sidewaysFriction * frictionFactor);

			Debug.Log ("Friction force is " + sidewaysFriction.magnitude);
			*/
		}

		lastY = gameObject.transform.position.y;
	}
		
	// Find the magnitude of friction from sliding sideways.
	Vector3 getSlideFriction(Vector3 Forward, Vector3 Velocity) {

		Vector3 ForwardUnit = Forward;
		ForwardUnit.Normalize ();

		Vector3 VelocityUnit = Velocity;
		VelocityUnit.Normalize ();

		// 0 = perpendicular, 1 = parallel, -1 opposite
		float dotProduct = Vector3.Dot(ForwardUnit, VelocityUnit);
		
		if (dotProduct >= 0 && dotProduct < 1) {
			// Calculate sideways vector magnitude.
			float speed = Forward.magnitude * (1 - dotProduct);
			Vector3 difference = ForwardUnit - VelocityUnit;
			difference *= speed;
			return difference;
		}

		return new Vector3(0,0,0);
	}
}
