using System;
using Events.FlagReached;
using Events.PlayerDeath;
using Other;
using UnityEngine;
using Zenject;

public class LevelManager : MonoBehaviour
{
    public enum FinishAction
    {
        LoadNextLevel,
        LoadCutscene,
        RestartLevel
    }

    public enum MultiplayerMode
    {
        SinglePlayer,
        MultiplayerCooperative,
        MultiplayerCompetitive
    }

    [SerializeField] private int levelNumber;

    [SerializeField] private FinishAction finishAction;
    [SerializeField] private MultiplayerMode multiplayerMode;


    private bool _atLeastOnePlayerHasDied;
    [Inject] private FlagReachedEventChannel _flagReachedEventChannel;

    [Inject] private PlayerDeathEventChannel _playerDeathEventChannel;

    private void OnEnable()
    {
        _playerDeathEventChannel.RegisterListener(HandlePlayerDeath);
        _flagReachedEventChannel.RegisterListener(HandleFlagReached);
    }

    private void OnDisable()
    {
        _playerDeathEventChannel.UnregisterListener(HandlePlayerDeath);
        _flagReachedEventChannel.UnregisterListener(HandleFlagReached);
    }

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

    private void HandlePlayerDeath(DeathData deathData)
    {
        if (multiplayerMode == MultiplayerMode.MultiplayerCompetitive)
        {
            if (_atLeastOnePlayerHasDied)
            {
                SceneLoader.ReloadScene();
                return;
            }

            _atLeastOnePlayerHasDied = true;
            return;
        }

        SceneLoader.ReloadScene();
    }

    public void HandleFlagReached(FlagReachedData flagReachedData)
    {
        LevelFinished();
    }
}