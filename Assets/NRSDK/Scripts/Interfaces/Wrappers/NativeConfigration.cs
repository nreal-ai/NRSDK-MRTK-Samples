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
    using System.Threading.Tasks;

    /// <summary> A native configration. </summary>
    public partial class NativeConfigration
    {
        /// <summary> The native interface. </summary>
        private NativeInterface m_NativeInterface;
        /// <summary> Dictionary of trackable image databases. </summary>
        private Dictionary<string, UInt64> m_TrackableImageDatabaseDict;

        /// <summary> Handle of the configuration. </summary>
        private UInt64 m_ConfigHandle = 0;
        /// <summary> Handle of the database. </summary>
        private UInt64 m_DatabaseHandle = 0;

        /// <summary> The last session configuration. </summary>
        private NRSessionConfig m_LastSessionConfig;
        /// <summary> The native trackable image. </summary>
        private NativeTrackableImage m_NativeTrackableImage;
        /// <summary> True if is update configuration lock, false if not. </summary>
        private bool m_IsUpdateConfigLock = false;

        /// <summary> Constructor. </summary>
        /// <param name="nativeInterface"> The native interface.</param>
        public NativeConfigration(NativeInterface nativeInterface)
        {
            m_NativeInterface = nativeInterface;
            m_LastSessionConfig = NRSessionConfig.CreateInstance(typeof(NRSessionConfig)) as NRSessionConfig;
            m_NativeTrackableImage = m_NativeInterface.NativeTrackableImage;
            m_TrackableImageDatabaseDict = new Dictionary<string, ulong>();
        }

        /// <summary> Updates the configuration described by config. </summary>
        /// <param name="config"> The configuration.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public async Task<bool> UpdateConfig(NRSessionConfig config)
        {
            if (m_IsUpdateConfigLock)
            {
                return false;
            }
            m_IsUpdateConfigLock = true;
            if (m_ConfigHandle == 0)
            {
                m_ConfigHandle = this.Create();
            }

            if (m_ConfigHandle == 0 || m_LastSessionConfig.Equals(config))
            {
                NRDebugger.Info("[NativeConfigration] Faild to Update NRSessionConfig!!!");
                m_IsUpdateConfigLock = false;
                return false;
            }

            await UpdatePlaneFindMode(config);
            await UpdateImageTrackingConfig(config);

            m_LastSessionConfig.CopyFrom(config);
            m_IsUpdateConfigLock = false;
            return true;
        }

        /// <summary> Updates the plane find mode described by config. </summary>
        /// <param name="config"> The configuration.</param>
        /// <returns> An asynchronous result. </returns>
        private Task UpdatePlaneFindMode(NRSessionConfig config)
        {
            return Task.Run(() =>
            {
                var currentmode = this.GetPlaneFindMode(m_ConfigHandle);
                if (currentmode != config.PlaneFindingMode)
                {
                    SetPlaneFindMode(m_ConfigHandle, config.PlaneFindingMode);
                }
            });
        }

        /// <summary> Updates the image tracking configuration described by config. </summary>
        /// <param name="config"> The configuration.</param>
        /// <returns> An asynchronous result. </returns>
        private Task UpdateImageTrackingConfig(NRSessionConfig config)
        {
            return Task.Run(() =>
            {
                switch (config.ImageTrackingMode)
                {
                    case TrackableImageFindingMode.DISABLE:
                        var result = SetTrackableImageDataBase(m_ConfigHandle, 0);
                        if (result)
                        {
                            m_TrackableImageDatabaseDict.Clear();
                        }
                        NRDebugger.Info("[NativeConfigration] Disable trackable image result : " + result);
                        break;
                    case TrackableImageFindingMode.ENABLE:
                        if (config.TrackingImageDatabase == null)
                        {
                            return;
                        }

                        if (!m_TrackableImageDatabaseDict.TryGetValue(config.TrackingImageDatabase.GUID, out m_DatabaseHandle))
                        {
                            DeployData(config.TrackingImageDatabase);
                            m_DatabaseHandle = m_NativeTrackableImage.CreateDataBase();
                            m_TrackableImageDatabaseDict.Add(config.TrackingImageDatabase.GUID, m_DatabaseHandle);
                        }
                        result = m_NativeTrackableImage.LoadDataBase(m_DatabaseHandle, config.TrackingImageDatabase.TrackingImageDataPath);
                        NRDebugger.Info("[NativeConfigration] LoadDataBase path:{0} result:{1} ", config.TrackingImageDatabase.TrackingImageDataPath, result);
                        result = SetTrackableImageDataBase(m_ConfigHandle, m_DatabaseHandle);
                        NRDebugger.Info("[NativeConfigration] SetTrackableImageDataBase result : " + result);
                        break;
                    default:
                        break;
                }
            });
        }

        /// <summary> Deploy data. </summary>
        /// <param name="database"> The database.</param>
        private void DeployData(NRTrackingImageDatabase database)
        {
            string deploy_path = database.TrackingImageDataOutPutPath;
            NRDebugger.Info("[TrackingImageDatabase] DeployData to path :" + deploy_path);
            ZipUtility.UnzipFile(database.RawData, deploy_path, NativeConstants.ZipKey);
        }

        /// <summary> Creates a new UInt64. </summary>
        /// <returns> An UInt64. </returns>
        private UInt64 Create()
        {
            UInt64 config_handle = 0;
            var result = NativeApi.NRConfigCreate(m_NativeInterface.TrackingHandle, ref config_handle);
            NativeErrorListener.Check(result, this, "Create");
            return config_handle;
        }

        /// <summary> Gets plane find mode. </summary>
        /// <param name="config_handle"> The Configuration handle to destroy.</param>
        /// <returns> The plane find mode. </returns>
        public TrackablePlaneFindingMode GetPlaneFindMode(UInt64 config_handle)
        {
            TrackablePlaneFindingMode mode = TrackablePlaneFindingMode.DISABLE;
            var result = NativeApi.NRConfigGetTrackablePlaneFindingMode(m_NativeInterface.TrackingHandle, config_handle, ref mode);
            NativeErrorListener.Check(result, this, "GetPlaneFindMode");
            return mode;
        }

        /// <summary> Sets plane find mode. </summary>
        /// <param name="config_handle"> The Configuration handle to destroy.</param>
        /// <param name="mode">          The mode.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool SetPlaneFindMode(UInt64 config_handle, TrackablePlaneFindingMode mode)
        {
            int mode_value;
            switch (mode)
            {
                case TrackablePlaneFindingMode.DISABLE:
                case TrackablePlaneFindingMode.HORIZONTAL:
                case TrackablePlaneFindingMode.VERTICLE:
                    mode_value = (int)mode;
                    break;
                case TrackablePlaneFindingMode.BOTH:
                    mode_value = ((int)TrackablePlaneFindingMode.HORIZONTAL) | ((int)TrackablePlaneFindingMode.VERTICLE);
                    break;
                default:
                    mode_value = (int)TrackablePlaneFindingMode.DISABLE;
                    break;
            }
            var result = NativeApi.NRConfigSetTrackablePlaneFindingMode(m_NativeInterface.TrackingHandle, config_handle, mode_value);
            NativeErrorListener.Check(result, this, "SetPlaneFindMode");
            return result == NativeResult.Success;
        }

        /// <summary> Gets trackable image data base. </summary>
        /// <param name="config_handle"> The Configuration handle to destroy.</param>
        /// <returns> The trackable image data base. </returns>
        public UInt64 GetTrackableImageDataBase(UInt64 config_handle)
        {
            UInt64 database_handle = 0;
            var result = NativeApi.NRConfigGetTrackableImageDatabase(m_NativeInterface.TrackingHandle, config_handle, ref database_handle);
            NativeErrorListener.Check(result, this, "GetTrackableImageDataBase");
            return database_handle;
        }

        /// <summary> Sets trackable image data base. </summary>
        /// <param name="config_handle">   The Configuration handle to destroy.</param>
        /// <param name="database_handle"> Handle of the database.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool SetTrackableImageDataBase(UInt64 config_handle, UInt64 database_handle)
        {
            var result = NativeApi.NRConfigSetTrackableImageDatabase(m_NativeInterface.TrackingHandle, config_handle, database_handle);
            NativeErrorListener.Check(result, this, "SetTrackableImageDataBase");
            return result == NativeResult.Success;
        }

        /// <summary> Destroys the given config_handle. </summary>
        /// <param name="config_handle"> The Configuration handle to destroy.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Destroy(UInt64 config_handle)
        {
            var result = NativeApi.NRConfigDestroy(m_NativeInterface.TrackingHandle, config_handle);
            NativeErrorListener.Check(result, this, "Destroy");
            return result == NativeResult.Success;
        }

        /// <summary> A native api. </summary>
        private struct NativeApi
        {
            /// <summary> Nr configuration create. </summary>
            /// <param name="session_handle">    Handle of the session.</param>
            /// <param name="out_config_handle"> [in,out] Handle of the out configuration.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRConfigCreate(UInt64 session_handle, ref UInt64 out_config_handle);

            /// <summary> Nr configuration destroy. </summary>
            /// <param name="session_handle"> Handle of the session.</param>
            /// <param name="config_handle">  Handle of the configuration.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRConfigDestroy(UInt64 session_handle, UInt64 config_handle);

            /// <summary> Nr configuration get trackable plane finding mode. </summary>
            /// <param name="session_handle">                   Handle of the session.</param>
            /// <param name="config_handle">                    Handle of the configuration.</param>
            /// <param name="out_trackable_plane_finding_mode"> [in,out] The out trackable plane finding mode.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRConfigGetTrackablePlaneFindingMode(UInt64 session_handle,
                UInt64 config_handle, ref TrackablePlaneFindingMode out_trackable_plane_finding_mode);

            /// <summary> Nr configuration set trackable plane finding mode. </summary>
            /// <param name="session_handle">               Handle of the session.</param>
            /// <param name="config_handle">                Handle of the configuration.</param>
            /// <param name="trackable_plane_finding_mode"> The trackable plane finding mode.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRConfigSetTrackablePlaneFindingMode(UInt64 session_handle,
                UInt64 config_handle, int trackable_plane_finding_mode);

            /// <summary> Nr configuration get trackable image database. </summary>
            /// <param name="session_handle">                      Handle of the session.</param>
            /// <param name="config_handle">                       Handle of the configuration.</param>
            /// <param name="out_trackable_image_database_handle"> [in,out] Handle of the out trackable
            ///                                                    image database.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRConfigGetTrackableImageDatabase(UInt64 session_handle,
                UInt64 config_handle, ref UInt64 out_trackable_image_database_handle);

            /// <summary> Nr configuration set trackable image database. </summary>
            /// <param name="session_handle">                  Handle of the session.</param>
            /// <param name="config_handle">                   Handle of the configuration.</param>
            /// <param name="trackable_image_database_handle"> Handle of the trackable image database.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRConfigSetTrackableImageDatabase(UInt64 session_handle,
                UInt64 config_handle, UInt64 trackable_image_database_handle);
        };
    }
}
