namespace NRKernal.NRExamples
{
    using System.Collections.Generic;
    using UnityEngine;

    public class NRHandMeshVisual : MonoBehaviour
    {
        [SerializeField]
        private NRHandMeshJointConfig m_HandPrefab;
        private NRHandMeshJointConfig m_HandMeshJoint;

        [SerializeField]
        private bool m_UpdateHandScale;
        // Use distance between these joints to calculate the length of hand
        private readonly List<HandJointID> m_HandLengthJoints = new List<HandJointID>() {
            HandJointID.MiddleTip, HandJointID.MiddleDistal, HandJointID.MiddleMiddle, HandJointID.MiddleProximal, HandJointID.Wrist
        };
        private float m_HandLength;

        private void Start()
        {
            CreateMeshVisuals();
        }

        private void CreateMeshVisuals()
        {
            if (m_HandPrefab != null)
            {
                m_HandMeshJoint = Instantiate(m_HandPrefab, transform);
                InitHandLength();
            }
        }

        private void InitHandLength()
        {
            m_HandLength = 0;
            for (int i = 0; i < m_HandLengthJoints.Count - 1; i++)
            {
                m_HandLength += Vector3.Distance(m_HandMeshJoint.HandJoint[(int)m_HandLengthJoints[i]].position, 
                    m_HandMeshJoint.HandJoint[(int)m_HandLengthJoints[i + 1]].position);
            }
        }

        private void OnEnable()
        {
            NRInput.Hands.OnHandStatesUpdated += OnHandTracking;
            NRInput.Hands.OnHandTrackingStopped += OnHandTrackingStopped;
        }

        private void OnDisable()
        {
            NRInput.Hands.OnHandStatesUpdated -= OnHandTracking;
            NRInput.Hands.OnHandTrackingStopped -= OnHandTrackingStopped;
        }

        private void OnHandTracking()
        {
            if (m_HandMeshJoint != null)
            {
                var handState = NRInput.Hands.GetHandState(m_HandMeshJoint.HandEnum);
                m_HandMeshJoint.gameObject.SetActive(handState.isTracked);
                if (handState.isTracked)
                {
                    if (m_UpdateHandScale)
                        UpdateHandScale(handState);
                    UpdateWristByMiddle(handState);
                    UpdateFingerJoint(handState);
                }
            }
        }

        private void UpdateHandScale(HandState handState)
        {
            // if the angle between knuckles is less than this value, the finger consider to be straight
            const float fingerStraightAngle = 15f;
            Vector3 handVec = handState.GetJointPose(m_HandLengthJoints[0]).position
                - handState.GetJointPose(m_HandLengthJoints[m_HandLengthJoints.Count - 1]).position;
            float length = 0;
            for (int i = 0; i < m_HandLengthJoints.Count - 1; i++)
            {
                Vector3 knuckleVec = handState.GetJointPose(m_HandLengthJoints[i]).position
                    - handState.GetJointPose(m_HandLengthJoints[i + 1]).position;
                if (Vector3.Angle(handVec, knuckleVec) > fingerStraightAngle)
                {
                    return;
                }
                length += knuckleVec.magnitude;
            }

            float totalScale = length / m_HandLength;
            float angle = Vector3.Angle(NRInput.CameraCenter.forward, handVec);
            if (Mathf.Abs(angle - 90) > fingerStraightAngle)
                return;
            var wristTransform = m_HandMeshJoint.HandJoint[0];
            totalScale = Mathf.Clamp(totalScale, 0.7f, 1.3f) * 1.1f;
            totalScale = Mathf.MoveTowards(wristTransform.localScale.x, totalScale, 0.01f);
            wristTransform.localScale = Vector3.one * totalScale;
        }

        private void UpdateWristByMiddle(HandState handState)
        {
            Transform wristTransform = m_HandMeshJoint.HandJoint[(int)HandJointID.Wrist];
            var wristPose = handState.GetJointPose(HandJointID.Wrist);
            wristPose.rotation *= Quaternion.Euler(m_HandMeshJoint.RotationOffset);
            wristTransform.rotation = wristPose.rotation;

            var middlePose = handState.GetJointPose(HandJointID.MiddleProximal);
            Transform middleTransform = m_HandMeshJoint.HandJoint[(int)HandJointID.MiddleProximal];
            Vector4 middleLocalPos = middleTransform.localPosition;
            middleLocalPos.w = 1;
            Vector3 tmp = Matrix4x4.Rotate(wristTransform.rotation) * Matrix4x4.Scale(wristTransform.localScale) * middleLocalPos;
            wristTransform.position = middlePose.position - tmp;
        }

        private void UpdateFingerJoint(HandState handState)
        {
            for (int i = (int)HandJointID.ThumbMetacarpal; i <= (int)HandJointID.PinkyTip; i++)
            {
                var t = m_HandMeshJoint.HandJoint[i];
                if (t != null)
                {
                    var pose = handState.GetJointPose((HandJointID)i);
                    t.rotation = pose.rotation * Quaternion.Euler(m_HandMeshJoint.RotationOffset);
                }
            }
        }

        private void OnHandTrackingStopped()
        {
            if (m_HandMeshJoint != null)
            {
                m_HandMeshJoint.gameObject.SetActive(false);
            }
        }
    }
}
