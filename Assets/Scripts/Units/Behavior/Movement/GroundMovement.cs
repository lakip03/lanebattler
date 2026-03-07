using UnityEngine;

public class GroundMovement : MovementBehavior
{
    public override void OnUpdate()
    {
        if (!IsMoving || GameUnit == null || !GameUnit.IsAlive()) return;
    
        float speed = GameUnit.UnitData.moveSpeed;
        var movementDirectionByUnitType = (GameUnit.isPlayerUnit ? 1f : -1f);
        transform.position += Vector3.forward * (movementDirectionByUnitType * (speed * Time.deltaTime));
    }
}