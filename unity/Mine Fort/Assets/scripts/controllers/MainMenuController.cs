﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MineFort.controllers
{
    public class MainMenuController : MonoBehaviour
    {
        public static MainMenuController Instance { get; protected set; }

        public ModsManager ModsManager { get; private set; }

        public SoundController SoundController { get; private set; }

        public DialogBoxManager DialogBoxManager { get; private set; }

        // Use this for initialization.
        public void OnEnable()
        {
            new PrototypeManager();
            new FunctionsManager();
        }

        public void Start()
        {
            Instance = this;

            new SpriteManager();
            new AudioManager();
            ModsManager = new ModsManager();

            TimeManager.Instance.IsPaused = true;

            // Create a Background.
            GameObject backgroundGO = new GameObject("Background");
            backgroundGO.AddComponent<SpriteRenderer>().sprite = SpriteManager.GetRandomSprite("Background");

            GameObject canvas = GameObject.Find("Canvas");

            // Create a Title.
            GameObject title = (GameObject)Instantiate(Resources.Load("UI/TitleMainMenu"));
            title.transform.SetParent(canvas.transform, false);
            title.SetActive(true);

            // Display Main Menu.
            GameObject mainMenu = (GameObject)Instantiate(Resources.Load("UI/MainMenu"));
            mainMenu.transform.SetParent(canvas.transform, false);
            mainMenu.SetActive(true);

            // Display the Settings Menu
            GameObject settingsMenu = (GameObject)Instantiate(Resources.Load("UI/SettingsMenu/SettingsMenu"));

            if (settingsMenu != null)
            {
                settingsMenu.name = "Settings Menu";
                settingsMenu.transform.SetParent(canvas.transform, false);
                settingsMenu.SetActive(true);
            }

            // Create dialogBoxes.
            GameObject dialogBoxes = new GameObject("Dialog Boxes");
            dialogBoxes.transform.SetParent(canvas.transform, false);
            DialogBoxManager = dialogBoxes.AddComponent<DialogBoxManager>();

            // Instantiate a FPSCounter.
            GameObject menuTop = (GameObject)Instantiate(Resources.Load("UI/MenuTop"));
            menuTop.name = "MenuTop";
            menuTop.transform.SetParent(canvas.transform, false);
            GameObject fpsCounter = menuTop.GetComponentInChildren<PerformanceHUDManager>().gameObject;
            fpsCounter.SetActive(true);

            // Destroy the currency
            Destroy(menuTop.GetComponentInChildren<CurrencyDisplay>().gameObject);

            // Dev Console
            GameObject devConsole = (GameObject)Instantiate(Resources.Load("UI/Console/DevConsole"));

            if (devConsole != null)
            {
                devConsole.name = "DevConsole-Spawned";
                devConsole.transform.SetParent(canvas.transform, false);
                devConsole.transform.SetAsLastSibling();
                devConsole.SetActive(true);
                DeveloperConsole.DevConsole.Close();
            }
        }
    }
}
