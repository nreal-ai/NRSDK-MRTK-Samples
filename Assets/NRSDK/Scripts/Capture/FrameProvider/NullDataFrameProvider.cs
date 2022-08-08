/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.Record
{
    using NRKernal;
    using UnityEngine;
    using System.Collections;

    /// <summary> An editor frame provider. </summary>
    public class NullDataFrameProvider : AbstractFrameProvider
    {
        protected UniversalTextureFrame m_DefaultFrame;
        private bool m_IsPlay = false;
        private int FPS;

        /// <summary> Default constructor. </summary>
        public NullDataFrameProvider(int fps)
        {
            FPS = fps;
            m_DefaultFrame = new UniversalTextureFrame();
            NRKernalUpdater.Instance.StartCoroutine(UpdateFrame());
        }

        /// <summary> Updates the frame. </summary>
        /// <returns> An IEnumerator. </returns>
        public IEnumerator UpdateFrame()
        {
            float timeInteval = 1f * 0.9f / FPS;
            float timeLast = 0f;
            while (true)
            {
                if (m_IsPlay)
                {
                    if (timeLast >= timeInteval)
                    {
                        m_DefaultFrame.timeStamp = NRSessionManager.Instance.TrackingSubSystem.GetHMDTimeNanos();
                        OnUpdate?.Invoke(m_DefaultFrame);
                        m_IsFrameReady = true;
                        timeLast = 0;
                    }
                }
                yield return new WaitForEndOfFrame();
                timeLast += Time.unscaledDeltaTime;
            }
        }

        /// <summary> Plays this object. </summary>
        public override void Play()
        {
            m_IsPlay = true;
        }

        /// <summary> Stops this object. </summary>
        public override void Stop()
        {
            m_IsPlay = false;
        }

        /// <summary> Releases this object. </summary>
        public override void Release()
        {
            m_IsPlay = false;
            NRKernalUpdater.Instance?.StopCoroutine(UpdateFrame());
        }
    }
}
