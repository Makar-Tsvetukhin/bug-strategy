using Constructions.LevelSystemCore;
using Projectiles.Factory;
using Source.Scripts.Missions;
using Source.Scripts.ResourcesSystem.ResourcesGlobalStorage;
using UnityEngine;
using Zenject;

namespace Constructions
{
    public class BeeWaxTower : ConstructionBase, IEvolveConstruction
    {
        [SerializeField] private TriggerBehaviour attackZone;
        [SerializeField] private BeeWaxTowerConfig config;

        [Inject] private readonly ProjectileFactory _projectileFactory;
        [Inject] private readonly IConstructionFactory _constructionFactory;
        [Inject] private readonly ITeamsResourcesGlobalStorage _teamsResourcesGlobalStorage;
        
        public override FractionType Fraction => FractionType.Bees;
        public override ConstructionID ConstructionID => ConstructionID.BeeWaxTower;

        public IConstructionLevelSystem LevelSystem => _levelSystem;
        private BeeWaxTowerLevelSystem _levelSystem;
        
        private BeeWaxTowerAttackProcessor _attackProcessor;

        protected override void OnAwake()
        {
            base.OnAwake();

            _attackProcessor = new BeeWaxTowerAttackProcessor(this, _projectileFactory, attackZone, transform, this);

            _levelSystem = new BeeWaxTowerLevelSystem(this, config, _teamsResourcesGlobalStorage,
                _healthStorage, _attackProcessor);
            
            _updateEvent += UpdateAttackProcessor;
            OnDestruction += SpawnStickyTile;

            Initialized += InitLevelSystem;
        }

        private void InitLevelSystem()
        {
            _levelSystem.Init(0);
        }
        
        private void UpdateAttackProcessor()
            => _attackProcessor.HandleUpdate(Time.deltaTime);

        private void SpawnStickyTile()
        {
            ConstructionBase construction = _constructionFactory.Create<ConstructionBase>(ConstructionID.BeeStickyTileConstruction, Affiliation);
            MissionData.ConstructionsRepository.AddConstruction(transform.position, construction);
            construction.transform.position = transform.position;
        }
    }
}