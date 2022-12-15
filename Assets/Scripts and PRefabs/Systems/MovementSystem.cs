using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class MovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime; //Stack abuse- I mean, stack-utilization!

        //Theres no Structural change so in comparison to AsteroidSpawnSystem it's a lot simpler.
        //This queries all entities with a Translation and a Velocity component. 
        //Foreach and WithCode are automatically Burst compiled here. yipee- but sometimes you want to not use burst with .WithoutBurst() when using some C# features. Crazy
        Entities.ForEach((ref Translation position, in VelocityComponent velocity) => {
            position.Value.xyz += velocity.Value * deltaTime;
        }).ScheduleParallel();
    }
}
