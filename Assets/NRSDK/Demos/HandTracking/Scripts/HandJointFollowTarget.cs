using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NRKernal.NRExamples
{
    public class HandJointFollowTarget : MonoBehaviour
    {
        public HandEnum handEnum;
        public HandJointID handJoint;
        public HandGesture activeByHandGesture = HandGesture.None;

        public bool alwaysWorldUp;
        public float worldUpOffset;

        private HandState m_RelatedHandState;

        private void Update()
        {
            if (!NRInput.Hands.IsRunning)
                return;
            m_RelatedHandState = NRInput.Hands.GetHandState(handEnum);
            UpdateJointPose();
        }

        private void UpdateJointPose()
        {
            if (m_RelatedHandState == null || !m_RelatedHandState.isTracked)
                return;
            var jointPose = m_RelatedHandState.GetJointPose(handJoint);

            if (alwaysWorldUp)
            {
                transform.position = jointPose.position + Vector3.up * worldUpOffset;
                var eulerAngles = jointPose.rotation.eulerAngles;
                eulerAngles.x = 0f;
                eulerAngles.z = 0f;
                transform.eulerAngles = eulerAngles;
            }
            else
            {
                transform.position = jointPose.position;
                transform.rotation = jointPose.rotation;
            }
        }

        private bool ShouldActive()
        {
            if (m_RelatedHandState == null || !m_RelatedHandState.isTracked)
                return false;
            if (activeByHandGesture == HandGesture.None)
                return true;
            if (m_RelatedHandState.currentGesture == activeByHandGesture)
                return true;
            return false;
        }
    }
}