using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NRKernal;

namespace NRSDK.MixedReality.Toolkit.Input
{
    using TrackingState = Microsoft.MixedReality.Toolkit.TrackingState;

    [MixedRealityController(SupportedControllerType.ArticulatedHand, new[] { Handedness.Left, Handedness.Right })]
    public class NRSDKArticulatedHand : BaseHand
    {
        protected ArticulatedHandDefinition handDefinition;

        protected ArticulatedHandDefinition HandDefinition => handDefinition ?? (handDefinition = Definition as ArticulatedHandDefinition);

        protected readonly Dictionary<TrackedHandJoint, MixedRealityPose> jointPoseDict = new Dictionary<TrackedHandJoint, MixedRealityPose>();

        protected readonly Dictionary<HandJointID, TrackedHandJoint> handJointMapping = new Dictionary<HandJointID, TrackedHandJoint>()
        {
            {HandJointID.Wrist, TrackedHandJoint.Wrist},
            {HandJointID.Palm, TrackedHandJoint.Palm},
            {HandJointID.ThumbMetacarpal, TrackedHandJoint.ThumbMetacarpalJoint},
            {HandJointID.ThumbProximal, TrackedHandJoint.ThumbProximalJoint},
            {HandJointID.ThumbDistal, TrackedHandJoint.ThumbDistalJoint},
            {HandJointID.ThumbTip, TrackedHandJoint.ThumbTip},
            {HandJointID.IndexProximal, TrackedHandJoint.IndexKnuckle},
            {HandJointID.IndexMiddle, TrackedHandJoint.IndexMiddleJoint},
            {HandJointID.IndexDistal, TrackedHandJoint.IndexDistalJoint},
            {HandJointID.IndexTip, TrackedHandJoint.IndexTip},
            {HandJointID.MiddleProximal, TrackedHandJoint.MiddleKnuckle},
            {HandJointID.MiddleMiddle, TrackedHandJoint.MiddleMiddleJoint},
            {HandJointID.MiddleDistal, TrackedHandJoint.MiddleDistalJoint},
            {HandJointID.MiddleTip, TrackedHandJoint.MiddleTip},
            {HandJointID.RingProximal, TrackedHandJoint.RingKnuckle},
            {HandJointID.RingMiddle, TrackedHandJoint.RingMiddleJoint},
            {HandJointID.RingDistal, TrackedHandJoint.RingDistalJoint},
            {HandJointID.RingTip, TrackedHandJoint.RingTip},
            {HandJointID.PinkyMetacarpal, TrackedHandJoint.PinkyMetacarpal},
            {HandJointID.PinkyProximal, TrackedHandJoint.PinkyKnuckle},
            {HandJointID.PinkyMiddle, TrackedHandJoint.PinkyMiddleJoint},
            {HandJointID.PinkyDistal, TrackedHandJoint.PinkyDistalJoint},
            {HandJointID.PinkyTip, TrackedHandJoint.PinkyTip}
        };

        public NRSDKArticulatedHand(
            TrackingState trackingState,
            Handedness controllerHandedness,
            IMixedRealityInputSource inputSource = null,
            MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions, new ArticulatedHandDefinition(inputSource, controllerHandedness)) { }

        public override bool IsInPointingPose => NRInput.Hands.IsRunning ? HandDefinition.IsInPointingPose : false;

        public override void SetupDefaultInteractions()
        {
            AssignControllerMappings(DefaultInteractions);
        }

        public override bool TryGetJoint(TrackedHandJoint joint, out MixedRealityPose pose)
        {
            return jointPoseDict.TryGetValue(joint, out pose);
        }

        public void UpdateController()
        {
            if (!Enabled)
                return;

            var handEnum = ControllerHandedness == Handedness.Left ? HandEnum.LeftHand : HandEnum.RightHand;
            var handState = NRInput.Hands.GetHandState(handEnum);
            var controllerHandEnum = ControllerHandedness == Handedness.Left ? ControllerHandEnum.Left : ControllerHandEnum.Right;
            var isTracked = NRInput.Hands.IsRunning ? handState.isTracked : false;
            IsPositionAvailable = IsRotationAvailable = isTracked;
            var lastState = TrackingState;
            TrackingState = isTracked ? TrackingState.Tracked : TrackingState.NotTracked;
            if (lastState != TrackingState)
            {
                CoreServices.InputSystem?.RaiseSourceTrackingStateChanged(InputSource, this, TrackingState);
            }

            MixedRealityPose pointerPose;
            if (NRInput.Hands.IsRunning)
            {
                UpdateHandJoints(handState);
                pointerPose = new MixedRealityPose(handState.pointerPose.position, handState.pointerPose.rotation);
            }
            else
            {
                pointerPose = MixedRealityPose.ZeroIdentity;
            }

            if (TrackingState == TrackingState.Tracked)
            {
                CoreServices.InputSystem?.RaiseSourcePoseChanged(InputSource, this, pointerPose);
            }

            if (Interactions == null)
            {
                Debug.LogError("Interactions Config Is Null For NRSDK");
                Enabled = false;
            }

            for (int i = 0; i < Interactions?.Length; i++)
            {
                switch (Interactions[i].InputType)
                {
                    case DeviceInputType.SpatialPointer:
                    case DeviceInputType.SpatialGrip:
                        Interactions[i].PoseData = pointerPose;
                        if (Interactions[i].Changed)
                        {
                            CoreServices.InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction, pointerPose);
                        }
                        break;
                    case DeviceInputType.Select:
                    case DeviceInputType.TriggerPress:
                    case DeviceInputType.GripPress:
                        Interactions[i].BoolData = NRInput.GetButton(controllerHandEnum, ControllerButton.TRIGGER);
                        if (Interactions[i].Changed)
                        {
                            if (Interactions[i].BoolData)
                            {
                                CoreServices.InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction);
                            }
                            else
                            {
                                CoreServices.InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction);
                            }
                        }
                        break;
                    case DeviceInputType.IndexFinger:
                        HandDefinition?.UpdateCurrentIndexPose(Interactions[i]);
                        break;
                    case DeviceInputType.ThumbStick:
                        HandDefinition?.UpdateCurrentTeleportPose(Interactions[i]);
                        break;
                }
            }
        }

        protected void UpdateHandJoints(HandState handState)
        {
            foreach (var jointID in handJointMapping.Keys)
            {
                var jointPose = handState.GetJointPose(jointID);
                TrackedHandJoint trackedJoint = handJointMapping[jointID];
                var boneRotation = jointPose.rotation * Quaternion.Euler(-90f, 0f, 0f);
                UpdateJointPose(trackedJoint, jointPose.position, boneRotation);
            }
            HandDefinition?.UpdateHandJoints(jointPoseDict);
        }

        protected void UpdateJointPose(TrackedHandJoint joint, Vector3 position, Quaternion rotation)
        {
            MixedRealityPose pose = new MixedRealityPose(position, rotation);
            if (!jointPoseDict.ContainsKey(joint))
            {
                jointPoseDict.Add(joint, pose);
            }
            else
            {
                jointPoseDict[joint] = pose;
            }
        }
    }
}

