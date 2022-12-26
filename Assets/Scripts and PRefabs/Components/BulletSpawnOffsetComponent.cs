using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct BulletSpawnOffsetComponent : IComponentData
{
    public float3 Value;
}