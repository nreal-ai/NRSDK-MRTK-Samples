/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.Experimental.NetWork
{
    using System;
    using UnityEngine;

    /// <summary> A network coroutine. </summary>
    internal class NetworkCoroutine : MonoBehaviour
    {
        /// <summary> Event queue for all listeners interested in applicationQuit events. </summary>
        private event Action ApplicationQuitEvent;

        /// <summary> The instance. </summary>
        private static NetworkCoroutine _instance;

        /// <summary> Gets the instance. </summary>
        /// <value> The instance. </value>
        public static NetworkCoroutine Instance
        {
            get
            {
                if (!_instance)
                {
                    GameObject socketClientObj = new GameObject("NetworkCoroutine");
                    _instance = socketClientObj.AddComponent<NetworkCoroutine>();
                    DontDestroyOnLoad(socketClientObj);
                }
                return _instance;
            }
        }

        /// <summary> Sets quit event. </summary>
        /// <param name="func"> The function.</param>
        public void SetQuitEvent(Action func)
        {
            if (ApplicationQuitEvent != null) return;
            ApplicationQuitEvent += func;
        }

        /// <summary> Executes the 'application quit' action. </summary>
        private void OnApplicationQuit()
        {
            ApplicationQuitEvent?.Invoke();
        }
    }
}