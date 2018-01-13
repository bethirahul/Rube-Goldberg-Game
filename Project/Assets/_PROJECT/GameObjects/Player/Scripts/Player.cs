using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.WSA;
using UnityEngine.Experimental.U2D;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
	#region Global_Variables
	public GameLogic GL;

	//  Teleport
	private bool isMoving;
	private Vector3 teleportLocation;
	private Vector3 startPosition;
	private float lerpDistance;
	private float totalDistance;

	//  Scene Transition
	public GameObject camera;

	//  RigidBody
	private Rigidbody rigidbody;
	#endregion
	
	//   S T A R T                                                                                                      
	void Start()
	{
		rigidbody = GetComponent<Rigidbody>();
		rigidbody.mass = 70;
	}

	public void Init()
	{
		isMoving = false;
	}

	public void InitTeleport(Vector3 tLoc)
	{
		teleportLocation = tLoc;
		startPosition = rigidbody.position;
		isMoving = true;
		rigidbody.isKinematic = true;
		lerpDistance = 0;
		totalDistance = Vector3.Distance(startPosition, teleportLocation);
	}

	//   U P D A T E                                                                                                    
	public void Teleport()
	{
		if(isMoving)
		{
			lerpDistance = lerpDistance + ((Time.deltaTime * GL.teleportSpeed) / totalDistance);
			rigidbody.position = Vector3.Lerp(startPosition, teleportLocation, lerpDistance);
			if(lerpDistance >= 1)
			{
				///rigidbody.position = teleportLocation;
				transform.position = teleportLocation;
				isMoving = false;
				rigidbody.isKinematic = false;
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

		Vector3 joystickDirection = new Vector3(joystickInput.x, 0, joystickInput.y);
		Vector3 direction = camera.transform.TransformDirection(joystickDirection);
		direction = new Vector3(direction.x, 0, direction.z);
		direction = direction * GL.moveSpeed * Time.deltaTime;
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
}
