using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AStar_2D.Demo;
using System;

public class Actor : MonoBehaviour
{
    public delegate void ActorAttackHandler(Actor actor);
    public ActorAttackHandler OnActorStartAttack;
    public ActorAttackHandler OnActorFinishAttack;

    public delegate void StatusHandler(Actor actor);
    public StatusHandler OnActorTaunt;
    public StatusHandler OnActorEndTaunt;
    public StatusHandler OnActorEndTauntAnimation;
    public StatusHandler OnActorFrosted;
    public StatusHandler OnActorEndFrosted;

    [SerializeField] private Camera m_Camera;

    public Animator anim;
    private AnimatedAgent animatedAgent;
    public Tile currentTile;
    public bool isSelected = false;
    public bool acted = false;
    public int posX, posY;
    public int attack, defense;
    public int specialAttackDamage;
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
    public Camera Camera
    {
        get { return m_Camera; }
        set { m_Camera = value; }
    }

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

    private IEnumerator KillActor()
    {
        AStar_2D.Demo.TileManager.Instance.setActorOnPosition(posX, posY, null);

        if (tag.Contains("Hero"))
        {
            AStar_2D.Demo.TileManager.Instance.alliesNumber--;
            EnemiesController.Instance.IdentifyPlayers();
        }
        else if (tag.Contains("Enemy"))
        {
            AStar_2D.Demo.TileManager.Instance.enemiesNumber--;
            EnemiesController.Instance.IdentifyEnemies();
            EnemiesController.Instance.RemoveEnemyFromList(this as Enemy);
        }

        currentTile.toggleWalkable();
        yield return new WaitForSeconds(3f);
   
        Destroy(gameObject);
    }
       
    public void TryMove(AStar_2D.Demo.Tile tileDestino)
    {
        animatedAgent = GetComponent<AStar_2D.Demo.AnimatedAgent>();

        if(tileDestino == currentTile)
        {

        }

        else if (animatedAgent.IsMoving || !tileDestino.IsWalkable) {
            return;
        }

        if (tileDestino.posX > posX + moveDis)
        {
            Debug.Log("Não posso mover pois o destino é " + tileDestino.posX + " minha pos em X é " + posX + " e meu movimento é " + moveDis);
            animatedAgent.setDestination(tileDestino.WorldPosition);    //Manda Mover   
            currentTile.toggleWalkable();                               //Marca o tile como não caminhável
            currentTile = tileDestino;                                  //Altera o tile atual do personagem
            setPos((int)tileDestino.getPos().x, (int)tileDestino.getPos().y);   //Redefine a posição do Actor  
            if (tag.Contains("Hero"))
            {
                GetComponent<AnimatedAgent>().moved = false;
                StartCoroutine(controlMovement());
            }
//            return;
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
       // if (KilledEnemyOnTurn)
       // {
       //     yield return new WaitForSeconds(3f);
       // }
        if (finishedAllActions())
        {
            TileManager.Instance.SendMessage("endAction");
            rotate = true;
            StartCoroutine(SetRotateToFalse());

        }
        else if (!mainAction)
            GetComponent<HeroController>().showWays(posX,posY);
    }
    public bool finishedAllActions()
    {
        if (mainAction && moveAction)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private IEnumerator SetRotateToFalse()
    {
        yield return new WaitForSeconds(1);
        rotate = false;
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

    protected void Fight (Actor opponent, bool specialAttack = false)
    {
        int attackValue = specialAttack ? specialAttackDamage : attack;
        opponent.TakeDamage(attackValue - opponent.defense);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health < 0) {
            health = 0;
        }

        float scaleX = health / maxHealth;

        anim.SetTrigger("Damage");
        anim.SetBool("Dead", health <= 0);
        healthBar.transform.localScale = new Vector3(scaleX, 1f, 1f);
        if (health <= 0)
        {
            StartCoroutine(KillActor());
        }
    }

    public void Heal(int healValue)
    {
        health += healValue;

        if (health > maxHealth)
        {
            health = maxHealth;
        }

        float scaleX = health / maxHealth;
        anim.SetTrigger("Healed");
        healthBar.transform.localScale = new Vector3(scaleX, 1f, 1f);
    }

    public virtual void ResetActions()
    {
        acted = false;
        moveAction = false;
        mainAction = false;
        GetComponent<AnimatedAgent>().moved = false;
    }

    public virtual void GetFrosted(int duration)
    {

    }
}

