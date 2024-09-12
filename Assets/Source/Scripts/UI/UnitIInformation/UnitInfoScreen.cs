﻿using System;
using System.Linq;
using UnityEngine;

namespace Source.Scripts.UI.UnitIInformation
{
    public class UnitInfoScreen : EntityInfoScreen
    {
        private UIRaceConfig _UIRaceConfig;
        private UnitActionsUIView _actionsUIView;
        private TacticsUIView _tacticsUIView;
        private BuldingsUIView _buldingsUIView;
        private UnitBase _unit;
        private UnitActionsType _actionsType;

        private UserBuilder _builder;
        
        private void Awake()
        {
            _builder = GameObject.Find("Builder").GetComponent<UserBuilder>();
            if(_builder == null)
                Debug.LogError("Builder is null");
            
            OnAwake();
            _UIRaceConfig = ConfigsRepository.FindConfig<UIRaceConfig>();

            _actionsUIView = UIScreenRepository.GetScreen<UnitActionsUIView>();
            _tacticsUIView = UIScreenRepository.GetScreen<TacticsUIView>();
            _buldingsUIView = UIScreenRepository.GetScreen<BuldingsUIView>();
            
            _actionsUIView.ButtonClicked += SetActiveAction;
            _buldingsUIView.ButtonClicked += OnBuldingInstance;
            _tacticsUIView.ButtonClicked += OnTacticsUse;
        
            _actionsUIView.BackButtonClicked += BackButtonsMenu;
            _buldingsUIView.BackButtonClicked += BackButtonsMenu;
            _tacticsUIView.BackButtonClicked += BackButtonsMenu;
        }

        public void SetUnit(UnitBase unit)
        {
            if (_unit == unit)
                return;

            _unit = unit;
            SetActiveAction(UnitActionsType.None);
        }
    
        private void UpdateView()
        {
            UnitType unitType = _unit.UnitType;

            try
            {
                UIUnitConfig unitUIConfig = _UIRaceConfig.UnitsUIConfigs[unitType];

                SetHealthPointsInfo(unitUIConfig.InfoSprite, _unit.HealthStorage);
                _actionsUIView.TurnOffButtons();
                _tacticsUIView.TurnOffButtons();
                _buldingsUIView.TurnOffButtons();

                switch (_actionsType)
                {
                    case UnitActionsType.None:
                        _actionsUIView.SetButtons(unitUIConfig.UnitSectionsDictionary, unitUIConfig.UnitSections
                            .Select(x => x.Key).ToList());
                        break;
                    case UnitActionsType.Tactics:
                        _tacticsUIView.SetButtons(unitUIConfig.UnitTacticsDictionary, unitUIConfig.UnitTactics
                            .Select(x => x.Key).ToList());
                        break;
                    case UnitActionsType.Constructions:
                        _buldingsUIView.SetButtons(unitUIConfig.UnitConstructionDictionary, unitUIConfig
                            .UnitConstruction
                            .Select(x => x.Key).ToList());
                        break;
                    case UnitActionsType.Abilities:

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception exp)
            {
                throw new Exception("Настоятельно рекомендую проверить есть ли конфиг (UIUnitConfig и добавлен ли он " +
                                    "в UIRaceConfig)  " + exp.Message);
            }
        }
    
        private void BackButtonsMenu()
            => SetActiveAction(UnitActionsType.None);
    
        private void SetActiveAction(UnitActionsType actionsType)
        {
            _actionsType = actionsType;
            UpdateView();
        }
        
        private void OnTacticsUse(UnitTacticsType unitTacticsType)
        {
            switch (unitTacticsType)
            {
                case UnitTacticsType.Build:
               
                    break;
                case UnitTacticsType.Repair:
                    // _unitBase.AutoGiveOrder();
                    break;
            }
        }
        
        private void OnBuldingInstance(ConstructionID constructionID) 
            => _builder.SpawnConstructionMovableModel(constructionID);
    }
}