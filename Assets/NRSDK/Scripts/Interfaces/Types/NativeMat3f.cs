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

    /// <summary> A native matrix 3f. </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NativeMat3f
    {
        /// <summary> The column 0. </summary>
        [MarshalAs(UnmanagedType.Struct)]
        public NativeVector3f column0;
        /// <summary> The first column. </summary>
        [MarshalAs(UnmanagedType.Struct)]
        public NativeVector3f column1;
        /// <summary> The second column. </summary>
        [MarshalAs(UnmanagedType.Struct)]
        public NativeVector3f column2;

        /// <summary>
        /// Indexer to get or set items within this collection using array index syntax. </summary>
        /// <param name="i"> Zero-based index of the entry to access.</param>
        /// <param name="j"> An int to process.</param>
        /// <returns> The indexed item. </returns>
        public float this[int i, int j]
        {
            get
            {
                if (j == 0)
                {
                    return column0[i];
                }
                if (j == 1)
                {
                    return column1[i];
                }
                if (j == 2)
                {
                    return column2[i];
                }
                return -1;
            }
            set
            {
                if (j == 0)
                {
                    column0[i] = value;
                }
                if (j == 1)
                {
                    column1[i] = value;
                }
                if (j == 2)
                {
                    column2[i] = value;
                }
            }
        }

        /// <summary> Constructor. </summary>
        /// <param name="vec0"> The vector 0.</param>
        /// <param name="vec1"> The first vector.</param>
        /// <param name="vec2"> The second vector.</param>
        public NativeMat3f(Vector3 vec0, Vector3 vec1, Vector3 vec2)
        {
            column0 = new NativeVector3f(vec0);
            column1 = new NativeVector3f(vec1);
            column2 = new NativeVector3f(vec2);
        }

        /// <summary> Gets the identity. </summary>
        /// <value> The identity. </value>
        public static NativeMat3f identity
        {
            get
            {
                return new NativeMat3f(Vector3.zero, Vector3.zero, Vector3.zero);
            }
        }

        /// <summary>
        /// Indexer to get or set items within this collection using array index syntax. </summary>
        /// <exception cref="IndexOutOfRangeException"> Thrown when the index is outside the required
        ///                                             range.</exception>
        /// <param name="index"> Zero-based index of the entry to access.</param>
        /// <returns> The indexed item. </returns>
        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return this.column0.X;
                    case 1:
                        return this.column1.X;
                    case 2:
                        return this.column2.X;
                    case 3:
                        return this.column0.Y;
                    case 4:
                        return this.column1.Y;
                    case 5:
                        return this.column2.Y;
                    case 6:
                        return this.column0.Z;
                    case 7:
                        return this.column1.Z;
                    case 8:
                        return this.column2.Z;
                    default:
                        throw new System.IndexOutOfRangeException("Invalid matrix index!");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        this.column0.X = value;
                        break;
                    case 1:
                        this.column1.X = value;
                        break;
                    case 2:
                        this.column2.X = value;
                        break;
                    case 3:
                        this.column0.Y = value;
                        break;
                    case 4:
                        this.column1.Y = value;
                        break;
                    case 5:
                        this.column2.Y = value;
                        break;
                    case 6:
                        this.column0.Z = value;
                        break;
                    case 7:
                        this.column1.Z = value;
                        break;
                    case 8:
                        this.column2.Z = value;
                        break;
                    default:
                        throw new System.IndexOutOfRangeException("Invalid matrix index!");
                }
            }
        }

        /// <summary> Convert this object into a string representation. </summary>
        /// <returns> A string that represents this object. </returns>
        public override string ToString()
        {
            return string.Format("column0:{0}\ncolumn1:{1}\ncolumn2:{2}\n",
                column0.ToString(), column1.ToString(), column2.ToString());
        }
    }
}
