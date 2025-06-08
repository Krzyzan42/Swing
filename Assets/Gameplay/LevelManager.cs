using System;
using System.Collections;
using System.Globalization;
using Events.FlagReached;
using Events.PlayerDeath;
using Gameplay.Misc;
using JetBrains.Annotations;
using LM;
using UI.MainGame;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private string levelId;

        [SerializeField] [CanBeNull] private string nextSceneName;

        [SerializeField] private FinishAction finishAction;
        [SerializeField] private MultiplayerMode multiplayerMode;

        public GameUIManager uiManager;

        private bool _atLeastOnePlayerHasDied;
        [Inject] private FlagReachedEventChannel _flagReachedEventChannel;

        [Inject] private PlayerDeathEventChannel _playerDeathEventChannel;
        [Inject] private SoundManager _soundManager;

        private float _startTime;

        private void Start()
        {
            _startTime = Time.time;

            _soundManager.Play("music2");
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
            _soundManager.Play("win");

            var milliseconds = (int)((Time.time - _startTime) * 1000f);
            LoadSaveSystem.SetLevelAsCompleted(levelId, nextSceneName, milliseconds, out var isNewRecord);
            var formattedTime = (Time.time - _startTime).ToString("0.00", CultureInfo.InvariantCulture);

            var username = PlayerPrefs.GetString("username", "Jantar");

            switch (finishAction)
            {
                case FinishAction.ShowVictoryLevelNext:
                    uiManager.EnableVictoryMenu(() => SceneLoader.LoadLevel(nextSceneName), isNewRecord, formattedTime,
                        username, milliseconds);
                    break;
                case FinishAction.ShowVictoryCutsceneNext:
                    uiManager.EnableVictoryMenu(() => SceneLoader.LoadCutscene(nextSceneName), isNewRecord,
                        formattedTime, username, milliseconds);
                    break;
                case FinishAction.RestartLevel:
                    SceneLoader.ReloadScene();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandlePlayerDeath(DeathData deathData)
        {
            StartCoroutine(HandlePlayerDeathDelayed(deathData));
        }

        private IEnumerator HandlePlayerDeathDelayed(DeathData _)
        {
            _soundManager.Play("hit");
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

        private enum MultiplayerMode
        {
            SinglePlayer,
            MultiplayerCooperative,
            MultiplayerCompetitive
        }

        private enum FinishAction
        {
            ShowVictoryLevelNext,
            ShowVictoryCutsceneNext,
            RestartLevel
        }
    }
}