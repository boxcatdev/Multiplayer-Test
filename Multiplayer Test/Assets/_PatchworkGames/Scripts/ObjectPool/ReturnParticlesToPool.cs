using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PatchworkGames
{
    public class ReturnParticlesToPool : MonoBehaviour
    {
        public void ReturnObjectToPool()
        {
            ObjectPoolManager.ReturnObjectToPool(gameObject);
        }
        private void OnParticleSystemStopped()
        {
            ObjectPoolManager.ReturnObjectToPool(gameObject);
        }
    }
}

