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

    [ExecuteInEditMode]
    public class NROverlay : OverlayBase
    {
        /// <summary>
        /// If true, the layer will be created as an external surface. externalSurfaceObject contains the Surface object. It's effective only on Android.
        /// </summary>
        [Tooltip("If true, the layer will be created as an external surface. externalSurfaceObject contains the Surface object. It's effective only on Android.")]
        public bool isExternalSurface = false;

        /// <summary>
        /// The width which will be used to create the external surface. It's effective only on Android.
        /// </summary>
        [Tooltip("The width which will be used to create the external surface. It's effective only on Android.")]
        public int externalSurfaceWidth = 0;

        /// <summary>
        /// The height which will be used to create the external surface. It's effective only on Android.
        /// </summary>
        [Tooltip("The height which will be used to create the external surface. It's effective only on Android.")]
        public int externalSurfaceHeight = 0;

        /// <summary>
        /// If true, the texture's content is copied to the compositor each frame.
        /// </summary>
        [Tooltip("If true, the texture's content is copied to the compositor each frame.")]
        public bool isDynamic = false;

        /// <summary>
        /// If true, the layer would be used to present protected content. The flag is effective only on Android.
        /// </summary>
        [Tooltip("If true, the layer would be used to present protected content. The flag is effective only on Android.")]
        public bool isProtectedContent = false;

        /// <summary>
        /// The Texture to show in the layer.
        /// </summary>
        [Tooltip("The Texture to show in the layer.")]
        public Texture texture;

        /// <summary>
        /// Which display this overlay should render to.
        /// </summary>
        [Tooltip("Which display this overlay should render to.")]
        public LayerSide layerSide = LayerSide.Both;

        /// <summary>
        /// Whether render this overlay as 0-dof.
        /// </summary>
        [Tooltip("Whether render this overlay as 0-dof.")]
        public bool isScreenSpace = false;

        /// <summary>
        /// Whether this overlay is 3D rendering layer.
        /// </summary>
        [Tooltip("Whether this overlay is 3D rendering layer.")]
        public bool is3DLayer = false;

        /// <summary>
        /// Preview the overlay in the editor using a mesh renderer.
        /// </summary>
        [Tooltip("Preview the overlay in the editor using a mesh renderer.")]
        public bool previewInEditor;

        private bool userTextureMask = false;

        public delegate void ExternalSurfaceObjectCreated();
        public delegate void BufferedSurfaceObjectChangeded(RenderTexture rt);
        public event ExternalSurfaceObjectCreated externalSurfaceObjectCreated;
        /// <summary> Only for RenderTexture imageType.</summary>
        public event BufferedSurfaceObjectChangeded onBufferChanged;

        public Texture MainTexture
        {
            get
            {
                return texture;
            }
            set
            {
                if (texture != value)
                {
                    SetDirty(true);
                    userTextureMask = true;
                    texture = value;
                }
            }
        }

        /// <summary> Determines the on-screen appearance of a layer. </summary>
        public enum OverlayShape
        {
            Quad,
            //Cylinder,
            //Cubemap,
            //OffcenterCubemap,
            //Equirect,
        }

        private NROverlayMeshGenerator m_MeshGenerator;
        public OverlayShape overlayShape { get; set; } = OverlayShape.Quad;
        private Matrix4x4 m_WorldToLeftDisplayMatrix = Matrix4x4.identity;
        private Matrix4x4 m_WorldToRightDisplayMatrix = Matrix4x4.identity;
        private Matrix4x4 m_OriginPose;

        public Rect[] sourceUVRect = new Rect[2] {
                new Rect(0, 0, 1, 1),
                new Rect(0, 0, 1, 1)
        };

        new void Start()
        {
            base.Start();
            ApplyMeshAndMat();
        }

        private void ApplyMeshAndMat()
        {
            m_MeshGenerator = gameObject.GetComponentInChildren<NROverlayMeshGenerator>();
            if (m_MeshGenerator == null)
            {
                var go = new GameObject("NROverlayMeshGenerator");
                // go.hideFlags = HideFlags.HideInHierarchy;
                go.transform.SetParent(this.transform, false);
                m_MeshGenerator = go.AddComponent<NROverlayMeshGenerator>();
            }
            m_MeshGenerator.SetOverlay(this);
        }

        protected override void Initialize()
        {
            base.Initialize();

            m_BufferSpec = new BufferSpec();
            if (isExternalSurface)
            {
                m_BufferSpec.size = new NativeResolution(externalSurfaceWidth, externalSurfaceHeight);
            }
            else if (texture != null)
            {
                m_BufferSpec.size = new NativeResolution(texture.width, texture.height);
            }

            m_BufferSpec.colorFormat = NRColorFormat.NR_COLOR_FORMAT_RGBA_8888;
            m_BufferSpec.depthFormat = NRDepthStencilFormat.NR_DEPTH_STENCIL_FORMAT_DEPTH_24;
            m_BufferSpec.samples = 1;
            m_BufferSpec.useExternalSurface = isExternalSurface;
            int flag = 0;
            if (isExternalSurface)
            {
                if (isProtectedContent)
                {
                    flag |= (int)NRExternalSurfaceFlags.NR_EXTERNAL_SURFACE_FLAG_PROTECTED;
                }
                if (is3DLayer)
                {
                    flag |= (int)NRExternalSurfaceFlags.NR_EXTERNAL_SURFACE_FLAG_SYNCHRONOUS;
                    flag |= (int)NRExternalSurfaceFlags.NR_EXTERNAL_SURFACE_FLAG_USE_TIMESTAMPS;
                }
            }
            m_BufferSpec.surfaceFlag = flag;

            m_OriginPose = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        }

        public override void CreateOverlayTextures()
        {
            ReleaseOverlayTextures();

            IntPtr texturePtr = IntPtr.Zero;

            if (isExternalSurface)
            {
                externalSurfaceObjectCreated?.Invoke();
            }
            else if (isDynamic)
            {
                if (texture == null)
                {
                    NRDebugger.Warning("Current texture is empty!!!");
                    return;
                }
                for (int i = 0; i < m_BufferSpec.bufferCount; ++i)
                {
                    RenderTexture rt = UnityExtendedUtility.CreateRenderTexture(
                        texture.width, texture.height, 24, RenderTextureFormat.ARGB32
                        );
                    texturePtr = rt.GetNativeTexturePtr();
                    Textures.Add(texturePtr, rt);
                }
            }
            else if (texture)
            {
                texturePtr = texture.GetNativeTexturePtr();
                Textures.Add(texturePtr, texture);
            }
        }

        public override void ReleaseOverlayTextures()
        {
            if (Textures.Count == 0)
            {
                return;
            }
            if (isDynamic)
            {
                foreach (var item in Textures)
                {
                    RenderTexture rt = item.Value as RenderTexture;
                    if (rt != null)
                    {
                        rt.Release();
                    }
                    else if (item.Value != null)
                    {
                        GameObject.Destroy(item.Value);
                    }
                }
            }
            Textures.Clear();
        }

        public override void CreateViewport()
        {
            base.CreateViewport();

            if (layerSide != LayerSide.Both)
            {
                m_ViewPorts = new ViewPort[1];
                m_ViewPorts[0].reprojection = isScreenSpace ? NRReprojectionType.NR_REPROJECTION_TYPE_NONE : NRReprojectionType.NR_REPROJECTION_TYPE_FULL;
                m_ViewPorts[0].sourceUV = sourceUVRect[0];
                m_ViewPorts[0].targetDisplay = layerSide;
                m_ViewPorts[0].layerID = m_LayerId;
                m_ViewPorts[0].is3DLayer = is3DLayer;
                if (is3DLayer)
                {
                    NativeDevice displayDev = layerSide == LayerSide.Left ? NativeDevice.LEFT_DISPLAY : NativeDevice.RIGHT_DISPLAY;
                    NRFrame.GetEyeFov(displayDev, ref m_ViewPorts[0].fov);
                }
                else
                {
                    m_ViewPorts[0].transform = CalculateCoords(layerSide);
                }
                NRSwapChainManager.Instance.CreateBufferViewport(ref m_ViewPorts[0]);
            }
            else
            {
                m_ViewPorts = new ViewPort[2];
                m_ViewPorts[0].reprojection = isScreenSpace ? NRReprojectionType.NR_REPROJECTION_TYPE_NONE : NRReprojectionType.NR_REPROJECTION_TYPE_FULL;
                m_ViewPorts[0].sourceUV = sourceUVRect[0];
                m_ViewPorts[0].targetDisplay = LayerSide.Left;
                m_ViewPorts[0].layerID = m_LayerId;
                m_ViewPorts[0].index = -1;
                if (is3DLayer)
                {
                    NRFrame.GetEyeFov(NativeDevice.LEFT_DISPLAY, ref m_ViewPorts[0].fov);
                }
                else
                {
                    m_ViewPorts[0].transform = CalculateCoords(LayerSide.Left);
                }
                m_ViewPorts[0].is3DLayer = is3DLayer;
                NRSwapChainManager.Instance.CreateBufferViewport(ref m_ViewPorts[0]);

                m_ViewPorts[1].reprojection = isScreenSpace ? NRReprojectionType.NR_REPROJECTION_TYPE_NONE : NRReprojectionType.NR_REPROJECTION_TYPE_FULL;
                m_ViewPorts[1].sourceUV = sourceUVRect[1];
                m_ViewPorts[1].targetDisplay = LayerSide.Right;
                m_ViewPorts[1].layerID = m_LayerId;
                m_ViewPorts[1].index = -1;
                if (is3DLayer)
                {
                    NRFrame.GetEyeFov(NativeDevice.RIGHT_DISPLAY, ref m_ViewPorts[1].fov);
                }
                else
                {
                    m_ViewPorts[1].transform = CalculateCoords(LayerSide.Right);
                }
                m_ViewPorts[1].is3DLayer = is3DLayer;
                NRSwapChainManager.Instance.CreateBufferViewport(ref m_ViewPorts[1]);
            }

            if (isScreenSpace)
            {
                CreateWorldToDisplayMatrix();
            }

            if (is3DLayer)
            {
                ClearMask();
            }
        }

        /// <summary>
        /// Update transform of viewPort.
        /// </summary>
        public override void UpdateViewPort()
        {
            base.UpdateViewPort();

            if (m_ViewPorts == null)
            {
                NRDebugger.Warning("Can not update view port for this layer:{0}", gameObject.name);
                return;
            }

            if (layerSide != LayerSide.Both)
            {
                if (!is3DLayer)
                {
                    m_ViewPorts[0].transform = CalculateCoords(layerSide);
                }
                NRSwapChainManager.Instance.UpdateBufferViewport(ref m_ViewPorts[0]);
            }
            else
            {
                if (!is3DLayer)
                {
                    m_ViewPorts[0].transform = CalculateCoords(LayerSide.Left);
                    m_ViewPorts[1].transform = CalculateCoords(LayerSide.Right);
                }

                NRSwapChainManager.Instance.UpdateBufferViewport(ref m_ViewPorts[0]);
                NRSwapChainManager.Instance.UpdateBufferViewport(ref m_ViewPorts[1]);
            }
        }

        public override void DestroyViewPort()
        {
            base.DestroyViewPort();
            if (m_ViewPorts != null)
            {
                foreach (var viewport in m_ViewPorts)
                {
                    NRSwapChainManager.Instance.DestroyBufferViewPort(viewport.nativeHandler);
                }
                m_ViewPorts = null;
            }
        }

        public void Apply()
        {
            SetDirty(true);
        }

        public override void SwapBuffers(IntPtr bufferHandler)
        {
            if (isDynamic)
            {
                Texture targetTexture;
                if (!Textures.TryGetValue(bufferHandler, out targetTexture))
                {
                    NRDebugger.Error("Can not find the texture:" + bufferHandler);
                    return;
                }
                onBufferChanged?.Invoke((RenderTexture)targetTexture);
            }
        }

        public IntPtr GetSurfaceId()
        {
            if (!isExternalSurface)
            {
                return IntPtr.Zero;
            }
            return NRSwapChainManager.Instance.GetSurfaceHandler(this.LayerId);
        }

        private void CreateWorldToDisplayMatrix()
        {
            var centerAnchor = NRSessionManager.Instance.NRHMDPoseTracker.centerAnchor;
            Matrix4x4 head_T_centerLocal = Matrix4x4.TRS(
                centerAnchor.transform.localPosition,
                centerAnchor.transform.localRotation,
                centerAnchor.transform.localScale);

            var leftCamera = NRSessionManager.Instance.NRHMDPoseTracker.leftCamera;
            Matrix4x4 head_T_leftLocal = Matrix4x4.TRS(
                leftCamera.transform.localPosition,
                leftCamera.transform.localRotation,
                leftCamera.transform.localScale);
            m_WorldToLeftDisplayMatrix = leftCamera.worldToCameraMatrix * leftCamera.transform.localToWorldMatrix *
                Matrix4x4.Inverse(head_T_leftLocal) * head_T_centerLocal;

            var rightCamera = NRSessionManager.Instance.NRHMDPoseTracker.rightCamera;
            Matrix4x4 head_T_rightLocal = Matrix4x4.TRS(
                rightCamera.transform.localPosition,
                rightCamera.transform.localRotation,
                rightCamera.transform.localScale);
            m_WorldToRightDisplayMatrix = rightCamera.worldToCameraMatrix * rightCamera.transform.localToWorldMatrix *
                Matrix4x4.Inverse(head_T_rightLocal) * head_T_centerLocal;
        }

        public void UpdateTransform(Matrix4x4 originMatrix, bool isScreenSpace)
        {
            this.isScreenSpace = isScreenSpace;
            if (this.isScreenSpace)
            {
                CreateWorldToDisplayMatrix();
            }

            m_OriginPose = originMatrix;
        }

        private Matrix4x4 CalculateCoords(LayerSide side)
        {
            if (isScreenSpace)
            {
                if (side == LayerSide.Left)
                {
                    return m_WorldToLeftDisplayMatrix * m_OriginPose;
                }
                else
                {
                    return m_WorldToRightDisplayMatrix * m_OriginPose;
                }
            }
            else
            {
                Camera imageCamera = null;
                if (side == LayerSide.Left)
                {
                    imageCamera = NRSessionManager.Instance.NRHMDPoseTracker.leftCamera;
                }
                else if (side == LayerSide.Right)
                {
                    imageCamera = NRSessionManager.Instance.NRHMDPoseTracker.rightCamera;
                }
                if (imageCamera == null)
                {
                    return transform.localToWorldMatrix;
                }
                return imageCamera.worldToCameraMatrix * transform.localToWorldMatrix;
            }
        }

        public void UpdateExternalSurface(NativeMat4f pose, Int64 timestamp, int index)
        {
            NRSwapChainManager.Instance.UpdateExternalSurface(m_LayerId, pose, timestamp, index);
        }

        private void ClearMask()
        {
            if (m_MeshGenerator != null)
            {
                GameObject.DestroyImmediate(m_MeshGenerator.gameObject);
                m_MeshGenerator = null;
            }
        }
        public override void Destroy()
        {
            base.Destroy();
            if (userTextureMask)
            {
                texture = null;
                userTextureMask = false;
            }
        }

        new void OnDestroy()
        {
            base.OnDestroy();
            ClearMask();
        }
    }
}