﻿using AStar_2D.Demo;
using UnityEngine;

public class Enemy : Actor
{
    public int Movement;
    public int Strength;
    public int BasicAttackDamage;

    public int Constitution;
    public float MaxHealth;
    public float Health;

    public int id = 0;
    protected Actor _currentTarget;

    public Actor TargetToLookAt { get; set; }

    public bool Frosted { get; set; }
    private int _frostDuration;

    void Start()
    {
        parentStart();
        Health = MaxHealth;
    }

    private void Update()
    {
        if (rotate && TargetToLookAt)
        {
            float speed = 5;

            Quaternion targetRotation;
            targetRotation = Quaternion.LookRotation(TargetToLookAt.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
        }
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
    }

    internal void DestinationUnreachable()
    {
        mainAction = true;
        moveAction = true;
        GetComponent<AnimatedAgent>().moved = true;
        rotate = true;
        StartCoroutine(SetRotateToFalse());
    }

    public override void Heal(int healValue)
    {
        Health += healValue;

        if (Health > GetMaxHealth())
        {
            Health = GetMaxHealth();
        }
        UpdateCharacterInfoNoSelection();
        float scaleX = Health / GetMaxHealth();
        anim.SetTrigger("Healed");
        _healParticle.Play();
        healthBar.transform.localScale = new Vector3(scaleX, 1f, 1f);
    }

    public override float GetMaxHealth()
    {
        return MaxHealth;
    }
    public override float GetCurrentHealth()
    {
        return Health;
    }

    public virtual void Attack(Actor target)
    {
        if (!target)
        {
            return;
        }
        transform.LookAt(target.transform);
        anim.SetTrigger("Attack");
        _currentTarget = target;
    }

    public void AttackSound()
    {
        AudioManager.Instance.Play("SkeletonAttack", true);
    }

    public void AttackHit()
    {
        BasicAttackFight(_currentTarget, this);
        mainAction = true;
    }

    protected override void BasicAttackFight(Actor opponent, Actor attackingActor)
    {
        int damage = Strength + BasicAttackDamage;
        opponent.TakeDamage(damage - opponent.GetCharacterDefense(), attackingActor);
    }

    public override void PerformDeathSpecifcsActions()
    {
        AudioManager.Instance.Play("SkeletonDeath");
        if (Frosted)
        {
            var ice = FindObjectOfType<IceController>();
            ice.BreakTheIce();
        }
    }

    public override void PlayDamageSound()
    {
        int random = UnityEngine.Random.Range(0, 3);
        if(random == 0)
        {
            AudioManager.Instance.Play("SkeletonHit1");
        }
        else if (random == 1)
        {
            AudioManager.Instance.Play("SkeletonHit2");
        }
        else if (random == 2)
        {
            AudioManager.Instance.Play("SkeletonHit3");
        }
    }

    internal void setId(int _id)
    {
        id = _id;
    }

    public override void GetFrosted(int duration)
    {
        _frostDuration = duration;
        Frosted = true;
        OnActorFrosted?.Invoke(this as Actor);
    }
    public override void ResetActions()
    {
        base.ResetActions();
        if(_frostDuration > 0)
        {
            _frostDuration--;
            if(_frostDuration == 0)
            {
                OnActorEndFrosted?.Invoke(this as Actor);
                Frosted = false;
                var ice = FindObjectOfType<IceController>();
                if (ice)
                {
                    ice.BreakTheIce();
                }
            }
        }
    }
    public override int GetCharacterDefense()
    {
        return Constitution;
    }


    public Vector2 TopLeft { get { return new Vector2(posX - 1, posY + 1); } }
    public Vector2 Top { get { return new Vector2(posX,posY + 1); } }
    public Vector2 TopRight { get { return new Vector2(posX + 1 , posY + 1); } }

    public Vector2 Right { get { return new Vector2(posX + 1 , posY); } }

    public Vector2 BottonRight { get { return new Vector2(posX + 1, posY - 1); } }
    public Vector2 Botton { get { return new Vector2(posX, posY - 1); } }
    public Vector2 BottonLeft { get { return new Vector2(posX - 1, posY - 1); } }

    public Vector2 Left { get { return new Vector2(posX - 1, posY); } }
}
