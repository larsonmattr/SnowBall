using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkiUserWindowsMRController : MonoBehaviour {

	// Assign the MixedRealityCamera for this.
	public GameObject hmd;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (hmd == null)
			return;

		// Get the roll from euler angles.
		float z = hmd.transform.localRotation.eulerAngles.z;

		// Keep HMD roll rotation in range of [180 to -180]
		if (z > 180)
			z = z - 360f;
		else if (z < -180)
			z = z + 360f;

		float rotationFactor = -z * 2f;
	}
}
