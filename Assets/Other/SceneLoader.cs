using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

enum Scene
{

}

public static class SceneLoader
{
    public static void loadLevel(int level)
	{
		SceneManager.LoadScene(level.ToString());
	}

	public static void loadMainMenu()
	{
		SceneManager.LoadScene("MainMenu");
	}
}
