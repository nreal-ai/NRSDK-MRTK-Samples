using System;
using UnityEngine;
using System.Collections;
using NRKernal.NRExamples;

namespace NRKernal.Experimental.NRExamples
{
    public class WebOverlayItem : MonoBehaviour
    {
        [Tooltip("The Overlay used to draw the webpage.")]
        [SerializeField] NROverlay m_WebOverlay;
        [Tooltip("The close button.")]
        [SerializeField] NRInteractiveItem m_CloseBtn;
        [Tooltip("Used to obtain the touch coordinates of the screen.")]
        [SerializeField] InteractiveScreen m_InteractiveScreen;
        [Tooltip("Url of webpage.")]
        [SerializeField] string URL;

        public enum WebState
        {
            Normal,
            XRMode
        }

        private AndroidWebViewPlayer m_WebViewPlayer;
        public event Action OnClose;
        private GameObject m_OverlayObj;
        private bool isInSwitchingMode = false;
        private NRDataSourceProvider m_SourceProvider;

        private const int DefaultLayerWidth = 1920;
        private const int DefaultLayerHeight = 1080;

        void OnEnable()
        {
            m_OverlayObj = m_WebOverlay.gameObject;
            m_WebOverlay.externalSurfaceObjectCreated += InitWebSurface;
            m_InteractiveScreen.onUpdateTouchState += UpdateTouchState;
            m_CloseBtn.onPointerClick.AddListener(Close);
        }

        void OnDisable()
        {
            m_WebOverlay.externalSurfaceObjectCreated -= InitWebSurface;
            m_InteractiveScreen.onUpdateTouchState -= UpdateTouchState;
            m_CloseBtn.onPointerClick.RemoveListener(Close);
        }

        // Update touch data of webview.
        private void UpdateTouchState(InteractiveScreen.TouchState touchstate)
        {
            if (m_WebViewPlayer != null)
            {
                m_WebViewPlayer.UpdateTouchState(touchstate.touchPoint, touchstate.timeStamp, touchstate.isTouching);
            }
        }

        private void InitWebSurface()
        {
            NRDebugger.Info("[OverlayWebViewExample] OnLayerCreated.");
            var surfaceJo = m_WebOverlay.GetSurfaceId();
            if (surfaceJo == IntPtr.Zero)
            {
                NRDebugger.Error("[OverlayWebViewExample] Init videosurface failed!");
                return;
            }

            if (m_WebViewPlayer == null)
            {
                m_WebViewPlayer = new AndroidWebViewPlayer();
                m_SourceProvider = new NRDataSourceProvider();
                m_WebViewPlayer.Initialize(
                    surfaceJo,
                    m_SourceProvider,
                    OnDrawBegin,
                    OnDrawEnd,
                    OnImmersiveStart,
                    OnImmersiveEnd
                );
                m_WebViewPlayer.LoadURL(URL);
            }
        }

        // When the web page starts drawing.
        private void OnDrawBegin(Int64 nanos, int index) { }

        // When the web page ends drawing.
        private void OnDrawEnd(Int64 nanos, int index)
        {
            if (isInSwitchingMode || 
                (NRSessionManager.Instance.NRRenderer != null && NRSessionManager.Instance.NRRenderer.CurrentState != NRRenderer.RendererState.Running))
            {
                return;
            }

            Pose pose = Pose.identity;
            // Update the surface of overlay.
            if (!m_SourceProvider.GetCachePoseByTime(nanos, ref pose))
            {
                NRFrame.GetHeadPoseByTime(ref pose, (ulong)nanos);
            }
            NativeMat4f matrix = NativeMat4f.identity;
            ConversionUtility.UnityPoseToApiPose(pose, out matrix);
            m_WebOverlay.UpdateExternalSurface(matrix, nanos, index);
        }

        // When enterd vrmode of webxr page.
        private void OnImmersiveStart()
        {
            NRDebugger.Info("[OverlayWebViewExample] OnImmersiveStart");
            MainThreadDispather.QueueOnMainThread(() =>
            {
                StartCoroutine(SwitchSurface(WebState.XRMode));
            });
        }

        // When exited vrmode of webxr page.
        private void OnImmersiveEnd()
        {
            NRDebugger.Info("[WebOverlay] OnImmersiveEnd");
            MainThreadDispather.QueueOnMainThread(() =>
            {
                StartCoroutine(SwitchSurface(WebState.Normal));
            });
        }

        private IEnumerator SwitchSurface(WebState state)
        {
            NRDebugger.Info("[WebOverlay] Start switch web surface:" + state.ToString());

            isInSwitchingMode = true;
            if (m_WebOverlay != null)
            {
                GameObject.DestroyImmediate(m_WebOverlay);
            }
            m_WebOverlay = CreateOverlayByMode(state);

            Action<IntPtr> onSurfaceCreated = null;
            AsyncTask<IntPtr> createSurfaceTask = new AsyncTask<IntPtr>(out onSurfaceCreated);

            m_WebOverlay.externalSurfaceObjectCreated += () =>
            {
                onSurfaceCreated?.Invoke(m_WebOverlay.GetSurfaceId());
            };

            // Wait for the surface object of current overlay is created.
            yield return createSurfaceTask.WaitForCompletion();
            if (m_WebViewPlayer == null)
            {
                this.InitWebSurface();
            }
            else
            {
                // Switch the surface of webview.
                m_WebViewPlayer.SetSurface(m_WebOverlay.GetSurfaceId(), true);
            }

            isInSwitchingMode = false;

            NRDebugger.Info("[WebOverlay] End switch web surface:" + state.ToString());
        }

        public void Reset(WebState state)
        {
            if (m_WebOverlay == null)
            {
                return;
            }

            NRDebugger.Info("[WebOverlay] Reset Overlay ByMode...");
            AppyOverlayAttributes(m_WebOverlay, state);
        }

        private void AppyOverlayAttributes(NROverlay overlay, WebState state)
        {
            switch (state)
            {
                case WebState.Normal:
                    overlay.isExternalSurface = true;
                    overlay.externalSurfaceWidth = DefaultLayerWidth;
                    overlay.externalSurfaceHeight = DefaultLayerHeight;
                    overlay.is3DLayer = false;
                    overlay.sourceUVRect[0] = new Rect(0, 0, 1, 1);
                    overlay.sourceUVRect[1] = new Rect(0, 0, 1, 1);
                    overlay.Apply();
                    break;
                case WebState.XRMode:
                    overlay.isExternalSurface = true;
                    overlay.externalSurfaceWidth = 2 * DefaultLayerWidth;
                    overlay.externalSurfaceHeight = DefaultLayerHeight;
                    // Set the params of overlay to display the webxr page which entered vrmode.
                    overlay.is3DLayer = true;
                    // Left display uv.
                    overlay.sourceUVRect[0] = new Rect(0, 0, 0.5f, 1f);
                    // Right display uv.
                    overlay.sourceUVRect[1] = new Rect(0.5f, 0, 0.5f, 1f);
                    overlay.Apply();
                    break;
                default:
                    break;
            }
        }

        private NROverlay CreateOverlayByMode(WebState state)
        {
            NRDebugger.Info("[WebOverlay] Create Overlay ByMode...");
            NROverlay overlay = m_OverlayObj.AddComponent<NROverlay>();
            AppyOverlayAttributes(overlay, state);
            return overlay;
        }

        public void Close()
        {
            m_WebViewPlayer?.Close();
            GameObject.Destroy(gameObject);
            OnClose?.Invoke();
        }
    }
}