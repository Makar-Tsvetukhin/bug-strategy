using Constructions.LevelSystemCore;
using Source.Scripts.ResourcesSystem.ResourcesGlobalStorage;
using UnityEngine;
using Zenject;

namespace Constructions
{
    public class AntFort : ConstructionBase, IEvolveConstruction
    {
        [SerializeField] private AntFortConfig config;

        [Inject] private readonly ITeamsResourcesGlobalStorage _teamsResourcesGlobalStorage;

        public override FractionType Fraction => FractionType.Ants;
        public override ConstructionID ConstructionID => ConstructionID.AntFort;
        
        public IConstructionLevelSystem LevelSystem { get; private set; }
        
        protected override void OnAwake()
        {
            base.OnAwake();

            LevelSystem = new AntFortLevelSystem(this, config, _teamsResourcesGlobalStorage,  _healthStorage);
            Initialized += InitLevelSystem;
        }

        private void InitLevelSystem()
            => LevelSystem.Init(0);
    }
}