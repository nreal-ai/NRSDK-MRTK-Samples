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
    using UnityEngine;
    using NRKernal;
    using System.Collections.Generic;
    using LitJson;
    using System.Text;

    /// <summary> An observer view net worker. </summary>
    public class NetWorkBehaviour
    {
        /// <summary> The net work client. </summary>
        protected NetWorkClient m_NetWorkClient;

        /// <summary> The limit waitting time. </summary>
        private const float limitWaittingTime = 5f;
        /// <summary> True if is connected, false if not. </summary>
        private bool m_IsConnected = false;
        /// <summary> True if is jonin success, false if not. </summary>
        private bool m_IsJoninSuccess = false;
        /// <summary> True if is closed, false if not. </summary>
        private bool m_IsClosed = false;
        private Coroutine checkServerAvailableCoroutine = null;
        private Dictionary<ulong, Action<JsonData>> _ResponseEvents = new Dictionary<ulong, Action<JsonData>>();

        public virtual void Listen()
        {
            if (m_NetWorkClient == null)
            {
                m_NetWorkClient = new NetWorkClient();
                m_NetWorkClient.OnDisconnect += OnDisconnect;
                m_NetWorkClient.OnConnect += OnConnected;
                m_NetWorkClient.OnJoinRoomResult += OnJoinRoomResult;
                m_NetWorkClient.OnMessageResponse += OnMessageResponse;
            }
        }

        private void OnMessageResponse(byte[] data)
        {
            ulong msgid = BitConverter.ToUInt64(data, 0);

            Action<JsonData> callback;
            if (!_ResponseEvents.TryGetValue(msgid, out callback))
            {
                NRDebugger.Warning("[NetWorkBehaviour] can not find the msgid bind event:" + msgid);
                return;
            }

            // Remove the header to get the msg.
            byte[] result = new byte[data.Length - sizeof(ulong)];
            Array.Copy(data, sizeof(ulong), result, 0, result.Length);
            string json = Encoding.UTF8.GetString(result);
            callback?.Invoke(JsonMapper.ToObject(json));
            NRDebugger.Info("[NetWorkBehaviour] OnMessageResponse hit...");
            _ResponseEvents.Remove(msgid);
        }

        /// <summary> Check server available. </summary>
        /// <param name="ip">       The IP.</param>
        /// <param name="callback"> The callback.</param>
        public void CheckServerAvailable(string ip, Action<bool> callback)
        {
            if (string.IsNullOrEmpty(ip))
            {
                callback?.Invoke(false);
            }
            else
            {
                if (checkServerAvailableCoroutine != null)
                {
                    NRKernalUpdater.Instance.StopCoroutine(checkServerAvailableCoroutine);
                }
                checkServerAvailableCoroutine = NRKernalUpdater.Instance.StartCoroutine(CheckServerAvailableCoroutine(ip, callback));
            }
        }

        /// <summary> Check server available coroutine. </summary>
        /// <param name="ip">       The IP.</param>
        /// <param name="callback"> The callback.</param>
        /// <returns> An IEnumerator. </returns>
        private IEnumerator CheckServerAvailableCoroutine(string ip, Action<bool> callback)
        {
            // Start to connect the server.
            m_NetWorkClient.Connect(ip, 6000);
            float timeLast = 0;
            while (!m_IsConnected)
            {
                if (timeLast > limitWaittingTime || m_IsClosed)
                {
                    NRDebugger.Info("[ObserverView] Connect the server TimeOut!");
                    callback?.Invoke(false);
                    yield break;
                }
                timeLast += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            // Start to enter the room.
            m_NetWorkClient.EnterRoomRequest();

            timeLast = 0;
            while (!m_IsJoninSuccess)
            {
                if (timeLast > limitWaittingTime || m_IsClosed)
                {
                    NRDebugger.Info("[ObserverView] Join the server TimeOut!");
                    callback?.Invoke(false);
                    yield break;
                }
                timeLast += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            callback?.Invoke(true);
        }

        public void SendMsg(JsonData data, Action<JsonData> onResponse, float timeout = 3)
        {
            NRKernalUpdater.Instance.StartCoroutine(SendMessage(data, onResponse, timeout));
        }

        private IEnumerator SendMessage(JsonData data, Action<JsonData> onResponse, float timeout)
        {
            if (data == null)
            {
                NRDebugger.Error("[NetWorkBehaviour] data is null!");
                yield break;
            }

            // Add msgid(current timestamp) as the header.
            ulong msgid = NRTools.GetTimeStamp();
            byte[] json_data = Encoding.UTF8.GetBytes(data.ToJson());
            byte[] total_data = new byte[json_data.Length + sizeof(ulong)];
            Array.Copy(BitConverter.GetBytes(msgid), 0, total_data, 0, sizeof(ulong));
            Array.Copy(json_data, 0, total_data, sizeof(ulong), json_data.Length);

            if (onResponse != null)
            {
                Action<JsonData> onResult;
                AsyncTask<JsonData> asyncTask = new AsyncTask<JsonData>(out onResult);
                _ResponseEvents[msgid] = onResult;
                m_NetWorkClient.SendMessage(total_data);

                NRKernalUpdater.Instance.StartCoroutine(SendMsgTimeOut(msgid, timeout));
                yield return asyncTask.WaitForCompletion();

                onResponse?.Invoke(asyncTask.Result);
            }
            else
            {
                m_NetWorkClient.SendMessage(total_data);
            }
        }

        private IEnumerator SendMsgTimeOut(UInt64 id, float timeout)
        {
            yield return new WaitForSeconds(timeout);

            Action<JsonData> callback;
            if (_ResponseEvents.TryGetValue(id, out callback))
            {
                NRDebugger.Warning("[NetWorkBehaviour] Send msg timeout, id:{0}", id);
                JsonData json = new JsonData();
                json["success"] = false;
                callback?.Invoke(json);
            }
        }

        #region Net msg
        /// <summary> Executes the 'connected' action. </summary>
        private void OnConnected()
        {
            NRDebugger.Info("[NetWorkBehaviour] OnConnected...");
            m_IsConnected = true;
        }

        /// <summary> Executes the 'disconnect' action. </summary>
        private void OnDisconnect()
        {
            NRDebugger.Info("[NetWorkBehaviour] OnDisconnect...");
            this.Close();
        }

        /// <summary> Executes the 'join room result' action. </summary>
        /// <param name="result"> True to result.</param>
        private void OnJoinRoomResult(bool result)
        {
            NRDebugger.Info("[NetWorkBehaviour] OnJoinRoomResult :" + result);
            m_IsJoninSuccess = result;
            if (!result)
            {
                this.Close();
            }
        }
        #endregion

        /// <summary> Closes this object. </summary>
        public virtual void Close()
        {
            if (checkServerAvailableCoroutine != null)
            {
                NRKernalUpdater.Instance.StopCoroutine(checkServerAvailableCoroutine);
            }
            m_NetWorkClient.ExitRoomRequest();
            m_NetWorkClient?.Dispose();
            m_NetWorkClient = null;
            checkServerAvailableCoroutine = null;
            m_IsClosed = true;
        }
    }
}