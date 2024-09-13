using System;
using System.Collections.Generic;
using UnityEngine;
using MiniMapSystem;
using Source.Scripts;
using Source.Scripts.Ai.InternalAis;
using Source.Scripts.ResourcesSystem;
using Unit;
using Unit.Effects;
using Unit.Effects.InnerProcessors;
using Unit.Effects.Interfaces;
using Unit.OrderValidatorCore;
using UnityEngine.AI;
using Zenject;

public abstract class UnitBase : MonoBehaviour, IUnit, ITriggerable, IDamagable, IUnitTarget, IMiniMapObject,
    SelectableSystem.ISelectable, IPoolable<UnitBase, UnitType>, IPoolEventListener,
    IHealable, IAffiliation,
    IEffectable, IPoisonEffectable, IStickyHoneyEffectable, IMoveSpeedChangeEffectable
{
    //
    private NavMeshAgent _navMeshAgent;

    [Inject] private readonly EffectsFactory _effectsFactory;
    private float _startMaxSpeed;

    public Vector3 Velocity => _navMeshAgent.velocity;
    //

    [SerializeField] private UnitVisibleZone _unitVisibleZone;
    [SerializeField] private UnitInteractionZone unitInteractionZone;
    [SerializeField] private UnitInteractionZone dynamicUnitZone;

    protected FloatStorage _healthStorage { get; set; } = new FloatStorage(100, 100);
    protected EntityStateMachine _stateMachine;
    protected List<AbilityBase> _abilites = new List<AbilityBase>();

    public bool IsSticky { get; private set; }
    public bool IsSelected { get; private set; }
    public bool IsActive { get; protected set; }
    public Vector3 TargetMovePosition { get; protected set; }
    protected abstract OrderValidatorBase OrderValidator { get; }
    public EffectsProcessor EffectsProcessor { get; protected set; }
    public MoveSpeedChangerProcessor MoveSpeedChangerProcessor { get; protected set; }
    public AffiliationEnum Affiliation { get; private set; }
    public abstract InternalAiBase InternalAi { get; protected set; }

    public bool IsAlive => IsActive && _healthStorage.CurrentValue > 0f;
    public Transform Transform => transform;
    public UnitVisibleZone VisibleZone => _unitVisibleZone;
    public UnitInteractionZone UnitInteractionZone => unitInteractionZone;
    public UnitInteractionZone DynamicUnitZone => dynamicUnitZone;
    public UnitTargetType TargetType => UnitTargetType.Other_Unit;
    public MiniMapObjectType MiniMapObjectType => MiniMapObjectType.Unit;
    public IReadOnlyFloatStorage HealthStorage => _healthStorage;
    public IReadOnlyList<AbilityBase> Abilities => _abilites;
    public EntityStateMachine StateMachine => _stateMachine;
    public UnitType Identifier => UnitType;
    public abstract FractionType Fraction { get; }
    public abstract UnitType UnitType { get; }

    private UnitPathData _currentPathData = new UnitPathData(null, UnitPathType.Idle);
    public UnitPathData CurrentPathData
    {
        get => _currentPathData;
        protected set
        {
            if (value == _currentPathData) return;

            if (!_currentPathData.Target.IsAnyNull())
                _currentPathData.Target.OnDeactivation -= OnPathTargetDeactivated;
                
            _currentPathData = value;
            if (!_currentPathData.Target.IsAnyNull())
                _currentPathData.Target.OnDeactivation += OnPathTargetDeactivated;

            OnUnitPathChange?.Invoke(this);
        }
    }

    public event Action<UnitBase> OnUnitPathChange;
    public event Action<UnitBase> OnUnitDied;
    public event Action OnUnitDiedEvent;
    public event Action<ITriggerable> OnDisableITriggerableEvent;
    public event Action OnSelect;
    public event Action OnDeselect;
    public event Action<UnitBase> ElementReturnEvent;
    public event Action<UnitBase> ElementDestroyEvent;
    public event Action OnTargetMovePositionChange;
    public event Action<IUnitTarget> OnDeactivation;
    public event Action TookDamage;
    public event Action<IUnitTarget> TookDamageWithAttacker;
    public event Action PathTargetDeactivated;

    private void Awake()
    {
        _navMeshAgent = gameObject.GetComponent<NavMeshAgent>();

        _startMaxSpeed = _navMeshAgent.speed;

        MoveSpeedChangerProcessor = new MoveSpeedChangerProcessor(_navMeshAgent);
        EffectsProcessor = new EffectsProcessor(this, _effectsFactory);

        OnAwake();
    }

    private void Start()
    {
        UnitPool.Instance.UnitCreation(this);

        OnStart();
    }

    public virtual void HandleUpdate(float time)
    {
        _stateMachine.OnUpdate();
      
        EffectsProcessor.HandleUpdate(time);
    }

    protected virtual void OnAwake() { }

    protected virtual void OnStart() { }

    public void SetDestination(Vector3 position)
    {
        _navMeshAgent.SetDestination(position);
    }

    public void Warp(Vector3 position)
    {
        _navMeshAgent.Warp(position);
    }

    public virtual void GiveOrder(GameObject target, Vector3 position)
        => AutoGiveOrder(target.GetComponent<IUnitTarget>(), position);
    
    public void SetAffiliation(AffiliationEnum affiliation)
    {
        Affiliation = affiliation;
    }

    public void TakeDamage(IDamageApplicator damageApplicator, float damageScale)
        => TakeDamage(null, damageApplicator, damageScale);
    
    public virtual void TakeDamage(IUnitTarget attacker, IDamageApplicator damageApplicator, float damageScale = 1)
    {
        if (!IsAlive)
        {
            Debug.LogError($"You try damage unit that already dead: {attacker} | {damageApplicator} | {this}");
            return;
        }

        _healthStorage.ChangeValue(-damageApplicator.Damage * damageScale);
        OnDamaged();
        TookDamage?.Invoke();
        TookDamageWithAttacker?.Invoke(attacker);

        if (!IsAlive)
        {
            OnUnitDied?.Invoke(this);
            OnUnitDiedEvent?.Invoke();
            ReturnInPool();
            return;
        }
    }

    public void TakeHeal(float value)
        => _healthStorage.ChangeValue(value);

    protected virtual void OnDamaged() { }

    public void Select()
    {
        if (IsSelected) return;

        IsSelected = true;
        OnSelect?.Invoke();
    }

    public void Deselect()
    {
        if (!IsSelected) return;

        IsSelected = false;
        OnDeselect?.Invoke();
    }

    public virtual void OnElementReturn()
    {
        IsActive = false;
        OnDeactivation?.Invoke(this);
        gameObject.SetActive(false);
    }

    public virtual void OnElementExtract()
    {
        IsActive = true;
        gameObject.SetActive(true);
        EffectsProcessor.Reset();
        MoveSpeedChangerProcessor.Reset();

        SwitchSticky(false);
    }

    public void AutoGiveOrder(IUnitTarget unitTarget)
        => AutoGiveOrder(unitTarget, transform.position);

    /// <param name="targetMovePosition"> move position that used if unitTarget is null</param>
    public void AutoGiveOrder(IUnitTarget unitTarget, Vector3 targetMovePosition)
    {
        targetMovePosition.y = 0;

        CurrentPathData = OrderValidator.AutoGiveOrder(unitTarget);
        if (!CurrentPathData.Target.IsAnyNull())
        {
            targetMovePosition = OrderValidator.CheckDistance(CurrentPathData)
                ? transform.position
                : unitTarget.Transform.position;
        }

        CalculateNewState(targetMovePosition);
    }

    public void HandleGiveOrder(IUnitTarget unitTarget, UnitPathType unitPathType)
        => HandleGiveOrder(unitTarget, unitPathType, transform.position);

    /// <param name="targetMovePosition"> move position that used if unitTarget is null</param>
    public void HandleGiveOrder(IUnitTarget unitTarget, UnitPathType unitPathType, Vector3 targetMovePosition)
    {
        targetMovePosition.y = 0;

        CurrentPathData = OrderValidator.HandleGiveOrder(unitTarget, unitPathType);
        if (!CurrentPathData.Target.IsAnyNull())
        {
            targetMovePosition = OrderValidator.CheckDistance(CurrentPathData)
                ? transform.position
                : unitTarget.Transform.position;
        }

        CalculateNewState(targetMovePosition);
    }

    private void OnPathTargetDeactivated(IUnitTarget _) 
        => PathTargetDeactivated?.Invoke();

    public EntityStateID EntityStateID;
    private void CalculateNewState(Vector3 newTargetMovePosition)
    {
        newTargetMovePosition.y = 0;
        if (TargetMovePosition != newTargetMovePosition)
        {
            TargetMovePosition = newTargetMovePosition;
            OnTargetMovePositionChange?.Invoke();
        }

        var curPos = transform.position;
        curPos.y = 0;
        if (TargetMovePosition == curPos)
        {
            var newState = CurrentPathData.PathType switch
            {
                UnitPathType.Attack => EntityStateID.Attack,
                UnitPathType.Collect_Resource => EntityStateID.ExtractionResource,
                UnitPathType.Storage_Resource => EntityStateID.StorageResource,
                UnitPathType.Build_Construction => EntityStateID.Build,
                UnitPathType.Move => EntityStateID.Idle,
                UnitPathType.Switch_Profession => EntityStateID.SwitchProfession,
                UnitPathType.Repair_Construction => throw new NotImplementedException(),
                UnitPathType.HideInConstruction => EntityStateID.HideInConstruction,
                _ => throw new NotImplementedException()
            };

            StateMachine.SetState(newState);
        }
        else
        {
            StateMachine.SetState(EntityStateID.Move);
        }

        EntityStateID = _stateMachine.ActiveState;
    }

    protected void ReturnInPool()
        => ElementReturnEvent?.Invoke(this);

    private void OnDisable()
    {
        OnDisableITriggerableEvent?.Invoke(this);
    }

    private void OnDestroy()
    {
        ElementDestroyEvent?.Invoke(this);
    }

    public void SwitchSticky(bool isSticky)
    {
        IsSticky = isSticky;
    }
}