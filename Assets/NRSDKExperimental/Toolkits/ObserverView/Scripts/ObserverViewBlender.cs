/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.Experimental.StreammingCast
{
    using NRKernal.Record;
    using UnityEngine;

    /// <summary> An observer view blender. </summary>
    public class ObserverViewBlender : IFrameConsumer
    {
        /// <summary> Target camera. </summary>
        protected Camera m_TargetCamera;
        /// <summary> The encoder. </summary>
        protected IEncoder m_Encoder;

        /// <summary> Gets or sets the width. </summary>
        /// <value> The width. </value>
        public int Width
        {
            get;
            private set;
        }

        /// <summary> Gets or sets the height. </summary>
        /// <value> The height. </value>
        public int Height
        {
            get;
            private set;
        }

        /// <summary> Gets the blend texture. </summary>
        /// <value> The blend texture. </value>
        public Texture BlendTexture
        {
            get
            {
                return m_TargetCamera?.targetTexture;
            }
        }

        /// <summary> Configs. </summary>
        /// <param name="camera">  The camera.</param>
        /// <param name="encoder"> The encoder.</param>
        /// <param name="param">   The parameter.</param>
        public virtual void Config(Camera camera, IEncoder encoder, CameraParameters param)
        {
            Width = param.cameraResolutionWidth;
            Height = param.cameraResolutionHeight;
            m_TargetCamera = camera;
            m_Encoder = encoder;

            m_TargetCamera.enabled = true;
            // As the texture will be used to blend with physical camera image, the alpha channel need to be 0. 
            m_TargetCamera.backgroundColor = new Color(0, 0, 0, 0);
            m_TargetCamera.targetTexture = UnityExtendedUtility.CreateRenderTexture(Width, Height, 24, RenderTextureFormat.ARGB32);
            m_TargetCamera.depthTextureMode = DepthTextureMode.Depth;
        }

        /// <summary> Executes the 'frame' action. </summary>
        /// <param name="frame"> The frame.</param>
        public virtual void OnFrame(UniversalTextureFrame frame)
        {
            // Commit frame                
            m_Encoder.Commit((RenderTexture)frame.textures[0], frame.timeStamp);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources. </summary>
        public void Dispose()
        {

        }
    }
}
