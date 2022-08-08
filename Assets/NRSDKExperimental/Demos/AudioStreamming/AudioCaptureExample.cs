using NRKernal.Record;
using System;
using UnityEngine;

namespace NRKernal.Experimental.NRExamples
{
    /// <summary> A audio capture example. </summary>
    public class AudioCaptureExample : MonoBehaviour
    {
        /// <summary> The audio capture. </summary>
        NRAudioCapture m_AudioCapture = null;
        public NRVideoCapture.AudioState audioState = NRVideoCapture.AudioState.ApplicationAndMicAudio;

        void Start()
        {
            CreateAudioCaptureTest();
        }

        void Update()
        {
            if (m_AudioCapture == null)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.R) || NRInput.GetButtonDown(ControllerButton.TRIGGER))
            {
                StartAudioCapture();
            }

            if (Input.GetKeyDown(KeyCode.T) || NRInput.GetButtonDown(ControllerButton.HOME))
            {
                StopAudioCapture();
            }
        }

        void CreateAudioCaptureTest()
        {
            if (m_AudioCapture != null)
                return;

            m_AudioCapture = NRAudioCapture.Create();
        }

        /// <summary> Starts audio capture. </summary>
        void StartAudioCapture()
        {
            if (m_AudioCapture != null)
            {
                NRDebugger.Info("Created AudioCapture Instance!");
                CameraParameters param = new CameraParameters();
                param.camMode = CamMode.None;
                param.frameRate = 30;
                // Set audio state, audio record needs the permission of "android.permission.RECORD_AUDIO",
                // Add it to your "AndroidManifest.xml" file in "Assets/Plugin".
                param.audioState = audioState;

                m_AudioCapture.StartAudioModeAsync(param, OnStartedAudioCaptureMode);
            }
        }

        /// <summary> Stops audio capture. </summary>
        void StopAudioCapture()
        {
            NRDebugger.Info("Stop Audio Capture!");
            m_AudioCapture.StopRecordingAsync(OnStoppedRecordingAudio);
        }

        /// <summary> Executes the 'started audio capture mode' action. </summary>
        /// <param name="result"> The result.</param>
        void OnStartedAudioCaptureMode(NRAudioCapture.AudioCaptureResult result)
        {
            NRDebugger.Info("Started Audio Capture Mode!");
            m_AudioCapture.StartRecordingAsync(string.Empty, OnStartedRecordingAudio);
        }

        /// <summary> Executes the 'stopped audio capture mode' action. </summary>
        /// <param name="result"> The result.</param>
        void OnStoppedAudioCaptureMode(NRAudioCapture.AudioCaptureResult result)
        {
            NRDebugger.Info("Stopped Audio Capture Mode!");
        }

        /// <summary> Executes the 'started recording audio' action. </summary>
        /// <param name="result"> The result.</param>
        void OnStartedRecordingAudio(NRAudioCapture.AudioCaptureResult result)
        {
            NRDebugger.Info("Started Recording Audio!");
        }

        /// <summary> Executes the 'stopped recording audio' action. </summary>
        /// <param name="result"> The result.</param>
        void OnStoppedRecordingAudio(NRAudioCapture.AudioCaptureResult result)
        {
            NRDebugger.Info("Stopped Recording Audio!");
            m_AudioCapture.StopAudioModeAsync(OnStoppedAudioCaptureMode);
        }
    }
}
