﻿using UnityEngine;

namespace SlotExtender.Configuration
{
    public class SEConfig : MonoBehaviour
    {
        public SEConfig Instance { get; private set; }
        
        public void Awake()
        {
            Instance = this;           
            DevConsole.RegisterConsoleCommand(this, "seconfig", false, false);
            Logger.Log("Information: Enter 'seconfig' command for configuration window.");
        }
        
        private void OnConsoleCommand_seconfig(NotificationCenter.Notification n)
        {
            ConfigUI configUI = new ConfigUI();
        }

        public SEConfig ()
        {
            if (Instance == null)
            {
                Instance = FindObjectOfType(typeof(SEConfig)) as SEConfig;

                if (Instance == null)
                {
                    GameObject seconfig_command = new GameObject().AddComponent<SEConfig>().gameObject;
                    seconfig_command.name = "SEConfig";
                    Instance = seconfig_command.GetComponent<SEConfig>();
                }
            }            
        }
    }
}