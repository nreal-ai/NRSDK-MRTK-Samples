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
    using System.Collections;
    using UnityEngine;

    public class NRTrackingModeChangedListener
    {
        public delegate void OnTrackStateChangedDel(bool trackChanging, RenderTexture leftRT, RenderTexture rightRT);
        public event OnTrackStateChangedDel OnTrackStateChanged;
        private NRTrackingModeChangedTip m_LostTrackingTip;
        private Coroutine m_EnableRenderCamera;
        private Coroutine m_DisableRenderCamera;
        private const float MinTimeLastLimited = 0;
        private const float MaxTimeLastLimited = 6f;

        public NRTrackingModeChangedListener()
        {
            NRHMDPoseTracker.OnChangeTrackingMode += OnChangeTrackingMode;
        }

        private void OnChangeTrackingMode(NRHMDPoseTracker.TrackingType origin, NRHMDPoseTracker.TrackingType target)
        {
            NRDebugger.Info("[NRTrackingModeChangedListener] OnChangeTrackingMode origin:{0} target:{1}", origin, target);
            if (target == NRHMDPoseTracker.TrackingType.Tracking0Dof || target == NRHMDPoseTracker.TrackingType.Tracking0DofStable)
            {
                return;
            }
            if (m_EnableRenderCamera != null)
            {
                NRKernalUpdater.Instance.StopCoroutine(m_EnableRenderCamera);
                m_EnableRenderCamera = null;
            }
            m_EnableRenderCamera = NRKernalUpdater.Instance.StartCoroutine(EnableTrackingInitializingRenderCamera());
        }

        public IEnumerator EnableTrackingInitializingRenderCamera()
        {
            if (m_LostTrackingTip == null)
            {
                m_LostTrackingTip = NRTrackingModeChangedTip.Create();
            }
            m_LostTrackingTip.gameObject.SetActive(true);
            var reason = NRFrame.LostTrackingReason;
            m_LostTrackingTip.SetMessage(NativeConstants.TRACKING_MODE_SWITCH_TIP);

            float begin_time = Time.realtimeSinceStartup;
            var endofFrame = new WaitForEndOfFrame();
            yield return endofFrame;
            yield return endofFrame;
            yield return endofFrame;
            NRDebugger.Info("[NRTrackingModeChangedListener] Enter tracking initialize mode...");
            OnTrackStateChanged?.Invoke(true, m_LostTrackingTip.LeftRT, m_LostTrackingTip.RightRT);

            NRHMDPoseTracker postTracker = NRSessionManager.Instance.NRHMDPoseTracker;
            while ((NRFrame.LostTrackingReason != LostTrackingReason.NONE || postTracker.IsTrackModeChanging || (Time.realtimeSinceStartup - begin_time) < MinTimeLastLimited)
                && (Time.realtimeSinceStartup - begin_time) < MaxTimeLastLimited)
            {
                NRDebugger.Info("[NRTrackingModeChangedListener] Wait for tracking: modeChanging={0}, lostTrackReason={1}",
                    postTracker.IsTrackModeChanging, NRFrame.LostTrackingReason);
                yield return endofFrame;
            }

            if (m_DisableRenderCamera == null)
            {
                m_DisableRenderCamera = NRKernalUpdater.Instance.StartCoroutine(DisableTrackingInitializingRenderCamera());
            }
            yield return m_DisableRenderCamera;
            m_DisableRenderCamera = null;
        }

        public IEnumerator DisableTrackingInitializingRenderCamera()
        {
            if (m_LostTrackingTip != null)
            {
                m_LostTrackingTip.gameObject.SetActive(false);
            }
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            OnTrackStateChanged?.Invoke(false, m_LostTrackingTip.LeftRT, m_LostTrackingTip.RightRT);
            NRDebugger.Info("[NRTrackingModeChangedListener] Exit tracking initialize mode...");
        }
    }
}
