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

    public class NRHandCapsuleVisual : MonoBehaviour
    {
        public class CapsuleVisualInfo
        {
            public bool shouldRender;
            public HandJointID startHandJointID;
            public HandJointID endHandJointID;
            public float capsuleRadius;
            public Vector3 startPos;
            public Vector3 endPos;
            public Material capsuleMat;

            public CapsuleVisualInfo(HandJointID startHandJointID, HandJointID endHandJointID)
            {
                this.startHandJointID = startHandJointID;
                this.endHandJointID = endHandJointID;
            }
        }

        public class CapsuleVisual
        {
            public CapsuleVisualInfo capsuleVisualInfo;

            private GameObject m_VisualGO;
            private Vector3 m_CapsuleScale;
            private MeshRenderer m_Renderer;
            private CapsuleCollider m_Collider;

            public CapsuleVisual(GameObject rootGO, CapsuleVisualInfo capsuleVisualInfo)
            {
                this.capsuleVisualInfo = capsuleVisualInfo;

                m_VisualGO = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                m_VisualGO.transform.SetParent(rootGO.transform);
                m_Collider = m_VisualGO.GetComponent<CapsuleCollider>();
                if (m_Collider)
                {
                    m_Collider.enabled = false;
                }
                m_Renderer = m_VisualGO.GetComponent<MeshRenderer>();
                if (capsuleVisualInfo.capsuleMat)
                {
                    m_Renderer.material = capsuleVisualInfo.capsuleMat;
                }
                m_CapsuleScale = Vector3.zero;
                m_VisualGO.transform.localScale = m_CapsuleScale;
            }

            public void OnUpdate()
            {
                if (m_VisualGO == null)
                    return;

                if (capsuleVisualInfo.shouldRender)
                {
                    DrawCapsuleVisual(capsuleVisualInfo.startPos, capsuleVisualInfo.endPos, capsuleVisualInfo.capsuleRadius);
                }
                else
                {
                    m_VisualGO.SetActive(false);
                }
            }

            private void DrawCapsuleVisual(Vector3 a, Vector3 b, float radius)
            {
                m_VisualGO.SetActive(true);
                m_VisualGO.transform.position = (a + b) * 0.5f;
                m_VisualGO.transform.up = a - b;

                m_CapsuleScale = Vector3.one;
                m_CapsuleScale.y = Vector3.Distance(a, b) * 0.5f;
                m_CapsuleScale.x = m_CapsuleScale.z = radius * 2f;
                m_VisualGO.transform.localScale = m_CapsuleScale;
            }
        }

        public class JointVisualInfo
        {
            public bool shouldRender;
            public HandJointID handJointID;
            public float jointRadius;
            public Vector3 jointPos;
            public Material jointMat;

            public JointVisualInfo(HandJointID handJointID)
            {
                this.handJointID = handJointID;
            }
        }

        public class JointVisual
        {
            public JointVisualInfo jointVisualInfo;

            private GameObject m_VisualGO;
            private Vector3 m_JointScale;
            private MeshRenderer m_Renderer;
            private SphereCollider m_Collider;

            public JointVisual(GameObject rootGO, JointVisualInfo jointVisualInfo)
            {
                this.jointVisualInfo = jointVisualInfo;

                m_VisualGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                m_VisualGO.transform.SetParent(rootGO.transform);
                m_Collider = m_VisualGO.GetComponent<SphereCollider>();
                if (m_Collider)
                {
                    m_Collider.enabled = false;
                }
                m_Renderer = m_VisualGO.GetComponent<MeshRenderer>();
                if (jointVisualInfo.jointMat)
                {
                    m_Renderer.material = jointVisualInfo.jointMat;
                }
                m_JointScale = Vector3.zero;
                m_VisualGO.transform.localScale = m_JointScale;
            }

            public void OnUpdate()
            {
                if (m_VisualGO == null)
                    return;

                if (jointVisualInfo.shouldRender)
                {
                    DrawJointVisual(jointVisualInfo.jointPos, jointVisualInfo.jointRadius);
                }
                else
                {
                    m_VisualGO.SetActive(false);
                }
            }

            private void DrawJointVisual(Vector3 pos, float radius)
            {
                m_VisualGO.SetActive(true);
                m_VisualGO.transform.position = pos;
                m_JointScale = Vector3.one * radius * 2f;
                m_VisualGO.transform.localScale = m_JointScale;
            }
        }

        public HandEnum handEnum;
        public Material capsuleMat;
        public float capsuleRadius = 0.003f;
        public bool showCapsule = true;
        public Material jointMat;
        public float jointRadius = 0.005f;
        public bool showJoint = true;

        private List<CapsuleVisual> m_CapsuleVisuals;
        private List<JointVisual> m_JointVisuals;

        private void Start()
        {
            CreateCapsuleVisuals();
            CreateJointVisuals();
        }

        private void CreateCapsuleVisuals()
        {
            var capsuleVisualInfoList = new List<CapsuleVisualInfo>()
            {
                new CapsuleVisualInfo(HandJointID.Wrist, HandJointID.ThumbMetacarpal),
                new CapsuleVisualInfo(HandJointID.Wrist, HandJointID.PinkyMetacarpal),
                new CapsuleVisualInfo(HandJointID.ThumbMetacarpal, HandJointID.ThumbProximal),
                new CapsuleVisualInfo(HandJointID.PinkyMetacarpal, HandJointID.PinkyProximal),
                new CapsuleVisualInfo(HandJointID.ThumbProximal, HandJointID.ThumbDistal),
                new CapsuleVisualInfo(HandJointID.ThumbDistal, HandJointID.ThumbTip),
                new CapsuleVisualInfo(HandJointID.IndexProximal, HandJointID.IndexMiddle),
                new CapsuleVisualInfo(HandJointID.IndexMiddle, HandJointID.IndexDistal),
                new CapsuleVisualInfo(HandJointID.IndexDistal, HandJointID.IndexTip),
                new CapsuleVisualInfo(HandJointID.MiddleProximal, HandJointID.MiddleMiddle),
                new CapsuleVisualInfo(HandJointID.MiddleMiddle, HandJointID.MiddleDistal),
                new CapsuleVisualInfo(HandJointID.MiddleDistal, HandJointID.MiddleTip),
                new CapsuleVisualInfo(HandJointID.RingProximal, HandJointID.RingMiddle),
                new CapsuleVisualInfo(HandJointID.RingMiddle, HandJointID.RingDistal),
                new CapsuleVisualInfo(HandJointID.RingDistal, HandJointID.RingTip),
                new CapsuleVisualInfo(HandJointID.PinkyProximal, HandJointID.PinkyMiddle),
                new CapsuleVisualInfo(HandJointID.PinkyMiddle, HandJointID.PinkyDistal),
                new CapsuleVisualInfo(HandJointID.PinkyDistal, HandJointID.PinkyTip),
                new CapsuleVisualInfo(HandJointID.ThumbProximal, HandJointID.IndexProximal),
                new CapsuleVisualInfo(HandJointID.IndexProximal, HandJointID.MiddleProximal),
                new CapsuleVisualInfo(HandJointID.MiddleProximal, HandJointID.RingProximal),
                new CapsuleVisualInfo(HandJointID.RingProximal, HandJointID.PinkyProximal)
            };

            m_CapsuleVisuals = new List<CapsuleVisual>();
            for (int i = 0; i < capsuleVisualInfoList.Count; i++)
            {
                var capsuleInfo = capsuleVisualInfoList[i];
                capsuleInfo.capsuleMat = capsuleMat;
                m_CapsuleVisuals.Add(new CapsuleVisual(gameObject, capsuleInfo));
            }
        }

        private void CreateJointVisuals()
        {
            m_JointVisuals = new List<JointVisual>();
            foreach (var item in System.Enum.GetValues(typeof(HandJointID)))
            {
                var jointID = (HandJointID)item;
                if (jointID > HandJointID.Invalid && jointID < HandJointID.Max)
                {
                    var jointVisualInfo = new JointVisualInfo(jointID);
                    jointVisualInfo.jointMat = jointMat;
                    m_JointVisuals.Add(new JointVisual(gameObject, jointVisualInfo));
                }
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

        private void OnHandTrackingStopped()
        {
            OnHandingTrackingLost();
        }

        private void OnHandTracking()
        {
            var handState = NRInput.Hands.GetHandState(handEnum);
            if (m_CapsuleVisuals != null)
            {
                for (int i = 0; i < m_CapsuleVisuals.Count; i++)
                {
                    var capsuleVisual = m_CapsuleVisuals[i];
                    UpstateCapsuleVisualInfo(capsuleVisual.capsuleVisualInfo, handState);
                    capsuleVisual.OnUpdate();
                }
            }
            if (m_JointVisuals != null)
            {
                for (int i = 0; i < m_JointVisuals.Count; i++)
                {
                    var jointVisual = m_JointVisuals[i];
                    UpdateJointVisualInfo(jointVisual.jointVisualInfo, handState);
                    jointVisual.OnUpdate();
                }
            }
        }

        private void UpstateCapsuleVisualInfo(CapsuleVisualInfo info, HandState handState)
        {
            info.shouldRender = handState.isTracked;
            info.startPos = handState.GetJointPose(info.startHandJointID).position;
            info.endPos = handState.GetJointPose(info.endHandJointID).position;
            info.capsuleRadius = capsuleRadius;
        }

        private void UpdateJointVisualInfo(JointVisualInfo info, HandState handState)
        {
            info.shouldRender = handState.isTracked;
            info.jointPos = handState.GetJointPose(info.handJointID).position;
            info.jointRadius = jointRadius;
        }

        private void OnHandingTrackingLost()
        {
            //force refresh visual
            OnHandTracking();
        }
    }
}
