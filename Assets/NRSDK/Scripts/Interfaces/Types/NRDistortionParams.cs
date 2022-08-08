/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/                  
* 
*****************************************************************************/

using System.Runtime.InteropServices;

namespace NRKernal
{
    /// <summary> Values that represent nr camera models. </summary>
    public enum NRCameraModel
    {
        /// <summary> An enum constant representing the nr camera model radial option. </summary>
        NR_CAMERA_MODEL_RADIAL = 1,
        /// <summary> An enum constant representing the nr camera model fisheye option. </summary>
        NR_CAMERA_MODEL_FISHEYE = 2,
    }

    /// <summary>
    ///     if camera_model == NR_CAMERA_MODEL_RADIAL,the first 4 value of distortParams is:
    /// // radial_k1、radial_k2、radial_r1、radial_r2. // else if camera_model ==
    /// NR_CAMERA_MODEL_FISHEYE: // fisheye_k1、fisheye_k2、fisheye_k3、fisheye_k4. </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NRDistortionParams
    {
        /// <summary> The camera model. </summary>
        [MarshalAs(UnmanagedType.I4)]
        public NRCameraModel cameraModel;
        /// <summary> The first distort parameters. </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float distortParams1;
        /// <summary> The second distort parameters. </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float distortParams2;
        /// <summary> The third distort parameters. </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float distortParams3;
        /// <summary> The fourth distort parameters. </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float distortParams4;
        /// <summary> The fifth distort parameters. </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float distortParams5;
        /// <summary> The distort parameters 6. </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float distortParams6;
        /// <summary> The distort parameters 7. </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float distortParams7;

        /// <summary> Convert this object into a string representation. </summary>
        /// <returns> A string that represents this object. </returns>
        public override string ToString()
        {
            return string.Format("cameraModel:{0} distortParams1:{1} distortParams2:{2} distortParams3:{3} distortParams4:{4} distortParams5:{5} distortParams6:{6} distortParams7:{7}",
                cameraModel, distortParams1, distortParams2, distortParams3, distortParams4, distortParams5, distortParams6, distortParams7);
        }
    }
}
