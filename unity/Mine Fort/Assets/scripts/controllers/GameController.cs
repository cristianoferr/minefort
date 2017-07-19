﻿using Rimworld.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Rimworld.controllers
{
    public class GameController : MonoBehaviour
    {
        // TODO: Should this be also saved with the world data?
        // If so - beginner task!
        public static readonly string GameVersion = "Someone_will_come_up_with_a_proper_naming_scheme_later";

        public static GameController Instance { get; protected set; }

        public KeyboardManager KeyboardManager { get; private set; }

        public SoundController SoundController { get; private set; }

        // If true, a modal dialog box is open, so normal inputs should be ignored.
        public bool IsModal { get; set; }

        public bool IsPaused
        {
            get
            {
                return TimeManager.Instance.IsPaused || IsModal;
            }

            set
            {
                TimeManager.Instance.IsPaused = value;
            }
        }

        // Path to the saves folder.
        public string FileSaveBasePath()
        {
            return System.IO.Path.Combine(Application.persistentDataPath, "Saves");
        }

        // Path to the world generator folder.
        public string GeneratorBasePath()
        {
            return System.IO.Path.Combine(Application.streamingAssetsPath, "WorldGen");
        }

        // Each time a scene is loaded.
        private void Awake()
        {
            EnableDontDestroyOnLoad();

            SoundController = new SoundController();

            // Load Keyboard Mapping.
            KeyboardManager = KeyboardManager.Instance;

            IsModal = false;
            IsPaused = false;

            KeyboardManager.RegisterInputAction("Pause", KeyboardMappedInputType.KeyUp, () => { IsPaused = !IsPaused; });
        }

        // Only on first time a scene is loaded.
        private void Start()
        {
            // Load settings.
            Settings.LoadSettings();

            // Add a gameobject that Localization
            this.gameObject.AddComponent<LocalizationLoader>();
        }

        private void Update()
        {
            TimeManager.Instance.Update(Time.deltaTime);
        }

        // Game Controller will persist between scenes. 
        private void EnableDontDestroyOnLoad()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this.gameObject);
                Destroy(this);
            }

            DontDestroyOnLoad(this);
        }

        private void OnApplicationQuit()
        {
            // Ensure that the audiomanager's resources get released properly on quit. This may only be a problem in the editor.
            AudioManager.Destroy();
        }
    }
}
