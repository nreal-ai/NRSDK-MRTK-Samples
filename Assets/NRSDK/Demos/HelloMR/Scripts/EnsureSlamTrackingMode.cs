/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

using System.Collections;
using UnityEngine;

namespace NRKernal.NRExamples
{
    /// <summary> An ensure slam tracking mode. </summary>
    public class EnsureSlamTrackingMode : MonoBehaviour
    {
        /// <summary> Type of the tracking. </summary>
        [SerializeField]
        private NRHMDPoseTracker.TrackingType m_TrackingType = NRHMDPoseTracker.TrackingType.Tracking6Dof;

        /// <summary> Starts this object. </summary>
        void Start()
        {
            StartCoroutine(EnsureTrackingType(m_TrackingType));
        }

        private IEnumerator EnsureTrackingType(NRHMDPoseTracker.TrackingType type)
        {
            WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
            if (m_TrackingType == NRHMDPoseTracker.TrackingType.Tracking0Dof && NRSessionManager.Instance.NRHMDPoseTracker.TrackingMode != m_TrackingType)
            {
                while (!NRSessionManager.Instance.NRHMDPoseTracker.ChangeTo0Dof(null))
                {
                    yield return waitForEndOfFrame;
                }
            }
            else if (m_TrackingType == NRHMDPoseTracker.TrackingType.Tracking0DofStable && NRSessionManager.Instance.NRHMDPoseTracker.TrackingMode != m_TrackingType)
            {
                while (!NRSessionManager.Instance.NRHMDPoseTracker.ChangeTo0DofStable(null))
                {
                    yield return waitForEndOfFrame;
                }
            }
            else if (m_TrackingType == NRHMDPoseTracker.TrackingType.Tracking3Dof && NRSessionManager.Instance.NRHMDPoseTracker.TrackingMode != m_TrackingType)
            {
                while (!NRSessionManager.Instance.NRHMDPoseTracker.ChangeTo3Dof(null))
                {
                    yield return waitForEndOfFrame;
                }
            }
            else if (m_TrackingType == NRHMDPoseTracker.TrackingType.Tracking6Dof && NRSessionManager.Instance.NRHMDPoseTracker.TrackingMode != m_TrackingType)
            {
                while (!NRSessionManager.Instance.NRHMDPoseTracker.ChangeTo6Dof(null))
                {
                    yield return waitForEndOfFrame;
                }
            }
        }
    }
}