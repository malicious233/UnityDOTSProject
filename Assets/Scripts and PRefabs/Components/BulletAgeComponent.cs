using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct BulletAgeComponent : IComponentData
{
    public float age;
    public float maxAge;
    public BulletAgeComponent(float maxAge)
    {
        this.maxAge = maxAge;
        age = 0;
    }
}
