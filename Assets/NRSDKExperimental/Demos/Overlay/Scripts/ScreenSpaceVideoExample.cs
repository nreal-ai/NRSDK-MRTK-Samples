using NRKernal.Experimental;
using UnityEngine;
using UnityEngine.Video;

namespace NRKernal.Experimental.NRExamples
{
    /// <summary>
    /// overlay ScreenSpace Videoplayer Example
    /// </summary>
    public class ScreenSpaceVideoExample : MonoBehaviour
    {
        [SerializeField]
        private NROverlay m_NROverlay;
        [SerializeField]
        private VideoPlayer m_Player;

        private RenderTexture m_RenderTexture;

        // Start is called before the first frame update
        void Start()
        {
            if (m_Player.clip == null)
            {
                Debug.Log("[ScreenSpaceVideoExample] Start: VideoClip is null");
                return;
            }

            int width = (int)m_Player.clip.width;
            int height = (int)m_Player.clip.height;

            m_RenderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);

            m_Player.targetTexture = m_RenderTexture;

            m_NROverlay.MainTexture = m_RenderTexture;
        }
    }
}