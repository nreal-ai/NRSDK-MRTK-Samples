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

    public enum HandEnum
    {
        None = -1,
        RightHand,
        LeftHand
    }

    public enum HandGesture
    {
        None = 0,
        OpenHand,
        Point,
        Grab,
        Victory
    }

    public enum HandJointID
    {
        Invalid = -1,
        Wrist = 0,
        Palm,
        ThumbMetacarpal,
        ThumbProximal,
        ThumbDistal,
        ThumbTip,
        IndexProximal,
        IndexMiddle,
        IndexDistal,
        IndexTip,
        MiddleProximal,
        MiddleMiddle,
        MiddleDistal,
        MiddleTip,
        RingProximal,
        RingMiddle,
        RingDistal,
        RingTip,
        PinkyMetacarpal,
        PinkyProximal,
        PinkyMiddle,
        PinkyDistal,
        PinkyTip,

        Max = PinkyTip + 1
    }

    /// <summary> Contains the details of current hand tracking info of left/right hand. </summary>
    public class HandState
    {
        public readonly HandEnum handEnum;
        public bool isTracked;
        public Pose pointerPose;
        public bool pointerPoseValid;
        public bool isPinching;
        public float pinchStrength;
        public HandGesture currentGesture;
        public readonly Dictionary<HandJointID, Pose> jointsPoseDict = new Dictionary<HandJointID, Pose>();

        public HandState(HandEnum handEnum)
        {
            this.handEnum = handEnum;
            Reset();
        }

        /// <summary> Reset the hand state to default. </summary>
        public void Reset()
        {
            isTracked = false;
            pointerPose = Pose.identity;
            pointerPoseValid = false;
            isPinching = false;
            pinchStrength = 0f;
            currentGesture = HandGesture.None;
            jointsPoseDict.Clear();
        }

        /// <summary>
        /// Returns the pose of the hand joint ID of this hand state.
        /// </summary>
        /// <param name="handJointID"></param>
        /// <returns></returns>
        public Pose GetJointPose(HandJointID handJointID)
        {
            Pose pose = Pose.identity;
            jointsPoseDict.TryGetValue(handJointID, out pose);
            return pose;
        }
    }
}
