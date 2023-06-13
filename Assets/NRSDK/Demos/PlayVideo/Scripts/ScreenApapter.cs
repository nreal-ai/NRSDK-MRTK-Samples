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
    public class ScreenApapter : MonoBehaviour
    {
        public enum ScreenType
        {
            Normal,
            LR3D
        }

        public ScreenType screenType = ScreenType.Normal;

        public GameObject normalScreen;
        public GameObject leftRightScreen;

        private VideoScreen m_Screen;

        private void Awake()
        {
            SetScreen();
        }

        public void SetScreen()
        {
            if (screenType == ScreenType.Normal)
            {
                normalScreen.SetActive(true);
                leftRightScreen.SetActive(false);
                m_Screen = new NormalScreen();
                m_Screen.SetScreen(normalScreen);
            }
            else
            {
                normalScreen.SetActive(false);
                leftRightScreen.SetActive(true);
                m_Screen = new LeftRightScreen();
                m_Screen.SetScreen(leftRightScreen);
            }
        }

        public void SetContent(Texture content)
        {
            m_Screen.SetContent(content);
        }
    }
}



