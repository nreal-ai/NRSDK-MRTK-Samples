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

    /// <summary> Manages Android permissions for the Unity application. </summary>
    public class NRAndroidPermissionsManager : AndroidJavaProxy, IAndroidPermissionsCheck
    {
        /// <summary> The instance. </summary>
        private static NRAndroidPermissionsManager _instance;
        /// <summary> The activity. </summary>
        private static AndroidJavaObject _activity;
        /// <summary> The permission service. </summary>
        private static AndroidJavaObject _permissionService;
        /// <summary> The current request. </summary>
        private static AsyncTask<AndroidPermissionsRequestResult> _currentRequest = null;
        /// <summary> The action on permissions request finished. </summary>
        private static Action<AndroidPermissionsRequestResult> _onPermissionsRequestFinished;
        /// <summary> The current screen capture request. </summary>
        private static AsyncTask<AndroidJavaObject> _curScreenCaptureRequest = null;
        /// <summary> The action on screen capture request finished. </summary>
        private static Action<AndroidJavaObject> _onScreenCaptureRequestFinished;
        private static AndroidJavaObject _mediaProjection;

        /// <summary> Constructs a new AndroidPermissionsManager. </summary>
        public NRAndroidPermissionsManager() : base(
            "ai.nreal.sdk.UnityAndroidPermissions$IPermissionRequestResult")
        {
        }

        /// <summary> Checks if an Android permission is granted to the application. </summary>
        /// <param name="permissionName"> The full name of the Android permission to check (e.g.
        ///                               android.permission.CAMERA).</param>
        /// <returns>
        /// <c>true</c> if <c>permissionName</c> is granted to the application, otherwise
        /// <c>false</c>. </returns>
        public static bool IsPermissionGranted(string permissionName)
        {
            if (Application.platform != RuntimePlatform.Android)
            {
                return true;
            }

            return GetPermissionsService().Call<bool>(
                "IsPermissionGranted", GetUnityActivity(), permissionName);
        }

        /// <summary> Requests an Android permission from the user. </summary>
        /// <param name="permissionName"> The permission to be requested (e.g. android.permission.CAMERA).</param>
        /// <returns>
        /// An asynchronous task that completes when the user has accepted or rejected the requested
        /// permission and yields a <see cref="AndroidPermissionsRequestResult"/> that summarizes the
        /// result. If this method is called when another permissions request is pending, <c>null</c>
        /// will be returned instead. </returns>
        public static AsyncTask<AndroidPermissionsRequestResult> RequestPermission(
            string permissionName)
        {
            if (NRAndroidPermissionsManager.IsPermissionGranted(permissionName))
            {
                return new AsyncTask<AndroidPermissionsRequestResult>(
                    new AndroidPermissionsRequestResult(
                        new string[] { permissionName }, new bool[] { true }));
            }

            if (_currentRequest != null)
            {
                NRDebugger.Error("Attempted to make simultaneous Android permissions requests.");
                return null;
            }

            _currentRequest =
                new AsyncTask<AndroidPermissionsRequestResult>(out _onPermissionsRequestFinished);
            GetPermissionsService().Call("RequestPermissionAsync", GetUnityActivity(),
                new[] { permissionName }, GetInstance());

            return _currentRequest;
        }

        /// <summary> Requests an Android permission from the user. </summary>
        /// <param name="permissionName"> The permission to be requested (e.g. android.permission.CAMERA).</param>
        /// <returns>
        /// An asynchronous task that completes when the user has accepted or rejected the requested
        /// permission and yields a <see cref="AndroidPermissionsRequestResult"/> that summarizes the
        /// result. If this method is called when another permissions request is pending, <c>null</c>
        /// will be returned instead. </returns>
        public AsyncTask<AndroidPermissionsRequestResult> RequestAndroidPermission(
            string permissionName)
        {
            return RequestPermission(permissionName);
        }

        /// <summary> Callback fired when a permission is granted. </summary>
        /// <param name="permissionName"> The name of the permission that was granted.</param>
        public virtual void OnPermissionGranted(string permissionName)
        {
            OnPermissionResult(permissionName, true);
        }

        /// <summary> Callback fired when a permission is denied. </summary>
        /// <param name="permissionName"> The name of the permission that was denied.</param>
        public virtual void OnPermissionDenied(string permissionName)
        {
            OnPermissionResult(permissionName, false);
        }

        /// <summary> Gets the instance. </summary>
        /// <returns> The instance. </returns>
        public static NRAndroidPermissionsManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new NRAndroidPermissionsManager();
            }

            return _instance;
        }

        /// <summary> Gets unity activity. </summary>
        /// <returns> The unity activity. </returns>
        private static AndroidJavaObject GetUnityActivity()
        {
            if (_activity == null)
            {
                AndroidJavaClass unityPlayer =
                    new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                _activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            }

            return _activity;
        }

        /// <summary> Gets permissions service. </summary>
        /// <returns> The permissions service. </returns>
        private static AndroidJavaObject GetPermissionsService()
        {
            if (_permissionService == null)
            {
                _permissionService =
                    new AndroidJavaObject("ai.nreal.sdk.UnityAndroidPermissions");
            }

            return _permissionService;
        }

        /// <summary> Callback fired on an Android permission result. </summary>
        /// <param name="permissionName"> The name of the permission.</param>
        /// <param name="granted">        If permission is granted or not.</param>
        private void OnPermissionResult(string permissionName, bool granted)
        {
            if (_onPermissionsRequestFinished == null)
            {
                Debug.LogErrorFormat(
                    "AndroidPermissionsManager received an unexpected permissions result {0}",
                    permissionName);
                return;
            }

            // Cache completion method and reset request state.
            var onRequestFinished = _onPermissionsRequestFinished;
            _currentRequest = null;
            _onPermissionsRequestFinished = null;

            onRequestFinished(new AndroidPermissionsRequestResult(new string[] { permissionName },
                new bool[] { granted }));
        }


        /// <summary> Requests Android screen capture from the user. </summary>
        /// <returns>
        /// An asynchronous task that completes when the user has approved or rejected the requested
        /// ScreenCapture and yields a <see cref="AndroidJavaObject"/> that store the Android MediaProjection
        /// result. If this method is called when another screen capture request is pending, <c>null</c>
        /// will be returned instead. </returns>
        public AsyncTask<AndroidJavaObject> RequestScreenCapture()
        {
            Debug.LogFormat("AndroidPermissionsManager RequestScreenCapture: cache={0}", _mediaProjection != null);
            if (_mediaProjection != null)
            {
                return new AsyncTask<AndroidJavaObject>(_mediaProjection);
            }

            if (_curScreenCaptureRequest != null)
            {
                NRDebugger.Error("Attempted to make simultaneous Android permissions requests.");
                return null;
            }

            _curScreenCaptureRequest =
                new AsyncTask<AndroidJavaObject>(out _onScreenCaptureRequestFinished);
            GetPermissionsService().Call("RequestScreenCaptureAsync", GetUnityActivity(), GetInstance());

            return _curScreenCaptureRequest;
        }

        /// <summary> Callback is fired when screen capture is granted. </summary>
        public virtual void OnScreenCaptureGranted(AndroidJavaObject mediaProjection)
        {
            OnScreenCaptureResult(true, mediaProjection);
        }

        /// <summary> Callback is fired when screen capture is denied. </summary>
        public virtual void OnScreenCaptureDenied()
        {
            OnScreenCaptureResult(false, null);
        }

        /// <summary> Callback fired on an Android screen capture result. </summary>
        /// <param name="granted">          If screen capture is granted or not.</param>
        /// <param name="mediaProjection">  The android MediaProjection object.</param>
        private void OnScreenCaptureResult(bool granted, AndroidJavaObject mediaProjection)
        {
            Debug.LogFormat("AndroidPermissionsManager OnScreenCaptureGranted: granted={0}, mediaProjection={1}", granted, mediaProjection!=null);
            if (_onScreenCaptureRequestFinished == null)
            {
                Debug.LogError("AndroidPermissionsManager received an unexpected screencapture result");
                return;
            }

            // Cache completion method and reset request state.
            var onRequestFinished = _onScreenCaptureRequestFinished;
            _curScreenCaptureRequest = null;
            _onScreenCaptureRequestFinished = null;
            
            if (granted)
                _mediaProjection = mediaProjection;

            onRequestFinished(mediaProjection);
        }
    }
}
