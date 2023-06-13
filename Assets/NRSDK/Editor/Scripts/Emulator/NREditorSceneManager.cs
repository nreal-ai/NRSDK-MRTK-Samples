/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.NREditor
{
    using UnityEngine;
    using UnityEditor;
    using System;

    /// <summary> Manager for nr editor scenes. </summary>
    public class NREditorSceneManager
    {
        /// <summary> True if scene initialized. </summary>
        private bool m_SceneInitialized;
        /// <summary> True to unload unused assets. </summary>
        private bool m_UnloadUnusedAssets;
        /// <summary> True to apply appearance. </summary>
        private bool m_ApplyAppearance;
        /// <summary> True to apply properties. </summary>
        private bool m_ApplyProperties;
        /// <summary> The instance. </summary>
        private static NREditorSceneManager m_Instance;
        /// <summary> The update callback. </summary>
        private EditorApplication.CallbackFunction m_UpdateCallback;
        /// <summary> Gets the instance. </summary>
        /// <value> The instance. </value>
        public static NREditorSceneManager Instance
        {
            get
            {
                if (NREditorSceneManager.m_Instance == null)
                {
                    Type typeFromHandle = typeof(NREditorSceneManager);
                    lock (typeFromHandle)
                    {
                        if (NREditorSceneManager.m_Instance == null)
                        {
                            NREditorSceneManager.m_Instance = new NREditorSceneManager();
                        }
                    }
                }
                return NREditorSceneManager.m_Instance;
            }
        }

        /// <summary> Gets a value indicating whether the scene initialized. </summary>
        /// <value> True if scene initialized, false if not. </value>
        public bool SceneInitialized { get { return m_SceneInitialized; } }

        /// <summary>
        /// Constructor that prevents a default instance of this class from being created. </summary>
        private NREditorSceneManager()
        {
            //return;
            m_UpdateCallback = new EditorApplication.CallbackFunction(EditorUpdate);
            if (EditorApplication.update == null || !EditorApplication.update.Equals(m_UpdateCallback))
            {
                EditorApplication.update = (EditorApplication.CallbackFunction)System.Delegate.Combine(EditorApplication.update, m_UpdateCallback);
            }

            m_SceneInitialized = false;
        }

        /// <summary> Initializes the scene. </summary>
        public void InitScene()
        {
            m_SceneInitialized = true;
        }

        /// <summary> Editor update. </summary>
        public void EditorUpdate()
        {
            NRTrackableBehaviour[] trackables = GameObject.FindObjectsOfType<NRTrackableBehaviour>();
            if (m_ApplyAppearance)
            {
                UpdateTrackableAppearance(trackables);
                m_ApplyAppearance = false;
            }
            if (m_ApplyProperties)
            {
                UpdateTrackableProperties(trackables);
                m_ApplyProperties = false;
            }
            if (m_UnloadUnusedAssets)
            {
                Resources.UnloadUnusedAssets();
                m_UnloadUnusedAssets = false;
            }
        }

        /// <summary> Updates the trackable appearance described by trackables. </summary>
        /// <param name="trackables"> The trackables.</param>
        private void UpdateTrackableAppearance(NRTrackableBehaviour[] trackables)
        {
            if (!Application.isPlaying)
            {
                for (int i = 0; i < trackables.Length; i++)
                {
                    NRTrackableBehaviour trackableBehaviour = trackables[i];
                    NRTrackableAccessor trackableAccessor = NRAccessorFactory.Create(trackableBehaviour);
                    if (trackableAccessor != null)
                    {
                        trackableAccessor.ApplyDataAppearance();
                    }
                }
            }
        }

        /// <summary> Updates the trackable properties described by trackables. </summary>
        /// <param name="trackables"> The trackables.</param>
        private void UpdateTrackableProperties(NRTrackableBehaviour[] trackables)
        {
            for (int i = 0; i < trackables.Length; i++)
            {
                NRTrackableBehaviour trackableBehaviour = trackables[i];
                NRTrackableAccessor trackableAccessor = NRAccessorFactory.Create(trackableBehaviour);
                if (trackableAccessor != null)
                {
                    trackableAccessor.ApplyDataProperties();
                }
            }
        }

        /// <summary> Unload unused assets. </summary>
        public void UnloadUnusedAssets()
        {
            m_UnloadUnusedAssets = true;
        }
    }
}