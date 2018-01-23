using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Analytics;
using Oculus.Platform;

public class GameLogic : MonoBehaviour
{
	#region Global_Variables
	//  Player
	private Player player;
	public float teleportSpeed;
	public float moveSpeed;
	public Transform centerCamTransform;
	public float maxPlayerVelocity;
	private AudioSource playerSpeaker;
	public AudioClip starAudio;
	public AudioClip goalAudio;
	public AudioClip gameOverAudio;
	public AudioClip teleportAudio;
	public float cameraRadius;

	//  Ball
	private Ball ball;
	public float ballResetSpeed;
	public float ballResetTime;
	public float fanSpeed;

	//  Controllers
	private ControllerLayout controllerLayout;
	private ControllerInput controllerInput;
	public OVRInput.Controller L_controller = OVRInput.Controller.LTouch;
	public OVRInput.Controller R_controller = OVRInput.Controller.RTouch;
	public GameObject L_controller_GO;
	public GameObject R_controller_GO;
	public LayerMask rayMask;
	public float rayRange;
	public float throwForce;
	public int lastFrametoCalcMotion;

	//  Level
	public int currentLevel;
	public int totalLevels;
	///public int totalStars;
	private int starsCollected;
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
	private SceneTransition sceneTransition;
	private GameObject L_mask;
	private GameObject R_mask;
	private Renderer L_maskRend;
	private Renderer R_maskRend;
	public Color maskColor;
	public Color ballResetMaskColor;
	public Color winMaskColor;
	private Color nullColor = new Color(0,0,0,0);
	public float sceneChangingTime;
	private float endTime;
	private float startTime;

	//  UI
	private VRButton[] button;
	private ControlsMenu controllerInfo;
	public GameObject message_GO;
	public Text message_Text;
	/*private GameObject gameOverMenu_GO;
	private Text levelFinished_text;
	private GameObject nextLevelText_GO;
	private GameObject gameOverText_GO;*/

	//  Object Spawner Menu
	[System.Serializable]
	public struct objSpawnerStruct
	{
		public GameObject GO;
		public string name;
		public int count;
		public int left;
		public Sprite sprite;

		public void Init()
		{
			left = count;
		}
	};
	public objSpawnerStruct[] objSpawner;

	#endregion

	//   S T A R T                                                                                                      
	void Start()
	{
		InitLevel();
		InvokeRepeating("PrintFrameRate", 0f, 1.0f);
	}

	private void PrintFrameRate()
	{
		float temp = 1.0f/Time.deltaTime;
		///if(temp < 90 && temp != 50)
			////Debug.Log(Time.time + ": Frame Rate below 90, its " + temp);
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
		controllerInfo = GameObject.Find("ControlsMenu_UI").GetComponent<ControlsMenu>();
		playerSpeaker = GameObject.Find("Player/OVRCameraRig/TrackingSpace/CenterEyeAnchor").GetComponent<AudioSource>();
		///playerSpeaker = GameObject.Find("Player/TrackingSpace/CenterEyeAnchor").GetComponent<AudioSource>();

		GameObject[] temp = GameObject.FindGameObjectsWithTag("Button");
		button = new VRButton[temp.Length];
		for(int i = 0; i < button.Length; i++)
			button[i] = temp[i].GetComponent<VRButton>();

		message_GO = GameObject.Find("Player/OVRCameraRig/TrackingSpace/CenterEyeAnchor/Message_UI");
		///message_GO = GameObject.Find("Player/TrackingSpace/CenterEyeAnchor/Message_UI");
		message_Text = message_GO.GetComponentInChildren<Text>();

		// Controller Layout
		if(controllerLayout.layout == ControllerLayout.layoutEnum.reversed)
			SwitchControllers();

		// Player
		player.Init();
		for(int i = 0; i < objSpawner.Length; i++)
			objSpawner[i].Init();
		controllerInput.Init();

		// Scene Transition
		InitSceneTransition(SceneTransition.starting);
		ResetAllButtons();

		if(currentLevel != 0)
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
	private void InitSceneTransition(SceneTransition sceneTrans)
	{
		sceneTransition = sceneTrans;
		L_mask.SetActive(true);
		R_mask.SetActive(true);
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
				star[i].transform.LookAt(centerCamTransform);
	}

	//  SCENE TRANSITION
	private void CheckSceneTransition()
	{
		if(sceneTransition == SceneTransition.starting)
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
		else if(sceneTransition == SceneTransition.ending || sceneTransition == SceneTransition.exit)
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
					UnityEngine.Application.Quit();
					Debug.Log("Application Exit executed in editor");
					return;
				}
				currentLevel++;
				SceneManager.LoadScene("Level" + currentLevel);
				///InitLevel();
			}
		}
	}

	//  INPUT
	private void CheckInput()
	{
		if(sceneTransition == SceneTransition.complete)
			controllerInput.CheckInput();
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

		controllerInfo.ChangeControllersInfo();

		if(controllerInput.isMenuOpen)
			controllerInput.ObjSpawnMenu_SetActive(true);
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

		DisplayMessage("Actions on controllers have switched");
	}

	public void RestartLevelButton()
	{
		Debug.Log("Restart Level Button pressed!");
		currentLevel--;
		InitSceneTransition(SceneTransition.ending);
	}

	public void ResetBallButton()
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
	public void InitTeleportPlayer(Vector3 tLoc)
	{
		if(currentLevel != 0 && sceneTransition == SceneTransition.complete)
		{
			player.InitTeleport(tLoc);
			playerSpeaker.clip = teleportAudio;
			playerSpeaker.Play();
		}
	}

	private void TeleportPlayer()
	{
		if(sceneTransition == SceneTransition.complete)
			player.Teleport();
	}

	public void MovePlayer(Vector2 joystickInput)
	{
		if(sceneTransition == SceneTransition.complete && currentLevel != 0)
			player.Move(joystickInput);
	}

	public void OpenObjectSpawMenu(bool state)
	{
		if(currentLevel != 0)
			controllerInput.ObjSpawnMenu_SetActive(state);
	}

	//  BALL
	public void BallTouchedGround()
	{
		///Debug.Log("Ball Touched Ground/Stage");
		L_mask.SetActive(true);
		R_mask.SetActive(true);
		L_maskRend.material.color = ballResetMaskColor;
		R_maskRend.material.color = ballResetMaskColor;
		Invoke("ResetMasks", 0.5f);
		ResetBall();
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
		if(ball.gameObject.transform.parent == null)
		{
			Debug.Log("**** Ball touched Finish, " + starsCollected + ":" + star.Length);///totalStars);
			if(isGameStarted)
			{
				if(starsCollected >= star.Length)
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
					}
					else
						playerSpeaker.clip = goalAudio;

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
		message_Text.text = message;
		message_GO.SetActive(true);
		CancelInvoke("TurnOff_message_GO");
		Invoke("TurnOff_message_GO", 3f);
	}

	public void TurnOff_message_GO()
	{
		message_GO.SetActive(false);
	}

	public void BallTouchedStar(GameObject collidedStar)
	{
		if(ball.gameObject.transform.parent == null && isGameStarted)
		{
			playerSpeaker.clip = starAudio;
			playerSpeaker.Play();

			starsCollected++;
			collidedStar.SetActive(false);
			Debug.Log("** Ball touched Star | Total Stars Collected: " + starsCollected);
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
		else
		{
			Debug.Log("Moving to Main Menu!");
			currentLevel = -1;
			InitSceneTransition(SceneTransition.ending);
		}
	}
}
