using System.Collections.Generic;
using PoisonFog;
using Projectiles;
using Source.Scripts.ResourcesSystem.ResourcesGlobalStorage;

namespace Source.Scripts.Missions
{
    public class MissionData
    {
        public readonly int MissionIndex;
        
        public readonly AffiliationEnum PlayerAffiliation;
        public readonly IReadOnlyDictionary<AffiliationEnum, FractionType> FractionTypes;

        public readonly UnitRepository UnitRepository = new();
        public readonly ProjectilesRepository ProjectilesRepository = new();
        public readonly PoisonFogsRepository PoisonFogsRepository = new();
        public readonly ResourceSourcesRepository ResourceSourcesRepository = new();
        public readonly ConstructionsRepository ConstructionsRepository = new();
        
        public readonly ITeamsResourcesGlobalStorage TeamsResourcesGlobalStorage;
        public readonly ConstructionSelector ConstructionSelector;

        public FractionType PlayerFraction => FractionTypes[PlayerAffiliation];
        
        public MissionData(int missionIndex, MissionConfig missionConfig, ITeamsResourcesGlobalStorage teamsResourcesGlobalStorage) 
        {
            MissionIndex = missionIndex;
            PlayerAffiliation = missionConfig.PlayerAffiliation;
            FractionTypes = missionConfig.FractionByAffiliation;
            TeamsResourcesGlobalStorage = teamsResourcesGlobalStorage;
            ConstructionSelector = new ConstructionSelector(ConstructionsRepository);
        }
    }
}