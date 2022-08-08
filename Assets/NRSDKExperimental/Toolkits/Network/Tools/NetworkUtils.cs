/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.Experimental.NetWork
{
    using System.Net;
    using System.Net.Sockets;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;

    /// <summary> A network utilities. </summary>
    public static class NetworkUtils
    {
        /// <summary> Get local ipv4, return null if faild. </summary>
        /// <returns> The local IPv4. </returns>
        public static string GetLocalIPv4()
        {
            string hostName = Dns.GetHostName(); //得到主机名
            IPHostEntry iPEntry = Dns.GetHostEntry(hostName);
            for (int i = 0; i < iPEntry.AddressList.Length; i++)
            {
                //从IP地址列表中筛选出IPv4类型的IP地址
                if (iPEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    return iPEntry.AddressList[i].ToString();
            }
            return null;
        }

        /// <summary> Byte 2 string. </summary>
        /// <param name="bytes"> The bytes.</param>
        /// <returns> A string. </returns>
        public static string Byte2String(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary> String 2 byte. </summary>
        /// <param name="str"> The string.</param>
        /// <returns> A byte[]. </returns>
        public static byte[] String2Byte(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }
    }

    /// <summary> Interface for serializer. </summary>
    public interface ISerializer
    {
        /// <summary> Serialize this object to the given stream. </summary>
        /// <param name="obj"> The object.</param>
        /// <returns> A byte[]. </returns>
        byte[] Serialize(object obj);

        /// <summary> Deserialize this object to the given stream. </summary>
        /// <typeparam name="T"> Generic type parameter.</typeparam>
        /// <param name="data"> The data.</param>
        /// <returns> A T. </returns>
        T Deserialize<T>(byte[] data) where T : class;
    }

    /// <summary> An object for persisting JSON data. </summary>
    public class JsonSerializer : ISerializer
    {
        /// <summary> Deserialize this object to the given stream. </summary>
        /// <typeparam name="T"> Generic type parameter.</typeparam>
        /// <param name="data"> The data.</param>
        /// <returns> A T. </returns>
        public T Deserialize<T>(byte[] data) where T : class
        {
            return LitJson.JsonMapper.ToObject<T>(Encoding.UTF8.GetString(data));
        }

        /// <summary> Serialize this object to the given stream. </summary>
        /// <param name="obj"> The object.</param>
        /// <returns> A byte[]. </returns>
        public byte[] Serialize(object obj)
        {
            return Encoding.UTF8.GetBytes(LitJson.JsonMapper.ToJson(obj));
        }
    }

    /// <summary> An object for persisting binary data. </summary>
    public class BinarySerializer : ISerializer
    {
        /// <summary> obj -> bytes,  return null if obj not mark as [Serializable]. </summary>
        /// <param name="obj"> The object.</param>
        /// <returns> A byte[]. </returns>
        public byte[] Serialize(object obj)
        {
            //物体不为空且可被序列化
            if (obj == null || !obj.GetType().IsSerializable)
                return null;
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, obj);
                byte[] data = stream.ToArray();
                return data;
            }
        }

        /// <summary> bytes -> obj, return null if obj not mark as [Serializable]. </summary>
        /// <typeparam name="T"> Generic type parameter.</typeparam>
        /// <param name="data"> The data.</param>
        /// <returns> A T. </returns>
        public T Deserialize<T>(byte[] data) where T : class
        {
            //数据不为空且T是可序列化的类型
            if (data == null || !typeof(T).IsSerializable)
                return null;
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream(data))
            {
                object obj = formatter.Deserialize(stream);
                return obj as T;
            }
        }
    }

    /// <summary> A serializer factory. </summary>
    public static class SerializerFactory
    {
        /// <summary> The serializer. </summary>
        private static ISerializer _Serializer;

        /// <summary> Creates a new ISerializer. </summary>
        /// <returns> An ISerializer. </returns>
        public static ISerializer Create()
        {
            if (_Serializer == null)
            {
                _Serializer = new JsonSerializer();
            }
            return _Serializer;
        }
    }
}