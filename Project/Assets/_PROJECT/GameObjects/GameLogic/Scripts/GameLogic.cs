using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
	#region Global_Variables
	//  Player
	public Player player;
	public float teleportSpeed;
	public float moveSpeed;
	public float playerWeight;

	//  Controllers
	private ControllerInput controllerInput;
	public OVRInput.Controller L_controller;
	public OVRInput.Controller R_controller;
	public GameObject L_controller_GO;
	public GameObject R_controller_GO;
	public LayerMask rayMask;
	public float rayRange;
	public float throwForce;
	public int lastFrametoCalcMotion;
	public GameObject teleportLocation_GO;

	//  Level
	public int currentLevel;
	///public static int nextLevel;

	//  Scene Changing
	private enum SceneTransition
	{
		starting,
		complete,
		ending,
		exit
	};
	private SceneTransition sceneTransition;
	public GameObject L_mask;
	public GameObject R_mask;
	private Renderer L_maskRend;
	private Renderer R_maskRend;
	public float sceneChangingTime;
	private float endTime;
	private float startTime;

	//  UI
	public VRButton mm_start_btn;
	public VRButton mm_exit_btn;
	public VRButton mm_switch_btn;
	public GameObject A_info;
	public GameObject B_info;
	#endregion

	//   A W A K E                                                                                                      
	void Awake()
	{
		DontDestroyOnLoad(transform.gameObject);
	}

	//   S T A R T                                                                                                      
	void Start()
	{
		InitLevel();
	}

	//  INIT LEVEL
	private void InitLevel()
	{
		// Components
		controllerInput = player.gameObject.GetComponent<ControllerInput>();
		L_maskRend = L_mask.GetComponent<Renderer>();
		R_maskRend = R_mask.GetComponent<Renderer>();

		Physics.IgnoreCollision(player.GetComponent<Collider>(), L_controller_GO.GetComponent<Collider>());
		Physics.IgnoreCollision(player.GetComponent<Collider>(), R_controller_GO.GetComponent<Collider>());

		// Player
		player.Init();
		controllerInput.Init();

		// Scene Transition
		InitSceneTransition(SceneTransition.starting);
		teleportLocation_GO.SetActive(false);
	}

	public void TeleportLocation_GO_SetActive(bool state)
	{
		if(currentLevel != 0)
			teleportLocation_GO.SetActive(state);
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
		CheckSceneTransition();
		CheckInput();
		TeleportPlayer();
		MoveGameObjects();
	}

	//  SCENE TRANSITION
	private void CheckSceneTransition()
	{
		if(sceneTransition == SceneTransition.starting)
		{
			float fraction = 1f - ((Time.time - startTime) / sceneChangingTime);
			Color color = L_maskRend.material.color;
			color = new Color(color.r, color.g, color.b, fraction);
			L_maskRend.material.color = color;
			R_maskRend.material.color = color;
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
			Color myColor = L_maskRend.material.color;
			myColor = new Color(myColor.r, myColor.g, myColor.b, fraction);
			L_maskRend.material.color = myColor;
			R_maskRend.material.color = myColor;
			if(fraction >= 1f)
			{
				if(sceneTransition == SceneTransition.exit)
				{
					Debug.Log("Application Exit");
					Application.Quit();
					Debug.Log("Application Exit executed in editor");
					return;
				}
				currentLevel++;
				SceneManager.LoadScene("Level" + currentLevel);
				InitLevel();
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
		Debug.Log("Start Button pressed!");
		///currentLevel = nextLevel;
		InitSceneTransition(SceneTransition.ending);
	}

	public void ExitButton()
	{
		Debug.Log("Exit Button pressed!");
		InitSceneTransition(SceneTransition.exit);
	}

	public void SwitchControllersButton()
	{
		Debug.Log("Switch Controllers Button pressed!");

		OVRInput.Controller temp_controller;
		temp_controller = L_controller;
		L_controller = R_controller;
		R_controller = temp_controller;

		GameObject temp_controller_GO;
		temp_controller_GO = L_controller_GO;
		L_controller_GO = R_controller_GO;
		R_controller_GO = temp_controller_GO;

		if(A_info.activeSelf)
		{
			A_info.SetActive(false);
			B_info.SetActive(true);
		}
		else
		{
			A_info.SetActive(true);
			B_info.SetActive(false);
		}
	}

	public void ResetAllButtons()
	{
		if(currentLevel == 0)
		{
			mm_start_btn.Init();
			mm_exit_btn.Init();
			mm_switch_btn.Init();
		}
	}

	//  TELEPORT PLAYER
	public void InitTeleportPlayer(Vector3 tLoc)
	{
		if(currentLevel != 0 && sceneTransition == SceneTransition.complete)
			player.InitTeleport(tLoc);
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

	// MOVE GAME OBJECTS
	private void MoveGameObjects()
	{
		
	}
}
