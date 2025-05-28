using System;
using Other;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public enum FinishAction
    {
        LoadNextLevel,
        LoadCutscene,
        RestartLevel
    }

    [SerializeField] private int levelNumber;

    [SerializeField] private FinishAction finishAction;

    public void LevelFinished()
    {
        switch (finishAction)
        {
            case FinishAction.LoadNextLevel:
                SceneLoader.LoadLevel(levelNumber + 1);
                break;
            case FinishAction.RestartLevel:
                SceneLoader.ReloadScene();
                break;
            case FinishAction.LoadCutscene:
                throw new NotImplementedException();
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}