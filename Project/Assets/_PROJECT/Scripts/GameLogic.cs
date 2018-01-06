using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
	public GameObject player;
	public GameObject oculusRig;
	public GameObject[] level;
	public int currentLevel;
	public int startLevel;

	//   S T A R T                                                                                                      
	void Start()
	{
		if(currentLevel == 0)
		{
			Debug.Log("Game started!");
			initGame();
		}
		else
		{
			Debug.Log("Level " + currentLevel + " started!");
		}
	}

	private void initGame()
	{
		Debug.Log("Game initiated!");
	}

	public void startButton()
	{
		Debug.Log("Start Button pressed!");
		currentLevel = startLevel;
		SceneManager.LoadScene("level" + currentLevel);
	}
	
	//   U P D A T E                                                                                                    
	void Update()
	{
		
	}
}
