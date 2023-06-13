/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NRKernal
{
    /// <summary> A nr notification listener. </summary>
    public class NRNotificationListener : MonoBehaviour
    {
        /// <summary> Values that represent levels. </summary>
        public enum Level
        {
            All = 0,
            Low,
            Middle,
            High,
            Off
        }

        // Only the message which level is higher than notifLevel would be shown.
        public Level notifLevel = Level.All;

        /// <summary> Notification object base. </summary>
        public class Notification
        {
            /// <summary> The notification listener. </summary>
            protected NRNotificationListener NotificationListener;

            /// <summary> Constructor. </summary>
            /// <param name="listener"> The listener.</param>
            public Notification(NRNotificationListener listener)
            {
                this.NotificationListener = listener;
            }

            /// <summary> Updates the state. </summary>
            public virtual void UpdateState() { }

            /// <summary> Executes the 'state changed' action. </summary>
            /// <param name="level"> The level.</param>
            public virtual void Trigger(Level level)
            {
                NotificationListener.Dispath(this, level);
            }
        }

        /// <summary> A low power notification. </summary>
        public class LowPowerNotification : Notification
        {
            /// <summary> Values that represent power states. </summary>
            public enum PowerState
            {
                /// <summary> An enum constant representing the full option. </summary>
                Full,
                /// <summary> An enum constant representing the middle option. </summary>
                Middle,
                /// <summary> An enum constant representing the low option. </summary>
                Low
            }
            /// <summary> The current state. </summary>
            private PowerState currentState = PowerState.Full;

            /// <summary> Constructor. </summary>
            /// <param name="listener"> The listener.</param>
            public LowPowerNotification(NRNotificationListener listener) : base(listener)
            {
            }

            /// <summary> Gets state by value. </summary>
            /// <param name="val"> The value.</param>
            /// <returns> The state by value. </returns>
            private PowerState GetStateByValue(float val)
            {
                if (val < 0.3f)
                {
                    return PowerState.Low;
                }
                else if (val < 0.4f)
                {
                    return PowerState.Middle;
                }
                return PowerState.Full;
            }

            /// <summary> Updates the state. </summary>
            public override void UpdateState()
            {
#if !UNITY_EDITOR
                var state = GetStateByValue(SystemInfo.batteryLevel);
#else
                var state = GetStateByValue(1f);
#endif

                if (state != currentState)
                {
                    if (state == PowerState.Low)
                    {
                        this.Trigger(Level.High);
                    }
                    else if (state == PowerState.Middle)
                    {
                        this.Trigger(Level.Middle);
                    }
                    this.currentState = state;
                }
            }
        }

        /// <summary> A slam state notification. </summary>
        public class SlamStateNotification : Notification
        {
            /// <summary> Values that represent slam states. </summary>
            private enum SlamState
            {
                /// <summary> An enum constant representing the none option. </summary>
                None,
                /// <summary> An enum constant representing the lost tracking option. </summary>
                LostTracking,
                /// <summary> An enum constant representing the tracking ready option. </summary>
                TrackingReady
            }
            /// <summary> The current state. </summary>
            private SlamState m_CurrentState = SlamState.None;

            /// <summary> Constructor. </summary>
            /// <param name="listener"> The listener.</param>
            public SlamStateNotification(NRNotificationListener listener) : base(listener)
            {
                NRHMDPoseTracker.OnHMDLostTracking += OnHMDLostTracking;
                NRHMDPoseTracker.OnHMDPoseReady += OnHMDPoseReady;
            }

            /// <summary> Executes the 'hmd pose ready' action. </summary>
            private void OnHMDPoseReady()
            {
                NRDebugger.Info("[SlamStateNotification] OnHMDPoseReady.");
                m_CurrentState = SlamState.TrackingReady;
            }

            /// <summary> Executes the 'hmd lost tracking' action. </summary>
            private void OnHMDLostTracking()
            {
                NRDebugger.Info("[SlamStateNotification] OnHMDLostTracking.");
                if (m_CurrentState != SlamState.LostTracking)
                {
                    this.Trigger(Level.Middle);
                    m_CurrentState = SlamState.LostTracking;
                }
            }
        }

        /// <summary> A temperature level notification. </summary>
        public class TemperatureLevelNotification : Notification
        {
            /// <summary> The current state. </summary>
            private GlassesTemperatureLevel currentState = GlassesTemperatureLevel.TEMPERATURE_LEVEL_NORMAL;

            /// <summary> Constructor. </summary>
            /// <param name="listener"> The listener.</param>
            public TemperatureLevelNotification(NRNotificationListener listener) : base(listener)
            {
            }

            /// <summary> Updates the state. </summary>
            public override void UpdateState()
            {
                base.UpdateState();

                var level = NRDevice.Subsystem.TemperatureLevel;
                if (currentState != level)
                {
                    if (level != GlassesTemperatureLevel.TEMPERATURE_LEVEL_NORMAL)
                    {
                        this.Trigger(level == GlassesTemperatureLevel.TEMPERATURE_LEVEL_HOT
                            ? Level.High : Level.Middle);
                    }

                    this.currentState = level;
                }
            }
        }

        /// <summary> Native interface error. </summary>
        public class NativeErrorNotification : Notification
        {
            public NRKernalError KernalError { get; private set; }
            /// <summary> Constructor. </summary>
            /// <param name="listener"> The listener.</param>
            public NativeErrorNotification(NRNotificationListener listener) : base(listener)
            {
                NRSessionManager.OnKernalError += OnSessionError;
            }

            private void OnSessionError(NRKernalError exception)
            {
                KernalError = exception;

                // Trigger the notification window
                if (KernalError is NRRGBCameraDeviceNotFindError
                    || KernalError is NRPermissionDenyError
                    || KernalError is NRUnSupportedHandtrackingCalculationError)
                {
                    this.Trigger(Level.High);
                }
            }

            public string ErrorTitle
            {
                get
                {
                    if (KernalError is NRRGBCameraDeviceNotFindError)
                    {
                        return "RGBCamera is disabled";
                    }
                    else if (KernalError is NRPermissionDenyError)
                    {
                        return "Permission Deny";
                    }
                    else if (KernalError is NRUnSupportedHandtrackingCalculationError)
                    {
                        return "Not support hand tracking calculation";
                    }
                    else
                    {
                        return KernalError.GetType().ToString();
                    }
                }
            }

            public string ErrorContent
            {
                get
                {
                    if (KernalError is NRRGBCameraDeviceNotFindError)
                    {
                        return NativeConstants.RGBCameraDeviceNotFindErrorTip;
                    }
                    else if (KernalError is NRUnSupportedHandtrackingCalculationError)
                    {
                        return NativeConstants.UnSupportedHandtrackingCalculationErrorTip;
                    }
                    else
                    {
                        return KernalError.GetErrorMsg();
                    }
                }
            }
        }

        /// <summary> True to enable, false to disable the low power tips. </summary>
        [Header("Whether to open the low power prompt")]
        public bool EnableLowPowerTips;
        /// <summary> The low power notification prefab. </summary>
        public NRNotificationWindow LowPowerNotificationPrefab;
        /// <summary> True to enable, false to disable the slam state tips. </summary>
        [Header("Whether to open the slam state prompt")]
        public bool EnableSlamStateTips;
        /// <summary> The slam state notification prefab. </summary>
        public NRNotificationWindow SlamStateNotificationPrefab;
        /// <summary> True to enable, false to disable the high temporary tips. </summary>
        [Header("Whether to open the over temperature prompt")]
        public bool EnableHighTempTips;
        /// <summary> The high temporary notification prefab. </summary>
        public NRNotificationWindow HighTempNotificationPrefab;
        [Header("Whether to open the native interface error prompt")]
        public bool EnableNativeNotifyTips;
        public NRNotificationWindow NativeErrorNotificationPrefab;

        /// <summary> List of notifications. </summary>
        protected Dictionary<Notification, NRNotificationWindow> NotificationDict = new Dictionary<Notification, NRNotificationWindow>();
        /// <summary> The tips last time. </summary>
        private Dictionary<Level, float> TipsLastTime = new Dictionary<Level, float>() {
            { Level.High,3.5f},
            { Level.Middle,2.5f},
            { Level.Low,1.5f}
        };

        /// <summary> A notification message. </summary>
        public struct NotificationMsg
        {
            /// <summary> The notification. </summary>
            public Notification notification;
            /// <summary> The level. </summary>
            public Level level;
        }
        /// <summary> Queue of notifications. </summary>
        private Queue<NotificationMsg> NotificationQueue = new Queue<NotificationMsg>();
        private float m_LockTime = 0f;
        private const float UpdateInterval = 1f;
        private float m_TimeLast = 0f;

        void Awake()
        {
            LowPowerNotificationPrefab.gameObject.SetActive(false);
            SlamStateNotificationPrefab.gameObject.SetActive(false);
            HighTempNotificationPrefab.gameObject.SetActive(false);
            NativeErrorNotificationPrefab.gameObject.SetActive(false);
        }

        void Start()
        {
            DontDestroyOnLoad(gameObject);
            RegistNotification();
        }

        /// <summary> Regist notification. </summary>
        protected virtual void RegistNotification()
        {
            if (NRSessionManager.Instance.NRSessionBehaviour.SessionConfig.EnableNotification)
            {
                if (EnableLowPowerTips) BindNotificationWindow(new LowPowerNotification(this), LowPowerNotificationPrefab);
                if (EnableSlamStateTips) BindNotificationWindow(new SlamStateNotification(this), SlamStateNotificationPrefab);
                if (EnableHighTempTips) BindNotificationWindow(new TemperatureLevelNotification(this), HighTempNotificationPrefab);
            }

            if (EnableNativeNotifyTips) BindNotificationWindow(new NativeErrorNotification(this), NativeErrorNotificationPrefab);
        }

        public void BindNotificationWindow(Notification notification, NRNotificationWindow window)
        {
            if (NotificationDict.ContainsKey(notification))
            {
                NRDebugger.Error("[NRNotificationListener] Already has the notification.");
                return;
            }
            NotificationDict.Add(notification, window);
        }

        /// <summary>
        /// Close all notification windows.
        /// </summary>
        public void ClearAll()
        {
            notifLevel = Level.Off;
        }

        void Update()
        {
            // For Editor test
            //if (Input.GetKeyDown(KeyCode.M))
            //{
            //    var notifys = NotificationDict.Keys.ToArray();
            //    this.Dispath(notifys[UnityEngine.Random.Range(0, notifys.Length - 1)], Level.Middle);
            //}
            //if (Input.GetKeyDown(KeyCode.N))
            //{
            //    var notifys = NotificationDict.Keys.ToArray();
            //    this.Dispath(notifys[UnityEngine.Random.Range(0, notifys.Length - 1)], Level.High);
            //}
            //if (Input.GetKeyDown(KeyCode.B))
            //{
            //    var notifys = NotificationDict.Keys.ToArray();
            //    this.Dispath(notifys[3], Level.High);
            //}

            m_TimeLast += Time.deltaTime;
            if (m_TimeLast < UpdateInterval)
            {
                return;
            }
            m_TimeLast = 0;

            foreach (var item in NotificationDict)
            {
                item.Key.UpdateState();
            }

            if (m_LockTime < float.Epsilon)
            {
                if (NotificationQueue.Count != 0)
                {
                    var msg = NotificationQueue.Dequeue();
                    this.OprateNotificationMsg(msg);
                    m_LockTime = TipsLastTime[msg.level];
                }
            }
            else
            {
                m_LockTime -= UpdateInterval;
            }
        }

        /// <summary> Dispaths notification message. </summary>
        /// <param name="notification"> The notification.</param>
        /// <param name="lev">          The level.</param>
        public void Dispath(Notification notification, Level lev)
        {
            if (lev >= notifLevel)
            {
                NotificationQueue.Enqueue(new NotificationMsg()
                {
                    notification = notification,
                    level = lev
                });
            }
        }

        /// <summary> Oprate notification message. </summary>
        /// <param name="msg"> The message.</param>
        protected virtual void OprateNotificationMsg(NotificationMsg msg)
        {
            NRNotificationWindow prefab = NotificationDict[msg.notification];
            Notification notification_obj = msg.notification;
            Level notification_level = msg.level;
            float duration = TipsLastTime[notification_level];
            Action onConfirm = null;
            // Notification window will not be destroyed automatic when lowpower and high level warning
            // Set it's duration to -1
            if (notification_obj is LowPowerNotification)
            {
                if (notification_level == Level.High)
                {
                    duration = -1f;
                    onConfirm = () =>
                    {
                        NRDevice.QuitApp();
                    };
                }
            }

            if (prefab != null)
            {
                NRDebugger.Info("[NRNotificationListener] Dispath:" + notification_obj.GetType().ToString());
                NRNotificationWindow notification = Instantiate(prefab);
                notification.gameObject.SetActive(true);
                notification.transform.SetParent(transform);

                if (notification_obj is NativeErrorNotification)
                {
                    string title = ((NativeErrorNotification)notification_obj).ErrorTitle;
                    string content = ((NativeErrorNotification)notification_obj).ErrorContent;
                    notification.SetLevle(notification_level)
                                .SetDuration(duration)
                                .SetTitle(title)
                                .SetContent(content)
                                .SetConfirmAction(onConfirm)
                                .Build();
                }
                else
                {
                    notification.SetLevle(notification_level)
                                .SetDuration(duration)
                                .SetConfirmAction(onConfirm)
                                .Build();
                }
            }
        }
    }
}