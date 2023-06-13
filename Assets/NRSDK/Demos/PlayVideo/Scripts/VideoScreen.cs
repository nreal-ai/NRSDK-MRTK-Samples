/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

using UnityEngine;

namespace NRKernal.NRExamples
{
    public class VideoScreen
    {
        protected readonly Vector2 LEFT_RIGHT_TEXTURE_SCALE = new Vector2(0.5f, 1f);
        protected readonly Vector2 LEFT_TEXTURE_OFFSET = new Vector2(0.0f, 0.0f);
        protected readonly Vector2 RIGHT_TEXTURE_OFFSET = new Vector2(0.5f, 0.0f);

        protected Shader m_Shader;
        protected Texture m_Texture;

        public VideoScreen()
        {
            CreateShader();
        }

        public virtual void SetContent(Texture content)
        {
            m_Texture = content;
        }

        public virtual void SetScreen(GameObject screen)
        {
            MeshRenderer[] render = screen.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < render.Length; i++)
            {
                render[i].material = new Material(m_Shader);
            }
        }

        protected void CreateShader()
        {
            m_Shader = Resources.Load<Shader>("Shaders/VideoShader");
        }
    }

    public class NormalScreen : VideoScreen
    {
        private Material m_Mat;

        public override void SetScreen(GameObject screen)
        {
            MeshRenderer[] render = screen.GetComponentsInChildren<MeshRenderer>();
            m_Mat = new Material(m_Shader);
            render[0].material = m_Mat;
        }

        public override void SetContent(Texture content)
        {
            base.SetContent(content);
            m_Mat.SetTexture("_MainTex", m_Texture);
        }
    }

    public class LeftRightScreen : VideoScreen
    {
        private Material m_LeftMat;
        private Material m_RightMat;

        public override void SetScreen(GameObject screen)
        {
            MeshRenderer[] render = screen.GetComponentsInChildren<MeshRenderer>();
            m_LeftMat = new Material(m_Shader);
            m_LeftMat.mainTextureScale = LEFT_RIGHT_TEXTURE_SCALE;
            m_LeftMat.mainTextureOffset = LEFT_TEXTURE_OFFSET;
            m_RightMat = new Material(m_Shader);
            m_RightMat.mainTextureScale = LEFT_RIGHT_TEXTURE_SCALE;
            m_RightMat.mainTextureOffset= RIGHT_TEXTURE_OFFSET;
            render[0].material = m_LeftMat;
            render[1].material = m_RightMat;
        }

        public override void SetContent(Texture content)
        {
            base.SetContent(content);
            m_LeftMat.SetTexture("_MainTex", m_Texture);
            m_RightMat.SetTexture("_MainTex", m_Texture);
        }
    }
}
