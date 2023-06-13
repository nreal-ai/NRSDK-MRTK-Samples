/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace NRKernal.NRExamples
{
    public class ObjectEmitter : MonoBehaviour
    {
        [SerializeField]
        Rigidbody obj;
        [SerializeField]
        int maxObjectNum = 100;
        [SerializeField]
        float minHeight;

        List<Rigidbody> activeObjects = new List<Rigidbody>();
        List<Rigidbody> reserveObjects = new List<Rigidbody>();

        void Update()
        {
            for (int i = activeObjects.Count - 1; i >= 0; i--)
            {
                if (activeObjects[i].transform.position.y < minHeight)
                {
                    activeObjects[i].gameObject.SetActive(false);
                    reserveObjects.Add(activeObjects[i]);
                    activeObjects.RemoveAt(i);
                }
            }

            if (NRInput.GetButtonDown(ControllerButton.TRIGGER))
            {
                InstantiateObject();
            }
        }

        void InstantiateObject()
        {
            Rigidbody rigidbody;
            if (activeObjects.Count >= maxObjectNum)
            {
                rigidbody = activeObjects[0];
                activeObjects.RemoveAt(0);
                activeObjects.Add(rigidbody);
            }
            else if (reserveObjects.Count != 0)
            {
                rigidbody = reserveObjects[0];
                rigidbody.gameObject.SetActive(true);
                reserveObjects.RemoveAt(0);
                activeObjects.Add(rigidbody);
            }
            else
            {
                rigidbody = Instantiate(obj, transform);
                activeObjects.Add(rigidbody);
            }
            rigidbody.transform.position = NRFrame.HeadPose.position + NRFrame.HeadPose.forward * 0.3f;
            rigidbody.transform.rotation = NRFrame.HeadPose.rotation;
            rigidbody.velocity = NRFrame.HeadPose.forward * 2f;
        }
    }
}
