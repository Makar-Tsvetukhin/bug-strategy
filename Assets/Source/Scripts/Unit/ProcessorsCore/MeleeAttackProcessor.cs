﻿using System;
using UnityEngine;

namespace Unit.ProcessorsCore
{
    public class MeleeAttackProcessor : AttackProcessorBase
    {
        public override event Action Attacked;
        public override event Action<IUnitTarget> TargetAttacked;

        public MeleeAttackProcessor(UnitBase unit, float attackRange, float damage, CooldownProcessor cooldownProcessor) 
            : base(unit, attackRange, damage, cooldownProcessor)
        { }
        
        protected override void Attack(IUnitTarget target)
        {
            if (target.TryCast(out IDamagable damageable))
            {
                damageable.TakeDamage(this);
                Attacked?.Invoke();
                TargetAttacked?.Invoke(target);
            }
#if UNITY_EDITOR
            else
                Debug.LogWarning($"Target {target} can't be attacked");
#endif
        }
    }
}