using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridVisualSystem : MonoBehaviour
{
    public static GridVisualSystem Instance { get; private set; }

    [Serializable]
    public struct GridVisualTypeMaterial
    {
        public GridVisualType gridVisualType;
        public Material material;
    }

    public enum GridVisualType
    {
        White,
        Blue,
        Red,
        RedSoft,
        Yellow,
        Green,
    }

    [SerializeField] private Transform gridSystemVisualPrefab;
    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;
    private GridVisualSystemSingle[,] gridVisualSystemSingleArray;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more that one GridVisualSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        gridVisualSystemSingleArray = new GridVisualSystemSingle[
            LevelGrid.Instance.GetWidth(),
            LevelGrid.Instance.GetHeight()
            ];

        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Transform gridSystemVisualSingleTransform =
                    Instantiate(
                        gridSystemVisualPrefab,
                        LevelGrid.Instance.GetWorldPosition(gridPosition),
                        Quaternion.identity);

                gridVisualSystemSingleArray[x, z] = gridSystemVisualSingleTransform.GetComponent<GridVisualSystemSingle>();
            }
        }

        UnitActionSystem.Instance.OnSelectedActionChange += UnitActionSystem_OnSelectedActionChange;
        LevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition;

        UpdateGridVisual();
    }

    private void UnitActionSystem_OnSelectedActionChange(object sender, System.EventArgs e)
    {
        UpdateGridVisual();
    }
    private void UpdateGridVisual()
    {
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        HideAllGridPosition();
        GridVisualType gridVisualType = GridVisualType.White;
        switch (selectedAction)
        {
            default:
            case MoveAction moveAction:
                gridVisualType = GridVisualType.White;
                break;
            case SpinAction spinAction:
                gridVisualType = GridVisualType.Blue;
                break;
            case ShootAction shootAction:
                gridVisualType = GridVisualType.Red;
                ShowGridPositionRange(selectedUnit.GetGridPosition(), shootAction.GetShootRange(), GridVisualType.RedSoft);
                break;

        }
        ShowGridPositionList(selectedAction.GetValidActionGridPositionList(), gridVisualType);
    }
    public void HideAllGridPosition()
    {
        foreach (var gridSingleMesh in gridVisualSystemSingleArray)
        {
            gridSingleMesh.Hide();
        }
    }
    private void ShowGridPositionRange(GridPosition gridPosition,int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();

        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z);
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) continue;
                int testDistance = (int)Mathf.Round(MathF.Sqrt(x * x + z * z));
                if (testDistance > range) { continue; }
                gridPositionList.Add(testGridPosition);
            }
        }

        ShowGridPositionList(gridPositionList, gridVisualType);
    }

    public void ShowGridPositionList(List<GridPosition> gridPositions, GridVisualType gridVisualType)
    {
        foreach (var gridPosition in gridPositions)
        {

            gridVisualSystemSingleArray[gridPosition.x, gridPosition.z].Show(UpdateGridVisualTypeMaterial(gridVisualType));
        }
    }

    private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, System.EventArgs e)
    {
        UpdateGridVisual();
    }
    private Material UpdateGridVisualTypeMaterial(GridVisualType gridVisualType)
    {
        foreach (var gridVisualTypeMaterial in gridVisualTypeMaterialList)
        {
            if (gridVisualTypeMaterial.gridVisualType == gridVisualType)
            {
                return gridVisualTypeMaterial.material;
            }  
        }
        return null;
    }


}
