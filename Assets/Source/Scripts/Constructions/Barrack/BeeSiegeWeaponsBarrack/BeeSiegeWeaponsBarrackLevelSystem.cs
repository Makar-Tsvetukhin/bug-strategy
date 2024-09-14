using System;
using BugStrategy.Constructions.ConstructionLevelSystemCore;
using BugStrategy.Constructions.UnitsRecruitingSystem;
using BugStrategy.ResourcesSystem.ResourcesGlobalStorage;
using BugStrategy.UnitsHideCore;

namespace BugStrategy.Constructions.BeeSiegeWeaponsBarrack
{
    [Serializable]
    public class BeeSiegeWeaponsBarrackLevelSystem : ConstructionLevelSystemBase<BeeSiegeWeaponsBarrackLevel>
    {
        private readonly UnitsRecruiter _recruiter;
        private readonly UnitsHider _hider;

        public BeeSiegeWeaponsBarrackLevelSystem(ConstructionBase construction, BeeSiegeWeaponsBarrackConfig config, 
            ITeamsResourcesGlobalStorage teamsResourcesGlobalStorage, FloatStorage healthStorage, 
            UnitsRecruiter recruiter, UnitsHider hider) 
            : base(construction, config.Levels,  teamsResourcesGlobalStorage, healthStorage)
        {
            _recruiter = recruiter;
            _hider = hider;
        }

        public override void Init(int initialLevelIndex)
        {
            base.Init(initialLevelIndex);
            
            _recruiter.SetStackCount(CurrentLevel.RecruitingSize);
            _recruiter.SetNewDatas(CurrentLevel.RecruitingData);
            _hider.SetCapacity(CurrentLevel.HiderCapacity);
        }
        
        protected override void LevelUpLogic()
        {
            base.LevelUpLogic();

            _recruiter.SetStackCount(CurrentLevel.RecruitingSize);
            _recruiter.SetNewDatas(CurrentLevel.RecruitingData);
            _hider.SetCapacity(CurrentLevel.HiderCapacity);
        }
    }
}