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
    private Entity m_Prefab;

    protected override void OnCreate()
    {
        //Entityquery for our players, which must have a PlayerTag
        m_PlayerQuery = GetEntityQuery(ComponentType.ReadWrite<PlayerTag>());

        m_BeginSimECB = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate()
    {
        if (m_Prefab == Entity.Null)
        {
            //Grab the converted Prefab
            m_Prefab = GetSingleton<PlayerAuthoringComponent>().Prefab;

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

        if (shoot == 1 && playerCount < 1)
        {
            EntityManager.Instantiate(m_Prefab); //Here we're using a different method to instantiate stuff than AsteroidSpawnSystem
            return;
        }



        Entities.ForEach((ref Translation translation, in Rotation rotation) => {
        }).Schedule();
    }
}
