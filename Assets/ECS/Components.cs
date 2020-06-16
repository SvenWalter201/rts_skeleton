using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;

public struct Name : IComponentData
{
    public NativeString32 N { get; set; }
}
public struct HealthManagement : IComponentData
{
    public float MaxHP { get; set; }
    public float CurrentHP { get; set; }

}

public struct Target : IComponentData
{
    public Entity T { get; set; }
}

public struct Weapon : IComponentData
{
    public float Damage { get; set; }
    public float AttackSpeed { get; set; }
    public float TimeUntilNextAttack { get; set; }
}

public struct Damage : IComponentData
{
    public float Amount { get; set; }
}

public struct Dead : IComponentData { }

public struct Controllable : IComponentData
{
    public bool Impaired { get; set; }
}

public struct AIControlled : IComponentData { }

public struct MovementParameters : IComponentData
{
    public float Speed { get; set; }
}