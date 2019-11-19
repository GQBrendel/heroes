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


    public int FrostAttackRange = 2;
    public int PetAttackRange = 10;

    private RFX4_PhysicsMotion _effect;

    protected override void Start()
    {
        base.Start();
        _icyTrailParticles.gameObject.SetActive(false);
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
}
