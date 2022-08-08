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

    /// <summary> Net msg type. </summary>
    public enum MessageType
    {
        /// <summary> Empty type. </summary>
        None = 0,

        /// <summary> Connect server. </summary>
        Connected = 1,
        /// <summary> Disconnect from server. </summary>
        Disconnect = 2,

        /// <summary> Heart beat. </summary>
        HeartBeat = 3,
        /// <summary> Enter room. </summary>
        EnterRoom = 4,
        /// <summary> Enter room. </summary>
        ExitRoom = 5,

        /// <summary> An enum constant representing the update camera Parameter option. </summary>
        UpdateCameraParam = 6,

        /// <summary> Used to synchronization message with the server. </summary>
        MessageSynchronization = 7,
    }

    /// <summary> (Serializable) an enter room data. </summary>
    [Serializable]
    public class EnterRoomData
    {
        /// <summary> Enter room result. </summary>
        public bool result;
    }

    /// <summary> (Serializable) an exit room data. </summary>
    [Serializable]
    public class ExitRoomData
    {
        /// <summary> Exit room result. </summary>
        public bool Suc;
    }

    /// <summary> (Serializable) a camera parameter. </summary>
    [Serializable]
    public class CameraParam
    {
        /// <summary> Camera fov. </summary>
        public Fov4f fov;
    }
}
