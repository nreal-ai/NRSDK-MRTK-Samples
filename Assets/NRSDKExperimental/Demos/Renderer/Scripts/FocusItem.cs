using UnityEngine;

public class FocusItem : MonoBehaviour
{
    public GameObject FrameObj;

    public void OnEnter()
    {
        FrameObj.SetActive(true);
    }

    public void OnOut()
    {
        FrameObj.SetActive(false);
    }
}
