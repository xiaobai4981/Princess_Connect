using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicCircleRotation : MonoBehaviour
{
    public Transform outerCircle;
    public Transform innerCircle;

    public float outerSpeed = 30f;   // 顺时针
    public float innerSpeed = -45f;  // 逆时针

    void Update()
    {
        // 2D旋转只需要操作Z轴
        if (outerCircle != null)
        {
            outerCircle.Rotate(0f, 0f, outerSpeed * Time.deltaTime);
        }

        if (innerCircle != null)
        {
            innerCircle.Rotate(0f, 0f, innerSpeed * Time.deltaTime);
        }
    }
}
