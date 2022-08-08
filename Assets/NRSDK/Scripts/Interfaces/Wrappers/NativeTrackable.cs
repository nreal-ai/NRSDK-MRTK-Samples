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
    using System.Runtime.InteropServices;

    /// <summary> 6-dof Trackable's Native API . </summary>
    public partial class NativeTrackable
    {
        /// <summary> The native interface. </summary>
        private NativeInterface m_NativeInterface;

        /// <summary> Constructor. </summary>
        /// <param name="nativeInterface"> The native interface.</param>
        public NativeTrackable(NativeInterface nativeInterface)
        {
            m_NativeInterface = nativeInterface;
        }

        public bool UpdateTrackables(TrackableType trackable_type, List<UInt64> trackables)
        {
            if (m_NativeInterface == null || m_NativeInterface.TrackingHandle == 0)
            {
                return false;
            }

            trackables.Clear();
            UInt64 trackable_list_handle = 0;
            var create_result = NativeApi.NRTrackableListCreate(m_NativeInterface.TrackingHandle, ref trackable_list_handle);
            var update_result = NativeApi.NRTrackingUpdateTrackables(m_NativeInterface.TrackingHandle, trackable_type, trackable_list_handle);
            int list_size = 0;
            var getsize_result = NativeApi.NRTrackableListGetSize(m_NativeInterface.TrackingHandle, trackable_list_handle, ref list_size);
            for (int i = 0; i < list_size; i++)
            {
                UInt64 trackable_handle = 0;
                var acquireitem_result = NativeApi.NRTrackableListAcquireItem(m_NativeInterface.TrackingHandle, trackable_list_handle, i, ref trackable_handle);
                if (acquireitem_result == NativeResult.Success) trackables.Add(trackable_handle);
            }
            var destroy_result = NativeApi.NRTrackableListDestroy(m_NativeInterface.TrackingHandle, trackable_list_handle);
            return (create_result == NativeResult.Success) && (update_result == NativeResult.Success)
                && (getsize_result == NativeResult.Success) && (destroy_result == NativeResult.Success);
        }

        /// <summary> Gets an identify. </summary>
        /// <param name="trackable_handle"> Handle of the trackable.</param>
        /// <returns> The identify. </returns>
        public UInt32 GetIdentify(UInt64 trackable_handle)
        {
            if (m_NativeInterface.TrackingHandle == 0)
            {
                return 0;
            }
            UInt32 identify = NativeConstants.IllegalInt;
            NativeApi.NRTrackableGetIdentifier(m_NativeInterface.TrackingHandle, trackable_handle, ref identify);
            return identify;
        }

        /// <summary> Gets trackable type. </summary>
        /// <param name="trackable_handle"> Handle of the trackable.</param>
        /// <returns> The trackable type. </returns>
        public TrackableType GetTrackableType(UInt64 trackable_handle)
        {
            if (m_NativeInterface.TrackingHandle == 0)
            {
                return TrackableType.TRACKABLE_BASE;
            }
            TrackableType trackble_type = TrackableType.TRACKABLE_BASE;
            NativeApi.NRTrackableGetType(m_NativeInterface.TrackingHandle, trackable_handle, ref trackble_type);
            return trackble_type;
        }

        /// <summary> Gets tracking state. </summary>
        /// <param name="trackable_handle"> Handle of the trackable.</param>
        /// <returns> The tracking state. </returns>
        public TrackingState GetTrackingState(UInt64 trackable_handle)
        {
            if (m_NativeInterface.TrackingHandle == 0)
            {
                return TrackingState.Stopped;
            }
            TrackingState status = TrackingState.Stopped;
            NativeApi.NRTrackableGetTrackingState(m_NativeInterface.TrackingHandle, trackable_handle, ref status);
            return status;
        }

        private partial struct NativeApi
        {
            /// <summary> Nr trackable list create. </summary>
            /// <param name="session_handle">            Handle of the session.</param>
            /// <param name="out_trackable_list_handle"> [in,out] Handle of the out trackable list.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackableListCreate(UInt64 session_handle,
                ref UInt64 out_trackable_list_handle);

            /// <summary> Nr trackable list destroy. </summary>
            /// <param name="session_handle">            Handle of the session.</param>
            /// <param name="trackable_list_handle"> Handle of the out trackable list.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackableListDestroy(UInt64 session_handle,
                UInt64 trackable_list_handle);

            /// <summary> Nr tracking update trackables. </summary>
            /// <param name="tracking_handle">           Handle of the tracking.</param>
            /// <param name="trackable_type">            Type of the trackable.</param>
            /// <param name="trackable_list_handle"> Handle of the out trackable list.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackingUpdateTrackables(UInt64 tracking_handle,
               TrackableType trackable_type, UInt64 trackable_list_handle);

            /// <summary> Nr trackable list get size. </summary>
            /// <param name="session_handle">        Handle of the session.</param>
            /// <param name="trackable_list_handle"> Handle of the trackable list.</param>
            /// <param name="out_list_size">         [in,out] Size of the out list.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackableListGetSize(UInt64 session_handle,
                UInt64 trackable_list_handle, ref int out_list_size);

            /// <summary> Nr trackable list acquire item. </summary>
            /// <param name="session_handle">        Handle of the session.</param>
            /// <param name="trackable_list_handle"> Handle of the trackable list.</param>
            /// <param name="index">                 Zero-based index of the.</param>
            /// <param name="out_trackable_item_handle">         [in,out] The out trackable.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackableListAcquireItem(UInt64 session_handle,
                UInt64 trackable_list_handle, int index, ref UInt64 out_trackable_item_handle);

            /// <summary> Nr trackable get identifier. </summary>
            /// <param name="session_handle">   Handle of the session.</param>
            /// <param name="trackable_handle"> Handle of the trackable.</param>
            /// <param name="out_identifier">   [in,out] Identifier for the out.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackableGetIdentifier(UInt64 session_handle,
                UInt64 trackable_handle, ref UInt32 out_identifier);

            /// <summary> Nr trackable get type. </summary>
            /// <param name="session_handle">     Handle of the session.</param>
            /// <param name="trackable_handle">   Handle of the trackable.</param>
            /// <param name="out_trackable_type"> [in,out] Type of the out trackable.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackableGetType(UInt64 session_handle,
                UInt64 trackable_handle, ref TrackableType out_trackable_type);

            /// <summary> Nr trackable get tracking state. </summary>
            /// <param name="session_handle">     Handle of the session.</param>
            /// <param name="trackable_handle">   Handle of the trackable.</param>
            /// <param name="out_tracking_state"> [in,out] State of the out tracking.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackableGetTrackingState(UInt64 session_handle,
                UInt64 trackable_handle, ref TrackingState out_tracking_state);
        };
    }
}
