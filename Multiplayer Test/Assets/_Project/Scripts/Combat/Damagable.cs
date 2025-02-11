using PatchworkGames;
using System;
using UnityEngine;

public class Damagable : MonoBehaviour
{
    [Header("Hitpoints")]
    [SerializeField] private int _currentHP;
    [SerializeField] private int _maxHP;
    [Space]
    [SerializeField] private bool _startMax = true;

    public int currentHP => _currentHP;
    public int maxHP => _maxHP;

    //[Header("VFX")]
    //[SerializeField] private PopupText _popupPrefab;

    public Action OnDamaged = delegate { };
    public Action OnHealed = delegate { };
    public Action OnDeath = delegate { };

    private void Start()
    {
        if (_startMax) DoHeal(_maxHP);

        //ObjectPoolManager.PrewarmObject(PrefabManager.PopupText.gameObject, 10, ObjectPoolManager.PoolType.Gameobject);
    }

    public void DoDamage(int amount)
    {
        int popupInt = amount > _currentHP ? _currentHP : amount;

        _currentHP -= amount;

        if (_currentHP <= 0)
        {
            _currentHP = 0;
            OnDeath?.Invoke();
        }
        else
        {
            OnDamaged?.Invoke();
        }

        SpawnDamagePopup(popupInt);
    }
    public void DoHeal(int amount)
    {
        _currentHP += amount;
        if (_currentHP > _maxHP) _currentHP = _maxHP;

        OnHealed?.Invoke();
    }

    private void SpawnDamagePopup(int damageAmount)
    {
        /*if (PrefabManager.PopupText == null) return;
        if (damageAmount <= 0) return;

        //float rand = UnityEngine.Random.Range(0f, 1f);
        //if (rand > 0.85f) return;

        ObjectPoolManager.SpawnObject(PrefabManager.PopupText.gameObject, 
            transform.position + (Vector3.up * 2.0f), Quaternion.identity)
            .GetComponent<PopupText>().Initialize(damageAmount.ToString()); // damageAmount.ToString() // PrefabManager.PhrasesSO.GetRandomHit()*/
    }


}
