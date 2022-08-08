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
    using UnityEngine;

#if UNITY_EDITOR
    using System.IO;
    using UnityEditor;
    using LitJson;
    using System.Text;
#endif

    /// <summary>
    /// A database storing a list of images to be detected and tracked by NRSDK. An image database
    /// supports up to 1000 images.Only one image database can be in use at any given time. </summary>
    public class NRTrackingImageDatabase : ScriptableObject
    {
        /// <summary> The images. </summary>
        [SerializeField]
        private List<NRTrackingImageDatabaseEntry> m_Images = new List<NRTrackingImageDatabaseEntry>();

        /// <summary> Information describing the raw. </summary>
        [SerializeField]
        private byte[] m_RawData = null;
        /// <summary> Gets information describing the raw. </summary>
        /// <value> Information describing the raw. </value>
        public byte[] RawData
        {
            get
            {
                return m_RawData;
            }
        }

        /// <summary> Unique identifier. </summary>
        public string GUID;

        /// <summary> Gets the full pathname of the tracking image data file. </summary>
        /// <value> The full pathname of the tracking image data file. </value>
        public string TrackingImageDataPath
        {
            get
            {
                return NRTools.GetTrackingImageDataGenPath() + GUID + "/";
            }
        }

        /// <summary> Gets the full pathname of the tracking image data out put file. </summary>
        /// <value> The full pathname of the tracking image data out put file. </value>
        public string TrackingImageDataOutPutPath
        {
            get
            {
                return NRTools.GetTrackingImageDataGenPath();
            }
        }

#if UNITY_EDITOR
        /// <summary> True if is raw data dirty, false if not. </summary>
        [SerializeField]
        private bool m_IsRawDataDirty = true;

        /// <summary> The CLI version. </summary>
        [SerializeField]
        private string m_CliVersion = string.Empty;

        /// <summary> Gets a value indicating whether this object is CLI updated. </summary>
        /// <value> True if this object is CLI updated, false if not. </value>
        public bool isCliUpdated
        {
            get
            {
                string cliBinaryPath;
                if (!FindCliBinaryPath(out cliBinaryPath))
                {
                    return false;
                }

                string currentCliVersion;
                {
                    string error;
                    ShellHelper.RunCommand(cliBinaryPath, "-version", out currentCliVersion, out error);
                }
                //NRDebugger.Info("current version:{0} old version:{1}", currentCliVersion, m_CliVersion);

                bool cliUpdated = m_CliVersion != currentCliVersion;
                return cliUpdated;
            }
        }
#endif

        /// <summary> Constructs a new <c>TrackingImageDatabase</c>. </summary>
        public NRTrackingImageDatabase()
        {
#if UNITY_EDITOR
            GUID = Guid.NewGuid().ToString();
#endif
        }

        /// <summary> Gets the number of images in the database. </summary>
        /// <value> The count. </value>
        public int Count
        {
            get
            {
                lock (m_Images)
                {
                    return m_Images.Count;
                }
            }
        }

        /// <summary>
        /// Gets or sets the image at the specified <c>index</c>. You can only modify the database in the
        /// Unity editor. </summary>
        /// <param name="index"> The zero-based index of the image entry to get or set.</param>
        /// <returns> The image entry at <c>index</c>. </returns>
        public NRTrackingImageDatabaseEntry this[int index]
        {
            get
            {
                lock (m_Images)
                {
                    return m_Images[index];
                }
            }

#if UNITY_EDITOR
            set
            {
                var oldValue = m_Images[index];
                m_Images[index] = value;

                if (oldValue.TextureGUID != m_Images[index].TextureGUID
                    || oldValue.Name != m_Images[index].Name
                    || oldValue.Width != m_Images[index].Width
                    || oldValue.Height != m_Images[index].Height)
                {
                    m_IsRawDataDirty = true;
                }

                EditorUtility.SetDirty(this);
            }
#endif
        }

#if UNITY_EDITOR
        
        /// <summary> Adds an image entry to the end of the database. </summary>
        /// <param name="entry"> The image entry to add.</param>
        public void Add(NRTrackingImageDatabaseEntry entry)
        {
            m_Images.Add(entry);
            EditorUtility.SetDirty(this);
        }
        

        
        /// <summary> Removes an image entry at a specified zero-based index. </summary>
        /// <param name="index"> The index of the image entry to remove.</param>
        public void RemoveAt(int index)
        {
            m_Images.RemoveAt(index);
            EditorUtility.SetDirty(this);
        }
        

        
        /// <summary> Rebuilds the database asset, if needed. </summary>
        public void BuildIfNeeded()
        {
            if (!m_IsRawDataDirty)
            {
                return;
            }
            m_IsRawDataDirty = false;

            string directory = NRTools.GetTrackingImageDataGenPath() + GUID;
            if (!Directory.Exists(directory))
            {
                ZipUtility.UnzipFile(RawData, NRTools.GetTrackingImageDataGenPath(), NativeConstants.ZipKey);
            }

            //Generate marker data by json file
            var result_json = TrackingImageDataPath + "markers.json";
            if (File.Exists(result_json))
            {
                var json_data = File.ReadAllText(result_json);

                StringBuilder str = new StringBuilder();
                str.AppendLine("# Number of markers");
                str.AppendLine(Count.ToString());

                DirectoryInfo dir = new DirectoryInfo(directory);
                var fsinfos = dir.GetDirectories();
                string dataPathName = "Data";

                if (fsinfos != null && fsinfos.Length > 0)
                {
                    Array.Sort(fsinfos, (dir1, dir2) => dir1.CreationTime.CompareTo(dir2.CreationTime));
                    dataPathName = fsinfos[0].Name;
                }
                for (int i = 0; i < Count; i++)
                {
                    var obj = JsonMapper.ToObject(json_data);
                    if (obj != null)
                    {
                        var image_info = obj[this[i].Name];
                        if (image_info != null)
                        {
                            str.AppendLine();
                            str.AppendLine(string.Format("./{0}/{1}", dataPathName, this[i].Name));
                            str.AppendLine("NFT");
                            str.AppendLine(string.Format("FILTER {0}", image_info["filter"]));
                            str.AppendLine(string.Format("MARKER_WIDTH {0}", image_info["physical_width"]));
                            str.AppendLine(string.Format("MARKER_HEIGHT {0}", image_info["physical_height"]));
                        }
                    }
                }

                File.WriteAllText(TrackingImageDataPath + "markers.dat", str.ToString());
            }

            string file_path = directory + "_zipFile";
            // Generate zip file
            ZipUtility.Zip(new string[1] { directory }, file_path, NativeConstants.ZipKey, new ZipUtility.ZipCallback(_result =>
            {
                m_IsRawDataDirty = _result ? false : true;
                if (!string.IsNullOrEmpty(file_path) && File.Exists(file_path))
                {
                    // Read the zip bytes
                    m_RawData = File.ReadAllBytes(file_path);
                    //NRDebugger.Info("Generate raw data success!" + file_path);
                    //m_IsNeedLoadRawData = false;

                    EditorUtility.SetDirty(this);
                    // Force a save to make certain build process will get updated asset.
                    AssetDatabase.SaveAssets();
                }
            }));

            UpdateClipVersion();
        }

        
        /// <summary> Updates the clip version. </summary>
        private void UpdateClipVersion()
        {
            string cliBinaryPath;
            if (!FindCliBinaryPath(out cliBinaryPath))
            {
                return;
            }

            string currentCliVersion;
            {
                string error;
                ShellHelper.RunCommand(cliBinaryPath, "-version", out currentCliVersion, out error);
            }
            //NRDebugger.Info("current version:{0} old version:{1}", currentCliVersion, m_CliVersion);

            m_CliVersion = currentCliVersion;
        }

        
        /// <summary> Gets the image entries that require updating of the image quality score. </summary>
        /// <returns> A list of image entries that require updating of the image quality score. </returns>
        public List<NRTrackingImageDatabaseEntry> GetDirtyQualityEntries()
        {
            var dirtyEntries = new List<NRTrackingImageDatabaseEntry>();
            for (int i = 0; i < m_Images.Count; ++i)
            {
                if (!string.IsNullOrEmpty(m_Images[i].Quality))
                {
                    continue;
                }

                dirtyEntries.Add(m_Images[i]);
            }

            if (dirtyEntries.Count == 0)
            {
                BuildIfNeeded();
            }

            return dirtyEntries;
        }
        

        
        /// <summary> Gets all entries. </summary>
        /// <returns> all entries. </returns>
        public List<NRTrackingImageDatabaseEntry> GetAllEntries()
        {
            var allEntries = new List<NRTrackingImageDatabaseEntry>();
            for (int i = 0; i < m_Images.Count; ++i)
            {
                allEntries.Add(m_Images[i]);
            }
            return allEntries;
        }
        

        
        /// <summary> Finds the path to the command-line tool used to generate a database. </summary>
        /// <param name="path"> [out] The path to the command-line tool that will be set if a valid path
        ///                     was found.</param>
        /// <returns> <c>true</c> if a valid path was found, <c>false</c> otherwise. </returns>
        public static bool FindCliBinaryPath(out string path)
        {
            var binaryName = NativeConstants.TrackingImageCliBinary;
            string[] cliBinaryGuid = AssetDatabase.FindAssets(binaryName);
            if (cliBinaryGuid.Length == 0)
            {
                NRDebugger.Info("Could not find required tool for building TrackingImageDatabase: {0}. " +
                    "Was it removed from the NRSDK?", binaryName);
                path = string.Empty;
                return false;
            }

            // Remove the '/Assets' from the project path since it will be added in the path below.
            string projectPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
            path = Path.Combine(projectPath, AssetDatabase.GUIDToAssetPath(cliBinaryGuid[0]));
            return !string.IsNullOrEmpty(path);
        }
        
#endif
    }
}
