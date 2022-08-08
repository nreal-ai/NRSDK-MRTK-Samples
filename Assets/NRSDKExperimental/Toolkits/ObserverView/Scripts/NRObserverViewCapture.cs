/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.Experimental.StreammingCast
{
    using UnityEngine;
    using NRKernal.Record;
    using System.Collections.Generic;
    using System;

    /// <summary> A nr observer view capture. </summary>
    public class NRObserverViewCapture : IDisposable
    {
        /// <summary> Default constructor. </summary>
        public NRObserverViewCapture()
        {
            IsRecording = false;
        }

        /// <summary> Finalizer. </summary>
        ~NRObserverViewCapture()
        {

        }

        /// <summary> A list of all the supported device resolutions for observer view videos. </summary>
        /// <value> The supported resolutions. </value>
        public static IEnumerable<Resolution> SupportedResolutions
        {
            get
            {
                NativeResolution rgbResolution = new NativeResolution(1280, 720);
                if (NRDevice.Subsystem.IsFeatureSupported(NRSupportedFeature.NR_FEATURE_RGB_CAMERA))
                    rgbResolution = NRFrame.GetDeviceResolution(NativeDevice.RGB_CAMERA);
                Resolution stand_resolution = new Resolution()
                {
                    width = rgbResolution.width,
                    height = rgbResolution.height,
                    refreshRate = NativeConstants.RECORD_FPS_DEFAULT,
                };
                yield return stand_resolution;

                Resolution low_resolution = new Resolution()
                {
                    width = stand_resolution.width / 2,
                    height = stand_resolution.height / 2,
                    refreshRate = NativeConstants.RECORD_FPS_DEFAULT,
                };
                yield return low_resolution;

                Resolution high_resolution = new Resolution()
                {
                    width = stand_resolution.width * 3 / 2,
                    height = stand_resolution.height * 3 / 2,
                    refreshRate = NativeConstants.RECORD_FPS_DEFAULT,
                };
                yield return high_resolution;
            }
        }

        /// <summary>
        /// Indicates whether or not the VideoCapture instance is currently recording video. </summary>
        /// <value> True if this object is recording, false if not. </value>
        public bool IsRecording { get; private set; }

        /// <summary> Context for the capture. </summary>
        private ObserverViewFrameCaptureContext m_CaptureContext;

        /// <summary> Gets the context. </summary>
        /// <returns> The context. </returns>
        public ObserverViewFrameCaptureContext GetContext()
        {
            return m_CaptureContext;
        }

        /// <summary> Gets the preview texture. </summary>
        /// <value> The preview texture. </value>
        public Texture PreviewTexture
        {
            get
            {
                return m_CaptureContext?.PreviewTexture;
            }
        }

        /// <summary> Creates an asynchronous. </summary>
        /// <param name="showHolograms">     True to show, false to hide the holograms.</param>
        /// <param name="onCreatedCallback"> The on created callback.</param>
        public static void CreateAsync(bool showHolograms, OnObserverViewResourceCreatedCallback onCreatedCallback)
        {
            NRObserverViewCapture capture = new NRObserverViewCapture();
            capture.m_CaptureContext = new ObserverViewFrameCaptureContext();
            onCreatedCallback?.Invoke(capture);
        }

        /// <summary>
        /// Returns the supported frame rates at which a video can be recorded given a resolution. </summary>
        /// <param name="resolution"> A recording resolution.</param>
        /// <returns> The frame rates at which the video can be recorded. </returns>
        public static IEnumerable<int> GetSupportedFrameRatesForResolution(Resolution resolution)
        {
            yield return NativeConstants.RECORD_FPS_DEFAULT;
        }

        /// <summary> Dispose must be called to shutdown the PhotoCapture instance. </summary>
        public void Dispose()
        {
            if (m_CaptureContext != null)
            {
                m_CaptureContext.Release();
                m_CaptureContext = null;
            }
        }

        /// <summary> Starts observer view mode asynchronous. </summary>
        /// <param name="setupParams">                Options for controlling the setup.</param>
        /// <param name="audioState">                 State of the audio.</param>
        /// <param name="onVideoModeStartedCallback"> The on video mode started callback.</param>
        public void StartObserverViewModeAsync(CameraParameters setupParams, AudioState audioState, OnObserverViewModeStartedCallback onVideoModeStartedCallback, bool autoAdaptBlendMode = false)
        {
            setupParams.camMode = CamMode.VideoMode;
            
            if (autoAdaptBlendMode)
            {
                var blendMode = m_CaptureContext.AutoAdaptBlendMode(setupParams.blendMode);
                if (blendMode != setupParams.blendMode)
                {
                    NRDebugger.Warning("[VideoCapture] AutoAdaptBlendMode : {0} => {1}", setupParams.blendMode, blendMode);
                    setupParams.blendMode = blendMode;
                }
            }
            m_CaptureContext.StartCaptureMode(setupParams);
            var result = new ObserverViewCaptureResult();
            result.resultType = CaptureResultType.Success;
            onVideoModeStartedCallback?.Invoke(result);
        }

        /// <summary> Starts observer view asynchronous. </summary>
        /// <param name="ip">                              The IP.</param>
        /// <param name="onStartedRecordingVideoCallback"> The on started recording video callback.</param>
        public void StartObserverViewAsync(string ip, OnStartedObserverViewCallback onStartedRecordingVideoCallback)
        {
            var captureResult = new ObserverViewCaptureResult();
            if (IsRecording)
            {
                captureResult.resultType = CaptureResultType.UnknownError;
                onStartedRecordingVideoCallback?.Invoke(captureResult);
            }
            else
            {
                m_CaptureContext.StartCapture(ip, (result) =>
                {
                    if (result)
                    {
                        IsRecording = true;
                        captureResult.resultType = CaptureResultType.Success;
                        onStartedRecordingVideoCallback?.Invoke(captureResult);
                    }
                    else
                    {
                        IsRecording = false;
                        captureResult.resultType = CaptureResultType.ServiceIsNotAvailable;
                        onStartedRecordingVideoCallback?.Invoke(captureResult);
                    }
                });
            }
        }

        /// <summary> Stops observer view asynchronous. </summary>
        /// <param name="onStoppedRecordingVideoCallback"> The on stopped recording video callback.</param>
        public void StopObserverViewAsync(OnStoppedObserverViewCallback onStoppedRecordingVideoCallback)
        {
            var result = new ObserverViewCaptureResult();
            if (!IsRecording)
            {
                result.resultType = CaptureResultType.UnknownError;
                onStoppedRecordingVideoCallback?.Invoke(result);
            }
            else
            {
                try
                {
                    m_CaptureContext.StopCapture();
                }
                catch (Exception e)
                {
                    NRDebugger.Info("Stop recording error :" + e.ToString());
                    throw;
                }
                IsRecording = false;
                result.resultType = CaptureResultType.Success;
                onStoppedRecordingVideoCallback?.Invoke(result);
            }
        }

        /// <summary> Stops observer view mode asynchronous. </summary>
        /// <param name="onVideoModeStoppedCallback"> The on video mode stopped callback.</param>
        public void StopObserverViewModeAsync(OnObserverViewModeStoppedCallback onVideoModeStoppedCallback)
        {
            m_CaptureContext.StopCaptureMode();
            var result = new ObserverViewCaptureResult();
            result.resultType = CaptureResultType.Success;
            onVideoModeStoppedCallback?.Invoke(result);
        }

        /// <summary> Contains the result of the capture request. </summary>
        public enum CaptureResultType
        {
            /// <summary>
            /// Specifies that the desired operation was successful.
            /// </summary>
            Success,

            /// <summary>
            /// Service is not available.
            /// </summary>
            ServiceIsNotAvailable,

            /// <summary>
            /// Specifies that an unknown error occurred.
            /// </summary>
            UnknownError
        }

        /// <summary>
        /// Specifies what audio sources should be recorded while recording the video. </summary>
        public enum AudioState
        {
            /// <summary>
            /// Only include the mic audio in the video recording.
            /// </summary>
            MicAudio = 0,

            /// <summary>
            /// Only include the application audio in the video recording.
            /// </summary>
            ApplicationAudio = 1,

            /// <summary>
            /// Include both the application audio as well as the mic audio in the video recording.
            /// </summary>
            ApplicationAndMicAudio = 2,

            /// <summary>
            /// Do not include any audio in the video recording.
            /// </summary>
            None = 3
        }

        /// <summary>
        /// A data container that contains the result information of a video recording operation. </summary>
        public struct ObserverViewCaptureResult
        {
            /// <summary>
            /// A generic result that indicates whether or not the VideoCapture operation succeeded. </summary>
            public CaptureResultType resultType;

            /// <summary> The specific HResult value. </summary>
            public long hResult;

            /// <summary> Indicates whether or not the operation was successful. </summary>
            /// <value> True if success, false if not. </value>
            public bool success
            {
                get
                {
                    return resultType == CaptureResultType.Success;
                }
            }
        }

        /// <summary> Called when the web camera begins recording the video. </summary>
        /// <param name="result"> Indicates whether or not video recording started successfully.</param>
        public delegate void OnStartedObserverViewCallback(ObserverViewCaptureResult result);

        /// <summary> Called when a VideoCapture resource has been created. </summary>
        /// <param name="captureObject"> The VideoCapture instance.</param>
        public delegate void OnObserverViewResourceCreatedCallback(NRObserverViewCapture captureObject);

        /// <summary> Called when video mode has been started. </summary>
        /// <param name="result"> Indicates whether or not video mode was successfully activated.</param>
        public delegate void OnObserverViewModeStartedCallback(ObserverViewCaptureResult result);

        /// <summary> Called when video mode has been stopped. </summary>
        /// <param name="result"> Indicates whether or not video mode was successfully deactivated.</param>
        public delegate void OnObserverViewModeStoppedCallback(ObserverViewCaptureResult result);

        /// <summary> Called when the video recording has been saved to the file system. </summary>
        /// <param name="result"> Indicates whether or not video recording was saved successfully to the
        ///                       file system.</param>
        public delegate void OnStoppedObserverViewCallback(ObserverViewCaptureResult result);
    }
}
