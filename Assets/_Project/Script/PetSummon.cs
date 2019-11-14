using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetSummon : MonoBehaviour
{
    public delegate void EnemyHitHandler();
    public delegate void FinishedAttackHandler();

    public EnemyHitHandler OnEnemyHit;
    public FinishedAttackHandler OnEnemyFinishedAttack;

    [SerializeField] private GameObject _petPrefab;
    private Tiger _tiger;


    public void SummonPet(Vector3 _position)
    {
        var tiger = Instantiate(_petPrefab, _position, _petPrefab.transform.rotation);
        _tiger = tiger.GetComponentInChildren<Tiger>();
        _tiger.OnEnemyHit += HandleEnemyHit;
        _tiger.OnEnemyFinishedAttack += HandleFinishedAttack;
    }

    private void HandleFinishedAttack()
    {
        OnEnemyFinishedAttack?.Invoke();
    }
    private void HandleEnemyHit()
    {
        OnEnemyHit?.Invoke();
    }
}
