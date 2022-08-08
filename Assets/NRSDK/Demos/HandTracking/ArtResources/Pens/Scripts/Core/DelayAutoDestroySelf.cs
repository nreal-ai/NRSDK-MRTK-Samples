using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayAutoDestroySelf : MonoBehaviour
{
    public bool triggerDestroyOnAwake;
    public float delaySeconds = 5f;

    private bool m_HasTriggedDestroy;

    void Awake()
    {
        if (triggerDestroyOnAwake)
        {
            DestroySelfWithDelay(delaySeconds);
        }
    }

    public void DestroySelfWithDelay(float delayTime)
    {
        if (m_HasTriggedDestroy)
            return;
        Invoke("DestroySelf", delayTime);
        m_HasTriggedDestroy = true;
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
