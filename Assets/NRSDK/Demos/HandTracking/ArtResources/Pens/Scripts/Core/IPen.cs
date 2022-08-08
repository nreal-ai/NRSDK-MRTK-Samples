using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPen
{
    bool IsDrawing { get; set; }
    Transform PenTransform { get; }
}
