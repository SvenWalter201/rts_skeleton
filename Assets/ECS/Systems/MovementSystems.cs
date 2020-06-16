using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class ControlledMovementSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Controllable controllable, ref MovementParameters movementparams, ref Translation position) => {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");
            
            position.Value.x += horizontalInput * movementparams.Speed * Time.DeltaTime;
            position.Value.y += verticalInput * movementparams.Speed * Time.DeltaTime;
        });
     }
}

public class PathfollowSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        //run on all entites with a pathFollow component, modifying their position
    }
}

