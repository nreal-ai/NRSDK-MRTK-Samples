/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.Record
{
    using UnityEngine;
    using NRKernal;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Records a audio from the MR images directly to disk. MR images comes from rgb camera or rgb
    /// camera image and virtual image blending. The final audio recording will be stored on the file
    /// system in the MP4 format. </summary>
    public class NRAudioCapture : IDisposable
    {
        /// <summary>
        /// Indicates whether or not the AudioCapture instance is currently recording audio. </summary>
        /// <value> True if this object is recording, false if not. </value>
        public bool IsRecording { get; private set; }

        /// <summary> Encoder for the capture. </summary>
        private AudioEncoder m_AudioEncoder;
        private AudioFilterStream m_AudioFilterStream;

        public event AudioDataCallBack OnAudioData;

        public int BytesPerSample { get { return NativeConstants.RECORD_AUDIO_BYTES_PER_SAMPLE; }}
        public int Channels { get { return NativeConstants.RECORD_AUDIO_CHANNEL; }}
        public int SamplesPerSec { get { return NativeConstants.RECORD_AUDIO_SAMPLERATE_DEFAULT; }}

        /// <summary> Create a NRAudioCapture object. </summary>
        public static NRAudioCapture Create()
        {
            NRAudioCapture capture = new NRAudioCapture();
            return capture;
        }

        /// <summary> Default constructor. </summary>
        public NRAudioCapture()
        {
            IsRecording = false;
        }

        /// <summary> Finalizer. </summary>
        ~NRAudioCapture()
        {

        }

        /// <summary> Dispose must be called to shutdown the AudioCapture instance. </summary>
        public void Dispose()
        {
            if (m_AudioEncoder != null)
            {
                m_AudioEncoder.Release();
                m_AudioEncoder = null;
            }
            if (m_AudioFilterStream != null)
            {
                m_AudioFilterStream.Dispose();
                m_AudioFilterStream = null;
            }
        }
        
        /// <summary> Starts recording asynchronous. </summary>
        /// <param name="filename">                        Filename of the file.</param>
        /// <param name="onStartedRecordingVideoCallback"> The on started recording video callback.</param>
        public void StartRecordingAsync(string filename, OnStartedRecordingAudioCallback onStartedRecordingAudioCallback)
        {
            float volumeFactorMic = 1.0f;
            float volumeFactorApp = 1.0f;
            if (NRDevice.Subsystem.GetDeviceType() == NRDeviceType.NrealLight)
            {
                volumeFactorMic = NativeConstants.RECORD_VOLUME_MIC;
                volumeFactorApp = NativeConstants.RECORD_VOLUME_APP;
            }
            StartRecordingAsync(filename, onStartedRecordingAudioCallback, volumeFactorMic, volumeFactorApp);
        }

        /// <summary> Starts recording asynchronous. </summary>
        /// <param name="filename">                        Filename of the file.</param>
        /// <param name="onStartedRecordingAudioCallback"> The on started recording audio callback.</param>
        public void StartRecordingAsync(string filename, OnStartedRecordingAudioCallback onStartedRecordingAudioCallback, float volumeFactorMic, float volumeFactorApp)
        {
            NRDebugger.Info("[AudioCapture] StartRecordingAsync: IsRecording={0}, volFactorMic={1}, volFactorApp={2}", IsRecording, volumeFactorMic, volumeFactorApp);
            var result = new AudioCaptureResult();
            if (IsRecording)
            {
                result.resultType = CaptureResultType.UnknownError;
                onStartedRecordingAudioCallback?.Invoke(result);
            }
            else
            {
                try
                {
                    if (m_AudioFilterStream == null)
                    {
                        m_AudioFilterStream = new AudioFilterStream();
                    }

                    m_AudioEncoder.AdjustVolume(RecorderIndex.REC_MIC, volumeFactorMic);
                    m_AudioEncoder.AdjustVolume(RecorderIndex.REC_APP, volumeFactorApp);

                    m_AudioEncoder.EncodeConfig.SetOutPutPath(filename);
                    m_AudioEncoder.Start(OnAudioDataCallback);

                    IsRecording = true;
                    result.resultType = CaptureResultType.Success;
                    onStartedRecordingAudioCallback?.Invoke(result);
                }
                catch (Exception ex)
                {
                    NRDebugger.Warning("[AudioCapture] StartRecordingAsync: {0}\n{1}", ex.Message, ex.StackTrace);
                    result.resultType = CaptureResultType.UnknownError;
                    onStartedRecordingAudioCallback?.Invoke(result);
                    throw;
                }
            }
        }

        private void OnAudioDataCallback(IntPtr data, UInt32 size)
        {
            // NRDebugger.Warning("[AudioCapture] OnAudioDataCallback: size={0}, data={1}", size, data);
            if (m_AudioFilterStream != null)
            {
                m_AudioFilterStream.OnAudioDataRead(data, size);
            }
            OnAudioData?.Invoke(data, size);
        }

        public bool FlushAudioData(ref byte[] outBytesData, ref int samples)
        {
            if (m_AudioFilterStream != null)
            {
                if (m_AudioFilterStream.Flush(ref outBytesData))
                {
                    samples = outBytesData.Length / Channels / BytesPerSample;
                    return true;
                }
            }
            return false;
        }

        /// <summary> Starts audio mode asynchronous. </summary>
        /// <param name="setupParams">                Options for controlling the setup.</param>
        /// <param name="onAudioModeStartedCallback"> The on audio mode started callback.</param>
        /// <param name="autoAdaptBlendMode"> Auto adaption for BlendMode based on supported feature on current device.</param>
        public void StartAudioModeAsync(CameraParameters setupParams, OnAudioModeStartedCallback onAudioModeStartedCallback)
        {
            if (Application.isEditor || Application.platform != RuntimePlatform.Android)
            {
                StartAudioMode(setupParams, onAudioModeStartedCallback);
                return;
            }
            
            bool recordMic = setupParams.CaptureAudioMic;
            bool recordApp = setupParams.CaptureAudioApplication;            
            if (recordMic && recordApp)
            {
                NRAndroidPermissionsManager.GetInstance().RequestAndroidPermission("android.permission.RECORD_AUDIO").ThenAction((requestResult) =>
                {
                    if (requestResult.IsAllGranted)
                    {
                        NRAndroidPermissionsManager.GetInstance().RequestScreenCapture().ThenAction((AndroidJavaObject mediaProjection) => 
                        {
                            if (mediaProjection != null)
                            {
                                setupParams.mediaProjection = mediaProjection;
                                StartAudioMode(setupParams, onAudioModeStartedCallback);
                            }
                            else
                            {
                                NRDebugger.Error("[AudioCapture] Screen capture is denied by user.");
                                var result = new AudioCaptureResult();
                                result.resultType = CaptureResultType.UnknownError;
                                onAudioModeStartedCallback?.Invoke(result);
                                NRSessionManager.Instance.OprateInitException(new NRPermissionDenyError(NativeConstants.ScreenCaptureDenyErrorTip));
                            }
                        });
                    }
                    else {
                        NRDebugger.Error("[AudioCapture] Record audio need the permission of 'android.permission.RECORD_AUDIO'.");
                        var result = new AudioCaptureResult();
                        result.resultType = CaptureResultType.UnknownError;
                        onAudioModeStartedCallback?.Invoke(result);
                        NRSessionManager.Instance.OprateInitException(new NRPermissionDenyError(NativeConstants.AudioPermissionDenyErrorTip));
                    }
                });
            }
            else if (recordMic)
            {
                NRAndroidPermissionsManager.GetInstance().RequestAndroidPermission("android.permission.RECORD_AUDIO").ThenAction((requestResult) =>
                {
                    if (requestResult.IsAllGranted)
                    {
                        StartAudioMode(setupParams, onAudioModeStartedCallback);
                    }
                    else {
                        NRDebugger.Error("[AudioCapture] Record audio need the permission of 'android.permission.RECORD_AUDIO'.");
                        var result = new AudioCaptureResult();
                        result.resultType = CaptureResultType.UnknownError;
                        onAudioModeStartedCallback?.Invoke(result);
                        NRSessionManager.Instance.OprateInitException(new NRPermissionDenyError(NativeConstants.AudioPermissionDenyErrorTip));
                    }
                });
            }
            else if (recordApp)
            {
                NRAndroidPermissionsManager.GetInstance().RequestScreenCapture().ThenAction((AndroidJavaObject mediaProjection) => 
                {
                    if (mediaProjection != null)
                    {
                        setupParams.mediaProjection = mediaProjection;
                        StartAudioMode(setupParams, onAudioModeStartedCallback);
                    }
                    else
                    {
                        NRDebugger.Error("[AudioCapture] Screen capture is denied by user.");
                        var result = new AudioCaptureResult();
                        result.resultType = CaptureResultType.UnknownError;
                        onAudioModeStartedCallback?.Invoke(result);
                        NRSessionManager.Instance.OprateInitException(new NRPermissionDenyError(NativeConstants.ScreenCaptureDenyErrorTip));
                    }
                });
            }
        }

        private void StartAudioMode(CameraParameters setupParams, OnAudioModeStartedCallback onAudioModeStartedCallback)
        {
            setupParams.camMode = CamMode.None;
            if (setupParams.frameRate <= 0)
                NRDebugger.Warning("[AudioCapture] frameRate need to be bigger than zero");
            
            m_AudioEncoder = new AudioEncoder();
            m_AudioEncoder.Config(setupParams);

            var result = new AudioCaptureResult();
            result.resultType = CaptureResultType.Success;
            onAudioModeStartedCallback?.Invoke(result);
        }

        /// <summary> Stops recording asynchronous. </summary>
        /// <param name="onStoppedRecordingAudioCallback"> The on stopped recording audio callback.</param>
        public void StopRecordingAsync(OnStoppedRecordingAudioCallback onStoppedRecordingAudioCallback)
        {
            var result = new AudioCaptureResult();
            if (!IsRecording)
            {
                result.resultType = CaptureResultType.UnknownError;
                onStoppedRecordingAudioCallback?.Invoke(result);
            }
            else
            {
                try
                {
                    m_AudioEncoder.Stop();
                    IsRecording = false;
                    result.resultType = CaptureResultType.Success;
                    onStoppedRecordingAudioCallback?.Invoke(result);
                }
                catch (Exception)
                {
                    result.resultType = CaptureResultType.UnknownError;
                    onStoppedRecordingAudioCallback?.Invoke(result);
                    throw;
                }
            }
        }

        /// <summary> Stops video mode asynchronous. </summary>
        /// <param name="onAudioModeStoppedCallback"> The on video mode stopped callback.</param>
        public void StopAudioModeAsync(OnAudioModeStoppedCallback onAudioModeStoppedCallback)
        {
            m_AudioEncoder?.Release();
            m_AudioEncoder = null;
            
            var result = new AudioCaptureResult();
            result.resultType = CaptureResultType.Success;
            onAudioModeStoppedCallback?.Invoke(result);
        }


        /// <summary> Contains the result of the capture request. </summary>
        public enum CaptureResultType
        {
            /// <summary>
            /// Specifies that the desired operation was successful.
            /// </summary>
            Success = 0,

            /// <summary>
            /// Specifies that an unknown error occurred.
            /// </summary>
            UnknownError = 1
        }

        /// <summary>
        /// A data container that contains the result information of a audio recording operation. </summary>
        public struct AudioCaptureResult
        {
            /// <summary>
            /// A generic result that indicates whether or not the AudioCapture operation succeeded. </summary>
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

        /// <summary> Called when the web camera begins recording the audio. </summary>
        /// <param name="result"> Indicates whether or not audio recording started successfully.</param>
        public delegate void OnStartedRecordingAudioCallback(AudioCaptureResult result);

        /// <summary> Called when audio mode has been started. </summary>
        /// <param name="result"> Indicates whether or not audio mode was successfully activated.</param>
        public delegate void OnAudioModeStartedCallback(AudioCaptureResult result);

        /// <summary> Called when audio mode has been stopped. </summary>
        /// <param name="result"> Indicates whether or not audio mode was successfully deactivated.</param>
        public delegate void OnAudioModeStoppedCallback(AudioCaptureResult result);

        /// <summary> Called when the audio recording has been saved to the file system. </summary>
        /// <param name="result"> Indicates whether or not audio recording was saved successfully to the
        ///                       file system.</param>
        public delegate void OnStoppedRecordingAudioCallback(AudioCaptureResult result);
    }
}
