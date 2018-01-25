using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Experimental.U2D;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Experimental.UIElements.StyleEnums;

public class Player : MonoBehaviour
{
	#region Global_Variables
	private GameLogic GL;

	//  Teleport
	private bool isMoving;
	public bool isOnPlatform;
	private Vector3 teleportLocation;
	private Vector3 startPosition;
	private float lerpDistance;
	private float totalDistance;

	//  Scene Transition
	public GameObject camera;

	//  RigidBody
	private Rigidbody rigidbody;

	//  Game Over Menu
	public GameObject gameOverMenu_GO;
	public Text 	  levelFinished_text;
	public GameObject nextLevelText_GO;
	public GameObject gameOverText_GO; // display gameover infront of camera
	#endregion
	
	//   S T A R T                                                                                                      
	void Awake()
	{
		rigidbody = GetComponent<Rigidbody>();
	}

	/*void Start()
	{
		
	}*/

	public void Init()
	{
		GL = GameObject.Find("GameLogic").GetComponent<GameLogic>();
		///camera = GameObject.Find("Player/OVRCameraRig/TrackingSpace/CenterEyeAnchor");

		isMoving = false;
		///isOnPlatform = false;

		levelFinished_text.text = "Level " + GL.currentLevel + " - Finished!";
	}

	public void InitTeleport(Vector3 tLoc)
	{
		teleportLocation = tLoc;
		startPosition = rigidbody.position;
		isMoving = true;
		rigidbody.isKinematic = true; // make player not colliding with anything when teleporting
		lerpDistance = 0;
		totalDistance = Vector3.Distance(startPosition, teleportLocation); // setting up distance to use lerp
	}

	//   U P D A T E                                                                                                    
	public void Teleport()
	{
		if(isMoving)
		{
			lerpDistance = lerpDistance + ((Time.deltaTime * GL.teleportSpeed) / totalDistance);
			rigidbody.position = Vector3.Lerp(startPosition, teleportLocation, lerpDistance); // calculate lerp by framerate for smooth movement
			if(lerpDistance >= 1)
			{
				///rigidbody.position = teleportLocation;
				transform.position = teleportLocation;
				isMoving = false;
				rigidbody.isKinematic = false; // make player collider with objects again after teleporting
			}
		}
	}

	public void Move(Vector2 joystickInput)
	{
		/*Vector3 joystickDirection = new Vector3(joystickInput.x, 0, joystickInput.y);
		Vector3 cameraDirection = new Vector3(camera.transform.forward.x, 0, camera.transform.forward.z);
		float angle = AngleBetween(Vector3.forward, cameraDirection) + AngleBetween(Vector3.forward, joystickDirection);
		Vector3 direction = Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward;
		direction = direction * moveSpeed * Time.deltaTime;
		rigidbody.position += direction;*/
		if(isMoving) // stop teleporting when joystick input is given
		{
			isMoving = false;
			rigidbody.isKinematic = false;
		}
		Vector3 joystickDirection = new Vector3(joystickInput.x, 0, joystickInput.y); // convert joystick input to XZ direction with respect to camera
		Vector3 direction = camera.transform.TransformDirection(joystickDirection);
		direction = new Vector3(direction.x, 0, direction.z);
		direction = direction * GL.moveSpeed * Time.deltaTime; // move player with certain speed set in gamelogic and framte rate
		///rigidbody.position += direction;
		transform.position += direction;
	}

	/*private float AngleBetween(Vector3 first, Vector3 second)
	{
		float angle = Vector3.Angle(first, second);
		Vector3 cross = Vector3.Cross(first, second);
		if(cross.y < 0)
			angle = -angle;
		return angle;
	}*/

	void OnTriggerStay(Collider collider)
	{
		///Debug.Log("Player standing on object with tag: " + collider.transform.tag + "; " + isOnPlatform);
		if(collider.transform.tag == "Stage") // to check if player is standing on stage
			isOnPlatform = true; 
		else
			isOnPlatform = false;
	}

	void OnTriggerExit()
	{
		isOnPlatform = false;
	}

	void Update()
	{
		if(rigidbody.velocity.magnitude >= GL.maxPlayerVelocity) // this is to limit player speed, to reduce VR sickness
		{
			///Debug.Log("Player velocirt was: " + rigidbody.velocity.magnitude);
			rigidbody.velocity = rigidbody.velocity.normalized * GL.maxPlayerVelocity;
			///Debug.Log(": Corrected to " + rigidbody.velocity.magnitude);
		}
		///AdjustPlayerWithCamera();
	}

	/*private void AdjustPlayerWithCamera()
	{
		Vector3 cameraPos = camera.transform.position;
		Vector3 camPos = new Vector3(cameraPos.x, transform.position.y, cameraPos.z);
		float dist = Mathf.Abs(Vector3.Distance(transform.position, camPos));
		float lerpDist = (dist - GL.cameraRadius)/GL.cameraRadius;
		if(dist > GL.cameraRadius)
		{
			Debug.Log("Camera = " + cameraPos + ", corrected = " + camPos + "; Player = " + transform.position);
			Vector3 lerpPos = Vector3.Lerp(transform.position, camPos, lerpDist);
			Debug.Log("Distance between = " + dist + ", lerpDist = " + lerpDist + "; lerpPos = " + lerpPos);
			transform.position = lerpPos;
			camera.transform.position = cameraPos;
			Debug.Log("New Camera position = " + camera.transform.position);
		}
	}*/
}
