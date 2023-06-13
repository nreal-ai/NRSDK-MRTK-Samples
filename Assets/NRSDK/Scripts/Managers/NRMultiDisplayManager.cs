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

    /// <summary> Manager for multi-displays. </summary>
    [HelpURL("https://developer.nreal.ai/develop/unity/customize-phone-controller")]
    public class NRMultiDisplayManager : MonoBehaviour
    {
        /// <summary> The default virtual displayer. </summary>
        [SerializeField] GameObject m_DefaultVirtualDisplayer;
        private NRVirtualDisplayer m_VirtualDisplayer;

        private void Start()
        {
            m_VirtualDisplayer = FindObjectOfType<NRVirtualDisplayer>();
            // Use the customise virtualdisplay if find one.
            if (m_VirtualDisplayer != null)
            {
                return;
            }

            // Use the default virtual display if can not find one.
#if UNITY_EDITOR
            Instantiate(m_DefaultVirtualDisplayer);
#else
            var virtualDisplayer = new GameObject("NRVirtualDisplayer").AddComponent<NRVirtualDisplayer>();
            GameObject.DontDestroyOnLoad(virtualDisplayer.gameObject);
#endif
        }
    }
}
