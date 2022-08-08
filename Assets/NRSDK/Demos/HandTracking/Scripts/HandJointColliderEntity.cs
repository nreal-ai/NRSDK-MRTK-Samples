using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NRKernal.NRExamples
{
    public class HandJointColliderEntity : MonoBehaviour
    {
        public HandEnum handEnum;
        public HandJointID handJoint;
        public HandGesture activeByHandGesture = HandGesture.None;

        private HandState m_RelatedHandState;
        private Collider m_JointCollider;
        private MeshRenderer m_MeshRenderer;

        public bool IsColliderActive { get; private set; }

        private void Start()
        {
            m_JointCollider = GetComponent<Collider>();
            m_JointCollider.enabled = false;
            m_MeshRenderer = GetComponent<MeshRenderer>();
            m_MeshRenderer.enabled = false;
        }

        private void Update()
        {
            m_RelatedHandState = NRInput.Hands.GetHandState(handEnum);
            IsColliderActive = ShouldActive();
            m_MeshRenderer.enabled = m_JointCollider.enabled = IsColliderActive;
            if (IsColliderActive)
            {
                UpdateJointPose();
            }
        }

        private void OnDisable()
        {
            IsColliderActive = false;
        }

        private bool ShouldActive()
        {
            if (!NRInput.Hands.IsRunning)
                return false;
            if (m_RelatedHandState == null || !m_RelatedHandState.isTracked)
                return false;
            if (activeByHandGesture == HandGesture.None)
                return true;
            if (m_RelatedHandState.currentGesture == activeByHandGesture)
                return true;
            return false;
        }

        private void UpdateJointPose()
        {
            if (m_RelatedHandState == null)
                return;
            var jointPose = m_RelatedHandState.GetJointPose(handJoint);
            transform.position = jointPose.position;
            transform.rotation = jointPose.rotation;
        }
    }
}
