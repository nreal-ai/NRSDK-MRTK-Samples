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
    public class NRSDKTouchController : BaseController, IMixedRealityHapticFeedback
    {
        protected ArticulatedHandDefinition handDefinition;

        protected ArticulatedHandDefinition HandDefinition => handDefinition ?? (handDefinition = Definition as ArticulatedHandDefinition);

        public NRSDKTouchController(
            TrackingState trackingState,
            Handedness controllerHandedness,
            IMixedRealityInputSource inputSource = null,
            MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions, new ArticulatedHandDefinition(inputSource, controllerHandedness)) { }

        public override bool IsInPointingPose => true;

        public override void SetupDefaultInteractions()
        {
            AssignControllerMappings(DefaultInteractions);
        }

        public void UpdateController()
        {
            if (!Enabled)
                return;

            var handEnum = ControllerHandedness == Handedness.Left ? HandEnum.LeftHand : HandEnum.RightHand;
            var controllerHandEnum = ControllerHandedness == Handedness.Left ? ControllerHandEnum.Left : ControllerHandEnum.Right;
            var isTracked = NRInput.CheckControllerAvailable(controllerHandEnum);
            var lastState = TrackingState;
            TrackingState = isTracked ? TrackingState.Tracked : TrackingState.NotTracked;
            if (lastState != TrackingState)
            {
                CoreServices.InputSystem?.RaiseSourceTrackingStateChanged(InputSource, this, TrackingState);
            }

            var controllerAnchor = controllerHandEnum == ControllerHandEnum.Left ? ControllerAnchorEnum.LeftModelAnchor : ControllerAnchorEnum.RightModelAnchor;
            var anchor = NRInput.RaycastMode == RaycastModeEnum.Gaze ? ControllerAnchorEnum.GazePoseTrackerAnchor : controllerAnchor;
            var controllerTransform = NRInput.AnchorsHelper.GetAnchor(anchor);
            var pointerPose = new MixedRealityPose(controllerTransform.position, controllerTransform.rotation);

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
                        Interactions[i].BoolData = NRInput.GetButton(controllerHandEnum, ControllerButton.TRIGGER);
                        if (Interactions[i].Changed)
                        {
                            if (Interactions[i].BoolData)
                            {
                                CoreServices.InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction);
                                Debug.Log("InputActions of Nreal touchpad: " + Interactions[i].MixedRealityInputAction.Description);
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

        public bool StartHapticImpulse(float intensity, float durationInSeconds = float.MaxValue)
        {
            bool supportHaptic = NRInput.GetControllerAvailableFeature(ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_HAPTIC_VIBRATE);
            if (supportHaptic)
            {
                NRInput.TriggerHapticVibration(durationInSeconds, intensity);
                return true;
            }
            return false;
        }

        public void StopHapticFeedback() { }
    }
}


