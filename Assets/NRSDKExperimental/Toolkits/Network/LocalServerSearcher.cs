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
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using UnityEngine;

    public class LocalServerSearcher : SingleTon<LocalServerSearcher>
    {
        public struct ServerInfoResult
        {
            public bool isSuccess;
            public IPEndPoint endPoint;
        }
        public delegate void OnGetSearchResult(ServerInfoResult result);
        private UdpClient client;
        private IPEndPoint endpoint;
        Thread m_ReceiveThread = null;
        private const string SEARCHSERVERIP = "FIND-SERVER";
        private const int BroadCastPort = 6001;
        private static float TimeoutWaittingTime = 3f;
        private IPEndPoint m_LocalServer;
        private Queue<OnGetSearchResult> m_Tasks = new Queue<OnGetSearchResult>();
        private Coroutine m_TimeOutCoroutine = null;

        public LocalServerSearcher()
        {
            client = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
            endpoint = new IPEndPoint(IPAddress.Parse("255.255.255.255"), BroadCastPort);
            MainThreadDispather.Initialize();
        }

        public void Search(OnGetSearchResult callback)
        {
            lock (m_Tasks)
            {
                m_Tasks.Enqueue(callback);
            }

            if (m_ReceiveThread == null)
            {
                m_ReceiveThread = new Thread(new ThreadStart(RecvThread));
                m_ReceiveThread.IsBackground = true;
                m_ReceiveThread.Start();
            }

            RequestForServerIP();
            if (m_TimeOutCoroutine != null)
            {
                NRKernalUpdater.Instance.StopCoroutine(m_TimeOutCoroutine);
                m_TimeOutCoroutine = null;
            }
            m_TimeOutCoroutine = NRKernalUpdater.Instance.StartCoroutine(TimeOut());
        }

        private void RequestForServerIP()
        {
            byte[] buf = Encoding.Default.GetBytes(SEARCHSERVERIP);
            client.Send(buf, buf.Length, endpoint);
        }

        private void RecvThread()
        {
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, BroadCastPort);
            while (true)
            {
                try
                {
                    byte[] buf = client.Receive(ref endpoint);
                    string data = Encoding.Default.GetString(buf);
                    NRDebugger.Info("[LocalServerSearcher] Get the server info:" + data);
                    if (!string.IsNullOrEmpty(data))
                    {
                        string[] param = data.Split(':');
                        if (param != null && param.Length == 2)
                        {
                            m_LocalServer = new IPEndPoint(IPAddress.Parse(param[0]), int.Parse(param[1]));
                            Response(m_LocalServer);
                        }
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        private IEnumerator TimeOut()
        {
            float time_last = 0f;
            while (true)
            {
                yield return new WaitForEndOfFrame();
                time_last += Time.deltaTime;
                if (time_last > TimeoutWaittingTime)
                {
                    Response(null);
                }
            }
        }

        private void Response(IPEndPoint endpoint)
        {
            if (m_Tasks.Count == 0)
            {
                return;
            }

            MainThreadDispather.QueueOnMainThread(() =>
            {
                ServerInfoResult result = new ServerInfoResult();
                result.endPoint = endpoint;
                result.isSuccess = endpoint != null;

                lock (m_Tasks)
                {
                    if (m_Tasks.Count == 0)
                    {
                        return;
                    }
                    var callback = m_Tasks.Dequeue();
                    while (callback != null)
                    {
                        try
                        {
                            callback?.Invoke(result);
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                        if (m_Tasks.Count == 0)
                        {
                            return;
                        }
                        callback = m_Tasks.Dequeue();
                    }
                }
            });
        }
    }
}