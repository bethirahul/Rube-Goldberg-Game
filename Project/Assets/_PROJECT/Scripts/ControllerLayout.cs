using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script stays in every scene, not destroyed. this has information if the controllers are switched
public class ControllerLayout : MonoBehaviour
{
	public enum layoutEnum
	{
		normal,
		reversed
	};
	public layoutEnum layout;

	void Awake()
	{
		DontDestroyOnLoad(gameObject);
		layout = layoutEnum.normal;
	}
}
