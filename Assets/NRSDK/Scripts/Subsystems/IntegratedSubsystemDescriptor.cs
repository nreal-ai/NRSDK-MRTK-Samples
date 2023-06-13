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
    using System.Collections.Generic;

    /// <summary>
    /// Information about a subsystem that can be queried before creating a subsystem instance.
    /// </summary>
    public class IntegratedSubsystemDescriptor : ISubsystemDescriptor
    {
        protected IntegratedSubsystemDescriptor() { }

        /// <summary>
        /// A unique string that identifies the subsystem that this Descriptor can create.
        /// </summary>
        public virtual string id { get; }

        public ISubsystem subsystem { get; protected set; }

        public virtual ISubsystem Create() => null;
    }

    public class IntegratedSubsystemDescriptor<TSubsystem> : IntegratedSubsystemDescriptor where TSubsystem : IntegratedSubsystem
    {
        protected static Dictionary<string, TSubsystem> m_SubsystemDict = new Dictionary<string, TSubsystem>();

        public IntegratedSubsystemDescriptor() { }

        public virtual new TSubsystem Create()
        {
            if (!m_SubsystemDict.ContainsKey(id))
            {
                try
                {
                    subsystem = (TSubsystem)Activator.CreateInstance(typeof(TSubsystem), this);
                    m_SubsystemDict.Add(id, (TSubsystem)subsystem);
                }
                catch (Exception e)
                {
                    NRDebugger.Error("Get the instance of Class({0}) faild.", typeof(TSubsystem).FullName);
                    throw e;
                }
            }
            return m_SubsystemDict[id];
        }
    }
}
