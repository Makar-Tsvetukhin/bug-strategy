using Constructions.LevelSystemCore;
using Source.Scripts;
using Source.Scripts.ResourcesSystem;
using Source.Scripts.ResourcesSystem.ResourcesGlobalStorage;
using UnitsHideCore;

namespace Constructions.BeeHospital
{
    public class BeeHospitalLevelSystem : ConstructionLevelSystemBase<BeeHospitalLevel>
    {
        private readonly UnitsHider _hider;
        private readonly HealProcessor _healProcessor;

        public BeeHospitalLevelSystem(ConstructionBase construction, BeeHospitalConfig config, 
            ITeamsResourcesGlobalStorage teamsResourcesGlobalStorage, FloatStorage healthStorage, UnitsHider hider, 
            HealProcessor healProcessor) 
            : base(construction, config.Levels,  teamsResourcesGlobalStorage, healthStorage)
        {
            _hider = hider;
            _healProcessor = healProcessor;
        }

        public override void Init(int initialLevelIndex)
        {
            base.Init(initialLevelIndex);
            
            _hider.SetCapacity(CurrentLevel.HiderCapacity);
            _healProcessor.SetHealPerSecond(CurrentLevel.HealPerSecond);
        }

        protected override void LevelUpLogic()
        {
            base.LevelUpLogic();

            _hider.SetCapacity(CurrentLevel.HiderCapacity);
            _healProcessor.SetHealPerSecond(CurrentLevel.HealPerSecond);
        }
    }
}