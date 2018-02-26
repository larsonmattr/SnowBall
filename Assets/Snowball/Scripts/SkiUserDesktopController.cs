using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using UnityStandardAssets.CrossPlatformInput;

public class SkiUserDesktopController : MonoBehaviour {

	private SkiWheelController m_SkiController;


	private void Awake()
	{
		// get the car controller
		m_SkiController = GetComponent<SkiWheelController>();
	}


	private void FixedUpdate()
	{
		// pass the input to the car!
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis ("Vertical");

		//float h = CrossPlatformInputManager.GetAxis("Horizontal");
		//float v = CrossPlatformInputManager.GetAxis("Vertical");
		//#if !MOBILE_INPUT
		//float handbrake = CrossPlatformInputManager.GetAxis("Jump");
		//m_SkiController.Move(h, v, v, handbrake);
		//#else
		m_SkiController.Move(h, v, v, 0f);
		//#endif
	}
}
