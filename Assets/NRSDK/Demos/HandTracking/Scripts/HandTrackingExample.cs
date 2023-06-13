using System;
using System.Collections;
using UnityEngine;

namespace NRKernal.NRExamples
{
    public class HandTrackingExample : MonoBehaviour
    {
        public ItemsCollector itemsCollector;
        public HandModelsManager handModelsManager;

        public void StartHandTracking()
        {
            Debug.Log("HandTrackingExample: StartHandTracking");
            NRInput.SetInputSource(InputSourceEnum.Hands);
        }

        public void StopHandTracking()
        {
            Debug.Log("HandTrackingExample: StopHandTracking");
            NRInput.SetInputSource(InputSourceEnum.Controller);
        }

        public void ToggleRaycastMode()
        {
            Debug.Log("HandTrackingExample: ToggleRaycastMode");
            NRInput.RaycastMode = NRInput.RaycastMode == RaycastModeEnum.Gaze ? RaycastModeEnum.Laser : RaycastModeEnum.Gaze;
        }

        public void HideRayFor5Seconds()
        {
            Debug.Log("HandTrackingExample: HideRayFor5Seconds");
            CancelInvoke("ShowRay");
            NRInput.RaycastersActive = false;
            Invoke("ShowRay", 5f);
        }

        public void SwitchHandVisual()
        {
            Debug.Log("HandTrackingExample: SwitchHandVisual");
            handModelsManager.ToggleHandModelsGroup();
        }

        public void ResetItems()
        {
            Debug.LogWarning("HandTrackingExample: ResetItems");
            itemsCollector.ResetItems();
        }

        private void ShowRay()
        {
            NRInput.RaycastersActive = true;
        }
    }
}
