using AStar_2D.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Necromancer : Enemy
{
    [SerializeField] private GameObject _skeletonPrefab;
    [SerializeField] private int _shieldDamage;
    [SerializeField] private GameObject _darkCloudEffect;
    [SerializeField] private GameObject _fireShieldEffect;

    private bool _shielded;

    public override void TryMoveEnemy(Tile tileDestino)
    {
        animatedAgent = GetComponent<AnimatedAgent>();

        if (tileDestino == currentTile)
        {

        }

        else if (animatedAgent.IsMoving || !tileDestino.IsWalkable)
        {
            return;
        }

        currentTile.toggleWalkable();                             
        currentTile = tileDestino;

        animatedAgent.PlaceAt(tileDestino.WorldPosition);

        setPos((int)tileDestino.getPos().x, (int)tileDestino.getPos().y);  
    }


    public void StartSummon()
    {
        anim.SetTrigger("Summon");
    }
    public void SummonSkeletons()
    {
        Vector2 spawnPos = GetSpawnForSkeleton();     
        TileManager.Instance.GenerateActor(_skeletonPrefab, spawnPos);

        spawnPos = GetSpawnForSkeleton();
        TileManager.Instance.GenerateActor(_skeletonPrefab, spawnPos);
    }
    private Vector2 GetSpawnForSkeleton()
    {
        Vector2 spawnPos = Vector2.zero;
        int limitX = TileManager.Instance.gridX;
        int limitY = TileManager.Instance.gridY;
        int randomX = 0;
        int randomY = 0;

        do
        {
            randomX = Random.Range(posX - 4, posX + 4);
            randomY = Random.Range(posY - 4, posY + 4);

            if (randomX > limitX)
            {
                randomX = limitX-1;
            }
            else if (randomX < 0)
            {
                randomX = 0;
            }
            if (randomY > limitY)
            {
                randomY = limitY-1;
            }
            else if (randomY < 0)
            {
                randomY = 0;
            }
        } while (!TileManager.Instance.getObjectOnPosition(randomX, randomY).IsWalkable); 

        spawnPos = new Vector2(randomX, randomY);
        return spawnPos;
    }

    public void EnableShield()
    {
        _shielded = true;
        _fireShieldEffect.SetActive(true);
    }

    public void CastFireShield()
    {
        anim.SetTrigger("Shield");
    }
    public void DisableFireShield()
    {
        _shielded = false;
        _fireShieldEffect.SetActive(false);
    }

    public float XoffSet;

    public override void Attack(Actor target)
    {
        if (!target)
        {
            return;
        }
        Vector3 dir = target.transform.position;
        Vector3 correctedLook = new Vector3(dir.x + XoffSet, dir.y, dir.z);

        transform.LookAt(correctedLook);
        anim.SetTrigger("Attack");
        _currentTarget = target;
    }



    public override void TakeDamage(int damage, Actor attackingActor)
    {
        TileManager.Instance.ShowDamageMessage(currentTile, damage, false);
        Health -= damage;
        PlayDamageSound();

        if (Health < 0)
        {
            Health = 0;
        }

        UpdateCharacterInfoNoSelection();

        float scaleX = Health / GetMaxHealth();

        anim.SetTrigger("Damage");
        anim.SetBool("Dead", Health <= 0);
        healthBar.transform.localScale = new Vector3(scaleX, 1f, 1f);
        if (Health <= 0)
        {
            attackingActor.KilledAnEnemy(XPValue);
            PerformDeathSpecifcsActions();
            StartCoroutine(KillActor());
        }
        if (_shielded)
        {
            attackingActor.TakeDamage(_shieldDamage - attackingActor.GetCharacterDefense(), this);
        }
    }

    public void StartTeleport()
    {
        anim.SetTrigger("Teleport");
        _darkCloudEffect.SetActive(true);
    }

    public void DisableCloudEffect()
    {
        _darkCloudEffect.SetActive(false);
    }
}
