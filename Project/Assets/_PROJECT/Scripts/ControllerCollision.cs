using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// THis script is mainly for grabbing and releasing objects

public class ControllerCollision : MonoBehaviour
{
	#region Global_Variables
	private GameLogic GL;
	public Player player;

	private OVRInput.Controller controller;
	private ControllerCollision otherCC;
	private GameObject otherController_GO;

	private Vector3[] lastPosition; // last positions and time are saved every frame to calculate throwing direction and force
	private Vector3[] playerLastPosition;
	private float[] timeStamp;

	//  Grab
	public GameObject holdingObject;
	private GameObject collidingObject;
	public Rigidbody holdingObject_rigidbody;
	private bool collisionHappenedFirst; // this helps to determine if collison with object happened before pressing the trigger
	private bool handTriggerPressed;
	private bool colliding;

	//  Debug
	public string controller_name;

	private OculusHaptics haptics;
	#endregion

	//   S T A R T                                                                                                      
	void Awake()
	{
		haptics = GetComponent<OculusHaptics>();
	}

	/*void Start()
	{
		GL = GameObject.Find("GameLogic").GetComponent<GameLogic>();
		player = GameObject.Find("Player").GetComponent<Player>();
	}*/

	// INIT
	public void Init()
	{
		GL = GameObject.Find("GameLogic").GetComponent<GameLogic>();
		///player = GameObject.Find("Player").GetComponent<Player>();

		lastPosition = new Vector3[GL.lastFrametoCalcMotion];
		playerLastPosition = new Vector3[GL.lastFrametoCalcMotion];
		timeStamp    = new float[GL.lastFrametoCalcMotion];

		GetController(); // this function is called to tell which controller is this - left or right
		ReleaseObject();
		collisionHappenedFirst = false;
		handTriggerPressed = false;
		colliding = false;
		for(int i = 0; i < lastPosition.Length; i++)
		{
			lastPosition[i] 	  = transform.position;
			playerLastPosition[i] = player.transform.position;
			timeStamp[i]    	  = Time.time;
		}
	}

	private void GetController()
	{
		//  Left
		if(gameObject == GL.L_controller_GO)
		{
			controller = GL.L_controller;
			controller_name = "Left"; /// Debug
			otherController_GO = GL.R_controller_GO;
			otherCC = otherController_GO.GetComponent<ControllerCollision>();
		}
		//  Right
		else
		{
			controller = GL.R_controller;
			controller_name = "Right"; /// Debug
			otherController_GO = GL.L_controller_GO;
			otherCC = otherController_GO.GetComponent<ControllerCollision>();
		}
	}

	//   U P D A T E                                                                                                    
	void Update()
	{
		UpdateLastPosition();

		GetController();
		if(OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, controller)) // when hand trigger is pressed
		{
			///Debug.Log(Time.time + " - " controller_name + ": Hand Trigger is being Pressed");
			handTriggerPressed = true;
			if(collisionHappenedFirst && holdingObject == null)
			{
				////Debug.Log(Time.time + " - " + controller_name + " controller Trigger is pressed while colliding with "
						///+ collidingObject.name);
				if(otherCC.holdingObject != null) // this is to check if the object about to hold is already held by the other hand
					if(otherCC.holdingObject.transform.parent.gameObject == otherController_GO)
					{
						///otherController_GO.GetComponent<ControllerCollision>().ReleaseObject();
						////Debug.Log("Taking over " + collidingObject.name + " from " + otherCC.controller_name + " controller");
						otherCC.holdingObject.transform.parent = null; // if so, remove links of that object with previous controller
						otherCC.holdingObject = null;
						otherCC.holdingObject_rigidbody = null;
						otherCC.collisionHappenedFirst = false;
						otherCC.GetComponent<OculusHaptics>().Vibrate(VibrationForce.Light);
						///otherCC.ReleaseObject();
					}

				holdingObject = collidingObject.gameObject; //  // now take control of the object which was colliding
				holdingObject.transform.SetParent(gameObject.transform);
				holdingObject_rigidbody = holdingObject.GetComponent<Rigidbody>();
				holdingObject_rigidbody.isKinematic = true; // turn off the physics of that object
				if(holdingObject.name == "Ball") // Reset the ball, this is to stop picking or catching ball after throwing
					GL.ResetGame();
				haptics.Vibrate(VibrationForce.Light);

				/*Physics.IgnoreCollision(holdingObject.GetComponent<Collider>(),
				                        player.GetComponent<Collider>());*/

				///Debug.Log(": Already Colliding, so Holding object");
			}
		}
		else
		{
			/*if(handTriggerPressed && holdingObject != null)
				Debug.Log(Time.time + ": Trigger Released");*/
			handTriggerPressed = false;
			ReleaseObject(); // Rlease object when hand trigger is released
		}
	}

	//  Last Position
	private void UpdateLastPosition() // take note of posiiton every frame
	{
		for(int i = lastPosition.Length-1; i >= 1; i--)
		{
			///Debug.Log("i = " + i);
			lastPosition[i] = lastPosition[i-1];
			playerLastPosition[i] = playerLastPosition[i-1];
			timeStamp[i]    = timeStamp[i-1];
		}
		lastPosition[0] = transform.position;
		///transform.Rotate(player.transform.rotation.eulerAngles);
		///lastPosition[0] = /*transform.TransformPoint(*/transform.localPosition;///);
		///transform.Rotate(-player.transform.rotation.eulerAngles);
		///lastPosition[0] = transform.TransformVector(OVRInput.GetLocalControllerPosition(controller));
		playerLastPosition[0] = player.transform.position;
		timeStamp[0]    = Time.time;
	}

	//  Collision Enter
	void OnTriggerEnter(Collider collider)
	{
		///Debug.Log(Time.time + ": Started colliding with " + collider.gameObject.tag + " object");
		if(collider.gameObject.tag == "Throwable" || collider.gameObject.tag == "Grabbable")
		{
			if(!handTriggerPressed && !colliding) // when colliding with objects, seee if hand controller is pressed first
			{
				///Debug.Log(": Collision happened first");
				collisionHappenedFirst = true;
			}
			colliding = true;
			if(collider.gameObject.tag == "Grabbable")
			{
				collidingObject = collider.transform.parent.gameObject;
				return;
			}
			collidingObject = collider.gameObject; // take a reference of the colliding object to check hand trigger input
		}
	}

	void OnTriggerStay(Collider collider) // this is useful if Trigger ENter is missed
	{
		if(!colliding && !handTriggerPressed &&
		   (collider.gameObject.tag == "Throwable" || collider.gameObject.tag == "Grabbable"))
		{
			///Debug.Log(Time.time + ": Started colliding with " + collider.gameObject.tag + " object");
			///Debug.Log(": Collision happened first - s*");
			collisionHappenedFirst = true;
			if(collider.gameObject.tag == "Grabbable")
			{
				collidingObject = collider.transform.parent.gameObject;
				return;
			}
			collidingObject = collider.gameObject;
		}
	}

	//  Collision Exit
	void OnTriggerExit(Collider collider)
	{
		///Debug.Log(Time.time + ": Stopped colliding with " + collider.gameObject.tag + " object");
		if(collider.gameObject.tag == "Throwable" || collider.gameObject.tag == "Grabbable")
			RemoveBallFromCollision();
	}

	public void RemoveBallFromCollision()
	{
		colliding = false;
		collisionHappenedFirst = false;
	}

	//  Release
	public void ReleaseObject()
	{
		if(holdingObject != null) // Release object only when holding an object
		{
			////Debug.Log(controller_name + " controller Released " + holdingObject.name);
			holdingObject.transform.SetParent(null);
			if(holdingObject.transform.tag == "Throwable") // if the object is of throwable type, then add direction and force
			{
				holdingObject_rigidbody.isKinematic = false;
				
				/*Vector3 a = lastPosition[lastPosition.Length - 1];
				Vector3 b = lastPosition[0];*/
				float timeTaken = timeStamp[0] - timeStamp[timeStamp.Length - 1];
				Vector3 playerVelocity = (playerLastPosition[0] - playerLastPosition[lastPosition.Length - 1]) / timeTaken;
				Vector3 handVelocity = ((lastPosition[0] - lastPosition[lastPosition.Length - 1]) / timeTaken) - playerVelocity;
				holdingObject_rigidbody.velocity = playerVelocity + (handVelocity * GL.throwForce);
				////Debug.Log("Player: " + playerVelocity.magnitude + "; Hand: " + handVelocity.magnitude
				///		+ "; Ball: " + holdingObject_rigidbody.velocity.magnitude);
				///float force = GL.throwForce;
				/*if(OVRInput.GetLocalControllerVelocity(controller).magnitude < 0.001f)
					force = 1f;*/
				///holdingObject_rigidbody.velocity = ((b - a) * GL.throwForce) / timeTaken;
				///holdingObject_rigidbody.velocity = /*transform.TransformVector*/(b - a) / timeTaken; // difference in distance and total time taken of last 5 frames

				/*Debug.Log("Controller Velocity was " + OVRInput.GetLocalControllerVelocity(controller) + " which is "
				         							 + OVRInput.GetLocalControllerVelocity(controller).magnitude);
				holdingObject_rigidbody.velocity =
							transform.TransformDirection(OVRInput.GetLocalControllerVelocity(controller));*/
				/*Vector3 hr = holdingObject.transform.localRotation.eulerAngles;
				Vector3 pr = player.gameObject.transform.rotation.eulerAngles;
				holdingObject.transform.rotation = Quaternion.Euler(hr+pr);*/

				holdingObject_rigidbody.angularVelocity =
							transform.TransformDirection(OVRInput.GetLocalControllerAngularVelocity(controller)); // change angle by taking hand controller angle too
				////holdingObject.transform.SetParent(null);

				///holdingObject.transform.Rotate(player.gameObject.transform.rotation.eulerAngles);
				if(holdingObject.name == "Ball")
					if(player.isOnPlatform)
						GL.BallLaunched(); // determine if ball is launched from stage
					else
						GL.DisplayMessage("Throw the ball from the platform");
			}
			holdingObject = null;
			holdingObject_rigidbody = null;
			haptics.Vibrate(VibrationForce.Light);
		}
	}
}
