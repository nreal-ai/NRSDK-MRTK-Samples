using UnityEngine;

namespace NRKernal.Experimental.NRExamples
{
    /// <summary> A render standlone. </summary>
    public class RenderStandlone : MonoBehaviour
    {
        /// <summary> The left. </summary>
        public Camera left;
        /// <summary> The right. </summary>
        public Camera right;

        /// <summary> The nr renderer. </summary>
        private NRRenderer m_NRRenderer;

        /// <summary> Starts this object. </summary>
        void Start()
        {
            m_NRRenderer = gameObject.AddComponent<NRRenderer>();
            m_NRRenderer.Initialize(left, right);
        }
    }
}
