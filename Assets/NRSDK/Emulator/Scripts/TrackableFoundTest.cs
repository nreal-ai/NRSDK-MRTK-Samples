using NRKernal;
using UnityEngine;

/// <summary> A trackable found test. </summary>
public class TrackableFoundTest : MonoBehaviour
{
    /// <summary> The observer. </summary>
    public TrackableObserver Observer;
    /// <summary> The object. </summary>
    public GameObject Obj;

    /// <summary> Starts this object. </summary>
    void Start()
    {
#if !UNITY_EDITOR
        Destroy(GameObject.Find("EmulatorRoom"));
#endif
        Obj.SetActive(false);
        Observer.FoundEvent += Found;
        Observer.LostEvent += Lost;
    }

    /// <summary> Founds. </summary>
    /// <param name="pos"> The position.</param>
    /// <param name="qua"> The qua.</param>
    private void Found(Vector3 pos, Quaternion qua)
    {
        Obj.transform.position = pos;
        Obj.transform.rotation = qua;
        Obj.SetActive(true);
    }

    /// <summary> Losts this object. </summary>
    private void Lost()
    {
        Obj.SetActive(false);
    }
}
