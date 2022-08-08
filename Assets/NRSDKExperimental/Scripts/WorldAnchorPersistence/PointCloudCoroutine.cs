/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.Experimental.Persistence
{
    using UnityEngine;

    internal class PointCloudCoroutine : MonoBehaviour
    {
        private static PointCloudCoroutine _instance;

        public static PointCloudCoroutine Instance
        {
            get
            {
                if (!_instance)
                {
                    GameObject pointCloudObj = new GameObject("PointCloudCoroutine");
                    _instance = pointCloudObj.AddComponent<PointCloudCoroutine>();
                    DontDestroyOnLoad(pointCloudObj);
                }
                return _instance;
            }
        }
    }
}
