﻿using System;
using Projectiles;
using Projectiles.Factory;
using UnityEngine;

namespace Unit.ProcessorsCore
{
    public class RangeAttackProcessor : AttackProcessorBase
    {
        private readonly ProjectileType _projectileType;
        private readonly ProjectileFactory _projectilesFactory;
        
        public override event Action Attacked;
        public override event Action<IUnitTarget> TargetAttacked;
        public event Action<ProjectileBase> ProjectileSpawned; 
        
        public RangeAttackProcessor(UnitBase unit, float attackRange, float damage,
            CooldownProcessor cooldownProcessor, ProjectileType projectileType, ProjectileFactory projectilesFactory)
            : base(unit, attackRange, damage, cooldownProcessor)
        {
            _projectileType = projectileType;
            _projectilesFactory = projectilesFactory;
        }
        
        protected sealed override void Attack(IUnitTarget target)
        {
            if (!target.CastPossible<IDamagable>())
            {
#if UNITY_EDITOR
                Debug.LogWarning($"Target {target} can't be attacked");
#endif
                return;
            }

            var projectile = _projectilesFactory.Create(_projectileType);
            projectile.transform.position = Transform.position;
            InitProjectileData(projectile, target);
            ProjectileSpawned?.Invoke(projectile);
            Attacked?.Invoke();
            TargetAttacked?.Invoke(target);
        }

        protected virtual void InitProjectileData(ProjectileBase projectile, IUnitTarget target)
        {
            projectile.Init(Affiliation, this);
            projectile.SetTarget(target);
        }
    }
}