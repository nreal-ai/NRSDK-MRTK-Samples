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

    
    /// <summary> A gaze tracker. </summary>
    public class GazeTracker : MonoBehaviour
    {
        /// <summary> The raycaster. </summary>
        [SerializeField]
        private NRPointerRaycaster m_Raycaster;
        /// <summary> True if is enabled, false if not. </summary>
        private bool m_IsEnabled;

        /// <summary> Gets the camera center. </summary>
        /// <value> The camera center. </value>
        private Transform CameraCenter
        {
            get
            {
                return NRInput.CameraCenter;
            }
        }

        /// <summary> Starts this object. </summary>
        private void Start()
        {
            OnControllerStatesUpdated();
        }

        /// <summary> Executes the 'enable' action. </summary>
        private void OnEnable()
        {
            NRInput.OnControllerStatesUpdated += OnControllerStatesUpdated;
        }

        /// <summary> Executes the 'disable' action. </summary>
        private void OnDisable()
        {
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
            if (CameraCenter == null)
                return;
            m_IsEnabled = (NRInput.RaycastersActive && NRInput.RaycastMode == RaycastModeEnum.Gaze);
            m_Raycaster.gameObject.SetActive(m_IsEnabled);
            if (m_IsEnabled)
            {
                transform.position = CameraCenter.position;
                transform.rotation = CameraCenter.rotation;
            }
        }
    }
    
}
