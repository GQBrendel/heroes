using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Actor
{

    public int id = 0;

    CursorManager mouseCursor;
    private Actor _currentTarget;

    public Actor TargetToLookAt { get; set; }

    public bool Frosted { get; set; }
    private int _frostDuration;

    void Start()
    {
        parentStart();
        mouseCursor = GameObject.FindGameObjectWithTag("GameController").GetComponent<CursorManager>();
    }

    void Update()
    {
        if (rotate && TargetToLookAt)
        {
            float speed = 5;

            Quaternion targetRotation;
            targetRotation = Quaternion.LookRotation(TargetToLookAt.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
        }
    }

    public void Attack(Actor target)
    {
        transform.LookAt(target.transform);
        anim.SetTrigger("Attack");
        _currentTarget = target;
    }

    public void AttackHit()
    {
        Fight(_currentTarget);
        mainAction = true;
    }

    public override void PerformDeathSpecifcsActions()
    {
        if (Frosted)
        {
            var ice = FindObjectOfType<IceController>();
            ice.BreakTheIce();
        }
    }


    public int limitX()
    {
        int random;
        do
        {
            random = UnityEngine.Random.Range(posX - moveDis, posX + moveDis);
            Debug.Log("Random X " + random);

        } while (random < 0 || random >= AStar_2D.Demo.TileManager.Instance.gridX || random == posX);
        return random;
    }

    public int limitY()
    {
        int random;
        do
        {
            random = UnityEngine.Random.Range(posY - moveDis, posY + moveDis);
            Debug.Log("Random Y " + random);

        } while (random < 0 || random >= AStar_2D.Demo.TileManager.Instance.gridY || random == posY);
        return random;
    }

    void OnMouseEnter()
    {
       // mouseCursor.setAttack();
    }

    void OnMouseExit()
    {
      //  mouseCursor.resetCursor();
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
                ice.BreakTheIce();

            }
        }
    }
}
