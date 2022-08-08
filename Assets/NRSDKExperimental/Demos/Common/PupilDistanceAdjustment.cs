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
using UnityEngine.UI;

namespace NRKernal.Experimental.NRExamples
{
    public class PupilDistanceAdjustment : MonoBehaviour
    {
        [SerializeField]
        private Text m_Value;
        [SerializeField]
        private Slider m_DistanceAdjSlider;
        [SerializeField]
        private Toggle m_NativeToggle;

        private Transform leftCamera;
        private Transform rightCamera;
        private Vector3 centerPose;
        private Vector3 forward;

        IEnumerator Start()
        {
            m_NativeToggle.isOn = false;
            leftCamera = NRSessionManager.Instance.NRHMDPoseTracker.leftCamera.transform;
            rightCamera = NRSessionManager.Instance.NRHMDPoseTracker.rightCamera.transform;
            while (NRFrame.SessionStatus != SessionState.Running)
            {
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(0.2f);

            float default_distance = Vector3.Distance(leftCamera.localPosition, rightCamera.localPosition);
            centerPose = (leftCamera.localPosition + rightCamera.localPosition) * 0.5f;
            forward = (rightCamera.localPosition - leftCamera.localPosition).normalized;
            m_Value.text = string.Format("Value:{0}", default_distance);
            m_DistanceAdjSlider.maxValue = 0.075f;
            m_DistanceAdjSlider.minValue = 0.05f;
            m_DistanceAdjSlider.value = default_distance;
            m_DistanceAdjSlider.onValueChanged.AddListener(OnValueChange);
        }

        void OnValueChange(float val)
        {
            leftCamera.localPosition = centerPose - forward * 0.5f * val;
            rightCamera.localPosition = centerPose + forward * 0.5f * val;
            // if (m_NativeToggle.isOn)
            //     NRDevice.Subsystem.SetEyeIPD((int)(val*100));
            m_Value.text = val.ToString();
        }
    }
}
