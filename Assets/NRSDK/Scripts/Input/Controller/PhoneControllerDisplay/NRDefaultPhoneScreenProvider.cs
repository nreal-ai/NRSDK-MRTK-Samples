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

    public class NRDefaultPhoneScreenProvider : NRPhoneScreenProviderBase
    {
        SystemButtonState m_buttonState = new SystemButtonState();
        private static AndroidJavaObject m_VirtualDisplayFragment;
        public class AndroidSystemButtonDataProxy : AndroidJavaProxy, ISystemButtonDataProxy
        {
            private NRPhoneScreenProviderBase m_Provider;

            public AndroidSystemButtonDataProxy(NRPhoneScreenProviderBase provider) : base("ai.nreal.virtualcontroller.ISystemButtonDataReceiver")
            {
                this.m_Provider = provider;
            }

            public void OnUpdate(AndroidJavaObject data)
            {
                SystemButtonState state = new SystemButtonState();
#if UNITY_2019_1_OR_NEWER
                sbyte[] sbuffer = data.Call<sbyte[]>("getRawData");
                byte[] bytes = new byte[sbuffer.Length];
                Buffer.BlockCopy(sbuffer, 0, bytes, 0, bytes.Length);
#else
                byte[] bytes = data.Call<byte[]>("getRawData");
#endif
                state.DeSerialize(bytes);
                m_Provider.OnSystemButtonDataChanged(state);
            }
        }

        public override void OnPreUpdate()
        {
            base.OnPreUpdate();
            m_buttonState.Reset();

            var data = m_VirtualDisplayFragment.Call<AndroidJavaObject>("GetSystemButtonState");
            if (data != null)
            {
                bool btnApp = data.Call<bool>("GetButtonApp");
                bool btnTouch = data.Call<bool>("GetButtonTouch");
                bool btnHome = data.Call<bool>("GetButtonHome");
                float touchX = data.Call<float>("GetTouchX");
                float touchY = data.Call<float>("GetTouchY");
                m_buttonState.Set(btnApp, btnTouch, btnHome, touchX, touchY);

                OnSystemButtonDataChanged(m_buttonState);
            }
        }

        public override void RegistFragment(AndroidJavaObject unityActivity, ISystemButtonDataProxy proxy)
        {
            NRDebugger.Info("[VirtualController] RegistFragment...");
            var VirtualDisplayFragment = new AndroidJavaClass("ai.nreal.virtualcontroller.VirtualControllerFragment");
            m_VirtualDisplayFragment = VirtualDisplayFragment.CallStatic<AndroidJavaObject>("RegistFragment", unityActivity, proxy);
        }

        public static void RegistDebugInfoProxy(AndroidJavaProxy proxy)
        {
            if (m_VirtualDisplayFragment != null)
            {
                m_VirtualDisplayFragment.Call("setDebugInfoProvider", proxy);
            }
        }

        public override ISystemButtonDataProxy CreateAndroidDataProxy()
        {
            // return new AndroidSystemButtonDataProxy(this);
            return null;
        }
    }
}
