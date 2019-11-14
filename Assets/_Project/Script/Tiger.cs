using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiger : MonoBehaviour
{

    public delegate void EnemyHitHandler();
    public delegate void FinishedAttackHandler();

    public EnemyHitHandler OnEnemyHit;
    public FinishedAttackHandler OnEnemyFinishedAttack;

    public void HitEnemy()
    {
        OnEnemyHit?.Invoke();
    }

    public void AttackFinished()
    {
        OnEnemyFinishedAttack?.Invoke();
        Destroy(transform.parent.gameObject);
    }
}
