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
    using UnityEngine;

    /// <summary> Create a YUV camera texture. </summary>
    public class NRRGBCamTextureYUV : CameraModelView
    {
        /// <summary> The on update. </summary>
        public Action<YUVTextureFrame> OnUpdate;
        /// <summary> A yuv texture frame. </summary>
        public struct YUVTextureFrame
        {
            /// <summary> The time stamp. </summary>
            public UInt64 timeStamp;
            /// <summary> The gain </summary>
            public UInt32 gain;
            /// <summary> The exposureTime </summary>
            public UInt32 exposureTime;
            /// <summary> The texture y coordinate. </summary>
            public Texture2D textureY;
            /// <summary> The texture u. </summary>
            public Texture2D textureU;
            /// <summary> The texture v. </summary>
            public Texture2D textureV;
            /// <summary> The buffer. </summary>
            public byte[] YBuf;
            /// <summary> The buffer. </summary>
            public byte[] UBuf;
            /// <summary> The buffer. </summary>
            public byte[] VBuf;
        }
        /// <summary> Information describing the frame. </summary>
        private YUVTextureFrame m_FrameData;

        /// <summary> Creates the tex. </summary>
        private void CreateTex()
        {
            if (m_FrameData.textureY == null)
            {
                m_FrameData = new YUVTextureFrame();
                m_FrameData.textureY = new Texture2D(Width, Height, TextureFormat.Alpha8, false);
                m_FrameData.textureU = new Texture2D(Width / 2, Height / 2, TextureFormat.Alpha8, false);
                m_FrameData.textureV = new Texture2D(Width / 2, Height / 2, TextureFormat.Alpha8, false);
            }
        }

        /// <summary> Gets the texture. </summary>
        /// <returns> The texture. </returns>
        public YUVTextureFrame GetTexture()
        {
            if (m_FrameData.textureY == null)
            {
                CreateTex();
            }
            return m_FrameData;
        }

        /// <summary> Default constructor. </summary>
        public NRRGBCamTextureYUV() : base(CameraImageFormat.YUV_420_888)
        {
            CreateTex();
        }

        /// <summary> Load raw texture data. </summary>
        /// <param name="frame"> .</param>
        protected override void OnRawDataUpdate(FrameRawData frame)
        {
            LoadYUVTexture(frame);
            OnUpdate?.Invoke(m_FrameData);
        }

        /// <summary> Loads yuv texture. </summary>
        /// <param name="frame"> The frame.</param>
        private void LoadYUVTexture(FrameRawData frame)
        {
            if (frame.data == null || frame.data.Length == 0)
            {
                NRDebugger.Error("[NRRGBCamTextureYUV] LoadYUVTexture error: frame is null");
                return;
            }

            int size = frame.data.Length;
            if (m_FrameData.YBuf == null)
            {
                m_FrameData.YBuf = new byte[size * 2 / 3];
                m_FrameData.UBuf = new byte[size / 6];
                m_FrameData.VBuf = new byte[size / 6];
            }
            if (m_FrameData.textureY == null)
            {
                CreateTex();
            }
            m_FrameData.timeStamp = frame.timeStamp;
            m_FrameData.gain = frame.gain;
            m_FrameData.exposureTime = frame.exposureTime;
            Array.Copy(frame.data, 0, m_FrameData.YBuf, 0, m_FrameData.YBuf.Length);
            Array.Copy(frame.data, m_FrameData.YBuf.Length, m_FrameData.UBuf, 0, m_FrameData.UBuf.Length);
            Array.Copy(frame.data, m_FrameData.YBuf.Length + m_FrameData.UBuf.Length, m_FrameData.VBuf, 0, m_FrameData.VBuf.Length);

            m_FrameData.textureY.LoadRawTextureData(m_FrameData.YBuf);
            m_FrameData.textureU.LoadRawTextureData(m_FrameData.UBuf);
            m_FrameData.textureV.LoadRawTextureData(m_FrameData.VBuf);

            m_FrameData.textureY.Apply();
            m_FrameData.textureU.Apply();
            m_FrameData.textureV.Apply();
        }

        /// <summary> On texture stopped. </summary>
        protected override void OnStopped()
        {
            base.OnStopped();
            GameObject.Destroy(m_FrameData.textureY);
            GameObject.Destroy(m_FrameData.textureU);
            GameObject.Destroy(m_FrameData.textureV);
            m_FrameData.textureY = null;
            m_FrameData.textureU = null;
            m_FrameData.textureV = null;
            m_FrameData.YBuf = null;
            m_FrameData.UBuf = null;
            m_FrameData.VBuf = null;
        }
    }
}
