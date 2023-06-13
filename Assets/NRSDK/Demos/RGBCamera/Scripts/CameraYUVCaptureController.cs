/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

using UnityEngine;
using UnityEngine.UI;

namespace NRKernal.NRExamples
{
    /// <summary> A controller for handling camera yuv captures. </summary>
    [HelpURL("https://developer.nreal.ai/develop/unity/rgb-camera")]
    public class CameraYUVCaptureController : MonoBehaviour
    {
        /// <summary> The capture image. </summary>
        public RawImage CaptureImage;
        /// <summary> Number of frames. </summary>
        public Text FrameCount;
        /// <summary> Gets or sets the yuv camera texture. </summary>
        /// <value> The yuv camera texture. </value>
        private NRRGBCamTextureYUV YuvCamTexture { get; set; }

        void Start()
        {
            YuvCamTexture = new NRRGBCamTextureYUV();
            BindYuvTexture(YuvCamTexture.GetTexture());
            YuvCamTexture.Play();
        }

        /// <summary> Updates this object. </summary>
        void Update()
        {
            if (YuvCamTexture == null)
            {
                return;
            }

            FrameCount.text = YuvCamTexture.FrameCount.ToString();
        }

        /// <summary> Plays this object. </summary>
        public void Play()
        {
            if (YuvCamTexture == null)
            {
                YuvCamTexture = new NRRGBCamTextureYUV();
            }
            YuvCamTexture.Play();

            // The origin texture will be destroyed after call "Stop",
            // Rebind the texture.
            BindYuvTexture(YuvCamTexture.GetTexture());
        }

        /// <summary> Bind yuv texture. </summary>
        /// <param name="frame"> The frame.</param>
        private void BindYuvTexture(NRRGBCamTextureYUV.YUVTextureFrame frame)
        {
            CaptureImage.enabled = true;
            CaptureImage.material.SetTexture("_MainTex", frame.textureY);
            CaptureImage.material.SetTexture("_UTex", frame.textureU);
            CaptureImage.material.SetTexture("_VTex", frame.textureV);
        }

        /// <summary> Pauses this object. </summary>
        public void Pause()
        {
            YuvCamTexture?.Pause();
        }

        /// <summary> Stops this object. </summary>
        public void Stop()
        {
            YuvCamTexture?.Stop();
            YuvCamTexture = null;
            CaptureImage.enabled = false;
        }

        /// <summary> Executes the 'destroy' action. </summary>
        void OnDestroy()
        {
            YuvCamTexture?.Stop();
            YuvCamTexture = null;
        }
    }
}
