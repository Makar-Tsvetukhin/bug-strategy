using UnityEngine;
using System;
using Source.Scripts;
using Source.Scripts.ResourcesSystem;

[Serializable]
public class AbilityBase
{
    [SerializeField] protected FloatStorage reloadTimer;
    [SerializeField] protected Sprite abilityIcon;

    public IReadOnlyFloatStorage ReloadTimer => reloadTimer;
    public float ReloadTime => reloadTimer.Capacity;
    public float CurrentTime => reloadTimer.CurrentValue;
    
    public Sprite AbilityIcon => abilityIcon;
    public bool Useable => CurrentTime >= ReloadTime;
    
    public virtual void OnInitialization()
    {
        
    }

    public virtual void OnUpdate(float time)
    {
        reloadTimer.ChangeValue(time);
    }

    public virtual void OnUse()
    {
        if (Useable)
            reloadTimer.SetValue(0);
    }
}
