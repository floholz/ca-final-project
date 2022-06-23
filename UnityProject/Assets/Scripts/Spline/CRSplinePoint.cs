using UnityEngine;
using Unity.Mathematics;

public class CRSplinePoint
{
    public float duration;
    public float distance;
    public float3 pos;

    public CRSplinePoint(float3 position, float distance, float duration = 0)
    {
        this.pos = position;
        this.distance = distance;
        this.duration = duration;
    }
}
