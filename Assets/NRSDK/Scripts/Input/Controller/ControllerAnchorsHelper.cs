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


    /// <summary> Values that represent controller anchor enums. </summary>
    public enum ControllerAnchorEnum
    {
        /// <summary> An enum constant representing the gaze pose tracker anchor option. </summary>
        GazePoseTrackerAnchor,
        /// <summary> An enum constant representing the right pose tracker anchor option. </summary>
        RightPoseTrackerAnchor,
        /// <summary> An enum constant representing the left pose tracker anchor option. </summary>
        LeftPoseTrackerAnchor,
        /// <summary> An enum constant representing the right model anchor option. </summary>
        RightModelAnchor,
        /// <summary> An enum constant representing the left model anchor option. </summary>
        LeftModelAnchor,
        /// <summary> An enum constant representing the right laser anchor option. </summary>
        RightLaserAnchor,
        /// <summary> An enum constant representing the left laser anchor option. </summary>
        LeftLaserAnchor
    }

    /// <summary>
    /// The class is for user to easily get the transform of common controller anchors. </summary>
    [HelpURL("https://developer.nreal.ai/develop/unity/controller")]
    public class ControllerAnchorsHelper : MonoBehaviour
    {
        /// <summary> The gaze pose tracker anchor. </summary>
        [SerializeField]
        private Transform m_GazePoseTrackerAnchor;
        /// <summary> The right pose tracker anchor. </summary>
        [SerializeField]
        private Transform m_RightPoseTrackerAnchor;
        /// <summary> The left pose tracker anchor. </summary>
        [SerializeField]
        private Transform m_LeftPoseTrackerAnchor;
        /// <summary> The right model anchor. </summary>
        [SerializeField]
        private Transform m_RightModelAnchor;
        /// <summary> The left model anchor. </summary>
        [SerializeField]
        private Transform m_LeftModelAnchor;
        /// <summary> The right laser anchor. </summary>
        [SerializeField]
        private Transform m_RightLaserAnchor;
        /// <summary> The left laser anchor. </summary>
        [SerializeField]
        private Transform m_LeftLaserAnchor;

        /// <summary> Gets an anchor. </summary>
        /// <param name="anchorEnum"> The anchor enum.</param>
        /// <returns> The anchor. </returns>
        public Transform GetAnchor(ControllerAnchorEnum anchorEnum)
        {
            switch (anchorEnum)
            {
                case ControllerAnchorEnum.GazePoseTrackerAnchor:
                    return m_GazePoseTrackerAnchor;
                case ControllerAnchorEnum.RightPoseTrackerAnchor:
                    return m_RightPoseTrackerAnchor;
                case ControllerAnchorEnum.LeftPoseTrackerAnchor:
                    return m_LeftPoseTrackerAnchor;
                case ControllerAnchorEnum.RightModelAnchor:
                    return m_RightModelAnchor;
                case ControllerAnchorEnum.LeftModelAnchor:
                    return m_LeftModelAnchor;
                case ControllerAnchorEnum.RightLaserAnchor:
                    return m_RightLaserAnchor;
                case ControllerAnchorEnum.LeftLaserAnchor:
                    return m_LeftLaserAnchor;
                default:
                    break;
            }
            return null;
        }
    }
}