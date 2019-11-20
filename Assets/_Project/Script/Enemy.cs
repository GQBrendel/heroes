using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Actor
{
    public int Movement;
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
        AudioManager.Instance.Play("SkeletonAttack");
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
        AudioManager.Instance.Play("SkeletonDeath");
        if (Frosted)
        {
            var ice = FindObjectOfType<IceController>();
            ice.BreakTheIce();
        }
    }

    public override void PlayDamageSound()
    {
        int random = Random.Range(0, 3);
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

    public Vector2 TopLeft { get { return new Vector2(posX - 1, posY + 1); } }
    public Vector2 Top { get { return new Vector2(posX,posY + 1); } }
    public Vector2 TopRight { get { return new Vector2(posX + 1 , posY + 1); } }

    public Vector2 Right { get { return new Vector2(posX + 1 , posY); } }

    public Vector2 BottonRight { get { return new Vector2(posX + 1, posY - 1); } }
    public Vector2 Botton { get { return new Vector2(posX, posY - 1); } }
    public Vector2 BottonLeft { get { return new Vector2(posX - 1, posY - 1); } }

    public Vector2 Left { get { return new Vector2(posX - 1, posY); } }
}
