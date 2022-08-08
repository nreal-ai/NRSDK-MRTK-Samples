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

    public class NRPhoneDisplayReplayceTool : MonoBehaviour
    {
        public virtual NRPhoneScreenProviderBase CreatePhoneScreenProvider()
        {
            return new NRDefaultPhoneScreenProvider();
        }
    }
}
