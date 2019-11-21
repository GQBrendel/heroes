using AStar_2D.Demo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterInfo))]
public class HeroController : Actor
{
    public delegate void LevelUpHandler(HeroController hero);
    public LevelUpHandler OnLevelUp;

    public GameObject blueMark;
    public GameObject friendMark;
    public GameObject enemyMark;
    [SerializeField] private GameObject _attackMark;
    [SerializeField] private GameObject _warningMark;

    [SerializeField] private Interactable _emptyTileInteractiblePrefab;
    [SerializeField] private Interactable _enemyTileInteractiblePrefab;
    [SerializeField] private Interactable _friendlyTileInteractiblePrefab;
    [SerializeField] private Interactable _selfTileInteractiblePrefab;
    [SerializeField] private ActionSelector _actionSelectorPrefab;
    [SerializeField] private ActionSelector _limitedActionSelectorPrefab;
    [SerializeField] protected CharacterInfo _characterInfo;

    [Range(1, 5), SerializeField] protected int _specialAttackCoolDownTime;
    [Range(1, 5), SerializeField] protected int _secondSpecialAttackCoolDownTime;
    [Range(1, 5), SerializeField] private int _frostDuration;

    public int FrostAttackDamage = 40;
    public int Level = 1;

    protected int _spinAttackCounter;
    private int _petAttackCounter;

    private bool _tauntActive;

    protected bool CanControl { get; set; }

    protected ActionSelector ActionSelector;
    private ActionSelector _limitedSelector;

    public Actor CurrentEnemy { get; set; }
    protected Actor CurrentAlly { get; set; }

    public int id = 0;

    protected virtual void Start()
    {
        parentStart();

        AnimatorEventListener[] animationEventListener = anim.GetBehaviours<AnimatorEventListener>();

        for (int i = 0; i < animationEventListener.Length; i++)
        {
            animationEventListener[i].Hero = this;
        }

        ActionSelector = Instantiate(_actionSelectorPrefab, transform.position, Quaternion.identity).GetComponent<ActionSelector>();
        ActionSelector.gameObject.SetActive(false);
        ActionSelector.SetController(this);

        _limitedSelector = Instantiate(_limitedActionSelectorPrefab, transform.position, Quaternion.identity).GetComponent<ActionSelector>();
        _limitedSelector.gameObject.SetActive(false);
        _limitedSelector.SetController(this);


        CanControl = true;

        StartCoroutine(FirstFrame());
    }
    private IEnumerator FirstFrame()
    {
        yield return new WaitForEndOfFrame();
        _characterInfo.UpdateCharacterInfoNoSelection(this);

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

    public virtual void CommandToFrost(Tile tile)
    {
    }

    public virtual void CommandToSummonPet(Tile tile)
    {
    }

    public virtual void SendHoverCommand(HeroesActions action)
    {
    }
    public virtual void SendLeaveHoverCommand(HeroesActions action)
    {
    }

    private bool ValidateCommand(HeroesActions action)
    {
        if(action == HeroesActions.Taunt || action == HeroesActions.Spin)
        {
            return ValidateBrute(action);
        }
        else if (action == HeroesActions.Frost || action == HeroesActions.Pet)
        {
            return ValidateArcher(action);
        }
        else if (action == HeroesActions.Thunder || action == HeroesActions.Heal)
        {
            return ValidateMage(action);
        }
        return true;
    }

    protected virtual bool ValidateBrute(HeroesActions action)
    {
        return false;
    }
    protected virtual bool ValidateArcher(HeroesActions action)
    {
        return false;
    }
    protected virtual bool ValidateMage(HeroesActions action)
    {
        return false;
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
        if (!ValidateCommand(action))
        {
            return;
        }

        ActionSelector.gameObject.SetActive(false);
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
            case HeroesActions.Thunder:
                CommandToThunderNew();
                break;
            case HeroesActions.Heal:
                CommandToHealNew();
                break;
        }
    }
    protected virtual void CommandToThunderNew()
    {
    }
    protected virtual void CommandToHealNew()
    {
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

    public virtual void CommandToTaunt()
    { 
    }
    public virtual void CommandToSpinAttack()
    {
    }

    public void CommandToMove(Tile tile)
    {
        TryMoveHero(tile);
    }
    public virtual void CommandToAttack(Tile tile)
    {
        if (TryAttack(tile, BasicAttackRange))
        {
            HideWays();
            anim.SetTrigger("Attack");
            FadeActions();
            TileManager.Instance.AttackingHero = null;
        }
    }

    protected virtual void FadeActions()
    {
    }

    public void Act(Tile tile)
    {
        if (!CanControl)
        {
            return;
        }

        string otherTag = tile.tileActor.tag;
      
        if (mainAction && moveAction)
        {
            StartCoroutine(EndAction());
        }
    }

    protected bool TryAttack(Tile tile, int checkRange)
    {
        if (tile.AttackMark == null)
        {
            TileManager.Instance.ShowFeedbackMesage(tile, "Out of Range");
            PlayOutOfRangeSound();
            return false;
        }
//        if (EuclidianDistance(this, tile.tileActor) > checkRange)
        {
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
    }

    public void FinishedSpin()
    {
        OnActorFinishSpinAttack?.Invoke(this);
        FinishedSpecialAttack();
    }

    protected void FinishedSpecialAttack()
    {
        CanControl = true;
        mainAction = true;

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
        Fight(CurrentEnemy, this);
    }

    public void FrostAttackHit()
    {
        CurrentEnemy.TakeDamage(FrostAttackDamage, this);
        CurrentEnemy.GetFrosted(_frostDuration);
    }

    public void FinishedAttack()
    {
        CanControl = true;
        mainAction = true;
        OnActorFinishAttack?.Invoke(this);
        TileManager.Instance.FrostingHero = null;
        TileManager.Instance.PetHero = null;
        TileManager.Instance.ThunderHero = null;
        TileManager.Instance.HealingHero = null;

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
            ActionSelector.gameObject.SetActive(true);
            ActionSelector.SetPosition(transform.position);
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

        tile.AttackMark = atkMark;

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

        Debug.Log("Health " + Health);
        Debug.Log("MaxHealth " + MaxhHealth);
        UpdateCharacterInfo();

    }

    protected override void UpdateCharacterInfo()
    {
        _characterInfo.UpdateCharacterInfo(this);
    }
    protected override void UpdateCharacterInfoNoSelection()
    {
        _characterInfo.UpdateCharacterInfoNoSelection(this);
    }

    public void unSelect()
    {
        HideWays();
        isSelected = false;
        UnLight();
    }


    internal void setId(int _id)
    {
        id = _id;
    }
    public virtual void PlayOutOfRangeSound()
    {
    }

    public override void KilledAnEnemy(int XPObtained)
    {
        bool levelUp = _characterInfo.ObtainXP(XPObtained);
        if (levelUp)
        {
            OnLevelUp?.Invoke(this);
        }
        UpdateCharacterInfo();
    }
    void OnDestroy()
    {
        EnemiesController.Instance.RemoveHeroFromList(this);
        TileManager.Instance.RemoveHeroFromList(this);
    }

}
