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

namespace NRKernal.Experimental.Persistence
{
    /// <summary>
    /// Uses 4 frame corner objects to visualize an TrackingImage.
    /// </summary> </summary>
    public class TrackingImageVisualizerForAnchor : MonoBehaviour
    {
        public Action OnConfirm;
        /// <summary> The TrackingImage to visualize. </summary>
        public NRTrackableImage Image;

        /// <summary>
        /// A model for the lower left corner of the frame to place when an image is detected. </summary>
        public GameObject FrameLowerLeft;

        /// <summary>
        /// A model for the lower right corner of the frame to place when an image is detected. </summary>
        public GameObject FrameLowerRight;

        /// <summary>
        /// A model for the upper left corner of the frame to place when an image is detected. </summary>
        public GameObject FrameUpperLeft;

        /// <summary>
        /// A model for the upper right corner of the frame to place when an image is detected. </summary>
        public GameObject FrameUpperRight;

        /// <summary> The axis. </summary>
        public GameObject Axis;

        /// <summary> The save control. </summary>
        public Button SaveButton;

        void Start()
        {
            SaveButton.onClick.AddListener(new UnityEngine.Events.UnityAction(() =>
            {
                OnConfirm?.Invoke();
            }));
        }

        /// <summary> Updates this object. </summary>
        public void Update()
        {
            if (Image == null || Image.GetTrackingState() != TrackingState.Tracking)
            {
                FrameLowerLeft.SetActive(false);
                FrameLowerRight.SetActive(false);
                FrameUpperLeft.SetActive(false);
                FrameUpperRight.SetActive(false);
                Axis.SetActive(false);
                SaveButton.gameObject.SetActive(false);
                return;
            }

            float halfWidth = Image.ExtentX / 2;
            float halfHeight = Image.ExtentZ / 2;
            FrameLowerLeft.transform.localPosition = (halfWidth * Vector3.left) + (halfHeight * Vector3.back);
            FrameLowerRight.transform.localPosition = (halfWidth * Vector3.right) + (halfHeight * Vector3.back);
            FrameUpperLeft.transform.localPosition = (halfWidth * Vector3.left) + (halfHeight * Vector3.forward);
            FrameUpperRight.transform.localPosition = (halfWidth * Vector3.right) + (halfHeight * Vector3.forward);
            SaveButton.transform.localPosition = (halfHeight + 0.1f) * Vector3.back;

            var center = Image.GetCenterPose();
            transform.position = center.position;
            transform.rotation = center.rotation;

            FrameLowerLeft.SetActive(true);
            FrameLowerRight.SetActive(true);
            FrameUpperLeft.SetActive(true);
            FrameUpperRight.SetActive(true);
            SaveButton.gameObject.SetActive(true);
            Axis.SetActive(true);
        }
    }
}
