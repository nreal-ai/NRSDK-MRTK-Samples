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
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary> A hand pointer. </summary>
    public class NRHandPointer : MonoBehaviour
    {
        /// <summary> The handEnum. </summary>
        public HandEnum handEnum;
        /// <summary> The raycaster. </summary>
        public NRPointerRaycaster raycaster;

        /// <summary> The default hand enum. </summary>
        private ControllerHandEnum m_controllerHandEnum;
        /// <summary> True if is enabled, false if not. </summary>
        private bool m_PointerEnabled;

        /// <summary> Awakes this object. </summary>
        private void Awake()
        {
            m_controllerHandEnum = handEnum == HandEnum.RightHand ? ControllerHandEnum.Right : ControllerHandEnum.Left;
            raycaster.RelatedHand = m_controllerHandEnum;
        }

        /// <summary> Executes the 'enable' action. </summary>
        private void OnEnable()
        {
            NRInput.OnControllerRecentering += OnRecentering;
            NRInput.OnControllerStatesUpdated += OnControllerStatesUpdated;
        }

        /// <summary> Executes the 'disable' action. </summary>
        private void OnDisable()
        {
            NRInput.OnControllerRecentering -= OnRecentering;
            NRInput.OnControllerStatesUpdated -= OnControllerStatesUpdated;
        }

        /// <summary> Executes the 'controller states updated' action. </summary>
        private void OnControllerStatesUpdated()
        {
            UpdateTracker();
        }

        /// <summary> Updates the tracker. </summary>
        private void UpdateTracker()
        {
            var handState = NRInput.Hands.GetHandState(handEnum);
            m_PointerEnabled = NRInput.RaycastersActive && NRInput.RaycastMode == RaycastModeEnum.Laser && NRInput.Hands.IsRunning && handState.pointerPoseValid;
            raycaster.gameObject.SetActive(m_PointerEnabled);
            if (m_PointerEnabled)
            {
                TrackPose();
            }
        }

        /// <summary> Track pose. </summary>
        private void TrackPose()
        {
            transform.position = NRInput.GetPosition(m_controllerHandEnum);
            transform.localRotation = NRInput.GetRotation(m_controllerHandEnum);
        }

        /// <summary> Executes the 'recentering' action. </summary>
        private void OnRecentering() { }
    }

}