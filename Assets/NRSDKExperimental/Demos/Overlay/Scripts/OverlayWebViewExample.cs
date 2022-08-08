using UnityEngine;

namespace NRKernal.Experimental.NRExamples
{
    public class OverlayWebViewExample : MonoBehaviour
    {
        [SerializeField] WebOverlayItem m_OverlayPrefab;
        private WebOverlayItem m_CurrentItem = null;

        void Awake()
        {
            m_OverlayPrefab.gameObject.SetActive(false);
        }

        void Start()
        {
            Load();
        }

        public void Load()
        {
            if (m_CurrentItem != null)
            {
                m_CurrentItem.Close();
                m_CurrentItem = null;
            }

            m_CurrentItem = GameObject.Instantiate(m_OverlayPrefab, transform);
            m_CurrentItem.Reset(WebOverlayItem.WebState.Normal);
            m_CurrentItem.gameObject.SetActive(true);
        }
    }
}
