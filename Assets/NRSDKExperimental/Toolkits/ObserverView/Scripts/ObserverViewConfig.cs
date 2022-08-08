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

    /// <summary> An observer view configuration. </summary>
    public struct ObserverViewConfig
    {
        /// <summary> The server IP. </summary>
        [SerializeField]
        public string serverIP;
        [SerializeField]
        public bool useDebugUI;
        [SerializeField]
        public Vector3 offset;

        /// <summary> Constructor. </summary>
        /// <param name="serverip"> The serverip.</param>
        /// <param name="usedebug"> (Optional) True to usedebug.</param>
        public ObserverViewConfig(string serverip, Vector3 offset, bool usedebug = false)
        {
            this.serverIP = serverip;
            this.useDebugUI = usedebug;
            this.offset = offset;
        }

        public override string ToString()
        {
            return serverIP;
        }
    }
}
