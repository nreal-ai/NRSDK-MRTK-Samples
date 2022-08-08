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
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using UnityEngine.Assertions;
    using System.Linq;

    public enum LayerTextureType
    {
        RenderTexture,
        StandardTexture,
        EglTexture
    }

    public enum DisplayType
    {
        Left = NativeDevice.LEFT_DISPLAY,
        Right = NativeDevice.RIGHT_DISPLAY
    }

    public struct BufferSpec
    {
        public NativeResolution size;
        public NRColorFormat colorFormat;
        public NRDepthStencilFormat depthFormat;
        public bool useExternalSurface;
        public int surfaceFlag;
        public int samples;
        // Number of an arrar texture, default is 1 means a non-layerd texture.
        public int bufferCount;

        public void Copy(BufferSpec bufferspec)
        {
            this.size = bufferspec.size;
            this.colorFormat = bufferspec.colorFormat;
            this.depthFormat = bufferspec.depthFormat;
            this.samples = bufferspec.samples;
            this.bufferCount = bufferspec.bufferCount;
            this.surfaceFlag = bufferspec.surfaceFlag;
            this.useExternalSurface = bufferspec.useExternalSurface;
        }

        public override string ToString()
        {
            return string.Format("[{0} bufferCount:{1} useExternalSurface:{2} surfaceFlag:{3}]"
                , size.ToString(), bufferCount, useExternalSurface, surfaceFlag);
        }
    }

    public enum LayerSide
    {
        Left = 0,
        Right = 1,
        Both = 2,
        [HideInInspector]
        Count = Both
    };

    public struct ViewPort
    {
        public int layerID;
        public int index;
        public Rect sourceUV;
        public LayerSide targetDisplay;
        public Matrix4x4 transform;
        public NativeFov4f fov;
        public NRReprojectionType reprojection;
        public bool is3DLayer;
        public UInt64 nativeHandler;

        public override string ToString()
        {
            return string.Format("[layerID:{0} targetEye:{1} reprojection:{2} index:{3}]", layerID, targetDisplay, reprojection, index);
        }
    };

    public class NRSwapChainManager : SingleTon<NRSwapChainManager>, NRKernal.IFrameProcessor
    {
        public static List<OverlayBase> Overlays = new List<OverlayBase>();
        private NativeRenderring m_NativeRenderring;
        private NativeSwapchain NativeSwapchain { get; set; }
        private UInt64 m_SwapchainHandle = 0;
        private UInt64 m_ViewportListHandle = 0;
        private UInt64 m_FrameHandle = 0;
        private Dictionary<int, IntPtr> m_LayerBuffersDict;
        private Queue<TaskInfo> m_TaskQueue;

        /// <summary>
        /// Max overlay count is MaxOverlayCount-2, the two overlay are left display and right display.
        /// </summary>
        private const int MaxOverlayCount = 7;

        public struct TaskInfo
        {
            public Action<OverlayBase> callback;
            public OverlayBase obj;
        }

        public UInt64 GetFrameHandle()
        {
            return m_FrameHandle;
        }

        private bool IsMultiThread
        {
            get
            {
                if (NRSessionManager.Instance.NRSessionBehaviour != null)
                {
                    return NRSessionManager.Instance.NRSessionBehaviour.SessionConfig.UseMultiThread;
                }
                else
                {
                    return true;
                }
            }
        }

        public bool IsInitialized { get; private set; }

        private AndroidJavaObject m_ProtectedCodec;
        protected AndroidJavaObject ProtectedCodec
        {
            get
            {
                if (m_ProtectedCodec == null)
                {
                    m_ProtectedCodec = new AndroidJavaObject("ai.nreal.protect.session.ProtectSession");
                }

                return m_ProtectedCodec;
            }
        }

        public NRSwapChainManager()
        {
            m_LayerBuffersDict = new Dictionary<int, IntPtr>();
            m_TaskQueue = new Queue<TaskInfo>();

            if (Application.isPlaying)
            {
                InitDisplayOverlay();

                var nrRenderer = NRSessionManager.Instance.NRRenderer;
                if (nrRenderer != null)
                    nrRenderer.SetFrameProcessor(this);
            }
        }

        // void OnSessionStateChanged(SessionState state)
        void InitDisplayOverlay()
        {
            NRDebugger.Info("[SwapChain] InitDisplayOverlay");
            var leftCamera = NRSessionManager.Instance.NRHMDPoseTracker.leftCamera;
            var lOverlay = leftCamera.gameObject.GetComponent<NRDisplayOverlay>();
            if (lOverlay == null)
            {
                lOverlay = leftCamera.gameObject.AddComponent<NRDisplayOverlay>();
                lOverlay.targetEye = NativeDevice.LEFT_DISPLAY;
            }

            var rightCamera = NRSessionManager.Instance.NRHMDPoseTracker.rightCamera;
            var rOverlay = rightCamera.gameObject.GetComponent<NRDisplayOverlay>();
            if (rOverlay == null)
            {
                rOverlay = rightCamera.gameObject.AddComponent<NRDisplayOverlay>();
                rOverlay.targetEye = NativeDevice.RIGHT_DISPLAY;
            }
        }

        public void Initialize(NativeRenderring nativeRenderring)
        {
            if (IsInitialized)
            {
                return;
            }

            NRDebugger.Info("[SwapChain] Initialize");
            m_NativeRenderring = nativeRenderring;
            NativeSwapchain = new NativeSwapchain(m_NativeRenderring);
#if !UNITY_EDITOR
            m_SwapchainHandle = NativeSwapchain.CreateSwapChain();
#endif
            IsInitialized = true;
        }

        public void Update()
        {
            if (!IsReady())
            {
                return;
            }

            if (m_TaskQueue.Count != 0)
            {
                while (m_TaskQueue.Count != 0)
                {
                    var task = m_TaskQueue.Dequeue();
                    task.callback.Invoke(task.obj);
                }
            }

            if (Overlays.Count != 0)
            {
                UpdateBufferViewportList();
#if !UNITY_EDITOR
                SwapDisplayOverlaysRenderBuffers();
#endif
            }
        }

        #region common functions
        private bool IsReady()
        {
#if UNITY_EDITOR
            return IsInitialized;
#else
            return IsInitialized && NRFrame.SessionStatus == SessionState.Running && NRSessionManager.Instance.NRRenderer.CurrentState == NRRenderer.RendererState.Running;
#endif
        }

        public void Add(OverlayBase overlay)
        {
            if (Overlays.Contains(overlay))
            {
                NRDebugger.Warning("[SwapChain] There is no this overlay:" + overlay.LayerId);
                return;
            }

            if (Overlays.Count == MaxOverlayCount)
            {
                throw new NotSupportedException("The current count of overlays exceeds the maximum!");
            }

            if (IsReady())
            {
                AddLayer(overlay);
            }
            else
            {
                m_TaskQueue.Enqueue(new TaskInfo()
                {
                    callback = AddLayer,
                    obj = overlay
                });
            }
        }

        public void Remove(OverlayBase overlay)
        {
            if (!Overlays.Contains(overlay))
            {
                return;
            }

            if (IsReady())
            {
                RemoveLayer(overlay);
            }
            else
            {
                m_TaskQueue.Enqueue(new TaskInfo()
                {
                    callback = RemoveLayer,
                    obj = overlay
                });
            }
        }

        private void AddLayer(OverlayBase overlay)
        {
            Overlays.Add(overlay);
            Overlays.Sort();

            BufferSpec bufferSpec = overlay.BufferSpec;
            bufferSpec.bufferCount = GetRecommandBufferCount(m_SwapchainHandle, IsMultiThread, overlay);
            overlay.BufferSpec = bufferSpec;

            NRDebugger.Info("[SwapChain] Add layer name:{0} buffer spec:{1}", overlay.gameObject.name, bufferSpec.ToString());

#if !UNITY_EDITOR
            overlay.NativeSpecHandler = NativeSwapchain.CreateBufferSpec(overlay.BufferSpec);
            overlay.LayerId = NativeSwapchain.CreateLayer(m_SwapchainHandle, overlay.NativeSpecHandler, bufferSpec.bufferCount);
#endif
            overlay.CreateOverlayTextures();
            if (overlay.Textures != null && overlay.Textures.Count != 0)
            {
#if !UNITY_EDITOR
                NativeSwapchain.SetLayerBuffer(m_SwapchainHandle, overlay.LayerId, overlay.Textures.Keys.ToArray());
#endif
            }

            overlay.CreateViewport();
            UpdateProtectContentSetting();
        }

        private void RemoveLayer(OverlayBase overlay)
        {
            NRDebugger.Info("[SwapChain] Remove layer:" + overlay.gameObject.name);
            overlay.ReleaseOverlayTextures();

#if !UNITY_EDITOR
            NativeSwapchain.DestroyBufferSpec(overlay.NativeSpecHandler);
            var viewports = overlay.ViewPorts;
            for (int i = 0; i < viewports.Length; i++)
            {
                if (viewports[i].nativeHandler != 0)
                {
                    NativeSwapchain.DestroyBufferViewport(viewports[i].nativeHandler);
                }
            }
            NativeSwapchain.DestroyLayer(m_SwapchainHandle, overlay.LayerId);
#endif

            Overlays.Remove(overlay);
            UpdateOverlayViewPortIndex();
            UpdateProtectContentSetting();
        }

        private bool useProtectContent = false;
        private void UpdateProtectContentSetting()
        {
            bool flag = false;
            for (int i = 0; i < Overlays.Count; i++)
            {
                var overlay = Overlays[i];
                if (overlay is NROverlay && ((NROverlay)overlay).isProtectedContent)
                {
                    flag = true;
                    break;
                }
            }

            if (flag != useProtectContent)
            {
                NRDebugger.Info("[SwapChain] Protect content setting changed.");
                try
                {
                    AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    var unityActivity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                    if (flag)
                    {
                        NRDebugger.Info("[SwapChain] Use protect content.");
                        ProtectedCodec.Call("start", unityActivity);
                    }
                    else
                    {
                        NRDebugger.Info("[SwapChain] Use un-protect content.");
                        ProtectedCodec.Call("stop", unityActivity);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }

                useProtectContent = flag;
            }
        }

        private void PrintOverlayInfo()
        {
            System.Text.StringBuilder st = new System.Text.StringBuilder();
            foreach (var overlay in Overlays)
            {
                if (overlay.IsActive)
                {
                    st.Append(string.Format("[Overlay] {0}\n", overlay.ToString()));
                }
            }

            NRDebugger.Info(st.ToString());
        }

        private void UpdateOverlayViewPortIndex()
        {
            int index = 0;
            foreach (var overlay in Overlays)
            {
                if (overlay.IsActive)
                {
                    var viewports = overlay.ViewPorts;
                    Assert.IsTrue(viewports != null && viewports.Length >= 1);
                    int count = viewports.Length;
                    for (int i = 0; i < count; i++)
                    {
                        viewports[i].index = index;
                        index++;
                    }
                }
            }
            PrintOverlayInfo();
        }

        private int GetViewportsCount()
        {
            int index = 0;
            foreach (var overlay in Overlays)
            {
                if (overlay.IsActive)
                {
                    var viewports = overlay.ViewPorts;
                    Assert.IsTrue(viewports != null && viewports.Length >= 1);
                    int count = viewports.Length;
                    for (int i = 0; i < count; i++)
                    {
                        index++;
                    }
                }
            }
            return index;
        }

        private int GetRecommandBufferCount(UInt64 swapchainhandler, bool usemultithread, OverlayBase overlay)
        {
            int flag = 0;
            if (overlay is NRDisplayOverlay)
            {
                flag = (int)NRSwapchainFlags.NR_SWAPCHAIN_LAYER_DYNAMIC_TEXTURE;
            }
            else if(overlay is NROverlay)
            {
                NROverlay layer = overlay as NROverlay;
                if (!layer.isExternalSurface && !layer.isDynamic)
                {
                    return 1;
                }

                if (layer.isExternalSurface)
                    flag = (int)NRSwapchainFlags.NR_SWAPCHAIN_EXTERNAL_SURFACE;
                else if (layer.isDynamic)
                    flag = (int)NRSwapchainFlags.NR_SWAPCHAIN_LAYER_DYNAMIC_TEXTURE;
            }

            if (usemultithread)
            {
                flag ^= (int)NRSwapchainFlags.NR_SWAPCHAIN_MULTI_THREAD_RENDERING;
            }

#if !UNITY_EDITOR
            return NativeSwapchain.GetRecommandBufferCount(swapchainhandler, flag);
#else
            return 0;
#endif
        }

        public IntPtr GetSurfaceHandler(int layerid)
        {
            if (m_SwapchainHandle == 0)
            {
                NRDebugger.Error("[SwapChain] GetSurfaceHandler error, swapchainHandle:{0} layerid:{1}", m_SwapchainHandle, layerid);
                return IntPtr.Zero;
            }
#if !UNITY_EDITOR
            return NativeSwapchain.GetSurface(m_SwapchainHandle, layerid);
#else
            return IntPtr.Zero;
#endif
        }
        #endregion

        #region viewport
        private void UpdateBufferViewportList()
        {
            // The order of layer composition is the same as that of viewport, rendering from front to back
#if !UNITY_EDITOR
            if (m_ViewportListHandle == 0)
            {
                //NRDebugger.Info("[SwapChain] Create native buffer viewport list");
                m_ViewportListHandle = NativeSwapchain.CreateBufferViewportList();
            }
            else
            {
                int viewPortCount = GetViewportsCount();
                int nativeViewPortCount = NativeSwapchain.GetBufferViewportListSize(m_ViewportListHandle);
                if (viewPortCount != nativeViewPortCount)
                {
                    NRDebugger.Info("[SwapChain] Recreate list,viewPortCount:{0} nativeViewPortCount:{1}", viewPortCount, nativeViewPortCount);
                    NativeSwapchain.DestroyBufferViewportList(m_ViewportListHandle);
                    m_ViewportListHandle = NativeSwapchain.CreateBufferViewportList();
                    UpdateOverlayViewPortIndex();
                }
            }
#endif

            for (int i = 0; i < Overlays.Count; ++i)
            {
                if (Overlays[i].IsActive)
                {
                    Overlays[i].UpdateViewPort();
                }
            }
        }

        public void CreateBufferViewport(ref ViewPort viewport)
        {
#if !UNITY_EDITOR
            UInt64 viewPortHandler = 0;
            var targetDisplay = viewport.targetDisplay == LayerSide.Left ? DisplayType.Left : DisplayType.Right;
            if (viewport.is3DLayer)
            {
                viewPortHandler = NativeSwapchain.CreateSourceFovBufferViewport(viewport.layerID,
                    viewport.reprojection, viewport.sourceUV, targetDisplay, viewport.fov);
            }
            else
            {
                viewPortHandler = NativeSwapchain.CreateBufferViewport(viewport.layerID, viewport.reprojection,
                    ref viewport.sourceUV, ref viewport.transform, targetDisplay);
            }
            viewport.nativeHandler = viewPortHandler;
#endif
            //NRDebugger.Info("[SwapChain] Create buffer view port:{0}", viewport.ToString());
            UpdateOverlayViewPortIndex();
        }


        /// <summary>
        /// Sync viewport setting with native.
        /// </summary>
        public void UpdateBufferViewport(ref ViewPort viewport)
        {
            if (viewport.index == -1)
            {
                NRDebugger.Error("[SwapChain] UpdateBufferViewport index error:" + viewport.ToString());
                return;
            }

            //NRDebugger.Info("[SwapChain] UpdateBufferViewport:{0}", viewport.ToString());
            var targetDisplay = viewport.targetDisplay == LayerSide.Left ? DisplayType.Left : DisplayType.Right;
#if !UNITY_EDITOR
            if (viewport.is3DLayer)
            {
                NativeSwapchain.UpdateSourceFovBufferViewport(viewport.nativeHandler, viewport.layerID, viewport.reprojection,
                    ref viewport.sourceUV, targetDisplay, viewport.fov);
            }
            else
            {
                NativeSwapchain.UpdateBufferViewportData(viewport.nativeHandler, viewport.layerID, viewport.reprojection,
                    ref viewport.sourceUV, ref viewport.transform, targetDisplay);
            }
#endif
            NativeSwapchain.SetBufferViewPort(m_ViewportListHandle, viewport.index, viewport.nativeHandler);
        }

        public void DestroyBufferViewPort(UInt64 viewportHandle)
        {
            NativeSwapchain.DestroyBufferViewPort(viewportHandle);
            UpdateOverlayViewPortIndex();
        }
        #endregion

        #region submit frame
        private int[] GetDynamicLayerIds()
        {
            List<int> ids = new List<int>();
            for (int i = 0; i < Overlays.Count; i++)
            {
                if (Overlays[i] is NRDisplayOverlay)
                {
                    ids.Add(Overlays[i].LayerId);
                }
                else if (Overlays[i] is NROverlay && ((NROverlay)Overlays[i]).isDynamic)
                {
                    ids.Add(Overlays[i].LayerId);
                }
            }
            return ids.ToArray();
        }

        /// <summary>
        /// Swap display overlays render buffers.
        /// </summary>
        private void SwapDisplayOverlaysRenderBuffers()
        {
            int[] layerids = GetDynamicLayerIds();
            Assert.IsTrue((layerids != null && layerids.Length != 0), "Dynamic layer is empty!");

            IntPtr[] layerBuffers = new IntPtr[layerids.Length];
            NativeSwapchain.AcquireCameraFrame(m_SwapchainHandle, ref m_FrameHandle, layerids, ref layerBuffers);
            for (int i = 0; i < layerBuffers.Length; i++)
            {
                m_LayerBuffersDict[layerids[i]] = layerBuffers[i];
            }

            for (int i = 0; i < Overlays.Count; ++i)
            {
                Overlays[i].SwapBuffers(GetBufferHandler(Overlays[i].LayerId));
            }
        }

        public IntPtr GetBufferHandler(int layerid)
        {
            IntPtr bufferid;
            if (!m_LayerBuffersDict.TryGetValue(layerid, out bufferid))
            {
                return IntPtr.Zero;
            }

            return bufferid;
        }

        public void SubmitFrame()
        {
            //NRDebugger.Info("[SwapChain] SubmitFrame IsInitialized:{0} _ViewportListHandle:{1} _FrameHandle:{2}", IsInitialized, m_ViewportListHandle, m_FrameHandle);
            if (!IsInitialized || m_ViewportListHandle == 0 || m_FrameHandle == 0)
            {
                NRDebugger.Warning("[SwapChain] Can not submit frame!");
                return;
            }
            NativeSwapchain.Submit(m_FrameHandle, m_ViewportListHandle);
        }

        public void UpdateExternalSurface(int layerid, NativeMat4f transform, Int64 timestamp, int index)
        {
            if (m_SwapchainHandle == 0)
            {
                NRDebugger.Warning("[SwapChain] Can not update external surface!");
                return;
            }
            NativeSwapchain.UpdateExternalSurface(m_SwapchainHandle, layerid, transform, timestamp, index);
        }
        #endregion

        public void Destroy()
        {
#if !UNITY_EDITOR
            if (m_ViewportListHandle != 0)
            {
                NativeSwapchain.DestroyBufferViewportList(m_ViewportListHandle);
                m_ViewportListHandle = 0;
            }

            if (m_SwapchainHandle != 0)
            {
                NativeSwapchain.DestroySwapChain(m_SwapchainHandle);
                m_SwapchainHandle = 0;
            }
#endif
        }
    }
}
