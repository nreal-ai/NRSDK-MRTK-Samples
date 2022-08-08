/****************************************************************************
* Copyright 2019 Nreal Techonology Limited.All rights reserved.
*
* This file is part of NRSDK.
*
* https://www.nreal.ai/        
*
*****************************************************************************/

namespace NRKernal
{
    using System;
    using System.Text;
    using System.Xml;
    using System.IO;
    using System.Linq.Expressions;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary> A list of the android. </summary>
    internal class AndroidGradleTemplate
    {
        private enum EPatcherType
        {
            EPT_NONE = 0,
            EPT_PLUGIN_VERSION = 1,
            EPT_ADD_DEPENDENCIES = 2,
            EPT_REMOVE_DEPENDENCIES = 3,
            EPT_ADD_SUPPORT = 4,
        }
        private interface IGradlePatcher
        {
            void PreprocessLine(string line);
            bool ProcessLine(string line, ref string result);
        }

        private class GradlePluginVersionPatcher : IGradlePatcher
        {
            const string PLUGIN_VERSION_TOKEN = "com.android.tools.build:gradle:";
            private int mMajorVersionNum;
            private int mMiddleVersionNum;
            private int mMinorVersionNum;
            public GradlePluginVersionPatcher()
            {
                mMajorVersionNum = 0;
                mMiddleVersionNum = 0;
                mMinorVersionNum = 0;
            }
            public void SetMinPluginVersion(int major, int middle, int minor)
            {
                mMajorVersionNum = major;
                mMiddleVersionNum = middle;
                mMinorVersionNum = minor;
            }

            public void PreprocessLine(string line)
            {

            }

            public bool ProcessLine(string line, ref string result)
            {
                bool updateVersion = false;
                var idx = line.IndexOf(PLUGIN_VERSION_TOKEN);
                if (idx > 0)
                {
                    string subLine = line.Substring(idx + PLUGIN_VERSION_TOKEN.Length);
                    string subVersion = subLine.Substring(0, subLine.IndexOf('\''));
                    Debug.LogFormat("subVersion : {0}", subVersion);

                    string[] versions = subVersion.Split('.');
                    if (versions.Length == 3)
                    {
                        int.TryParse(versions[0], out int vMain);
                        int.TryParse(versions[1], out int vMiddle);
                        int.TryParse(versions[2], out int vMin);
                        
                        if (vMain < mMajorVersionNum)
                        {
                            updateVersion = true;
                        }
                        else if(vMain == mMajorVersionNum)
                        {
                            if(vMiddle < mMiddleVersionNum)
                            {
                                updateVersion = true;
                            }
                            else if(vMiddle == mMiddleVersionNum)
                            {
                                if(vMin < mMinorVersionNum)
                                {
                                    updateVersion = true;
                                }
                            }
                        }

                        if (updateVersion)
                        {
                            result = line.Replace(subVersion, "3.4.3");
                            Debug.LogFormat("update gradle setting : {0} --> {1}", subVersion, "3.4.3");
                        }
                    }
                }
                return updateVersion;
            }
        }

        private class GradleAddDependenciesPatcher : IGradlePatcher
        {
            const string DEPS_MARK = "**DEPS**";
            private List<string> mDependencies;
            public GradleAddDependenciesPatcher()
            {
                mDependencies = new List<string>();
            }

            public void AddDependency(string dependency)
            {
                mDependencies.Add(dependency);
            }

            public void PreprocessLine(string line)
            {
                for(int i = mDependencies.Count - 1; i >= 0; i--)
                {
                    if(line.Contains(mDependencies[i]))
                    {
                        //this dependency is already in the gradle file
                        mDependencies.RemoveAt(i);
                    }
                }
            }
            public bool ProcessLine(string line, ref string result)
            {
                if(mDependencies.Count > 0 && line.Contains(DEPS_MARK))
                {
                    result = "    " + string.Join("\n    ", mDependencies);
                    result = result + "\n" + line;
                    return true;
                }
                return false;
            }
        }

        private class GradleRemoveDependenciesPatcher : IGradlePatcher
        {
            private List<string> mDependencies;
            public GradleRemoveDependenciesPatcher()
            {
                mDependencies = new List<string>();
            }

            public void RemoveDependency(string dependency)
            {
                mDependencies.Add(dependency);
            }

            public void PreprocessLine(string line)
            {

            }

            public bool ProcessLine(string line, ref string result)
            {
                bool includeDeps = false;
                for (int i = 0; i < mDependencies.Count; i++)
                {
                    if (line.Contains(mDependencies[i]))
                    {
                        includeDeps = true;
                        //remove this line
                        result = null;
                        break;
                    }
                }
                return includeDeps;
            }
        }

        private class GradleAddSupportPatcher : IGradlePatcher
        {
            private Dictionary<string, bool> mKeyTokenAlreadyInFile = new Dictionary<string, bool>();
            private bool mIsFirst = true;
            private List<string> mTokenList = new List<string>();
            public void PreprocessLine(string line)
            {
                for(int i = 0; i < mTokenList.Count; i++)
                {
                    var token = mTokenList[i];
                    if (line.Contains(token))
                    {
                        mKeyTokenAlreadyInFile[token] = true;
                    }
                }
            }

            public bool ProcessLine(string line, ref string result)
            {
                string tempLine = "";
                if (mIsFirst)
                {
                    tempLine = GetSupportStringNotInFile();
                    mIsFirst = false;
                }

                foreach (var pair in mKeyTokenAlreadyInFile)
                {
                    if (pair.Value)
                    {
                        if (line.Contains(pair.Key))
                        {
                            result = tempLine + pair.Key + "=true";
                            return true;
                        }
                    }
                }
                result = tempLine + line;
                return false;
            }

            private string GetSupportStringNotInFile()
            {
                string line = "";
                foreach (var pair in mKeyTokenAlreadyInFile)
                {
                    if (!pair.Value)
                    {
                        line = string.Format("{0}=true\n{1}", pair.Key, line);
                    }
                }
                return line;
            }

            public void AddSupport(string keyToken)
            {
                mKeyTokenAlreadyInFile.Add(keyToken, false);
                mTokenList.Add(keyToken);
            }
        }

        Dictionary<EPatcherType, IGradlePatcher> mPatchers = null;
        string m_Path;
        public AndroidGradleTemplate(string path)
        {
            m_Path = path;
            mPatchers = new Dictionary<EPatcherType, IGradlePatcher>();
        }

        private T GetOrAddPatcher<T>(EPatcherType type) where T : IGradlePatcher, new()
        {
            if (!mPatchers.TryGetValue(type, out IGradlePatcher patcher))
            {
                patcher = new T();
                mPatchers.Add(type, patcher);
            }
            return (T)patcher;
        }
        public void SetMinPluginVersion(int major, int middle, int minor)
        {
            GradlePluginVersionPatcher pluginVersionPatcher = GetOrAddPatcher<GradlePluginVersionPatcher>(
                EPatcherType.EPT_PLUGIN_VERSION);
            pluginVersionPatcher.SetMinPluginVersion(major, middle, minor);
        }

        public void AddDenpendency(string dependency)
        {
            GradleAddDependenciesPatcher addDepPatcher = GetOrAddPatcher<GradleAddDependenciesPatcher>(
                EPatcherType.EPT_ADD_DEPENDENCIES);
            addDepPatcher.AddDependency(dependency);
        }

        public void RemoveDependency(string dependency)
        {
            GradleRemoveDependenciesPatcher removeDepPatcher = GetOrAddPatcher<GradleRemoveDependenciesPatcher>(
                EPatcherType.EPT_REMOVE_DEPENDENCIES);
            removeDepPatcher.RemoveDependency(dependency);
        }

        public void AddSupport(string keyToken)
        {
            GradleAddSupportPatcher addSupportPatcher = GetOrAddPatcher<GradleAddSupportPatcher>(
                EPatcherType.EPT_ADD_SUPPORT);
            addSupportPatcher.AddSupport(keyToken);
        }

        public void PreprocessGradleFile()
        {
            if (mPatchers.Count <= 0)
                return;
            try
            {
                List<string> content = new List<string>();
                var lines = File.ReadAllLines(m_Path);
                string newLine = null;
                foreach (string line in lines)
                {
                    foreach (var pair in mPatchers)
                    {
                        pair.Value.PreprocessLine(line);
                    }
                }

                foreach (string line in lines)
                {
                    newLine = line;
                    foreach (var pair in mPatchers)
                    {
                        if(pair.Value.ProcessLine(line, ref newLine))
                        {
                            break;
                        }
                    }
                    //Original line may be empty, not null
                    if (newLine != null)
                    {
                        content.Add(newLine);
                    }
                }
                
                File.WriteAllLines(m_Path, content);
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("PreprocessGradleFile exception : {0}", ex.Message);
            }
        }
    }

}
