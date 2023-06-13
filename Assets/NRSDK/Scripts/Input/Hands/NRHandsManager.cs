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
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary> A manager of hand states. </summary>
    public class NRHandsManager
    {
        private readonly Dictionary<HandEnum, NRHand> m_HandsDict;
        private readonly HandState[] m_HandStates; // index 0 represents right and index 1 represents left
        private readonly OneEuroFilter[] m_OneEuroFilters;
        private IHandStatesService m_HandStatesService;
        private bool m_Inited;

        public Action OnHandTrackingStarted;
        public Action OnHandStatesUpdated;
        public Action OnHandTrackingStopped;

        /// <summary>
        /// Returns true if the hand tracking is now running normally
        /// </summary>
        public bool IsRunning
        {
            get
            {
                if (m_HandStatesService != null)
                {
                    return m_HandStatesService.IsRunning;
                }
                return false;
            }
        }

        public NRHandsManager()
        {
            m_HandsDict = new Dictionary<HandEnum, NRHand>();
            m_HandStates = new HandState[2] { new HandState(HandEnum.RightHand), new HandState(HandEnum.LeftHand) };
            m_OneEuroFilters = new OneEuroFilter[2] { new OneEuroFilter(), new OneEuroFilter() };
        }

        /// <summary>
        /// Regist the left or right NRHand. There would be at most one NRHand for each hand enum
        /// </summary>
        /// <param name="hand"></param>
        internal void RegistHand(NRHand hand)
        {
            if (hand == null || hand.HandEnum == HandEnum.None)
                return;
            var handEnum = hand.HandEnum;
            if (m_HandsDict.ContainsKey(handEnum))
            {
                m_HandsDict[handEnum] = hand;
            }
            else
            {
                m_HandsDict.Add(handEnum, hand);
            }
        }

        /// <summary>
        /// UnRegist the left or right NRHand
        /// </summary>
        /// <param name="hand"></param>
        internal void UnRegistHand(NRHand hand)
        {
            if (hand == null)
                return;
            m_HandsDict.Remove(hand.HandEnum);
        }

        /// <summary>
        /// Init hand tracking with a certain service
        /// </summary>
        /// <param name="handStatesService"></param>
        internal void Init(IHandStatesService handStatesService = null)
        {
            if (m_Inited)
                return;

            m_HandStatesService = handStatesService;
            if (m_HandStatesService == null)
            {
#if UNITY_EDITOR
                m_HandStatesService = new NREmulatorHandStatesService();
#else
                m_HandStatesService = new NRHandStatesService();
#endif
            }

            NRInput.OnControllerStatesUpdated += UpdateHandTracking;
            m_Inited = true;
            NRDebugger.Info("[HandsManager] Hand Tracking Inited");
        }

        /// <summary>
        /// Returns true if start hand tracking success
        /// </summary>
        /// <returns></returns>
        internal bool StartHandTracking()
        {
            if (!m_Inited)
            {
                Init();
            }

            if (IsRunning)
            {
                NRDebugger.Info("[HandsManager] Hand Tracking Start: Success");
                return true;
            }

            if (m_HandStatesService != null && m_HandStatesService.RunService())
            {
                NRDebugger.Info("[HandsManager] Hand Tracking Start: Success");
                NRInput.SwitchControllerProvider(typeof(NRHandControllerProvider));
                OnHandTrackingStarted?.Invoke();
                return true;
            }

            NRDebugger.Info("[HandsManager] Hand Tracking Start: Failed");
            return false;
        }

        /// <summary>
        /// Returns true if stop hand tracking success
        /// </summary>
        /// <returns></returns>
        internal bool StopHandTracking()
        {
            if (!m_Inited)
            {
                NRDebugger.Info("[HandsManager] Hand Tracking Stop: Success");
                return true;
            }

            if (!IsRunning)
            {
                NRDebugger.Info("[HandsManager] Hand Tracking Stop: Success");
                return true;
            }

            if (m_HandStatesService != null && m_HandStatesService.StopService())
            {
                NRDebugger.Info("[HandsManager] Hand Tracking Stop: Success");
                NRInput.SwitchControllerProvider(ControllerProviderFactory.controllerProviderType);
                ResetHandStates();
                OnHandTrackingStopped?.Invoke();
                return true;
            }
            NRDebugger.Info("[HandsManager] Hand Tracking Stop: Failed");
            return false;
        }

        /// <summary>
        /// Get the current hand state of the left or right hand
        /// </summary>
        /// <param name="handEnum"></param>
        /// <returns></returns>
        public HandState GetHandState(HandEnum handEnum)
        {
            switch (handEnum)
            {
                case HandEnum.RightHand:
                    return m_HandStates[0];
                case HandEnum.LeftHand:
                    return m_HandStates[1];
                default:
                    break;
            }
            return null;
        }

        /// <summary>
        /// Get the left or right NRHand if it has been registed.
        /// </summary>
        /// <param name="handEnum"></param>
        /// <returns></returns>
        public NRHand GetHand(HandEnum handEnum)
        {
            NRHand hand;
            if (m_HandsDict != null && m_HandsDict.TryGetValue(handEnum, out hand))
            {
                return hand;
            }
            return null;
        }

        /// <summary>
        /// Returns true if user is now performing the systemGesture
        /// </summary>
        /// <returns></returns>
        public bool IsPerformingSystemGesture()
        {
            return IsPerformingSystemGesture(HandEnum.LeftHand) || IsPerformingSystemGesture(HandEnum.RightHand);
        }

        /// <summary>
        /// Returns true if user is now performing the systemGesture
        /// </summary>
        /// <param name="handEnum"></param>
        /// <returns></returns>
        public bool IsPerformingSystemGesture(HandEnum handEnum)
        {
            return IsPerformingSystemGesture(GetHandState(handEnum));
        }

        private void ResetHandStates()
        {
            for (int i = 0; i < m_HandStates.Length; i++)
            {
                m_HandStates[i].Reset();
            }
        }

        private void UpdateHandTracking()
        {
            if (!IsRunning)
                return;
            m_HandStatesService.UpdateStates(m_HandStates);
            UpdateHandPointer();
            OnHandStatesUpdated?.Invoke();
        }

        private void UpdateHandPointer()
        {
            for (int i = 0; i < m_HandStates.Length; i++)
            {
                var handState = m_HandStates[i];
                if (handState == null)
                    continue;

                CalculatePointerPose(handState);
            }
        }

        private void CalculatePointerPose(HandState handState)
        {
            if (handState.isTracked)
            {
                var wristPose = handState.GetJointPose(HandJointID.Wrist);
                var cameraTransform = NRInput.CameraCenter;
                handState.pointerPoseValid = Vector3.Angle(cameraTransform.forward, wristPose.forward) < 70f;

                if (handState.pointerPoseValid)
                {
                    Vector3 middleToRing = (handState.GetJointPose(HandJointID.MiddleProximal).position
                                          - handState.GetJointPose(HandJointID.RingProximal).position).normalized;
                    Vector3 middleToWrist = (handState.GetJointPose(HandJointID.MiddleProximal).position
                                           - handState.GetJointPose(HandJointID.Wrist).position).normalized;
                    Vector3 middleToCenter = Vector3.Cross(middleToWrist, middleToRing).normalized;
                    var pointerPosition = handState.GetJointPose(HandJointID.MiddleProximal).position
                                        + middleToWrist * 0.02f
                                        + middleToRing * 0.01f
                                        + middleToCenter * (handState.handEnum == HandEnum.RightHand ? 0.06f : -0.06f);

                    var handDirection = pointerPosition - wristPose.position;
                    var handRotation = Quaternion.LookRotation(handDirection);
                    var handtoCamera = wristPose.position - (cameraTransform.position - 0.08f * cameraTransform.forward);
                    float handtoCameraY = handtoCamera.y;
                    handtoCamera.y = 0;
                    var handtoCameraRot = Quaternion.LookRotation(handtoCamera);
                    var pointerRotation = Quaternion.Lerp(handtoCameraRot, handRotation, 0.3f)
                        * Quaternion.Euler(new Vector3(handtoCameraY * -150f - 30f, handState.handEnum == HandEnum.RightHand ? -15f : 15f, 0f));
                    Vector3 pointerRotationToV3 = pointerRotation * Vector3.forward;
                    pointerRotation = Quaternion.LookRotation(m_OneEuroFilters[(int)handState.handEnum].Step(Time.realtimeSinceStartup, pointerRotationToV3));
                    handState.pointerPose = new Pose(pointerPosition, pointerRotation);
                }
            }
            else
            {
                handState.pointerPoseValid = false;
            }
        }

        private bool IsPerformingSystemGesture(HandState handState)
        {
            if (!IsRunning)
            {
                return false;
            }
            return handState.currentGesture == HandGesture.System;
        }

        public class OneEuroFilter
        {
            public float Beta = 0.01f;
            public float MinCutoff = 2.0f;
            const float DCutOff = 1.0f;
            (float t, Vector3 x, Vector3 dx) _prev;

            public Vector3 Step(float t, Vector3 x)
            {
                var t_e = t - _prev.t;

                if (t_e < 1e-5f)
                    return _prev.x;

                var dx = (x - _prev.x) / t_e;
                var dx_res = Vector3.Lerp(_prev.dx, dx, Alpha(t_e, DCutOff));

                var cutoff = MinCutoff + Beta * dx_res.magnitude;
                var x_res = Vector3.Lerp(_prev.x, x, Alpha(t_e, cutoff));

                _prev = (t, x_res, dx_res);

                return x_res;
            }

            static float Alpha(float t_e, float cutoff)
            {
                var r = 2 * Mathf.PI * cutoff * t_e;
                return r / (r + 1);
            }
        }
    }
}
