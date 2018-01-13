using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerCollision : MonoBehaviour
{
	#region Global_Variables
	public GameLogic GL;
	public Player player;

	private OVRInput.Controller controller;
	///private GameObject otherController_GO;

	//  Grab
	private GameObject holdingObject;
	private GameObject collidingObject;
	private Rigidbody holdingObject_rigidbody;
	private bool collisionHappenedFirst;
	private bool handTriggerPressed;
	private bool colliding;

	//  Debug
	private string controller_name;
	///public ControllerInput controllerInput;
	#endregion

	public void Init()
	{
		GetController();
		ReleaseObject();
		collisionHappenedFirst = false;
		handTriggerPressed = false;
		colliding = false;
	}

	private void GetController()
	{
		//  Left
		if(gameObject == GL.L_controller_GO)
		{
			controller = GL.L_controller;
			///otherController_GO = GL.R_controller_GO;
			controller_name = "Left"; /// Debug
		}
		//  Right
		else
		{
			controller = GL.R_controller;
			///otherController_GO = GL.L_controller_GO;
			controller_name = "Right"; /// Debug
		}
	}

	//   U P D A T E                                                                                                    
	void Update()
	{
		GetController();
		if(OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, controller))
		{
			Debug.Log(Time.time + ": Hand Trigger is being Pressed");
			///Gizmos.DrawLine(gameObject.transform.position, 1);
			handTriggerPressed = true;
			if(collisionHappenedFirst && holdingObject == null)
			{
				holdingObject = collidingObject.gameObject;
				holdingObject.transform.SetParent(gameObject.transform);
				holdingObject_rigidbody = holdingObject.GetComponent<Rigidbody>();
				holdingObject_rigidbody.isKinematic = true;

				Physics.IgnoreCollision(holdingObject.GetComponent<Collider>(),
				                        player.GetComponent<Collider>());

				Debug.Log(": Already Colliding, so Holding object");
			}
		}
		else
		{
			if(handTriggerPressed)
			{
				Debug.Log(Time.time + ": Trigger Released");
				Gizmos.color = Color.red;
				Gizmos.DrawRay(gameObject.transform.position, gameObject.transform.forward);
			}
			handTriggerPressed = false;
			ReleaseObject();
		}
	}

	void OnTriggerEnter(Collider collider)
	{
		Debug.Log(Time.time + ": Started colliding with " + collider.gameObject.tag + " object");
		if(collider.gameObject.tag == "Throwable")
		{
			if(!handTriggerPressed && !colliding)
			{
				Debug.Log(": Collision happened first");
				collisionHappenedFirst = true;
			}
			colliding = true;
			collidingObject = collider.gameObject;
		}
	}

	/*void OnTriggerStay(Collider collider)
	{
		Debug.Log(Time.time + ": Is colliding with " + collider.gameObject.tag + " object");
	}*/

	void OnTriggerExit(Collider collider)
	{
		Debug.Log(Time.time + ": Stopped colliding with " + collider.gameObject.tag + " object");
		if(collider.gameObject.tag == "Throwable")
		{
			colliding = false;
			collisionHappenedFirst = false;
		}
	}

	private void ReleaseObject()
	{
		if(holdingObject != null)
		{
			holdingObject.transform.SetParent(null);
			holdingObject_rigidbody.isKinematic = false;
			Physics.IgnoreCollision(holdingObject.GetComponent<Collider>(), player.GetComponent<Collider>(), false);

			holdingObject_rigidbody.velocity =
						transform.TransformDirection(OVRInput.GetLocalControllerVelocity(controller)) * GL.throwForce;

			holdingObject_rigidbody.angularVelocity =
						transform.TransformDirection(OVRInput.GetLocalControllerAngularVelocity(controller));

			Debug.Log(Time.time + ": Object Released with velocity: " + holdingObject_rigidbody.velocity +
				", angular: " + holdingObject_rigidbody.angularVelocity);
			holdingObject = null;
			holdingObject_rigidbody = null;
		}
	}
}
