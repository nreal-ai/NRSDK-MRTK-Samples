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
    public class IntegratedSubsystem : ISubsystem
    {
        protected ISubsystemDescriptor m_Descripe;

        public IntegratedSubsystem(ISubsystemDescriptor descripe)
        {
            m_Descripe = descripe;
        }

        /// <summary>
        /// Whether or not the subsystem is running.
        /// </summary>
        public virtual bool running { get; protected set; } = false;

        /// <summary>
        /// Starts an instance of a subsystem.
        /// </summary>
        public virtual void Start()
        {
            running = true;
        }

        /// <summary>
        /// Pause an instance of a subsystem.
        /// </summary>
        public virtual void Pause()
        {
            running = false;
        }

        /// <summary>
        /// Resume an instance of a subsystem.
        /// </summary>
        public virtual void Resume()
        {
            running = true;
        }

        /// <summary>
        /// Destroys this instance of a subsystem.
        /// </summary>
        public virtual void Stop()
        {
            running = false;
        }
    }

    public class IntegratedSubsystem<TSubsystemDescriptor> : IntegratedSubsystem where TSubsystemDescriptor : ISubsystemDescriptor
    {
        public IntegratedSubsystem(TSubsystemDescriptor descripe) : base(descripe) { }

        public virtual TSubsystemDescriptor SubsystemDescriptor => (TSubsystemDescriptor)m_Descripe;
    }
}
