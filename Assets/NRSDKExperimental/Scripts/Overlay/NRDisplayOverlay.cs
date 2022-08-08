/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.Experimental
{
    using System;
    using UnityEngine;

    [ScriptOrder(NativeConstants.NRRENDER_ORDER)]
    [RequireComponent(typeof(Camera))]
    public class NRDisplayOverlay : OverlayBase
    {
        /// <summary>
        /// Which display this overlay should render to.
        /// </summary>
        [Tooltip("Which display this overlay should render to.")]
        public NativeDevice targetEye;

        private Camera m_RenderCamera;
        private int m_EyeWidth;
        private int m_EyeHeight;

        private bool m_IsTrackingModeChanging = false;
        private RenderTexture m_HijackRenderTextureLeft = null;
        private RenderTexture m_HijackRenderTextureRight = null;

        protected override void Initialize()
        {
            base.Initialize();

            m_RenderCamera = gameObject.GetComponent<Camera>();

#if !UNITY_EDITOR
            NRDevice.Instance.Init();
            var resolution = NRFrame.GetDeviceResolution(NativeDevice.LEFT_DISPLAY);
            m_EyeWidth = resolution.width;
            m_EyeHeight = resolution.height;
#else
            m_EyeWidth = 1920;
            m_EyeHeight = 1080;
#endif
            this.compositionDepth = int.MaxValue;
            m_BufferSpec.size = new NativeResolution((int)m_EyeWidth, (int)m_EyeHeight);
            m_BufferSpec.colorFormat = NRColorFormat.NR_COLOR_FORMAT_RGBA_8888;
            m_BufferSpec.depthFormat = NRDepthStencilFormat.NR_DEPTH_STENCIL_FORMAT_DEPTH_24;
            m_BufferSpec.samples = 1;
            m_BufferSpec.useExternalSurface = false;

            NRSessionManager.Instance.TrackingLostListener.OnTrackStateChanged -= OnTrackStateChanged;
            NRSessionManager.Instance.TrackingLostListener.OnTrackStateChanged += OnTrackStateChanged;
        }

        private void OnTrackStateChanged(bool trackChanging, RenderTexture leftRT, RenderTexture rightRT)
        {
            if (trackChanging)
            {
                m_IsTrackingModeChanging = true;
                m_HijackRenderTextureLeft = leftRT;
                m_HijackRenderTextureRight = rightRT;
                m_RenderCamera.targetTexture = null;
                SetDirty(true);
            }
            else
            {
                m_IsTrackingModeChanging = false;
                m_HijackRenderTextureLeft = null;
                m_HijackRenderTextureRight = null;
                SetDirty(true);
            }
        }

        public override void SwapBuffers(IntPtr bufferHandler)
        {
            if (!Textures.ContainsKey(bufferHandler))
            {
                NRDebugger.Error("[NRDisplayOverlay] Can not find the texture:" + bufferHandler);
                return;
            }

            Texture targetTexture;
            Textures.TryGetValue(bufferHandler, out targetTexture);
            RenderTexture renderTexture = targetTexture as RenderTexture;
            if (renderTexture == null)
            {
                NRDebugger.Error("[NRDisplayOverlay] The texture is null...");
                return;
            }
            if (m_IsTrackingModeChanging)
            {
                var srcTexture = (targetEye == NativeDevice.LEFT_DISPLAY) ? m_HijackRenderTextureLeft : m_HijackRenderTextureRight;
                Graphics.CopyTexture(srcTexture, renderTexture);
            }
            else
            {
                m_RenderCamera.targetTexture = renderTexture;
            }
        }

        public override void CreateViewport()
        {
            m_ViewPorts = new ViewPort[1];
            m_ViewPorts[0].reprojection = m_IsTrackingModeChanging ? NRReprojectionType.NR_REPROJECTION_TYPE_NONE : NRReprojectionType.NR_REPROJECTION_TYPE_FULL;
            m_ViewPorts[0].sourceUV = new Rect(0, 0, 1, 1);
            m_ViewPorts[0].targetDisplay = targetEye == NativeDevice.LEFT_DISPLAY ? LayerSide.Left : LayerSide.Right;
            m_ViewPorts[0].layerID = m_LayerId;
            NRFrame.GetEyeFov(targetEye, ref m_ViewPorts[0].fov);
            m_ViewPorts[0].is3DLayer = true;
            NRSwapChainManager.Instance.CreateBufferViewport(ref m_ViewPorts[0]);
        }

        public override void DestroyViewPort()
        {
            base.DestroyViewPort();
            if (m_ViewPorts != null)
            {
                var viewport = m_ViewPorts[0];
                NRSwapChainManager.Instance.DestroyBufferViewPort(viewport.nativeHandler);
                m_ViewPorts = null;
            }
        }

        public override void UpdateViewPort()
        {
            if (m_ViewPorts == null)
            {
                NRDebugger.Warning("Can not update view port for this layer:{0}", gameObject.name);
                return;
            }

            NRSwapChainManager.Instance.UpdateBufferViewport(ref m_ViewPorts[0]);
        }

        public override void CreateOverlayTextures()
        {
            NRDebugger.Info("[NRDisplayOverlay] Create DefaultEyeTextures. ");
            ReleaseOverlayTextures();
            for (int i = 0; i < m_BufferSpec.bufferCount; i++)
            {
                RenderTexture texture = UnityExtendedUtility.CreateRenderTexture((int)m_EyeWidth, (int)m_EyeHeight, 24, RenderTextureFormat.ARGB32);
                IntPtr texturePtr = texture.GetNativeTexturePtr();
                Textures.Add(texturePtr, texture);
            }
        }

        public override void ReleaseOverlayTextures()
        {
            if (Textures.Count == 0)
            {
                return;
            }

            NRDebugger.Info("[NRDisplayOverlay] ReleaseOverlayTextures. ");
            foreach (var item in Textures)
            {
                RenderTexture rt = item.Value as RenderTexture;
                if (rt != null) rt.Release();
            }

            Textures.Clear();
        }

        new void OnDestroy()
        {
            base.OnDestroy();
            if (NRSessionManager.Instance.TrackingLostListener != null)
                NRSessionManager.Instance.TrackingLostListener.OnTrackStateChanged -= OnTrackStateChanged;
        }
    }
}
