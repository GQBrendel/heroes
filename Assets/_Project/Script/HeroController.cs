using AStar_2D.Demo;
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

    private Interactable _emptyTileMenu;
    private Interactable _enemyTileMenu;
    private Interactable _friendlyTileMenu;

    public int id = 0;

    private bool _canControl = true;


    private void Start ()
    {
        parentStart();
        var interactibleObject = Instantiate(_emptyTileInteractiblePrefab.gameObject, this.transform);
        _emptyTileMenu = interactibleObject.GetComponent<Interactable>();

        interactibleObject = Instantiate(_enemyTileInteractiblePrefab.gameObject, this.transform);
        _enemyTileMenu = interactibleObject.GetComponent<Interactable>();

        interactibleObject = Instantiate(_friendlyTileInteractiblePrefab.gameObject, this.transform);
        _friendlyTileMenu = interactibleObject.GetComponent<Interactable>();
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

    private void OpenTileOptions(Tile tile, Interactable interactableType)
    {
        RadialMenuSpawner.instance.SpawnMenu(interactableType, this, tile);
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
        if (tile.tileActor == null)
        {
            OpenTileOptions(tile, _emptyTileMenu);
            return;
        }

        string otherTag = tile.tileActor.tag;

        if (otherTag.Contains("Hero"))
        {
            Debug.Log("Clique em aliado");
            OpenTileOptions(tile, _friendlyTileMenu);
            return;
        }
        else if (otherTag.Contains("Enemy"))
        {
            OpenTileOptions(tile, _enemyTileMenu);       
        }
        if(mainAction && moveAction)
        TileManager.Instance.SendMessage("endAction");
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

        }
    }

    public void FinishedAttack()
    {
        _canControl = true;
        mainAction = true;
        fight(_currentEnemy);
        showWays(posX, posY);
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
