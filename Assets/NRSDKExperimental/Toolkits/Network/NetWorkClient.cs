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

    /// <summary> A net work client. </summary>
    public class NetWorkClient : IDisposable
    {
        /// <summary> Event queue for all listeners interested in OnJoinRoomResult events. </summary>
        public event Action<bool> OnJoinRoomResult;
        /// <summary> Event queue for all listeners interested in OnCameraParamUpdate events. </summary>
        public event Action<CameraParam> OnCameraParamUpdate;
        /// <summary> Event queue for all listeners interested in OnMessageResponse events. </summary>
        public event Action<byte[]> OnMessageResponse;
        /// <summary> Event queue for all listeners interested in OnDisconnect events. </summary>
        public event Action OnDisconnect;
        /// <summary> Event queue for all listeners interested in OnConnect events. </summary>
        public event Action OnConnect;


        /// <summary> Default constructor. </summary>
        public NetWorkClient()
        {
            NetworkSession.Register(MessageType.Connected, OnConnected);
            NetworkSession.Register(MessageType.Disconnect, OnDisConnected);
            NetworkSession.Register(MessageType.HeartBeat, HeartbeatResponse);
            NetworkSession.Register(MessageType.EnterRoom, EnterRoomResponse);
            NetworkSession.Register(MessageType.ExitRoom, ExitRoomResponse);
            NetworkSession.Register(MessageType.UpdateCameraParam, UpdateCameraParamResponse);
            NetworkSession.Register(MessageType.MessageSynchronization, MessageSynchronizationResponse);
        }

        /// <summary> Join the server's room. </summary>
        public void EnterRoomRequest()
        {
            NetworkSession.Enqueue(MessageType.EnterRoom);
        }

        /// <summary> Exit room request. </summary>
        public void ExitRoomRequest()
        {
            NetworkSession.Enqueue(MessageType.ExitRoom);
        }

        /// <summary> Updates the camera parameter request. </summary>
        public void UpdateCameraParamRequest()
        {
            NetworkSession.Enqueue(MessageType.UpdateCameraParam);
        }

        /// <summary> Join the server's room. </summary>
        public void SendMessage(byte[] data)
        {
            NetworkSession.Enqueue(MessageType.MessageSynchronization, data);
        }

        /// <summary> Connects. </summary>
        /// <param name="ip">   The IP.</param>
        /// <param name="port"> The port.</param>
        public void Connect(string ip, int port)
        {
            NetworkSession.Connect(ip, port);
        }

        /// <summary> Executes the 'connected' action. </summary>
        /// <param name="data"> The data.</param>
        private void OnConnected(byte[] data)
        {
            OnConnect?.Invoke();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources. </summary>
        public void Dispose()
        {
            NetworkSession.UnRegister(MessageType.Connected, OnConnected);
            NetworkSession.UnRegister(MessageType.Disconnect, OnDisConnected);
            NetworkSession.UnRegister(MessageType.HeartBeat, HeartbeatResponse);
            NetworkSession.UnRegister(MessageType.EnterRoom, EnterRoomResponse);
            NetworkSession.UnRegister(MessageType.ExitRoom, ExitRoomResponse);
            NetworkSession.UnRegister(MessageType.UpdateCameraParam, UpdateCameraParamResponse);
            NetworkSession.UnRegister(MessageType.MessageSynchronization, MessageSynchronizationResponse);
            NetworkSession.Close();
        }

        /// <summary> Executes the 'dis connected' action. </summary>
        /// <param name="data"> The data.</param>
        private void OnDisConnected(byte[] data)
        {
            OnDisconnect?.Invoke();
        }

        #region Net msg response
        /// <summary> Heartbeat response. </summary>
        /// <param name="data"> The data.</param>
        private void HeartbeatResponse(byte[] data)
        {
            NetworkSession.Received = true;
            NRDebugger.Debug("Receive a heart beat package.");
        }

        /// <summary> Enter room response. </summary>
        /// <param name="data"> The data.</param>
        private void EnterRoomResponse(byte[] data)
        {
            EnterRoomData result = SerializerFactory.Create().Deserialize<EnterRoomData>(data);

            if (result.result)
            {
                NRDebugger.Debug("Join the room success.");
                OnJoinRoomResult?.Invoke(true);
            }
            else
            {
                NRDebugger.Warning("Join the room faild.");
                OnJoinRoomResult?.Invoke(false);
            }
        }

        /// <summary> Exit room response. </summary>
        /// <param name="data"> The data.</param>
        private void ExitRoomResponse(byte[] data)
        {
            ExitRoomData result = SerializerFactory.Create().Deserialize<ExitRoomData>(data);
            if (result.Suc)
            {
                NRDebugger.Debug("Exit the room success.");
            }
            else
            {
                NRDebugger.Warning("Exit the room faild.");
            }
        }

        /// <summary> Updates the camera parameter response described by data. </summary>
        /// <param name="data"> The data.</param>
        private void UpdateCameraParamResponse(byte[] data)
        {
            CameraParam result = SerializerFactory.Create().Deserialize<CameraParam>(data);
            OnCameraParamUpdate?.Invoke(result);
            NRDebugger.Info(result.fov.ToString());
        }

        private void MessageSynchronizationResponse(byte[] data)
        {
            OnMessageResponse?.Invoke(data);
        }
        #endregion
    }
}