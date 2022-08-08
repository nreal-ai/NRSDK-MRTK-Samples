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
    using System.Diagnostics.CodeAnalysis;
    using System.Xml;
    using UnityEditor;
    using UnityEditor.Build;
    using System.IO;
#if UNITY_2018_1_OR_NEWER
    using UnityEditor.Build.Reporting;
    using UnityEngine;
    using UnityEditor.Android;
    using System.Text;
    using System.Text.RegularExpressions;
#endif

#if UNITY_2018_1_OR_NEWER
    /// <summary> The preprocess build base. </summary>
    internal class PreprocessBuildBase : IPreprocessBuildWithReport
#else
    internal class PreprocessBuildBase : IPreprocessBuild
#endif
    {
        /// <summary>
        /// <para>Returns the relative callback order for callbacks.  Callbacks with lower values are
        /// called before ones with higher values.</para> </summary>
        /// <value> The callback order. </value>
        public int callbackOrder
        {
            get
            {
                return 0;
            }
        }

#if UNITY_2018_1_OR_NEWER
        /// <summary>
        /// <para>Implement this function to receive a callback before the build is started.</para> </summary>
        /// <param name="report"> A report containing information about the build, such as its target
        ///                       platform and output path.</param>
        public void OnPreprocessBuild(BuildReport report)
        {
            OnPreprocessBuild(report.summary.platform, report.summary.outputPath);
        }
#endif

        /// <summary> Executes the 'preprocess build' action. </summary>
        /// <param name="target"> Target for the.</param>
        /// <param name="path">   Full pathname of the file.</param>
        public virtual void OnPreprocessBuild(BuildTarget target, string path)
        {
            if (target == BuildTarget.Android)
            {
                OnPreprocessBuildForAndroid();
            }
        }

        private const string DefaultXML = @"<?xml version='1.0' encoding='utf-8'?>
<manifest
    xmlns:android='http://schemas.android.com/apk/res/android'
    package='com.unity3d.player'
    xmlns:tools='http://schemas.android.com/tools'
    android:installLocation='preferExternal'>
  <uses-sdk tools:overrideLibrary='com.nreal.glasses_sdk'/>
  <supports-screens
      android:smallScreens='true'
      android:normalScreens='true'
      android:largeScreens='true'
      android:xlargeScreens='true'
      android:anyDensity='true'/>

  <application
      android:theme='@style/UnityThemeSelector'
      android:icon='@mipmap/app_icon'
      android:label='@string/app_name'>
    <activity android:name='com.unity3d.player.UnityPlayerActivity'>
      <intent-filter>
        <action android:name='android.intent.action.MAIN' />
        <category android:name='android.intent.category.INFO' tools:node='replace'/>
      </intent-filter>
    </activity>
    <meta-data android:name='nreal_sdk' android:value='true'/>
    <meta-data android:name='com.nreal.supportDevices' android:value=''/>
  </application>
  <uses-permission android:name='android.permission.BLUETOOTH'/>
</manifest>
        ";

        /// <summary> True if is show on desktop, false if not. </summary>
        private const bool isShowOnDesktop = true;
        /// <summary> Executes the 'preprocess build for android' action. </summary>
        [MenuItem("NRSDK/PreprocessBuildForAndroid")]
        public static void OnPreprocessBuildForAndroid()
        {
            string basePath = Application.dataPath + "/Plugins/Android";
            if (!Directory.Exists(Application.dataPath + "/Plugins"))
            {
                Directory.CreateDirectory(Application.dataPath + "/Plugins");
            }
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }
            string xmlPath = Application.dataPath + "/Plugins/Android/AndroidManifest.xml";

            if (!File.Exists(xmlPath))
            {
                string xml = DefaultXML.Replace("\'", "\"");
                if (isShowOnDesktop)
                {
                    xml = xml.Replace("<category android:name=\"android.intent.category.INFO\" tools:node=\"replace\"/>",
                       "<category android:name=\"android.intent.category.LAUNCHER\" />");
                }
                File.WriteAllText(xmlPath, xml);
            }

            AutoGenerateAndroidManifest(xmlPath);

            ApplySettingsToConfig();
            AutoGenerateAndroidGradleTemplate();
            AssetDatabase.Refresh();
        }

        private static void ApplySettingsToConfig()
        {
            NRProjectConfig projectConfig = NRProjectConfigHelper.GetProjectConfig();

            var sessionConfigGuids = AssetDatabase.FindAssets("t:NRSessionConfig");
            foreach (var item in sessionConfigGuids)
            {
                var config = AssetDatabase.LoadAssetAtPath<NRSessionConfig>(
                    AssetDatabase.GUIDToAssetPath(item));
                config.SetUseMultiThread(PlayerSettings.GetMobileMTRendering(BuildTargetGroup.Android));
                config.SetProjectConfig(projectConfig);
                EditorUtility.SetDirty(config);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("NRSDK/AddAudioPermission")]
        public static void AddAudioPermission()
        {
            string xmlPath = Application.dataPath + "/Plugins/Android/AndroidManifest.xml";
            var androidManifest = new AndroidManifest(xmlPath);
            androidManifest.SetAudioRecordPermission();
            androidManifest.Save();
        }

        [MenuItem("NRSDK/InitVideoPlayerDemoEnv")]
        public static void InitVideoPlayerDemoEnv()
        {
            string javaDir = "";
            string[] assets = AssetDatabase.FindAssets("AndroidMediaPlayer");
            foreach (var asset in assets)
            {
                string path = AssetDatabase.GUIDToAssetPath(asset);
                if (path.Contains("NRSDKExperimental") && path.EndsWith(".java"))
                {
                    javaDir = Path.GetDirectoryName(path);
                    break;
                }
            }

            if (!string.IsNullOrEmpty(javaDir))
            {
                string[] files = Directory.GetFiles(javaDir, "*.java");
                foreach (var file in files)
                {
                    PluginImporter importer = (PluginImporter)AssetImporter.GetAtPath(file);
                    if (importer != null)
                    {
                        importer.SetCompatibleWithPlatform(BuildTarget.Android, true);
                        importer.SaveAndReimport();
                    }
                }
            }

            //set project dependency
            string gradleFile = "mainTemplate.gradle";
            if (UseCustomGradleFile(gradleFile))
            {
                string gradleInProject = Application.dataPath + "/Plugins/Android/" + gradleFile;
                AndroidGradleTemplate gradleTmp = new AndroidGradleTemplate(gradleInProject);
                gradleTmp.AddDenpendency("implementation 'com.google.android.exoplayer:exoplayer:2.13.2'");
                gradleTmp.PreprocessGradleFile();
            }
            EditorUtility.DisplayDialog("Tips", "Added ExoPlayer library dependency. The scene Overlay-VideoPlayer now supports DRM video playback.", "OK");
        }

        /// <summary> Automatic generate android manifest. </summary>
        /// <param name="path"> Full pathname of the file.</param>
        public static void AutoGenerateAndroidManifest(string path)
        {
            var androidManifest = new AndroidManifest(path);
            //bool needrequestLegacyExternalStorage = (GetAndroidTargetApiLevel() >= 29);
            //androidManifest.SetExternalStorage(needrequestLegacyExternalStorage);
            // androidManifest.SetPackageReadPermission();
            //androidManifest.SetCameraPermission();
            androidManifest.SetBlueToothPermission();
            androidManifest.SetSDKMetaData();
            androidManifest.SetAPKDisplayedOnLauncher(isShowOnDesktop);

            androidManifest.Save();
        }

        public static int GetAndroidTargetApiLevel()
        {
            if (PlayerSettings.Android.targetSdkVersion == AndroidSdkVersions.AndroidApiLevelAuto)
            {
                string androidsdkRoot = EditorPrefs.GetString("AndroidSdkRoot");
                string[] androidVersions = Directory.GetDirectories(Path.Combine(androidsdkRoot, "platforms"));
                string regex = "android-([0-9]*)";
                int maxVersion = (int)AndroidSdkVersions.AndroidApiLevelAuto;
                foreach (var item in androidVersions)
                {
                    MatchCollection coll = Regex.Matches(item, regex);
                    if (coll != null && coll[0] != null && coll[0].Groups[1] != null)
                    {
                        string result = coll[0].Groups[1].Value;
                        int versionCode = int.Parse(result);
                        if (versionCode > maxVersion)
                        {
                            maxVersion = versionCode;
                        }
                    }
                }
                return maxVersion;
            }
            else
            {
                return (int)PlayerSettings.Android.targetSdkVersion;
            }
        }

        /// In order to support 'queries' in manifest, gradle plugin must be higher than 3.4.3. 
        /// For unity version lower than 2020, this can be modified by using 'Custom Gradle Template', then modify the gradle plugin version in template file
        public static void AutoGenerateAndroidGradleTemplate()
        {
            string version = Application.unityVersion;
            Debug.Log("UnityVersion: " + version);
            string vMain = version.Substring(0, 4);
            int nVersion = 0;
            int.TryParse(vMain, out nVersion);
            if (nVersion < 2018)
                Debug.LogErrorFormat("NRSDK require unity version higher than 2018");
            else if (nVersion == 2018)
                AutoGenerateAndroidGradleTemplate("mainTemplate.gradle");
            else
                AutoGenerateAndroidGradleTemplate("baseProjectTemplate.gradle");

            //unity2020 should add support android.useAndroidX manually
            if (nVersion >= 2020)
            {
                AutoGenerateGradleProperties("gradleTemplate.properties");
            }
        }

        /// <summary>
        /// Use custom gradle file, copy from unity install directory to project directory.
        /// </summary>
        /// <param name="templateFileName"> gradle file name. </param>
        private static bool UseCustomGradleFile(string templateFileName)
        {
            string gradleInProject = Application.dataPath + "/Plugins/Android/" + templateFileName;
            if (!File.Exists(gradleInProject))
            {
                string unityEditorPath = EditorApplication.applicationPath;

                string gradleReletPath = "/PlaybackEngines/AndroidPlayer/Tools/GradleTemplates/" + templateFileName;
#if UNITY_EDITOR_WIN
                gradleReletPath = "/data" + gradleReletPath;
#endif
                string gradleFullPath = unityEditorPath.Substring(0, unityEditorPath.LastIndexOf("/")) + gradleReletPath;
                Debug.LogFormat("Copy gradle template : {0} --> {1}", gradleFullPath, gradleInProject);

                if (File.Exists(gradleFullPath))
                    File.Copy(gradleFullPath, gradleInProject);
                else
                {
                    Debug.LogErrorFormat("GradleTemplate of unity not found : {0}", gradleFullPath);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Generate custom gradle file, set minal
        /// </summary>
        /// <param name="templateFileName"> Gradle file name. </param>
        private static void AutoGenerateAndroidGradleTemplate(string templateFileName)
        {
            if(!UseCustomGradleFile(templateFileName))
            {
                return;
            }
            string gradleInProject = Application.dataPath + "/Plugins/Android/" + templateFileName;
            AndroidGradleTemplate gradleTmp = new AndroidGradleTemplate(gradleInProject);
            gradleTmp.SetMinPluginVersion(3, 4, 3);
            gradleTmp.PreprocessGradleFile();
        }


        private static void AutoGenerateGradleProperties(string templateFileName)
        {
            if (!UseCustomGradleFile(templateFileName))
            {
                return;
            }
            string gradleInProject = Application.dataPath + "/Plugins/Android/" + templateFileName;
            AndroidGradleTemplate gradleTmp = new AndroidGradleTemplate(gradleInProject);
            gradleTmp.AddSupport("android.useAndroidX");
            gradleTmp.PreprocessGradleFile();
        }
    }
}