using AStar_2D.Demo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : Actor
{
    public GameObject blueMark;
    public GameObject friendMark;
    public GameObject enemyMark;

    [SerializeField] private Interactable _emptyTileInteractiblePrefab;
    [SerializeField] private Interactable _enemyTileInteractiblePrefab;
    [SerializeField] private Interactable _friendlyTileInteractiblePrefab;
    [SerializeField] private Interactable _selfTileInteractiblePrefab;
    [SerializeField] private PetSummon _petSummon;

    [Range(1, 5), SerializeField] private int _tauntDuration;
    [Range(1, 5), SerializeField] private int _specialAttackCoolDownTime;
    [Range(1, 5), SerializeField] protected int _secondSpecialAttackCoolDownTime;
    [Range(1, 5), SerializeField] private int _frostDuration;

    private int _tauntCounter;
    private int _spinAttackCounter;
    protected int _frostAttackCounter;
    private int _petAttackCounter;

    private bool _tauntActive;

    protected bool CanControl { get; set; }

    private Interactable _emptyTileMenu;
    protected Interactable _friendlyTileMenu;
    protected Interactable _selfTileMenu;
    protected Interactable _enemyTileMenu;

    public Actor CurrentEnemy { get; set; }
    protected Actor CurrentAlly { get; set; }

    public int id = 0;

    protected virtual void Start()
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
        if (_petSummon)
        {
            _petSummon.OnEnemyHit += HandlePetHit;
            _petSummon.OnEnemyFinishedAttack += HandlePetFinishedAttack;
        }
        CanControl = true;
    }

    void Update()
    {
        if (!CanControl)
        {
            return;
        }

        if (rotate)
        {
            float speed = 5;

            GameObject closestEnemy = FindClosestObjWithTag("Enemy");

            if (closestEnemy != null)
            {
                Quaternion targetRotation;
                targetRotation = Quaternion.LookRotation(closestEnemy.transform.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
            }
        }
    }

    private void OpenTileOptions(Tile tile, Interactable interactableType)
    {
        RadialMenuSpawner.instance.SpawnMenu(interactableType, this, tile);
    }

    public virtual void CommandToThunder(Tile tile)
    {

    }
    public virtual void CommandToHeal(Tile tile)
    {

    }

    public void CommandToCancelAction()
    {
        TileManager.Instance.cancelAction();
    }

    public void CommandToFrost(Tile tile)
    {
        if (_frostAttackCounter > 0)
        {
            return;
        }
        if (mainAction)
        {
            Debug.LogError("Frost Attack being called but this unity already used main action");
            return;
        }

        if (TryAttack(tile))
        {
            anim.SetTrigger("Frost");
        }
        else
        {
            return;
        }

        _frostAttackCounter = _secondSpecialAttackCoolDownTime;
        //_enemyTileMenu.FadeAction("Frost", _frostAttackCounter);
        //_enemyTileMenu.FadeAction("Pet", _petAttackCounter);
        FadeActions();
    }

    public void CommandToSummonPet(Tile tile)
    {
        if (_petAttackCounter > 0)
        {
            return;
        }
        if (mainAction)
        {
            Debug.LogError("Spin Attack being called but this unity already used main action");
            return;
        }

        _petAttackCounter = _specialAttackCoolDownTime;

        //_enemyTileMenu.FadeAction("Pet", _petAttackCounter);
        //_enemyTileMenu.FadeAction("Frost", _frostAttackCounter);
        FadeActions();

        CanControl = false;
        HideWays();

        OnActorStartAttack?.Invoke(this);

        CurrentEnemy = tile.tileActor;
        _petSummon.SummonPet(tile.transform.position);
        anim.SetTrigger("PetAttack");
    }

    private void HandlePetFinishedAttack()
    {
        FinishedSpecialAttack();
    }

    private void HandlePetHit()
    {
        Fight(CurrentEnemy, true);
    }

    public void CommandToTaunt()
    {
        if (_tauntCounter > 0)
        {
            return;
        }
        if (mainAction)
        {
            Debug.LogError("Taunt being called but this unity already used main action");
            return;
        }
        _tauntCounter = _tauntDuration;
        //_selfTileMenu.FadeAction("Taunt", _tauntCounter);
        //_selfTileMenu.FadeAction("Spin", _spinAttackCounter);
        FadeActions();

        CanControl = false;
        HideWays();
        OnActorTaunt?.Invoke(this);
        anim.SetTrigger("Taunt");
        _tauntActive = true;
    }
    internal void CommandToSpinAttack()
    {
        if (_spinAttackCounter > 0)
        {
            return;
        }
        if (mainAction)
        {
            Debug.LogError("Spin Attack being called but this unity already used main action");
            return;
        }

        _spinAttackCounter = _specialAttackCoolDownTime;

        FadeActions();

        CanControl = false;
        HideWays();
        anim.SetTrigger("Spin");
        OnActorStartAttack?.Invoke(this);
    }

    public void CommandToMove(Tile tile)
    {
        TryMove(tile);
    }
    public void CommandToAttack(Tile tile)
    {
        if (TryAttack(tile))
        {
            anim.SetTrigger("Attack");
            FadeActions();
        }
    }

    protected virtual void FadeActions()
    {
        _enemyTileMenu.FadeAction("Attack");

        if(gameObject.name == "Brute(Clone)")
        {
            _selfTileMenu.FadeAction("Spin", _spinAttackCounter);
            _selfTileMenu.FadeAction("Taunt", _tauntCounter);
        }
        else if (gameObject.name == "Archer(Clone)")
        {
            _enemyTileMenu.FadeAction("Frost", _frostAttackCounter);
            _enemyTileMenu.FadeAction("Pet", _petAttackCounter);
        }
    }

    public void Act(Tile tile)
    {
        if (!CanControl)
        {
            return;
        }
        if (tile.tileActor == null)
        {
            OpenTileOptions(tile, _emptyTileMenu);
            return;
        }

        string otherTag = tile.tileActor.tag;

        if (tile == currentTile)
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

    protected bool TryAttack(Tile tile)
    {
        if (EuclidianDistance(this, tile.tileActor) > attackRange)
        {
            TileManager.Instance.ShowFeedbackMesage(tile, "Out of Range");
            Debug.Log("Enemy out of Range");
            return false;
        }
        if (mainAction)
        {
            Debug.Log("Ja atacaou neste turno");
            return false;
        }
        else
        {
            CanControl = false;
            CurrentEnemy = tile.tileActor;
            transform.LookAt(CurrentEnemy.transform);
            HideWays();
            OnActorStartAttack?.Invoke(this);
            return true;
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
                if (_tauntCounter == 0)
                {
                    _selfTileMenu.RemoveFade("Taunt");
                    OnActorEndTaunt?.Invoke(this);
                    _tauntActive = false;
                }
                else
                {
                    _selfTileMenu.FadeAction("Taunt", _tauntCounter);
                }
            }
        }
        else
        {
            _selfTileMenu.RemoveFade("Taunt");
        }

        if (_frostAttackCounter > 0)
        {
            _frostAttackCounter--;
            if (_frostAttackCounter == 0)
            {
                _enemyTileMenu.RemoveFade("Frost");
            }
            else
            {
                _enemyTileMenu.FadeAction("Frost", _frostAttackCounter);
            }
        }
        else
        {
            _enemyTileMenu.RemoveFade("Frost");
        }
        if (_spinAttackCounter > 0)
        {
            _spinAttackCounter--;
            if (_spinAttackCounter == 0)
            {
                _selfTileMenu.RemoveFade("Spin");
            }
            else
            {
                _selfTileMenu.FadeAction("Spin", _spinAttackCounter);
            }
        }
        else
        {
            _selfTileMenu.RemoveFade("Spin");
        }
        
        if (_petAttackCounter > 0)
        {
            _petAttackCounter--;
            if (_petAttackCounter == 0)
            {
                _enemyTileMenu.RemoveFade("Pet");
            }
            else
            {
                _enemyTileMenu.FadeAction("Pet", _petAttackCounter);
            }
        }
        else
        {
            _enemyTileMenu.RemoveFade("Pet");
        }

        _enemyTileMenu.RemoveFade("Attack");
    }

    public void FinishedSpin()
    {
        FinishedSpecialAttack();
    }

    private void FinishedSpecialAttack()
    {
        CanControl = true;
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

    public virtual void AttackHit()
    {
        Fight(CurrentEnemy);
    }

    public void FrostAttackHit()
    {
        CurrentEnemy.TakeDamage(0);
        CurrentEnemy.GetFrosted(_frostDuration);
    }

    public void FinishedAttack()
    {
        CanControl = true;
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
        CanControl = true;
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

    public virtual void SelectHero()
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
    protected float EuclidianDistance(Actor A1, Actor A2)
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
