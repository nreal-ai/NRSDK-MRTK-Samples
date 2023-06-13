/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

using UnityEngine;
using UnityEngine.Video;

namespace NRKernal.NRExamples
{
    public class VideoPlayerDemo : MonoBehaviour
    {
        public VideoPlayer videoPlayer;

        public ScreenApapter screenApapter;

        private void Start()
        {
            videoPlayer.Play();
            videoPlayer.prepareCompleted += VideoPlayer_prepareCompleted;
        }

        private void VideoPlayer_prepareCompleted(VideoPlayer source)
        {
            screenApapter.SetContent(source.texture);
        }
    }
}

