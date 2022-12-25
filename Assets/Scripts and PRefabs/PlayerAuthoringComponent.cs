using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct PlayerAuthoringComponent : IComponentData
{
    public Entity Prefab;
}
