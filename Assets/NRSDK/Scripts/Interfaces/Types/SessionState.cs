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
    /// <summary> Device Session State. </summary>
    public enum SessionState
    {
        /// <summary>
        /// UnInitialize means the NRSDK has not been initialized.
        /// </summary>
        UnInitialized = 0,

        /// <summary>
        /// Initialized means the NRSDK has been initialized.
        /// </summary>
        Initialized,

        /// <summary>
        /// Running means the object is being tracked and its state is valid.
        /// </summary>
        Running,

        /// <summary>
        /// Paused means the object is being tracked and its state is paused.
        /// </summary>
        Paused,

        /// <summary>
        /// Destroyed means that NRSDK session has been destroyed.
        /// </summary>
        Destroyed
    }
}
