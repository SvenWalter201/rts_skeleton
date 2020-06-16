using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System;
using UnityEngine.UIElements;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine.Rendering;
using Unity.Rendering;
using Unity.Physics;
using Unity.Physics.Authoring;
using Unity.Collections;

public class Testing : MonoBehaviour
{
    [SerializeField]
    private Mesh mesh = default;
    [SerializeField]
    private UnityEngine.Material moveUnitMaterial = default;
    [SerializeField]
    private UnityEngine.Material physicsUnitMaterial = default;


    private EntityManager manager;
    private EntityArchetype combatActor;
    private EntityArchetype physicsActor;
    private EntityArchetype movingActor;
    private EntityArchetype pathfindingAgent;

    private void Awake()
    {
        manager = World.DefaultGameObjectInjectionWorld.EntityManager;


        InitArchetypes();
        CreateTestEntities();
        CreatePhysicsTestEntities();
        CreatePathfindingAIAgents();
    }

    private void InitArchetypes()
    {
        combatActor = manager.CreateArchetype(
           typeof(Name),
           typeof(HealthManagement),
           typeof(Weapon),
           typeof(Target)
        );
        movingActor = manager.CreateArchetype(
            typeof(Name),
            typeof(LocalToWorld),
            typeof(Translation),
            typeof(MovementParameters),
            typeof(RenderMesh),
            typeof(RenderBounds)
        );
        physicsActor = manager.CreateArchetype(
            typeof(Name),
            typeof(LocalToWorld),
            typeof(Translation),
            typeof(Rotation),
            typeof(RenderMesh),
            typeof(RenderBounds),
            typeof(PhysicsCollider),
            typeof(WorldRenderBounds),
            typeof(PerInstanceCullingTag),
            typeof(PhysicsMass),
            typeof(PhysicsDamping)
     );

        pathfindingAgent = manager.CreateArchetype(
            typeof(AIControlled));
    }

    private void CreateTestEntities()
    {
        Entity e1 = manager.CreateEntity(combatActor);
        manager.SetComponentData(e1, new Name { N = "CombatActor 1" });
        manager.SetComponentData(e1, new HealthManagement { MaxHP = 100f, CurrentHP = 100f });
        manager.SetComponentData(e1, new Weapon { Damage = 50f, AttackSpeed = 2f, TimeUntilNextAttack = 0f });
        Entity e2 = manager.CreateEntity(combatActor);
        manager.SetComponentData(e2, new Name { N = "CombatActor 2" });
        manager.SetComponentData(e2, new HealthManagement { MaxHP = 600f, CurrentHP = 600f });
        manager.SetComponentData(e2, new Weapon { Damage = 30f, AttackSpeed = 0.5f, TimeUntilNextAttack = 0f });

        manager.SetComponentData(e1, new Target { T = e2 });
        manager.SetComponentData(e2, new Target { T = e1 });

        Entity e3 = manager.CreateEntity(movingActor);
        manager.SetComponentData(e3, new Name { N = "MovingActor1" });
        manager.SetComponentData(e3, new Translation { Value = new float3(0, 0, 0) });
        manager.SetComponentData(e3, new MovementParameters { Speed = 10f });
        manager.SetSharedComponentData(e3, new RenderMesh { mesh = mesh, material = moveUnitMaterial });
        manager.AddComponent<Controllable>(e3);
    }

    private void CreatePathfindingAIAgents()
    {
        manager.CreateEntity(pathfindingAgent, 10, Allocator.Temp);
    }

    private void CreatePhysicsTestEntities()
    {
        Entity e1 = manager.CreateEntity(physicsActor);
        manager.SetComponentData(e1, new Name { N = "PhysicsActor1" });
        manager.SetComponentData(e1, new Translation { Value = new float3(0, 0, 0) });
        manager.SetSharedComponentData(e1, new RenderMesh { mesh = mesh, material = physicsUnitMaterial });

        Entity e2 = manager.CreateEntity(physicsActor);
        manager.SetComponentData(e2, new Name { N = "PhysicsActor2" });
        manager.SetComponentData(e2, new Translation { Value = new float3(0, 4, 0) });
        manager.SetSharedComponentData(e2, new RenderMesh { mesh = mesh, material = physicsUnitMaterial });


        //manager.AddComponent<PhysicsVelocity>(e2);
        //manager.SetComponentData(e2, new PhysicsVelocity { Linear = 0.0f, Angular = 0.0f });
    }
}
