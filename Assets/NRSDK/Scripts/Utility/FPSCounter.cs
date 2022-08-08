namespace NRKernal
{
    using UnityEngine;

    /// <summary> The FPS counter. </summary>
    public class FPSCounter : MonoBehaviour
    {
        /// <summary> The frame range. </summary>
        public int frameRange = 60;

        /// <summary> Gets or sets the average FPS. </summary>
        /// <value> The average FPS. </value>
        public int AverageFPS { get; private set; }
        /// <summary> Gets or sets the highest FPS. </summary>
        /// <value> The highest FPS. </value>
        public int HighestFPS { get; private set; }
        /// <summary> Gets or sets the lowest FPS. </summary>
        /// <value> The lowest FPS. </value>
        public int LowestFPS { get; private set; }

        /// <summary> Buffer for FPS data. </summary>
        int[] fpsBuffer;
        /// <summary> Zero-based index of the FPS buffer. </summary>
        int fpsBufferIndex;

        /// <summary> Updates this object. </summary>
        void Update()
        {
            if (fpsBuffer == null || fpsBuffer.Length != frameRange)
            {
                InitializeBuffer();
            }
            UpdateBuffer();
            CalculateFPS();
        }

        /// <summary> Initializes the buffer. </summary>
        void InitializeBuffer()
        {
            if (frameRange <= 0)
            {
                frameRange = 1;
            }
            fpsBuffer = new int[frameRange];
            fpsBufferIndex = 0;
        }

        /// <summary> Updates the buffer. </summary>
        void UpdateBuffer()
        {
            fpsBuffer[fpsBufferIndex++] = (int)(1f / Time.unscaledDeltaTime);
            if (fpsBufferIndex >= frameRange)
            {
                fpsBufferIndex = 0;
            }
        }

        /// <summary> Calculates the FPS. </summary>
        void CalculateFPS()
        {
            int sum = 0;
            int highest = 0;
            int lowest = int.MaxValue;
            for (int i = 0; i < frameRange; i++)
            {
                int fps = fpsBuffer[i];
                sum += fps;
                if (fps > highest)
                {
                    highest = fps;
                }
                if (fps < lowest)
                {
                    lowest = fps;
                }
            }
            AverageFPS = (int)((float)sum / frameRange);
            HighestFPS = highest;
            LowestFPS = lowest;
        }
    }
}