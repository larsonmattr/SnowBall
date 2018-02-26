using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkiWheelController : MonoBehaviour {

	float convertFactor = 100f;
	float pushFactor = 10000f;
	float frictionFactor = 100f;

	public GameObject leftSki;
	public GameObject rightSki;

	// Ski slope-driven accelleration.
	private float lastY = 0f;
	public Rigidbody m_Rigidbody;

	// These are based on CarController from Unity Standard Assets (it won't make much sense at first for a Skier, but will diverge with time).
	[SerializeField] private WheelCollider[] m_WheelColliders = new WheelCollider[4];
	[SerializeField] private GameObject[] m_WheelMeshes = new GameObject[4];
	[SerializeField] private Vector3 m_CentreOfMassOffset;
	[SerializeField] private float m_FullTorqueOverAllWheels;
	[Range(0, 1)] [SerializeField] private float m_TractionControl; // 0 is no traction control, 1 is full interference

	// CarController turning
	[SerializeField] private float m_MaximumSteerAngle;
	[Range(0, 1)] [SerializeField] private float m_SteerHelper; // 0 is raw physics , 1 the car will grip in the direction it is facing
	private float m_OldRotation;
	private float m_SteerAngle;
	private Quaternion[] m_WheelMeshLocalRotations;

	// CarController torque
	private float m_CurrentTorque;

	// Use this for initialization
	void Start () {
		lastY = gameObject.transform.position.y;
		m_Rigidbody = GetComponent<Rigidbody>();
		m_CurrentTorque = m_FullTorqueOverAllWheels - (m_TractionControl*m_FullTorqueOverAllWheels);

		m_WheelMeshLocalRotations = new Quaternion[4];
		for (int i = 0; i < 4; i++)
		{
			m_WheelMeshLocalRotations[i] = m_WheelMeshes[i].transform.localRotation;
		}
		m_WheelColliders[0].attachedRigidbody.centerOfMass = m_CentreOfMassOffset;
	}

	/**
	 *  Ski pole collisions will cause forward movements.
	 */
	public void HandleSkiPoleCollision() {
		//rate += 5f;
		// Debug.Log("Push!");

		// Movement should go to ski player's forward vector in World Space.
		// A skiing character might be better represented by a forward force on the body (actually ski poles push the body).
		Vector3 forward = gameObject.transform.forward;
		Vector3 force = forward * pushFactor;
		m_Rigidbody.AddForce (force);

		/*
		// CarController derived
		// float accel = pushFactor;
		//float thrustTorque = accel * (m_CurrentTorque / 4f);
		float thrustTorque = pushFactor;

		for (int i = 0; i < 4; i++)
		{
			m_WheelColliders[i].motorTorque = thrustTorque;
		}
		*/
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

	void FixedUpdate () {

		// TODO: replace the height check to see if on the ground, with 
		// collision detection checking against the hit with "Terrain"

		// Ski moves should only rotate if we are on the ground.
		float height = getHeightOverTerrain ();
		// Debug.Log ("height is " + height);
	
		// If we are moving, allow the ski(s) and player to rotate based on HMD roll.
		// This may need to be changed to Lerp to the correct orientation, not snap.
		float speed = m_Rigidbody.velocity.magnitude;
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
			m_Rigidbody.AddForce (force);

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

	public void Move(float steering, float accel, float footbrake, float handbrake)
	{
		// Forward movements just treat as ski pole accelerations.
		if (accel > 0) {
			HandleSkiPoleCollision ();
		}
			
		int LF_WHEEL = 0;
		int RF_WHEEL = 1;
		for (int i = 0; i < 4; i++)
		{
			Quaternion quat;
			Vector3 position;
			m_WheelColliders[i].GetWorldPose(out position, out quat);
			m_WheelMeshes[i].transform.position = position;
			m_WheelMeshes[i].transform.rotation = quat;
		}

		// TODO: make the skis use the Y-axis rotation of the two front tires...

		//clamp input values
		steering = Mathf.Clamp(steering, -1, 1);
		//AccelInput = accel = Mathf.Clamp(accel, 0, 1);
		//BrakeInput = footbrake = -1*Mathf.Clamp(footbrake, -1, 0);
		//handbrake = Mathf.Clamp(handbrake, 0, 1);

		//Set the steer on the front wheels.
		//Assuming that wheels 0 and 1 are the front wheels.
		m_SteerAngle = steering*m_MaximumSteerAngle;
		m_WheelColliders[0].steerAngle = m_SteerAngle;
		m_WheelColliders[1].steerAngle = m_SteerAngle;

		// Move the skis also
		Quaternion skiRotation = Quaternion.AngleAxis (m_SteerAngle, Vector3.up);
		leftSki.transform.localRotation = skiRotation;
		rightSki.transform.localRotation = skiRotation;

		SteerHelper();
		/*
		ApplyDrive(accel, footbrake);
		CapSpeed();

		//Set the handbrake.
		//Assuming that wheels 2 and 3 are the rear wheels.
		if (handbrake > 0f)
		{
			var hbTorque = handbrake*m_MaxHandbrakeTorque;
			m_WheelColliders[2].brakeTorque = hbTorque;
			m_WheelColliders[3].brakeTorque = hbTorque;
		}


		CalculateRevs();
		GearChanging();

		AddDownForce();
		CheckForWheelSpin();
		TractionControl();
		*/
	}

	private void SteerHelper()
	{
		for (int i = 0; i < 4; i++)
		{
			WheelHit wheelhit;
			m_WheelColliders[i].GetGroundHit(out wheelhit);
			if (wheelhit.normal == Vector3.zero)
				return; // wheels arent on the ground so dont realign the rigidbody velocity
		}

		// this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
		if (Mathf.Abs(m_OldRotation - transform.eulerAngles.y) < 10f)
		{
			var turnadjust = (transform.eulerAngles.y - m_OldRotation) * m_SteerHelper;
			Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
			m_Rigidbody.velocity = velRotation * m_Rigidbody.velocity;
		}
		m_OldRotation = transform.eulerAngles.y;
	}
}
