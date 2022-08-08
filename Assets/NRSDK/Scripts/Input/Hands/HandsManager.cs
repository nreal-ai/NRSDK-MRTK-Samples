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
    public class HandsManager
    {
        private readonly Dictionary<HandEnum, NRHand> m_HandsDict;
        private readonly HandState[] m_HandStates; // index 0 represents right and index 1 represents left
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

        public HandsManager()
        {
            m_HandsDict = new Dictionary<HandEnum, NRHand>();
            m_HandStates = new HandState[2] { new HandState(HandEnum.RightHand), new HandState(HandEnum.LeftHand) };
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

                CaculatePointerPose(handState);
                CaculatePinchState(handState);
            }
        }

        private void CaculatePointerPose(HandState handState)
        {
            if (handState.isTracked)
            {
                var palmPose = handState.GetJointPose(HandJointID.Palm);
                var cameraTransform = NRInput.CameraCenter;
                handState.pointerPoseValid = Vector3.Angle(cameraTransform.forward, palmPose.forward) < 70f;

                if (handState.pointerPoseValid)
                {
                    var cameraWorldUp = NRFrame.GetWorldMatrixFromUnityToNative().MultiplyVector(Vector3.up).normalized;
                    var rayEndPoint = palmPose.position;
                    var right = Vector3.Cross(rayEndPoint - cameraTransform.position, cameraWorldUp).normalized;
                    var horizontalVec = right * (handState.handEnum == HandEnum.RightHand ? -1f : 1f);
                    var rayStartPoint = cameraTransform.position + horizontalVec * 0.14f - cameraWorldUp * 0.16f;

                    var foward = (rayEndPoint - rayStartPoint).normalized;
                    var upwards = Vector3.Cross(right, foward);
                    var pointerRotation = Quaternion.LookRotation(foward, upwards);
                    var pointerPosition = rayEndPoint + cameraWorldUp * 0.01f + foward * 0.06f - horizontalVec * 0.03f;
                    handState.pointerPose = new Pose(pointerPosition, pointerRotation);
                }
            }
            else
            {
                handState.pointerPoseValid = false;
            }
        }

        private void CaculatePinchState(HandState handState)
        {
            handState.pinchStrength = HandStateUtility.GetIndexFingerPinchStrength(handState);
            handState.isPinching = handState.pinchStrength > float.Epsilon;
        }

        private bool IsPerformingSystemGesture(HandState handState)
        {
            if (!IsRunning)
            {
                return false;
            }
            return HandStateUtility.IsSystemGesture(handState);
        }
    }
}
