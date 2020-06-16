//#define LOG


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;




public class TransferDamageDataSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, ref Name name, ref Target target, ref Weapon weapon) =>
        {
            if (!EntityManager.Exists(target.T))
            {
                EntityManager.RemoveComponent<Target>(entity);
            }
            else if (weapon.TimeUntilNextAttack <= 0f)
            {
                weapon.TimeUntilNextAttack = 1.0f/weapon.AttackSpeed;
#if LOG
                Debug.Log(target.T.Index + " was targeted by " + name.N);
#endif
                EntityManager.AddComponent<Damage>(target.T);
                EntityManager.SetComponentData(target.T, new Damage { Amount = weapon.Damage });
            }
            else
            {
                weapon.TimeUntilNextAttack -= Time.DeltaTime;
            }
        });
    }
}

public class TakeDamageSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, ref Name name, ref Damage damage, ref HealthManagement health) => {
            health.CurrentHP -= damage.Amount;
#if LOG
            Debug.Log(name.N + " took " + damage.Amount + "Damage.");
            Debug.Log(name.N + " is now at " + health.CurrentHP + " health.");
#endif
            EntityManager.RemoveComponent<Damage>(entity);
            if(health.CurrentHP <= 0f)
            {
                EntityManager.AddComponent<Dead>(entity);
            }
        });
    }
}

public class CleanupDeadSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, ref Name name, ref Dead dead) =>
        {
            Debug.Log(name.N + " is Dead.");
            EntityManager.DestroyEntity(entity);
        });
    }
}
