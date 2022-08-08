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
    using UnityEngine;
    using UnityEngine.UI;

    public class FPSConfigView : MonoBehaviour
    {
        public SettingRegionTrigger m_TriggerRegion;
        public delegate void OnButtonClick(FirstPersonStreammingCast.OnResponse response);
        public event OnButtonClick OnStreamBtnClicked;
        public event OnButtonClick OnRecordBtnClicked;
        public Button m_StreamBtn;
        public Button m_RecordBtn;
        public Transform m_PanelRoot;
        public Color NormalColor;
        public Color ActiveColor;

        void Start()
        {
            m_StreamBtn.onClick.AddListener(() =>
            {
                OnStreamBtnClicked?.Invoke(OnStreamButtonResponse);
            });

            m_RecordBtn.onClick.AddListener(() =>
            {
                OnRecordBtnClicked?.Invoke(OnRecordButtonResponse);
            });

            m_TriggerRegion.onPointerEnter.AddListener(ShowPanel);
            m_TriggerRegion.onPointerOut.AddListener(HidePanel);

            HidePanel();
        }

        private bool m_IsRecordButtonActive = false;
        private void OnRecordButtonResponse(bool result)
        {
            if (!result)
            {
                return;
            }
            m_IsRecordButtonActive = !m_IsRecordButtonActive;
            m_RecordBtn.GetComponent<Image>().color = m_IsRecordButtonActive ? ActiveColor : NormalColor;
            m_StreamBtn.gameObject.SetActive(!m_IsRecordButtonActive);
            HidePanel();
        }

        private bool m_IsStreamButtonActive = false;
        private void OnStreamButtonResponse(bool result)
        {
            if (!result)
            {
                return;
            }
            m_IsStreamButtonActive = !m_IsStreamButtonActive;
            m_StreamBtn.GetComponent<Image>().color = m_IsStreamButtonActive ? ActiveColor : NormalColor;
            m_RecordBtn.gameObject.SetActive(!m_IsStreamButtonActive);
            HidePanel();
        }

        /// <summary> Shows the panel. </summary>
        private void ShowPanel()
        {
            m_PanelRoot.gameObject.SetActive(true);
        }

        /// <summary> Hides the panel. </summary>
        private void HidePanel()
        {
            m_PanelRoot.gameObject.SetActive(false);
        }
    }
}
