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
    using System;
    using UnityEngine;

    /// <summary> A trackable image in the real world detected by NRInternel. </summary>
    public class NRTrackableImage : NRTrackable
    {
        internal NRTrackableImageSubsystem TrackableImageSubsystem
        {
            get
            {
                return NRSessionManager.Instance.TrackableFactory.TrackableImageSubsystem;
            }
        }

        /// <summary> Constructor. </summary>
        /// <param name="nativeHandle">    Handle of the native.</param>
        /// <param name="nativeInterface"> The native interface.</param>
        internal NRTrackableImage(UInt64 nativeHandle) : base(nativeHandle)
        {
        }

        /// <summary>
        /// Gets the position and orientation of the marker's center in Unity world space. </summary>
        /// <returns> The center pose. </returns>
        public override Pose GetCenterPose()
        {
            if (NRFrame.SessionStatus != SessionState.Running)
            {
                return Pose.identity;
            }
            var native_pose = TrackableImageSubsystem.GetCenterPose(TrackableNativeHandle);
            return ConversionUtility.ApiWorldToUnityWorld(native_pose);
        }

        /// <summary> Gets the width of marker. </summary>
        /// <value> The extent x coordinate. </value>
        public float ExtentX
        {
            get
            {
                return Size.x;
            }
        }

        /// <summary> Gets the height of marker. </summary>
        /// <value> The extent z coordinate. </value>
        public float ExtentZ
        {
            get
            {
                return Size.y;
            }
        }

        /// <summary> Get the size of trackable image. size of trackable imag(width、height). </summary>
        /// <value> The size. </value>
        public Vector2 Size
        {
            get
            {
                if (NRFrame.SessionStatus != SessionState.Running)
                {
                    return Vector2.zero;
                }
                return TrackableImageSubsystem.GetSize(TrackableNativeHandle);
            }
        }
    }
}
