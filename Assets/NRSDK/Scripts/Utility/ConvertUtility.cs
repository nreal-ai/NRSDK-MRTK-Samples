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
    using System;
    using System.Text;
    using UnityEngine.Assertions;

    public static class ConvertUtility
    {
        public static float IntBitsToFloat(int v)
        {
            byte[] buf = BitConverter.GetBytes(v);
            return BitConverter.ToSingle(buf, 0);
        }

        public static int FloatToRawIntBits(float v)
        {
            byte[] buf = BitConverter.GetBytes(v);
            return BitConverter.ToInt32(buf, 0);
        }

        public static string ToString(this float[] data)
        {
            Assert.IsTrue(data != null);
            StringBuilder st = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                st.Append(data[i] + " ");
            }
            return st.ToString();
        }
    }
}
