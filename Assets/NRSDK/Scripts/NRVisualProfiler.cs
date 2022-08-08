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
    using System.Text;
    using UnityEngine;
    using UnityEngine.Profiling;

#if WINDOWS_UWP
using Windows.System;
#endif

    /// <summary>
    /// ABOUT: The VisualProfiler provides a drop in, single file, solution for viewing your Nreal
    /// Unity application's frame rate and memory usage. Missed frames are displayed over time to
    /// visually find problem areas. Memory is reported as current, peak and max usage in a bar graph.
    /// 
    /// USAGE: To use this profiler simply add this script as a component of any GameObject in your
    /// Unity scene. The profiler is initially active and visible (toggle-able via the IsVisible
    /// property), but can be toggled via the enabled/disable voice commands keywords.
    /// 
    /// NOTE: For improved rendering performance you can optionally include the "Nreal/Instanced-
    /// Colored" shader in your project along with the VisualProfiler. </summary>
    public class NRVisualProfiler : MonoBehaviour
    {
        /// <summary> The maximum length of the string. </summary>
        private static readonly int maxStringLength = 32;
        /// <summary> The maximum target frame rate. </summary>
        private static readonly int maxTargetFrameRate = 120;
        /// <summary> The maximum frame timings. </summary>
        private static readonly int maxFrameTimings = 128;
        /// <summary> The frame range. </summary>
        private static readonly int frameRange = 30;
        /// <summary> The default window rotation. </summary>
        private static readonly Vector2 defaultWindowRotation = new Vector2(10.0f, 20.0f);
        /// <summary> The default window scale. </summary>
        private static readonly Vector3 defaultWindowScale = new Vector3(0.2f, 0.04f, 1.0f);
        /// <summary> The used memory string. </summary>
        private static readonly string usedMemoryString = "Used: ";
        /// <summary> The peak memory string. </summary>
        private static readonly string peakMemoryString = "Peak: ";
        /// <summary> The limit memory string. </summary>
        private static readonly string limitMemoryString = "Limit: ";

        /// <summary> Gets or sets the window parent. </summary>
        /// <value> The window parent. </value>
        public Transform WindowParent { get; set; } = null;

        /// <summary> True if is visible, false if not. </summary>
        [Header("Profiler Settings")]
        [SerializeField, Tooltip("Is the profiler currently visible.")]
        private bool isVisible = true;

        /// <summary> Gets or sets a value indicating whether this object is visible. </summary>
        /// <value> True if this object is visible, false if not. </value>
        public bool IsVisible
        {
            get { return isVisible; }
            set { isVisible = value; }
        }

        /// <summary> The frame sample rate. </summary>
        [SerializeField, Tooltip("The amount of time, in seconds, to collect frames for frame rate calculation.")]
        private float frameSampleRate = 0.1f;

        /// <summary> Gets or sets the frame sample rate. </summary>
        /// <value> The frame sample rate. </value>
        public float FrameSampleRate
        {
            get { return frameSampleRate; }
            set { frameSampleRate = value; }
        }

        /// <summary> The window anchor. </summary>
        [Header("Window Settings")]
        [SerializeField, Tooltip("What part of the view port to anchor the window to.")]
        private TextAnchor windowAnchor = TextAnchor.LowerCenter;

        /// <summary> Gets or sets the window anchor. </summary>
        /// <value> The window anchor. </value>
        public TextAnchor WindowAnchor
        {
            get { return windowAnchor; }
            set { windowAnchor = value; }
        }

        /// <summary> The window offset. </summary>
        [SerializeField, Tooltip("The offset from the view port center applied based on the window anchor selection.")]
        private Vector2 windowOffset = new Vector2(0.1f, 0.1f);

        /// <summary> Gets or sets the window offset. </summary>
        /// <value> The window offset. </value>
        public Vector2 WindowOffset
        {
            get { return windowOffset; }
            set { windowOffset = value; }
        }

        /// <summary> The window scale. </summary>
        [SerializeField, Range(0.5f, 5.0f), Tooltip("Use to scale the window size up or down, can simulate a zooming effect.")]
        private float windowScale = 1.0f;

        /// <summary> Gets or sets the window scale. </summary>
        /// <value> The window scale. </value>
        public float WindowScale
        {
            get { return windowScale; }
            set { windowScale = Mathf.Clamp(value, 0.5f, 5.0f); }
        }

        /// <summary> The window follow speed. </summary>
        [SerializeField, Range(0.0f, 100.0f), Tooltip("How quickly to interpolate the window towards its target position and rotation.")]
        private float windowFollowSpeed = 5.0f;

        /// <summary> Gets or sets the window follow speed. </summary>
        /// <value> The window follow speed. </value>
        public float WindowFollowSpeed
        {
            get { return windowFollowSpeed; }
            set { windowFollowSpeed = Mathf.Abs(value); }
        }

        /// <summary> The toggle keyworlds. </summary>
        [Header("UI Settings")]
        [SerializeField, Tooltip("Voice commands to toggle the profiler on and off.")]
        private string[] toggleKeyworlds = new string[] { "Profiler", "Toggle Profiler", "Show Profiler", "Hide Profiler" };
        /// <summary> The displayed decimal digits. </summary>
        [SerializeField, Range(0, 3), Tooltip("How many decimal places to display on numeric strings.")]
        private int displayedDecimalDigits = 1;
        /// <summary> The base color. </summary>
        [SerializeField, Tooltip("The color of the window backplate.")]
        private Color baseColor = new Color(80 / 256.0f, 80 / 256.0f, 80 / 256.0f, 1.0f);
        /// <summary> The target frame rate color. </summary>
        [SerializeField, Tooltip("The color to display on frames which meet or exceed the target frame rate.")]
        private Color targetFrameRateColor = new Color(127 / 256.0f, 186 / 256.0f, 0 / 256.0f, 1.0f);
        /// <summary> The missed frame rate color. </summary>
        [SerializeField, Tooltip("The color to display on frames which fall below the target frame rate.")]
        private Color missedFrameRateColor = new Color(242 / 256.0f, 80 / 256.0f, 34 / 256.0f, 1.0f);
        /// <summary> The memory used color. </summary>
        [SerializeField, Tooltip("The color to display for current memory usage values.")]
        private Color memoryUsedColor = new Color(0 / 256.0f, 164 / 256.0f, 239 / 256.0f, 1.0f);
        /// <summary> The memory peak color. </summary>
        [SerializeField, Tooltip("The color to display for peak (aka max) memory usage values.")]
        private Color memoryPeakColor = new Color(255 / 256.0f, 185 / 256.0f, 0 / 256.0f, 1.0f);
        /// <summary> The memory limit color. </summary>
        [SerializeField, Tooltip("The color to display for the platforms memory usage limit.")]
        private Color memoryLimitColor = new Color(150 / 256.0f, 150 / 256.0f, 150 / 256.0f, 1.0f);

        /// <summary> The window. </summary>
        private GameObject window;
        /// <summary> The CPU frame rate text. </summary>
        private TextMesh cpuFrameRateText;

#if USING_XR_SDK && !UNITY_EDITOR
        /// <summary> The Dropped frame count in last one second. </summary>
        private TextMesh droppedFrameCount;
        private static readonly string droppedFrameCountString = "DroppedFrameCount: {0}";
#endif
        /// <summary> The GPU frame rate text. </summary>
        private TextMesh gpuFrameRateText;
        /// <summary> The used memory text. </summary>
        private TextMesh usedMemoryText;
        /// <summary> The peak memory text. </summary>
        private TextMesh peakMemoryText;
        /// <summary> The limit memory text. </summary>
        private TextMesh limitMemoryText;
        /// <summary> The used anchor. </summary>
        private Transform usedAnchor;
        /// <summary> The peak anchor. </summary>
        private Transform peakAnchor;
        /// <summary> The window horizontal rotation. </summary>
        private Quaternion windowHorizontalRotation;
        /// <summary> The window horizontal rotation inverse. </summary>
        private Quaternion windowHorizontalRotationInverse;
        /// <summary> The window vertical rotation. </summary>
        private Quaternion windowVerticalRotation;
        /// <summary> The window vertical rotation inverse. </summary>
        private Quaternion windowVerticalRotationInverse;

        /// <summary> The frame information matrices. </summary>
        private Matrix4x4[] frameInfoMatrices;
        /// <summary> List of colors of the frame informations. </summary>
        private Vector4[] frameInfoColors;
        /// <summary> The frame information property block. </summary>
        private MaterialPropertyBlock frameInfoPropertyBlock;
        /// <summary> Identifier for the color. </summary>
        private int colorID;
        /// <summary> Identifier for the parent matrix. </summary>
        private int parentMatrixID;
        /// <summary> Number of frames. </summary>
        private int frameCount;
        /// <summary> The stopwatch. </summary>
        private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        /// <summary> The frame timings. </summary>
        private FrameTiming[] frameTimings = new FrameTiming[maxFrameTimings];
        /// <summary> The CPU frame rate strings. </summary>
        private string[] cpuFrameRateStrings;
        /// <summary> The GPU frame rate strings. </summary>
        private string[] gpuFrameRateStrings;
        /// <summary> Buffer for string data. </summary>
        private char[] stringBuffer = new char[maxStringLength];

        /// <summary> The memory usage. </summary>
        private ulong memoryUsage;
        /// <summary> The peak memory usage. </summary>
        private ulong peakMemoryUsage;
        /// <summary> The limit memory usage. </summary>
        private ulong limitMemoryUsage;

        /// <summary> Rendering resources. </summary>
        [SerializeField, HideInInspector]
        private Material defaultMaterial;
        /// <summary> The default instanced material. </summary>
        [SerializeField, HideInInspector]
        private Material defaultInstancedMaterial;
        /// <summary> The background material. </summary>
        private Material backgroundMaterial;
        /// <summary> The foreground material. </summary>
        private Material foregroundMaterial;
        /// <summary> The text material. </summary>
        private Material textMaterial;
        /// <summary> The quad mesh. </summary>
        private Mesh quadMesh;

        private Transform m_CenterCamera;
        private Transform CenterCamera
        {
            get
            {
                if (m_CenterCamera == null)
                {
                    if (NRSessionManager.Instance.CenterCameraAnchor != null)
                    {
                        m_CenterCamera = NRSessionManager.Instance.CenterCameraAnchor;
                    }
                    else
                    {
                        m_CenterCamera = Camera.main.transform;
                    }
                }
                return m_CenterCamera;
            }
        }

        /// <summary> The instance. </summary>
        private static NRVisualProfiler m_Instance = null;
        /// <summary> Gets the instance. </summary>
        /// <value> The instance. </value>
        public static NRVisualProfiler Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    GameObject go = new GameObject("VisualProfiler");
                    DontDestroyOnLoad(go);
                    m_Instance = go.AddComponent<NRVisualProfiler>();
                }
                return m_Instance;
            }
        }

        /// <summary> Awakes this object. </summary>
        void Awake()
        {
            if (m_Instance != null && m_Instance != this)
            {
                DestroyImmediate(this.gameObject);
                return;
            }

            m_Instance = this;
        }

        /// <summary> Switches. </summary>
        /// <param name="flag"> True to flag.</param>
        public void Switch(bool flag)
        {
            this.gameObject.SetActive(flag);
        }

        /// <summary> Resets this object. </summary>
        private void Reset()
        {
            if (defaultMaterial == null)
            {
                defaultMaterial = new Material(Shader.Find("Nreal/Instanced-Colored"));
                defaultMaterial.SetFloat("_ZWrite", 0.0f);
                defaultMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Disabled);
                defaultMaterial.renderQueue = 5000;
            }

            if (defaultInstancedMaterial == null)
            {
                Shader defaultInstancedShader = Shader.Find("Nreal/Instanced-Colored");

                if (defaultInstancedShader != null)
                {
                    defaultInstancedMaterial = new Material(defaultInstancedShader);
                    defaultInstancedMaterial.enableInstancing = true;
                    defaultInstancedMaterial.SetFloat("_ZWrite", 0.0f);
                    defaultInstancedMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Disabled);
                    defaultInstancedMaterial.renderQueue = 5000;
                }
                else
                {
                    Debug.LogWarning("A shader supporting instancing could not be found for the VisualProfiler, falling back to traditional rendering. This may impact performance.");
                }
            }

            if (Application.isPlaying)
            {
                backgroundMaterial = new Material(defaultMaterial);
                foregroundMaterial = new Material(defaultMaterial);
                defaultMaterial.renderQueue = foregroundMaterial.renderQueue - 1;
                backgroundMaterial.renderQueue = defaultMaterial.renderQueue - 1;

                MeshRenderer meshRenderer = new GameObject().AddComponent<TextMesh>().GetComponent<MeshRenderer>();
                textMaterial = new Material(meshRenderer.sharedMaterial);
                textMaterial.renderQueue = defaultMaterial.renderQueue;
                Destroy(meshRenderer.gameObject);

                MeshFilter quadMeshFilter = GameObject.CreatePrimitive(PrimitiveType.Quad).GetComponent<MeshFilter>();

                if (defaultInstancedMaterial != null)
                {
                    // Create a quad mesh with artificially large bounds to disable culling for instanced rendering.
                    quadMesh = quadMeshFilter.mesh;
                    quadMesh.bounds = new Bounds(Vector3.zero, Vector3.one * float.MaxValue);
                }
                else
                {
                    quadMesh = quadMeshFilter.sharedMesh;
                }

                Destroy(quadMeshFilter.gameObject);
            }

            stopwatch.Reset();
            stopwatch.Start();
        }

        /// <summary> Starts this object. </summary>
        private void Start()
        {
            Reset();
            BuildWindow();
            BuildFrameRateStrings();
        }

        /// <summary> Executes the 'destroy' action. </summary>
        private void OnDestroy()
        {
            Destroy(window);
        }

        /// <summary> Late update. </summary>
        private void LateUpdate()
        {
            if (window == null)
            {
                return;
            }

            // Update window transformation.
            if (window.activeSelf && CenterCamera != null)
            {
                float t = Time.deltaTime * windowFollowSpeed;
                window.transform.position = Vector3.Lerp(window.transform.position, CalculateWindowPosition(CenterCamera.transform), t);
                window.transform.rotation = Quaternion.Slerp(window.transform.rotation, CalculateWindowRotation(CenterCamera.transform), t);
                window.transform.localScale = defaultWindowScale * windowScale;
            }

            // Capture frame timings every frame and read from it depending on the frameSampleRate.
            FrameTimingManager.CaptureFrameTimings();

            ++frameCount;
            float elapsedSeconds = stopwatch.ElapsedMilliseconds * 0.001f;

            if (elapsedSeconds >= frameSampleRate)
            {
                int cpuFrameRate = (int)(1.0f / (elapsedSeconds / frameCount));
                int gpuFrameRate = 0;

                // Many platforms do not yet support the FrameTimingManager. When timing data is returned from the FrameTimingManager we will use
                // its timing data, else we will depend on the stopwatch.
                uint frameTimingsCount = FrameTimingManager.GetLatestTimings((uint)Mathf.Min(frameCount, maxFrameTimings), frameTimings);

                if (frameTimingsCount != 0)
                {
                    float cpuFrameTime, gpuFrameTime;
                    AverageFrameTiming(frameTimings, frameTimingsCount, out cpuFrameTime, out gpuFrameTime);
                    cpuFrameRate = (int)(1.0f / (cpuFrameTime / frameCount));
                    gpuFrameRate = (int)(1.0f / (gpuFrameTime / frameCount));
                }

                // Update frame rate text.
                cpuFrameRateText.text = cpuFrameRateStrings[Mathf.Clamp(cpuFrameRate, 0, maxTargetFrameRate)];
#if USING_XR_SDK && !UNITY_EDITOR
                int dropped_framecount = 0;
                NRSessionManager.Instance.XRDisplaySubsystem?.TryGetDroppedFrameCount(out dropped_framecount);
                droppedFrameCount.text = string.Format(droppedFrameCountString, dropped_framecount);
#endif

                if (gpuFrameRate != 0)
                {
                    gpuFrameRateText.gameObject.SetActive(true);
                    gpuFrameRateText.text = gpuFrameRateStrings[Mathf.Clamp(gpuFrameRate, 0, maxTargetFrameRate)];
                }

                // Update frame colors.
                for (int i = frameRange - 1; i > 0; --i)
                {
                    frameInfoColors[i] = frameInfoColors[i - 1];
                }

                // Ideally we would query a device specific API (like the HolographicFramePresentationReport) to detect missed frames.
                // But, many of these APIs are inaccessible in Unity. Currently missed frames are assumed when the average cpuFrameRate 
                // is under the target frame rate.
                frameInfoColors[0] = (cpuFrameRate < ((int)(AppFrameRate) - 1)) ? missedFrameRateColor : targetFrameRateColor;
                frameInfoPropertyBlock.SetVectorArray(colorID, frameInfoColors);

                // Reset timers.
                frameCount = 0;
                stopwatch.Reset();
                stopwatch.Start();
            }

            // Draw frame info.
            if (window.activeSelf)
            {
                Matrix4x4 parentLocalToWorldMatrix = window.transform.localToWorldMatrix;

                //if (defaultInstancedMaterial != null)
                //{
                //    frameInfoPropertyBlock.SetMatrix(parentMatrixID, parentLocalToWorldMatrix);
                //    Graphics.DrawMeshInstanced(quadMesh, 0, defaultInstancedMaterial, frameInfoMatrices, frameInfoMatrices.Length, frameInfoPropertyBlock, UnityEngine.Rendering.ShadowCastingMode.Off, false);
                //}
                //else
                //{
                // If a instanced material is not available, fall back to non-instanced rendering.
                for (int i = 0; i < frameInfoMatrices.Length; ++i)
                {
                    frameInfoPropertyBlock.SetColor(colorID, frameInfoColors[i]);
                    Graphics.DrawMesh(quadMesh, parentLocalToWorldMatrix * frameInfoMatrices[i], defaultMaterial, 0, null, 0, frameInfoPropertyBlock, false, false, false);
                }
                //}
            }

            // Update memory statistics.
            ulong limit = AppMemoryUsageLimit;

            if (limit != limitMemoryUsage)
            {
                if (window.activeSelf && WillDisplayedMemoryUsageDiffer(limitMemoryUsage, limit, displayedDecimalDigits))
                {
                    MemoryUsageToString(stringBuffer, displayedDecimalDigits, limitMemoryText, limitMemoryString, limit);
                }

                limitMemoryUsage = limit;
            }

            ulong usage = AppMemoryUsage;

            if (usage != memoryUsage)
            {
                usedAnchor.localScale = new Vector3((float)usage / limitMemoryUsage, usedAnchor.localScale.y, usedAnchor.localScale.z);

                if (window.activeSelf && WillDisplayedMemoryUsageDiffer(memoryUsage, usage, displayedDecimalDigits))
                {
                    MemoryUsageToString(stringBuffer, displayedDecimalDigits, usedMemoryText, usedMemoryString, usage);
                }

                memoryUsage = usage;
            }

            if (memoryUsage > peakMemoryUsage)
            {
                peakAnchor.localScale = new Vector3((float)memoryUsage / limitMemoryUsage, peakAnchor.localScale.y, peakAnchor.localScale.z);

                if (window.activeSelf && WillDisplayedMemoryUsageDiffer(peakMemoryUsage, memoryUsage, displayedDecimalDigits))
                {
                    MemoryUsageToString(stringBuffer, displayedDecimalDigits, peakMemoryText, peakMemoryString, memoryUsage);
                }

                peakMemoryUsage = memoryUsage;
            }

            window.SetActive(isVisible);
        }

        /// <summary> Calculates the window position. </summary>
        /// <param name="cameraTransform"> The camera transform.</param>
        /// <returns> The calculated window position. </returns>
        private Vector3 CalculateWindowPosition(Transform cameraTransform)
        {
            float windowDistance = Mathf.Max(16.0f / Camera.main.fieldOfView, Camera.main.nearClipPlane + 0.25f);
            Vector3 position = cameraTransform.position + (cameraTransform.forward * windowDistance);
            Vector3 horizontalOffset = cameraTransform.right * windowOffset.x;
            Vector3 verticalOffset = cameraTransform.up * windowOffset.y;

            switch (windowAnchor)
            {
                case TextAnchor.UpperLeft: position += verticalOffset - horizontalOffset; break;
                case TextAnchor.UpperCenter: position += verticalOffset; break;
                case TextAnchor.UpperRight: position += verticalOffset + horizontalOffset; break;
                case TextAnchor.MiddleLeft: position -= horizontalOffset; break;
                case TextAnchor.MiddleRight: position += horizontalOffset; break;
                case TextAnchor.LowerLeft: position -= verticalOffset + horizontalOffset; break;
                case TextAnchor.LowerCenter: position -= verticalOffset; break;
                case TextAnchor.LowerRight: position -= verticalOffset - horizontalOffset; break;
            }

            return position;
        }

        /// <summary> Calculates the window rotation. </summary>
        /// <param name="cameraTransform"> The camera transform.</param>
        /// <returns> The calculated window rotation. </returns>
        private Quaternion CalculateWindowRotation(Transform cameraTransform)
        {
            Quaternion rotation = cameraTransform.rotation;

            switch (windowAnchor)
            {
                case TextAnchor.UpperLeft: rotation *= windowHorizontalRotationInverse * windowVerticalRotationInverse; break;
                case TextAnchor.UpperCenter: rotation *= windowHorizontalRotationInverse; break;
                case TextAnchor.UpperRight: rotation *= windowHorizontalRotationInverse * windowVerticalRotation; break;
                case TextAnchor.MiddleLeft: rotation *= windowVerticalRotationInverse; break;
                case TextAnchor.MiddleRight: rotation *= windowVerticalRotation; break;
                case TextAnchor.LowerLeft: rotation *= windowHorizontalRotation * windowVerticalRotationInverse; break;
                case TextAnchor.LowerCenter: rotation *= windowHorizontalRotation; break;
                case TextAnchor.LowerRight: rotation *= windowHorizontalRotation * windowVerticalRotation; break;
            }

            return rotation;
        }

        /// <summary> Builds the window. </summary>
        private void BuildWindow()
        {
            // Initialize property block state.
            colorID = Shader.PropertyToID("_Color");
            parentMatrixID = Shader.PropertyToID("_ParentLocalToWorldMatrix");
            WindowParent = transform;
            // Build the window root.
            {
                window = CreateQuad("VisualProfiler", null);
                window.transform.parent = WindowParent;
                InitializeRenderer(window, backgroundMaterial, colorID, baseColor);
                window.transform.localScale = defaultWindowScale;
                windowHorizontalRotation = Quaternion.AngleAxis(defaultWindowRotation.y, Vector3.right);
                windowHorizontalRotationInverse = Quaternion.Inverse(windowHorizontalRotation);
                windowVerticalRotation = Quaternion.AngleAxis(defaultWindowRotation.x, Vector3.up);
                windowVerticalRotationInverse = Quaternion.Inverse(windowVerticalRotation);
            }

            // Add frame rate text and frame indicators.
            {
                cpuFrameRateText = CreateText("CPUFrameRateText", new Vector3(-0.495f, 0.5f, 0.0f), window.transform, TextAnchor.UpperLeft, textMaterial, Color.white, string.Empty);
#if USING_XR_SDK && !UNITY_EDITOR
                droppedFrameCount = CreateText("DroppedFrameCount", new Vector3(0, 0.5f, 0.0f), window.transform, TextAnchor.UpperLeft, textMaterial, Color.white, string.Empty);
#endif
                gpuFrameRateText = CreateText("GPUFrameRateText", new Vector3(0.495f, 0.5f, 0.0f), window.transform, TextAnchor.UpperRight, textMaterial, Color.white, string.Empty);
                gpuFrameRateText.gameObject.SetActive(false);

                frameInfoMatrices = new Matrix4x4[frameRange];
                frameInfoColors = new Vector4[frameRange];
                Vector3 scale = new Vector3(1.0f / frameRange, 0.2f, 1.0f);
                Vector3 position = new Vector3(0.5f - (scale.x * 0.5f), 0.15f, 0.0f);

                for (int i = 0; i < frameRange; ++i)
                {
                    frameInfoMatrices[i] = Matrix4x4.TRS(position, Quaternion.identity, new Vector3(scale.x * 0.8f, scale.y, scale.z));
                    position.x -= scale.x;
                    frameInfoColors[i] = targetFrameRateColor;
                }

                frameInfoPropertyBlock = new MaterialPropertyBlock();
                frameInfoPropertyBlock.SetVectorArray(colorID, frameInfoColors);
            }

            // Add memory usage text and bars.
            {
                usedMemoryText = CreateText("UsedMemoryText", new Vector3(-0.495f, 0.0f, 0.0f), window.transform, TextAnchor.UpperLeft, textMaterial, memoryUsedColor, usedMemoryString);
                peakMemoryText = CreateText("PeakMemoryText", new Vector3(0.0f, 0.0f, 0.0f), window.transform, TextAnchor.UpperCenter, textMaterial, memoryPeakColor, peakMemoryString);
                limitMemoryText = CreateText("LimitMemoryText", new Vector3(0.495f, 0.0f, 0.0f), window.transform, TextAnchor.UpperRight, textMaterial, Color.white, limitMemoryString);

                GameObject limitBar = CreateQuad("LimitBar", window.transform);
                InitializeRenderer(limitBar, defaultMaterial, colorID, memoryLimitColor);
                limitBar.transform.localScale = new Vector3(0.99f, 0.2f, 1.0f);
                limitBar.transform.localPosition = new Vector3(0.0f, -0.37f, 0.0f);

                {
                    usedAnchor = CreateAnchor("UsedAnchor", limitBar.transform);
                    GameObject bar = CreateQuad("UsedBar", usedAnchor);
                    Material material = new Material(foregroundMaterial);
                    material.renderQueue = material.renderQueue + 1;
                    InitializeRenderer(bar, material, colorID, memoryUsedColor);
                    bar.transform.localScale = Vector3.one;
                    bar.transform.localPosition = new Vector3(0.5f, 0.0f, 0.0f);
                }
                {
                    peakAnchor = CreateAnchor("PeakAnchor", limitBar.transform);
                    GameObject bar = CreateQuad("PeakBar", peakAnchor);
                    InitializeRenderer(bar, foregroundMaterial, colorID, memoryPeakColor);
                    bar.transform.localScale = Vector3.one;
                    bar.transform.localPosition = new Vector3(0.5f, 0.0f, 0.0f);
                }
            }

            window.SetActive(isVisible);
        }

        /// <summary> Builds frame rate strings. </summary>
        private void BuildFrameRateStrings()
        {
            cpuFrameRateStrings = new string[maxTargetFrameRate + 1];
            gpuFrameRateStrings = new string[maxTargetFrameRate + 1];
            string displayedDecimalFormat = string.Format("{{0:F{0}}}", displayedDecimalDigits);

            StringBuilder stringBuilder = new StringBuilder(32);
            StringBuilder milisecondStringBuilder = new StringBuilder(16);

            for (int i = 0; i < cpuFrameRateStrings.Length; ++i)
            {
                float miliseconds = (i == 0) ? 0.0f : (1.0f / i) * 1000.0f;
                milisecondStringBuilder.AppendFormat(displayedDecimalFormat, miliseconds);
                stringBuilder.AppendFormat("CPU: {0} fps ({1} ms)", i.ToString(), milisecondStringBuilder.ToString());
                cpuFrameRateStrings[i] = stringBuilder.ToString();
                stringBuilder.Length = 0;
                stringBuilder.AppendFormat("GPU: {0} fps ({1} ms)", i.ToString(), milisecondStringBuilder.ToString());
                gpuFrameRateStrings[i] = stringBuilder.ToString();
                milisecondStringBuilder.Length = 0;
                stringBuilder.Length = 0;
            }
        }

        /// <summary> Creates an anchor. </summary>
        /// <param name="name">   The name.</param>
        /// <param name="parent"> The parent.</param>
        /// <returns> The new anchor. </returns>
        private static Transform CreateAnchor(string name, Transform parent)
        {
            Transform anchor = new GameObject(name).transform;
            anchor.parent = parent;
            anchor.localScale = Vector3.one;
            anchor.localPosition = new Vector3(-0.5f, 0.0f, 0.0f);

            return anchor;
        }

        /// <summary> Creates a quad. </summary>
        /// <param name="name">   The name.</param>
        /// <param name="parent"> The parent.</param>
        /// <returns> The new quad. </returns>
        private static GameObject CreateQuad(string name, Transform parent)
        {
            GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Destroy(quad.GetComponent<Collider>());
            quad.name = name;
            quad.transform.parent = parent;

            return quad;
        }

        /// <summary> Creates a text. </summary>
        /// <param name="name">     The name.</param>
        /// <param name="position"> The position.</param>
        /// <param name="parent">   The parent.</param>
        /// <param name="anchor">   The anchor.</param>
        /// <param name="material"> The material.</param>
        /// <param name="color">    The color.</param>
        /// <param name="text">     The text.</param>
        /// <returns> The new text. </returns>
        private static TextMesh CreateText(string name, Vector3 position, Transform parent, TextAnchor anchor, Material material, Color color, string text)
        {
            GameObject obj = new GameObject(name);
            obj.transform.localScale = Vector3.one * 0.0016f;
            obj.transform.parent = parent;
            obj.transform.localPosition = position;
            TextMesh textMesh = obj.AddComponent<TextMesh>();
            textMesh.fontSize = 48;
            textMesh.anchor = anchor;
            textMesh.color = color;
            textMesh.text = text;
            textMesh.richText = false;

            Renderer renderer = obj.GetComponent<Renderer>();
            renderer.sharedMaterial = material;

            OptimizeRenderer(renderer);

            return textMesh;
        }

        /// <summary> Initializes the renderer. </summary>
        /// <param name="obj">      The object.</param>
        /// <param name="material"> The material.</param>
        /// <param name="colorID">  Identifier for the color.</param>
        /// <param name="color">    The color.</param>
        /// <returns> A Renderer. </returns>
        private static Renderer InitializeRenderer(GameObject obj, Material material, int colorID, Color color)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            renderer.sharedMaterial = material;

            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor(colorID, color);
            renderer.SetPropertyBlock(propertyBlock);

            OptimizeRenderer(renderer);

            return renderer;
        }

        /// <summary> Optimize renderer. </summary>
        /// <param name="renderer"> The renderer.</param>
        private static void OptimizeRenderer(Renderer renderer)
        {
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            renderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
            renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            renderer.allowOcclusionWhenDynamic = false;
        }

        /// <summary> Memory usage to string. </summary>
        /// <param name="stringBuffer">           Buffer for string data.</param>
        /// <param name="displayedDecimalDigits"> The displayed decimal digits.</param>
        /// <param name="textMesh">               The text mesh.</param>
        /// <param name="prefixString">           The prefix string.</param>
        /// <param name="memoryUsage">            The memory usage.</param>
        private static void MemoryUsageToString(char[] stringBuffer, int displayedDecimalDigits, TextMesh textMesh, string prefixString, ulong memoryUsage)
        {
            // Using a custom number to string method to avoid the overhead, and allocations, of built in string.Format/StringBuilder methods.
            // We can also make some assumptions since the domain of the input number (memoryUsage) is known.
            float memoryUsageMB = ConvertBytesToMegabytes(memoryUsage);
            int memoryUsageIntegerDigits = (int)memoryUsageMB;
            int memoryUsageFractionalDigits = (int)((memoryUsageMB - memoryUsageIntegerDigits) * Mathf.Pow(10.0f, displayedDecimalDigits));
            int bufferIndex = 0;

            for (int i = 0; i < prefixString.Length; ++i)
            {
                stringBuffer[bufferIndex++] = prefixString[i];
            }

            bufferIndex = MemoryItoA(memoryUsageIntegerDigits, stringBuffer, bufferIndex);
            stringBuffer[bufferIndex++] = '.';

            if (memoryUsageFractionalDigits != 0)
            {
                bufferIndex = MemoryItoA(memoryUsageFractionalDigits, stringBuffer, bufferIndex);
            }
            else
            {
                for (int i = 0; i < displayedDecimalDigits; ++i)
                {
                    stringBuffer[bufferIndex++] = '0';
                }
            }

            stringBuffer[bufferIndex++] = 'M';
            stringBuffer[bufferIndex++] = 'B';
            textMesh.text = new string(stringBuffer, 0, bufferIndex);
        }

        /// <summary> Memory ito a. </summary>
        /// <param name="value">        The value.</param>
        /// <param name="stringBuffer"> Buffer for string data.</param>
        /// <param name="bufferIndex">  Zero-based index of the buffer.</param>
        /// <returns> An int. </returns>
        private static int MemoryItoA(int value, char[] stringBuffer, int bufferIndex)
        {
            int startIndex = bufferIndex;

            for (; value != 0; value /= 10)
            {
                stringBuffer[bufferIndex++] = (char)((char)(value % 10) + '0');
            }

            char temp;
            for (int endIndex = bufferIndex - 1; startIndex < endIndex; ++startIndex, --endIndex)
            {
                temp = stringBuffer[startIndex];
                stringBuffer[startIndex] = stringBuffer[endIndex];
                stringBuffer[endIndex] = temp;
            }

            return bufferIndex;
        }

        /// <summary> Gets the application frame rate. </summary>
        /// <value> The application frame rate. </value>
        private static float AppFrameRate
        {
            get
            {
                // If the current XR SDK does not report refresh rate information, assume 60Hz.
                //float refreshRate = UnityEngine.XR.XRDevice.refreshRate;
                //return ((int)refreshRate == 0) ? 60.0f : refreshRate;
                return 45f;
            }
        }

        /// <summary> Average frame timing. </summary>
        /// <param name="frameTimings">      The frame timings.</param>
        /// <param name="frameTimingsCount"> Number of frame timings.</param>
        /// <param name="cpuFrameTime">      [out] The CPU frame time.</param>
        /// <param name="gpuFrameTime">      [out] The GPU frame time.</param>
        private static void AverageFrameTiming(FrameTiming[] frameTimings, uint frameTimingsCount, out float cpuFrameTime, out float gpuFrameTime)
        {
            double cpuTime = 0.0f;
            double gpuTime = 0.0f;

            for (int i = 0; i < frameTimingsCount; ++i)
            {
                cpuTime += frameTimings[i].cpuFrameTime;
                gpuTime += frameTimings[i].gpuFrameTime;
            }

            cpuTime /= frameTimingsCount;
            gpuTime /= frameTimingsCount;

            cpuFrameTime = (float)(cpuTime * 0.001);
            gpuFrameTime = (float)(gpuTime * 0.001);
        }

        /// <summary> Gets the application memory usage. </summary>
        /// <value> The application memory usage. </value>
        private static ulong AppMemoryUsage
        {
            get
            {
#if WINDOWS_UWP
                return MemoryManager.AppMemoryUsage;
#else
                return (ulong)Profiler.GetTotalAllocatedMemoryLong();
#endif
            }
        }

        /// <summary> Gets the application memory usage limit. </summary>
        /// <value> The application memory usage limit. </value>
        private static ulong AppMemoryUsageLimit
        {
            get
            {
#if WINDOWS_UWP
                return MemoryManager.AppMemoryUsageLimit;
#else
                return ConvertMegabytesToBytes(SystemInfo.systemMemorySize);
#endif
            }
        }

        /// <summary> Will displayed memory usage differ. </summary>
        /// <param name="oldUsage">               The old usage.</param>
        /// <param name="newUsage">               The new usage.</param>
        /// <param name="displayedDecimalDigits"> The displayed decimal digits.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        private static bool WillDisplayedMemoryUsageDiffer(ulong oldUsage, ulong newUsage, int displayedDecimalDigits)
        {
            float oldUsageMBs = ConvertBytesToMegabytes(oldUsage);
            float newUsageMBs = ConvertBytesToMegabytes(newUsage);
            float decimalPower = Mathf.Pow(10.0f, displayedDecimalDigits);

            return (int)(oldUsageMBs * decimalPower) != (int)(newUsageMBs * decimalPower);
        }

        /// <summary> Convert megabytes to bytes. </summary>
        /// <param name="megabytes"> The megabytes.</param>
        /// <returns> The megabytes converted to bytes. </returns>
        private static ulong ConvertMegabytesToBytes(int megabytes)
        {
            return ((ulong)megabytes * 1024UL) * 1024UL;
        }

        /// <summary> Convert bytes to megabytes. </summary>
        /// <param name="bytes"> The bytes.</param>
        /// <returns> The bytes converted to megabytes. </returns>
        private static float ConvertBytesToMegabytes(ulong bytes)
        {
            return (bytes / 1024.0f) / 1024.0f;
        }
    }
}
