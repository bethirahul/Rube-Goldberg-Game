using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
	public Player player;
	private ControllerInput controllerInput;
	///public GameObject l_controller_GO;
	public GameObject r_controller_GO;
	public int currentLevel;
	public int startLevel;

	//  Scene Changing
	private enum SceneTransition
	{
		starting,
		complete,
		ending
	};
	private SceneTransition sceneTransition;
	public GameObject l_mask;
	public GameObject r_mask;
	private Renderer l_maskRend;
	private Renderer r_maskRend;
	public float sceneChangingTime;
	private float endTime;
	private float startTime;

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
		l_maskRend = l_mask.GetComponent<Renderer>();
		r_maskRend = r_mask.GetComponent<Renderer>();

		// Player
		player.Init();

		// Scene Transition
		sceneTransition = SceneTransition.starting;
		InitSceneTransition();
	}

	//  SCENE TRANSITION
	private void InitSceneTransition()
	{
		l_mask.SetActive(true);
		r_mask.SetActive(true);
		r_controller_GO.SetActive(false);
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
			///Debug.Log("2. Now the time is " + Time.time);
			///Debug.Log("2. Elapsed time: " + (Time.time - startTime));
			///Debug.Log("2. fraction: " + fraction);
			Color color = l_maskRend.material.color;
			color = new Color(color.r, color.g, color.b, fraction);
			l_maskRend.material.color = color;
			r_maskRend.material.color = color;
			if(fraction <= 0f)
			{
				///Debug.Log("3. Scene Anim Complete");
				sceneTransition = SceneTransition.complete;
				l_mask.SetActive(false);
				r_mask.SetActive(false);
				///l_controller_GO.SetActive(true);
				r_controller_GO.SetActive(true);
			}
		}
		else if(sceneTransition == SceneTransition.ending)
		{
			float fraction = (Time.time - startTime) / sceneChangingTime;
			Color myColor = l_maskRend.material.color;
			myColor = new Color(myColor.r, myColor.g, myColor.b, fraction);
			l_maskRend.material.color = myColor;
			r_maskRend.material.color = myColor;
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
		currentLevel = startLevel;
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

	// MOVE GAME OBJECTS
	private void MoveGameObjects()
	{
		
	}
}
