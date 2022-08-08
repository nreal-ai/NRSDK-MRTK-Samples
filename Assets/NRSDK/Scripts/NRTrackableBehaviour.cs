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
    using UnityEngine;

    /// <summary> Base classes for all trackable monobehaviour objects. </summary>
    public class NRTrackableBehaviour : MonoBehaviour
    {
        /// <summary> The trackable. </summary>
        public NRTrackable Trackable;

        /// <summary> Initializes this object. </summary>
        /// <param name="trackable"> The trackable.</param>
        public void Initialize(NRTrackable trackable)
        {
            Trackable = trackable;
        }

        /// <summary> Name of the trackable. </summary>
        [HideInInspector, SerializeField]
        protected string m_TrackableName = "";

        /// <summary> True to preserve child size. </summary>
        [HideInInspector, SerializeField]
        protected bool m_PreserveChildSize;

        /// <summary> True to initialized in editor. </summary>
        [HideInInspector, SerializeField]
        protected bool m_InitializedInEditor;

        /// <summary> Zero-based index of the database. </summary>
        [HideInInspector, SerializeField]
        protected int m_DatabaseIndex = -1;

        /// <summary> Gets or sets the name of the trackable. </summary>
        /// <value> The name of the trackable. </value>
        public string TrackableName
        {
            get
            {
                return m_TrackableName;
            }
            set
            {
                m_TrackableName = value;
            }
        }

        /// <summary> Gets or sets a value indicating whether the preserve child size. </summary>
        /// <value> True if preserve child size, false if not. </value>
        public bool PreserveChildSize
        {
            get
            {
                return m_PreserveChildSize;
            }
            set
            {
                m_PreserveChildSize = value;
            }
        }

        /// <summary> Gets or sets a value indicating whether the initialized in editor. </summary>
        /// <value> True if initialized in editor, false if not. </value>
        public bool InitializedInEditor
        {
            get
            {
                return m_InitializedInEditor;
            }
            set
            {
                m_InitializedInEditor = value;
            }
        }

        /// <summary> Gets or sets the zero-based index of the database. </summary>
        /// <value> The database index. </value>
        public int DatabaseIndex
        {
            get
            {
                return m_DatabaseIndex;
            }
            set
            {
                m_DatabaseIndex = value;
            }
        }
    }
}
