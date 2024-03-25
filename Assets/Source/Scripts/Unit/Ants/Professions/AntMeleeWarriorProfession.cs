﻿using System;
using Unit.Ants.Configs.Professions;
using Unit.OrderValidatorCore;
using Unit.ProcessorsCore;

namespace Unit.Ants.Professions
{
    [Serializable]
    public sealed class AntMeleeWarriorProfession : AntWarriorProfessionBase
    {
        public override ProfessionType ProfessionType => ProfessionType.MeleeWarrior;

        public override OrderValidatorBase OrderValidatorBase { get; }
        public override CooldownProcessor CooldownProcessor { get; }
        public override AttackProcessorBase AttackProcessor { get; }
        
        public AntMeleeWarriorProfession(AntBase ant, AntMeleeWarriorConfig antHandItem)
            : base(antHandItem.AntProfessionRang)
        {
            CooldownProcessor = new CooldownProcessor(antHandItem.Cooldown);
            AttackProcessor = new MeleeAttackProcessor(ant, antHandItem.AttackRange, antHandItem.Damage, CooldownProcessor);
            OrderValidatorBase = new WarriorOrderValidator(ant, antHandItem.AttackRange, CooldownProcessor, AttackProcessor);
           
            AttackProcessor.OnEnterEnemyInZone += EnterInZone;
            OrderValidatorBase.OnEnterInZone += EnterInZone;
        }

        public override void HandleUpdate(float time)
        {
            base.HandleUpdate(time);
            
            CooldownProcessor.HandleUpdate(time);
        }
    }
}