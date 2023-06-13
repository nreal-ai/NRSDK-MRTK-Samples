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

    /// <summary> Enumeration of handedness. </summary>
    public enum ControllerHandEnum
    {
        /// <summary> An enum constant representing the right option. </summary>
        Right = 0,
        /// <summary> An enum constant representing the left option. </summary>
        Left = 1
    }

    /// <summary> Enumeration of raycast mode. Normally, suggest using "Laser" mode. </summary>
    public enum RaycastModeEnum
    {
        /// <summary> An enum constant representing the gaze option. </summary>
        Gaze,
        /// <summary> An enum constant representing the laser option. </summary>
        Laser
    }

    /// <summary> Enumeration of input source type. </summary>
    public enum InputSourceEnum
    {
        /// <summary> An enum constant representing the hands option. </summary>
        Hands,
        /// <summary> An enum constant representing the controller option. </summary>
        Controller
    }

    /// <summary> Enumeration of controller visual types. </summary>
    public enum ControllerVisualType
    {
        /// <summary> An enum constant representing the none option. </summary>
        None = 0,
        /// <summary> An enum constant representing the nreal light option. </summary>
        NrealLight = 1,
        /// <summary> An enum constant representing the phone option. </summary>
        Phone = 2
    }

    /// <summary>
    /// The main class to handle controller related things, such as to get controller states, update
    /// controller states Through this class, application would create a controllerProvider which
    /// could be custom, then the controllerProvider iteself would define how to update the
    /// controller states, so that every frame NRInput could get the right states.There are max two
    /// states for one controllerProvider. </summary>
    [HelpURL("https://developer.nreal.ai/develop/unity/controller")]
    [ScriptOrder(NativeConstants.NRINPUT_ORDER)]
    public partial class NRInput : SingletonBehaviour<NRInput>
    {
        /// <summary> True to emulate virtual display in editor. </summary>
        [Tooltip("If enable this, phone virtual controller would be shown in Unity Editor")]
        [SerializeField]
        private bool m_EmulateVirtualDisplayInEditor;
        /// <summary> The override camera center. </summary>
        [SerializeField]
        private Transform m_OverrideCameraCenter;
        /// <summary> The anchor helper. </summary>
        [SerializeField]
        private ControllerAnchorsHelper m_AnchorHelper;
        /// <summary> The raycast mode. </summary>
        [SerializeField]
        private RaycastModeEnum m_RaycastMode = RaycastModeEnum.Laser;
        /// <summary> The current input source type. </summary>
        [SerializeField]
        private InputSourceEnum m_InputSourceType = InputSourceEnum.Controller;
        /// <summary> The click interval. </summary>
        [SerializeField]
        private float m_ClickInterval = 0.3f;
        /// <summary> The drag threshold. </summary>
        [SerializeField]
        private float m_DragThreshold = 0.02f;
        /// <summary> Manager for visual. </summary>
        private ControllerVisualManager m_VisualManager;
        /// <summary> Number of last controllers. </summary>
        private int m_LastControllerCount;
        /// <summary> True to reticle visual active. </summary>
        private bool m_ReticleVisualActive = true;
        /// <summary> True to laser visual active. </summary>
        private bool m_LaserVisualActive = true;
        /// <summary> True to controller visual active. </summary>
        private bool m_ControllerVisualActive = true;
        /// <summary> True to enable, false to disable the haptic vibration. </summary>
        private bool m_HapticVibrationEnabled = true;
        /// <summary> True to active, false to disactive the gameobjects of raycasters. </summary>
        private bool m_RaycastersActive = true;
        /// <summary> Whether has checked the camera center. </summary>
        private bool m_HasCheckedCameraCenter;
        /// <summary> True means will do something OnValidate. </summary>
        private bool m_IsListeningToEditorValidateEvents = false;
        /// <summary> The cached input source type in Editor. </summary>
        private InputSourceEnum m_EditorCachedInputSourceType = InputSourceEnum.Controller;

        /// <summary> True to ignore recenter callback. </summary>
        private static bool m_IgnoreRecenterCallback = false;
        /// <summary> The domain hand. </summary>
        private static ControllerHandEnum m_DomainHand = ControllerHandEnum.Right;
        /// <summary> The controller provider. </summary>
        private static ControllerProviderBase m_ControllerProvider;
        /// <summary> The states. </summary>
        private static ControllerState[] m_States = new ControllerState[MAX_CONTROLLER_STATE_COUNT]
        {
            new ControllerState(),
            new ControllerState()
        };

        /// <summary> Max count of controllerstates supported per frame. </summary>
        public const int MAX_CONTROLLER_STATE_COUNT = 2;

        /// <summary> Event invoked whenever the domain hand has changed. </summary>
        public static Action<ControllerHandEnum> OnDomainHandChanged;

        /// <summary> Event invoked whenever a controller device is connected. </summary>
        public static Action OnControllerConnected;

        /// <summary> Event invoked whenever a controller device is disconnected. </summary>
        public static Action OnControllerDisconnected;

        /// <summary> Event invoked before controller devices are going to recenter. </summary>
        public static Action OnBeforeControllerRecenter;

        /// <summary> Event invoked whenever controller devices are recentering. </summary>
        internal static Action OnControllerRecentering;

        /// <summary> Event invoked whenever controller devices are recentered. </summary>
        public static Action OnControllerRecentered;

        /// <summary> Event invoked whenever controller devices states are updated. </summary>
        public static Action OnControllerStatesUpdated;

        /// <summary>
        /// Determine whether to show reticle visuals, could be get and set at runtime. </summary>
        /// <value> True if reticle visual active, false if not. </value>
        public static bool ReticleVisualActive { get { return Instance.m_ReticleVisualActive; } set { Instance.m_ReticleVisualActive = value; } }

        /// <summary> Determine whether to show laser visuals, could be get and set at runtime. </summary>
        /// <value> True if laser visual active, false if not. </value>
        public static bool LaserVisualActive { get { return Instance.m_LaserVisualActive; } set { Instance.m_LaserVisualActive = value; } }

        /// <summary>
        /// Determine whether to show controller visuals, could be get and set at runtime. </summary>
        /// <value> True if controller visual active, false if not. </value>
        public static bool ControllerVisualActive { get { return Instance.m_ControllerVisualActive; } set { Instance.m_ControllerVisualActive = value; } }

        /// <summary> Determine whether enable haptic vibration. </summary>
        /// <value> True if haptic vibration enabled, false if not. </value>
        public static bool HapticVibrationEnabled { get { return Instance.m_HapticVibrationEnabled; } set { Instance.m_HapticVibrationEnabled = value; } }

        /// <summary>
        /// Determine whether to active raycaster gameobjects, could be get and set at runtime. </summary>
        /// <value> True if active raycaster gameobjects, false if not. </value>
        public static bool RaycastersActive { get { return Instance.m_RaycastersActive; } set { Instance.m_RaycastersActive = value; } }

        /// <summary> Determine whether emulate phone virtual display in Unity Editor. </summary>
        /// <value> True if emulate virtual display in editor, false if not. </value>
        public static bool EmulateVirtualDisplayInEditor { get { return Instance ? Instance.m_EmulateVirtualDisplayInEditor : false; } }

        /// <summary> It's a helper to get controller anchors which are frequently used. </summary>
        /// <value> The anchors helper. </value>
        public static ControllerAnchorsHelper AnchorsHelper { get { return Instance.m_AnchorHelper; } }

        /// <summary> Get the current enumeration of handedness. </summary>
        /// <value> The domain hand. </value>
        public static ControllerHandEnum DomainHand { get { return m_DomainHand; } }

        /// <summary> Determine which raycast mode to use. </summary>
        /// <value> The raycast mode. </value>
        public static RaycastModeEnum RaycastMode { get { return Instance.m_RaycastMode; } set { Instance.m_RaycastMode = value; } }

        /// <summary> Get the current input source type. </summary>
        /// <value> The input source type. </value>
        public static InputSourceEnum CurrentInputSourceType { get { return Instance.m_InputSourceType; } }

        /// <summary> Get and set button click interval. </summary>
        /// <value> The click interval. </value>
        public static float ClickInterval { get { return Instance.m_ClickInterval; } set { Instance.m_ClickInterval = value; } }

        /// <summary> Get and set pointer drag threshold. </summary>
        /// <value> The drag threshold. </value>
        public static float DragThreshold { get { return Instance.m_DragThreshold; } set { Instance.m_DragThreshold = value; } }

        /// <summary> Get the transform of the camera which controllers are following. </summary>
        /// <value> The camera center. </value>
        public static Transform CameraCenter { get { return Instance.GetCameraCenter(); } }

        /// <summary> The HandsManager which controls the hand-tracking. </summary>
        public static NRHandsManager Hands = new NRHandsManager();

        /// <summary> Starts this object. </summary>
        private void Start()
        {
            if (isDirty)
            {
                return;
            }
            Init();
        }

        /// <summary> Executes the 'update' action. </summary>
        private void OnUpdate()
        {
            if (m_ControllerProvider == null)
                return;
            UpdateControllerProvider();
        }

        /// <summary> Updates the controller provider. </summary>
        private void UpdateControllerProvider()
        {
            if (m_ControllerProvider.Inited)
            {
                m_ControllerProvider.Update();
                if (OnControllerStatesUpdated != null)
                {
                    OnControllerStatesUpdated();
                }
                CheckControllerConnection();
                CheckControllerRecentered();
                CheckControllerButtonEvents();
            }
            else
            {
                m_ControllerProvider.Update();
#if !UNITY_EDITOR
                if (m_ControllerProvider is NRControllerProvider)
                {
                    m_DomainHand = ((NRControllerProvider)m_ControllerProvider).GetHandednessType();
                    NRDebugger.Info("[NRInput] Set default domain hand:" + m_DomainHand);
                }
#endif
            }
        }

        /// <summary> Executes the 'enable' action. </summary>
        private void OnEnable()
        {
            if (isDirty)
            {
                return;
            }
            NRKernalUpdater.OnPostUpdate += OnUpdate;
            m_ControllerProvider?.OnResume();
        }

        /// <summary> Executes the 'disable' action. </summary>
        private void OnDisable()
        {
            if (isDirty)
            {
                return;
            }
            NRKernalUpdater.OnPostUpdate -= OnUpdate;
            m_ControllerProvider?.OnPause();
        }

#if UNITY_EDITOR
        /// <summary> Executes the 'validate' action. </summary>
        private void OnValidate()
        {
            if (!m_IsListeningToEditorValidateEvents)
                return;
            if (m_EditorCachedInputSourceType != m_InputSourceType)
            {
                SetInputSource(m_InputSourceType);
            }
        }
#endif

        /// <summary> Gets a version. </summary>
        /// <param name="index"> Zero-based index of the.</param>
        /// <returns> The version. </returns>
        public string GetVersion(int index)
        {
            if (m_ControllerProvider is NRControllerProvider)
            {
                return ((NRControllerProvider)m_ControllerProvider).GetVersion(index);
            }
            else
            {
                return "0.0.0";
            }
        }

        /// <summary> Destroys this object. </summary>
        internal static void Destroy()
        {
            if (m_ControllerProvider != null)
            {
                m_ControllerProvider.OnDestroy();
                m_ControllerProvider = null;
            }
        }

        /// <summary>
        /// Base OnDestroy method that destroys the Singleton's unique instance. Called by Unity when
        /// destroying a MonoBehaviour. Scripts that extend Singleton should be sure to call
        /// base.OnDestroy() to ensure the underlying static Instance reference is properly cleaned up. </summary>
        new void OnDestroy()
        {
            if (isDirty)
            {
                return;
            }
            base.OnDestroy();
            Destroy();
        }

        /// <summary> Check controller connection. </summary>
        private void CheckControllerConnection()
        {
            int currentControllerCount = GetAvailableControllersCount();
            if (m_LastControllerCount < currentControllerCount)
            {
                if (OnControllerConnected != null)
                {
                    OnControllerConnected();
                }
            }
            else if (m_LastControllerCount > currentControllerCount)
            {
                if (OnControllerDisconnected != null)
                {
                    OnControllerDisconnected();
                }
            }
            m_LastControllerCount = currentControllerCount;
        }

        /// <summary> Check controller recentered. </summary>
        private void CheckControllerRecentered()
        {
            if (GetControllerState(DomainHand).recentered)
            {
                if (m_IgnoreRecenterCallback == false && OnBeforeControllerRecenter != null)
                {
                    OnBeforeControllerRecenter();
                }
                if (OnControllerRecentering != null)
                {
                    OnControllerRecentering();
                }
                if (m_IgnoreRecenterCallback == false && OnControllerRecentered != null)
                {
                    OnControllerRecentered();
                }
                m_IgnoreRecenterCallback = false;
            }
        }

        /// <summary> Check controller button events. </summary>
        private void CheckControllerButtonEvents()
        {
            int currentControllerCount = GetAvailableControllersCount();
            for (int i = 0; i < currentControllerCount; i++)
            {
                m_States[i].CheckButtonEvents();
            }
        }

        /// <summary> Executes the 'application pause' action. </summary>
        /// <param name="paused"> True if paused.</param>
        private void OnApplicationPause(bool paused)
        {
            if (m_ControllerProvider == null || !m_ControllerProvider.Inited)
                return;
            if (paused)
            {
                m_ControllerProvider.OnPause();
            }
            else
            {
                m_ControllerProvider.OnResume();
                m_IgnoreRecenterCallback = true;
                m_ControllerProvider.Recenter();
            }
        }

        /// <summary> Initializes this object. </summary>
        private void Init()
        {
            NRDebugger.Info("[NRInput] Init");
            NRDevice.Instance.Init();
            m_VisualManager = gameObject.AddComponent<ControllerVisualManager>();
            m_VisualManager.Init(m_States);
            SwitchControllerProvider(ControllerProviderFactory.controllerProviderType);

#if UNITY_EDITOR
            InitEmulator();
            m_IsListeningToEditorValidateEvents = true;
#endif
            SetInputSourceSafely(m_InputSourceType);
        }

#if UNITY_EDITOR
        private void InitEmulator()
        {
            if (!NREmulatorManager.Inited && !GameObject.Find("NREmulatorManager"))
            {
                NREmulatorManager.Inited = true;
                GameObject.Instantiate(Resources.Load("Prefabs/NREmulatorManager"));
            }
            if (!GameObject.Find("NREmulatorController"))
            {
                Instantiate(Resources.Load<GameObject>("Prefabs/NREmulatorController"));
            }
        }
#endif

        /// <summary> Gets camera center. </summary>
        /// <returns> The camera center. </returns>
        private Transform GetCameraCenter()
        {
            if (m_OverrideCameraCenter == null)
            {
                m_HasCheckedCameraCenter = true;
                return NRSessionManager.Instance.CenterCameraAnchor;
            }
            else
            {
                if (!m_HasCheckedCameraCenter)
                {
                    CheckCameraCenter();
                }
                return m_OverrideCameraCenter;
            }
        }

        /// <summary> To guarantee the camera center was right. </summary>
        private void CheckCameraCenter()
        {
            if (m_OverrideCameraCenter != null
                && NRSessionManager.Instance != null
                && NRSessionManager.Instance.NRSessionBehaviour != null)
            {
                var cameraRigTransform = NRSessionManager.Instance.NRSessionBehaviour.transform;
                if (m_OverrideCameraCenter.parent == cameraRigTransform)
                {
                    m_OverrideCameraCenter = NRSessionManager.Instance.CenterCameraAnchor;
                }
            }
            m_HasCheckedCameraCenter = true;
        }

        /// <summary> Convert hand to index. </summary>
        /// <param name="handEnum"> .</param>
        /// <returns> The hand converted to index. </returns>
        private static int ConvertHandToIndex(ControllerHandEnum handEnum)
        {
            if (GetAvailableControllersCount() < 2)
            {
                return DomainHand == handEnum ? 0 : 1;
            }
            else
            {
                return (int)handEnum;
            }
        }

        /// <summary> Gets controller state. </summary>
        /// <param name="hand"> The hand.</param>
        /// <returns> The controller state. </returns>
        private static ControllerState GetControllerState(ControllerHandEnum hand)
        {
            return m_States[ConvertHandToIndex(hand)];
        }

        /// <summary>
        /// Set the current input source with fallback
        /// </summary>
        /// <param name="inputSourceType"></param>
        private static void SetInputSourceSafely(InputSourceEnum inputSourceType)
        {
            var adaptInputSourceType = AdaptInputSource(inputSourceType);
            if (adaptInputSourceType != inputSourceType)
            {
                NRDebugger.Warning("[NRInput] AutoAdaptInputSource : {0} => {1}", inputSourceType, adaptInputSourceType);
                inputSourceType = adaptInputSourceType;
            }

            if (SetInputSource(inputSourceType))
            {
                return;
            }
            var fallbackInputSourceType = InputSourceEnum.Controller;
            NRDebugger.Info("[NRInput] Set Input Source To {0} Failed. Now Set Input Source To Fallback: {1}", inputSourceType, fallbackInputSourceType);
            SetInputSource(fallbackInputSourceType);
        }

        /// <summary> Auto adaption for inputSource based on supported feature on current device. </summary>
        /// <returns> Fallback inputSource. </returns>
        private static InputSourceEnum AdaptInputSource(InputSourceEnum inputSourceType)
        {
            if (inputSourceType == InputSourceEnum.Hands && !NRDevice.Subsystem.IsFeatureSupported(NRSupportedFeature.NR_FEATURE_HANDTRACKING))
                return InputSourceEnum.Controller;

            return inputSourceType;
        }

        /// <summary>
        /// To swith the controller provider
        /// </summary>
        /// <param name="providerType"></param>
        internal static void SwitchControllerProvider(Type providerType)
        {
            if (m_ControllerProvider != null && m_ControllerProvider.GetType() == providerType)
                return;
            var nextControllerProvider = ControllerProviderFactory.GetOrCreateControllerProvider(providerType, m_States);
            if (nextControllerProvider == null)
                return;
            if (m_ControllerProvider != null)
            {
                m_ControllerProvider.OnPause();
            }
            m_ControllerProvider = nextControllerProvider;
            if (m_ControllerProvider != null)
            {
                m_ControllerProvider.OnResume();
            }
        }

        /// <summary> Set the current enumeration of handedness. </summary>
        /// <param name="handEnum"> .</param>
        public static void SetDomainHandMode(ControllerHandEnum handEnum)
        {
            if (m_DomainHand == handEnum)
                return;
            m_DomainHand = handEnum;
            if (OnDomainHandChanged != null)
            {
                OnDomainHandChanged(m_DomainHand);
            }
        }

        /// <summary> Set the current input source. </summary>
        /// <param name="inputSourceType"></param>
        /// <returns> The result of setting input source. </returns>
        public static bool SetInputSource(InputSourceEnum inputSourceType)
        {
            NRDebugger.Info("[NRInput] Set Input Source: " + inputSourceType);
            if (Instance == null)
            {
                return false;
            }

            bool success = true;
            switch (inputSourceType)
            {
                case InputSourceEnum.Hands:
                    success = Hands.StartHandTracking();
                    break;
                case InputSourceEnum.Controller:
                    success = Hands.StopHandTracking();
                    break;
                default:
                    break;
            }

            if (success)
            {
                Instance.m_InputSourceType = inputSourceType;

#if UNITY_EDITOR
                Instance.m_EditorCachedInputSourceType = inputSourceType;
#endif
            }
            NRDebugger.Info("[NRInput] Input Source Set. Current Input Source = " + CurrentInputSourceType);
            return success;
        }

        /// <summary> Get the current count of controllers which are connected and available. </summary>
        /// <returns> The available controllers count. </returns>
        public static int GetAvailableControllersCount()
        {
            if (m_ControllerProvider == null)
            {
                return 0;
            }
            return m_ControllerProvider.ControllerCount;
        }

        /// <summary> Get the ControllerType of current controller. </summary>
        /// <returns> The controller type. </returns>
        public static ControllerType GetControllerType()
        {
            return GetControllerState(DomainHand).controllerType;
        }

        /// <summary> Get the ConnectionState of current controller. </summary>
        /// <returns> The controller connection state. </returns>
        public static ControllerConnectionState GetControllerConnectionState()
        {
            return GetControllerState(DomainHand).connectionState;
        }

        /// <summary> Returns true if the controller is available. </summary>
        /// <param name="handEnum"> .</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public static bool CheckControllerAvailable(ControllerHandEnum handEnum)
        {
            if (m_ControllerProvider is NRHandControllerProvider)
            {
                return Hands.GetHandState(handEnum == ControllerHandEnum.Right ? HandEnum.RightHand : HandEnum.LeftHand).pointerPoseValid;
            }

            int availableCount = GetAvailableControllersCount();
            if (availableCount == 2)
            {
                return true;
            }
            if (availableCount == 1)
            {
                return handEnum == DomainHand;
            }
            return false;
        }

        /// <summary> Returns true if the current controller supports the certain feature. </summary>
        /// <param name="feature"> The feature.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public static bool GetControllerAvailableFeature(ControllerAvailableFeature feature)
        {
            if (GetAvailableControllersCount() == 0)
                return false;
            return GetControllerState(m_DomainHand).IsFeatureAvailable(feature);
        }

        /// <summary> Returns true if the button is currently pressed this frame. </summary>
        /// <param name="button"> The button.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public static bool GetButton(ControllerButton button)
        {
            return GetButton(m_DomainHand, button);
        }

        /// <summary> Returns true if the button was pressed down this frame. </summary>
        /// <param name="button"> The button.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public static bool GetButtonDown(ControllerButton button)
        {
            return GetButtonDown(m_DomainHand, button);
        }

        /// <summary> Returns true if the button was released this frame. </summary>
        /// <param name="button"> The button.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public static bool GetButtonUp(ControllerButton button)
        {
            return GetButtonUp(m_DomainHand, button);
        }

        /// <summary> Returns true if the touchpad is being touched. </summary>
        /// <returns> True if touching, false if not. </returns>
        public static bool IsTouching()
        {
            return IsTouching(m_DomainHand);
        }

        /// <summary>
        /// Returns a Vector2 touch position on touchpad of the domain controller, range: x(-1f ~ 1f), y(-
        /// 1f ~ 1f) </summary>
        /// <returns> The touch. </returns>
        public static Vector2 GetTouch()
        {
            return GetTouch(m_DomainHand);
        }

        /// <summary> Returns a Vector2 delta touch value on touchpad of the domain controller. </summary>
        /// <returns> The delta touch. </returns>
        public static Vector2 GetDeltaTouch()
        {
            return GetDeltaTouch(m_DomainHand);
        }

        /// <summary>
        /// Returns the current position of the domain controller if 6dof, otherwise returns Vector3.zero. </summary>
        /// <returns> The position. </returns>
        public static Vector3 GetPosition()
        {
            return GetPosition(m_DomainHand);
        }

        /// <summary> Returns the current rotation of the domain controller. </summary>
        /// <returns> The rotation. </returns>
        public static Quaternion GetRotation()
        {
            return GetRotation(m_DomainHand);
        }

        /// <summary> Returns the gyro sensor value of the domain controller. </summary>
        /// <returns> The gyro. </returns>
        public static Vector3 GetGyro()
        {
            return GetGyro(m_DomainHand);
        }

        /// <summary> Returns the accel sensor value of the domain controller. </summary>
        /// <returns> The accel. </returns>
        public static Vector3 GetAccel()
        {
            return GetAccel(m_DomainHand);
        }

        /// <summary> Returns the magnetic sensor value of the domain controller. </summary>
        /// <returns> The magnitude. </returns>
        public static Vector3 GetMag()
        {
            return GetMag(m_DomainHand);
        }

        /// <summary> Returns the battery level of the domain controller. </summary>
        /// <returns> The controller battery. </returns>
        public static int GetControllerBattery()
        {
            return GetControllerBattery(DomainHand);
        }

        /// <summary> Trigger vibration of the domain controller. </summary>
        /// <param name="durationSeconds"> (Optional) The duration in seconds.</param>
        /// <param name="frequency">       (Optional) The frequency.</param>
        /// <param name="amplitude">       (Optional) The amplitude.</param>
        public static void TriggerHapticVibration(float durationSeconds = 0.1f, float frequency = 200f, float amplitude = 0.8f)
        {
            TriggerHapticVibration(m_DomainHand, durationSeconds, frequency, amplitude);
        }

        /// <summary>
        /// Returns true if the button is currently pressed this frame on a certain handedness
        /// controller. </summary>
        /// <param name="hand">   The hand.</param>
        /// <param name="button"> The button.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public static bool GetButton(ControllerHandEnum hand, ControllerButton button)
        {
            return GetControllerState(hand).GetButton(button);
        }

        /// <summary>
        /// Returns true if the button was pressed down this frame on a certain handedness controller. </summary>
        /// <param name="hand">   The hand.</param>
        /// <param name="button"> The button.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public static bool GetButtonDown(ControllerHandEnum hand, ControllerButton button)
        {
            return GetControllerState(hand).GetButtonDown(button);
        }

        /// <summary>
        /// Returns true if the button was released this frame on a certain handedness controller. </summary>
        /// <param name="hand">   The hand.</param>
        /// <param name="button"> The button.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public static bool GetButtonUp(ControllerHandEnum hand, ControllerButton button)
        {
            return GetControllerState(hand).GetButtonUp(button);
        }

        /// <summary>
        /// Returns true if the touchpad is being touched this frame on a certain handedness controller. </summary>
        /// <param name="hand"> The hand.</param>
        /// <returns> True if touching, false if not. </returns>
        public static bool IsTouching(ControllerHandEnum hand)
        {
            return GetControllerState(hand).isTouching;
        }

        /// <summary>
        /// Returns a Vector2 touch position on touchpad of a certain handedness controller, range: x(-1f
        /// ~ 1f), y(-1f ~ 1f) </summary>
        /// <param name="hand"> The hand.</param>
        /// <returns> The touch. </returns>
        public static Vector2 GetTouch(ControllerHandEnum hand)
        {
            return GetControllerState(hand).touchPos;
        }

        /// <summary>
        /// Returns a Vector2 delta touch value on touchpad of a certain handedness controller. </summary>
        /// <param name="hand"> The hand.</param>
        /// <returns> The delta touch. </returns>
        public static Vector2 GetDeltaTouch(ControllerHandEnum hand)
        {
            return GetControllerState(hand).deltaTouch;
        }

        /// <summary>
        /// Returns the current position of a certain handedness controller if 6dof, otherwise returns
        /// Vector3.zero. </summary>
        /// <param name="hand"> The hand.</param>
        /// <returns> The position. </returns>
        public static Vector3 GetPosition(ControllerHandEnum hand)
        {
            return GetControllerState(hand).position;
        }

        /// <summary> Returns the current rotation of a certain handedness controller. </summary>
        /// <param name="hand"> The hand.</param>
        /// <returns> The rotation. </returns>
        public static Quaternion GetRotation(ControllerHandEnum hand)
        {
            return GetControllerState(hand).rotation;
        }

        /// <summary> Returns the gyro sensor value of a certain handedness controller. </summary>
        /// <param name="hand"> The hand.</param>
        /// <returns> The gyro. </returns>
        public static Vector3 GetGyro(ControllerHandEnum hand)
        {
            return GetControllerState(hand).gyro;
        }

        /// <summary> Returns the accel sensor value of a certain handedness controller. </summary>
        /// <param name="hand"> The hand.</param>
        /// <returns> The accel. </returns>
        public static Vector3 GetAccel(ControllerHandEnum hand)
        {
            return GetControllerState(hand).accel;
        }

        /// <summary> Returns the magnetic sensor value of a certain handedness controller. </summary>
        /// <param name="hand"> The hand.</param>
        /// <returns> The magnitude. </returns>
        public static Vector3 GetMag(ControllerHandEnum hand)
        {
            return GetControllerState(hand).mag;
        }

        /// <summary>
        /// Returns the battery level of a certain handedness controller, range from 0 to 100. </summary>
        /// <param name="hand"> The hand.</param>
        /// <returns> The controller battery. </returns>
        public static int GetControllerBattery(ControllerHandEnum hand)
        {
            return GetControllerState(hand).batteryLevel;
        }

        /// <summary> Trigger vibration of a certain handedness controller. </summary>
        /// <param name="hand">            The hand.</param>
        /// <param name="durationSeconds"> (Optional) The duration in seconds.</param>
        /// <param name="frequency">       (Optional) The frequency.</param>
        /// <param name="amplitude">       (Optional) The amplitude.</param>
        public static void TriggerHapticVibration(ControllerHandEnum hand, float durationSeconds = 0.1f, float frequency = 200f, float amplitude = 0.8f)
        {
            if (!HapticVibrationEnabled)
                return;
            if (GetAvailableControllersCount() == 0)
                return;
            m_ControllerProvider.TriggerHapticVibration(ConvertHandToIndex(hand), durationSeconds, frequency, amplitude);
        }

        /// <summary> Recenter controller. </summary>
        public static void RecenterController()
        {
            if (GetAvailableControllersCount() == 0)
                return;
            m_IgnoreRecenterCallback = false;
            m_ControllerProvider.Recenter();
        }

        /// <summary> Add button down event listerner. </summary>
        /// <param name="hand">   The hand.</param>
        /// <param name="button"> The button.</param>
        /// <param name="action"> The action.</param>
        public static void AddDownListener(ControllerHandEnum hand, ControllerButton button, Action action)
        {
            GetControllerState(hand).AddButtonListener(ButtonEventType.Down, button, action);
        }

        /// <summary> Remove button down event listerner. </summary>
        /// <param name="hand">   The hand.</param>
        /// <param name="button"> The button.</param>
        /// <param name="action"> The action.</param>
        public static void RemoveDownListener(ControllerHandEnum hand, ControllerButton button, Action action)
        {
            GetControllerState(hand).RemoveButtonListener(ButtonEventType.Down, button, action);
        }

        /// <summary> Add button pressing event listerner. </summary>
        /// <param name="hand">   The hand.</param>
        /// <param name="button"> The button.</param>
        /// <param name="action"> The action.</param>
        public static void AddPressingListener(ControllerHandEnum hand, ControllerButton button, Action action)
        {
            GetControllerState(hand).AddButtonListener(ButtonEventType.Pressing, button, action);
        }

        /// <summary> Remove button pressing event listerner. </summary>
        /// <param name="hand">   The hand.</param>
        /// <param name="button"> The button.</param>
        /// <param name="action"> The action.</param>
        public static void RemovePressingListener(ControllerHandEnum hand, ControllerButton button, Action action)
        {
            GetControllerState(hand).RemoveButtonListener(ButtonEventType.Pressing, button, action);
        }

        /// <summary> Add button up event listerner. </summary>
        /// <param name="hand">   The hand.</param>
        /// <param name="button"> The button.</param>
        /// <param name="action"> The action.</param>
        public static void AddUpListener(ControllerHandEnum hand, ControllerButton button, Action action)
        {
            GetControllerState(hand).AddButtonListener(ButtonEventType.Up, button, action);
        }

        /// <summary> Remove button up event listerner. </summary>
        /// <param name="hand">   The hand.</param>
        /// <param name="button"> The button.</param>
        /// <param name="action"> The action.</param>
        public static void RemoveUpListener(ControllerHandEnum hand, ControllerButton button, Action action)
        {
            GetControllerState(hand).RemoveButtonListener(ButtonEventType.Up, button, action);
        }

        /// <summary> Add button click event listerner. </summary>
        /// <param name="hand">   The hand.</param>
        /// <param name="button"> The button.</param>
        /// <param name="action"> The action.</param>
        public static void AddClickListener(ControllerHandEnum hand, ControllerButton button, Action action)
        {
            GetControllerState(hand).AddButtonListener(ButtonEventType.Click, button, action);
        }

        /// <summary> Remove button click event listerner. </summary>
        /// <param name="hand">   The hand.</param>
        /// <param name="button"> The button.</param>
        /// <param name="action"> The action.</param>
        public static void RemoveClickListener(ControllerHandEnum hand, ControllerButton button, Action action)
        {
            GetControllerState(hand).RemoveButtonListener(ButtonEventType.Click, button, action);
        }
    }
}
