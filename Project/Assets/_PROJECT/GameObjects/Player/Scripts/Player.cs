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
	private bool isMoving;
	private Vector3 teleportLocation;
	public float teleportSpeed;
	private Vector3 startPosition;
	private float lerpDistance;
	private float totalDistance;
	public float moveSpeed;

	public GameObject camera;
	public GameObject r_controller_GO;
	private Collider collider;
	private Rigidbody rigidbody;
	#endregion
	
	//   S T A R T                                                                                                      
	void Start()
	{
		collider = GetComponent<Collider>();
		rigidbody = GetComponent<Rigidbody>();
		rigidbody.mass = 70;
		Physics.IgnoreCollision(collider, r_controller_GO.GetComponent<Collider>());
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
			lerpDistance = lerpDistance + ((Time.deltaTime * teleportSpeed) / totalDistance);
			///gameObject.transform.position = Vector3.Lerp(startPosition, teleportLocation, lerpDistance);
			rigidbody.position = Vector3.Lerp(startPosition, teleportLocation, lerpDistance);
			if(lerpDistance >= 1)
			{
				rigidbody.position = teleportLocation;
				isMoving = false;
				rigidbody.isKinematic = false;
			}
		}
	}

	public void Move(Vector2 joystickInput)
	{
		Vector3 joystickDirection = new Vector3(joystickInput.x, 0, joystickInput.y);
		Vector3 cameraDirection = new Vector3(camera.transform.forward.x, 0, camera.transform.forward.z);
		/*float angle1 = GetAngle(Vector3.forward, cameraDirection);
		float angle2 = GetAngle(Vector3.forward, joystickDirection);
		float angle3 = angle1 + angle2;*/
		float angle = AngleBetween(Vector3.forward, cameraDirection) + AngleBetween(Vector3.forward, joystickDirection);
		Vector3 direction = Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward;
		direction = direction * moveSpeed * Time.deltaTime;
		///gameObject.transform.position = gameObject.transform.position + direction;
		rigidbody.position += direction;
	}

	private float AngleBetween(Vector3 first, Vector3 second)
	{
		float angle = Vector3.Angle(first, second);
		Vector3 cross = Vector3.Cross(first, second);
		if(cross.y < 0)
			angle = -angle;
		return angle;
	}
}
