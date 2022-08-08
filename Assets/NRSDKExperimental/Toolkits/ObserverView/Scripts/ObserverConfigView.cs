/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.Experimental.StreammingCast
{
    using System.IO;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.Events;
    using NRKernal.NRExamples;

    /// <summary> A configuration view. </summary>
    public class ObserverConfigView : MonoBehaviour
    {
        /// <summary> Back, called when the configuration view. </summary>
        /// <param name="config"> The configuration.</param>
        public delegate void ConfigViewCallBack(ObserverViewConfig config);
        /// <summary> Event queue for all listeners interested in OnConfigrationChanged events. </summary>
        public event ConfigViewCallBack OnConfigrationChanged;
        /// <summary> The IP field. </summary>
        public InputField m_IPField;
        /// <summary> The use debug. </summary>
        public Toggle m_UseDebug;
        public NRInteractiveItem m_CloseBtn;
        public Transform m_Root;

        /// <summary> The current configuration. </summary>
        private ObserverViewConfig currentConfig;
        private const string DefaultServerIP = "192.168.0.100";
        private const float CLICK_INTERVAL = 0.2f;
        private int CLICK_TRIGGER_TIMES = 3;
        private float lastClickTime = 0f;
        private int clickTimes = 0;

        /// <summary> Gets the full pathname of the configuration file. </summary>
        /// <value> The full pathname of the configuration file. </value>
        public static string ConfigPath
        {
            get
            {
                return Path.Combine(Application.persistentDataPath, "ObserverViewConfig.json");
            }
        }

        void Start()
        {
            m_IPField.onValueChanged.AddListener((value) =>
            {
                if (!value.Equals(currentConfig.serverIP))
                {
                    currentConfig.serverIP = value;
                    UpdateConfig();
                }

            });

            m_UseDebug.onValueChanged.AddListener((value) =>
            {
                if (value != currentConfig.useDebugUI)
                {
                    currentConfig.useDebugUI = value;
                    UpdateConfig();
                }
            });

            m_CloseBtn.onPointerClick.AddListener(new UnityAction(() =>
            {
                SwitchView(false);
            }));

            NRInput.AddClickListener(ControllerHandEnum.Left, ControllerButton.HOME, OnHomeButtonClick);
            NRInput.AddClickListener(ControllerHandEnum.Right, ControllerButton.HOME, OnHomeButtonClick);

            this.LoadConfig();

            SwitchView(false);
        }

        private void OnHomeButtonClick()
        {
            if (clickTimes == 0)
            {
                clickTimes++;
                lastClickTime = Time.realtimeSinceStartup;
            }
            else if (Time.realtimeSinceStartup - lastClickTime < CLICK_INTERVAL)
            {
                clickTimes++;
                lastClickTime = Time.realtimeSinceStartup;
            }
            else
            {
                clickTimes = 0;
            }

            if (clickTimes == CLICK_TRIGGER_TIMES)
            {
                SwitchView(!m_Root.gameObject.activeInHierarchy);
            }
        }

        private void SwitchView(bool flag)
        {
            m_Root.gameObject.SetActive(flag);
        }

        /// <summary> Loads the configuration. </summary>
        private void LoadConfig()
        {
            if (!File.Exists(ConfigPath))
            {
                NRDebugger.Info("[ConfigView] File is not exist :" + ConfigPath);
                currentConfig = new ObserverViewConfig(DefaultServerIP, Vector3.zero);
            }
            else
            {
                NRDebugger.Info("[ConfigView] Load config from:" + ConfigPath);
                currentConfig = JsonUtility.FromJson<ObserverViewConfig>(File.ReadAllText(ConfigPath));
            }
            m_IPField.text = currentConfig.serverIP;
            m_UseDebug.isOn = currentConfig.useDebugUI;
            OnConfigrationChanged?.Invoke(currentConfig);
        }

        /// <summary> Config will Works at next run time. </summary>
        private void UpdateConfig()
        {
            var json = JsonUtility.ToJson(currentConfig);
            File.WriteAllText(ConfigPath, json);
            NRDebugger.Info("[ConfigView] Save config :" + json);
            OnConfigrationChanged?.Invoke(currentConfig);
        }
    }
}
