using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class SetGameSettingsSystem : MonoBehaviour, IConvertGameObjectToEntity
{
    //My bad, this shouldnt be called a System

    public float asteroidVelocity = 10f;
    public float playerForce = 50f;
    public float playerFriction = 50f;
    public float bulletVelocity = 500f;
    public float bulletFireRate = 50;

    public int numAsteroids = 200;
    public int levelWidth = 2048;
    public int levelHeight = 2048;
    public int levelDepth = 2048;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        // Call methods on 'dstManager' to create runtime components on 'entity' here. Remember that:
        //
        // * You can add more than one component to the entity. It's also OK to not add any at all.
        //
        // * If you want to create more than one entity from the data in this class, use the 'conversionSystem'
        //   to do it, instead of adding entities through 'dstManager' directly.
        //
        // For example,
        //   dstManager.AddComponentData(entity, new Unity.Transforms.Scale { Value = scale });
        var settings = default(GameSettingsComponent);
        settings.asteroidVelocity = asteroidVelocity;
        settings.playerForce = playerForce;
        settings.playerFriction = playerFriction;
        settings.bulletVelocity = bulletVelocity;
        settings.bulletFireRate = bulletFireRate;

        settings.numAsteroids = numAsteroids;
        settings.levelWidth = levelWidth;
        settings.levelHeight = levelHeight;
        settings.levelDepth = levelDepth;
        dstManager.AddComponentData(entity, settings);
        
    }
}
