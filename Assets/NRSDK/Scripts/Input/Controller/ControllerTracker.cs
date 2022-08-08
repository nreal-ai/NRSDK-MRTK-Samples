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
    
    /// <summary> A controller tracker. </summary>
    public class ControllerTracker : MonoBehaviour
    {
        /// <summary> The default hand enum. </summary>
        public ControllerHandEnum defaultHandEnum;
        /// <summary> The raycaster. </summary>
        public NRPointerRaycaster raycaster;
        /// <summary> The model anchor. </summary>
        public Transform modelAnchor;

        /// <summary> True if is enabled, false if not. </summary>
        private bool m_IsEnabled;
        /// <summary> True if is 6dof, false if not. </summary>
        private bool m_Is6dof;
        /// <summary> The default local offset. </summary>
        private Vector3 m_DefaultLocalOffset;
        /// <summary> Cache world matrix. </summary>
        private Matrix4x4 m_CachedWorldMatrix = Matrix4x4.identity;

        /// <summary> Gets the camera center. </summary>
        /// <value> The camera center. </value>
        private Transform CameraCenter
        {
            get
            {
                return NRInput.CameraCenter;
            }
        }

        /// <summary> Awakes this object. </summary>
        private void Awake()
        {
            m_DefaultLocalOffset = transform.localPosition;
            raycaster.RelatedHand = defaultHandEnum;
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

        private void Start()
        {            
            m_Is6dof = NRInput.GetControllerAvailableFeature(ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_POSITION)
                && NRInput.GetControllerAvailableFeature(ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_ROTATION);
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
            m_IsEnabled = NRInput.CheckControllerAvailable(defaultHandEnum) && !NRInput.Hands.IsRunning;
            raycaster.gameObject.SetActive(m_IsEnabled && NRInput.RaycastersActive && NRInput.RaycastMode == RaycastModeEnum.Laser);
            modelAnchor.gameObject.SetActive(m_IsEnabled);
            if (m_IsEnabled)
            {
                TrackPose();
            }
        }

        /// <summary> Track pose. </summary>
        private void TrackPose()
        {
            Pose poseInAPIWorld = new Pose(NRInput.GetPosition(defaultHandEnum), NRInput.GetRotation(defaultHandEnum));
            Pose pose = ApplyWorldMatrix(poseInAPIWorld);
            transform.position = m_Is6dof ? pose.position : CameraCenter.TransformPoint(m_DefaultLocalOffset);
            transform.rotation = pose.rotation;
        }

        /// <summary> Apply world transform. </summary>
        private Pose ApplyWorldMatrix(Pose pose)
        {
            var objectMatrix = ConversionUtility.GetTMatrix(pose.position, pose.rotation);
            var object_in_world = m_CachedWorldMatrix * objectMatrix;
            return new Pose(ConversionUtility.GetPositionFromTMatrix(object_in_world),
                ConversionUtility.GetRotationFromTMatrix(object_in_world));
        }

        /// <summary> Executes the 'recentering' action. </summary>
        private void OnRecentering()
        {
            Plane horizontal_plane = new Plane(Vector3.up, Vector3.zero);
            Vector3 horizontalFoward = horizontal_plane.ClosestPointOnPlane(CameraCenter.forward).normalized;
            Quaternion horizontalRot = Quaternion.LookRotation(horizontalFoward, Vector3.up);

            Vector3 position = m_Is6dof ? transform.position : Vector3.zero;
            m_CachedWorldMatrix = ConversionUtility.GetTMatrix(position, horizontalRot);
        }
    }
}