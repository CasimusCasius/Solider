using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBusyUI : MonoBehaviour
{
    private void Start()
    {
        UnitActionSystem.Instance.OnBusyChange += UnitActionSystem_OnBusyChange;
        SetBusyUIVisible(false);
    }

    private void UnitActionSystem_OnBusyChange(object sender, bool isBusy)
    {
        SetBusyUIVisible(isBusy);
    }

    private void SetBusyUIVisible(bool isBusy)
    {
        gameObject.SetActive(isBusy);
    }
}
