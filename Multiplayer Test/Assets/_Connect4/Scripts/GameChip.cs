using UnityEngine;

public class GameChip : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;

    public void ChangeMaterial(Material material)
    {
        if (meshRenderer != null) meshRenderer.material = material;
    }
}
