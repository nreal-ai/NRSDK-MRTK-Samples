/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

using System.Collections.Generic;
using UnityEngine;

namespace NRKernal.NRExamples
{
    /// <summary> A tracking infomation tips. </summary>
    [HelpURL("https://developer.nreal.ai/develop/discover/introduction-nrsdk")]
    public class TrackingInfomationTips : SingletonBehaviour<TrackingInfomationTips>
    {
        /// <summary> Dictionary of tips. </summary>
        private Dictionary<TipType, GameObject> m_TipsDict = new Dictionary<TipType, GameObject>();
        /// <summary> Values that represent tip types. </summary>
        public enum TipType
        {
            /// <summary> An enum constant representing the un initialized option. </summary>
            UnInitialized,
            /// <summary> An enum constant representing the lost tracking option. </summary>
            LostTracking,
            /// <summary> An enum constant representing the none option. </summary>
            None
        }
        /// <summary> The tips layer. </summary>
        [Tooltip("Camera would only show the layer of TipsLayer when lost tracking.\n" +
            "Select the layer which you want to show when lost tracking.")]
        [SerializeField]
        private LayerMask m_TipsLayer;
        /// <summary> The current tip. </summary>
        private GameObject m_CurrentTip;
        /// <summary> The center camera. </summary>
        private Camera centerCamera;

        /// <summary> The origin layer l camera. </summary>
        private int originLayerLCam;
        /// <summary> The origin layer camera. </summary>
        private int originLayerRCam;

        /// <summary> Starts this object. </summary>
        void Start()
        {
            originLayerLCam = NRSessionManager.Instance.NRHMDPoseTracker.leftCamera.cullingMask;
            originLayerRCam = NRSessionManager.Instance.NRHMDPoseTracker.rightCamera.cullingMask;
            centerCamera = NRSessionManager.Instance.NRHMDPoseTracker.centerCamera;
            ShowTips(TipType.UnInitialized);
        }

        /// <summary> Executes the 'enable' action. </summary>
        private void OnEnable()
        {
            NRHMDPoseTracker.OnHMDLostTracking += OnHMDLostTracking;
            NRHMDPoseTracker.OnHMDPoseReady += OnHMDPoseReady;
        }

        /// <summary> Executes the 'disable' action. </summary>
        private void OnDisable()
        {
            NRHMDPoseTracker.OnHMDLostTracking -= OnHMDLostTracking;
            NRHMDPoseTracker.OnHMDPoseReady -= OnHMDPoseReady;
        }

        /// <summary> Executes the 'hmd pose ready' action. </summary>
        private void OnHMDPoseReady()
        {
            NRDebugger.Info("[NRHMDPoseTracker] OnHMDPoseReady");
            ShowTips(TipType.None);
        }

        /// <summary> Executes the 'hmd lost tracking' action. </summary>
        private void OnHMDLostTracking()
        {
            NRDebugger.Info("[NRHMDPoseTracker] OnHMDLostTracking:" + NRFrame.LostTrackingReason);
            ShowTips(TipType.LostTracking);
        }

        /// <summary> Shows the tips. </summary>
        /// <param name="type"> The type.</param>
        public void ShowTips(TipType type)
        {
            switch (type)
            {
                case TipType.UnInitialized:
                case TipType.LostTracking:
                    GameObject go;
                    m_TipsDict.TryGetValue(type, out go);
                    int layer = LayerMaskToLayer(m_TipsLayer);
                    if (go == null)
                    {
                        go = Instantiate(Resources.Load(type.ToString() + "Tip"), centerCamera.transform) as GameObject;
                        m_TipsDict.Add(type, go);
                        go.layer = layer;
                        foreach (Transform child in go.transform)
                        {
                            child.gameObject.layer = layer;
                        }
                    }
                    if (go != m_CurrentTip)
                    {
                        m_CurrentTip?.SetActive(false);
                        go.SetActive(true);
                        m_CurrentTip = go;
                    }
                    NRSessionManager.Instance.NRHMDPoseTracker.leftCamera.cullingMask = 1 << layer;
                    NRSessionManager.Instance.NRHMDPoseTracker.rightCamera.cullingMask = 1 << layer;
                    break;
                case TipType.None:
                    if (m_CurrentTip != null)
                    {
                        m_CurrentTip.SetActive(false);
                    }
                    m_CurrentTip = null;
                    NRSessionManager.Instance.NRHMDPoseTracker.leftCamera.cullingMask = originLayerLCam;
                    NRSessionManager.Instance.NRHMDPoseTracker.rightCamera.cullingMask = originLayerRCam;
                    break;
                default:
                    break;
            }
        }

        /// <summary> Layer mask to layer. </summary>
        /// <param name="layerMask"> .</param>
        /// <returns> The last layer of the layer mask. </returns>
        public static int LayerMaskToLayer(LayerMask layerMask)
        {
            int layerNumber = 0;
            int layer = layerMask.value;
            while (layer > 0)
            {
                layer = layer >> 1;
                layerNumber++;
            }
            return layerNumber - 1;
        }

        /// <summary>
        /// Base OnDestroy method that destroys the Singleton's unique instance. Called by Unity when
        /// destroying a MonoBehaviour. Scripts that extend Singleton should be sure to call
        /// base.OnDestroy() to ensure the underlying static Instance reference is properly cleaned up. </summary>
        new void OnDestroy()
        {
            if (isDirty) return;
            if (m_TipsDict != null)
            {
                foreach (var item in m_TipsDict)
                {
                    if (item.Value != null)
                    {
                        GameObject.Destroy(item.Value);
                    }
                }
            }
        }
    }
}