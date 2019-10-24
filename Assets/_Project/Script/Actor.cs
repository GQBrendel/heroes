using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
// Import the AStar_2D namespace
using AStar_2D;
using AStar_2D.Pathfinding;
using System;
using AStar_2D.Demo;

public class Actor : MonoBehaviour
{    
    public Animator anim;
    private AStar_2D.Demo.AnimatedAgent animatedAgent;
    public AStar_2D.Demo.Tile currentTile;
    public bool isSelected = false;
    public bool acted = false;
    public int posX, posY;
    public int attack, defense;
    public float maxHealth;
    public bool rotate = false;    
    public GameObject personalCanvas;

    protected bool isActing = false;
    protected int moveDis = 2;
    protected float health;
    protected Vector2 lookingAtTile;

    private Image healthBar;

    public bool moveAction, mainAction;
    public float attackRange = 1.5f;

    protected void parentStart()
    {
        this.anim = GetComponent<Animator>();
        List<Image> childrens = new List<Image>(personalCanvas.gameObject.GetComponentsInChildren<Image>());

        foreach (Image children in childrens)
        {
            if (children.tag == "HealthBar") {
                healthBar = children.gameObject.GetComponent<Image>();
            }
        }
        
        health = maxHealth;
    }

    protected void parentUpdate()
    {
        if (health <= 0) {
            killActor();
        }
    }

    public void HighLight()
    {/*
        transform.GetChild(2).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);*/
    }

    public void UnLight()
    {/*
        transform.GetChild(1).gameObject.SetActive(true);
        transform.GetChild(2).gameObject.SetActive(false);*/
    }

    public void setPos(int x, int y)
    {
        AStar_2D.Demo.TileManager.Instance.setActorOnPosition(posX, posY, null);
        posX = x;
        posY = y;
        AStar_2D.Demo.TileManager.Instance.setActorOnPosition(posX, posY, this);
    }

    public void killActor()
    {
        AStar_2D.Demo.TileManager.Instance.setActorOnPosition(posX, posY, null);

        if (tag.Contains("Hero")) {
            AStar_2D.Demo.TileManager.Instance.alliesNumber--;
            EnemiesController.Instance.IdentifyPlayers();
        } else if (tag.Contains("Enemy"))
        {
            AStar_2D.Demo.TileManager.Instance.enemiesNumber--;
            EnemiesController.Instance.IdentifyEnemies();
        }

        currentTile.toggleWalkable();
        Destroy(gameObject);
    }
       
    public void tryMove(AStar_2D.Demo.Tile tileDestino)
    {
        animatedAgent = GetComponent<AStar_2D.Demo.AnimatedAgent>();

        if (animatedAgent.IsMoving || !tileDestino.IsWalkable) {
            return;
        }

        if (tileDestino.posX > posX + moveDis)
        {
            Debug.Log("Não posso mover pois o destino é " + tileDestino.posX + " minha pos em X é " + posX + " e meu movimento é " + moveDis);
            return;
        }

        if (tileDestino.posX < posX - moveDis) {
            return;
        }

        if (tileDestino.posY > posY + moveDis) {
            return;
        }

        if (tileDestino.posY < posY - moveDis) {
            return;
        }

        transform.LookAt(tileDestino.transform);  //Olha na direção do movimento

        if (this.tag.Contains("Hero")) {
            AStar_2D.Demo.TileManager.Instance.selectedHero.GetComponent<HeroController>().HideWays();
        }

        animatedAgent.setDestination(tileDestino.WorldPosition);    //Manda Mover   
        currentTile.toggleWalkable();                               //Marca o tile como não caminhável
        currentTile = tileDestino;                                  //Altera o tile atual do personagem
        setPos((int)tileDestino.getPos().x, (int)tileDestino.getPos().y);   //Redefine a posição do Actor  
        if(tag.Contains("Hero"))
        {
            GetComponent<AnimatedAgent>().moved = false;
            StartCoroutine(controlMovement());
        }
    }
    IEnumerator controlMovement()
    {
        yield return new WaitUntil(() => GetComponent<AnimatedAgent>().moved);
        currentTile.toggleWalkable();
        checkActions();
        if (finishedAllActions())
        {
         //   TileManager.Instance.EndAction();
        }
        else if (!mainAction)
        {
            GetComponent<HeroController>().showWays(posX, posY);
        }
    }
    public bool finishedAllActions()
    {
        if (mainAction && moveAction)
            return true;
        else
            return false;
    }
    public void checkActions()
    {
        if (!moveAction)
            moveAction = true;
        else
            mainAction = true;
    }
    public GameObject FindClosestObjWithTag(string tag)
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag(tag);
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;

        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
       // rotate = true;
        return closest;
    }

    public int getMoveDis()
    {
        return moveDis;
    }

    public bool checkPos(int x, int y)
    {
        if (x == posX && y == posY) {
            return true;
        }

        return false;
    }

    public Vector2 getPos()
    {
        return new Vector2((int)posX, (int)posY);
    }

    public void setCurrentTile(AStar_2D.Demo.Tile tile)
    {
            currentTile = tile;
    }

    protected IEnumerator wait()
    {
        yield return new WaitForSeconds(2);
        rotate = false;
    }

    protected void fight (Actor opponent)
    {
        opponent.takeDamage(attack - opponent.defense);
    }

    protected void takeDamage(int damage)
    {
        health -= damage;

        if (health < 0) {
            health = 0;
        }

        float scaleX = health / maxHealth;

        healthBar.transform.localScale = new Vector3(scaleX, 1f,1f);
    }

    public void resetActions()
    {
        acted = false;
        moveAction = false;
        mainAction = false;
        GetComponent<AnimatedAgent>().moved = false;
        
    }


}

