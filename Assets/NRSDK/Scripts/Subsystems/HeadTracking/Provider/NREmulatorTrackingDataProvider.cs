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
    using UnityEngine;

#if UNITY_EDITOR
    public class NREmulatorTrackingDataProvider : ITrackingDataProvider
    {
        private NREmulatorHeadPose m_NREmulatorHeadPose;

        public NREmulatorTrackingDataProvider() { }

        public void Start()
        {
            MainThreadDispather.QueueOnMainThread(() =>
            {
                if (!NREmulatorManager.Inited && !GameObject.Find("NREmulatorManager"))
                {
                    NREmulatorManager.Inited = true;
                    GameObject.Instantiate(Resources.Load("Prefabs/NREmulatorManager"));
                }
                if (!GameObject.Find("NREmulatorHeadPos"))
                {
                    m_NREmulatorHeadPose = GameObject.Instantiate<NREmulatorHeadPose>(
                        Resources.Load<NREmulatorHeadPose>("Prefabs/NREmulatorHeadPose")
                    );
                }
            });
        }

        public void Pause() { }

        public void Resume() { }

        public void Stop() { }

        public bool GetFramePresentHeadPose(ref Pose pose, ref LostTrackingReason lostReason, ref ulong timestamp)
        {
            if (m_NREmulatorHeadPose == null)
            {
                return false;
            }
            pose = m_NREmulatorHeadPose.headPose;
            lostReason = LostTrackingReason.NONE;
            timestamp = NRTools.GetTimeStamp();
            return true;
        }

        public bool GetHeadPose(ref Pose pose, ulong timestamp)
        {
            if (m_NREmulatorHeadPose == null)
            {
                return false;
            }
            pose = m_NREmulatorHeadPose.headPose;
            return true;
        }

        public bool InitTrackingMode(TrackingMode mode)
        {
            return true;
        }

        public bool SwitchTrackingMode(TrackingMode mode)
        {
            return true;
        }

        public void Recenter() { }
		
        public bool GetFramePresentTimeByCount(int count, ref ulong timestamp)
        {
            timestamp = NRTools.GetTimeStamp();
			return true;
		}
		
        public ulong GetHMDTimeNanos()
        {
            return 0;
        }
    }
#endif
}
