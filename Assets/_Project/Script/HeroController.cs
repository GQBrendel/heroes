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
    [SerializeField] private GameObject _attackMark;
    [SerializeField] private GameObject _warningMark;

    [SerializeField] private Interactable _emptyTileInteractiblePrefab;
    [SerializeField] private Interactable _enemyTileInteractiblePrefab;
    [SerializeField] private Interactable _friendlyTileInteractiblePrefab;
    [SerializeField] private Interactable _selfTileInteractiblePrefab;
    [SerializeField] private PetSummon _petSummon;
    [SerializeField] private ActionSelector _actionSelectorPrefab;
    [SerializeField] private ActionSelector _limitedActionSelectorPrefab;

    [Range(1, 5), SerializeField] private int _tauntDuration;
    [Range(1, 5), SerializeField] protected int _specialAttackCoolDownTime;
    [Range(1, 5), SerializeField] protected int _secondSpecialAttackCoolDownTime;
    [Range(1, 5), SerializeField] private int _frostDuration;

    public int FrostAttackDamage = 40;

    private int _tauntCounter;
    protected int _spinAttackCounter;
    protected int _frostAttackCounter;
    private int _petAttackCounter;

    private bool _tauntActive;

    protected bool CanControl { get; set; }

    private ActionSelector _actionSelector;
    private ActionSelector _limitedSelector;

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

        _actionSelector = Instantiate(_actionSelectorPrefab, transform.position, Quaternion.identity).GetComponent<ActionSelector>();
        _actionSelector.gameObject.SetActive(false);
        _actionSelector.SetController(this);

        _limitedSelector = Instantiate(_limitedActionSelectorPrefab, transform.position, Quaternion.identity).GetComponent<ActionSelector>();
        _limitedSelector.gameObject.SetActive(false);
        _limitedSelector.SetController(this);


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

    public void CommandToPassTurn()
    {
        mainAction = true;
        moveAction = true;

        TileManager.Instance.SendMessage("endAction");
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

        FadeActions();

        CanControl = false;
        HideWays();

        OnActorStartAttack?.Invoke(this);

        CurrentEnemy = tile.tileActor;

        transform.LookAt(CurrentEnemy.transform);
        _petSummon.SummonPet(tile.transform.position);
        anim.SetTrigger("PetAttack");
    }

    private void HandlePetFinishedAttack()
    {
        FinishedSpecialAttack();
    }

    private void HandlePetHit()
    {
        TileManager.Instance.PetHero = null;
        Fight(CurrentEnemy, true);
    }

    public void SendHoverCommand(HeroesActions action)
    {
        switch (action)
        {
            case HeroesActions.Spin:
                ShowWarningMarks();
                break;
        }
    }
    public void SendLeaveHoverCommand(HeroesActions action)
    {
        switch (action)
        {
            case HeroesActions.Spin:
                HideWays();
                break;
        }
    }

    public void SendCommand(HeroesActions action)
    {
        if(mainAction)
        {
            if(action != HeroesActions.Move && action != HeroesActions.Passturn)
            {
                return;
            }
        }

        _actionSelector.gameObject.SetActive(false);
        _limitedSelector.gameObject.SetActive(false);
        switch (action)
        {
            case HeroesActions.Move:
                CommandToMoveNew();
                break;
            case HeroesActions.Attack:
                CommandToAttackNew();
                break;
            case HeroesActions.Passturn:
                CommandToPassTurn();
                break;
            case HeroesActions.Taunt:
                CommandToTaunt();
                break;
            case HeroesActions.Spin:
                CommandToSpinAttack();
                break;
            case HeroesActions.Frost:
                CommandToFrostNew();
                break;
            case HeroesActions.Pet:
                CommandToSummonPetNew();
                break;
        }

    }
    protected virtual void CommandToFrostNew()
    {
    }
    protected virtual void CommandToSummonPetNew()
    {
    }

    private void CommandToMoveNew()
    {
        ShowWays(posX,posY);
        TileManager.Instance.MovingHero = this;
    }
    private void CommandToAttackNew()
    {
        ShowAttackMarks(BasicAttackRange);
        TileManager.Instance.AttackingHero = this;
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
        FadeActions();

        CanControl = false;
        HideWays();
        OnActorTaunt?.Invoke(this);
        anim.SetTrigger("Taunt");
        _tauntActive = true;
    }
    public virtual void CommandToSpinAttack()
    {
        mainAction = true;
        moveAction = true;

        TileManager.Instance.SendMessage("endAction");
        rotate = true;
        StartCoroutine(SetRotateToFalse());
    }

    public void CommandToMove(Tile tile)
    {
        TryMove(tile);
    }
    public virtual void CommandToAttack(Tile tile)
    {
        if (TryAttack(tile))
        {
            HideWays();
            anim.SetTrigger("Attack");
            FadeActions();
            TileManager.Instance.AttackingHero = null;
        }
    }

    protected virtual void FadeActions()
    {
        _actionSelector.FadeAction(HeroesActions.Attack);

        _enemyTileMenu.FadeAction("Attack");

        if(gameObject.name == "Brute(Clone)")
        {
            _selfTileMenu.FadeAction("Spin", _spinAttackCounter);
            _selfTileMenu.FadeAction("Taunt", _tauntCounter);

            _actionSelector.FadeAction(HeroesActions.Spin, _spinAttackCounter);
            _actionSelector.FadeAction(HeroesActions.Taunt, _tauntCounter);

        }
        else if (gameObject.name == "Archer(Clone)")
        {
            _enemyTileMenu.FadeAction("Frost", _frostAttackCounter);
            _enemyTileMenu.FadeAction("Pet", _petAttackCounter);

          //  _actionSelector.FadeAction("Frost", _frostAttackCounter);
          //  _actionSelector.FadeAction("Pet", _petAttackCounter);
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
        if (EuclidianDistance(this, tile.tileActor) > BasicAttackRange)
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
                    _actionSelector.RemoveFade(HeroesActions.Taunt);
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

            _actionSelector.RemoveFade(HeroesActions.Taunt);
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

            _actionSelector.RemoveFade(HeroesActions.Spin);
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

        _actionSelector.RemoveFade(HeroesActions.Attack);
    }

    public void FinishedSpin()
    {
        OnActorFinishSpinAttack?.Invoke(this);
        FinishedSpecialAttack();
    }

    private void FinishedSpecialAttack()
    {
        CanControl = true;
        mainAction = true;

        //showWays(posX, posY);

        OnActorFinishAttack?.Invoke(this);

        if (finishedAllActions())
        {
            StartCoroutine(EndAction());
        }
        else
        {
            ShowOptionsforActions(true);
        }
    }

    private IEnumerator EndAction()
    {
        yield return null;
        TileManager.Instance.SendMessage("endAction");
    }

    public virtual void SpinAttackHit()
    {
    }

    public virtual void AttackHit()
    {
        Fight(CurrentEnemy);
    }

    public void FrostAttackHit()
    {
        CurrentEnemy.TakeDamage(FrostAttackDamage);
        CurrentEnemy.GetFrosted(_frostDuration);
    }

    public void FinishedAttack()
    {
        CanControl = true;
        mainAction = true;
        OnActorFinishAttack?.Invoke(this);
        TileManager.Instance.FrostingHero = null;
        TileManager.Instance.PetHero = null;

        if (finishedAllActions())
        {
            StartCoroutine(EndAction());
        }
        else
        {
            ShowOptionsforActions(true);
        }
    }

    public void FinishedTaunt()
    {
        CanControl = true;
        mainAction = true;

        OnActorEndTauntAnimation?.Invoke(this);

        if (finishedAllActions())
        {
            StartCoroutine(EndAction());
        }
        else
        {
            ShowOptionsforActions(true);
        }
    }

    public override void ShowOptionsforActions(bool limited)
    {
        if (limited)
        {
            _limitedSelector.gameObject.SetActive(true);
            _limitedSelector.SetPosition(transform.position);
        }
        else
        {
            _actionSelector.gameObject.SetActive(true);
            _actionSelector.SetPosition(transform.position);
        }
    }

    public void ShowWays(int x, int y)
    {
        if (finishedAllActions())
        {
            return;
        }

        // Marcas para direita e esquerda
        SpawnMark("x", getMoveDis());
        SpawnMark("x", -getMoveDis());

        // Marcas para cima e baixo
        SpawnMark("z", getMoveDis());
        SpawnMark("z", -getMoveDis());

        GameObject go = Instantiate(blueMark) as GameObject;
        go.transform.position = transform.position;
        go.transform.SetParent(transform);
    }

    public void ShowAttackMarks(int range)
    {
        SpawnAttackMark("x", _attackMark, range, 0, range);
        SpawnAttackMark("x", _attackMark, -range, 0, range);

        SpawnAttackMark("z", _attackMark, range, 0, range);
        SpawnAttackMark("z", _attackMark, -range, 0, range);
    }
    public void ShowWarningMarks()
    {
        SpawnAttackMark("x", _warningMark, BasicAttackRange);
        SpawnAttackMark("x", _warningMark, -BasicAttackRange);

        SpawnAttackMark("z", _warningMark, BasicAttackRange);
        SpawnAttackMark("z", _warningMark, -BasicAttackRange);
    }

    private void SpawnAttackMark(string axis, GameObject mark, int pos = 1, int defaultZ = 0, int range = 1)
    {
        if (pos == 0)
        {
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
            SpawnAttackMark("x", mark, range, pos, range);
            SpawnAttackMark("x", mark, - range, pos, range);

            spawnZ += pos;
        }

        pos += pos < 0 ? 1 : -1;

        if (spawnX >= TileManager.Instance.gridX || spawnX < 0)
        {
            SpawnAttackMark(axis, mark, pos, defaultZ, range);
            return;
        }

        if (spawnZ >= TileManager.Instance.gridX || spawnZ < 0)
        {
            SpawnAttackMark(axis, mark, pos, defaultZ, range);
            return;
        }

        Tile tile = TileManager.Instance.getObjectOnPosition(spawnX, spawnZ);

        GameObject atkMark = null;

        atkMark = Instantiate(mark) as GameObject;
        atkMark.transform.position = new Vector3(tile.transform.position.x, transform.position.y - 0.01f, tile.transform.position.z);

        SpawnAttackMark(axis, mark, pos, defaultZ, range);
    }

    private void SpawnMark(string axis, int pos = 1, int defaultZ = 0 )
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
            SpawnMark("x", getMoveDis(), pos);
            SpawnMark("x", -getMoveDis(), pos);

            spawnZ += pos;
        }

        pos += pos < 0 ? 1 : -1;

        if (spawnX >= TileManager.Instance.gridX || spawnX < 0) {
            SpawnMark(axis, pos, defaultZ);
            return;
        }

        if (spawnZ >= TileManager.Instance.gridX || spawnZ < 0) {
            SpawnMark(axis, pos, defaultZ);
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

        } else
        {
            if (tile.IsWalkable)
            {
                mark = Instantiate(blueMark) as GameObject;
            }
            else
            {
                mark = Instantiate(enemyMark) as GameObject;
            }
        }

        mark.transform.SetParent(transform);
        mark.transform.position = new Vector3(
            tile.transform.position.x,
            transform.position.y - 0.01f,
            tile.transform.position.z
        );

        SpawnMark(axis, pos, defaultZ);
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

        ShowOptionsforActions(false);
//        showWays(posX,posY);    
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

        int result = (int)Math.Sqrt(((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1)));
        Debug.Log("Distance is " + result);
        return result;
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
