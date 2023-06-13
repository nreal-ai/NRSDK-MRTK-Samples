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
    using UnityEngine;

    /// <summary>
    /// Oprate AR system state and handles the session lifecycle for application layer. </summary>
    [HelpURL("https://developer.nreal.ai/develop/discover/introduction-nrsdk")]
    [ScriptOrder(NativeConstants.NRSESSIONBEHAVIOUR_ORDER)]
    public class NRSessionBehaviour : SingletonBehaviour<NRSessionBehaviour>
    {
        /// <summary> LogVevel of NRSDK should be while release. </summary>
        [Tooltip("LogLevel of NRSDK.")]
		[SerializeField]
        LogLevel LogLevel = LogLevel.Info;
        /// <summary> The SessionConfig of nrsession. </summary>
        [Tooltip("A scriptable object specifying the NRSDK session configuration.")]
		[SerializeField]
        public NRSessionConfig SessionConfig;

        /// <summary>
        /// Base Awake method that sets the Singleton's unique instance. Called by Unity when
        /// initializing a MonoBehaviour. Scripts that extend Singleton should be sure to call
        /// base.Awake() to ensure the static Instance reference is properly created. </summary>
        new void Awake()
        {
#if NRDEBUG
            NRDebugger.logLevel = LogLevel.All;
#elif !UNITY_EDITOR
            NRDebugger.logLevel = Debug.isDebugBuild ? LogLevel.Debug : LogLevel;
#else
            NRDebugger.logLevel = LogLevel;
#endif
            Debug.LogFormat("[SessionBehaviour] Awake:  logLevel={0}", NRDebugger.logLevel);

            base.Awake();
            if (isDirty) return;

            NRDebugger.Info("[SessionBehaviour] Awake: CreateSession");
            NRSessionManager.Instance.CreateSession(this);
        }

        /// <summary> Starts this object. </summary>
        void Start()
        {
            if (isDirty) return;
            NRDebugger.Info("[SessionBehaviour] Start: StartSession");
            NRSessionManager.Instance.StartSession();
        }

        /// <summary> Executes the 'application pause' action. </summary>
        /// <param name="pause"> True to pause.</param>
        private void OnApplicationPause(bool pause)
        {
            if (isDirty) return;
            NRDebugger.Info("[SessionBehaviour] OnApplicationPause: {0}", pause);
            if (pause)
            {
                NRSessionManager.Instance.DisableSession();
            }
            else
            {
                NRSessionManager.Instance.ResumeSession();
            }
        }

        /// <summary> Executes the 'enable' action. </summary>
        void OnEnable()
        {
            if (isDirty) return;
            NRDebugger.Info("[SessionBehaviour] OnEnable: ResumeSession");
            NRSessionManager.Instance.ResumeSession();
        }

        /// <summary> Executes the 'disable' action. </summary>
        void OnDisable()
        {
            if (isDirty) return;
            NRDebugger.Info("[SessionBehaviour] OnDisable: DisableSession");
            NRSessionManager.Instance.DisableSession();
        }

        /// <summary>
        /// Base OnDestroy method that destroys the Singleton's unique instance. Called by Unity when
        /// destroying a MonoBehaviour. Scripts that extend Singleton should be sure to call
        /// base.OnDestroy() to ensure the underlying static Instance reference is properly cleaned up. </summary>
        new void OnDestroy()
        {
            if (isDirty) return;
            base.OnDestroy();
            NRDebugger.Info("[SessionBehaviour] OnDestroy: DestroySession");
            NRSessionManager.Instance.DestroySession();
        }
    }
}
