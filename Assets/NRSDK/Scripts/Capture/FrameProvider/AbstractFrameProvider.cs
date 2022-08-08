/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.Record
{
    using UnityEngine;

    /// <summary> An abstract frame provider. </summary>
    public abstract class AbstractFrameProvider
    {
        /// <summary> Updates the image frame described by frame. </summary>
        /// <param name="frame"> The frame.</param>
        public delegate void UpdateImageFrame(UniversalTextureFrame frame);
        /// <summary> The on update. </summary>
        public UpdateImageFrame OnUpdate;
        /// <summary> True if is frame ready, false if not. </summary>
        protected bool m_IsFrameReady = false;

        /// <summary> Gets frame information. </summary>
        /// <returns> The frame information. </returns>
        public virtual Resolution GetFrameInfo() { return new Resolution(); }

        /// <summary> Query if this object is frame ready. </summary>
        /// <returns> True if frame ready, false if not. </returns>
        public virtual bool IsFrameReady() { return m_IsFrameReady; }

        /// <summary> Plays this object. </summary>
        public virtual void Play() { }

        /// <summary> Stops this object. </summary>
        public virtual void Stop() { }

        /// <summary> Releases this object. </summary>
        public virtual void Release() { }
    }
}
