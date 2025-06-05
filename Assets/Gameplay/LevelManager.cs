using System;
using System.Collections;
using Events.FlagReached;
using Events.PlayerDeath;
using Gameplay.Misc;
using JetBrains.Annotations;
using UI.MainGame;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class LevelManager : MonoBehaviour
    {
        public enum FinishAction
        {
            ShowVictory,
            LoadCutscene,
            RestartLevel
        }

        public enum MultiplayerMode
        {
            SinglePlayer,
            MultiplayerCooperative,
            MultiplayerCompetitive
        }

        [SerializeField] private string levelId;
        [SerializeField] [CanBeNull] private string nextLevelId;

        [SerializeField] private FinishAction finishAction;
        [SerializeField] private MultiplayerMode multiplayerMode;

        public GameUIManager uiManager;

        private bool _atLeastOnePlayerHasDied;
        [Inject] private FlagReachedEventChannel _flagReachedEventChannel;

        [Inject] private PlayerDeathEventChannel _playerDeathEventChannel;

        private float _startTime;

        private void Start()
        {
            _startTime = Time.time;
        }

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

        private void LevelFinished()
        {
            var milliseconds = (int)((Time.time - _startTime) * 1000f);
            LoadSaveSystem.SetLevelAsCompleted(levelId, nextLevelId, milliseconds, out var isNewRecord);

            switch (finishAction)
            {
                case FinishAction.ShowVictory:
                    uiManager.EnableVictoryMenu(() => SceneLoader.LoadLevel(nextLevelId), isNewRecord);
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
            StartCoroutine(HandlePlayerDeathDelayed(deathData));
        }

        private IEnumerator HandlePlayerDeathDelayed(DeathData deathData)
        {
            yield return new WaitForSeconds(0.5f);
            if (multiplayerMode == MultiplayerMode.MultiplayerCompetitive)
            {
                if (_atLeastOnePlayerHasDied)
                {
                    SceneLoader.ReloadScene();
                    yield break;
                }

                _atLeastOnePlayerHasDied = true;
                yield break;
            }

            SceneLoader.ReloadScene();
        }

        public void HandleFlagReached(FlagReachedData flagReachedData)
        {
            LevelFinished();
        }
    }
}