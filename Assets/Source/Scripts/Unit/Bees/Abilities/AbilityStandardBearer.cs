using AttackCooldownChangerSystem;
using MoveSpeedChangerSystem;

namespace Unit.Bees
{
    public class AbilityStandardBearer
    {
        private readonly SphereTrigger _abilityZone;
        private readonly float _attackSpeedIncreasePower;
        private readonly float _moveSpeedIncreasePower;
        
        public AbilityStandardBearer(SphereTrigger abilityZone, float attackSpeedIncreasePower, float moveSpeedIncreasePower, 
            float abilityRadius)
        {
            _abilityZone = abilityZone;
            _attackSpeedIncreasePower = attackSpeedIncreasePower;
            _moveSpeedIncreasePower = moveSpeedIncreasePower;
            _abilityZone.SetRadius(abilityRadius);
            
            _abilityZone.EnterEvent += Enter;
            _abilityZone.ExitEvent += Exit;
        }

        private void Enter(ITriggerable triggerable)
        {
            if (triggerable.TryCast(out IAttackCooldownChangeable attackSpeedChangeable) 
                && attackSpeedChangeable.Affiliation == AffiliationEnum.Bees)
            {
                attackSpeedChangeable.AttackCooldownChanger.Apply(_attackSpeedIncreasePower);
            }
            
            if (triggerable.TryCast(out IMoveSpeedChangeable moveSpeedChangeable) 
                && moveSpeedChangeable.Affiliation == AffiliationEnum.Bees)
            {
                moveSpeedChangeable.MoveSpeedChangerProcessor.Apply(_moveSpeedIncreasePower);
            }
        }

        private void Exit(ITriggerable triggerable)
        {
            if (triggerable.TryCast(out IAttackCooldownChangeable attackSpeedChangeable) 
                && attackSpeedChangeable.Affiliation == AffiliationEnum.Bees)
            {
                attackSpeedChangeable.AttackCooldownChanger.DeApply(_attackSpeedIncreasePower);
            }
            
            if (triggerable.TryCast(out IMoveSpeedChangeable moveSpeedChangeable) 
                && moveSpeedChangeable.Affiliation == AffiliationEnum.Bees)
            {
                moveSpeedChangeable.MoveSpeedChangerProcessor.DeApply(_moveSpeedIncreasePower);
            }
        }
    }
}