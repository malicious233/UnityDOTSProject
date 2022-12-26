using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class SetBulletSpawnOffset : MonoBehaviour, IConvertGameObjectToEntity
{
    public GameObject bulletSpawnPoint;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var bulletOffset = default(BulletSpawnOffsetComponent);

        var offsetVector = bulletSpawnPoint.transform.position;
        bulletOffset.Value = new float3(offsetVector.x, offsetVector.y, offsetVector.z);

        dstManager.AddComponentData(entity, bulletOffset);
        
    }
}
