/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

using UnityEngine;
using UnityEngine.SceneManagement;

namespace NRKernal.NRExamples
{
    /// <summary> The examples hub. </summary>
    public class ExamplesHub : SingletonBehaviour<ExamplesHub>
    {
        /// <summary> The scenes. </summary>
        private string[] m_Scenes = new string[] {
            "HelloMR",
            "ImageTracking",
            "Input-ControllerInfo",
            "Input-Interaction",
            "RGBCamera",
            "RGBCamera-Capture",
            "RGBCamera-Record"
        };
        /// <summary> The current index. </summary>
        private int m_CurrentIndex = 0;
        /// <summary> Gets or sets the current index. </summary>
        /// <value> The current index. </value>
        public int CurrentIndex
        {
            get
            {
                return m_CurrentIndex;
            }
            private set
            {
                m_CurrentIndex = value;
                if (m_CurrentIndex < 0 || m_CurrentIndex >= m_Scenes.Length)
                {
                    m_CurrentIndex = 0;
                }
            }
        }
        /// <summary> True if is lock, false if not. </summary>
        private bool m_IsLock = false;

        /// <summary> Updates this object. </summary>
        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                LoadNextScene();
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                LoadLastScene();
            }

#endif
            if (NRInput.GetTouch().x > 0.8f)
            {
                LoadNextScene();
            }
            if (NRInput.GetTouch().x < -0.8f)
            {
                LoadLastScene();
            }
        }

        /// <summary> Loads next scene. </summary>
        public void LoadNextScene()
        {
            if (m_IsLock)
            {
                return;
            }

            m_IsLock = true;
            CurrentIndex++;
            if (CanSceneLoaded(m_Scenes[CurrentIndex]))
            {
                SceneManager.LoadScene(m_Scenes[CurrentIndex]);
            }
            Invoke("Unlock", 1f);
        }

        /// <summary> Loads last scene. </summary>
        public void LoadLastScene()
        {
            if (m_IsLock)
            {
                return;
            }

            m_IsLock = true;
            CurrentIndex--;
            if (CanSceneLoaded(m_Scenes[CurrentIndex]))
            {
                SceneManager.LoadScene(m_Scenes[CurrentIndex]);
            }
            Invoke("Unlock", 1f);
        }

        /// <summary> Unlocks this object. </summary>
        private void Unlock()
        {
            m_IsLock = false;
        }

        /// <summary> Determine if we can scene loaded. </summary>
        /// <param name="name"> The name.</param>
        /// <returns> True if we can scene loaded, false if not. </returns>
        private bool CanSceneLoaded(string name)
        {
            return (SceneUtility.GetBuildIndexByScenePath(name) != -1) &&
                !SceneManager.GetActiveScene().name.Equals(name);
        }
    }
}
