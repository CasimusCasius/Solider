using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisualSystemSingle : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;


    public void Show(Material material)
    {
        meshRenderer.enabled = true;
        meshRenderer.material = material;
    }

    public void Hide()
    {
        meshRenderer.enabled = false;
    }
}
