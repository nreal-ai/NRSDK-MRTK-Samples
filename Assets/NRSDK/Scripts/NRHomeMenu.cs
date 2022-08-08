using UnityEngine;
using UnityEngine.UI;

namespace NRKernal.NRExamples
{
    /// <summary> A nr home menu. </summary>
    public class NRHomeMenu : MonoBehaviour
    {
        /// <summary> The confirm control. </summary>
        public Button confirmBtn;

        /// <summary> The cancel control. </summary>
        public Button cancelBtn;

        /// <summary> The instance. </summary>
        private static NRHomeMenu m_Instance;

        /// <summary> Full pathname of the menu prefab file. </summary>
        private static string m_MenuPrefabPath = "NRUI/NRHomeMenu";

        /// <summary> Transform of center camera. </summary>
        private Transform CameraCenter { get { return NRInput.CameraCenter; } }

        /// <summary> True if is showing, false if not. </summary>
        public static bool IsShowing { get; private set; }

        /// <summary> Action to excute on home menu show. </summary>
        public static System.Action OnHomeMenuShow;

        /// <summary> Action to excute on home menu hide. </summary>
        public static System.Action OnHomeMenuHide;

        /// <summary> Starts this object. </summary>
        void Start()
        {
            confirmBtn.onClick.AddListener(OnComfirmButtonClick);
            cancelBtn.onClick.AddListener(OnCancelButtonClick);
        }

        /// <summary> Updates this object. </summary>
        void Update()
        {
            if (IsShowing && NRInput.RaycastMode == RaycastModeEnum.Laser)
            {
                FollowCamera();
            }
        }

        /// <summary> Executes the 'comfirm button click' action. </summary>
        private void OnComfirmButtonClick()
        {
            Hide();
            AppManager.QuitApplication();
        }

        /// <summary> Executes the 'cancel button click' action. </summary>
        private void OnCancelButtonClick()
        {
            Hide();
        }

        /// <summary> Follow camera. </summary>
        private void FollowCamera()
        {
            if (m_Instance && CameraCenter)
            {
                m_Instance.transform.position = CameraCenter.position;
                m_Instance.transform.rotation = CameraCenter.rotation;
            }
        }

        /// <summary> Creates the menu. </summary>
        private static void CreateMenu()
        {
            GameObject menuPrefab = Resources.Load<GameObject>(m_MenuPrefabPath);
            if (menuPrefab == null)
            {
                NRDebugger.Error("Can not find prefab: " + m_MenuPrefabPath);
                return;
            }
            GameObject menuGo = Instantiate(menuPrefab);
            m_Instance = menuGo.GetComponent<NRHomeMenu>();
            if (m_Instance)
            {
                DontDestroyOnLoad(menuGo);
            }
            else
            {
                Destroy(menuGo);
            }
        }

        /// <summary> Toggles this object. </summary>
        public static void Toggle()
        {
            if (IsShowing)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        /// <summary> Shows this object. </summary>
        public static void Show()
        {
            if (m_Instance == null)
            {
                CreateMenu();
            }

            if (m_Instance)
            {
                m_Instance.gameObject.SetActive(true);
                IsShowing = true;
                if (NRInput.RaycastMode == RaycastModeEnum.Gaze)
                {
                    m_Instance.FollowCamera();
                }
                if (OnHomeMenuShow != null)
                {
                    OnHomeMenuShow();
                }
            }
        }

        /// <summary> Hides this object. </summary>
        public static void Hide()
        {
            if (m_Instance)
            {
                m_Instance.gameObject.SetActive(false);
                IsShowing = false;
                if (OnHomeMenuHide != null)
                {
                    OnHomeMenuHide();
                }
            }
        }
    }
}
