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
    using System.IO;
    using UnityEngine;
    using ICSharpCode.SharpZipLib.Zip;
    using System;

    /// <summary> A zip utility. </summary>
    public class ZipUtility
    {
        #region ZipCallback
        /// <summary> A zip callback. </summary>
        public class ZipCallback
        {
            /// <summary> The on pre zip callback. </summary>
            public Action<ZipEntry> OnPreZipCallback;
            /// <summary> The on post zip callback. </summary>
            public Action<ZipEntry> OnPostZipCallback;
            /// <summary> The on finished callback. </summary>
            public Action<bool> OnFinishedCallback;

            /// <summary> Constructor. </summary>
            /// <param name="onprezip">  The onprezip.</param>
            /// <param name="onpostzip"> The onpostzip.</param>
            /// <param name="onfinish">  The onfinish.</param>
            public ZipCallback(Action<ZipEntry> onprezip, Action<ZipEntry> onpostzip, Action<bool> onfinish)
            {
                OnPreZipCallback = onprezip;
                OnPostZipCallback = onpostzip;
                OnFinishedCallback = onfinish;
            }

            /// <summary> Constructor. </summary>
            /// <param name="onfinish"> The onfinish.</param>
            public ZipCallback(Action<bool> onfinish) : this(null, null, onfinish)
            {
            }

            /// <summary> Executes the 'pre zip' action. </summary>
            /// <param name="_entry"> The entry.</param>
            /// <returns> True if it succeeds, false if it fails. </returns>
            public virtual bool OnPreZip(ZipEntry _entry)
            {
                if (OnPreZipCallback != null)
                {
                    OnPreZipCallback(_entry);
                }
                return true;
            }

            /// <summary> Executes the 'post zip' action. </summary>
            /// <param name="_entry"> The entry.</param>
            public virtual void OnPostZip(ZipEntry _entry)
            {
                if (OnPostZipCallback != null)
                {
                    OnPostZipCallback(_entry);
                }
            }

            /// <summary> Executes the 'finished' action. </summary>
            /// <param name="_result"> True to result.</param>
            public virtual void OnFinished(bool _result)
            {
                if (OnFinishedCallback != null)
                {
                    OnFinishedCallback(_result);
                }
            }
        }
        #endregion

        #region UnzipCallback
        /// <summary> An unzip callback. </summary>
        public abstract class UnzipCallback
        {
            /// <summary> The on pre zip callback. </summary>
            public Action<ZipEntry> OnPreZipCallback;
            /// <summary> The on post zip callback. </summary>
            public Action<ZipEntry> OnPostZipCallback;
            /// <summary> The on finished callback. </summary>
            public Action<bool> OnFinishedCallback;

            /// <summary> Constructor. </summary>
            /// <param name="onprezip">  The onprezip.</param>
            /// <param name="onpostzip"> The onpostzip.</param>
            /// <param name="onfinish">  The onfinish.</param>
            public UnzipCallback(Action<ZipEntry> onprezip, Action<ZipEntry> onpostzip, Action<bool> onfinish)
            {
                OnPreZipCallback = onprezip;
                OnPostZipCallback = onpostzip;
                OnFinishedCallback = onfinish;
            }

            /// <summary> Constructor. </summary>
            /// <param name="onfinish"> The onfinish.</param>
            public UnzipCallback(Action<bool> onfinish) : this(null, null, onfinish)
            {
            }

            /// <summary> Executes the 'pre unzip' action. </summary>
            /// <param name="_entry"> The entry.</param>
            /// <returns> True if it succeeds, false if it fails. </returns>
            public virtual bool OnPreUnzip(ZipEntry _entry)
            {
                if (OnPreZipCallback != null)
                {
                    OnPreZipCallback(_entry);
                }
                return true;
            }

            /// <summary> Executes the 'post unzip' action. </summary>
            /// <param name="_entry"> The entry.</param>
            public virtual void OnPostUnzip(ZipEntry _entry)
            {
                if (OnPostZipCallback != null)
                {
                    OnPostZipCallback(_entry);
                }
            }

            /// <summary> Executes the 'finished' action. </summary>
            /// <param name="_result"> True to result.</param>
            public virtual void OnFinished(bool _result)
            {
                if (OnFinishedCallback != null)
                {
                    OnFinishedCallback(_result);
                }
            }
        }
        #endregion

        /// <summary> Zips. </summary>
        /// <param name="_fileOrDirectoryArray"> Array of file or directories.</param>
        /// <param name="_outputPathName">       Full pathname of the output file.</param>
        /// <param name="_password">             (Optional) The password.</param>
        /// <param name="_zipCallback">          (Optional) The zip callback.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public static bool Zip(string[] _fileOrDirectoryArray, string _outputPathName, string _password = null, ZipCallback _zipCallback = null)
        {
            if ((null == _fileOrDirectoryArray) || string.IsNullOrEmpty(_outputPathName))
            {
                if (null != _zipCallback)
                    _zipCallback.OnFinished(false);

                return false;
            }

            ZipOutputStream zipOutputStream = new ZipOutputStream(File.Create(_outputPathName));
            zipOutputStream.SetLevel(6);    // 压缩质量和压缩速度的平衡点
            if (!string.IsNullOrEmpty(_password))
                zipOutputStream.Password = _password;

            for (int index = 0; index < _fileOrDirectoryArray.Length; ++index)
            {
                bool result = false;
                string fileOrDirectory = _fileOrDirectoryArray[index];
                if (Directory.Exists(fileOrDirectory))
                    result = ZipDirectory(fileOrDirectory, string.Empty, zipOutputStream, _zipCallback);
                else if (File.Exists(fileOrDirectory))
                    result = ZipFile(fileOrDirectory, string.Empty, zipOutputStream, _zipCallback);

                if (!result)
                {
                    if (null != _zipCallback)
                        _zipCallback.OnFinished(false);

                    return false;
                }
            }

            zipOutputStream.Finish();
            zipOutputStream.Close();

            if (null != _zipCallback)
                _zipCallback.OnFinished(true);

            return true;
        }

        /// <summary> Unzip file. </summary>
        /// <param name="_filePathName">  Full pathname of the file file.</param>
        /// <param name="_outputPath">    Full pathname of the output file.</param>
        /// <param name="_password">      (Optional) The password.</param>
        /// <param name="_unzipCallback"> (Optional) The unzip callback.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public static bool UnzipFile(string _filePathName, string _outputPath, string _password = null, UnzipCallback _unzipCallback = null)
        {
            if (string.IsNullOrEmpty(_filePathName) || string.IsNullOrEmpty(_outputPath))
            {
                if (null != _unzipCallback)
                    _unzipCallback.OnFinished(false);

                return false;
            }

            try
            {
                return UnzipFile(File.OpenRead(_filePathName), _outputPath, _password, _unzipCallback);
            }
            catch (System.Exception _e)
            {
                NRDebugger.Error("[ZipUtility.UnzipFile]: " + _e.ToString());

                if (null != _unzipCallback)
                    _unzipCallback.OnFinished(false);

                return false;
            }
        }

        /// <summary> Unzip file. </summary>
        /// <param name="_fileBytes">     The file in bytes.</param>
        /// <param name="_outputPath">    Full pathname of the output file.</param>
        /// <param name="_password">      (Optional) The password.</param>
        /// <param name="_unzipCallback"> (Optional) The unzip callback.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public static bool UnzipFile(byte[] _fileBytes, string _outputPath, string _password = null, UnzipCallback _unzipCallback = null)
        {
            if ((null == _fileBytes) || string.IsNullOrEmpty(_outputPath))
            {
                if (null != _unzipCallback)
                    _unzipCallback.OnFinished(false);

                return false;
            }

            bool result = UnzipFile(new MemoryStream(_fileBytes), _outputPath, _password, _unzipCallback);
            if (!result)
            {
                if (null != _unzipCallback)
                    _unzipCallback.OnFinished(false);
            }

            return result;
        }

        /// <summary> Unzip file. </summary>
        /// <param name="_inputStream">   Stream to read data from.</param>
        /// <param name="_outputPath">    Full pathname of the output file.</param>
        /// <param name="_password">      (Optional) The password.</param>
        /// <param name="_unzipCallback"> (Optional) The unzip callback.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public static bool UnzipFile(Stream _inputStream, string _outputPath, string _password = null, UnzipCallback _unzipCallback = null)
        {
            if ((null == _inputStream) || string.IsNullOrEmpty(_outputPath))
            {
                if (null != _unzipCallback)
                    _unzipCallback.OnFinished(false);

                return false;
            }

            // 创建文件目录
            if (!Directory.Exists(_outputPath))
                Directory.CreateDirectory(_outputPath);

            // 解压Zip包
            ZipEntry entry = null;
            using (ZipInputStream zipInputStream = new ZipInputStream(_inputStream))
            {
                if (!string.IsNullOrEmpty(_password))
                    zipInputStream.Password = _password;

                while (null != (entry = zipInputStream.GetNextEntry()))
                {
                    if (string.IsNullOrEmpty(entry.Name))
                        continue;

                    if ((null != _unzipCallback) && !_unzipCallback.OnPreUnzip(entry))
                        continue;   // 过滤

                    string filePathName = Path.Combine(_outputPath, entry.Name);

                    // 创建文件目录
                    if (entry.IsDirectory)
                    {
                        Directory.CreateDirectory(filePathName);
                        continue;
                    }

                    // 写入文件
                    try
                    {
                        using (FileStream fileStream = File.Create(filePathName))
                        {
                            byte[] bytes = new byte[1024];
                            while (true)
                            {
                                int count = zipInputStream.Read(bytes, 0, bytes.Length);
                                if (count > 0)
                                    fileStream.Write(bytes, 0, count);
                                else
                                {
                                    if (null != _unzipCallback)
                                        _unzipCallback.OnPostUnzip(entry);

                                    break;
                                }
                            }
                        }
                    }
                    catch (System.Exception _e)
                    {
                        NRDebugger.Error("[ZipUtility.UnzipFile]: " + _e.ToString());

                        if (null != _unzipCallback)
                            _unzipCallback.OnFinished(false);

                        return false;
                    }
                }
            }

            if (null != _unzipCallback)
                _unzipCallback.OnFinished(true);

            return true;
        }

        /// <summary> Zip file. </summary>
        /// <param name="_filePathName">    Full pathname of the file file.</param>
        /// <param name="_parentRelPath">   Full pathname of the parent relative file.</param>
        /// <param name="_zipOutputStream"> The zip output stream.</param>
        /// <param name="_zipCallback">     (Optional) The zip callback.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        private static bool ZipFile(string _filePathName, string _parentRelPath, ZipOutputStream _zipOutputStream, ZipCallback _zipCallback = null)
        {
            //Crc32 crc32 = new Crc32();
            ZipEntry entry = null;
            FileStream fileStream = null;
            try
            {
                string entryName = _parentRelPath + '/' + Path.GetFileName(_filePathName);
                entry = new ZipEntry(entryName);
                entry.DateTime = System.DateTime.Now;

                if ((null != _zipCallback) && !_zipCallback.OnPreZip(entry))
                    return true;    // 过滤

                fileStream = File.OpenRead(_filePathName);
                byte[] buffer = new byte[fileStream.Length];
                fileStream.Read(buffer, 0, buffer.Length);
                fileStream.Close();

                entry.Size = buffer.Length;

                //crc32.Reset();
                //crc32.Update(buffer);
                //entry.Crc = crc32.Value;

                _zipOutputStream.PutNextEntry(entry);
                _zipOutputStream.Write(buffer, 0, buffer.Length);
            }
            catch (System.Exception _e)
            {
                NRDebugger.Error("[ZipUtility.ZipFile]: " + _e.ToString());
                return false;
            }
            finally
            {
                if (null != fileStream)
                {
                    fileStream.Close();
                    fileStream.Dispose();
                }
            }

            if (null != _zipCallback)
                _zipCallback.OnPostZip(entry);

            return true;
        }

        /// <summary> Zip directory. </summary>
        /// <param name="_path">            Full pathname of the file.</param>
        /// <param name="_parentRelPath">   Full pathname of the parent relative file.</param>
        /// <param name="_zipOutputStream"> The zip output stream.</param>
        /// <param name="_zipCallback">     (Optional) The zip callback.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        private static bool ZipDirectory(string _path, string _parentRelPath, ZipOutputStream _zipOutputStream, ZipCallback _zipCallback = null)
        {
            ZipEntry entry = null;
            try
            {
                string entryName = Path.Combine(_parentRelPath, Path.GetFileName(_path) + '/');
                entry = new ZipEntry(entryName);
                entry.DateTime = System.DateTime.Now;
                entry.Size = 0;

                if ((null != _zipCallback) && !_zipCallback.OnPreZip(entry))
                    return true;    // 过滤

                _zipOutputStream.PutNextEntry(entry);
                _zipOutputStream.Flush();

                string[] files = Directory.GetFiles(_path);
                for (int index = 0; index < files.Length; ++index)
                    ZipFile(files[index], Path.Combine(_parentRelPath, Path.GetFileName(_path)), _zipOutputStream, _zipCallback);
            }
            catch (System.Exception _e)
            {
                NRDebugger.Error("[ZipUtility.ZipDirectory]: " + _e.ToString());
                return false;
            }

            string[] directories = Directory.GetDirectories(_path);
            for (int index = 0; index < directories.Length; ++index)
            {
                if (!ZipDirectory(directories[index], Path.Combine(_parentRelPath, Path.GetFileName(_path)), _zipOutputStream, _zipCallback))
                    return false;
            }

            if (null != _zipCallback)
                _zipCallback.OnPostZip(entry);

            return true;
        }
    }
}