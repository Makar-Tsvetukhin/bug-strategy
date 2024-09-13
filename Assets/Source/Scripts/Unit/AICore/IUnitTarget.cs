using System;
using UnityEngine;

public interface IUnitTarget
{
    public Transform Transform { get; }
    public UnitTargetType TargetType { get; }
    public AffiliationEnum Affiliation { get; }
    public bool IsActive { get; }
    
    /// <summary>
    /// mean something like: unit die or construction destructed, also it invoke OnDestroy
    /// </summary>
    public event Action<IUnitTarget> OnDeactivation;
}
