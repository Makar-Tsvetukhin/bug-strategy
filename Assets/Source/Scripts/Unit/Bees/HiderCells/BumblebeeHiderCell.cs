using CustomTimer;
using UnitsHideCore;

namespace Unit.Bees
{
    public class BumblebeeHiderCell : HiderCellBase
    {
        public readonly Timer CooldownValue;
        
        public BumblebeeHiderCell(UnitBase unit, CooldownProcessor cooldownProcessor) : base(unit)
        {
            CooldownValue = new Timer(cooldownProcessor.DefaultCapacity, cooldownProcessor.CurrentValue);
        }
        
        public override void HandleUpdate(float time)
        {
            CooldownValue.Tick(time);
        }
    }
}