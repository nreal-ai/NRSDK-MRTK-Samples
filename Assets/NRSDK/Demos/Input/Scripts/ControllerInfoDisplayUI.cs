/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

using System;
using UnityEngine;
using UnityEngine.UI;

namespace NRKernal.NRExamples
{
    /// <summary> A controller information display user interface. </summary>
    public class ControllerInfoDisplayUI : MonoBehaviour
    {
        /// <summary> The main information text. </summary>
        public Text mainInfoText;
        /// <summary> The extra information text. </summary>
        public Text extraInfoText;

        /// <summary> The extra information string. </summary>
        private string m_ExtraInfoStr;
        /// <summary> The maximum line. </summary>
        private int m_MaxLine = 20;
        /// <summary> The current debug hand. </summary>
        private ControllerHandEnum m_CurrentDebugHand = ControllerHandEnum.Right;

        private Transform m_CenterCamera;
        private Transform CenterCamera
        {
            get
            {
                if (m_CenterCamera == null)
                {
                    if (NRSessionManager.Instance.CenterCameraAnchor != null)
                    {
                        m_CenterCamera = NRSessionManager.Instance.CenterCameraAnchor;
                    }
                    else if (Camera.main != null)
                    {
                        m_CenterCamera = Camera.main.transform;
                    }
                }
                return m_CenterCamera;
            }
        }

        /// <summary> Updates this object. </summary>
        private void Update()
        {
            if (NRInput.GetAvailableControllersCount() < 2)
            {
                m_CurrentDebugHand = NRInput.DomainHand;
            }
            else
            {
                if (NRInput.GetButtonDown(ControllerHandEnum.Right, ControllerButton.TRIGGER))
                {
                    m_CurrentDebugHand = ControllerHandEnum.Right;
                }
                else if (NRInput.GetButtonDown(ControllerHandEnum.Left, ControllerButton.TRIGGER))
                {
                    m_CurrentDebugHand = ControllerHandEnum.Left;
                }
            }

            if (NRInput.GetButtonDown(m_CurrentDebugHand, ControllerButton.TRIGGER))
                AddExtraInfo("trigger_btn_down");

            if (NRInput.GetButtonDown(m_CurrentDebugHand, ControllerButton.HOME))
                AddExtraInfo("home_btn_down");

            if (NRInput.GetButtonDown(m_CurrentDebugHand, ControllerButton.APP))
                AddExtraInfo("app_btn_down");

            if (NRInput.GetButtonUp(m_CurrentDebugHand, ControllerButton.TRIGGER))
                AddExtraInfo("trigger_btn_up");

            if (NRInput.GetButtonUp(m_CurrentDebugHand, ControllerButton.HOME))
                AddExtraInfo("home_btn_up");

            if (NRInput.GetButtonUp(m_CurrentDebugHand, ControllerButton.APP))
                AddExtraInfo("app_btn_up");

            FollowMainCam();
            RefreshInfoTexts();
        }

        /// <summary> Follow main camera. </summary>
        private void FollowMainCam()
        {
            transform.position = CenterCamera.position;
            transform.rotation = CenterCamera.rotation;
        }

        /// <summary> Refresh information texts. </summary>
        private void RefreshInfoTexts()
        {
            mainInfoText.text =
                "controller count: " + NRInput.GetAvailableControllersCount().ToString() + "\n"
                + "type: " + NRInput.GetControllerType().ToString() + "\n"
                + "current debug hand: " + m_CurrentDebugHand.ToString() + "\n"
                + "position available: " + NRInput.GetControllerAvailableFeature(ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_POSITION).ToString() + "\n"
                + "rotation available: " + NRInput.GetControllerAvailableFeature(ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_ROTATION).ToString() + "\n"
                + "gyro available: " + NRInput.GetControllerAvailableFeature(ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_GYRO).ToString() + "\n"
                + "accel available: " + NRInput.GetControllerAvailableFeature(ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_ACCEL).ToString() + "\n"
                + "mag available: " + NRInput.GetControllerAvailableFeature(ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_MAG).ToString() + "\n"
                + "battery available: " + NRInput.GetControllerAvailableFeature(ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_BATTERY).ToString() + "\n"
                + "vibration available: " + NRInput.GetControllerAvailableFeature(ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_HAPTIC_VIBRATE).ToString() + "\n"
                + "rotation: " + NRInput.GetRotation(m_CurrentDebugHand).ToString("F3") + "\n"
                + "position: " + NRInput.GetPosition(m_CurrentDebugHand).ToString("F3") + "\n"
                + "touch: " + NRInput.GetTouch(m_CurrentDebugHand).ToString("F3") + "\n"
                + "trigger button: " + NRInput.GetButton(m_CurrentDebugHand, ControllerButton.TRIGGER).ToString() + "\n"
                + "home button: " + NRInput.GetButton(m_CurrentDebugHand, ControllerButton.HOME).ToString() + "\n"
                + "app button: " + NRInput.GetButton(m_CurrentDebugHand, ControllerButton.APP).ToString() + "\n"
                + "grip button: " + NRInput.GetButton(m_CurrentDebugHand, ControllerButton.GRIP).ToString() + "\n"
                + "touchpad button: " + NRInput.GetButton(m_CurrentDebugHand, ControllerButton.TOUCHPAD_BUTTON).ToString() + "\n"
                + "gyro: " + NRInput.GetGyro(m_CurrentDebugHand).ToString("F3") + "\n"
                + "accel: " + NRInput.GetAccel(m_CurrentDebugHand).ToString("F3") + "\n"
                + "mag: " + NRInput.GetMag(m_CurrentDebugHand).ToString("F3") + "\n"
                + "battery: " + NRInput.GetControllerBattery(m_CurrentDebugHand);
            extraInfoText.text = m_ExtraInfoStr;
            //Debug.Log("istouching:" + NRInput.IsTouching() + " value:" + NRInput.GetTouch(m_CurrentDebugHand).ToString("F3"));
            PrintInputState();
        }

        private void PrintInputState()
        {
            Debug.LogFormat("istouching:{0} getbutton app:{1} trigger:{2} home:{3} \n" +
                "getbuttondown app:{4} trigger:{5} home:{6} \n" +
                "getbuttonup app:{7} trigger:{8} home:{9} \n" +
                "origin touch:[{10}] gettouch:{11}", NRInput.IsTouching(), NRInput.GetButton(ControllerButton.APP), NRInput.GetButton(ControllerButton.TRIGGER), NRInput.GetButton(ControllerButton.HOME)
                , NRInput.GetButtonDown(ControllerButton.APP), NRInput.GetButtonDown(ControllerButton.TRIGGER), NRInput.GetButtonDown(ControllerButton.HOME)
                , NRInput.GetButtonUp(ControllerButton.APP), NRInput.GetButtonUp(ControllerButton.TRIGGER), NRInput.GetButtonUp(ControllerButton.HOME)
                , NRVirtualDisplayer.SystemButtonState.ToString(), NRInput.GetTouch(m_CurrentDebugHand).ToString("F3"));
        }

        /// <summary> Adds an extra information. </summary>
        /// <param name="infoStr"> The information string.</param>
        private void AddExtraInfo(string infoStr)
        {
            if (string.IsNullOrEmpty(infoStr))
                return;
            if (string.IsNullOrEmpty(m_ExtraInfoStr))
                m_ExtraInfoStr = infoStr;
            else
                m_ExtraInfoStr = m_ExtraInfoStr + Environment.NewLine + infoStr;
            int count = m_ExtraInfoStr.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Length;
            if (count > m_MaxLine)
                m_ExtraInfoStr = m_ExtraInfoStr.Substring(m_ExtraInfoStr.IndexOf(Environment.NewLine) + Environment.NewLine.Length);
        }
    }
}
