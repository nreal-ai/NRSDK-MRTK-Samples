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
    public class FrameBlender : BlenderBase
    {
        /// <summary> Target camera. </summary>
        protected Camera m_TargetCamera;
        /// <summary> The encoder. </summary>
        protected IEncoder m_Encoder;
        /// <summary> The blend material. </summary>
        private Material m_BlendMaterial;
        /// <summary> The blend mode. </summary>
        protected BlendMode m_BlendMode;
        /// <summary> The RGB source. </summary>
        protected RenderTexture m_RGBSource;
        /// <summary> The temporary combine tex. </summary>
        protected Texture2D m_TempCombineTex;

        /// <summary> The blend texture. </summary>
        private RenderTexture m_BlendTexture;
        /// <summary> Gets or sets the blend texture. </summary>
        /// <value> The blend texture. </value>
        public override RenderTexture BlendTexture
        {
            get
            {
                return m_BlendTexture;
            }
            protected set
            {
                m_BlendTexture = value;
            }
        }

        /// <summary> Gets the RGB texture. </summary>
        /// <value> The RGB texture. </value>
        public RenderTexture RGBTexture
        {
            get
            {
                return m_RGBSource;
            }
        }

        /// <summary> Gets the virtual texture. </summary>
        /// <value> The virtual texture. </value>
        public RenderTexture VirtualTexture
        {
            get
            {
                return m_TargetCamera.targetTexture;
            }
        }

        /// <summary> Initializes this object. </summary>
        /// <param name="camera">  The camera.</param>
        /// <param name="encoder"> The encoder.</param>
        /// <param name="param">   The parameter.</param>
        public override void Init(Camera camera, IEncoder encoder, CameraParameters param)
        {
            Width = param.cameraResolutionWidth;
            Height = param.cameraResolutionHeight;
            m_BlendMode = param.blendMode;
            m_TargetCamera = camera;
            m_Encoder = encoder;

            // An extra rendering is required when BlendMode is RGBOnly or WidescreenBlend.
            switch (m_BlendMode)
            {
                case BlendMode.RGBOnly:
                    BlendTexture = UnityExtendedUtility.CreateRenderTexture(Width, Height, 24, RenderTextureFormat.ARGB32, false);
                    break;
                case BlendMode.VirtualOnly:
                    BlendTexture = UnityExtendedUtility.CreateRenderTexture(Width, Height, 24, RenderTextureFormat.ARGB32, false);
                    break;
                case BlendMode.Blend:
                    BlendTexture = UnityExtendedUtility.CreateRenderTexture(Width, Height, 24, RenderTextureFormat.ARGB32, false);
                    break;
                case BlendMode.WidescreenBlend:
                    BlendTexture = UnityExtendedUtility.CreateRenderTexture(2 * Width, Height, 24, RenderTextureFormat.ARGB32, false);
                    m_RGBSource = UnityExtendedUtility.CreateRenderTexture(Width, Height, 24, RenderTextureFormat.ARGB32, false);
                    m_TempCombineTex = new Texture2D(2 * Width, Height, TextureFormat.ARGB32, false);
                    break;
                default:
                    break;
            }

            m_TargetCamera.enabled = false;
            m_TargetCamera.targetTexture = UnityExtendedUtility.CreateRenderTexture(Width, Height, 24, RenderTextureFormat.ARGB32);
        }

        /// <summary> Executes the 'frame' action. </summary>
        /// <param name="frame"> The frame.</param>
        public override void OnFrame(UniversalTextureFrame frame)
        {
            Texture2D frametex = frame.textures[0] as Texture2D;
            if (m_BlendMode != BlendMode.RGBOnly)
            {
                m_TargetCamera.Render();
            }

            if (m_BlendMaterial == null)
            {
                CreatBlendMaterial(m_BlendMode, frame.textureType);
            }

            bool isyuv = frame.textureType == TextureType.YUV;
            switch (m_BlendMode)
            {
                case BlendMode.RGBOnly:
                    const string MainTextureStr = "_MainTex";
                    if (isyuv)
                    {
                        const string UTextureStr = "_UTex";
                        const string VTextureStr = "_VTex";
                        m_BlendMaterial.SetTexture(MainTextureStr, frame.textures[0]);
                        m_BlendMaterial.SetTexture(UTextureStr, frame.textures[1]);
                        m_BlendMaterial.SetTexture(VTextureStr, frame.textures[2]);
                    }
                    else
                    {
                        m_BlendMaterial.SetTexture(MainTextureStr, frame.textures[0]);
                    }
                    Graphics.Blit(frame.textures[0], BlendTexture, m_BlendMaterial);
                    break;
                case BlendMode.VirtualOnly:
                    m_BlendMaterial.SetTexture("_MainTex", m_TargetCamera.targetTexture);
                    Graphics.Blit(m_TargetCamera.targetTexture, BlendTexture, m_BlendMaterial);
                    break;
                case BlendMode.Blend:
                    m_BlendMaterial.SetTexture("_MainTex", m_TargetCamera.targetTexture);
                    if (isyuv)
                    {
                        m_BlendMaterial.SetTexture("_YTex", frame.textures[0]);
                        m_BlendMaterial.SetTexture("_UTex", frame.textures[1]);
                        m_BlendMaterial.SetTexture("_VTex", frame.textures[2]);
                    }
                    else
                    {
                        m_BlendMaterial.SetTexture("_BcakGroundTex", frame.textures[0]);
                    }
                    Graphics.Blit(m_TargetCamera.targetTexture, BlendTexture, m_BlendMaterial);
                    break;
                case BlendMode.WidescreenBlend:
                    if (isyuv)
                    {
                        throw new System.Exception("Not support yuv texture for this mode...");
                    }
                    CombineTexture(frametex, m_TargetCamera.targetTexture, m_TempCombineTex, BlendTexture);
                    break;
                default:
                    break;
            }

            // Commit frame                
            m_Encoder.Commit(BlendTexture, frame.timeStamp);
            FrameCount++;
        }

        private void CreatBlendMaterial(BlendMode mode, TextureType texturetype)
        {
            string shader_name;
            if (mode == BlendMode.Blend)
            {
                shader_name = "Record/Shaders/AlphaBlend{0}";
                shader_name = string.Format(shader_name, texturetype == TextureType.RGB ? "" : "YUV");
            }
            else if (mode == BlendMode.VirtualOnly)
            {
                shader_name = "Record/Shaders/NormalTexture";
            }
            else
            {
                shader_name = "Record/Shaders/NormalBlend{0}";
                shader_name = string.Format(shader_name, texturetype == TextureType.RGB ? "" : "YUV");
            }
            m_BlendMaterial = new Material(Resources.Load<Shader>(shader_name));
        }

        /// <summary> Combine texture. </summary>
        /// <param name="bgsource">   The bgsource.</param>
        /// <param name="foresource"> The foresource.</param>
        /// <param name="tempdest">   The tempdest.</param>
        /// <param name="dest">       Destination for the.</param>
        private void CombineTexture(Texture2D bgsource, RenderTexture foresource, Texture2D tempdest, RenderTexture dest)
        {
            const string MainTextureStr = "_MainTex";
            m_BlendMaterial.SetTexture(MainTextureStr, m_RGBSource);
            Graphics.Blit(bgsource, m_RGBSource, m_BlendMaterial);

            RenderTexture prev = RenderTexture.active;
            RenderTexture.active = m_RGBSource;
            tempdest.ReadPixels(new Rect(0, 0, m_RGBSource.width, m_RGBSource.height), 0, 0);

            RenderTexture.active = foresource;
            tempdest.ReadPixels(new Rect(0, 0, foresource.width, foresource.height), foresource.width, 0);
            tempdest.Apply();
            RenderTexture.active = prev;

            Graphics.Blit(tempdest, dest);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources. </summary>
        public override void Dispose()
        {
            m_BlendTexture?.Release();
            m_RGBSource?.Release();

            GameObject.Destroy(m_TempCombineTex);
            m_BlendTexture = null;
            m_RGBSource = null;
            m_TempCombineTex = null;
        }
    }
}
