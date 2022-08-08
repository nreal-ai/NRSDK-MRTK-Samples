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
    using System.Collections;
    using System;
    using System.Collections.Generic;

#if USING_XR_SDK
    using UnityEngine.XR;
#endif

    /// <summary> Hmd pose tracker event. </summary>
    public delegate void HMDPoseTrackerEvent();
    public delegate void HMDPoseTrackerModeChangeEvent(NRHMDPoseTracker.TrackingType origin, NRHMDPoseTracker.TrackingType target);
    public delegate void OnTrackingModeChanged(NRHMDPoseTracker.TrackingModeChangedResult result);


    /// <summary>
    /// Interface of external slam provider.
    /// </summary>
    public interface IExternSlamProvider
    {
        /// <summary>
        /// Get head pose at the time of timeStamp
        /// </summary>
        /// <param name="timeStamp"> The specified time. </param>
        /// <returns></returns>
        Pose GetHeadPoseAtTime(UInt64 timeStamp);
    }
    
    /// <summary>
    /// HMDPoseTracker update the infomations of pose tracker. This component is used to initialize
    /// the camera parameter, update the device posture, In addition, application can change
    /// TrackingType through this component. </summary>
    [HelpURL("https://developer.nreal.ai/develop/discover/introduction-nrsdk")]
    public class NRHMDPoseTracker : MonoBehaviour
    {
        /// <summary> Event queue for all listeners interested in OnHMDPoseReady events. </summary>
        public static event HMDPoseTrackerEvent OnHMDPoseReady;
        /// <summary> Event queue for all listeners interested in OnHMDLostTracking events. </summary>
        public static event HMDPoseTrackerEvent OnHMDLostTracking;
        /// <summary> Event queue for all listeners interested in OnChangeTrackingMode events. </summary>
        public static event HMDPoseTrackerModeChangeEvent OnChangeTrackingMode;

        public struct TrackingModeChangedResult
        {
            public bool success;
            public TrackingType trackingType;
        }

        /// <summary> HMD tracking type. </summary>
        public enum TrackingType
        {
            /// <summary>
            /// Track the position an rotation.
            /// </summary>
            Tracking6Dof = 0,

            /// <summary>
            /// Track the rotation only.
            /// </summary>
            Tracking3Dof = 1,

            /// <summary>
            /// Track nothing.
            /// </summary>
            Tracking0Dof = 2,

            /// <summary>
            /// Track nothing. Use rotation to make tracking smoothly.
            /// </summary>
            Tracking0DofStable = 3
        }

        /// <summary> Type of the tracking. </summary>
        [SerializeField]
        private TrackingType m_TrackingType = TrackingType.Tracking6Dof;

        /// <summary> Gets the tracking mode. </summary>
        /// <value> The tracking mode. </value>
        public TrackingType TrackingMode
        {
            get
            {
                return m_TrackingType;
            }
        }

        /// <summary> Auto adapt trackingType while not supported. </summary>
        public bool TrackingModeAutoAdapt = true;

        /// <summary> Use relative coordinates or not. </summary>
        public bool UseRelative = false;
        /// <summary> The last reason. </summary>
        private LostTrackingReason m_LastReason = LostTrackingReason.INITIALIZING;
        private IExternSlamProvider m_externSlamProvider = null;

        /// <summary> The left camera. </summary>
        public Camera leftCamera;
        /// <summary> The center camera. </summary>
        public Camera centerCamera;
        public Transform centerAnchor;
        Pose HeadRotFromCenter = Pose.identity;
        /// <summary> The right camera. </summary>
        public Camera rightCamera;
        private bool m_ModeChangeLock = false;
        public bool IsTrackModeChanging
        {
            get { return m_ModeChangeLock; }
        }

#if USING_XR_SDK
        static internal List<XRNodeState> nodeStates = new List<XRNodeState>();
        static internal void GetNodePoseData(XRNode node, out Pose resultPose)
        {
            InputTracking.GetNodeStates(nodeStates);
            for (int i = 0; i < nodeStates.Count; i++)
            {
                var nodeState = nodeStates[i];
                if (nodeState.nodeType == node)
                {
                    nodeState.TryGetPosition(out resultPose.position);
                    nodeState.TryGetRotation(out resultPose.rotation);
                    return;
                }
            }
            resultPose = Pose.identity;
        }
#endif

        /// <summary> Awakes this object. </summary>
        void Awake()
        {
#if UNITY_EDITOR || USING_XR_SDK
            if (leftCamera != null)
                leftCamera.enabled = false;
            if (rightCamera != null)
                rightCamera.enabled = false;
            centerCamera.depth = 1;
            centerCamera.enabled = true;
#else
            if (leftCamera != null)
                leftCamera.enabled = true;
            if (rightCamera != null)
                rightCamera.enabled = true;
            centerCamera.enabled = false;
#endif
            StartCoroutine(Initialize());
        }

        /// <summary> Executes the 'enable' action. </summary>
        void OnEnable()
        {
#if USING_XR_SDK && !UNITY_EDITOR
            Application.onBeforeRender += OnUpdate;
#else
            NRKernalUpdater.OnUpdate += OnUpdate;
#endif
        }

        /// <summary> Executes the 'disable' action. </summary>
        void OnDisable()
        {
#if USING_XR_SDK && !UNITY_EDITOR
            Application.onBeforeRender -= OnUpdate;
#else
            NRKernalUpdater.OnUpdate -= OnUpdate;
#endif
        }

        /// <summary> Executes the 'update' action. </summary>
        void OnUpdate()
        {
            CheckHMDPoseState();
            UpdatePoseByTrackingType();
        }

        /// <summary> Auto adaption for current working trackingType based on supported feature on current device. </summary>
        public void AutoAdaptTrackingType()
        {
            if (TrackingModeAutoAdapt)
            {
                TrackingType adjustTrackingType = AdaptTrackingType(m_TrackingType);
                if (adjustTrackingType != m_TrackingType)
                {
                    NRDebugger.Warning("[NRHMDPoseTracker] AutoAdaptTrackingType : {0} => {1}", m_TrackingType, adjustTrackingType);
                    m_TrackingType = adjustTrackingType;
                }
            }
        }

        /// <summary> Auto adaption for trackingType based on supported feature on current device. </summary>
        /// <returns> fallback trackingType. </returns>
        public static TrackingType AdaptTrackingType(TrackingType mode)
        {
            switch (mode)
            {
                case TrackingType.Tracking6Dof:
                    {
                        if (NRDevice.Subsystem.IsFeatureSupported(NRSupportedFeature.NR_FEATURE_TRACKING_6DOF))
                            return TrackingType.Tracking6Dof;
                        else if (NRDevice.Subsystem.IsFeatureSupported(NRSupportedFeature.NR_FEATURE_TRACKING_3DOF))
                            return TrackingType.Tracking3Dof;
                        else
                            return TrackingType.Tracking0Dof;
                    }
                case TrackingType.Tracking3Dof:
                    {
                        if (NRDevice.Subsystem.IsFeatureSupported(NRSupportedFeature.NR_FEATURE_TRACKING_3DOF))
                            return TrackingType.Tracking3Dof;
                        else
                            return TrackingType.Tracking0Dof;
                    }
                case TrackingType.Tracking0DofStable:
                    {
                        if (NRDevice.Subsystem.IsFeatureSupported(NRSupportedFeature.NR_FEATURE_TRACKING_3DOF))
                            return TrackingType.Tracking0DofStable;
                        else
                            return TrackingType.Tracking0Dof;
                    }
            }
            return mode;
        }

        /// <summary> Change mode. </summary>
        /// <param name="trackingtype">        The trackingtype.</param>
        /// <param name="OnModeChanged"> The mode changed call back and return the result.</param>
        private bool ChangeMode(TrackingType trackingtype, OnTrackingModeChanged OnModeChanged)
        {
            NRDebugger.Info("[NRHMDPoseTracker] Begin ChangeMode to:" + trackingtype);
            TrackingModeChangedResult result = new TrackingModeChangedResult();
            if (trackingtype == m_TrackingType || m_ModeChangeLock)
            {
                result.success = false;
                result.trackingType = m_TrackingType;
                OnModeChanged?.Invoke(result);
                NRDebugger.Warning("[NRHMDPoseTracker] Change tracking mode faild...");
                return false;
            }

            OnChangeTrackingMode?.Invoke(m_TrackingType, trackingtype);
            NRSessionManager.OnChangeTrackingMode?.Invoke(m_TrackingType, trackingtype);

#if !UNITY_EDITOR
            m_ModeChangeLock = true;
            AsyncTaskExecuter.Instance.RunAction(() =>
            {
                result.success = NRSessionManager.Instance.NativeAPI.NativeTracking.SwitchTrackingMode((TrackingMode)trackingtype);

                if (result.success)
                {
                    m_TrackingType = trackingtype;
                }
                result.trackingType = m_TrackingType;
                OnModeChanged?.Invoke(result);
                m_ModeChangeLock = false;
                NRDebugger.Info("[NRHMDPoseTracker] End ChangeMode, result:" + result.success);
            });
#else
            m_TrackingType = trackingtype;
            result.success = true;
            result.trackingType = m_TrackingType;
            OnModeChanged?.Invoke(result);
#endif
            return true;
        }

        private void OnApplicationPause(bool pause)
        {
            NRDebugger.Info("[NRHMDPoseTracker] OnApplicationPause : pause={0}, headPos={1}, cachedWorldMatrix={2}",
                pause, NRFrame.HeadPose.ToString("F2"), cachedWorldMatrix.ToString());
            if (pause)
            {
                this.CacheWorldMatrix();
            }
        }

        Pose GetCachPose()
        {
            Pose cachePose;
            if (UseRelative)
                cachePose = new Pose(transform.localPosition, transform.localRotation);
            else
                cachePose = new Pose(transform.position, transform.rotation);
        
            // remove the neck mode relative position.
            if (m_TrackingType != TrackingType.Tracking6Dof)
            {
                cachePose.position = ConversionUtility.GetPositionFromTMatrix(cachedWorldMatrix);
            }
            return cachePose;
        }

        /// <summary> Change to 6 degree of freedom. </summary>
        /// <param name="OnModeChanged"> The mode changed call back and return the result.</param>
        /// <param name="autoAdapt"> Auto trackingType adaption based on supported features on current device.</param>
        public bool ChangeTo6Dof(OnTrackingModeChanged OnModeChanged, bool autoAdapt = false) 
        {
            var trackType = TrackingType.Tracking6Dof;
            if (autoAdapt)
                trackType = AdaptTrackingType(trackType);
            Pose cachePose = GetCachPose();
            return ChangeMode(trackType, (NRHMDPoseTracker.TrackingModeChangedResult result) =>
            {
                if (result.success)
                    CacheWorldMatrix(cachePose);
                if (OnModeChanged != null)
                    OnModeChanged(result);
            });
        }

        /// <summary> Change to 3 degree of freedom. </summary>
        /// <param name="OnModeChanged"> The mode changed call back and return the result.</param>
        /// <param name="autoAdapt"> Auto trackingType adaption based on supported features on current device.</param>
        public bool ChangeTo3Dof(OnTrackingModeChanged OnModeChanged, bool autoAdapt = false)
        {
            var trackType = TrackingType.Tracking3Dof;
            if (autoAdapt)
                trackType = AdaptTrackingType(trackType);
            Pose cachePose = GetCachPose();
            return ChangeMode(trackType, (NRHMDPoseTracker.TrackingModeChangedResult result) =>
            {
                if (result.success)
                    CacheWorldMatrix(cachePose);
                if (OnModeChanged != null)
                    OnModeChanged(result);
            });
        }

        /// <summary> Change to 0 degree of freedom. </summary>
        /// <param name="OnModeChanged"> The mode changed call back and return the result.</param>
        /// <param name="autoAdapt"> Auto trackingType adaption based on supported features on current device.</param>
        public bool ChangeTo0Dof(OnTrackingModeChanged OnModeChanged, bool autoAdapt = false)
        {
            var trackType = TrackingType.Tracking0Dof;
            if (autoAdapt)
                trackType = AdaptTrackingType(trackType);
            Pose cachePose = GetCachPose();
            return ChangeMode(trackType, (NRHMDPoseTracker.TrackingModeChangedResult result) =>
            {
                if (result.success)
                    CacheWorldMatrix(cachePose);
                if (OnModeChanged != null)
                    OnModeChanged(result);
            });
        }

        /// <summary> Change to 3 degree of freedom. </summary>
        /// <param name="OnModeChanged"> The mode changed call back and return the result.</param>
        /// <param name="autoAdapt"> Auto trackingType adaption based on supported features on current device.</param>
        public bool ChangeTo0DofStable(OnTrackingModeChanged OnModeChanged, bool autoAdapt = false)
        {
            var trackType = TrackingType.Tracking0DofStable;
            if (autoAdapt)
                trackType = AdaptTrackingType(trackType);
            Pose cachePose = GetCachPose();
            return ChangeMode(trackType, (NRHMDPoseTracker.TrackingModeChangedResult result) =>
            {
                if (result.success)
                    CacheWorldMatrix(cachePose);
                if (OnModeChanged != null)
                    OnModeChanged(result);
            });
        }

        private Matrix4x4 cachedWorldMatrix = Matrix4x4.identity;
        /// <summary> Cache the world matrix. </summary>
        public void CacheWorldMatrix()
        {
            Pose cachePose = GetCachPose();
            CacheWorldMatrix(cachePose);
        }

        public void CacheWorldMatrix(Pose pose)
        {
            NRDebugger.Info("[NRHMDPoseTracker] CacheWorldMatrix : UseRelative={0}, trackType={1}, pos={2}", UseRelative, m_TrackingType, pose.ToString("F2"));
            Plane horizontal_plane = new Plane(Vector3.up, Vector3.zero);
            Vector3 forward_use_gravity = horizontal_plane.ClosestPointOnPlane(pose.forward).normalized;
            Quaternion rotation_use_gravity = Quaternion.LookRotation(forward_use_gravity, Vector3.up);
            cachedWorldMatrix = ConversionUtility.GetTMatrix(pose.position, rotation_use_gravity);
            NRDebugger.Info("CacheWorldMatrix Adjust : pos={0}, {1}, cachedWorldMatrix={2}", 
                pose.position.ToString("F2"), rotation_use_gravity.ToString("F2"), cachedWorldMatrix.ToString());
        }

        /// <summary> 
        /// Reset world matrix ,position:(0,0,0) ,rotation:(x,0,z) 
        /// </summary>
        public void ResetWorldMatrix()
        {
            Pose pose;
            if (UseRelative)
            {
                pose = new Pose(transform.localPosition, transform.localRotation);
            }
            else
            {
                pose = new Pose(transform.position, transform.rotation);
            }

            Plane horizontal_plane = new Plane(Vector3.up, Vector3.zero);
            Vector3 forward_use_gravity = horizontal_plane.ClosestPointOnPlane(pose.forward).normalized;
            Quaternion rotation_use_gravity = Quaternion.LookRotation(forward_use_gravity, Vector3.up);
            var matrix = ConversionUtility.GetTMatrix(pose.position, rotation_use_gravity);
            cachedWorldMatrix = Matrix4x4.Inverse(matrix) * cachedWorldMatrix;
        }

        /// <summary>
        /// Get the world offset matrix from native.
        /// </summary>
        /// <returns></returns>
        public Matrix4x4 GetWorldOffsetMatrixFromNative()
        {
            Matrix4x4 parentTransfromMatrix;
            if (UseRelative)
            {
                if (transform.parent == null)
                {
                    parentTransfromMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
                }
                else
                {
                    parentTransfromMatrix = Matrix4x4.TRS(transform.parent.position, transform.parent.rotation, Vector3.one);
                }
            }
            else
            {
                parentTransfromMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
            }

            return parentTransfromMatrix * cachedWorldMatrix;
        }

        private Pose ApplyWorldMatrix(Pose pose)
        {
            var objectMatrix = ConversionUtility.GetTMatrix(pose.position, pose.rotation);
            var object_in_world = cachedWorldMatrix * objectMatrix;
            return new Pose(ConversionUtility.GetPositionFromTMatrix(object_in_world),
                ConversionUtility.GetRotationFromTMatrix(object_in_world));
        }

        /// <summary> Initializes this object. </summary>
        /// <returns> An IEnumerator. </returns>
        private IEnumerator Initialize()
        {
            while (NRFrame.SessionStatus != SessionState.Running)
            {
                NRDebugger.Debug("[NRHMDPoseTracker] Waitting to initialize.");
                yield return new WaitForEndOfFrame();
            }

#if !UNITY_EDITOR && !USING_XR_SDK
            leftCamera.enabled = false;
            rightCamera.enabled = false;
            bool result;
            var matrix_data = NRFrame.GetEyeProjectMatrix(out result, leftCamera.nearClipPlane, leftCamera.farClipPlane);
            Debug.Assert(result, "[NRHMDPoseTracker] GetEyeProjectMatrix failed.");
            if (result)
            {
                NRDebugger.Info("[NRHMDPoseTracker] Left Camera Project Matrix : {0}", matrix_data.LEyeMatrix.ToString());
                NRDebugger.Info("[NRHMDPoseTracker] RightCamera Project Matrix : {0}", matrix_data.REyeMatrix.ToString());

                leftCamera.projectionMatrix = matrix_data.LEyeMatrix;
                rightCamera.projectionMatrix = matrix_data.REyeMatrix;
                // set center camera's projection matrix with LEyeMatrix, in case some logic is using it
                centerCamera.projectionMatrix = matrix_data.LEyeMatrix;

                var eyeposeFromHead = NRFrame.EyePoseFromHead;
                leftCamera.transform.localPosition = eyeposeFromHead.LEyePose.position;
                leftCamera.transform.localRotation = eyeposeFromHead.LEyePose.rotation;
                rightCamera.transform.localPosition = eyeposeFromHead.REyePose.position;
                rightCamera.transform.localRotation = eyeposeFromHead.REyePose.rotation;
                centerAnchor.localPosition = centerCamera.transform.localPosition = (leftCamera.transform.localPosition + rightCamera.transform.localPosition) * 0.5f;
                centerAnchor.localRotation = centerCamera.transform.localRotation = Quaternion.Lerp(leftCamera.transform.localRotation, rightCamera.transform.localRotation, 0.5f);
                var centerRotMatrix = ConversionUtility.GetTMatrix(Vector3.zero, centerAnchor.localRotation).inverse;
                HeadRotFromCenter = new Pose(Vector3.zero, ConversionUtility.GetRotationFromTMatrix(centerRotMatrix));
            }
#endif

            NRDebugger.Info("[NRHMDPoseTracker] Initialized success.");
        }

        public void RegisterSlamProvider(IExternSlamProvider provider)
        {
            NRDebugger.Info("[NRHMDPoseTracker] RegisterSlamProvider");
            m_externSlamProvider = provider;
        }

        public bool GetHeadPoseByTimeInUnityWorld(ref Pose headPose, ulong timeStamp)
        {
            var nativeHeadPose = Pose.identity;
            if (m_TrackingType == TrackingType.Tracking0Dof)
            {
                nativeHeadPose = HeadRotFromCenter;
            }
            else
            {
                if (m_externSlamProvider != null)
                {
                    nativeHeadPose = m_externSlamProvider.GetHeadPoseAtTime(timeStamp);
                }
                else
                {
                    if (timeStamp == NRFrame.CurrentPoseTimeStamp)
                    {
                        nativeHeadPose = NRFrame.HeadPose;
                    }
                    else
                    {
                        if (!NRFrame.GetHeadPoseByTime(ref nativeHeadPose, timeStamp))
                            return false;
                    }
                }
                if (m_TrackingType == TrackingType.Tracking0DofStable)
                {
                    nativeHeadPose.rotation = HeadRotFromCenter.rotation * nativeHeadPose.rotation;
                }
            }
            headPose = nativeHeadPose;
            headPose = cachedWorldMatrix.Equals(Matrix4x4.identity) ? headPose : ApplyWorldMatrix(headPose);
            
            return true;
        }

        /// <summary> Updates the pose by tracking type. </summary>
        private void UpdatePoseByTrackingType()
        {
            Pose headPose = Pose.identity;
#if USING_XR_SDK && !UNITY_EDITOR
            Pose centerPose;
            GetNodePoseData(XRNode.Head, out headPose);
            headPose = cachedWorldMatrix.Equals(Matrix4x4.identity) ? headPose : ApplyWorldMatrix(headPose);
            GetNodePoseData(XRNode.CenterEye, out centerPose);
            centerCamera.transform.localPosition = Vector3.zero;
            centerCamera.transform.localRotation = Quaternion.identity;
            centerAnchor.localPosition = centerPose.position;
            centerAnchor.localRotation = centerPose.rotation;
#else

            if (!GetHeadPoseByTimeInUnityWorld(ref headPose, NRFrame.CurrentPoseTimeStamp))
                return;
            // NRDebugger.Info("[NRHMDPoseTracker] UpdatePose: trackType={2}, pos={0} --> {1}", NRFrame.HeadPose.ToString("F2"), headPose.ToString("F2"), m_TrackingType);
#endif

            if (UseRelative)
            {
                transform.localRotation = headPose.rotation;
                transform.localPosition = headPose.position;
            }
            else
            {
                transform.rotation = headPose.rotation;
                transform.position = headPose.position;
            }
        }

        /// <summary> Check hmd pose state. </summary>
        private void CheckHMDPoseState()
        {
            if (NRFrame.SessionStatus != SessionState.Running 
                || TrackingMode == TrackingType.Tracking0Dof
                || TrackingMode == TrackingType.Tracking0DofStable
                || IsTrackModeChanging)
            {
                return;
            }

            var currentReason = NRFrame.LostTrackingReason;
            // When LostTrackingReason changed.
            if (currentReason != m_LastReason)
            {
                if (currentReason != LostTrackingReason.NONE && m_LastReason == LostTrackingReason.NONE)
                {
                    OnHMDLostTracking?.Invoke();
                    NRSessionManager.OnHMDLostTracking?.Invoke();
                }
                else if (currentReason == LostTrackingReason.NONE && m_LastReason != LostTrackingReason.NONE)
                {
                    OnHMDPoseReady?.Invoke();
                    NRSessionManager.OnHMDPoseReady?.Invoke();
                }
                m_LastReason = currentReason;
            }
        }
    }
}
