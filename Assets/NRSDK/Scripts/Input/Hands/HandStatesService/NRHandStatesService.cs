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
    public class NRHandStatesService : IHandStatesService
    {
        private NativeHandTracking m_NativeHandTracking;

        public bool IsRunning { private set; get; }

        public bool RunService()
        {
            if (m_NativeHandTracking == null)
            {
                m_NativeHandTracking = new NativeHandTracking();
            }
            bool success = m_NativeHandTracking.Start();
            if (success)
            {
                IsRunning = true;
            }
            return success;
        }

        public bool StopService()
        {
            if (m_NativeHandTracking != null)
            {
                bool success = m_NativeHandTracking.Stop();
                if (success)
                {
                    IsRunning = false;
                }
                return success;
            }
            return false;
        }

        public void UpdateStates(HandState[] handStates)
        {
            if (m_NativeHandTracking != null)
            {
                m_NativeHandTracking.Update(handStates);
            }
        }

        public void PauseService()
        {
            if (m_NativeHandTracking != null)
            {
                bool success = m_NativeHandTracking.Pause();
                if (success)
                {
                    IsRunning = false;
                }
            }
        }

        public void ResumeService()
        {
            if (m_NativeHandTracking != null)
            {
                bool success = m_NativeHandTracking.Resume();
                if (success)
                {
                    IsRunning = true;
                }
            }
        }

        public void DestroyService()
        {
            if (m_NativeHandTracking != null)
            {
                bool success = m_NativeHandTracking.Destroy();
                if (success)
                {
                    IsRunning = false;
                    m_NativeHandTracking = null;
                }
            }
        }
    }
}
