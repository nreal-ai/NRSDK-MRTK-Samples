/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

using UnityEngine;

namespace NRKernal.Experimental.NRExamples
{
    /// <summary> A switch plane detect. </summary>
    public class SwitchPlaneDetect : MonoBehaviour
    {
        /// <summary> Switch plane detect configuration. </summary>
        /// <param name="flag"> True to flag.</param>
        private void SwitchPlaneDetectConfig(bool flag)
        {
            var config = NRSessionManager.Instance.NRSessionBehaviour.SessionConfig;
            config.PlaneFindingMode = flag ? TrackablePlaneFindingMode.HORIZONTAL : TrackablePlaneFindingMode.DISABLE;
            NRSessionManager.Instance.SetConfiguration(config);
        }

        /// <summary> Enables the plane detect. </summary>
        public void EnablePlaneDetect() { SwitchPlaneDetectConfig(true); }
        /// <summary> Disables the plane detect. </summary>
        public void DisablePlaneDetect() { SwitchPlaneDetectConfig(false); }
    }
}
