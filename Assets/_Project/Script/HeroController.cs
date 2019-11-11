﻿using AStar_2D.Demo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : Actor
{
    private Actor _currentEnemy;
    

    public GameObject blueMark;
    public GameObject friendMark;
    public GameObject enemyMark;

    [SerializeField] private Interactable _emptyTileInteractiblePrefab;
    [SerializeField] private Interactable _enemyTileInteractiblePrefab;
    [SerializeField] private Interactable _friendlyTileInteractiblePrefab;
    [SerializeField] private Interactable _selfTileInteractiblePrefab;

    [Range(1, 5), SerializeField] private int _tauntDuration;

    private int _turnToEndTaunt;
    private int _tauntCounter;
    private bool _tauntActive;

    private bool _canControl = true;
    private Interactable _emptyTileMenu;
    private Interactable _enemyTileMenu;
    private Interactable _friendlyTileMenu;
    private Interactable _selfTileMenu;

    public int id = 0;



    private void Start ()
    {
        parentStart();
        var interactibleObject = Instantiate(_emptyTileInteractiblePrefab.gameObject);
        _emptyTileMenu = interactibleObject.GetComponent<Interactable>();

        interactibleObject = Instantiate(_enemyTileInteractiblePrefab.gameObject);
        _enemyTileMenu = interactibleObject.GetComponent<Interactable>();

        interactibleObject = Instantiate(_friendlyTileInteractiblePrefab.gameObject);
        _friendlyTileMenu = interactibleObject.GetComponent<Interactable>();

        interactibleObject = Instantiate(_selfTileInteractiblePrefab.gameObject);
        _selfTileMenu = interactibleObject.GetComponent<Interactable>();


        AnimatorEventListener[] animationEventListener = anim.GetBehaviours<AnimatorEventListener>();

        for (int i = 0; i < animationEventListener.Length; i++)
        {
            animationEventListener[i].Hero = this;
        }
        
    }
	
	void Update ()
    {
        if (!_canControl)
        {
            return;
        }

        parentUpdate();

        if (rotate)
        {
            float speed = 5;

            GameObject closestEnemy = FindClosestObjWithTag("Enemy");

            if (closestEnemy != null) {
                Quaternion targetRotation;
                targetRotation = Quaternion.LookRotation(closestEnemy.transform.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
               // StartCoroutine(wait());
            }            
        }
    }

    internal void CommandToSpinAttack()
    {
        if (mainAction)
        {
            Debug.LogError("Spin Attack being called but this unity already used main action");
            return;
        }

        _canControl = false;


        HideWays();
        anim.SetTrigger("Spin");
        OnActorStartAttack?.Invoke(this);

    }

    private void OpenTileOptions(Tile tile, Interactable interactableType)
    {
        RadialMenuSpawner.instance.SpawnMenu(interactableType, this, tile);
    }

    public void CommandToCancelAction()
    {
        TileManager.Instance.cancelAction();
    }

    public void CommandToTaunt()
    {
        if(_tauntCounter > 0)
        {
            return;
        }
        if (mainAction)
        {
            Debug.LogError("Taunt being called but this unity already used main action");
            return;
        }
        _tauntCounter = _tauntDuration;
        _selfTileMenu.FadeAction("Taunt", _tauntCounter);

        _canControl = false;
        HideWays();
        OnActorTaunt?.Invoke(this);
        anim.SetTrigger("Taunt");
        _tauntActive = true;

      //  _turnToEndTaunt = _tauntDuration + TileManager.Instance.CurrentTurn;
    }

    public void CommandToMove(Tile tile)
    {
        TryMove(tile);
    }
    public void CommandToAttack(Tile tile)
    {
        TryAttack(tile);
    }

    public void Act(Tile tile)
    {
        if (!_canControl)
        {
            return;
        }
        if (tile.tileActor == null)
        {
            OpenTileOptions(tile, _emptyTileMenu);
            return;
        }

        string otherTag = tile.tileActor.tag;

        if(tile == currentTile)
        {
            OpenTileOptions(tile, _selfTileMenu);
        }

        else if (otherTag.Contains("Hero"))
        {
            Debug.Log("Clique em aliado");
            OpenTileOptions(tile, _friendlyTileMenu);
            return;
        }
        else if (otherTag.Contains("Enemy"))
        {
            OpenTileOptions(tile, _enemyTileMenu);       
        }
        if (mainAction && moveAction)
        {
            StartCoroutine(EndAction());
        }
    }

    private void TryAttack(Tile tile)
    {
        if (euclidianDistance(this, tile.tileActor) > attackRange)
        {
            TileManager.Instance.ShowFeedbackMesage(tile, "Out of Range");
            Debug.Log("Enemy out of Range");
            return;
        }
        if (mainAction)
        {
            Debug.Log("Ja atacaou neste turno");
            return;
        }
        else
        {
            _canControl = false;
            _currentEnemy = tile.tileActor;
            transform.LookAt(_currentEnemy.transform);
            HideWays();
            anim.SetTrigger("Attack");
            OnActorStartAttack?.Invoke(this);
        }
    }

    public override void ResetActions()
    {
        base.ResetActions();
        if (_tauntActive)
        {
            if (_tauntCounter > 0)
            {
                _tauntCounter--;
                _selfTileMenu.FadeAction("Taunt", _tauntCounter);
            }
            if (_tauntCounter == 0)
            {
                _selfTileMenu.RemoveFade("Taunt");
                OnActorEndTaunt?.Invoke(this);
                _tauntActive = false;
            }
        }
        /*
        if (_turnToEndTaunt == TileManager.Instance.CurrentTurn)
        {
            _selfTileMenu.RemoveFade("Taunt");
            OnActorEndTaunt?.Invoke(this);
        }*/
    }



    public void FinishedSpin()
    {
        _canControl = true;
        mainAction = true;

        showWays(posX, posY);

        OnActorFinishAttack?.Invoke(this);

        if (mainAction && moveAction)
        {
            StartCoroutine(EndAction());
        }

    }
    private IEnumerator EndAction()
    {
        yield return null;
        /*
        if (KilledEnemyOnTurn)
        {
            yield return new WaitUntil(() => ReadyToEndTurn);
        }*/
        TileManager.Instance.SendMessage("endAction");
    }

    public void SpinAttackHit()
    {
        List<Actor> adjacentActors = TileManager.Instance.GetAdjacentActors(currentTile);
        foreach (var actor in adjacentActors)
        {
            Fight(actor);
        }

    }

    public void AttackHit()
    {
        Fight(_currentEnemy);
    }

    public void FinishedAttack()
    {
        _canControl = true;
        mainAction = true;
        showWays(posX, posY);
        OnActorFinishAttack?.Invoke(this);

        if (mainAction && moveAction)
        {
            StartCoroutine(EndAction());
        }
    }

    public void FinishedTaunt()
    {
        _canControl = true;
        mainAction = true;
        showWays(posX, posY);

        OnActorEndTauntAnimation?.Invoke(this);

        if (mainAction && moveAction)
        {
            StartCoroutine(EndAction());
        }
    }


    public void showWays(int x, int y)
    {
        // Marcas para direita e esquerda
        spawnBlueMark("x", getMoveDis());
        spawnBlueMark("x", -getMoveDis());

        // Marcas para cima e baixo
        spawnBlueMark("z", getMoveDis());
        spawnBlueMark("z", -getMoveDis());

        GameObject go = Instantiate(blueMark) as GameObject;
        go.transform.position = transform.position;
        go.transform.SetParent(transform);

    }

    private void spawnBlueMark(string axis, int pos = 1, int defaultZ = 0 )
    {
        if (pos == 0) {
            return;
        }

        int spawnX = posX;
        int spawnZ = posY + defaultZ;

        if (axis == "x")
        {
            spawnX += pos;
        }
        else if (axis == "z")
        {
            spawnBlueMark("x", getMoveDis(), pos);
            spawnBlueMark("x", -getMoveDis(), pos);

            spawnZ += pos;
        }

        pos += pos < 0 ? 1 : -1;

        if (spawnX >= TileManager.Instance.gridX || spawnX < 0) {
            spawnBlueMark(axis, pos, defaultZ);
            return;
        }

        if (spawnZ >= TileManager.Instance.gridX || spawnZ < 0) {
            spawnBlueMark(axis, pos, defaultZ);
            return;
        }

        Tile tile = TileManager.Instance.getObjectOnPosition(spawnX, spawnZ);

        GameObject mark = null;

        if (tile.tileActor != null) {
            if (tile.tileActor.gameObject.tag.Contains("Hero")) {

                mark = Instantiate(friendMark) as GameObject;
            } else if (tile.tileActor.gameObject.tag.Contains("Enemy")) {

                mark = Instantiate(enemyMark) as GameObject;
            }

        } else {
            mark = Instantiate(blueMark) as GameObject;
        }

        mark.transform.SetParent(transform);
        mark.transform.position = new Vector3(
            tile.transform.position.x,
            transform.position.y - 0.01f,
            tile.transform.position.z
        );

        spawnBlueMark(axis, pos, defaultZ);
    }

    public void HideWays()
    {
        GameObject[] marks;
        marks = GameObject.FindGameObjectsWithTag("MoveMark");

        foreach (GameObject mark in marks) {
            Destroy(mark);
        }
    }

    public void selectHero()
    {
        isSelected = true;
        HighLight();
        showWays(posX,posY);
    }
       
    public void unSelect()
    {
        HideWays();
        isSelected = false;
        UnLight();
    }
    float euclidianDistance(Actor A1, Actor A2)
    {
        int x1, x2, y1, y2;
        x1 = (int)A1.getPos().x;
        y1 = (int)A1.getPos().y;
        x2 = (int)A2.getPos().x;
        y2 = (int)A2.getPos().y;

        return (float)Math.Sqrt(((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1)));
    }
    internal void setId(int _id)
    {
        id = _id;
    }

    void OnDestroy()
    {
        EnemiesController.Instance.RemoveHeroFromList(this);
        TileManager.Instance.RemoveHeroFromList(this);
    }

}
