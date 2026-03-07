using System;
using UnityEngine;

public abstract class MovementBehavior : MonoBehaviour
{
    protected GameUnit GameUnit;
    public bool IsMoving;
    
    public virtual void OnInitialize(GameUnit owner)
    {
        GameUnit = owner;
        IsMoving = true;
    }
    public virtual void OnDeath() => IsMoving = false;
    public void Stop() => IsMoving = false;
    public void Resume() => IsMoving = true;

    public virtual void OnUpdate()
    {
        // Base implementation (empty or default logic)
    }
}