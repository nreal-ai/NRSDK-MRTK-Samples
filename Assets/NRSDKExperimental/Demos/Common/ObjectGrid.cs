using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGrid : MonoBehaviour
{
    public Vector2 Spacing;
    public int row;
    public int col;

    [ContextMenu("Range")]
    public void Range()
    {
        if (col == 0 || row == 0)
        {
            return;
        }
        Vector3[] poseList = new Vector3[row * col];
        Vector2 offset_origin = new Vector2(-0.5f * (col - 1) * Spacing.x, 0.5f * (row - 1) * Spacing.y);
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                Vector2 offset = new Vector2(j * Spacing.x, -i * Spacing.y) + offset_origin;
                poseList[i * col + j] = Vector3.up * offset.y + Vector3.right * offset.x;
            }
        }

        for (int index = 0; index < transform.childCount; index++)
        {
            transform.GetChild(index).localPosition = poseList[index];
        }
    }
}
