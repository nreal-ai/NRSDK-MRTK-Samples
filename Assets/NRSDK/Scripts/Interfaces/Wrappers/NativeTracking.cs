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
    using System.Runtime.InteropServices;

    /// <summary> Native Tracking API. </summary>
    public partial class NativeTracking
    {
        /// <summary> The native interface. </summary>
        private NativeInterface m_NativeInterface;
        /// <summary> Handle of the tracking. </summary>
        private UInt64 m_TrackingHandle;

        /// <summary> Constructor. </summary>
        /// <param name="nativeInterface"> The native interface.</param>
        public NativeTracking(NativeInterface nativeInterface)
        {
            m_NativeInterface = nativeInterface;
        }

        /// <summary> Creates a new bool. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Create()
        {
            NativeResult result = NativeApi.NRTrackingCreate(ref m_TrackingHandle);
            NativeErrorListener.Check(result, this, "Create");
            m_NativeInterface.TrackingHandle = m_TrackingHandle;
            return result == NativeResult.Success;
        }

        /// <summary> Inits tracking mode. </summary>
        /// <param name="mode"> The mode.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool InitTrackingMode(TrackingMode mode)
        {
            if (m_TrackingHandle == 0)
            {
                return false;
            }
            NativeResult result = NativeApi.NRTrackingInitSetTrackingMode(m_TrackingHandle, mode);
            NativeErrorListener.Check(result, this, "InitTrackingMode");
            return result == NativeResult.Success;
        }

        /// <summary> Starts this object. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Start()
        {
            if (m_TrackingHandle == 0)
            {
                return false;
            }
            NativeResult result = NativeApi.NRTrackingStart(m_TrackingHandle);
            NativeErrorListener.Check(result, this, "Start");
            return result == NativeResult.Success;
        }

        /// <summary> Switch tracking mode. </summary>
        /// <param name="mode"> The mode.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool SwitchTrackingMode(TrackingMode mode)
        {
            NativeResult result = NativeApi.NRTrackingSetTrackingMode(m_TrackingHandle, mode);
            NativeErrorListener.Check(result, this, "SwitchTrackingMode");
            return result == NativeResult.Success;
        }

        /// <summary> Pauses this object. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Pause()
        {
            if (m_TrackingHandle == 0)
            {
                return false;
            }
            NativeResult result = NativeApi.NRTrackingPause(m_TrackingHandle);
            NativeErrorListener.Check(result, this, "Pause");
            return result == NativeResult.Success;
        }

        /// <summary> Resumes this object. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Resume()
        {
            if (m_TrackingHandle == 0)
            {
                return false;
            }
            NativeResult result = NativeApi.NRTrackingResume(m_TrackingHandle);
            NativeErrorListener.Check(result, this, "Resume");
            return result == NativeResult.Success;
        }

        /// <summary> only worked at 3dof mode. </summary>
        public void Recenter()
        {
            if (m_TrackingHandle == 0)
            {
                return;
            }
            var result = NativeApi.NRTrackingRecenter(m_TrackingHandle);
            NativeErrorListener.Check(result, this, "Recenter");
        }

        /// <summary> Destroys this object. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Destroy()
        {
            if (m_TrackingHandle == 0)
            {
                return false;
            }
            NativeResult result = NativeApi.NRTrackingDestroy(m_TrackingHandle);
            NativeErrorListener.Check(result, this, "Destroy");
            m_TrackingHandle = 0;
            m_NativeInterface.TrackingHandle = m_TrackingHandle;
            return result == NativeResult.Success;
        }

        private partial struct NativeApi
        {
            /// <summary> Nr tracking create. </summary>
            /// <param name="out_tracking_handle"> [in,out] Handle of the out tracking.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackingCreate(ref UInt64 out_tracking_handle);

            /// <summary> Nr tracking initialize set tracking mode. </summary>
            /// <param name="tracking_handle"> Handle of the tracking.</param>
            /// <param name="tracking_mode">   The tracking mode.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackingInitSetTrackingMode(UInt64 tracking_handle, TrackingMode tracking_mode);

            /// <summary> Nr tracking start. </summary>
            /// <param name="tracking_handle"> Handle of the tracking.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackingStart(UInt64 tracking_handle);

            /// <summary> Nr tracking set tracking mode. </summary>
            /// <param name="tracking_handle"> Handle of the tracking.</param>
            /// <param name="tracking_mode">   The tracking mode.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackingSetTrackingMode(UInt64 tracking_handle, TrackingMode tracking_mode);

            /// <summary> Nr tracking destroy. </summary>
            /// <param name="tracking_handle"> Handle of the tracking.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackingDestroy(UInt64 tracking_handle);

            /// <summary> Nr tracking pause. </summary>
            /// <param name="tracking_handle"> Handle of the tracking.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackingPause(UInt64 tracking_handle);

            /// <summary> Nr tracking resume. </summary>
            /// <param name="tracking_handle"> Handle of the tracking.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackingResume(UInt64 tracking_handle);

            /// <summary> Nr tracking recenter. </summary>
            /// <param name="tracking_handle"> Handle of the tracking.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackingRecenter(UInt64 tracking_handle);
        };
    }
}
