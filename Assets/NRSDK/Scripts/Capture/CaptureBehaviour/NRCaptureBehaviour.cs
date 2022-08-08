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
    using System.IO;

    /// <summary>
    /// Capture a image from the MR world. You can capture a RGB only,Virtual only or Blended image
    /// through this class. </summary>
    public class NRCaptureBehaviour : CaptureBehaviourBase
    {
        /// <summary> Gets the image encoder. </summary>
        /// <value> The image encoder. </value>
        private ImageEncoder ImageEncoder
        {
            get
            {
                return this.GetContext().GetEncoder() as ImageEncoder;
            }
        }

        /// <summary> Does the given file. </summary>
        /// <param name="width">   The width.</param>
        /// <param name="height">  The height.</param>
        /// <param name="format">  Describes the format to use.</param>
        /// <param name="outpath"> The outpath.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Do(int width, int height, PhotoCaptureFileOutputFormat format, string outpath)
        {
            var data = this.ImageEncoder.Encode(width, height, format);
            if (data == null)
            {
                return false;
            }
            File.WriteAllBytes(outpath, data);

            return true;
        }

        /// <summary> Does the given file. </summary>
        /// <param name="width">  The width.</param>
        /// <param name="height"> The height.</param>
        /// <param name="format"> Describes the format to use.</param>
        /// <param name="data">   [in,out] The data.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Do(int width, int height, PhotoCaptureFileOutputFormat format, ref byte[] data)
        {
            data = this.ImageEncoder.Encode(width, height, format);
            if (data == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Capture a image Asyn. if system supports AsyncGPUReadback, using AsyncGPUReadback to get the
        /// captured image, else getting the image by synchronization way. </summary>
        /// <param name="task"> The task.</param>
        private void DoAsyn(CaptureTask task)
        {
            if (SystemInfo.supportsAsyncGPUReadback)
            {
                this.ImageEncoder.Commit(task);
            }
            else
            {
                var data = ImageEncoder.Encode(task.Width, task.Height, task.CaptureFormat);
                if (task.OnReceive != null)
                {
                    task.OnReceive(task, data);
                }
            }
        }

        /// <summary>
        /// Capture a image Asyn. if system supports AsyncGPUReadback, using AsyncGPUReadback to get the
        /// captured image, else getting the image by synchronization way. </summary>
        /// <param name="oncapturedcallback"> The oncapturedcallback.</param>
        public void DoAsyn(NRPhotoCapture.OnCapturedToMemoryCallback oncapturedcallback)
        {
            var captureTask = new CaptureTask();
            var cameraParam = this.GetContext().RequestCameraParam();
            captureTask.Width = cameraParam.cameraResolutionWidth;
            captureTask.Height = cameraParam.cameraResolutionHeight;
            captureTask.CaptureFormat = cameraParam.pixelFormat == CapturePixelFormat.PNG ? PhotoCaptureFileOutputFormat.PNG : PhotoCaptureFileOutputFormat.JPG;
            captureTask.OnReceive += (task, data) =>
            {
                if (oncapturedcallback != null)
                {
                    var result = new NRPhotoCapture.PhotoCaptureResult();
                    result.resultType = NRPhotoCapture.CaptureResultType.Success;
                    CapturePixelFormat format = task.CaptureFormat == PhotoCaptureFileOutputFormat.PNG ? CapturePixelFormat.PNG : CapturePixelFormat.JPEG;
                    PhotoCaptureFrame frame = new PhotoCaptureFrame(format, data);
                    oncapturedcallback(result, frame);
                }
            };

            this.DoAsyn(captureTask);
        }

        /// <summary> Does the given file. </summary>
        /// <param name="filename">         Filename of the file.</param>
        /// <param name="fileOutputFormat"> The file output format.</param>
        public void Do(string filename, PhotoCaptureFileOutputFormat fileOutputFormat)
        {
            var cameraParam = this.GetContext().RequestCameraParam();
            this.Do(cameraParam.cameraResolutionWidth,
                    cameraParam.cameraResolutionHeight,
                    fileOutputFormat,
                    filename
            );
        }
    }
}