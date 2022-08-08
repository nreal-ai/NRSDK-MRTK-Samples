/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.Experimental
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    public class OverlayBase : MonoBehaviour, IComparable<OverlayBase>
    {
        /// <summary>
        /// The compositionDepth defines the order of the NROverlays in composition. The overlay with smaller compositionDepth would be composited in the front of the overlay with larger compositionDepth.
        /// </summary>
        [Tooltip("The compositionDepth defines the order of the NROverlays in composition. The overlay with smaller compositionDepth would be composited in the front of the overlay with larger compositionDepth.")]
        public int compositionDepth = 0;
        [Tooltip("Whether active this overlay when script start")]
        public bool ActiveOnStart = true;
        private bool alreadyAddedToSwapChain = false;
        public Dictionary<IntPtr, Texture> Textures = new Dictionary<IntPtr, Texture>();
        protected BufferSpec m_BufferSpec;
        protected ViewPort[] m_ViewPorts;
        protected int m_LayerId = 0;
        private bool m_IsDirty = false;
        private bool m_IsActive = true;

        public ViewPort[] ViewPorts
        {
            get
            {
                return m_ViewPorts;
            }
        }

        public BufferSpec BufferSpec
        {
            get { return m_BufferSpec; }
            set
            {
                m_BufferSpec.Copy(value);
            }
        }

        public int LayerId
        {
            get
            {
                return m_LayerId;
            }
            set
            {
                m_LayerId = value;
            }
        }

        public bool IsActive
        {
            get { return m_IsActive; }
            private set { m_IsActive = value; }
        }

        public UInt64 NativeSpecHandler { get; set; }

        public int CompareTo(OverlayBase that)
        {
            return this.compositionDepth.CompareTo(that.compositionDepth);
        }

        public virtual Texture GetTexturePtr()
        {
            return null;
        }

        protected void SetDirty(bool value)
        {
            m_IsDirty = m_IsDirty == true ? true : value;
        }

        void OnEnable()
        {
            IsActive = true;
        }

        void OnDisable()
        {
            IsActive = false;
        }

        protected void Start()
        {
            if (ActiveOnStart)
            {
                InitAndActive();
            }
        }

        public void InitAndActive()
        {
            if (!alreadyAddedToSwapChain)
            {
                Initialize();
                NRSwapChainManager.Instance.Add(this);
                alreadyAddedToSwapChain = true;
            }
        }

        void Update()
        {
            if (m_IsDirty && m_ViewPorts != null)
            {
                DestroyViewPort();
                CreateViewport();
                m_IsDirty = false;
            }
        }

        public virtual void Destroy()
        {
            if (alreadyAddedToSwapChain)
            {
                NRSwapChainManager.Instance.Remove(this);
                alreadyAddedToSwapChain = false;
            }
        }

        protected void OnDestroy()
        {
            this.Destroy();
        }

        protected virtual void Initialize() { }

        public virtual void CreateOverlayTextures() { }

        public virtual void ReleaseOverlayTextures() { }

        public virtual void CreateViewport() { }

        public virtual void UpdateViewPort() { }

        public virtual void DestroyViewPort() { }

        /// <summary> Just for display overlay. </summary>
        public virtual void SwapBuffers(IntPtr bufferHandler) { }

        public override string ToString()
        {
            if (ViewPorts.Length == 1)
            {
                return string.Format("LayerId:{0}, go:{1}, depth:{2}, viewIndex:{3}, BufferSpec:{4}", m_LayerId, gameObject.name, compositionDepth, ViewPorts[0].index, m_BufferSpec.ToString());
            }
            else if (ViewPorts.Length == 2)
            {
                return string.Format("LayerId:{0}, go:{1}, depth:{2}, viewIndex:{3}_{4}, BufferSpec:{5}", m_LayerId, gameObject.name, compositionDepth, ViewPorts[0].index, ViewPorts[1].index, m_BufferSpec.ToString());
            }
            return string.Empty;
        }
    }
}
