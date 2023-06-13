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
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary> A native vector 3f. </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NativeVector3f
    {
        /// <summary> The X coordinate. </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float X;
        /// <summary> The Y coordinate. </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float Y;
        /// <summary> The Z coordinate. </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float Z;

        /// <summary> Constructor. </summary>
        /// <param name="v"> A Vector3 to process.</param>
        public NativeVector3f(Vector3 v)
        {
            X = v.x;
            Y = v.y;
            Z = v.z;
        }

        public static NativeVector3f identity
        {
            get
            {
                return new NativeVector3f(Vector3.zero);
            }
        }

        /// <summary>
        /// Indexer to get or set items within this collection using array index syntax. </summary>
        /// <param name="i"> Zero-based index of the entry to access.</param>
        /// <returns> The indexed item. </returns>
        public float this[int i]
        {
            get
            {
                if (i == 0)
                {
                    return X;
                }
                if (i == 1)
                {
                    return Y;
                }
                if (i == 2)
                {
                    return Z;
                }
                return -1;
            }
            set
            {
                if (i == 0)
                {
                    X = value;
                }
                if (i == 1)
                {
                    Y = value;
                }
                if (i == 2)
                {
                    Z = value;
                }
            }
        }

        /// <summary> Converts this object to an unity vector 3. </summary>
        /// <returns> This object as a Vector3. </returns>
        public Vector3 ToUnityVector3()
        {
            return new Vector3(X, Y, -Z);
        }
        /// <summary> Convert this object into a string representation. </summary>
        /// <returns> A string that represents this object. </returns>
        public override string ToString()
        {
            return string.Format("x:{0}, y:{1}, z:{2}", X, Y, Z);
        }
    }
}
