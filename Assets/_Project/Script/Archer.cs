using AStar_2D.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : HeroController
{
    [SerializeField] private ParticleSystem _icyTrailParticles;
    [SerializeField] private GameObject _iceArrowPrefab;
    [SerializeField] private Transform _iceArrowOrigin;
    [SerializeField] private GameObject _effectOnCollision;
    [SerializeField] private PetSummon _petSummon;

    [Range(1, 5), SerializeField] protected int _petSpellCooldown;
    [Range(1, 5), SerializeField] protected int _frostAttackCooldown;

    private int _petCounter = 0;
    private int _frostCounter = 0;


    public int FrostAttackRange = 2;
    public int PetAttackRange = 10;

    private RFX4_PhysicsMotion _effect;

    protected override void Start()
    {
        base.Start();
        _icyTrailParticles.gameObject.SetActive(false);
        _petSummon.OnEnemyHit += HandlePetHit;
        _petSummon.OnEnemyFinishedAttack += HandlePetFinishedAttack;
        
    }
    private void HandlePetHit()
    {
        TileManager.Instance.PetHero = null;
        Fight(CurrentEnemy, true);
    }
    private void HandlePetFinishedAttack()
    {
        FinishedSpecialAttack();
    }

    public override void SelectHero()
    {
        base.SelectHero();
        PlayRandomSelectionAudio();
    }

    private void PlayRandomSelectionAudio()
    {
        int random = Random.Range(0, 2);

        switch (random)
        {
            case 0:
                AudioManager.Instance.Play("ArcherYes");
                break;
            case 1:
                AudioManager.Instance.Play("ArcherReady");
                break;
        }
    }

    protected override bool ValidateArcher(HeroesActions action)
    {
        if (action == HeroesActions.Frost)
        {
            return _frostCounter == 0;
        }
        else if (action == HeroesActions.Pet)
        {
            return _petCounter == 0;
        }
        return false;
    }

    protected override void CommandToFrostNew()
    {
        TileManager.Instance.FrostingHero = this;
        ShowAttackMarks(FrostAttackRange);
    }
    protected override void CommandToSummonPetNew()
    {
        TileManager.Instance.PetHero = this;
        ShowAttackMarks(PetAttackRange);
    }

    public override void AttackHit()
    {
        base.AttackHit();
        AudioManager.Instance.Play("Bow");
    }
    public override void CommandToFrost(Tile tile)
    {
        if (mainAction)
        {
            Debug.LogError("Frost Attack being called but this unity already used main action");
            return;
        }

        if (TryAttack(tile, FrostAttackRange))
        {
            anim.SetTrigger("Frost");
        }
        else
        {
            return;
        }

        _frostCounter = _frostAttackCooldown;
        //_frostAttackCounter = _secondSpecialAttackCoolDownTime;
        //_enemyTileMenu.FadeAction("Frost", _frostAttackCounter);
        //_enemyTileMenu.FadeAction("Pet", _petAttackCounter);
        FadeActions();
    }
    public override void CommandToSummonPet(Tile tile)
    {
        if (mainAction)
        {
            Debug.LogError("Spin Attack being called but this unity already used main action");
            return;
        }

        if (TryAttack(tile, PetAttackRange))
        {
        }
        else
        {
            return;
        }

        _petCounter = _petSpellCooldown;

        FadeActions();

        CanControl = false;
        HideWays();

        OnActorStartAttack?.Invoke(this);

        CurrentEnemy = tile.tileActor;

        transform.LookAt(CurrentEnemy.transform);
        _petSummon.SummonPet(tile.transform.position);
        anim.SetTrigger("PetAttack");
    }

    public void CastIceArrow()
    {
        var effectObject = Instantiate(_iceArrowPrefab, _iceArrowOrigin.position, _iceArrowOrigin.rotation);
        _effect = effectObject.GetComponentInChildren<RFX4_PhysicsMotion>();
        _effect.OnEffectCollide += HandleEffectCollided;

    }
    private void HandleEffectCollided()
    {
        if(GetComponent<HeroController>().CurrentEnemy == null)
        {
            GetComponent<HeroController>().CurrentEnemy = FindObjectOfType<Enemy>();

        }
        Vector3 spawnPosition = GetComponent<HeroController>().CurrentEnemy.transform.position;
        var instance = Instantiate(_effectOnCollision, spawnPosition, new Quaternion()) as GameObject;
    }

    public void TurnOnParticles()
    {
        _icyTrailParticles.gameObject.SetActive(true);
    }
    public void TurnOffParticles()
    {
        _icyTrailParticles.gameObject.SetActive(false);
    }

    public override void ResetActions()
    {
        base.ResetActions();
        if (_frostCounter > 0)
        {
            _frostCounter--;
            if (_frostCounter == 0)
            {
                ActionSelector.RemoveFade(HeroesActions.Frost);
            }
            else
            {
                ActionSelector.FadeAction(HeroesActions.Frost, _frostCounter);
            }
        }
        if (_petCounter > 0)
        {
            _petCounter--;
            if (_petCounter == 0)
            {
                ActionSelector.RemoveFade(HeroesActions.Pet);
            }
            else
            {
                ActionSelector.FadeAction(HeroesActions.Pet, _petCounter);
            }
        }
    }

}
