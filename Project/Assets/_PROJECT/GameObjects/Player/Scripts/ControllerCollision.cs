using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerCollision : MonoBehaviour
{
	public ControllerInput controllerInput;

	void OnTriggerStay(Collider collider)
	{
		controllerInput.CheckGrabInput(collider);
	}
}
