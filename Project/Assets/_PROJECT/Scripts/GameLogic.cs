// This is the main file for logic

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Analytics;
using Oculus.Platform;

// This file is alsmost accesed by every other script
using System.Globalization;


public class GameLogic : MonoBehaviour
{
	#region Global_Variables
	//  Player
	private Player player;
	public float teleportSpeed; // SPeed at which player is moved when teleporting
	public float moveSpeed;     // SPeed at which player moves using Joystick
	public Transform centerCamTransform; // Camera position at a given frame
	public float maxPlayerVelocity;      // Max velocity of player, to limit player speed while falling, to reduce VR sickness
	private AudioSource playerSpeaker;   // player AUdio Source
	public AudioClip starAudio;          // Audio when ball touches star
	public AudioClip goalAudio;			 // Audio when ball touches goal
	public AudioClip gameOverAudio;		 // Game Over sound when all levels are done
	public AudioClip teleportAudio;		 // THis is played when player starts teleporting
	///public float cameraRadius;			 

	//  Ball
	private Ball ball;
	public float ballResetSpeed; // the slowest the ball can go so that the game can reset the ball
	public float ballResetTime;  // how long the game have to wait for ball to move before resetting
	public float fanSpeed;		 // Speed of fan at which it acts on the ball

	//  Controllers
	private ControllerLayout controllerLayout; // Controller Layout script, this holds which controller bindings are held by which controller
	private ControllerInput controllerInput;	// Script for teleporting, clicking buttons and object spawning
	public OVRInput.Controller L_controller = OVRInput.Controller.LTouch; // L - Left
	public OVRInput.Controller R_controller = OVRInput.Controller.RTouch; // R - Right
	public GameObject L_controller_GO; // GO = Game Object
	public GameObject R_controller_GO;
	public LayerMask rayMask; // teleporting raycaster, which objects to ignore based on layers
	public float rayRange;    // Range of raycasting
	public float throwForce;  // FOrce at wheich the ball is thrown, this helps to add or decrease the actual force
	public int lastFrametoCalcMotion; // last frame from preset frame to calculate motion of hand while throwing ball

	//  Level
	public int currentLevel; // Preset level of the Scene
	public int totalLevels;  // Total levels in the game
	///public int totalStars;
	private int starsCollected; // total stars collected by the ball
	///private bool levelFinished;
	private GameObject[] star;
	public bool isGameStarted;

	//  Scene Changing
	private enum SceneTransition
	{
		starting,
		complete,
		ending,
		exit
	};
	private SceneTransition sceneTransition; // used for scene changing animations
	private GameObject L_mask;				 
	private GameObject R_mask;
	private Renderer L_maskRend;
	private Renderer R_maskRend;
	public Color maskColor;					 // screen tint
	public Color ballResetMaskColor;
	public Color winMaskColor;
	private Color nullColor = new Color(0,0,0,0);
	public float sceneChangingTime;			 // scene transition animation duration
	private float endTime;
	private float startTime;

	//  UI
	private VRButton[] button;
	private ControlsMenu controllerInfo = null;	 // Menu showing controller layout and option to switch controllers
	public GameObject message_GO;			 // Message overlay while playing game
	public Text message_Text;				 // text to go into that message overlay
	public float messageDuration;
	/*private GameObject gameOverMenu_GO;
	private Text levelFinished_text;
	private GameObject nextLevelText_GO;
	private GameObject gameOverText_GO;*/

	//  Object Spawner Menu
	[System.Serializable]
	public struct objSpawnerStruct // object spawner menu item
	{
		public GameObject GO;	// object prefab to spawn
		public string name;     // name of the item to display
		public int count;		// number of items allowed to spawn
		public int left;		// number of items left to spawn
		public Sprite sprite;	// image of the object to show

		public void Init()
		{
			left = count;
		}
	};
	public objSpawnerStruct[] objSpawner;

	public OculusHaptics L_haptics;
	public OculusHaptics R_haptics;

	#endregion

	//   S T A R T                                                                                                      
	void Start()
	{
		InitLevel();
		///InvokeRepeating("PrintFrameRate", 0f, 1.0f);
	}

	private void PrintFrameRate()
	{
		float temp = 1.0f/Time.deltaTime;
		if(temp < 90 && temp != 50)
			Debug.Log(Time.time + ": Frame Rate below 90, its " + temp);
	}

	//  INIT LEVEL
	private void InitLevel()
	{
		// Components
		controllerLayout = GameObject.Find("ControllerLayout").GetComponent<ControllerLayout>();
		player = GameObject.Find("Player").GetComponent<Player>();
		controllerInput = player.gameObject.GetComponent<ControllerInput>();
		L_controller_GO = GameObject.Find("Player/OVRCameraRig/TrackingSpace/LeftHandAnchor");
		R_controller_GO = GameObject.Find("Player/OVRCameraRig/TrackingSpace/RightHandAnchor");
		L_mask = GameObject.Find("Player/OVRCameraRig/TrackingSpace/LeftEyeAnchor/Mask");
		R_mask = GameObject.Find("Player/OVRCameraRig/TrackingSpace/RightEyeAnchor/Mask");
		/*L_controller_GO = GameObject.Find("Player/TrackingSpace/LeftHandAnchor");
		R_controller_GO = GameObject.Find("Player/TrackingSpace/RightHandAnchor");
		L_mask = GameObject.Find("Player/TrackingSpace/LeftEyeAnchor/Mask");
		R_mask = GameObject.Find("Player/TrackingSpace/RightEyeAnchor/Mask");*/
		L_maskRend = L_mask.GetComponent<Renderer>();
		R_maskRend = R_mask.GetComponent<Renderer>();
		GameObject temp2 = GameObject.Find("ControlsMenu_UI");
		if(temp2 != null)
			controllerInfo = temp2.GetComponent<ControlsMenu>();
		playerSpeaker = GameObject.Find("Player/OVRCameraRig/TrackingSpace/CenterEyeAnchor").GetComponent<AudioSource>();
		///playerSpeaker = GameObject.Find("Player/TrackingSpace/CenterEyeAnchor").GetComponent<AudioSource>();

		L_haptics = L_controller_GO.GetComponent<OculusHaptics>();
		R_haptics = R_controller_GO.GetComponent<OculusHaptics>();

		GameObject[] temp = GameObject.FindGameObjectsWithTag("Button");
		button = new VRButton[temp.Length];
		for(int i = 0; i < button.Length; i++)
			button[i] = temp[i].GetComponent<VRButton>(); // Stores all objeccts which are buttons

		message_GO = GameObject.Find("Player/OVRCameraRig/TrackingSpace/CenterEyeAnchor/Message_UI");
		///message_GO = GameObject.Find("Player/TrackingSpace/CenterEyeAnchor/Message_UI");
		message_Text = message_GO.GetComponentInChildren<Text>();

		// Controller Layout
		if(controllerLayout.layout == ControllerLayout.layoutEnum.reversed)
			SwitchControllers(); // if controllers are reveresed previously, then switch now.

		// Player
		player.Init();
		for(int i = 0; i < objSpawner.Length; i++)
			objSpawner[i].Init();	
		controllerInput.Init();

		// Scene Transition
		InitSceneTransition(SceneTransition.starting);
		ResetAllButtons();

		if(currentLevel != 0)	// all except main menu level - level 0
		{
			ball = GameObject.Find("Ball").GetComponent<Ball>();

			centerCamTransform = GameObject.Find("Player/OVRCameraRig/TrackingSpace/CenterEyeAnchor").transform;
			///centerCamTransform = GameObject.Find("Player/TrackingSpace/CenterEyeAnchor").transform;


			/*gameOverMenu_GO = GameObject.Find("GameOverMenu_Pivot");
			levelFinished_text
				= GameObject.Find("GameOverMenu_UI/GameOverMenu_Canvas/Heading_Text").GetComponent<Text>();
			nextLevelText_GO = GameObject.Find("GameOverMenu_UI/GameOverMenu_Canvas/NextLevel_Text");
			gameOverText_GO  = GameObject.Find("GameOverMenu_UI/GameOverMenu_Canvas/GameOver_Text");
			levelFinished_text.text = "Level " + currentLevel + " - Finished!";
			nextLevelText_GO.SetActive(true);
			gameOverMenu_GO.SetActive(false);*/

			star = GameObject.FindGameObjectsWithTag("Star");
			AllStars_SetActive(true);

			starsCollected = 0;
			///levelFinished = false;
			isGameStarted = false;
		}
	}

	private void AllStars_SetActive(bool state)
	{
		for(int i = 0; i < star.Length; i++)
			star[i].SetActive(state);
	}

	//  SCENE TRANSITION
	private void InitSceneTransition(SceneTransition sceneTrans) // Intitating scene transition by setting timer and speed
	{
		sceneTransition = sceneTrans;
		L_mask.SetActive(true);
		R_mask.SetActive(true);
		L_maskRend.material.color = maskColor;
		R_maskRend.material.color = maskColor;
		L_controller_GO.SetActive(false);
		R_controller_GO.SetActive(false);
		startTime = Time.time;
		endTime = startTime + sceneChangingTime;
	}

	//   U P D A T E                                                                                                    
	void Update()
	{
		CheckInput();
		TeleportPlayer();
		StarsLookAtPlayer();
		CheckSceneTransition();
	}

	private void StarsLookAtPlayer()
	{
		if(currentLevel != 0)
			for(int i = 0; i < star.Length; i++)
				star[i].transform.LookAt(centerCamTransform); // all stars look at player. this makes them easy to see
	}

	//  SCENE TRANSITION
	private void CheckSceneTransition()
	{
		if(sceneTransition == SceneTransition.starting) // going from dark to light
		{
			float fraction = 1f - ((Time.time - startTime) / sceneChangingTime);
			Color myColor = new Color(maskColor.r, maskColor.g, maskColor.b, fraction);
			L_maskRend.material.color = myColor;
			R_maskRend.material.color = myColor;
			if(fraction <= 0f)
			{
				sceneTransition = SceneTransition.complete;
				L_mask.SetActive(false);
				R_mask.SetActive(false);
				L_controller_GO.SetActive(true);
				R_controller_GO.SetActive(true);
			}
		}
		else if(sceneTransition == SceneTransition.ending || sceneTransition == SceneTransition.exit) // going from light to dark
		{
			float fraction = (Time.time - startTime) / sceneChangingTime;
			Color myColor = new Color(maskColor.r, maskColor.g, maskColor.b, fraction);
			L_maskRend.material.color = myColor;
			R_maskRend.material.color = myColor;
			if(fraction >= 1f)
			{
				if(sceneTransition == SceneTransition.exit)
				{
					Debug.Log("Application Exit");
					UnityEngine.Application.Quit(); // exit application at the end of exit animation
					Debug.Log("Application Exit executed in editor");
					return;
				}
				currentLevel++;
				SceneManager.LoadScene("Level" + currentLevel); // goto next scene at the end of animation for next level
				///InitLevel();
			}
		}
	}

	//  INPUT
	private void CheckInput()
	{
		if(sceneTransition == SceneTransition.complete)
			controllerInput.CheckInput(); // don't check input when scene transitioning
	}

	//  BUTTONS
	public void StartButton()
	{
		////Debug.Log("Start Button pressed!");
		///currentLevel = nextLevel;
		InitSceneTransition(SceneTransition.ending);
	}

	public void ExitButton()
	{
		///Debug.Log("Exit Button pressed!");
		InitSceneTransition(SceneTransition.exit);
	}

	public void SwitchControllersButton()
	{
		///Debug.Log("Switch Controllers Button pressed!");

		L_controller_GO.GetComponent<ControllerCollision>().ReleaseObject();
		R_controller_GO.GetComponent<ControllerCollision>().ReleaseObject();

		SwitchControllers();

		if(controllerLayout.layout == ControllerLayout.layoutEnum.normal)
			controllerLayout.layout = ControllerLayout.layoutEnum.reversed;
		else
			controllerLayout.layout = ControllerLayout.layoutEnum.normal;

		if(controllerInfo != null)
			controllerInfo.ChangeControllersInfo();

		if(controllerInput.isMenuOpen)
			controllerInput.ObjSpawnMenu_SetActive(true); // change object spawner menu also along with switching controllers
	}

	private void SwitchControllers()
	{
		OVRInput.Controller temp_controller;
		temp_controller = L_controller;
		L_controller = R_controller;
		R_controller = temp_controller;

		GameObject temp_controller_GO;
		temp_controller_GO = L_controller_GO;
		L_controller_GO = R_controller_GO;
		R_controller_GO = temp_controller_GO;

		OculusHaptics temp_haptics;
		temp_haptics = L_haptics;
		L_haptics = R_haptics;
		R_haptics = temp_haptics;

		DisplayMessage("Actions on controllers have switched");
		L_haptics.VibrateTime(VibrationForce.Medium, 1f);
		R_haptics.VibrateTime(VibrationForce.Medium, 1f);
	}

	public void RestartLevelButton()
	{
		Debug.Log("Restart Level Button pressed!");
		currentLevel--; // Decrement by one as the scene transition animation increments and goes to next level
		InitSceneTransition(SceneTransition.ending);
	}

	public void ResetBallButton() // Release ball which if holding and reset it
	{
		Debug.Log("Reset Ball Button pressed");
		ControllerCollision L_controllerCollision = L_controller_GO.GetComponent<ControllerCollision>();
		ControllerCollision R_controllerCollision = R_controller_GO.GetComponent<ControllerCollision>();
		if(L_controllerCollision.holdingObject == ball.gameObject)
		{
			Debug.Log("Releasing Ball from Left hand");
			L_controllerCollision.ReleaseObject();
			L_controllerCollision.RemoveBallFromCollision();
		}
		else if(R_controllerCollision.holdingObject == ball.gameObject)
		{
			Debug.Log("Releasing Ball from Right hand");
			R_controllerCollision.ReleaseObject();
			R_controllerCollision.RemoveBallFromCollision();
		}
		ResetBall();
	}

	public void ResetAllButtons()
	{
		for(int i = 0; i < button.Length; i++)
			button[i].Init();
	}

	//  TELEPORT PLAYER
	public void InitTeleportPlayer(Vector3 tLoc) // start teleporting only from level 1 and play audio before teleporting
	{
		if(currentLevel != 0 && sceneTransition == SceneTransition.complete)
		{
			player.InitTeleport(tLoc);
			playerSpeaker.clip = teleportAudio;
			playerSpeaker.Play();
			R_haptics.Vibrate(VibrationForce.Medium);
		}
	}

	private void TeleportPlayer() // teleport player only from level 1
	{
		if(sceneTransition == SceneTransition.complete)
			player.Teleport();
	}

	public void MovePlayer(Vector2 joystickInput) // dont move player when scene transitiioning
	{
		if(sceneTransition == SceneTransition.complete && currentLevel != 0)
			player.Move(joystickInput);
	}

	public void OpenObjectSpawMenu(bool state) // only show object spawner menu when level starts
	{
		if(currentLevel != 0)
		{
			controllerInput.ObjSpawnMenu_SetActive(state);
			L_haptics.Vibrate(VibrationForce.Medium);
		}
	}

	//  BALL
	public void BallTouchedGround() 
	{
		///Debug.Log("Ball Touched Ground/Stage");
		L_mask.SetActive(true);
		R_mask.SetActive(true);
		L_maskRend.material.color = ballResetMaskColor;
		R_maskRend.material.color = ballResetMaskColor; // show red tint for hald second when ball touches ground
		Invoke("ResetMasks", 0.5f);
		ResetBall();
		L_haptics.Vibrate(VibrationForce.Medium);
		L_haptics.Vibrate(VibrationForce.Medium);
		R_haptics.Vibrate(VibrationForce.Medium);
		R_haptics.Vibrate(VibrationForce.Medium);
		///playerSpeaker.clip = failedTryAudio;
		///playerSpeaker.Play();
	}

	private void ResetMasks()
	{
		L_mask.SetActive(false);
		R_mask.SetActive(false);
		L_maskRend.material.color = nullColor;
		R_maskRend.material.color = nullColor;
	}

	private void ResetBall()
	{
		////Debug.Log("Ball Reset");
		ball.Reset();
		ResetGame();
	}

	public void ResetGame()
	{
		starsCollected = 0;
		isGameStarted = false;
		AllStars_SetActive(true);
	}

	public void BallTouchedFinish()
	{
		if(ball.gameObject.transform.parent == null) // check if ball is wantedly touched while holding in hand
		{
			Debug.Log("**** Ball touched Finish, " + starsCollected + ":" + star.Length);///totalStars);
			if(isGameStarted) // check if ball is thrown from stage
			{
				if(starsCollected >= star.Length) // check if all stars are collected
				{
					Debug.Log("Level Finished");
					///levelFinished = true;
					L_mask.SetActive(true);
					R_mask.SetActive(true);
					L_maskRend.material.color = winMaskColor;
					R_maskRend.material.color = winMaskColor;
					Invoke("ResetMasks", 0.5f);
					ball.gameObject.SetActive(false);
					if(currentLevel == totalLevels)
					{
						player.nextLevelText_GO.SetActive(false);
						player.gameOverText_GO.SetActive(true);
						playerSpeaker.clip = gameOverAudio;

						L_haptics.VibrateTime(VibrationForce.Hard, 1.5f);
						R_haptics.VibrateTime(VibrationForce.Hard, 1.5f);
					}
					else
					{
						playerSpeaker.clip = goalAudio;
						L_haptics.Vibrate(VibrationForce.Hard);
						L_haptics.Vibrate(VibrationForce.Hard);
						L_haptics.Vibrate(VibrationForce.Hard);
						R_haptics.Vibrate(VibrationForce.Hard);
						R_haptics.Vibrate(VibrationForce.Hard);
						R_haptics.Vibrate(VibrationForce.Hard);
					}

					playerSpeaker.Play();
					player.gameOverMenu_GO.SetActive(true);
					Invoke("ChangeLevel", 3.333f);
				}
				else
				{
					DisplayMessage("Only " + starsCollected + " of " + star.Length + " star(s) collected!");
					BallTouchedGround();
				}
			}
			else
			{
				DisplayMessage("Ball must be thrown from the stage");
				BallTouchedGround();
			}
		}
	}

	public void DisplayMessage(string message)
	{
		///Debug.Log("Display Message called with message: " + message);
		message_Text.text = message;
		message_GO.SetActive(true);
		CancelInvoke("TurnOff_message_GO");
		Invoke("TurnOff_message_GO", messageDuration); // Disable message after set time
	}

	public void TurnOff_message_GO()
	{
		///Debug.Log("Display Message Turned off");
		message_GO.SetActive(false);
	}

	public void BallTouchedStar(GameObject collidedStar)
	{
		if(ball.gameObject.transform.parent == null && isGameStarted) // check if ball is wantedly touched by holding in hand
		{
			playerSpeaker.clip = starAudio;
			playerSpeaker.Play();
			L_haptics.Vibrate(VibrationForce.Hard);
			R_haptics.Vibrate(VibrationForce.Hard);

			starsCollected++;
			collidedStar.SetActive(false);
			///Debug.Log("** Ball touched Star | Total Stars Collected: " + starsCollected);
			DisplayMessage(starsCollected + " of " + star.Length + " star(s) collected");
		}
	}

	public void BallLaunched()
	{
		Debug.Log("Ball launched from platform ---------------------------");
		isGameStarted = true;
	}

	private void ChangeLevel()
	{
		if(totalLevels != currentLevel) 
			InitSceneTransition(SceneTransition.ending);
		else // goto main menu after last level
		{
			Debug.Log("Moving to Main Menu!");
			currentLevel = -1;
			InitSceneTransition(SceneTransition.ending);
		}
	}
}
