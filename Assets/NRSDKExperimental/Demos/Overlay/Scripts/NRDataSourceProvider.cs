/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/         
* 
*****************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace NRKernal.Experimental.NRExamples
{
    public interface IPoseProvider
    {
        Int64 getRecommendedTimeNanos();
        float[] getHeadPose(Int64 nanos);
        float[] getEyePoseFromHead(int eye);
        float[] getEyeFov(int eye);
        byte[] getControllerData();
    }

    public class NRDataSourceProvider : IPoseProvider
    {
        private WebXRController controllerInfo;
        private byte[] rawData;

        public struct PoseInfo
        {
            public Int64 timestamp;
            public Pose pose;
        }
        public Queue<PoseInfo> m_CachePose;
        private const int MaxCacheCount = 10;

        public bool GetCachePoseByTime(Int64 timestamp, ref Pose pose)
        {
            while (m_CachePose.Count != 0)
            {
                var item = m_CachePose.Dequeue();
                if (item.timestamp == timestamp)
                {
                    pose.position = item.pose.position;
                    pose.rotation = item.pose.rotation;
                    return true;
                }
            }
            NRDebugger.Info("[NRDataSourceProvider] Missing cache pose:" + timestamp);
            return false;
        }

        public NRDataSourceProvider()
        {
            controllerInfo = new WebXRController();
            m_CachePose = new Queue<PoseInfo>();
        }

        public float[] getEyeFov(int eye)
        {
            NativeFov4f fov = new NativeFov4f();
            NRFrame.GetEyeFov((NativeDevice)eye, ref fov);
            return fov.ToXRFloats();
        }

        public float[] getEyePoseFromHead(int eye)
        {
            var eyePoseFromHead = NRFrame.EyePoseFromHead;
            if (eye == (int)NativeDevice.LEFT_DISPLAY)
            {
                return PoseToFloatArray(eyePoseFromHead.LEyePose);
            }
            else
            {
                return PoseToFloatArray(eyePoseFromHead.REyePose);
            }
        }

        public float[] getHeadPose(Int64 nanos)
        {
            Pose pose = Pose.identity;
            NRFrame.GetHeadPoseByTime(ref pose, (UInt64)nanos);
            if (m_CachePose.Count > MaxCacheCount)
            {
                m_CachePose.Dequeue();
            }
            m_CachePose.Enqueue(new PoseInfo()
            {
                timestamp = nanos,
                pose = pose
            });
            return PoseToFloatArray(pose);
        }

        private static float[] PoseToFloatArray(Pose pose)
        {
            NativeMat4f matrix = NativeMat4f.identity;
            ConversionUtility.UnityPoseToApiPose(pose, out matrix);
            return matrix.ToFloats();
        }

        public Int64 getRecommendedTimeNanos()
        {
            ulong timestamp = 0;
            if (NRFrame.SessionStatus == SessionState.Running)
            {
                NRSessionManager.Instance.TrackingSubSystem.GetFramePresentTimeByCount(2, ref timestamp);
            }
            return (Int64)timestamp;
        }

        public byte[] getControllerData()
        {
            var handControllerAnchor = NRInput.DomainHand == ControllerHandEnum.Left ? ControllerAnchorEnum.LeftLaserAnchor : ControllerAnchorEnum.RightLaserAnchor;
            Transform laserAnchor = NRInput.AnchorsHelper.GetAnchor(NRInput.RaycastMode == RaycastModeEnum.Gaze ? ControllerAnchorEnum.GazePoseTrackerAnchor : handControllerAnchor);

            controllerInfo.UpdateData(
                true,
                NRInput.DomainHand == ControllerHandEnum.Left,
                laserAnchor.transform.position,
                laserAnchor.transform.rotation,
                NRInput.GetTouch(),
                NRVirtualDisplayer.SystemButtonState
            );
            return controllerInfo.Serialize();
        }
    }
}
