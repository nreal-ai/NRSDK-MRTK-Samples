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
    public interface ILifecycle
    {
        /// <summary>
        /// Starts an instance of a object.
        /// </summary>
        void Start();

        /// <summary>
        /// Pause an instance of a object.
        /// </summary>
        void Pause();

        /// <summary>
        /// Resume an instance of a object.
        /// </summary>
        void Resume();

        /// <summary>
        /// Stops this instance of a object.
        /// </summary>
        void Stop();
    }

    public interface ISubsystem : ILifecycle
    {
        /// <summary>
        //  Will be true if asking the subsystem to start was successful. False in the case
        //  that the subsystem has stopped, was asked to stop or has not been started yet.
        /// </summary>
        bool running { get; }
    }

    public interface ISubsystemDescriptor
    {
        /// <summary>
        /// A unique string that identifies the subsystem that this Descriptor can create.
        /// </summary>
        string id { get; }

        /// <summary>
        /// Creates an ISubsystem from this descriptor.
        /// </summary>
        /// <returns>An instance of ISubsystem. </returns>
        ISubsystem Create();
    }
}
