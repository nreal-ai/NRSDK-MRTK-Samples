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
    using System.Collections.Generic;

    /// <summary> A frame capture context factory. </summary>
    public class FrameCaptureContextFactory
    {
        /// <summary> List of contexts. </summary>
        private static List<FrameCaptureContext> m_ContextList = new List<FrameCaptureContext>();

        /// <summary> Creates a new FrameCaptureContext. </summary>
        /// <returns> A FrameCaptureContext. </returns>
        public static FrameCaptureContext Create()
        {
            FrameCaptureContext context = new FrameCaptureContext();

            m_ContextList.Add(context);
            return context;
        }

        /// <summary> Dispose all context. </summary>
        public static void DisposeAllContext()
        {
            foreach (var item in m_ContextList)
            {
                if (item != null)
                {
                    item.StopCapture();
                    item.Release();
                }
            }
        }
    }
}
