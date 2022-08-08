using NRKernal;
using UnityEngine;
using UnityEngine.UI;

public class FocusManager : MonoBehaviour
{
    private Transform m_HeadTransfrom;
    private Vector3 m_FocusPosition;
    RaycastHit hitResult;
    private FocusItem currentFocusItem;

    void Start()
    {
        m_HeadTransfrom = NRSessionManager.Instance.CenterCameraAnchor;

        //NRInput.ReticleVisualActive = false;
        //NRInput.LaserVisualActive = false;
    }

    private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
    void Update()
    {
        if (Physics.Raycast(new Ray(m_HeadTransfrom.position, m_HeadTransfrom.forward), out hitResult, 100))
        {
            m_FocusPosition = m_HeadTransfrom.InverseTransformPoint(hitResult.point);

            var item = hitResult.collider.GetComponent<FocusItem>();
            if (item != null && currentFocusItem != item)
            {
                currentFocusItem?.OnOut();

                currentFocusItem = item;
                currentFocusItem.OnEnter();
            }

#if USING_XR_SDK && !UNITY_EDITOR
            // Debug.LogFormat("SetFocusPlane position:{0} normal:{1}", m_FocusPosition.ToString(), hitResult.normal.ToString());
            NRSessionManager.Instance.XRDisplaySubsystem?.SetFocusPlane(m_FocusPosition,hitResult.normal,Vector3.zero);
#else
            NRSessionManager.Instance.NRRenderer?.SetFocusDistance(m_FocusPosition.magnitude);
#endif
        }
        else
        {
            currentFocusItem?.OnOut();
            currentFocusItem = null;
        }

        if (Time.frameCount % 100 == 0 || stopwatch.ElapsedMilliseconds >= 20)
        {
            Debug.Log("time cost a frame:" + stopwatch.ElapsedMilliseconds);
        }
        stopwatch.Reset();
        stopwatch.Start();
    }

    void OnDrawGizmos()
    {
        if (hitResult.collider != null)
        {
            Gizmos.DrawSphere(hitResult.point, 0.1f);
            Gizmos.DrawLine(m_HeadTransfrom.position, hitResult.point);
        }
    }
}
