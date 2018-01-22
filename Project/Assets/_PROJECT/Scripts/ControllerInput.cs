using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml.Linq;
using UnityEngine.Experimental.UIElements.StyleEnums;


///using UnityEngine.iOS;
///using UnityEngine.Experimental.UIElements;

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
	private Image obj_img;
	private Text objName_text;
	private Text objCount_text;
	private int displayCount;
	private bool isChangedAlready;
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
		objSpawnMenu_GO = GameObject.Find("ObjSpawnMenu_UI");
		obj_img 	    = GameObject.Find("ObjSpawnMenu_UI/ObjSpawnMenu_Canvas/Obj_Img").GetComponent<Image>();
		objName_text    = GameObject.Find("ObjSpawnMenu_UI/ObjSpawnMenu_Canvas/ObjName_Text").GetComponent<Text>();
		objCount_text   = GameObject.Find("ObjSpawnMenu_UI/ObjSpawnMenu_Canvas/Count_Text").GetComponent<Text>();
		ObjSpawnMenu_SetActive(false);
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
			ObjSpawnMenu_SetActive(!isMenuOpen);
		}

		/*if(OVRInput.GetUp(OVRInput.Button.Two, GL.L_controller))
		{
			Debug.Log("Object Spawner Menu Button released");
			ObjSpawnMenu_SetActive(false);
		}*/

		if(isMenuOpen)
		{
			joystickInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, GL.L_controller);
			/*if(joystickInput != Vector2.zero)
				Debug.Log("Joystick Input: " + joystickInput.x);*/
			if(Mathf.Abs(joystickInput.x) < 0.3f && isChangedAlready)
				isChangedAlready = false;
			else if(Mathf.Abs(joystickInput.x) >= 0.8f && !isChangedAlready)
			{
				isChangedAlready = true;
				SetSpawnObject((int)Mathf.Round(joystickInput.x));
			}
			if(OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick, GL.L_controller))
			{
				Debug.Log("Joystick Button Pressed");
				///////////////////////////////////Spawn object
			}
		}
	}

	public void ObjSpawnMenu_SetActive(bool state)
	{
		isMenuOpen = state;
		if(state)
		{
			isChangedAlready = false;
			objSpawnMenu_GO.SetActive(true);
			displayCount = 0;
			SetSpawnObject(displayCount);
			objSpawnMenu_GO.transform.SetParent(GL.L_controller_GO.transform);
			objSpawnMenu_GO.transform.rotation = GL.L_controller_GO.transform.rotation;
			objSpawnMenu_GO.transform.position = GL.L_controller_GO.transform.TransformPoint(new Vector3(0,0.175f,0));
		}
		else
		{
			objSpawnMenu_GO.transform.SetParent(null);
			objSpawnMenu_GO.SetActive(false);
		}
	}

	private void SetSpawnObject(int inc)
	{
		displayCount = displayCount + inc;
		if(displayCount < 0)
			displayCount = GL.objSpawner.Length - 1;
		else if(displayCount >= GL.objSpawner.Length)
			displayCount = 0;
		Debug.Log("Object set to " + displayCount);
		obj_img.sprite = GL.objSpawner[displayCount].sprite;
		objName_text.text = GL.objSpawner[displayCount].name;
		objCount_text.text = GL.objSpawner[displayCount].left + "/" + GL.objSpawner[displayCount].count + " left";
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
