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
    using UnityEngine;

    public class UnityExtendedUtility
    {
        public static RenderTexture CreateRenderTexture(int width, int height, int depth = 24, RenderTextureFormat format = RenderTextureFormat.ARGB32, bool usequaAnti = true)
        {
            // Fixed UNITY_2018_2 editor preview effect for video capture and photo capture.
#if UNITY_2018_2 && UNITY_EDITOR
            var rt = new RenderTexture(width, height, depth, format, NRRenderer.isLinearColorSpace ? RenderTextureReadWrite.Linear : RenderTextureReadWrite.Default);
#else
            var rt = new RenderTexture(width, height, depth, format, NRRenderer.isLinearColorSpace ? RenderTextureReadWrite.sRGB : RenderTextureReadWrite.Default);
#endif
            rt.wrapMode = TextureWrapMode.Clamp;
            if (QualitySettings.antiAliasing > 0 && usequaAnti)
            {
                rt.antiAliasing = QualitySettings.antiAliasing;
            }
            rt.Create();
            return rt;
        }
    }
}
