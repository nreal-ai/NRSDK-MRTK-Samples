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

    public class HandStateUtility
    {
        public static float GetIndexFingerCurlStrength(HandState handState)
        {
            Pose wristPose;
            Pose fingerTipPose;
            Pose fingerProximal;
            if (handState.jointsPoseDict.TryGetValue(HandJointID.Wrist, out wristPose) &&
               handState.jointsPoseDict.TryGetValue(HandJointID.IndexTip, out fingerTipPose) &&
               handState.jointsPoseDict.TryGetValue(HandJointID.IndexProximal, out fingerProximal))
            {
                return CalculateCurl(wristPose.position, fingerProximal.position, fingerTipPose.position);
            }
            return 0f;
        }

        public static float GetMiddleFingerCurlStrength(HandState handState)
        {
            Pose wristPose;
            Pose fingerTipPose;
            Pose fingerProximal;
            if (handState.jointsPoseDict.TryGetValue(HandJointID.Wrist, out wristPose) &&
               handState.jointsPoseDict.TryGetValue(HandJointID.MiddleTip, out fingerTipPose) &&
               handState.jointsPoseDict.TryGetValue(HandJointID.MiddleProximal, out fingerProximal))
            {
                return CalculateCurl(wristPose.position, fingerProximal.position, fingerTipPose.position);
            }
            return 0f;
        }

        public static float GetRingFingerCurlStrength(HandState handState)
        {
            Pose wristPose;
            Pose fingerTipPose;
            Pose fingerProximal;
            if (handState.jointsPoseDict.TryGetValue(HandJointID.Wrist, out wristPose) &&
               handState.jointsPoseDict.TryGetValue(HandJointID.RingTip, out fingerTipPose) &&
               handState.jointsPoseDict.TryGetValue(HandJointID.RingProximal, out fingerProximal))
            {
                return CalculateCurl(wristPose.position, fingerProximal.position, fingerTipPose.position);
            }
            return 0f;
        }

        public static float GetPinkyFingerCurlStrength(HandState handState)
        {
            Pose wristPose;
            Pose fingerTipPose;
            Pose fingerProximal;
            if (handState.jointsPoseDict.TryGetValue(HandJointID.Wrist, out wristPose) &&
               handState.jointsPoseDict.TryGetValue(HandJointID.PinkyTip, out fingerTipPose) &&
               handState.jointsPoseDict.TryGetValue(HandJointID.PinkyProximal, out fingerProximal))
            {
                return CalculateCurl(wristPose.position, fingerProximal.position, fingerTipPose.position);
            }
            return 0f;
        }

        /// <summary>
        /// Curl calculation of a finger based on the angle made by vectors wristToFingerKuncle and fingerKuckleToFingerTip.
        /// </summary>
        private static float CalculateCurl(Vector3 wristPosition, Vector3 fingerKnucklePosition, Vector3 fingerTipJoint)
        {
            var palmToFinger = (fingerKnucklePosition - wristPosition).normalized;
            var fingerKnuckleToTip = (fingerKnucklePosition - fingerTipJoint).normalized;

            var curl = Vector3.Dot(fingerKnuckleToTip, palmToFinger);
            // Redefining the range from [-1,1] to [0,1]
            curl = (curl + 1) / 2.0f;
            return curl;
        }

        /// <summary>
        /// Pinch calculation of the index finger with the thumb based on the distance between the finger tip and the thumb tip.
        /// 4 cm (0.04 unity units) is the treshold for fingers being far apart and pinch being read as 0.
        /// </summary>
        /// <param name="handedness">Handedness to query joint pose against.</param>
        /// <returns> Float ranging from 0 to 1. 0 if the thumb and finger are not pinched together, 1 if thumb finger are pinched together</returns>
        private const float IndexThumbSqrMagnitudeThreshold = 0.0016f;
        public static float GetIndexFingerPinchStrength(HandState handState)
        {
            if (!handState.isTracked)
            {
                return 0f;
            }

            if (handState.currentGesture == HandGesture.Grab)
            {
                return 1f;
            }

            var indexPosition = handState.GetJointPose(HandJointID.IndexTip).position;
            var thumbPosition = handState.GetJointPose(HandJointID.ThumbTip).position;
            var distanceVector = indexPosition - thumbPosition;
            float indexThumbSqrMagnitude = distanceVector.sqrMagnitude;
            float pinchStrength = Mathf.Clamp(1 - indexThumbSqrMagnitude / IndexThumbSqrMagnitudeThreshold, 0.0f, 1.0f);
            return pinchStrength;
        }

        /*
         * System gesture should satisfy these conditions below:
         * 1,Palm face to camera 
         * 2,Keep thumb finger curl to palm, other fingers straight 
         * 3,If left hand, fingers should point to right. If right hand, to left.
        */
        public static bool IsSystemGesture(HandState handState)
        {
            if ((!handState.isTracked) || handState.isPinching || handState.pointerPoseValid)
            {
                return false;
            }

            //palm should face to camera
            var palmPose = handState.GetJointPose(HandJointID.Palm);
            Vector3 palmBackward = -palmPose.forward;
            Transform cameraTransform = NRInput.CameraCenter;
            bool isPalmFacingCamera = Vector3.Angle(palmBackward, cameraTransform.forward) < 35f;
            if (!isPalmFacingCamera)
            {
                return false;
            }

            //fingers should point to right or left
            var fingersShouldPointTo = cameraTransform.right * (handState.handEnum == HandEnum.RightHand ? -1f : 1f);
            var fingersNowPointTo = palmPose.up;
            var isPointToCorrect = Vector3.Angle(fingersShouldPointTo, fingersNowPointTo) < 35f;
            if (!isPointToCorrect)
            {
                return false;
            }

            //thumb should curl to palm
            var thumbToward = handState.GetJointPose(HandJointID.ThumbDistal).up;
            var indexToward = handState.GetJointPose(HandJointID.IndexProximal).up;
            var planeNormal = Vector3.Cross(thumbToward, indexToward);
            var signByHand = handState.handEnum == HandEnum.RightHand ? 1f : -1f;
            if (Vector3.Dot(planeNormal, palmPose.forward * signByHand) < 0f)
            {
                return false;
            }

            //other fingers should be straight
            float indexCurlStr = GetIndexFingerCurlStrength(handState);
            float middleCurlStr = GetMiddleFingerCurlStrength(handState);
            float ringCurlStr = GetRingFingerCurlStrength(handState);
            if (indexCurlStr < 0.2f
                && middleCurlStr < 0.2f
                && ringCurlStr < 0.2f)
            {
                return true;
            }
            return false;
        }
    }
}
