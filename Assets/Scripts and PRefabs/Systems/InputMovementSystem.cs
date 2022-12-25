using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class InputMovementSystem : SystemBase
{

    protected override void OnCreate()
    {
        //We need the Gamesettings component
        RequireSingletonForUpdate<GameSettingsComponent>(); 
    }
    protected override void OnUpdate()
    {
        var gameSettings = GetSingleton<GameSettingsComponent>();
        var deltaTime = Time.DeltaTime;

        //WASD
        byte right, left, thrust, reverseThrust;
        right = left = thrust = reverseThrust = 0; //I love this syntax

        float mouseX = 0;
        float mouseY = 0;

        //we grab "WASD" for thrusting
        if (Input.GetKey("d"))
        {
            right = 1;
        }
        if (Input.GetKey("a"))
        {
            left = 1;
        }
        if (Input.GetKey("w"))
        {
            thrust = 1;
        }
        if (Input.GetKey("s"))
        {
            reverseThrust = 1;
        }

        if (Input.GetMouseButton(1))
        {
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");

        }

        Entities.WithAll<PlayerTag>().ForEach((Entity entity, ref Rotation rotation, ref VelocityComponent velocity) => {

            //This bit could be done more elegantly. Input is also not normalized
            //Later I might add spacebar control similar to Outer Wild's ship controls.
            float horizontalInput = 0;
            if (right == 1)
            {
                horizontalInput += 1;
            }
            if (left == 1)
            {
                horizontalInput -= 1;
            }

            float verticalInput = 0;
            if (thrust == 1)
            {
                verticalInput += 1;
            }
            if (reverseThrust == 1)
            {
                verticalInput -= 1;
            }

            velocity.Value += (math.mul(rotation.Value, new float3(0, 0, 1)).xyz) * verticalInput * gameSettings.playerForce * deltaTime;
            velocity.Value += (math.mul(rotation.Value, new float3(1, 0, 0)).xyz) * horizontalInput * gameSettings.playerForce * deltaTime;

            //Friction
            velocity.Value = Vector3.MoveTowards(velocity.Value, new float3(0,0,0), gameSettings.playerFriction * deltaTime);
            

            if (mouseX != 0 || mouseY != 0)
            {   //move the mouse
                float lookSpeedH = 2f;
                float lookSpeedV = 2f;

                
                Quaternion currentQuaternion = rotation.Value;
                float yaw = currentQuaternion.eulerAngles.y;
                float pitch = currentQuaternion.eulerAngles.x;

                //MOVING WITH MOUSE
                yaw += lookSpeedH * mouseX;
                pitch -= lookSpeedV * mouseY;
                Quaternion newQuaternion = Quaternion.identity;
                newQuaternion.eulerAngles = new Vector3(pitch, yaw, 0);
                rotation.Value = newQuaternion;
            }
        }).ScheduleParallel();
    }
}
