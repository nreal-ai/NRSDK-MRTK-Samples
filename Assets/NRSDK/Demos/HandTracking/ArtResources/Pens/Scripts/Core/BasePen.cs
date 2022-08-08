using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePen : MonoBehaviour, IPen
{
    public bool IsDrawing { get; set; }
    public Transform PenTransform { get { return transform; } }
}
