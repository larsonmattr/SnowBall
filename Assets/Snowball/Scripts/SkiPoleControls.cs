using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkiPoleControls : MonoBehaviour {

	SkiController controller = null;

	// This should be the snow effect of a collison of the poles with the ground.
	public GameObject SnowParticleEffect;

	// Keep track of the snow effect, create additional if needed.
	private GameObject poof;

	// Use this for initialization
	void Start () {
		GameObject obj = GameObject.Find ("SkiPlayer");
		controller = obj.GetComponent (typeof(SkiController)) as SkiController; 
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter(Collision collision)
	{
			
		// Where we would spawn snow effects.
		foreach (ContactPoint contact in collision.contacts) {
			if ("Terrain" == contact.otherCollider.name) {


				if (controller != null) {
					Debug.Log ("Add speed");
					controller.HandleSkiPoleCollision ();
				}

				Vector3 pt = contact.point;
				pt.y += 0.2f;

				poof = Instantiate(SnowParticleEffect, pt, Quaternion.identity) as GameObject;
				Destroy (poof, 0.5f);
			}

			print(contact.thisCollider.name + " hit " + contact.otherCollider.name);
			Debug.DrawRay(contact.point, contact.normal, Color.white);
		}
			
	}

	// add on collision stay to keep poofing.
	void OnCollisionStay(Collision collision) {
		foreach (ContactPoint contact in collision.contacts) {
			if ("Terrain" == contact.otherCollider.name) {
				Vector3 pt = contact.point;
				pt.y += 0.2f;

				if (poof == null) {
					poof = Instantiate (SnowParticleEffect, pt, Quaternion.identity) as GameObject;
					Destroy (poof, 0.5f);
				} else {
					poof.transform.position = pt;
				}
			}
		}
	}
}
