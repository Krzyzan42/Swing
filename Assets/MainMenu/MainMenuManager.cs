using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Button startBtn, exitBtn;

    // Start is called before the first frame update
    void Start()
    {
        startBtn.onClick.AddListener(startGame);
        exitBtn.onClick.AddListener(exitGame);
    }

	private void startGame()
	{
		SceneLoader.loadLevel(1);
	}

	private void exitGame()
	{
		Application.Quit();
		EditorApplication.ExitPlaymode();
	}

}
