using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
	public GameObject player;
	private ControllerInput controllerInput;
	public int currentLevel;
	public int startLevel;

	//  Scene Changing
	private enum sceneTransitionEnum
	{
		starting,
		complete,
		ending
	};
	private sceneTransitionEnum sceneTransition;
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
		Debug.Log("GameLogic started");
		controllerInput = player.GetComponent<ControllerInput>();
		l_maskRend = l_mask.GetComponent<Renderer>();
		r_maskRend = r_mask.GetComponent<Renderer>();
		sceneTransition = sceneTransitionEnum.starting;
		if(currentLevel == 0)
		{
			Debug.Log("Game started!");
			initGame();
		}
		else
		{
			Debug.Log("Level " + currentLevel + " started!");
		}
		initSceneChange();
	}

	private void initGame()
	{
		Debug.Log("Game initiated!");
	}

	public void startButton()
	{
		Debug.Log("Start Button pressed!");
		currentLevel = startLevel;
		sceneTransition = sceneTransitionEnum.ending;
		initSceneChange();
		///SceneManager.LoadScene("level" + currentLevel);
	}

	private void initSceneChange()
	{
		Debug.Log("Changing Scene Anim Started");
		l_mask.SetActive(true);
		r_mask.SetActive(true);
		startTime = Time.time;
		endTime = startTime + sceneChangingTime;
		Debug.Log("1. Current Time is " + startTime);
		Debug.Log("1. End Time is " + endTime);
	}
	
	//   U P D A T E                                                                                                    
	void Update()
	{
		if(sceneTransition == sceneTransitionEnum.starting)
		{
			float fraction = 1f - ((Time.time - startTime) / sceneChangingTime);
			Debug.Log("2. Now the time is " + Time.time);
			Debug.Log("2. Elapsed time: " + (Time.time - startTime));
			Debug.Log("2. fraction: " + fraction);
			Color color = l_maskRend.material.color;
			color = new Color(color.r, color.g, color.b, fraction);
			l_maskRend.material.color = color;
			r_maskRend.material.color = color;
			if(fraction <= 0f)
			{
				Debug.Log("3. Scene Anim Complete");
				sceneTransition = sceneTransitionEnum.complete;
				l_mask.SetActive(false);
				r_mask.SetActive(false);
			}
		}
		else if(sceneTransition == sceneTransitionEnum.ending)
		{
			float fraction = (Time.time - startTime) / sceneChangingTime;
			Color color = l_maskRend.material.color;
			color = new Color(color.r, color.g, color.b, fraction);
			l_maskRend.material.color = color;
			r_maskRend.material.color = color;
			if(fraction >= 1f)
			{
				SceneManager.LoadScene("level" + currentLevel);
			}
		}
		else
		{
			controllerInput.checkInput();
		}
	}
}
