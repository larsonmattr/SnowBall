using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkiPoleControls : MonoBehaviour {

	SkiController controller = null;

	// Use this for initialization
	void Start () {
		GameObject obj = GameObject.Find ("SkiPlayer");
		controller = obj.GetComponent (typeof(SkiController)) as SkiController; 
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other) {
		// Debug.Log ("Collider event");
		if (controller != null) {
			Debug.Log ("Add speed");
			controller.HandleSkiPoleCollision ();
		}
	}

}
