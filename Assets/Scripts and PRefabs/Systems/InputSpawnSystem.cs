using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class InputSpawnSystem : SystemBase
{
    //Our player query
    private EntityQuery m_PlayerQuery;

    //For our structural changes
    private BeginSimulationEntityCommandBufferSystem m_BeginSimECB;
    
    //Save our player prefab
    private Entity m_PlayerPrefab;
    private Entity m_BulletPrefab;

    private float m_PerSecond = 5f;
    private float m_NextTime = 0;

    protected override void OnCreate()
    {
        //Entityquery for our players, which must have a PlayerTag
        m_PlayerQuery = GetEntityQuery(ComponentType.ReadWrite<PlayerTag>());

        m_BeginSimECB = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<GameSettingsComponent>();
    }
    protected override void OnUpdate()
    {
        if (m_PlayerPrefab == Entity.Null || m_BulletPrefab == Entity.Null)
        {
            //Grab the converted Prefabs
            m_PlayerPrefab = GetSingleton<PlayerAuthoringComponent>().Prefab;
            m_BulletPrefab = GetSingleton<BulletAuthoringComponent>().Prefab;

            //We must return here or else we'd run into errors. Silly ECS.
            return;
        }
        byte shoot;
        shoot = 0;
        var playerCount = m_PlayerQuery.CalculateChunkCountWithoutFiltering();

        if (Input.GetKey("space"))
        {
            shoot = 1;
        }

        //Space spawns one player only
        if (shoot == 1 && playerCount < 1)
        {
            EntityManager.Instantiate(m_PlayerPrefab); //Here we're using a different method to instantiate stuff than AsteroidSpawnSystem
            return;
        }

        var commandBuffer = m_BeginSimECB.CreateCommandBuffer().AsParallelWriter();

        var gameSettings = GetSingleton<GameSettingsComponent>();
        var bulletPrefab = m_BulletPrefab;

        var canShoot = false;
        if (UnityEngine.Time.time >= m_NextTime)
        {
            canShoot = true;
            m_NextTime += (1 / gameSettings.bulletFireRate);
        }



        Entities.WithAll<PlayerTag>().ForEach((
            Entity entity, int entityInQueryIndex, in Translation position, in Rotation rotation, in VelocityComponent velocity, in BulletSpawnOffsetComponent bulletOffset) =>
    {
        //If we arent holding spacebar we dont have to do anything
        if (shoot != 1 || !canShoot)
        {
            return;
        }

        //Instantiate our bullet
        var bulletEntity = commandBuffer.Instantiate(entityInQueryIndex, bulletPrefab);

        //we set the bullets position as the player's position + the bullet spawn offset
        //math.mul(rotation.Value,bulletOffset.Value) finds the position of the bullet offset in the given rotation
        //think of it as finding the LocalToParent of the bullet offset (because the offset needs to be rotated in the players direction)
        var newPosition = new Translation { Value = position.Value};
        commandBuffer.SetComponent(entityInQueryIndex, bulletEntity, newPosition);


        // bulletVelocity * math.mul(rotation.Value, new float3(0,0,1)).xyz) takes linear direction of where facing and multiplies by velocity
        // adding to the players physics Velocity makes sure that it takes into account the already existing player velocity (so if shoot backwards while moving forwards it stays in place)
        var vel = new VelocityComponent { Value = (gameSettings.bulletVelocity * math.mul(rotation.Value, new float3(0, 0, 1)).xyz) + velocity.Value };

        commandBuffer.SetComponent(entityInQueryIndex, bulletEntity, vel);
    }).ScheduleParallel();
        m_BeginSimECB.AddJobHandleForProducer(Dependency);
    }
}
