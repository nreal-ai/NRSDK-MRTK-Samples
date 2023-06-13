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
    using System.Collections.Generic;
    using UnityEngine;

    public class NREmulatorHandStatesService : IHandStatesService
    {
        public bool IsRunning { protected set; get; }

        private Dictionary<KeyCode, HandState> m_TestHandStatesDict;
        private HandState m_TargetHandState;
        private HandState m_DefaultLostHandState;
        private HandState m_DefaultTrackedHandState;
        private Pose m_HandRootPose = Pose.identity;
        private float m_CurrentHandRootFowardOffset;
        private float m_TargetHandRootFowardOffset;
        private Vector2 m_HandRootMovedAngles;
        private bool m_Inited;

        private const string MOUSE_AXIS_MOUSE_X = "Mouse X";
        private const string MOUSE_AXIS_MOUSE_Y = "Mouse Y";
        private const string MOUSE_AXIS_MOUSE_SCROLLWHEEL = "Mouse ScrollWheel";
        private const float MAX_MOVED_ANGLE_X = 90f;
        private const float MAX_MOVED_ANGLE_Y = 90f;
        private const float MOUSE_MOVE_SENSITIVITY = 500f;
        private const float MOUSE_SCROLL_SENSITIVITY = 45f;
        private const float HAND_ROOT_LERP_SPEED = 12f;
        private static Vector3 HAND_ROOT_DEFAULT_LOCAL_POSITION = new Vector3(0.06f, -0.1f, 0.2f);

        public virtual bool RunService()
        {
            Init();
            IsRunning = true;
            return true;
        }

        public virtual bool StopService()
        {
            Init();
            IsRunning = false;
            return true;
        }

        public virtual void UpdateStates(HandState[] handStates)
        {
            if (!IsRunning)
                return;
            handStates[1].Reset();
            var currentHandState = handStates[0];

            bool isTracked = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
            if (isTracked)
            {
                m_TargetHandState = m_DefaultTrackedHandState;
                foreach (KeyValuePair<KeyCode, HandState> kvPair in m_TestHandStatesDict)
                {
                    if (Input.GetKey(kvPair.Key))
                    {
                        m_TargetHandState = kvPair.Value;
                        break;
                    }
                }
            }
            else
            {
                m_TargetHandState = m_DefaultLostHandState;
            }
            ApplyToTargetHandState(currentHandState, m_TargetHandState);
        }

        public virtual void PauseService()
        {
            IsRunning = false;
        }

        public virtual void ResumeService()
        {
            IsRunning = true;
        }

        public virtual void DestroyService()
        {
            IsRunning = false;
        }

        protected virtual void Init()
        {
            if (m_Inited)
                return;

            m_DefaultLostHandState = CreateTestHandState_Lost_None(HandEnum.RightHand);
            m_DefaultTrackedHandState = CreateTestHandState_Found_OpenHand(HandEnum.RightHand);
            m_TestHandStatesDict = new Dictionary<KeyCode, HandState>()
            {
                {KeyCode.Mouse0, CreateTestHandState_Found_Pinch(HandEnum.RightHand)},
                {KeyCode.Mouse1, CreateTestHandState_Found_Point(HandEnum.RightHand)},
                {KeyCode.Mouse2, CreateTestHandState_Found_SystemGesture(HandEnum.RightHand)}
            };

            m_Inited = true;
        }

        protected void SetHandJointPose(HandState handState, HandJointID jointID, Pose pose)
        {
            if (handState.jointsPoseDict.ContainsKey(jointID))
            {
                handState.jointsPoseDict[jointID] = pose;
            }
            else
            {
                handState.jointsPoseDict.Add(jointID, pose);
            }
        }

        private void UpdateHandRootPose(HandState handState)
        {
            var cameraCenter = NRInput.CameraCenter;
            if (handState.isTracked)
            {
                m_TargetHandRootFowardOffset += Input.GetAxis(MOUSE_AXIS_MOUSE_SCROLLWHEEL) * MOUSE_SCROLL_SENSITIVITY * Time.deltaTime;
                m_TargetHandRootFowardOffset = Mathf.Clamp(m_TargetHandRootFowardOffset, -0.15f, 0.3f);
                m_CurrentHandRootFowardOffset = Mathf.Lerp(m_CurrentHandRootFowardOffset, m_TargetHandRootFowardOffset, HAND_ROOT_LERP_SPEED * Time.deltaTime);
                m_HandRootPose.position = cameraCenter.TransformPoint(HAND_ROOT_DEFAULT_LOCAL_POSITION) + m_HandRootPose.forward * m_CurrentHandRootFowardOffset;

                float delta_angle_x = Input.GetAxis(MOUSE_AXIS_MOUSE_X) * MOUSE_MOVE_SENSITIVITY * Time.deltaTime;
                float delat_angle_y = Input.GetAxis(MOUSE_AXIS_MOUSE_Y) * MOUSE_MOVE_SENSITIVITY * Time.deltaTime;
                m_HandRootMovedAngles += new Vector2(delta_angle_x, -delat_angle_y);
                m_HandRootMovedAngles.x = Mathf.Clamp(m_HandRootMovedAngles.x, -MAX_MOVED_ANGLE_X, MAX_MOVED_ANGLE_X);
                m_HandRootMovedAngles.y = Mathf.Clamp(m_HandRootMovedAngles.y, -MAX_MOVED_ANGLE_Y, MAX_MOVED_ANGLE_Y);

                var localRotation = Quaternion.Euler(Vector3.up * m_HandRootMovedAngles.x + Vector3.right * m_HandRootMovedAngles.y);
                m_HandRootPose.rotation = cameraCenter.rotation * localRotation;
            }
            else
            {
                m_CurrentHandRootFowardOffset = m_TargetHandRootFowardOffset = 0f;
                m_HandRootPose.position = cameraCenter.TransformPoint(HAND_ROOT_DEFAULT_LOCAL_POSITION);
                m_HandRootPose.rotation = cameraCenter.rotation;
                m_HandRootMovedAngles = Vector2.zero;
            }
        }

        private void ApplyToTargetHandState(HandState currentHandState, HandState targetHandState)
        {
            UpdateHandRootPose(targetHandState);

            currentHandState.isTracked = targetHandState.isTracked;
            currentHandState.currentGesture = targetHandState.currentGesture;

            Quaternion handRootQuation = m_HandRootPose.rotation;
            foreach (var jointPoseItem in targetHandState.jointsPoseDict)
            {
                var relativePosition = jointPoseItem.Value.position - HAND_ROOT_DEFAULT_LOCAL_POSITION;
                var relativePoseToRoot = new Pose(relativePosition, jointPoseItem.Value.rotation);
                var jointPosition = m_HandRootPose.position + handRootQuation * relativePoseToRoot.position;
                var jointRotation = handRootQuation * relativePoseToRoot.rotation;
                SetHandJointPose(currentHandState, jointPoseItem.Key, new Pose(jointPosition, jointRotation));
            }
        }

        #region Methods To Create Preset HandStates

        private HandState CreateTestHandState_Lost_None(HandEnum handEnum)
        {
            var handState = new HandState(handEnum);
            handState.isTracked = false;
            handState.currentGesture = HandGesture.None;
            return handState;
        }

        private HandState CreateTestHandState_Found_OpenHand(HandEnum handEnum)
        {
            var handState = new HandState(handEnum);
            handState.isTracked = true;
            handState.currentGesture = HandGesture.OpenHand;
            HandJointPoseDataUtility.JsonToDict(HandJointsArrayData.HandJointsArrayData_Right_OpenHand_Json, handState.jointsPoseDict);
            return handState;
        }

        private HandState CreateTestHandState_Found_Pinch(HandEnum handEnum)
        {
            var handState = new HandState(handEnum);
            handState.isTracked = true;
            handState.currentGesture = HandGesture.Grab;
            HandJointPoseDataUtility.JsonToDict(HandJointsArrayData.HandJointsArrayData_Right_Grab_Json, handState.jointsPoseDict);
            return handState;
        }

        private HandState CreateTestHandState_Found_Point(HandEnum handEnum)
        {
            var handState = new HandState(handEnum);
            handState.isTracked = true;
            handState.currentGesture = HandGesture.Point;
            HandJointPoseDataUtility.JsonToDict(HandJointsArrayData.HandJointsArrayData_Right_Point_Json, handState.jointsPoseDict);
            return handState;
        }

        private HandState CreateTestHandState_Found_SystemGesture(HandEnum handEnum)
        {
            var handState = new HandState(handEnum);
            handState.isTracked = true;
            handState.currentGesture = HandGesture.None;
            HandJointPoseDataUtility.JsonToDict(HandJointsArrayData.HandJointsArrayData_Right_SystemGesture_Json, handState.jointsPoseDict);
            return handState;
        }

        #endregion

    }
}
