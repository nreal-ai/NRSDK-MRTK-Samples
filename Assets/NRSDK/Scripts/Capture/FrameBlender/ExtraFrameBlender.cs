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
    using UnityEngine;

    /// <summary> A frame blender. </summary>
    public class ExtraFrameBlender : BlenderBase
    {
        /// <summary> Target camera. </summary>
        protected Camera m_TargetCamera;
        /// <summary> The encoder. </summary>
        protected IEncoder m_Encoder;
        /// <summary> The blend material. </summary>
        private Material m_BackGroundMat;
        private NRBackGroundRender m_NRBackGroundRender;
        private NRCameraInitializer m_DeviceParamInitializer;

        private RenderTexture m_BlendTexture;
        /// <summary> Gets or sets the blend texture. </summary>
        /// <value> The blend texture. </value>
        public override RenderTexture BlendTexture
        {
            get
            {
                return m_BlendTexture;
            }
        }

        /// <summary> Initializes this object. </summary>
        /// <param name="camera">  The camera.</param>
        /// <param name="encoder"> The encoder.</param>
        /// <param name="param">   The parameter.</param>
        public override void Init(Camera camera, IEncoder encoder, CameraParameters param)
        {
            base.Init(camera, encoder, param);

            Width = param.cameraResolutionWidth;
            Height = param.cameraResolutionHeight;
            m_TargetCamera = camera;
            m_Encoder = encoder;
            BlendMode = param.blendMode;

            m_NRBackGroundRender = m_TargetCamera.gameObject.GetComponent<NRBackGroundRender>();
            if (m_NRBackGroundRender == null)
            {
                m_NRBackGroundRender = m_TargetCamera.gameObject.AddComponent<NRBackGroundRender>();
            }
            m_NRBackGroundRender.enabled = false;
            m_DeviceParamInitializer = camera.gameObject.GetComponent<NRCameraInitializer>();

            m_TargetCamera.enabled = false;
            m_BlendTexture = UnityExtendedUtility.CreateRenderTexture(Width, Height, 24, RenderTextureFormat.ARGB32);
            m_TargetCamera.targetTexture = m_BlendTexture;
        }

        /// <summary> Executes the 'frame' action. </summary>
        /// <param name="frame"> The frame.</param>
        public override void OnFrame(UniversalTextureFrame frame)
        {
            base.OnFrame(frame);

            if (!m_DeviceParamInitializer.IsInitialized)
            {
                return;
            }

            if (m_BackGroundMat == null)
            {
                m_BackGroundMat = CreatBlendMaterial(BlendMode, frame.textureType);
                m_NRBackGroundRender.SetMaterial(m_BackGroundMat);
            }

            bool isyuv = frame.textureType == TextureType.YUV;
            const string MainTextureStr = "_MainTex";
            const string UTextureStr = "_UTex";
            const string VTextureStr = "_VTex";

            switch (BlendMode)
            {
                case BlendMode.VirtualOnly:
                    m_NRBackGroundRender.enabled = false;
                    m_TargetCamera.Render();
                    break;
                case BlendMode.RGBOnly:
                case BlendMode.Blend:
                case BlendMode.WidescreenBlend:
                    if (isyuv)
                    {
                        m_BackGroundMat.SetTexture(MainTextureStr, frame.textures[0]);
                        m_BackGroundMat.SetTexture(UTextureStr, frame.textures[1]);
                        m_BackGroundMat.SetTexture(VTextureStr, frame.textures[2]);
                    }
                    else
                    {
                        m_BackGroundMat.SetTexture(MainTextureStr, frame.textures[0]);
                    }
                    m_NRBackGroundRender.enabled = true;
                    m_TargetCamera.Render();
                    break;
                default:
                    m_NRBackGroundRender.enabled = false;
                    break;
            }

            // Commit frame                
            m_Encoder.Commit(BlendTexture, frame.timeStamp);
            FrameCount++;
        }

        private Material CreatBlendMaterial(BlendMode mode, TextureType texturetype)
        {
            string shader_name;
            shader_name = "Record/Shaders/NRBackground{0}";
            shader_name = string.Format(shader_name, texturetype == TextureType.RGB ? "" : "YUV");
            return new Material(Resources.Load<Shader>(shader_name));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources. </summary>
        public override void Dispose()
        {
            base.Dispose();

            m_BlendTexture?.Release();
            m_BlendTexture = null;
        }
    }
}
