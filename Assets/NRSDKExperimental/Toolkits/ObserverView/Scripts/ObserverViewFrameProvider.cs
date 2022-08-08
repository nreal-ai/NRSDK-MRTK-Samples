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
    using System.Collections;
    using UnityEngine;

    /// <summary> An observer view frame provider. </summary>
    public class ObserverViewFrameProvider : AbstractFrameProvider
    {
        /// <summary> Source camera. </summary>
        private Camera m_SourceCamera;
        /// <summary> Source frame. </summary>
        private UniversalTextureFrame m_SourceFrame;
        /// <summary> True if is play, false if not. </summary>
        private bool isPlay = false;
        /// <summary> The FPS. </summary>
        private int _FPS;

        /// <summary> Init provider with the camera target texture. </summary>
        /// <param name="camera"> camera target texture.</param>
        /// <param name="fps">    (Optional) The FPS.</param>
        public ObserverViewFrameProvider(Camera camera, int fps = 30)
        {
            this.m_SourceCamera = camera;
            this.m_SourceFrame.textures = new Texture[1];
            this.m_SourceFrame.textureType = TextureType.RGB;
            this._FPS = fps;

            NRKernalUpdater.Instance.StartCoroutine(UpdateFrame());
        }

        /// <summary> Updates the frame. </summary>
        /// <returns> An IEnumerator. </returns>
        public IEnumerator UpdateFrame()
        {
            while (true)
            {
                if (isPlay)
                {
                    m_SourceFrame.textures[0] = m_SourceCamera.targetTexture;
                    m_SourceFrame.timeStamp = NRTools.GetTimeStamp();
                    OnUpdate?.Invoke(m_SourceFrame);
                }
                yield return new WaitForSeconds(1 / _FPS);
            }
        }

        /// <summary> Gets frame information. </summary>
        /// <returns> The frame information. </returns>
        public override Resolution GetFrameInfo()
        {
            Resolution resolution = new Resolution();
            resolution.width = m_SourceFrame.textures[0].width;
            resolution.height = m_SourceFrame.textures[0].height;
            return resolution;
        }

        /// <summary> Plays this object. </summary>
        public override void Play()
        {
            isPlay = true;
        }

        /// <summary> Stops this object. </summary>
        public override void Stop()
        {
            isPlay = false;
        }

        /// <summary> Releases this object. </summary>
        public override void Release()
        {
            isPlay = false;
            NRKernalUpdater.Instance.StopCoroutine(UpdateFrame());
        }
    }
}
