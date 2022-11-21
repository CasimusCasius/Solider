using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShootAction : BaseAction
{
    public event EventHandler<OnShootEventArgs> OnShoot;
    public class OnShootEventArgs:EventArgs
    {
        public Unit targetUnit;
        public Unit shootingUnit;
    }

    private enum State
    {
        Aming,
        Shooting,
        Cooloff
    }
    [SerializeField] int maxShootDistance = 7;
    [SerializeField] float aimingRotationSpeed = 20f;
    [SerializeField] float aimingStateTime = 1.0f;
    [SerializeField] float shootingStateTime = 0.1f;
    [SerializeField] float CooloffStateTime = 0.5f;
    [SerializeField] int damageAmount = 40;
    private State state;
    private Unit targetUnit;
    private float stateTimer;
    private bool canShootBullet;
    public override string GetActionName()
    {
        return "Shoot";
    }
    private void Update()
    {
        if (!isActive) { return; }
        stateTimer -= Time.deltaTime;
        switch (state)
        {
            case State.Aming:
                Aim();
                break;
            case State.Shooting:
                if (canShootBullet)
                {
                    Shoot();
                    canShootBullet = false;
                }
                break;
            case State.Cooloff:
                break;
        }
        if (stateTimer <= Mathf.Epsilon) { NextState(); }
    }

    private void Aim()
    {
        Vector3 aimDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * aimingRotationSpeed);
    }

    private void NextState()
    {
        switch (state)
        {
            case State.Aming:
                state = State.Shooting;
                stateTimer = shootingStateTime;
                break;
            case State.Shooting:
                state = State.Cooloff;
                stateTimer = CooloffStateTime;
                break;
            case State.Cooloff:
                ActionEnd();
                break;
        }
    }
    private void Shoot()
    {
        targetUnit.Damage(damageAmount);
        OnShoot?.Invoke(this, new OnShootEventArgs { shootingUnit = unit,
            targetUnit = this.targetUnit});
    }
    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();
        return GetValidActionGridPositionList(unitGridPosition);
    }
    public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition)
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        

        for (int x = -maxShootDistance; x <= maxShootDistance; x++)
        {
            for (int z = -maxShootDistance; z <= maxShootDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) continue;

                int testDistance = Mathf.RoundToInt(Mathf.Sqrt(x * x + z * z));
                if (testDistance > maxShootDistance) continue;

                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) continue;
                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
                if (targetUnit.IsEnemy() == unit.IsEnemy()) continue; // Both in the same team
                validGridPositionList.Add(testGridPosition);
            }
        }
        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        state = State.Aming;
        stateTimer = aimingStateTime;
        canShootBullet = true;

        ActionStart(onActionComplete);
    }
    public Unit GetTargetUnit()=>targetUnit;
    public int GetShootRange() => maxShootDistance;

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        return new EnemyAIAction 
        { 
            gridPosition = gridPosition,
            actionValue =Mathf.RoundToInt(100 +(1f-targetUnit.GetHealthNormalized()) * 100f),
        };
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        return GetValidActionGridPositionList(gridPosition).Count;
    }

}
