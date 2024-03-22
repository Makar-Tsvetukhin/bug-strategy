using Construction.TownHalls;
using Unit.ProfessionsCore.Processors;
using UnityEngine;

namespace Unit.States
{
    public class StorageResourceState : EntityStateBase
    {
        public override EntityStateID EntityStateID => EntityStateID.StorageResource;

        private readonly MovingUnit _unit;
        private readonly ResourceExtractionProcessor _resourceExtractionProcessor;
        
        public StorageResourceState(MovingUnit unit, ResourceExtractionProcessor resourceExtractionProcessor)
        {
            _unit = unit;
            _resourceExtractionProcessor = resourceExtractionProcessor;
        }
        
        public override void OnStateEnter()
        {
            if (!_resourceExtractionProcessor.GotResource ||
                !_unit.CurrentPathData.Target.CastPossible<TownHallBase>())
            {
#if UNITY_EDITOR
                Debug.LogWarning($"Some problem: " +
                                 $"{!_unit.CurrentPathData.Target.CastPossible<TownHallBase>()}");            
#endif
                _unit.AutoGiveOrder(null);
                return;
            }
            
            _resourceExtractionProcessor.StorageResources();
            
            _unit.AutoGiveOrder(null);
        }

        public override void OnStateExit()
        {

        }

        public override void OnUpdate()
        {
            
        }
    }
}