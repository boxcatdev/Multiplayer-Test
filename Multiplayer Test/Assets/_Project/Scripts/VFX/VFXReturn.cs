using PatchworkGames;
using UnityEngine;

public class VFXReturn : MonoBehaviour
{
    private void OnParticleSystemStopped()
    {
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }

}
