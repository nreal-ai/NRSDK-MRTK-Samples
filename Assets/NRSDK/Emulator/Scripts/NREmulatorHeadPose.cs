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
    public class NREmulatorHeadPose : MonoBehaviour
    {

        private GameObject m_CameraTarget;
        /// <summary> regular speed. </summary>
        public float HeadMoveSpeed = 1.0f;
        /// <summary> How sensitive it with mouse. </summary>
        public float HeadRotateSpeed = 1.0f;

        public Pose headPose { get; private set; }
        private float m_HeadRotationLerpSpeed = 5f;

        private void Start()
        {
            DontDestroyOnLoad(this);
            if (m_CameraTarget == null)
            {
                m_CameraTarget = new GameObject("NREmulatorCameraTarget");
                m_CameraTarget.transform.position = Vector3.zero;
                m_CameraTarget.transform.rotation = Quaternion.identity;
                DontDestroyOnLoad(m_CameraTarget);
            }

            NRHMDPoseTracker.OnChangeTrackingMode += OnChangeTrackingMode;
        }

        private void OnChangeTrackingMode(NRHMDPoseTracker.TrackingType origin, NRHMDPoseTracker.TrackingType target)
        {
            m_CameraTarget.transform.position = Vector3.zero;
            m_CameraTarget.transform.rotation = Quaternion.identity;

            headPose = Pose.identity;
        }

        private void Update()
        {
            if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
            {
                UpdateHeadPosByInput();
            }
        }

        void UpdateHeadPosByInput()
        {
            var trackMode = NRSessionManager.Instance.NRHMDPoseTracker.TrackingMode;
            if (trackMode == NRHMDPoseTracker.TrackingType.Tracking0Dof)
                return;

            Vector3 pos = m_CameraTarget.transform.position;
            Quaternion q = m_CameraTarget.transform.rotation;

            if (Input.GetKey(KeyCode.Space))
            {
                float mouse_x = Input.GetAxis("Mouse X") * HeadRotateSpeed;
                float mouse_y = Input.GetAxis("Mouse Y") * HeadRotateSpeed;
                Vector3 mouseMove = new Vector3(m_CameraTarget.transform.eulerAngles.x - mouse_y, m_CameraTarget.transform.eulerAngles.y + mouse_x, 0);
                q = Quaternion.Euler(mouseMove);
            }

            if (trackMode == NRHMDPoseTracker.TrackingType.Tracking0DofStable)
            {
                q = Quaternion.Slerp(q, Quaternion.identity, m_HeadRotationLerpSpeed * Time.deltaTime);
            }
            else if (trackMode == NRHMDPoseTracker.TrackingType.Tracking6Dof)
            {
                Vector3 p = GetBaseInput();
                p = p * HeadMoveSpeed * Time.deltaTime;
                pos += p;
            }

            m_CameraTarget.transform.position = pos;
            m_CameraTarget.transform.rotation = q;

            headPose = new Pose(pos, q);
        }

        private Vector3 GetBaseInput()
        {
            Vector3 p_Velocity = new Vector3();
            if (Input.GetKey(KeyCode.W))
            {
                p_Velocity += m_CameraTarget.transform.forward.normalized;
            }
            if (Input.GetKey(KeyCode.S))
            {
                p_Velocity += -m_CameraTarget.transform.forward.normalized;
            }
            if (Input.GetKey(KeyCode.A))
            {
                p_Velocity += -m_CameraTarget.transform.right.normalized;
            }
            if (Input.GetKey(KeyCode.D))
            {
                p_Velocity += m_CameraTarget.transform.right.normalized;
            }
            return p_Velocity;
        }
    }
#endif
}