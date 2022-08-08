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
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary> Contains information captured from the web camera. </summary>
    public sealed class PhotoCaptureFrame : IDisposable
    {
        /// <summary> The data. </summary>
        private byte[] data;

        /// <summary> Constructor. </summary>
        /// <param name="format"> Describes the format to use.</param>
        /// <param name="data">   The data.</param>
        public PhotoCaptureFrame(CapturePixelFormat format, byte[] data)
        {
            this.data = data;
            this.pixelFormat = format;
        }

        /// <summary> Finalizer. </summary>
        ~PhotoCaptureFrame()
        {

        }

        /// <summary> The length of the raw IMFMediaBuffer which contains the image captured. </summary>
        /// <value> The length of the data. </value>
        public int dataLength { get; }

        /// <summary> Specifies whether or not spatial data was captured. </summary>
        /// <value> True if this object has location data, false if not. </value>
        public bool hasLocationData { get; }

        /// <summary> The raw image data pixel format. </summary>
        /// <value> The pixel format. </value>
        public CapturePixelFormat pixelFormat { get; }

        /// <summary> Copies the raw image data into buffer described by byteBuffer. </summary>
        /// <param name="byteBuffer"> Buffer for byte data.</param>
        public void CopyRawImageDataIntoBuffer(List<byte> byteBuffer)
        {

        }

        /// <summary> Disposes the PhotoCaptureFrame and any resources it uses. </summary>
        public void Dispose()
        {

        }

        /// <summary>
        /// Provides a COM pointer to the native IMFMediaBuffer that contains the image data. </summary>
        /// <returns> A native COM pointer to the IMFMediaBuffer which contains the image data. </returns>
        public IntPtr GetUnsafePointerToBuffer()
        {
            return IntPtr.Zero;
        }

        /// <summary> Attempts to get camera to world matrix. </summary>
        /// <param name="cameraToWorldMatrix"> [out] The camera to world matrix.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool TryGetCameraToWorldMatrix(out Matrix4x4 cameraToWorldMatrix)
        {
            cameraToWorldMatrix = Matrix4x4.identity;
            return true;
        }

        /// <summary> Attempts to get projection matrix. </summary>
        /// <param name="projectionMatrix"> [out] The projection matrix.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool TryGetProjectionMatrix(out Matrix4x4 projectionMatrix)
        {
            projectionMatrix = Matrix4x4.identity;
            return true;
        }

        /// <summary> Attempts to get projection matrix. </summary>
        /// <param name="nearClipPlane">    The near clip plane.</param>
        /// <param name="farClipPlane">     The far clip plane.</param>
        /// <param name="projectionMatrix"> [out] The projection matrix.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool TryGetProjectionMatrix(float nearClipPlane, float farClipPlane, out Matrix4x4 projectionMatrix)
        {
            projectionMatrix = Matrix4x4.identity;
            return true;
        }

        /// <summary>
        /// This method will copy the captured image data into a user supplied texture for use in Unity. </summary>
        /// <param name="targetTexture"> The target texture that the captured image data will be copied to.</param>
        public void UploadImageDataToTexture(Texture2D targetTexture)
        {
            if (data == null)
            {
                return;
            }
            ImageConversion.LoadImage(targetTexture, data);
        }
    }
}
