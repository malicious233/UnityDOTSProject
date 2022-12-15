
using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class AsteroidSpawnSystem : SystemBase
{

    //The query for our asteroids
    private EntityQuery m_AsteroidQuery;

    //For our structural change
    private BeginSimulationEntityCommandBufferSystem m_BeginSimECB;

    //Query to find GameSettingsComponent
    private EntityQuery m_GameSettingsQuery;

    //This will save our asteroid prefab to be used to spawn asteroids
    private Entity m_Prefab;

    protected override void OnCreate()
    {
        //This is an entityquery for our asteroids, they must have an AsteroidTag. A bit like uhh.. a get Monobehavior component????
        m_AsteroidQuery = GetEntityQuery(ComponentType.ReadWrite<AsteroidTag>());

        m_GameSettingsQuery = GetEntityQuery(ComponentType.ReadWrite<GameSettingsComponent>());

        //this grabs the System to be used in OnUpdate
        m_BeginSimECB = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();

        //This says "do not go to OnUpdate until an entity that meets this query"
        //With the gameobject conversion we need to make sure the conversion is complete before continuing
        RequireForUpdate(m_GameSettingsQuery);
        //I wonder what happens if we dont write this.
    }

    protected override void OnUpdate()
    {
        #region Unity generated comment
        // Assign values to local variables captured in your job here, so that it has
        // everything it needs to do its work when it runs later.
        // For example,
        //     float deltaTime = Time.DeltaTime;

        // This declares a new kind of job, which is a unit of work to do.
        // The job is declared as an Entities.ForEach with the target components as parameters,
        // meaning it will process all entities in the world that have both
        // Translation and Rotation components. Change it to process the component
        // types you want.
        #endregion

        if (m_Prefab == Entity.Null)
        {
            //We get the converted PrefabCollection entitys Authoringcomponent and get the prefab value.
            m_Prefab = GetSingleton<AsteroidAuthoringComponent>().Prefab;

            //Seemingly we need to return here for this Update because the variable was just set. ECS amirite.
            return;
        }

        //ECS wants us to declare all the local variables for within the job (Probably optimizing for the stack) as you cannto call this inside of the Job itself

        var settings = GetSingleton<GameSettingsComponent>();

        //We create our commandBuffer to 'record' our structural changes... sure, I'll go with that.
        var commandBuffer = m_BeginSimECB.CreateCommandBuffer();

        //Fetches current amount of asteroids in the EntityQuery (Why is the function named like this)
        var count = m_AsteroidQuery.CalculateChunkCountWithoutFiltering();

        //This has to be a local variable. Lovely boilerplate
        var asteroidPrefab = m_Prefab;

        //We will use this to generate random positions
        var rand = new Unity.Mathematics.Random((uint)Stopwatch.GetTimestamp());

        Job.WithCode(() =>
        {
            for (int i = count; i < settings.numAsteroids; ++i)
            {
                var padding = 0.1f;

                var xPosition = rand.NextFloat(-1f * ((settings.levelWidth) / 2 - padding), (settings.levelWidth) / 2 - padding);
                // so the y value must be from negative levelHeight/2 to positive levelHeight/2 (within padding)
                var yPosition = rand.NextFloat(-1f * ((settings.levelHeight) / 2 - padding), (settings.levelHeight) / 2 - padding);
                // so the z value must be from negative levelDepth/2 to positive levelDepth/2 (within padding)
                var zPosition = rand.NextFloat(-1f * ((settings.levelDepth) / 2 - padding), (settings.levelDepth) / 2 - padding);

                var chooseFace = rand.NextFloat(0, 6);

                //Spawn at the edges of the cube
                if (chooseFace < 1) { xPosition = -1 * ((settings.levelWidth) / 2 - padding); }
                else if (chooseFace < 2) { xPosition = (settings.levelWidth) / 2 - padding; }
                else if (chooseFace < 3) { yPosition = -1 * ((settings.levelHeight) / 2 - padding); }
                else if (chooseFace < 4) { yPosition = (settings.levelHeight) / 2 - padding; }
                else if (chooseFace < 5) { zPosition = -1 * ((settings.levelDepth) / 2 - padding); }
                else if (chooseFace < 6) { zPosition = (settings.levelDepth) / 2 - padding; }

                Translation pos = new Translation { Value = new float3(xPosition, yPosition, zPosition) };

                //On our command buffer we record that we created an entity from our asteroidprefab
                Entity e = commandBuffer.Instantiate(asteroidPrefab);

                //We set the Translationcomponent of the asteroidprefab equal to our new Translation. 
                commandBuffer.SetComponent(e, pos);

                //Generate a random normalized Vector3. Wait why are we making a Vector3? not float3 like the rest?
                var randomVel = new Vector3(rand.NextFloat(-1, 1), rand.NextFloat(-1, 1), rand.NextFloat(-1, 1));
                randomVel.Normalize();

                //Set it equal to the game settings
                randomVel = randomVel * settings.asteroidVelocity;

                //Create new VelocityComponent with the velocity data.
                var vel = new VelocityComponent { Value = new float3(randomVel.x, randomVel.y, randomVel.z) };

                //Set the velocity component to the one in our prefab.
                //SetComponent does seemingly not need to know if the component has a VelocityComponent or not. A bit like Monobehavior Getcomponent then??
                commandBuffer.SetComponent(e, vel);

            }
        }).Schedule();

        //This will add our dependency to be played back on the BeginSimulationEntityCommandBuffer... what is a Dependency in this context????
        m_BeginSimECB.AddJobHandleForProducer(Dependency);

        /*
        Entities.ForEach((ref Translation translation, in Rotation rotation) => {
            #region Unity generated comment
            // Implement the work to perform for each entity here.
            // You should only access data that is local or that is a
            // field on this job. Note that the 'rotation' parameter is
            // marked as 'in', which means it cannot be modified,
            // but allows this job to run in parallel with other jobs
            // that want to read Rotation component data.
            // For example,
            //     translation.Value += math.mul(rotation.Value, new float3(0, 0, 1)) * deltaTime;
            #endregion

        }).Schedule();
        */
    }
}
