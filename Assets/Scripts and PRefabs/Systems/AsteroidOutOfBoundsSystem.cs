using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

//We're adding this system within FixedStepSimulationGroup, to utilize Physics?
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(EndFixedStepSimulationEntityCommandBufferSystem))]
public partial class AsteroidOutOfBoundsSystem : SystemBase
{
    
    private EndFixedStepSimulationEntityCommandBufferSystem m_EndFixedStepSimECB;

    protected override void OnCreate()
    {
        //Grab the EndFixedStepSim for our OnUpdate
        m_EndFixedStepSimECB = World.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();

        //Make sure to not update until we have our Component.
        RequireSingletonForUpdate<GameSettingsComponent>();
    }
    protected override void OnUpdate()
    {
        //We want to run this pararell so we need to call this for our CommandBuffer.
        var commandBuffer = m_EndFixedStepSimECB.CreateCommandBuffer().AsParallelWriter();

        var settings = GetSingleton<GameSettingsComponent>();

        //EntityinQueryIndex is needed for pararell jobs to work
        //We'll only be affecting Asteroids for this. Perhaps we could use a different Tag such as "OutOfBoundTag" or something so it is reusable.
        Entities.WithAll<AsteroidTag>().ForEach((Entity entity, int entityInQueryIndex, in Translation position) => 
        {
            //We check if the current Translation value is out of bounds
            if (Mathf.Abs(position.Value.x) > settings.levelWidth / 2 ||
                Mathf.Abs(position.Value.y) > settings.levelHeight / 2 ||
                Mathf.Abs(position.Value.z) > settings.levelDepth / 2)
            {
                //Add DestroyTag component to entity, destruction will be handled by a seperate system.
                commandBuffer.AddComponent(entityInQueryIndex, entity, new DestroyTag());
                return;
            }
        }).ScheduleParallel();

        m_EndFixedStepSimECB.AddJobHandleForProducer(Dependency);
    }

    //We are placing this System in a specific SystemGroup, the FixedStepSimulationGroup, because we want to-
    //-update it before EndFixedStepSimulationEntityCommandBuffer (jesus fucking christ that name is so long) because the latter is where
    //recorded structural changes will playback.
}
