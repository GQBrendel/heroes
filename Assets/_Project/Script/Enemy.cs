using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Actor {

    public int id = 0;

    CursorManager mouseCursor;

    void Start()
    {
        parentStart();
        mouseCursor = GameObject.FindGameObjectWithTag("GameController").GetComponent<CursorManager>();
    }

    void Update()
    {
        parentUpdate();

        if (rotate)
        {
            float speed = 5;

            Quaternion targetRotation;
            targetRotation = Quaternion.LookRotation(FindClosestObjWithTag("Hero").transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
            StartCoroutine(wait());
        }
    }
    public void Attack(Actor target)
    {
        transform.LookAt(target.transform);
        anim.SetTrigger("Attack");
        mainAction = true;
        fight(target);
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
        mouseCursor.setAttack();
    }

    void OnMouseExit()
    {
        mouseCursor.resetCursor();
    }

    internal void setId(int _id)
    {
        id = _id;
    }

    void OnDestroy()
    {
        EnemiesController.Instance.RemoveEnemyFromList(this);
    }
}
