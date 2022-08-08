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
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using UnityEngine;

    /// <summary> Call back. </summary>
    /// <param name="data"> The data.</param>
    public delegate void CallBack(byte[] data);

    /// <summary> A network session. </summary>
    public static class NetworkSession
    {
        /// <summary> Clent connect state. </summary>
        private enum ClientState
        {
            /// <summary> Disconnect. </summary>
            None,
            /// <summary> Connect server success. </summary>
            Connected,
        }

        /// <summary> Message type dictionary. </summary>
        private static Dictionary<MessageType, CallBack> m_CallBacks = new Dictionary<MessageType, CallBack>();
        /// <summary> Message queue. </summary>
        private static Queue<byte[]> m_Messages;
        /// <summary> Client current state. </summary>
        private static ClientState m_CurState;
        /// <summary> Gets or sets the current state. </summary>
        /// <value> The current state. </value>
        private static ClientState CurState
        {
            get
            {
                return m_CurState;
            }
            set
            {
                m_CurState = value;
                if (m_CurState == ClientState.Connected)
                {
                    CallBack callback;
                    if (m_CallBacks.TryGetValue(MessageType.Connected, out callback))
                    {
                        callback?.Invoke(null);
                    }
                }
                else
                {
                    CallBack callback;
                    if (m_CallBacks.TryGetValue(MessageType.Disconnect, out callback))
                    {
                        callback?.Invoke(null);
                    }
                }
            }
        }
        /// <summary> The client. </summary>
        private static TcpClient m_Client;
        /// <summary> The stream. </summary>
        private static NetworkStream m_Stream;

        /// <summary> Target address. </summary>
        private static IPAddress m_Address;
        /// <summary> The port. </summary>
        private static int m_Port;

        /// <summary> Heart beat time stamp. </summary>
        private const float HEARTBEAT_TIME = 1;
        /// <summary> Time from last heart beat package. </summary>
        private static float m_Timer = HEARTBEAT_TIME;
        /// <summary> Get heart beat msg from server. </summary>
        public static bool Received = true;

        #region coroutines
        /// <summary> Connects the coroutine. </summary>
        /// <returns> An IEnumerator. </returns>
        private static IEnumerator ConnectCoroutine()
        {
            m_Client = new TcpClient();

            IAsyncResult async = m_Client.BeginConnect(m_Address, m_Port, null, null);
            while (!async.IsCompleted)
            {
                NRDebugger.Info("Contecting server...");
                yield return null;
            }
            try
            {
                m_Client.EndConnect(async);
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Conncet server faild :" + ex.Message);
                yield break;
            }

            // Get data stream
            try
            {
                m_Stream = m_Client.GetStream();
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Connect server faild:" + ex.Message);
                yield break;
            }
            if (m_Stream == null)
            {
                Debug.LogWarning("Connect server faild: data stream is empty");
                yield break;
            }

            CurState = ClientState.Connected;
            m_Messages = new Queue<byte[]>();
            NRDebugger.Info("Connect server success.");

            // Set asyn msg send
            NetworkCoroutine.Instance.StartCoroutine(HeartBeat());
            // Set asyn msg receive
            NetworkCoroutine.Instance.StartCoroutine(ReceiveCoroutine());
            // Set quit event
            NetworkCoroutine.Instance.SetQuitEvent(() => { m_Client.Close(); CurState = ClientState.None; });
        }

        /// <summary> Heart beat. </summary>
        /// <returns> An IEnumerator. </returns>
        private static IEnumerator HeartBeat()
        {
            while (CurState == ClientState.Connected)
            {
                m_Timer += Time.deltaTime;
                if (m_Messages.Count > 0)
                {
                    byte[] data = m_Messages.Dequeue();
                    yield return WriteCoroutine(data);
                }

                // Heart beat strategy
                if (m_Timer >= HEARTBEAT_TIME)
                {
                    // if dont receive last heart beat package.
                    if (!Received)
                    {
                        CurState = ClientState.None;
                        Debug.LogWarning("Heart beat error. disconnect server.");
                        yield break;
                    }
                    m_Timer = 0;
                    byte[] data = Pack(MessageType.HeartBeat);
                    yield return WriteCoroutine(data);

                    NRDebugger.Debug("Send a heart beat package.");
                }
                yield return null;
            }
        }

        /// <summary> Receive coroutine. </summary>
        /// <returns> An IEnumerator. </returns>
        private static IEnumerator ReceiveCoroutine()
        {
            while (CurState == ClientState.Connected)
            {
                byte[] data = new byte[4];

                int length;         // msg len
                MessageType type;   // msg type
                int receive = 0;    // receive len

                IAsyncResult async = m_Stream.BeginRead(data, 0, data.Length, null, null);
                while (!async.IsCompleted)
                {
                    yield return null;
                }
                try
                {
                    receive = m_Stream.EndRead(async);
                }
                catch (Exception ex)
                {
                    CurState = ClientState.None;
                    Debug.LogWarning("Receive msg package head erro:" + ex.Message);
                    yield break;
                }
                if (receive < data.Length)
                {
                    CurState = ClientState.None;
                    Debug.LogWarning("Receive msg package head erro");
                    yield break;
                }

                using (MemoryStream stream = new MemoryStream(data))
                {
                    BinaryReader binary = new BinaryReader(stream, Encoding.UTF8); // parase data using UTF-8
                    try
                    {
                        length = binary.ReadUInt16();
                        type = (MessageType)binary.ReadUInt16();
                    }
                    catch (Exception)
                    {
                        CurState = ClientState.None;
                        Debug.LogWarning("Receive msg package head erro");
                        yield break;
                    }
                }

                if (length - 4 > 0)
                {
                    data = new byte[length - 4];
                    async = m_Stream.BeginRead(data, 0, data.Length, null, null);
                    while (!async.IsCompleted)
                    {
                        yield return null;
                    }
                    try
                    {
                        receive = m_Stream.EndRead(async);
                    }
                    catch (Exception ex)
                    {
                        CurState = ClientState.None;
                        Debug.LogWarning("Receive msg package head erro:" + ex.Message);
                        yield break;
                    }
                    if (receive < data.Length)
                    {
                        CurState = ClientState.None;
                        Debug.LogWarning("Receive msg package head erro");
                        yield break;
                    }
                }
                else
                {
                    data = new byte[0];
                    receive = 0;
                }

                if (m_CallBacks.ContainsKey(type))
                {
                    CallBack method = m_CallBacks[type];
                    method(data);
                }
                else
                {
                    NRDebugger.Warning("Did not regist the msg callback : " + type);
                }
            }
        }

        /// <summary> Writes a coroutine. </summary>
        /// <param name="data"> The data.</param>
        /// <returns> An IEnumerator. </returns>
        private static IEnumerator WriteCoroutine(byte[] data)
        {
            if (CurState != ClientState.Connected || m_Stream == null)
            {
                Debug.LogWarning("Connect error, can not receive msg");
                yield break;
            }

            IAsyncResult async = m_Stream.BeginWrite(data, 0, data.Length, null, null);
            while (!async.IsCompleted)
            {
                yield return null;
            }
            try
            {
                m_Stream.EndWrite(async);
            }
            catch (Exception ex)
            {
                CurState = ClientState.None;
                Debug.LogWarning("Send msg erro:" + ex.Message);
            }
        }
        #endregion

        /// <summary> Connect server. </summary>
        /// <param name="address"> (Optional) The address.</param>
        /// <param name="port">    (Optional) The port.</param>
        public static void Connect(string address = null, int port = 8848)
        {
            // Can not connect again after connected.
            if (CurState == ClientState.Connected)
            {
                NRDebugger.Info("Has connected server.");
                return;
            }
            if (address == null)
                address = NetworkUtils.GetLocalIPv4();

            // Cancle when get the ipaddress failed.
            if (!IPAddress.TryParse(address, out m_Address))
            {
                Debug.LogWarning("IP erro, try again.");
                return;
            }

            m_Port = port;
            // Connect service.
            NetworkCoroutine.Instance.StartCoroutine(ConnectCoroutine());
        }

        /// <summary> Closes this object. </summary>
        public static void Close()
        {
            if (CurState == ClientState.Connected)
            {
                m_Client.Close();
                m_CurState = ClientState.None;
            }
            NetworkCoroutine.Instance.StopAllCoroutines();
        }

        /// <summary> Regist callback event. </summary>
        /// <param name="type">   The type.</param>
        /// <param name="method"> The method.</param>
        public static void Register(MessageType type, CallBack method)
        {
            if (!m_CallBacks.ContainsKey(type))
                m_CallBacks.Add(type, method);
            else
                Debug.LogWarning("Regist the same msg type.");
        }

        /// <summary> Un register. </summary>
        /// <param name="type">   The type.</param>
        /// <param name="method"> The method.</param>
        public static void UnRegister(MessageType type, CallBack method)
        {
            if (m_CallBacks.ContainsKey(type))
            {
                m_CallBacks.Remove(type);
            }
        }

        /// <summary> Join the msg queue. </summary>
        /// <param name="type"> The type.</param>
        /// <param name="data"> (Optional) The data.</param>
        public static void Enqueue(MessageType type, byte[] data = null)
        {
            // Pack the data
            byte[] bytes = Pack(type, data);

            if (CurState == ClientState.Connected)
            {
                m_Messages.Enqueue(bytes);
            }
        }

        /// <summary> Pack the byte data. </summary>
        /// <param name="type"> The type.</param>
        /// <param name="data"> (Optional) The data.</param>
        /// <returns> A byte[]. </returns>
        private static byte[] Pack(MessageType type, byte[] data = null)
        {
            MessagePacker packer = new MessagePacker();
            if (data != null)
            {
                packer.Add((ushort)(4 + data.Length)); // msg length
                packer.Add((ushort)type);              // msg type
                packer.Add(data);                      // msg content
            }
            else
            {
                packer.Add(4);
                packer.Add((ushort)type);
            }
            return packer.Package;
        }
    }
}