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
    using UnityEngine.UI;

    /// <summary> A nr glasses initialize error tip. </summary>
    public class NRGlassesInitErrorTip : MonoBehaviour
    {
        /// <summary> Event queue for all listeners interested in OnPreComfirm events. </summary>
        public static event Action OnPreComfirm;
        /// <summary> Event queue for all listeners interested in OnConfirm events. </summary>
        public event Action OnConfirm;
        /// <summary> The confirm control. </summary>
        public Button m_ConfirmBtn;
        /// <summary> The tips. </summary>
        public Text m_Tips;

        /// <summary> Initializes this object. </summary>
        /// <param name="msg">     The message.</param>
        /// <param name="confirm"> The confirm.</param>
        public virtual void Init(string msg, Action confirm)
        {
            m_Tips.text = msg;
            OnConfirm += confirm;
            m_ConfirmBtn.onClick.AddListener(() =>
            {
                OnConfirm?.Invoke();
            });

            Invoke("AutoConfirm", 5f);
        }

        /// <summary> Starts this object. </summary>
        private void Start()
        {
            var inputmodule = GameObject.FindObjectOfType<NRInputModule>();
            if (inputmodule != null)
            {
                GameObject.Destroy(inputmodule.gameObject);
            }

            OnPreComfirm?.Invoke();
        }

        /// <summary> Automatic confirm. </summary>
        private void AutoConfirm()
        {
            OnConfirm?.Invoke();
        }
    }
}
