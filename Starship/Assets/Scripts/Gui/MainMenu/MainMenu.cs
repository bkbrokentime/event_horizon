﻿using System;
using System.Linq;
using GameStateMachine.States;
using Services.Gui;
using Services.Messenger;
using Session;
using Constructor.Ships;
using Constructor.Satellites;
using Database.Legacy;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Model;
using GameServices.GameManager;
using GameServices.Gui;
using GameServices.Settings;
using Services.IAP;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gui.MainMenu
{
    public class MainMenu : MonoBehaviour
    {
        [Inject] private readonly IInAppPurchasing _inAppPurchasing;
        [Inject] private readonly IGameDataManager _gameDataManager;
        [Inject] private readonly GameSettings _gameSettings;
        [Inject] private readonly IDatabase _database;
        [Inject] private readonly GuiHelper _guiHelper;

        [Inject]
        private void Initialize(
            StartGameSignal.Trigger startGameTrigger,
            StartQuickBattleSignal.Trigger startBattleTrigger,
            StartEndlessBattleSignal.Trigger startEndlessBattleTrigger,
            OpenConstructorSignal.Trigger openConstructorTrigger,
     //       OpenSatelliteConstructorSignal.Trigger openSatelliteConstructorTrigger,
            OpenEhopediaSignal.Trigger openEchopediaTrigger,
            IMessenger messenger,
            ISessionData gameSession,
            IGuiManager guiManager)
        {
            _startGameTrigger = startGameTrigger;
            _startBattleTrigger = startBattleTrigger;
            _startEndlessBattleTrigger = startEndlessBattleTrigger;

            _openConstructorTrigger = openConstructorTrigger;
     //       _openSatelliteConstructorTrigger = openSatelliteConstructorTrigger;
            _openEchopediaTrigger = openEchopediaTrigger;
            _gameSession = gameSession;
            _guiManager = guiManager;

            _inputField.text = _gameSettings.EditorText;
     //       _inputSatelliteField.text = _gameSettings.EditorText;

            messenger.AddListener(EventType.SessionCreated, UpdateButtons);
            messenger.AddListener(EventType.DatabaseLoaded, UpdateButtons);
            UpdateButtons();
        }

        [SerializeField] private Button _startGameButton;
        [SerializeField] private Button _continueGameButton;
        [SerializeField] private Button _constructorButton;
     //   [SerializeField] private Button _constructorSatelliteButton;
        [SerializeField] private InputField _inputField;
     //   [SerializeField] private InputField _inputSatelliteField;

        public void StartGame()
        {
            _startGameTrigger.Fire();
        }
        
        public void StartBattle()
        {
            _guiManager.OpenWindow(Common.WindowNames.SelectDifficultyDialog, OnDialogClosed);
        }
        public void StartEndlessBattle()
        {
            _guiManager.OpenWindow(Common.WindowNames.SelectDifficultyDialog, OnEndlessDialogClosed);
        }

        public void OpenConstructor()
        {
            _gameSettings.EditorText = _inputField.text;

            int shipId;
            ShipBuild build = null;
            if (!int.TryParse(_inputField.text.Replace("*", string.Empty), out shipId))
                UnityEngine.Debug.Log("invalid id: " + _inputField.text);
            else
                build = _database.GetShipBuild(new ItemId<ShipBuild>(shipId));

            if (build == null)
            {
                UnityEngine.Debug.Log("ship not found: " + _inputField.text);
                build = _database.ShipBuildList.FirstOrDefault();
            }

            if (build == null)
                return;

            _openConstructorTrigger.Fire(new EditorModeShip(build, _database));
        }
        /*
        public void OpenSatelliteConstructor()
        {
            _gameSettings.EditorText = _inputSatelliteField.text;

            int shipId;
            SatelliteBuild build = null;
            if (!int.TryParse(_inputSatelliteField.text.Replace("*", string.Empty), out shipId))
                UnityEngine.Debug.Log("invalid id: " + _inputSatelliteField.text);
            else
                build = _database.GetSatelliteBuild(new ItemId<SatelliteBuild>(shipId));

            if (build == null)
            {
                UnityEngine.Debug.Log("Satellite not found: " + _inputSatelliteField.text);
                build = _database.SatelliteBuildList.FirstOrDefault();
            }

            if (build == null)
                return;

            _openSatelliteConstructorTrigger.Fire(new EditorModeSatellite(build, _database));
        }
        */
        public void ShowPrivacyPolicy()
        {
            Application.OpenURL("https://zipagames.com/policy.html");
        }
        
        public void JoinDiscord()
        {
            Application.OpenURL("https://discordapp.com/invite/yFFvF7m");
        }
        
        public void JoinVk()
        {
            Application.OpenURL("https://vk.com/official_event_horizon_group");
        }

        public void Echopedia()
        {
            _openEchopediaTrigger.Fire();
        }

        public void Exit()
        {
            Application.Quit();
        }

        public void RestorePurchases()
        {
            _gameDataManager.RestorePurchases();
        }

        private void OnDialogClosed(WindowExitCode result)
        {
            _gameSettings.EditorText = _inputField.text;

            switch (result)
            {
                case WindowExitCode.Option1:
                    _startBattleTrigger.Fire(true, _inputField.text);
                    break;
                case WindowExitCode.Option2:
                    _startBattleTrigger.Fire(false, _inputField.text);
                    break;
            }
        }
        private void OnEndlessDialogClosed(WindowExitCode result)
        {
            _gameSettings.EditorText = _inputField.text;

            switch (result)
            {
                case WindowExitCode.Option1:
                    _startEndlessBattleTrigger.Fire(true, _inputField.text);
                    break;
                case WindowExitCode.Option2:
                    _startEndlessBattleTrigger.Fire(false, _inputField.text);
                    break;
            }
        }

        private void UpdateButtons()
        {
            var gameExists = _gameSession.IsGameStarted;
            UnityEngine.Debug.Log("MainMenu.UpdateButtons - " + gameExists);

            _startGameButton.gameObject.SetActive(!gameExists);
            _continueGameButton.gameObject.SetActive(gameExists);
            _constructorButton.gameObject.SetActive(_database.IsEditable);
      //      _constructorSatelliteButton.gameObject.SetActive(_database.IsEditable);
        }

        private StartGameSignal.Trigger _startGameTrigger;
        private StartQuickBattleSignal.Trigger _startBattleTrigger;
        private StartEndlessBattleSignal.Trigger _startEndlessBattleTrigger;
        private OpenConstructorSignal.Trigger _openConstructorTrigger;
        //private OpenSatelliteConstructorSignal.Trigger _openSatelliteConstructorTrigger;
        private OpenEhopediaSignal.Trigger _openEchopediaTrigger;
        private ISessionData _gameSession;
        private IGuiManager _guiManager;
    }
}
