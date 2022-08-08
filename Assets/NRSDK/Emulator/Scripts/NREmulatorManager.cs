/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/               
* 
*****************************************************************************/

namespace NRKernal
{

#if UNITY_EDITOR
    using UnityEngine;

    /// <summary> Manager for nr emulators. </summary>
    internal class NREmulatorManager : MonoBehaviour
    {
        /// <summary> Gets or sets the instance. </summary>
        /// <value> The instance. </value>
        public static NREmulatorManager Instance { get; set; }

        /// <summary> Identifier for the simulation plane. </summary>
        public static int SIMPlaneID = 0;

        /// <summary> True if inited. </summary>
        public static bool Inited = false;

        /// <summary> The center camera. </summary>
        private Camera centerCam = null;

        /// <summary> Starts this object. </summary>
        private void Start()
        {
            DontDestroyOnLoad(this);
            Instance = this;
        }

        /// <summary> Query if 'worldPos' is in game view. </summary>
        /// <param name="worldPos"> The world position.</param>
        /// <returns> True if in game view, false if not. </returns>
        public bool IsInGameView(Vector3 worldPos)
        {
            if (centerCam == null) centerCam = GameObject.Find("CenterCamera").GetComponent<Camera>();
            Transform camTransform = centerCam.transform;
            Vector2 viewPos = centerCam.WorldToViewportPoint(worldPos);
            Vector3 dir = (worldPos - camTransform.position).normalized;
            float dot = Vector3.Dot(camTransform.forward, dir);
            if (dot > 0 && viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
#endif
}

