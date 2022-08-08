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
using System;

namespace NRKernal
{
    /// <summary> Form for viewing the nr notification. </summary>
    public class NRNotificationWindow : MonoBehaviour
    {
        /// <summary> (Serializable) information about the notification. </summary>
        [Serializable]
        public struct NotificationInfo
        {
            /// <summary> The sprite. </summary>
            public Sprite sprite;
            /// <summary> The title. </summary>
            public string title;
            /// <summary> The message. </summary>
            public string message;
        }

        /// <summary> Information describing the high level. </summary>
        public NotificationInfo m_HighLevelInfo;
        /// <summary> Information describing the middle level. </summary>
        public NotificationInfo m_MiddleLevelInfo;

        /// <summary> The icon. </summary>
        [SerializeField] Image m_Icon;
        /// <summary> The title. </summary>
        [SerializeField] Text m_Title;
        /// <summary> The message. </summary>
        [SerializeField] Text m_Message;
        /// <summary> The confirm control. </summary>
        [SerializeField] Button m_ConfirmBtn;

        protected NRNotificationListener.Level m_Level = NRNotificationListener.Level.Low;
        protected event Action OnConfirm;
        protected float m_Duration = 2f;
        private string m_TitleExtra;
        private string m_MessageExtra;

        /// <summary> Fill data. </summary>
        /// <param name="level">    The level.</param>
        /// <param name="duration"> (Optional) The duration.</param>
        public virtual NRNotificationWindow Build()
        {
            NotificationInfo info;

            switch (m_Level)
            {
                case NRNotificationListener.Level.High:
                    info = m_HighLevelInfo;
                    break;
                case NRNotificationListener.Level.Middle:
                    info = m_MiddleLevelInfo;
                    m_ConfirmBtn?.gameObject.SetActive(false);
                    break;
                case NRNotificationListener.Level.Low:
                default:
                    GameObject.Destroy(gameObject);
                    return this;
            }

            m_Icon.sprite = info.sprite;
            if (!string.IsNullOrEmpty(m_TitleExtra))
            {
                m_Title.text = m_TitleExtra;
            }
            else
            {
                m_Title.text = info.title;
            }

            if (!string.IsNullOrEmpty(m_MessageExtra))
            {
                m_Message.text = m_MessageExtra;
            }
            else
            {
                m_Message.text = info.message;
            }

            m_ConfirmBtn?.onClick.AddListener(() =>
            {
                OnConfirm?.Invoke();
                AutoDestroy();
            });

            if (m_Duration > 0)
            {
                Invoke("AutoDestroy", m_Duration);
            }

            return this;
        }

        public NRNotificationWindow SetTitle(string title)
        {
            this.m_TitleExtra = title;
            return this;
        }

        public NRNotificationWindow SetContent(string content)
        {
            this.m_MessageExtra = content;
            return this;
        }

        public NRNotificationWindow SetLevle(NRNotificationListener.Level level)
        {
            this.m_Level = level;
            return this;
        }

        public NRNotificationWindow SetConfirmAction(Action callback)
        {
            this.OnConfirm += callback;
            return this;
        }

        public NRNotificationWindow SetDuration(float duration)
        {
            this.m_Duration = duration;
            return this;
        }

        /// <summary> Automatic destroy. </summary>
        private void AutoDestroy()
        {
            GameObject.Destroy(gameObject);
        }
    }
}
