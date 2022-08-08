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
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
    using System.IO;
#endif

    /// <summary> Hold the total infomation of a image data base item. </summary>
    [Serializable]
    public struct NRTrackingImageDatabaseEntry
    {
        /// <summary> The name assigned to the tracked image. </summary>
        public string Name;

        /// <summary> The width of the image in meters. </summary>
        public float Width;

        /// <summary> The height of the image in meters. </summary>
        public float Height;

        /// <summary> The quality of the image. </summary>
        public string Quality;

        /// <summary> The Unity GUID for this entry. </summary>
        public string TextureGUID;

        /// <summary> Contructs a new Augmented Image database entry. </summary>
        /// <param name="name">   The image name.</param>
        /// <param name="width">  The image width in meters or 0 if the width is unknown.</param>
        /// <param name="height"> The height of the image in meters.</param>
        public NRTrackingImageDatabaseEntry(string name, float width, float height)
        {
            Name = name;
            TextureGUID = string.Empty;
            Width = width;
            Height = height;
            Quality = string.Empty;
            TextureGUID = string.Empty;
        }

#if UNITY_EDITOR
        
        /// <summary> Contructs a new Augmented Image database entry. </summary>
        /// <param name="name">    The image name.</param>
        /// <param name="texture"> The texture.</param>
        /// <param name="width">   The image width in meters or 0 if the width is unknown.</param>
        /// <param name="height">  The height of the image in meters.</param>
        public NRTrackingImageDatabaseEntry(string name, Texture2D texture, float width, float height)
        {
            Name = name;
            TextureGUID = string.Empty;
            Width = width;
            Quality = string.Empty;
            Height = height;
            Texture = texture;
        }

        /// <summary> Contructs a new Augmented Image database entry. </summary>
        /// <param name="name">    The image name.</param>
        /// <param name="texture"> The texture.</param>
        public NRTrackingImageDatabaseEntry(string name, Texture2D texture)
        {
            Name = name;
            TextureGUID = string.Empty;
            Width = 0;
            Quality = string.Empty;
            Height = 0;
            Texture = texture;
        }

        /// <summary> Contructs a new Augmented Image database entry. </summary>
        /// <param name="texture"> The texture.</param>
        public NRTrackingImageDatabaseEntry(Texture2D texture)
        {
            Name = "Unnamed";
            TextureGUID = string.Empty;
            Width = 0;
            Quality = string.Empty;
            Height = 0;
            Texture = texture;
        }

        /// <summary> Gets or sets the texture. </summary>
        /// <value> The texture. </value>
        public Texture2D Texture
        {
            get
            {
                return AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(TextureGUID));
            }
            set
            {
                string path = AssetDatabase.GetAssetPath(value);
                TextureGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(value));
                var fileName = Path.GetFileName(path);
                Name = fileName.Replace(Path.GetExtension(fileName), string.Empty);
            }
        }

        /// <summary> Convert this object into a string representation. </summary>
        /// <returns> A string that represents this object. </returns>
        public override string ToString()
        {
            return string.Format("Name:{0} Quality:{1}", Name, Quality);
        }
        
#endif
    }

}
