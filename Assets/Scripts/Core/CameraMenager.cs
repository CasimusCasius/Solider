using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMenager : MonoBehaviour
{
    [SerializeField] private GameObject actionCameraGameObject;

    private void Start()
    {
        BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
        BaseAction.OnAnyActionEnded += BaseAction_OnAnyActionEnded;

        SetActionCameraEnabled(false);
    }

    private void BaseAction_OnAnyActionStarted(object sender, System.EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                SetActionCameraPosition(shootAction);
                SetActionCameraEnabled(true);
                break;
        }
    }



    private void BaseAction_OnAnyActionEnded(object sender, System.EventArgs e)
    {

        switch (sender)
        {
            case ShootAction shootAction:
                SetActionCameraEnabled(false);
                break;
        }
    }
    private void SetActionCameraEnabled(bool enable)
    {
        actionCameraGameObject.SetActive(enable);
    }
    private void SetActionCameraPosition(ShootAction shootAction)
    {
        Unit shooterUnit = shootAction.GetUnit();
        Unit targetUnit = shootAction.GetTargetUnit();
        Vector3 cameraCharacterHeight = Vector3.up * 1.7f;
        Vector3 shootDir = (targetUnit.GetWorldPosition() - shooterUnit.GetWorldPosition()).normalized;

        float shoulderOffsetAmount = 0.5f;
        Vector3 shoulderOffset = Quaternion.Euler(0, 90, 0) * shootDir * shoulderOffsetAmount;

        Vector3 actionCameraPosition =
            shooterUnit.GetWorldPosition() + cameraCharacterHeight + shoulderOffset + shootDir * -1;

        actionCameraGameObject.transform.position = actionCameraPosition;
        actionCameraGameObject.transform.LookAt(targetUnit.GetWorldPosition() + cameraCharacterHeight);
    }


}
