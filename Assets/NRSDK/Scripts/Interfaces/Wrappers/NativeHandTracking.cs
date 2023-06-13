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
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// Hand Tracking's Native API.
    /// </summary>
    internal partial class NativeHandTracking
    {
        public enum NRHandType
        {
            NR_HAND_TYPE_UNKNOWN = -1,
            NR_HAND_TYPE_LEFT = 0,
            NR_HAND_TYPE_RIGHT = 1
        }

        public enum NRHandJointType
        {
            NR_HAND_JOINT_TYPE_INVALID = -1,
            NR_HAND_JOINT_TYPE_THUMB_0 = 0,
            NR_HAND_JOINT_TYPE_THUMB_1 = 1,
            NR_HAND_JOINT_TYPE_THUMB_2 = 2,
            NR_HAND_JOINT_TYPE_THUMB_3 = 3,
            NR_HAND_JOINT_TYPE_INDEX_1 = 4,
            NR_HAND_JOINT_TYPE_INDEX_2 = 5,
            NR_HAND_JOINT_TYPE_INDEX_3 = 6,
            NR_HAND_JOINT_TYPE_INDEX_4 = 7,
            NR_HAND_JOINT_TYPE_MIDDLE_1 = 8,
            NR_HAND_JOINT_TYPE_MIDDLE_2 = 9,
            NR_HAND_JOINT_TYPE_MIDDLE_3 = 10,
            NR_HAND_JOINT_TYPE_MIDDLE_4 = 11,
            NR_HAND_JOINT_TYPE_RING_1 = 12,
            NR_HAND_JOINT_TYPE_RING_2 = 13,
            NR_HAND_JOINT_TYPE_RING_3 = 14,
            NR_HAND_JOINT_TYPE_RING_4 = 15,
            NR_HAND_JOINT_TYPE_PINKY_0 = 16,
            NR_HAND_JOINT_TYPE_PINKY_1 = 17,
            NR_HAND_JOINT_TYPE_PINKY_2 = 18,
            NR_HAND_JOINT_TYPE_PINKY_3 = 19,
            NR_HAND_JOINT_TYPE_PINKY_4 = 20,
            NR_HAND_JOINT_TYPE_PALM_CENTER = 21,
            NR_HAND_JOINT_TYPE_WRIST_BEGIN = 22,
            NR_HAND_JOINT_TYPE_WRIST_END = 23,
            NR_HAND_JOINT_TYPE_WRIST_CENTER = 24
        }

        public enum NRHandJointMask : UInt64
        {
            NR_HAND_JOINT_TYPE_MASK_THUMB_0 = (1L << NRHandJointType.NR_HAND_JOINT_TYPE_THUMB_0),
            NR_HAND_JOINT_TYPE_MASK_THUMB_1 = (1L << NRHandJointType.NR_HAND_JOINT_TYPE_THUMB_1),
            NR_HAND_JOINT_TYPE_MASK_THUMB_2 = (1L << NRHandJointType.NR_HAND_JOINT_TYPE_THUMB_2),
            NR_HAND_JOINT_TYPE_MASK_THUMB_3 = (1L << NRHandJointType.NR_HAND_JOINT_TYPE_THUMB_3),
            NR_HAND_JOINT_TYPE_MASK_INDEX_1 = (1L << NRHandJointType.NR_HAND_JOINT_TYPE_INDEX_1),
            NR_HAND_JOINT_TYPE_MASK_INDEX_2 = (1L << NRHandJointType.NR_HAND_JOINT_TYPE_INDEX_2),
            NR_HAND_JOINT_TYPE_MASK_INDEX_3 = (1L << NRHandJointType.NR_HAND_JOINT_TYPE_INDEX_3),
            NR_HAND_JOINT_TYPE_MASK_INDEX_4 = (1L << NRHandJointType.NR_HAND_JOINT_TYPE_INDEX_4),
            NR_HAND_JOINT_TYPE_MASK_MIDDLE_1 = (1L << NRHandJointType.NR_HAND_JOINT_TYPE_MIDDLE_1),
            NR_HAND_JOINT_TYPE_MASK_MIDDLE_2 = (1L << NRHandJointType.NR_HAND_JOINT_TYPE_MIDDLE_2),
            NR_HAND_JOINT_TYPE_MASK_MIDDLE_3 = (1L << NRHandJointType.NR_HAND_JOINT_TYPE_MIDDLE_3),
            NR_HAND_JOINT_TYPE_MASK_MIDDLE_4 = (1L << NRHandJointType.NR_HAND_JOINT_TYPE_MIDDLE_4),
            NR_HAND_JOINT_TYPE_MASK_RING_1 = (1L << NRHandJointType.NR_HAND_JOINT_TYPE_RING_1),
            NR_HAND_JOINT_TYPE_MASK_RING_2 = (1L << NRHandJointType.NR_HAND_JOINT_TYPE_RING_2),
            NR_HAND_JOINT_TYPE_MASK_RING_3 = (1L << NRHandJointType.NR_HAND_JOINT_TYPE_RING_3),
            NR_HAND_JOINT_TYPE_MASK_RING_4 = (1L << NRHandJointType.NR_HAND_JOINT_TYPE_RING_4),
            NR_HAND_JOINT_TYPE_MASK_PINKY_0 = (1L << NRHandJointType.NR_HAND_JOINT_TYPE_PINKY_0),
            NR_HAND_JOINT_TYPE_MASK_PINKY_1 = (1L << NRHandJointType.NR_HAND_JOINT_TYPE_PINKY_1),
            NR_HAND_JOINT_TYPE_MASK_PINKY_2 = (1L << NRHandJointType.NR_HAND_JOINT_TYPE_PINKY_2),
            NR_HAND_JOINT_TYPE_MASK_PINKY_3 = (1L << NRHandJointType.NR_HAND_JOINT_TYPE_PINKY_3),
            NR_HAND_JOINT_TYPE_MASK_PINKY_4 = (1L << NRHandJointType.NR_HAND_JOINT_TYPE_PINKY_4),
            NR_HAND_JOINT_TYPE_MASK_PALM_CENTER = (1L << NRHandJointType.NR_HAND_JOINT_TYPE_PALM_CENTER),
            NR_HAND_JOINT_TYPE_MASK_WRIST_BEGIN = (1L << NRHandJointType.NR_HAND_JOINT_TYPE_WRIST_BEGIN),
            NR_HAND_JOINT_TYPE_MASK_WRIST_END = (1L << NRHandJointType.NR_HAND_JOINT_TYPE_WRIST_END),
            NR_HAND_JOINT_TYPE_MASK_WRIST_CENTER = (1L << NRHandJointType.NR_HAND_JOINT_TYPE_WRIST_CENTER),
            NR_HAND_JOINT_TYPE_MASK_ALL = 0x7FFFFFFFFFFFFFFF
        }

        public enum NRGestureType
        {
            NR_GESTURE_TYPE_UNKNOWN = -1,
            NR_GESTURE_TYPE_OPEN_HAND = 0,
            NR_GESTURE_TYPE_GRAB = 1,
            NR_GESTURE_TYPE_PINCH = 2,
            NR_GESTURE_TYPE_POINT = 3,
            NR_GESTURE_TYPE_VICTORY = 4,
            NR_GESTURE_TYPE_CALL = 5,
            NR_GESTURE_TYPE_SYSTEM = 6,
        }

        public enum NRGestureTypeMask : UInt64
        {
            NR_GESTURE_TYPE_MASK_OPEN_HAND = (1L << NRGestureType.NR_GESTURE_TYPE_OPEN_HAND),
            NR_GESTURE_TYPE_MASK_GRAB = (1L << NRGestureType.NR_GESTURE_TYPE_GRAB),
            NR_GESTURE_TYPE_MASK_PINCH = (1L << NRGestureType.NR_GESTURE_TYPE_PINCH),
            NR_GESTURE_TYPE_MASK_POINT = (1L << NRGestureType.NR_GESTURE_TYPE_POINT),
            NR_GESTURE_TYPE_MASK_VICTORY = (1L << NRGestureType.NR_GESTURE_TYPE_VICTORY),
            NR_GESTURE_TYPE_MASK_CALL = (1L << NRGestureType.NR_GESTURE_TYPE_CALL),
            NR_GESTURE_TYPE_MASK_SYSTEM = (1L << NRGestureType.NR_GESTURE_TYPE_SYSTEM),
            NR_GESTURE_TYPE_MASK_ALL = 0x7FFFFFFFFFFFFFFF
        }

        public enum NRHandTrackingSupportFunction
        {
            NR_HANDTRACKING_SUPPORT_TIMESTAMP = 0,
            NR_HANDTRACKING_SUPPORT_HAND_JOINT_POSITION = 1,
            NR_HANDTRACKING_SUPPORT_HAND_JOINT_ROTATION = 2
        }

        public enum NRHandTrackingSupportFunctionMask : UInt64
        {
            NR_HANDTRACKING_SUPPORT_MASK_TIMESTAMP = (1L << NRHandTrackingSupportFunction.NR_HANDTRACKING_SUPPORT_TIMESTAMP),
            NR_HANDTRACKING_SUPPORT_MASK_HAND_JOINT_POSITION = (1L << NRHandTrackingSupportFunction.NR_HANDTRACKING_SUPPORT_HAND_JOINT_POSITION),
            NR_HANDTRACKING_SUPPORT_MASK_HAND_JOINT_ROTATION = (1L << NRHandTrackingSupportFunction.NR_HANDTRACKING_SUPPORT_HAND_JOINT_ROTATION)
        }

        private UInt64 m_HandTrackingHandle = 0;
        private UInt64 m_TrackingDataHandle = 0;
        private UInt64 m_CurrentHMDTimeNanos = 0;
        private UInt64 m_AvailableHandJointTypeMask = 0;
        private UInt64 m_AvailableGestureTypeMask = 0;
        private UInt64 m_SupportedFunctionMask = 0;

        private Dictionary<NRHandJointType, HandJointID> m_JointMapping = new Dictionary<NRHandJointType, HandJointID>
        {
            {NRHandJointType.NR_HAND_JOINT_TYPE_WRIST_CENTER, HandJointID.Wrist},
            {NRHandJointType.NR_HAND_JOINT_TYPE_PALM_CENTER, HandJointID.Palm},
            {NRHandJointType.NR_HAND_JOINT_TYPE_THUMB_0, HandJointID.ThumbMetacarpal},
            {NRHandJointType.NR_HAND_JOINT_TYPE_THUMB_1, HandJointID.ThumbProximal},
            {NRHandJointType.NR_HAND_JOINT_TYPE_THUMB_2, HandJointID.ThumbDistal},
            {NRHandJointType.NR_HAND_JOINT_TYPE_THUMB_3, HandJointID.ThumbTip},
            {NRHandJointType.NR_HAND_JOINT_TYPE_INDEX_1, HandJointID.IndexProximal},
            {NRHandJointType.NR_HAND_JOINT_TYPE_INDEX_2, HandJointID.IndexMiddle},
            {NRHandJointType.NR_HAND_JOINT_TYPE_INDEX_3, HandJointID.IndexDistal},
            {NRHandJointType.NR_HAND_JOINT_TYPE_INDEX_4, HandJointID.IndexTip},
            {NRHandJointType.NR_HAND_JOINT_TYPE_MIDDLE_1, HandJointID.MiddleProximal},
            {NRHandJointType.NR_HAND_JOINT_TYPE_MIDDLE_2, HandJointID.MiddleMiddle},
            {NRHandJointType.NR_HAND_JOINT_TYPE_MIDDLE_3, HandJointID.MiddleDistal},
            {NRHandJointType.NR_HAND_JOINT_TYPE_MIDDLE_4, HandJointID.MiddleTip},
            {NRHandJointType.NR_HAND_JOINT_TYPE_RING_1, HandJointID.RingProximal},
            {NRHandJointType.NR_HAND_JOINT_TYPE_RING_2, HandJointID.RingMiddle},
            {NRHandJointType.NR_HAND_JOINT_TYPE_RING_3, HandJointID.RingDistal},
            {NRHandJointType.NR_HAND_JOINT_TYPE_RING_4, HandJointID.RingTip},
            {NRHandJointType.NR_HAND_JOINT_TYPE_PINKY_0, HandJointID.PinkyMetacarpal},
            {NRHandJointType.NR_HAND_JOINT_TYPE_PINKY_1, HandJointID.PinkyProximal},
            {NRHandJointType.NR_HAND_JOINT_TYPE_PINKY_2, HandJointID.PinkyMiddle},
            {NRHandJointType.NR_HAND_JOINT_TYPE_PINKY_3, HandJointID.PinkyDistal},
            {NRHandJointType.NR_HAND_JOINT_TYPE_PINKY_4, HandJointID.PinkyTip}
        };

        private Transform m_CachedCameraRigTransform; //instead of using this, should use CameraRigTransform
        private Transform CameraRigTransform
        {
            get
            {
                if (m_CachedCameraRigTransform == null)
                {
                    if (NRSessionManager.Instance != null)
                    {
                        m_CachedCameraRigTransform = NRSessionManager.Instance.NRSessionBehaviour.transform;
                    }
                }
                return m_CachedCameraRigTransform;
            }
        }

        public NativeHandTracking()
        {
            var result = NativeApi.NRHandTrackingCreate(ref m_HandTrackingHandle);
            NativeErrorListener.Check(result, this, "Create");
        }
        
        public bool Start()
        {
            if (m_HandTrackingHandle == 0)
                return false;
            var result = NativeApi.NRHandTrackingStart(m_HandTrackingHandle);
            NativeErrorListener.Check(result, this, "Start", true);

            NativeApi.NRHandTrackingGetAvailableHandJoint(m_HandTrackingHandle, ref m_AvailableHandJointTypeMask);
            NativeApi.NRHandTrackingGetAvailableGestureType(m_HandTrackingHandle, ref m_AvailableGestureTypeMask);
            NativeApi.NRHandTrackingGetSupportedFunctions(m_HandTrackingHandle, ref m_SupportedFunctionMask);

            return result == NativeResult.Success;
        }

        public void Update(HandState[] handStates)
        {
            if (m_HandTrackingHandle == 0)
                return;

            ulong timeStamp = NRFrame.CurrentPoseTimeStamp;
            if (timeStamp == 0)
            {
                NRSessionManager.Instance.NativeAPI.NativeRenderring.GetFramePresentTime(ref timeStamp);
            }
            if (!TryAcquireHandTrackingDataHandle(timeStamp))
                return;

            UInt64 hmd_time_nanos = 0;
            var timeResult = NativeApi.NRHandTrackingDataGetHMDTimeNanos(m_TrackingDataHandle, ref hmd_time_nanos);
            if (timeResult != NativeResult.Success)
            {
                NativeErrorListener.Check(timeResult, this, "hmd_time_nanos");
            }

            UInt32 hand_count = 0;
            var countResult = NativeApi.NRHandTrackingDataGetHandsCount(m_TrackingDataHandle, ref hand_count);
            if (countResult != NativeResult.Success)
            {
                NativeErrorListener.Check(countResult, this, "hand_count");
            }

            for (int i = 0; i < handStates.Length; i++)
            {
                handStates[i].isTracked = false;
                handStates[i].pointerPoseValid = false;
                handStates[i].currentGesture = HandGesture.None;
            }

            bool rightHandUpdated = false;
            bool leftHandUpdated = false;

            for (int i = 0; i < hand_count; i++)
            {
                UInt64 hand_state_handle = 0;
                var handStateCreateResult = NativeApi.NRHandStateCreate(m_TrackingDataHandle, (UInt32)i, ref hand_state_handle);
                if (handStateCreateResult != NativeResult.Success)
                {
                    NativeErrorListener.Check(handStateCreateResult, this, "hand_state_handle");
                    continue;
                }

                bool isTracked = false;
                NativeApi.NRHandStateIsTracked(hand_state_handle, ref isTracked);

                NRHandType handType = NRHandType.NR_HAND_TYPE_UNKNOWN;
                NativeApi.NRHandStateGetHandType(hand_state_handle, ref handType);

                if (!isTracked)
                    continue;

                if (rightHandUpdated && leftHandUpdated)
                    continue;

                HandState handState = null;
                if (handType == NRHandType.NR_HAND_TYPE_RIGHT && !rightHandUpdated)
                {
                    handState = handStates[0];
                    rightHandUpdated = true;
                }
                else if(handType == NRHandType.NR_HAND_TYPE_LEFT && !leftHandUpdated)
                {
                    handState = handStates[1];
                    leftHandUpdated = true;
                }

                if (handState == null)
                    continue;

                NRGestureType gestureType = NRGestureType.NR_GESTURE_TYPE_UNKNOWN;
                NativeApi.NRHandStateGetGestureType(hand_state_handle, ref gestureType);

                UInt32 handJointCount = 0;
                NativeApi.NRHandStateGetHandJointCount(hand_state_handle, ref handJointCount);
                //NativeApi.NRHandStateGetConfidence(hand_state_handle, ref handState.confidence);

                handState.isTracked = true;
                handState.pointerPoseValid = true;
                handState.currentGesture = GetMappedGesture(gestureType);

                for (UInt32 j = 0; j < handJointCount; j++)
                {
                    UInt64 hand_joint_state_handle = 0;
                    if (NativeApi.NRHandJointStateCreate(hand_state_handle, j, ref hand_joint_state_handle) == NativeResult.Success)
                    {
                        NRHandJointType handJointType = NRHandJointType.NR_HAND_JOINT_TYPE_INVALID;
                        NativeApi.NRHandJointGetHandJointType(hand_joint_state_handle, ref handJointType);

                        NativeMat4f handJointPose = new NativeMat4f(Matrix4x4.identity);
                        NativeApi.NRHandJointGetHandJointPose(hand_joint_state_handle, ref handJointPose);

                        HandJointID handJointID;
                        if (m_JointMapping.TryGetValue(handJointType, out handJointID))
                        {
                            var pose = GetWorldPose(handJointPose);
                            SetHandJointPose(handState, handJointID, pose);
                        }
                    }
                }
            }
        }

        public bool Pause()
        {
            if (m_HandTrackingHandle == 0)
                return false;
            var result = NativeApi.NRHandTrackingPause(m_HandTrackingHandle);
            NativeErrorListener.Check(result, this, "Pause");
            return result == NativeResult.Success;
        }

        public bool Resume()
        {
            if (m_HandTrackingHandle == 0)
                return false;
            var result = NativeApi.NRHandTrackingResume(m_HandTrackingHandle);
            NativeErrorListener.Check(result, this, "Resume");
            return result == NativeResult.Success;
        }

        public bool Stop()
        {
            if (m_HandTrackingHandle == 0)
                return false;
            var result = NativeApi.NRHandTrackingStop(m_HandTrackingHandle);
            NativeErrorListener.Check(result, this, "Stop");
            return result == NativeResult.Success;
        }

        public bool Destroy()
        {
            if (m_HandTrackingHandle == 0)
                return false;
            var result = NativeApi.NRHandTrackingDestroy(m_HandTrackingHandle);
            NativeErrorListener.Check(result, this, "Destroy");
            return result == NativeResult.Success;
        }

        private bool TryUpdateHandTracking()
        {
            var result = NativeApi.NRHandTrackingUpdate(m_HandTrackingHandle);
            return result == NativeResult.Success;
        }

        private bool TryCreateTrackingDataHandle()
        {
            var result = NativeApi.NRHandTrackingDataCreate(m_HandTrackingHandle, ref m_TrackingDataHandle);
            return result == NativeResult.Success;
        }

        private bool TryAcquireHandTrackingDataHandle(UInt64 timestamp)
        {
            var result = NativeApi.NRAcquireHandTrackingData(m_HandTrackingHandle, timestamp, ref m_TrackingDataHandle);
            return result == NativeResult.Success;
        }

        private HandEnum GetMappedHandEnum(NRHandType handType)
        {
            switch (handType)
            {
                case NRHandType.NR_HAND_TYPE_LEFT:
                    return HandEnum.LeftHand;
                case NRHandType.NR_HAND_TYPE_RIGHT:
                    return HandEnum.RightHand;
                default:
                    break;
            }
            return HandEnum.None;
        }

        private HandGesture GetMappedGesture(NRGestureType gestureType)
        {
            switch (gestureType)
            {
                case NRGestureType.NR_GESTURE_TYPE_OPEN_HAND:
                    return HandGesture.OpenHand;
                case NRGestureType.NR_GESTURE_TYPE_GRAB:
                    return HandGesture.Grab;
                case NRGestureType.NR_GESTURE_TYPE_PINCH:
                    return HandGesture.Pinch;
                case NRGestureType.NR_GESTURE_TYPE_POINT:
                    return HandGesture.Point;
                case NRGestureType.NR_GESTURE_TYPE_VICTORY:
                    return HandGesture.Victory;
                case NRGestureType.NR_GESTURE_TYPE_CALL:
                    return HandGesture.Call;
                case NRGestureType.NR_GESTURE_TYPE_SYSTEM:
                    return HandGesture.System;
                default:
                    break;
            }
            return HandGesture.None;
        }

        private void SetHandJointPose(HandState handState, HandJointID jointID, Pose pose)
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

        private Pose GetWorldPose(NativeMat4f jointPose)
        {
            if (CameraRigTransform == null)
            {
                return Pose.identity;
            }
            Pose unitypose;
            ConversionUtility.ApiPoseToUnityPose(jointPose, out unitypose);
            return ConversionUtility.ApiWorldToUnityWorld(unitypose);
        }

        private partial struct NativeApi
        {
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHandTrackingCreate(ref UInt64 hand_tracking_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHandTrackingStart(UInt64 hand_tracking_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHandTrackingPause(UInt64 hand_tracking_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHandTrackingResume(UInt64 hand_tracking_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHandTrackingStop(UInt64 hand_tracking_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHandTrackingDestroy(UInt64 hand_tracking_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHandTrackingGetAvailableHandJoint(UInt64 hand_tracking_handle, ref UInt64 out_available_hand_joint_type_mask);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHandTrackingGetAvailableGestureType(UInt64 hand_tracking_handle, ref UInt64 out_available_gesture_type_mask);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHandTrackingGetSupportedFunctions(UInt64 hand_tracking_handle, ref UInt64 out_supported_function_mask);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHandTrackingUpdate(UInt64 hand_tracking_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHandTrackingDataCreate(UInt64 hand_tracking_handle, ref UInt64 out_hand_tracking_data_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRAcquireHandTrackingData(UInt64 hand_tracking_handle, UInt64 timestamp, ref UInt64 out_hand_tracking_data_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHandTrackingDataDestroy(UInt64 hand_tracking_data_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHandTrackingDataGetHMDTimeNanos(UInt64 hand_tracking_data_handle, ref UInt64 out_hmd_time_nanos);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHandTrackingDataGetHandsCount(UInt64 hand_tracking_data_handle, ref UInt32 out_hand_count);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHandStateCreate(UInt64 hand_tracking_data_handle, UInt32 hand_index, ref UInt64 out_hand_state_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHandStateDestroy(UInt64 hand_state_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHandStateGetHandType(UInt64 hand_state_handle, ref NRHandType out_hand_type);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHandStateGetGestureType(UInt64 hand_state_handle, ref NRGestureType out_gesture_type);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHandStateIsTracked(UInt64 hand_state_handle, ref bool out_is_tracked);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHandStateGetHandJointCount(UInt64 hand_state_handle, ref UInt32 out_hand_joint_count);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHandJointStateCreate(UInt64 hand_state_handle, UInt32 hand_joint_index, ref UInt64 out_hand_joint_state_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHandJointStateDestroy(UInt64 hand_joint_state_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHandJointGetHandJointType(UInt64 hand_joint_state_handle, ref NRHandJointType out_hand_joint_type);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHandJointGetHandJointPose(UInt64 hand_joint_state_handle, ref NativeMat4f out_hand_joint_pose);

            //[DllImport(NativeConstants.NRNativeLibrary)]
            //public static extern NativeResult NRHandStateGetConfidence(UInt64 hand_state_handle, ref float out_confidence);
        }
    }
}
