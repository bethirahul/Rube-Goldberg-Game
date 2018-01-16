using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.iOS;

public class ControllerInput : MonoBehaviour
{
	#region Global_Variables
	public GameLogic GL;
	private Player player;

	//  HOLDING
	public bool isMenuOpen;
	/*private GameObject L_holdingObject;
	private GameObject R_holdingObject;*/

	//  TELEPORT
	public GameObject teleportLocation_GO;
	public LineRenderer ray;
	#endregion

	//   S T A R T                                                                                                      
	void Start()
	{
		player = GetComponent<Player>();
	}

	// Init
	public void Init()
	{
		ray.gameObject.SetActive(false);
		GL.L_controller_GO.GetComponent<ControllerCollision>().Init();
		GL.R_controller_GO.GetComponent<ControllerCollision>().Init();
		isMenuOpen = false;
		/*L_holdingObject = null;
		R_holdingObject = null;*/
	}
	
	//   U P D A T E                                                                                                    

	// INPUT
	public void CheckInput()
	{
		// B Button Press
		if(OVRInput.Get(OVRInput.Button.Two, GL.R_controller))
		{
			ray.gameObject.SetActive(true);
			ray.SetPosition(0, GL.R_controller_GO.transform.position);

			RaycastHit hit;
			if(Physics.Raycast(GL.R_controller_GO.transform.position,
			                   GL.R_controller_GO.transform.forward, out hit, GL.rayRange, GL.rayMask))
			{
				ray.SetPosition(1, hit.point);

				if(hit.transform.tag != "Button")
					GL.ResetAllButtons();

				if(hit.transform.tag == "Button")
					hit.collider.gameObject.GetComponent<VRButton>().Hover();

				else if(hit.transform.tag == "Ground")
				{
					teleportLocation_GO.transform.position = hit.point;
					GL.TeleportLocation_GO_SetActive(true);
				}
				else
					GroundRay(hit.point, true);
			}
			else
			{
				GL.ResetAllButtons();
				Vector3 rayEndPoint = (GL.R_controller_GO.transform.forward * GL.rayRange) +
									  GL.R_controller_GO.transform.position;
				ray.SetPosition(1, rayEndPoint);

				GroundRay(rayEndPoint, true);
			}
		}

		// B Button Up
		if(OVRInput.GetUp(OVRInput.Button.Two, GL.R_controller))
		{
			Debug.Log("B button released");
			ray.gameObject.SetActive(false);
			GL.TeleportLocation_GO_SetActive(false);

			RaycastHit hit;
			if(Physics.Raycast(GL.R_controller_GO.transform.position,
			                   GL.R_controller_GO.transform.forward, out hit, GL.rayRange, GL.rayMask))
			{
				if(hit.transform.tag != "Button")
					GL.ResetAllButtons();

				if(hit.transform.tag == "Button")
					hit.collider.gameObject.GetComponent<VRButton>().Click();

				else if(hit.transform.tag == "Ground")
					GL.InitTeleportPlayer(hit.point);

				else
					GroundRay(hit.point, false);
			}
			else
			{
				GL.ResetAllButtons();
				Vector3 rayEndPoint = (GL.R_controller_GO.transform.forward * GL.rayRange) +
									  GL.R_controller_GO.transform.position;
				ray.SetPosition(1, rayEndPoint);

				GroundRay(rayEndPoint, false);
			}
		}

		// Joystick
		Vector2 joystickInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, GL.R_controller);
		if(joystickInput != Vector2.zero)
		{
			if(!isMenuOpen)
				GL.MovePlayer(joystickInput);
			else
			{
				
			}
		}

		// Hand Trigger Up
		/*if(OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, r_controller))
		{
			Debug.Log("Hand Trigger Released");
			if(holdingObject != null)
				ReleaseObject();
		}*/
	}

	// Ground Ray
	private void GroundRay(Vector3 startPoint, bool isButtonPress)
	{
		RaycastHit groundHit;
		if(Physics.Raycast(startPoint, -Vector3.up, out groundHit, GL.rayRange, GL.rayMask))
		{
			if(isButtonPress)
			{
				teleportLocation_GO.transform.position = groundHit.point;
				GL.TeleportLocation_GO_SetActive(true);
			}
			else
				GL.InitTeleportPlayer(groundHit.point);
		}
		else
		{
			if(isButtonPress)
				GL.TeleportLocation_GO_SetActive(false);
			Debug.Log("Ground Ray didn't hit ground: Cannot Teleport!");
			///Debug.Log("First Ray end: " + startPoint);
		}
	}

	// HOLDING
	/*public void CheckGrabInput(Collider collider, GameObject controller_GO)
	{
		OVRInput.Controller controller;
		GameObject holdingObject;
		if(controller_GO == GL.L_controller_GO)
		{
			controller = GL.L_controller;
			holdingObject = L_holdingObject;
			Debug.Log("Left Controller colliding with object of tag: " + collider.transform.tag);
		}
		else
		{
			controller = GL.R_controller;
			holdingObject = R_holdingObject;
			Debug.Log("Right Controller colliding with object of tag: " + collider.transform.tag);
		}

		if(OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, controller) &&
		   collider.transform.tag == "Throwable" &&
		   holdingObject == null)
		{
			holdingObject = collider.gameObject;
			GrabObject(controller);
		}
		else if(!OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, controller) && holdingObject != null)
		{
			ReleaseObject(controller);
		}
	}

	// Grab
	private void GrabObject()
	{
		holdingObject.transform.SetParent(GL.R_controller_GO.transform);
		holdingObject.GetComponent<Rigidbody>().isKinematic = true;
		Physics.IgnoreCollision(holdingObject.GetComponent<Collider>(), player.GetComponent<Collider>());
		Debug.Log("Holding object");
	}

	// Release
	private void ReleaseObject()
	{
		holdingObject.transform.SetParent(null);
		Rigidbody rigidbody = holdingObject.GetComponent<Rigidbody>();
		rigidbody.isKinematic = false;
		Physics.IgnoreCollision(holdingObject.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
		rigidbody.velocity =
						transform.TransformDirection(OVRInput.GetLocalControllerVelocity(GL.R_controller)) * throwForce;
		rigidbody.angularVelocity =
						transform.TransformDirection(OVRInput.GetLocalControllerAngularVelocity(GL.R_controller));
		holdingObject = null;
		Debug.Log("Object Released with velocity: " + rigidbody.velocity + ", angular: " + rigidbody.angularVelocity);
	}*/
}
