using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

//We are going to update late once all other systems are complete, because
//wer dont wan to destroy the Entity before other systems have had to 
//interact with it
[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public partial class AsteroidDestroyerSystem : SystemBase
{

    private EndSimulationEntityCommandBufferSystem m_EndSimECB;

    protected override void OnCreate()
    {
        m_EndSimECB = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate()
    {
        //In order to run it pararell
        var commandBuffer = m_EndSimECB.CreateCommandBuffer().AsParallelWriter();

        Entities
        .WithAll<DestroyTag, AsteroidTag>()
        .ForEach((Entity entity, int entityInQueryIndex) =>
        {
            commandBuffer.DestroyEntity(entityInQueryIndex, entity);
        }).ScheduleParallel();

        //Then add dependencies for those jobs for the EndSimulation whatever BufferSystem to record structural changes.
        m_EndSimECB.AddJobHandleForProducer(Dependency);
    }
}
