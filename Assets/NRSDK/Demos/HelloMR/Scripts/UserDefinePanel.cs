/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

using System.Collections;
using UnityEngine;

namespace NRKernal.NRExamples
{
    /// <summary> Panel for editing the user define. </summary>
    [HelpURL("https://developer.nreal.ai/develop/unity/customize-phone-controller")]
    public class UserDefinePanel : MonoBehaviour
    {
        /// <summary> The user define panel. </summary>
        public GameObject m_UserDefinePanel;

        /// <summary> Starts this object. </summary>
        void Start()
        {
            StartCoroutine(RigistUserDefinePanel());
        }

        /// <summary> Rigist user define panel. </summary>
        /// <returns> An IEnumerator. </returns>
        private IEnumerator RigistUserDefinePanel()
        {
            while (GameObject.FindObjectOfType<NRVirtualDisplayer>() == null)
            {
                yield return new WaitForEndOfFrame();
            }
            var virtualdisplayer = GameObject.FindObjectOfType<NRVirtualDisplayer>();

            if (virtualdisplayer.transform.GetComponentInChildren<Canvas>() == null)
            {
                yield break;
            }
            var root = virtualdisplayer.transform.GetComponentInChildren<Canvas>().transform;
            Instantiate(m_UserDefinePanel, root);
        }
    }
}