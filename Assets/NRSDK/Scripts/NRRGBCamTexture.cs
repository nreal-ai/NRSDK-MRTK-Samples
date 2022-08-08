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

    /// <summary> Create a rgb camera texture. </summary>
    public class NRRGBCamTexture : CameraModelView
    {
        /// <summary> When the data of RGBCamera is updated, it will be called. </summary>
        public Action<CameraTextureFrame> OnUpdate;
        /// <summary> The current frame. </summary>
        public CameraTextureFrame CurrentFrame;
        /// <summary> The texture. </summary>
        private Texture2D m_Texture;
        /// <summary> Information describing the raw. </summary>
        private byte[] m_RawData;
        public byte[] RawData { get { return m_RawData; } }

        /// <summary> Default constructor. </summary>
        public NRRGBCamTexture() : base(CameraImageFormat.RGB_888)
        {
            this.m_Texture = CreateTexture();
            this.CurrentFrame.texture = this.m_Texture;
        }

        /// <summary> Creates the texture. </summary>
        /// <returns> The new texture. </returns>
        private Texture2D CreateTexture()
        {
            return new Texture2D(Width, Height, TextureFormat.RGB24, false);
        }

        /// <summary> Gets the texture. </summary>
        /// <returns> The texture. </returns>
        public Texture2D GetTexture()
        {
            if (m_Texture == null)
            {
                this.m_Texture = CreateTexture();
                this.CurrentFrame.texture = this.m_Texture;
            }
            return m_Texture;
        }

        /// <summary> Load raw texture data. </summary>
        /// <param name="rgbRawDataFrame"> .</param>
        protected override void OnRawDataUpdate(FrameRawData frame)
        {
            if (m_Texture == null)
            {
                this.m_Texture = CreateTexture();
            }
            int dataSize = frame.data.Length;
            if (m_RawData == null || m_RawData.Length != dataSize)
            {
                m_RawData = new byte[dataSize];
            }
            Array.Copy(frame.data, 0, m_RawData, 0, dataSize);
             
            m_Texture.LoadRawTextureData(m_RawData);
            m_Texture.Apply();

            CurrentFrame.timeStamp = frame.timeStamp;
            CurrentFrame.gain = frame.gain;
            CurrentFrame.exposureTime = frame.exposureTime;
            CurrentFrame.texture = m_Texture;

            OnUpdate?.Invoke(CurrentFrame);
        }

        /// <summary> On texture stopped. </summary>
        protected override void OnStopped()
        {
            GameObject.Destroy(m_Texture);
            this.m_Texture = null;
            m_RawData = null;
            this.CurrentFrame.texture = null;
        }
    }
}
