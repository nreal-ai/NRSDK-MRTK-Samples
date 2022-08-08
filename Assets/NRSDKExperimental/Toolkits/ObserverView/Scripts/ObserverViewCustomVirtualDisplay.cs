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

    /// <summary> An observer view custom virtual display. </summary>
    public class ObserverViewCustomVirtualDisplay : MonoBehaviour
    {
        /// <summary> The show control. </summary>
        public Button showBtn;
        /// <summary> The hide control. </summary>
        public Button hideBtn;
        /// <summary> The base controller panel. </summary>
        public GameObject baseControllerPanel;

        /// <summary> Starts this object. </summary>
        private void Start()
        {
            showBtn.onClick.AddListener(() => { SetBaseControllerEnabled(true); });
            hideBtn.onClick.AddListener(() => { SetBaseControllerEnabled(false); });
        }

        /// <summary> Sets base controller enabled. </summary>
        /// <param name="isEnabled"> True if is enabled, false if not.</param>
        private void SetBaseControllerEnabled(bool isEnabled)
        {
            baseControllerPanel.SetActive(isEnabled);
        }
    }
}
