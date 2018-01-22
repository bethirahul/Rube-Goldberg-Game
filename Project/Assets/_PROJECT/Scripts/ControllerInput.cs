using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.iOS;
using UnityEngine.Experimental.UIElements;

public class ControllerInput : MonoBehaviour
{
	#region Global_Variables
	private GameLogic GL;
	private Player player;

	//  TELEPORT
	private GameObject teleportLocation_GO;
	public LineRenderer ray;

	//  OBJECT SPAWNER MENU
	private bool isMenuOpen;
	private GameObject objSpawnMenu_GO;
	private Image objSpawnMenu_img;
	#endregion

	//   S T A R T                                                                                                      
	void Awake()
	{
		player = GetComponent<Player>();
	}

	/*void Start()
	{
		if(GL == null) GL = GameObject.Find("GameLogic").GetComponent<GameLogic>();
		teleportLocation_GO = GameObject.Find("TeleportLocation");
		///ray = GameObject.Find("Ray").GetComponent<LineRenderer>();
	}*/

	// Init
	public void Init()
	{
		GL = GameObject.Find("GameLogic").GetComponent<GameLogic>();
		teleportLocation_GO = GameObject.Find("TeleportLocation");
		TeleportLocation_SetActive(false);

		ray.gameObject.SetActive(false);
		GL.L_controller_GO.GetComponent<ControllerCollision>().Init();
		GL.R_controller_GO.GetComponent<ControllerCollision>().Init();
		/*L_holdingObject = null;
		R_holdingObject = null;*/

		isMenuOpen = false;
		objSpawnMenu_GO  = GameObject.Find("ObjSpawnMenu_UI");
		objSpawnMenu_img = GameObject.Find("ObjSpawnMenu_UI/ObjSpawnMenu_Canvas/Obj_Img").GetComponent<Image>();
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
				{
					TeleportLocation_SetActive(false);
					hit.collider.gameObject.GetComponent<VRButton>().Hover();
				}
				else
				if(hit.transform.tag == "Ground")
				{
					if(teleportLocation_GO != null)
						teleportLocation_GO.transform.position = hit.point;
					TeleportLocation_SetActive(true);
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
			TeleportLocation_SetActive(false);

			RaycastHit hit;
			if(Physics.Raycast(GL.R_controller_GO.transform.position,
			                   GL.R_controller_GO.transform.forward, out hit, GL.rayRange, GL.rayMask))
			{
				if(hit.transform.tag != "Button")
					GL.ResetAllButtons();

				if(hit.transform.tag == "Button")
					hit.collider.gameObject.GetComponent<VRButton>().Click();
				else
				if(hit.transform.tag == "Ground" || hit.transform.tag == "Stage")
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
			GL.MovePlayer(joystickInput);

		// Hand Trigger Up
		/*if(OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, r_controller))
		{
			Debug.Log("Hand Trigger Released");
			if(holdingObject != null)
				ReleaseObject();
		}*/

		// Object Spawner Menu
		if(OVRInput.GetDown(OVRInput.Button.Two, GL.L_controller))
		{
			Debug.Log("Object Spawner Menu Button pressed");
			isMenuOpen = true;
			ObjSpawnMenu_SetActive(true);
		}

		if(OVRInput.GetUp(OVRInput.Button.Two, GL.L_controller))
		{
			Debug.Log("Object Spawner Menu Button released");
			isMenuOpen = false;
			ObjSpawnMenu_SetActive(false);
		}
	}

	public void ObjSpawnMenu_SetActive(bool state)
	{
		
	}

	// Ground Ray
	private void GroundRay(Vector3 startPoint, bool isButtonPress)
	{
		RaycastHit groundHit;
		if(Physics.Raycast(startPoint, -Vector3.up, out groundHit, GL.rayRange, GL.rayMask))
		{
			if(isButtonPress)
			{
				if(teleportLocation_GO != null)
					teleportLocation_GO.transform.position = groundHit.point;
				TeleportLocation_SetActive(true);
			}
			else
				GL.InitTeleportPlayer(groundHit.point);
		}
		else
		{
			if(isButtonPress)
				TeleportLocation_SetActive(false);
			Debug.Log("Ground Ray didn't hit ground: Cannot Teleport!");
			///Debug.Log("First Ray end: " + startPoint);
		}
	}

	private void TeleportLocation_SetActive(bool state)
	{
		if(teleportLocation_GO != null)
			teleportLocation_GO.SetActive(state);
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
