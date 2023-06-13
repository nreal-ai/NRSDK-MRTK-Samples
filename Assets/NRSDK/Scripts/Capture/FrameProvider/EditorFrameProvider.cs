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
    using NRKernal;
    using UnityEngine;

    /// <summary> An editor frame provider. </summary>
    public class EditorFrameProvider : NullDataFrameProvider
    {
        public EditorFrameProvider() : base(NativeConstants.RECORD_FPS_DEFAULT)
        {
            Texture temp = Resources.Load<Texture2D>("Record/Textures/captureDefault");
            var mat = new Material(Resources.Load<Shader>("Record/Shaders/NRBackground"));
            RenderTexture rt = UnityExtendedUtility.CreateRenderTexture(temp.width, temp.height, 24, RenderTextureFormat.ARGB32, false);
            Graphics.Blit(temp, rt, mat);

            m_DefaultFrame.textures = new Texture[1];
            m_DefaultFrame.textureType = TextureType.RGB;
            m_DefaultFrame.textures[0] = rt;
        }
    }
}
