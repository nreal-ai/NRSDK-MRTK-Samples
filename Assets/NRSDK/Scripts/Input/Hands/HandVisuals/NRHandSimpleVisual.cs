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
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class NRHandSimpleVisual : MonoBehaviour
    {
        public HandEnum handEnum;
        public GameObject jointPrefab;

        protected readonly Dictionary<HandJointID, Transform> joints = new Dictionary<HandJointID, Transform>();

        private void OnEnable()
        {
            NRInput.Hands.OnHandStatesUpdated += OnHandStatesUpdated;
            NRInput.Hands.OnHandTrackingStopped += OnHandTrackingStopped;
        }

        private void OnDisable()
        {
            NRInput.Hands.OnHandStatesUpdated -= OnHandStatesUpdated;
            NRInput.Hands.OnHandTrackingStopped += OnHandTrackingStopped;
        }

        private void OnHandStatesUpdated()
        {
            UpdateHandVisual();
        }

        private void OnHandTrackingStopped()
        {
            OnHandTrackingLost();
        }

        private void UpdateHandVisual()
        {
            var handState = NRInput.Hands.GetHandState(handEnum);
            if (handState != null && handState.isTracked)
            {
                foreach (var jointID in handState.jointsPoseDict.Keys)
                {
                    Transform jointTransform = null;
                    if (joints.TryGetValue(jointID, out jointTransform))
                    {
                        jointTransform.gameObject.SetActive(true);
                        //Debug.LogError("jointTransform position = " + jointTransform.position.ToString("f2"));
                    }
                    else
                    {
                        GameObject jointObj = CreateJointObj(jointID);
                        if(jointObj == null)
                        {
                            Debug.LogError("Create joint failed, joint ID = " + jointID);
                            continue;
                        }
                        jointTransform = jointObj.transform;
                        jointTransform.name = jointID.ToString() + " Transform";
                        jointTransform.SetParent(transform);
                        jointTransform.localScale = Vector3.one * 0.01f;
                        joints.Add(jointID, jointTransform);
                    }
                    if (jointTransform)
                    {
                        jointTransform.position = handState.jointsPoseDict[jointID].position;
                        jointTransform.rotation = handState.jointsPoseDict[jointID].rotation;
                    }
                }
            }
            else
            {
                OnHandTrackingLost();
            }
        }

        private void OnHandTrackingLost()
        {
            foreach (Transform jointTransform in joints.Values)
            {
                if (jointTransform)
                {
                    jointTransform.gameObject.SetActive(false);
                }
            }
        } 

        private GameObject CreateJointObj(HandJointID handJointID)
        {
            return Instantiate(jointPrefab);
        }

        public void SetHandCollidersEnabled(bool isEnabled)
        {
            var collidersInChildren = transform.GetComponentsInChildren<Collider>(true);
            if(collidersInChildren != null)
            {
                for (int i = 0; i < collidersInChildren.Length; i++)
                {
                    collidersInChildren[i].enabled = isEnabled;
                }
            }
        }
    }
}
