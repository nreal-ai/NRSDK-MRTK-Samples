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

    /// <summary> A helper to format string. </summary>
    public static class StringUtility
    {
        /// <summary> The cached string builder. </summary>
        [ThreadStatic]
        private static StringBuilder s_CachedStringBuilder = null;

        /// <summary> Get format string. </summary>
        /// <exception cref="Exception"> Thrown when an exception error condition occurs.</exception>
        /// <param name="format"> Describes the format to use.</param>
        /// <param name="arg0">   The argument 0.</param>
        /// <returns> The formatted value. </returns>
        public static string Format(string format, object arg0)
        {
            if (format == null)
            {
                throw new Exception("Format is invalid.");
            }

            CheckCachedStringBuilder();
            s_CachedStringBuilder.Length = 0;
            s_CachedStringBuilder.AppendFormat(format, arg0);
            return s_CachedStringBuilder.ToString();
        }

        /// <summary> Get format string. </summary>
        /// <exception cref="Exception"> Thrown when an exception error condition occurs.</exception>
        /// <param name="format"> Describes the format to use.</param>
        /// <param name="arg0">   The argument 0.</param>
        /// <param name="arg1">   The first argument.</param>
        /// <returns> The formatted value. </returns>
        public static string Format(string format, object arg0, object arg1)
        {
            if (format == null)
            {
                throw new Exception("Format is invalid.");
            }

            CheckCachedStringBuilder();
            s_CachedStringBuilder.Length = 0;
            s_CachedStringBuilder.AppendFormat(format, arg0, arg1);
            return s_CachedStringBuilder.ToString();
        }

        /// <summary> Get format string. </summary>
        /// <exception cref="Exception"> Thrown when an exception error condition occurs.</exception>
        /// <param name="format"> Describes the format to use.</param>
        /// <param name="arg0">   The argument 0.</param>
        /// <param name="arg1">   The first argument.</param>
        /// <param name="arg2">   The second argument.</param>
        /// <returns> The formatted value. </returns>
        public static string Format(string format, object arg0, object arg1, object arg2)
        {
            if (format == null)
            {
                throw new Exception("Format is invalid.");
            }

            CheckCachedStringBuilder();
            s_CachedStringBuilder.Length = 0;
            s_CachedStringBuilder.AppendFormat(format, arg0, arg1, arg2);
            return s_CachedStringBuilder.ToString();
        }

        /// <summary> Get format string. </summary>
        /// <exception cref="Exception"> Thrown when an exception error condition occurs.</exception>
        /// <param name="format"> Describes the format to use.</param>
        /// <param name="args">   A variable-length parameters list containing arguments.</param>
        /// <returns> The formatted value. </returns>
        public static string Format(string format, params object[] args)
        {
            if (format == null)
            {
                throw new Exception("Format is invalid.");
            }

            if (args == null)
            {
                throw new Exception("Args is invalid.");
            }

            CheckCachedStringBuilder();
            s_CachedStringBuilder.Length = 0;
            s_CachedStringBuilder.AppendFormat(format, args);
            return s_CachedStringBuilder.ToString();
        }

        /// <summary> Get full name by type. </summary>
        /// <typeparam name="T"> Generic type parameter.</typeparam>
        /// <param name="name"> The name.</param>
        /// <returns> The full name. </returns>
        public static string GetFullName<T>(string name)
        {
            return GetFullName(typeof(T), name);
        }

        /// <summary> Get full name by type. </summary>
        /// <exception cref="Exception"> Thrown when an exception error condition occurs.</exception>
        /// <param name="type"> The type.</param>
        /// <param name="name"> The name.</param>
        /// <returns> The full name. </returns>
        public static string GetFullName(Type type, string name)
        {
            if (type == null)
            {
                throw new Exception("Type is invalid.");
            }

            string typeName = type.FullName;
            return string.IsNullOrEmpty(name) ? typeName : Format("{0}.{1}", typeName, name);
        }

        /// <summary> Gets byte length string. </summary>
        /// <param name="byteLength"> Length of the byte.</param>
        /// <returns> The byte length string. </returns>
        public static string GetByteLengthString(long byteLength)
        {
            if (byteLength < 1024L) // 2 ^ 10
            {
                return Format("{0} B", byteLength.ToString());
            }

            if (byteLength < 1048576L) // 2 ^ 20
            {
                return Format("{0} KB", (byteLength / 1024f).ToString("F2"));
            }

            if (byteLength < 1073741824L) // 2 ^ 30
            {
                return Format("{0} MB", (byteLength / 1048576f).ToString("F2"));
            }

            if (byteLength < 1099511627776L) // 2 ^ 40
            {
                return Format("{0} GB", (byteLength / 1073741824f).ToString("F2"));
            }

            if (byteLength < 1125899906842624L) // 2 ^ 50
            {
                return Format("{0} TB", (byteLength / 1099511627776f).ToString("F2"));
            }

            if (byteLength < 1152921504606846976L) // 2 ^ 60
            {
                return Format("{0} PB", (byteLength / 1125899906842624f).ToString("F2"));
            }

            return Format("{0} EB", (byteLength / 1152921504606846976f).ToString("F2"));
        }

        /// <summary> Check cached string builder. </summary>
        private static void CheckCachedStringBuilder()
        {
            if (s_CachedStringBuilder == null)
            {
                s_CachedStringBuilder = new StringBuilder(1024);
            }
        }
    }
}