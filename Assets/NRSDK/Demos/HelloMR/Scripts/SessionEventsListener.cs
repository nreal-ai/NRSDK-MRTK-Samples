/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.NRExamples
{
    using UnityEngine;

    public class SessionEventsListener : MonoBehaviour
    {
        void Awake()
        {
            // Tracking mode changed event.
            NRSessionManager.OnChangeTrackingMode += OnChangeTrackingMode;
            // Glasses disconnected event.
            NRSessionManager.OnGlassesDisconnect += OnGlassesDisconnect;
            // Glasses state changed event:PutOn,PutOff.
            NRSessionManager.OnGlassesStateChanged += OnGlassesStateChanged;
            // Tracking state lost event.
            NRSessionManager.OnHMDLostTracking += OnHMDLostTracking;
            // Tracking state ready event.
            NRSessionManager.OnHMDPoseReady += OnHMDPoseReady;
            // Session kernal error event, such as NRRGBCameraDeviceNotFindError, NRPermissionDenyError, NRUnSupportedHandtrackingCalculationError.
            NRSessionManager.OnKernalError += OnKernalError;
        }

        /// <summary>
        /// Session kernal error event.
        /// </summary>
        /// <param name="exception">NRRGBCameraDeviceNotFindError, NRPermissionDenyError, NRUnSupportedHandtrackingCalculationError</param>
        private void OnKernalError(NRKernalError exception)
        {
            NRDebugger.Info("[SessionEventsListener] OnKernalError.");
        }

        private void OnHMDPoseReady()
        {
            NRDebugger.Info("[SessionEventsListener] OnHMDPoseReady.");
        }

        private void OnHMDLostTracking()
        {
            NRDebugger.Info("[SessionEventsListener] OnHMDLostTracking.");
        }

        private void OnGlassesStateChanged(NRDevice.GlassesEventType eventtype)
        {
            NRDebugger.Info("[SessionEventsListener] OnGlassesStateChanged:" + eventtype.ToString());
        }

        private void OnGlassesDisconnect(GlassesDisconnectReason reason)
        {
            NRDebugger.Info("[SessionEventsListener] OnGlassesDisconnect:" + reason.ToString());
        }

        private void OnChangeTrackingMode(NRHMDPoseTracker.TrackingType origin, NRHMDPoseTracker.TrackingType target)
        {
            NRDebugger.Info("[SessionEventsListener] OnChangeTrackingMode, from:{0} to:{1}", origin, target);
        }
    }
}