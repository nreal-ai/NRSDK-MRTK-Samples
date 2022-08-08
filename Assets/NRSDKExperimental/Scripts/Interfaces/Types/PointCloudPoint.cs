/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.Experimental
{
    using System;
    using System.Runtime.InteropServices;

    public enum ConfidenceLevel
    {
        Nice = 0,
        Good,
        Normal,
        Poor,
        Bad,
        VeryBad
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PointCloudPoint
    {
        [MarshalAs(UnmanagedType.I8)]
        public Int64 Id;
        [MarshalAs(UnmanagedType.Struct)]
        public NativeVector3f Position;
        [MarshalAs(UnmanagedType.R4)]
        public float Confidence;

        public ConfidenceLevel confidenceLevel
        {
            get
            {
                return ParaseToConfidenceLevel((int)Confidence);
            }
        }

        public override string ToString()
        {
            return string.Format("id:{0} pos:{1} confidence:{2}", Id, Position, Confidence);
        }

        public static ConfidenceLevel ParaseToConfidenceLevel(int val)
        {
            if (val < (int)ConfidenceLevel.Nice)
            {
                return ConfidenceLevel.Nice;
            }
            else if (val > (int)ConfidenceLevel.VeryBad)
            {
                return ConfidenceLevel.VeryBad;
            }
            else
            {
                return (ConfidenceLevel)val;
            }
        }
    }
}
