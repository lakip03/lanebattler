using UnityEngine;

public class FlyingMovement : MovementBehavior
{
    [SerializeField] private float upDistance;
    private bool isInitialized = false;
    public override void OnInitialize(GameUnit owner)
    {
        GameUnit = owner;
        IsMoving = true;

        if (!isInitialized)
        {
            transform.position += Vector3.up * upDistance;
            isInitialized = true;
        }
        
    }
    
    public override void OnUpdate()
    {
        if (!IsMoving || GameUnit == null || !GameUnit.IsAlive()) return;
    
        float speed = GameUnit.UnitData.moveSpeed;
        var movementDirectionByUnitType = (GameUnit.isPlayerUnit ? 1f : -1f);
        transform.position += Vector3.forward * (movementDirectionByUnitType * (speed * Time.deltaTime));
    }
}