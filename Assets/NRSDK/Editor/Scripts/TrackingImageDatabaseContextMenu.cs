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
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    
    /// <summary> A tracking image database context menu. </summary>
    public static class TrackingImageDatabaseContextMenu
    {
        /// <summary> Message describing the supported image format list. </summary>
        private const string _SupportedImageFormatListMessage = "PNG and JPEG";

        /// <summary> The supported image extensions. </summary>
        private static readonly List<string> k_SupportedImageExtensions = new List<string>()
        {
            ".png", ".jpg", ".jpeg"
        };

        /// <summary> The unsupported image extensions. </summary>
        private static readonly List<string> k_UnsupportedImageExtensions = new List<string>()
        {
            ".psd", ".tiff", ".tga", ".gif", ".bmp", ".iff", ".pict"
        };

        /// <summary> Adds assets to new tracking image database. </summary>
        [MenuItem("Assets/Create/NRSDK/TrackingImageDatabase", false, 2)]
        private static void AddAssetsToNewTrackingImageDatabase()
        {
            var selectedImagePaths = new List<string>();
            bool unsupportedImagesSelected = false;

            selectedImagePaths = GetSelectedImagePaths(out unsupportedImagesSelected);
            if (unsupportedImagesSelected)
            {
                var message = string.Format("Some selected images could not be added to the TrackingImageDatabase because " +
                    "they are not in a supported format.  Supported image formats are {0}.",
                    _SupportedImageFormatListMessage);
                Debug.LogWarningFormat(message);
                EditorUtility.DisplayDialog("Unsupported Images Selected", message, "Ok");
            }

            if (selectedImagePaths.Count > 0)
            {
                var newDatabase = ScriptableObject.CreateInstance<NRTrackingImageDatabase>();

                var newEntries = new List<NRTrackingImageDatabaseEntry>();
                foreach (var imagePath in selectedImagePaths)
                {
                    var fileName = Path.GetFileName(imagePath);
                    var imageName = fileName.Replace(Path.GetExtension(fileName), string.Empty);
                    newEntries.Add(new NRTrackingImageDatabaseEntry(imageName,
                        AssetDatabase.LoadAssetAtPath<Texture2D>(imagePath)));
                }

                newEntries = newEntries.OrderBy(x => x.Name).ToList();


                foreach (var entry in newEntries)
                {
                    newDatabase.Add(entry);
                }

                string selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);
                if (selectedPath == string.Empty)
                {
                    selectedPath = "Assets";
                }
                else if (Path.GetExtension(selectedPath) != string.Empty)
                {
                    selectedPath = selectedPath.Replace(Path.GetFileName(selectedPath), string.Empty);
                }

                var newAssetPath = AssetDatabase.GenerateUniqueAssetPath(
                    Path.Combine(selectedPath, "TrackingImageDatabase.asset"));
                AssetDatabase.CreateAsset(newDatabase, newAssetPath);
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = newDatabase;
            }
        }

        /// <summary> Adds assets to new tracking image database validation. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        [MenuItem("Assets/Create/NRSDK/TrackingImageDatabase", true, 2)]
        private static bool AddAssetsToNewTrackingImageDatabaseValidation()
        {
            bool unsupportedSelected;
            return GetSelectedImagePaths(out unsupportedSelected).Count > 0;
        }

        /// <summary> Gets selected image paths. </summary>
        /// <param name="unsupportedImagesSelected"> [out] True if unsupported images selected.</param>
        /// <returns> The selected image paths. </returns>
        private static List<string> GetSelectedImagePaths(out bool unsupportedImagesSelected)
        {
            var selectedImagePaths = new List<string>();

            unsupportedImagesSelected = false;
            foreach (var GUID in Selection.assetGUIDs)
            {
                var path = AssetDatabase.GUIDToAssetPath(GUID);
                var extension = Path.GetExtension(path).ToLower();

                if (k_SupportedImageExtensions.Contains(extension))
                {
                    selectedImagePaths.Add(AssetDatabase.GUIDToAssetPath(GUID));
                }
                else if (k_UnsupportedImageExtensions.Contains(extension))
                {
                    unsupportedImagesSelected = true;
                }
            }
            return selectedImagePaths;
        }
    }
    
}
