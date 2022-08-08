using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Input.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using NRKernal;
using NRKernal.NRExamples;

namespace NRSDK.MixedReality.Toolkit.Input
{
    using TrackingState = Microsoft.MixedReality.Toolkit.TrackingState;

    [MixedRealityDataProvider(typeof(IMixedRealityInputSystem),
        SupportedPlatforms.Android | SupportedPlatforms.WindowsEditor | SupportedPlatforms.MacEditor,
        "NRSDK Device Manager")]
    public class NRSDKDeviceManager : BaseInputDeviceManager, IMixedRealityCapabilityCheck
    {
        private readonly Dictionary<Handedness, BaseController> m_Controllers = new Dictionary<Handedness, BaseController>();
        private IMixedRealityController[] m_ActiveControllers = System.Array.Empty<IMixedRealityController>();
        private NRHomeMenu m_HomeMenu;
        private bool m_IsHomeMenuAdapted;

        public NRSDKDeviceManager(
            IMixedRealityInputSystem inputSystem,
            string name,
            uint priority,
            BaseMixedRealityProfile profile) : base(inputSystem, name, priority, profile) { }

        public bool CheckCapability(MixedRealityCapability capability) => capability == MixedRealityCapability.ArticulatedHand;

        public override IMixedRealityController[] GetActiveControllers() => m_ActiveControllers;

        public override void Initialize()
        {
            base.Initialize();

            if (EventSystem.current)
            {
                var MRInputModule = EventSystem.current.GetComponent<MixedRealityInputModule>();
                if (MRInputModule)
                {
                    MRInputModule.forceModuleActive = true;
                }
            }
        }

        public override void Update()
        {
            base.Update();

            NRInput.ControllerVisualActive = NRInput.CurrentInputSourceType == InputSourceEnum.Controller;
            NRInput.LaserVisualActive = false;
            NRInput.ReticleVisualActive = false;
            NRInput.RaycastersActive = false;

            UpdateController(Handedness.Right);
            UpdateController(Handedness.Left);

            if (NRHomeMenu.IsShowing && !m_IsHomeMenuAdapted)
            {
                AdaptHomeMenu();
            }
        }

        private void UpdateController(Handedness handedness)
        {
            bool isTracked = IsControllerTracked(handedness);
            if (isTracked)
            {
                var inputSystem = Service as IMixedRealityInputSystem;
                IMixedRealityPointer[] pointers;
                IMixedRealityInputSource inputSource;

                m_Controllers.TryGetValue(handedness, out BaseController controller);
                switch (NRInput.CurrentInputSourceType)
                {
                    case InputSourceEnum.Hands:
                        if (controller == null)
                        {
                            pointers = RequestPointers(SupportedControllerType.ArticulatedHand, handedness);
                            inputSource = inputSystem?.RequestNewGenericInputSource($"NRSDK Articulated Hand", pointers, InputSourceType.Hand);
                            var hand = new NRSDKArticulatedHand(TrackingState.NotTracked, handedness, inputSource);
                            hand.UpdateController();
                            controller = hand;
                            m_Controllers.Add(handedness, controller);
                            UpdateActiveControllers();
                        }
                        else if (controller is NRSDKArticulatedHand)
                        {
                            (controller as NRSDKArticulatedHand).UpdateController();
                        }
                        else
                        {
                            RemoveController(handedness);
                            return;
                        }
                        break;
                    case InputSourceEnum.Controller:
                        if (controller == null)
                        {
                            pointers = RequestPointers(SupportedControllerType.ArticulatedHand, handedness);
                            inputSource = inputSystem?.RequestNewGenericInputSource($"NRSDK Touch Controller", pointers, InputSourceType.Hand);
                            var touchController = new NRSDKTouchController(TrackingState.NotTracked, handedness, inputSource);

                            touchController.UpdateController();
                            controller = touchController;
                            m_Controllers.Add(handedness, controller);
                            UpdateActiveControllers();
                        }
                        else if (controller is NRSDKTouchController)
                        {
                            (controller as NRSDKTouchController).UpdateController();
                        }
                        else
                        {
                            RemoveController(handedness);
                            return;
                        }
                        break;
                    default:
                        break;
                }

                if (controller != null)
                {
                    for (int i = 0; i < controller.InputSource?.Pointers?.Length; i++)
                    {
                        controller.InputSource.Pointers[i].Controller = controller;
                    }
                    inputSystem.RaiseSourceDetected(controller.InputSource, controller);
                }
            }
            else
            {
                RemoveController(handedness);
            }
        }

        private bool IsControllerTracked(Handedness handedness)
        {
            bool isTracked = false;
            switch (NRInput.CurrentInputSourceType)
            {
                case InputSourceEnum.Hands:
                    var handEnum = handedness == Handedness.Left ? HandEnum.LeftHand : HandEnum.RightHand;
                    isTracked = NRInput.Hands.GetHandState(handEnum).isTracked;
                    break;
                case InputSourceEnum.Controller:
                    var controllerHandEnum = handedness == Handedness.Left ? ControllerHandEnum.Left : ControllerHandEnum.Right;
                    isTracked = NRInput.CheckControllerAvailable(controllerHandEnum);
                    break;
                default:
                    break;
            }
            return isTracked;
        }

        private void RemoveController(Handedness handedness)
        {
            BaseController controller;
            if (m_Controllers.TryGetValue(handedness, out controller))
            {
                CoreServices.InputSystem?.RaiseSourceLost(controller.InputSource, controller);
                m_Controllers.Remove(handedness);
                RecyclePointers(controller.InputSource);
                UpdateActiveControllers();
            }
        }

        private void UpdateActiveControllers()
        {
            m_ActiveControllers = m_Controllers.Values.ToArray<IMixedRealityController>();
        }

        private void AdaptHomeMenu()
        {
            m_IsHomeMenuAdapted = true;
            m_HomeMenu = GameObject.FindObjectOfType<NRHomeMenu>();
            var canvas = m_HomeMenu?.GetComponentInChildren<Canvas>();
            if (canvas)
            {
                if (canvas.gameObject.GetComponent<GraphicRaycaster>() == null)
                {
                    canvas.gameObject.AddComponent<GraphicRaycaster>();
                }
                if (canvas.gameObject.GetComponent<CanvasUtility>() == null)
                {
                    canvas.gameObject.AddComponent<CanvasUtility>();
                }
            }
        }
    }
}


