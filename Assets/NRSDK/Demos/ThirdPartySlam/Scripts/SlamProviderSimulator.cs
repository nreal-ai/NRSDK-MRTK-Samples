/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NRKernal;

/// <summary>
/// Simple demo for external slam provider. Try to move host gameobject in unity editor.
/// </summary>
public class SlamProviderSimulator : MonoBehaviour, IExternSlamProvider
{
    // Start is called before the first frame update
    void Start()
    {
        NRSessionManager.Instance.NRHMDPoseTracker.RegisterSlamProvider(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Pose GetHeadPoseAtTime(UInt64 timeStamp)
    {
        return new Pose(transform.position, transform.rotation);
    }
}
