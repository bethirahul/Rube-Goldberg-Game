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

	//  Controllers
	private ControllerInput controllerInput;
	public OVRInput.Controller L_controller;
	public OVRInput.Controller R_controller;
	public GameObject L_controller_GO;
	public GameObject R_controller_GO;
	public LayerMask rayMask;
	public float rayRange;
	public float throwForce;

	//  Level
	public int currentLevel;
	public int nextLevel;

	//  Scene Changing
	private enum SceneTransition
	{
		starting,
		complete,
		ending
	};
	private SceneTransition sceneTransition;
	public GameObject L_mask;
	public GameObject R_mask;
	private Renderer L_maskRend;
	private Renderer R_maskRend;
	public float sceneChangingTime;
	private float endTime;
	private float startTime;
	#endregion

	//   S T A R T                                                                                                      
	void Start()
	{
		InitForAllLevels();
		InitFromLevel1();
	}

	//  INIT for ALL LEVELS
	private void InitForAllLevels()
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
		sceneTransition = SceneTransition.starting;
		InitSceneTransition();
	}

	//  SCENE TRANSITION
	private void InitSceneTransition()
	{
		L_mask.SetActive(true);
		R_mask.SetActive(true);
		L_controller_GO.SetActive(false);
		R_controller_GO.SetActive(false);
		startTime = Time.time;
		endTime = startTime + sceneChangingTime;
	}

	//  INIT from LEVEL 1
	private void InitFromLevel1()
	{
		
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
				///Debug.Log("3. Scene Anim Complete");
				sceneTransition = SceneTransition.complete;
				L_mask.SetActive(false);
				R_mask.SetActive(false);
				L_controller_GO.SetActive(true);
				R_controller_GO.SetActive(true);
			}
		}
		else if(sceneTransition == SceneTransition.ending)
		{
			float fraction = (Time.time - startTime) / sceneChangingTime;
			Color myColor = L_maskRend.material.color;
			myColor = new Color(myColor.r, myColor.g, myColor.b, fraction);
			L_maskRend.material.color = myColor;
			R_maskRend.material.color = myColor;
			if(fraction >= 1f)
				SceneManager.LoadScene("Level" + currentLevel);
		}
	}

	//  INPUT
	private void CheckInput()
	{
		if(sceneTransition == SceneTransition.complete)
			controllerInput.CheckInput();
	}

	//  BUTTONS
	public void startButton()
	{
		Debug.Log("Start Button pressed!");
		currentLevel = nextLevel;
		sceneTransition = SceneTransition.ending;
		InitSceneTransition();
		///SceneManager.LoadScene("level" + currentLevel);
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
