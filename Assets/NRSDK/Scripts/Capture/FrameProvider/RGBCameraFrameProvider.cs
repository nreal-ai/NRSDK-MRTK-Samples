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

    /// <summary> A RGB camera frame provider. </summary>
    public class RGBCameraFrameProvider : AbstractFrameProvider
    {
        /// <summary> The RGB tex. </summary>
        private CameraModelView m_CameraTexture;
        private UniversalTextureFrame frameInfo;

        /// <summary> Default constructor. </summary>
        public RGBCameraFrameProvider()
        {
            var active_format = NativeCameraProxy.GetActiveCameraImageFormat(NRRgbCamera.ID);
            NRDebugger.Info("[CameraFrameProvider] Use format:{0}", active_format);
            switch (active_format)
            {
                case CameraImageFormat.YUV_420_888:
                    m_CameraTexture = new NRRGBCamTextureYUV();
                    ((NRRGBCamTextureYUV)m_CameraTexture).OnUpdate += UpdateYUVFrame;
                    frameInfo.textures = new Texture[3];
                    frameInfo.textureType = TextureType.YUV;
                    break;
                case CameraImageFormat.RGB_888:
                    m_CameraTexture = new NRRGBCamTexture();
                    ((NRRGBCamTexture)m_CameraTexture).OnUpdate += UpdateRGBFrame;
                    frameInfo.textures = new Texture[1];
                    frameInfo.textureType = TextureType.RGB;
                    break;
                default:
                    break;
            }
        }

        private void UpdateYUVFrame(NRRGBCamTextureYUV.YUVTextureFrame frame)
        {
            frameInfo.timeStamp = frame.timeStamp;
            frameInfo.gain = frame.gain;
            frameInfo.exposureTime = frame.exposureTime;
            frameInfo.textures[0] = frame.textureY;
            frameInfo.textures[1] = frame.textureU;
            frameInfo.textures[2] = frame.textureV;
            OnUpdate?.Invoke(frameInfo);
            m_IsFrameReady = true;
        }

        /// <summary> Updates the frame described by frame. </summary>
        /// <param name="frame"> The frame.</param>
        private void UpdateRGBFrame(CameraTextureFrame frame)
        {
            frameInfo.timeStamp = frame.timeStamp;
            frameInfo.gain = frame.gain;
            frameInfo.exposureTime = frame.exposureTime;
            frameInfo.textures[0] = frame.texture;
            OnUpdate?.Invoke(frameInfo);
            m_IsFrameReady = true;
        }

        /// <summary> Gets frame information. </summary>
        /// <returns> The frame information. </returns>
        public override Resolution GetFrameInfo()
        {
            Resolution resolution = new Resolution();
            resolution.width = m_CameraTexture.Width;
            resolution.height = m_CameraTexture.Height;
            return resolution;
        }

        /// <summary> Plays this object. </summary>
        public override void Play()
        {
            m_CameraTexture.Play();
        }

        /// <summary> Stops this object. </summary>
        public override void Stop()
        {
            m_CameraTexture.Pause();
        }

        /// <summary> Releases this object. </summary>
        public override void Release()
        {
            m_CameraTexture.Stop();
            m_CameraTexture = null;
        }
    }
}
