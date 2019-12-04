using AStar_2D.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brute : HeroController
{
    [Range(1, 5), SerializeField] private int _tauntDuration;
    [Range(1, 5), SerializeField] protected int _spinAttackCooldown;
    [Range(1, 5), SerializeField] protected int _tauntCooldown;

    private int _spinCounter = 0;
    private int _tauntCounter = 0;

    public override void AttackHit()
    {
        base.AttackHit();
        AudioManager.Instance.Play("AxeHit");
    }

    public override void CommandToAttack(Tile tile)
    {
        if (TryAttack(tile, BasicAttackRange))
        {
            AudioManager.Instance.Play("MaleAttackGrunt");
            anim.SetTrigger("Attack");
            FadeActions();
            TileManager.Instance.AttackingHero = null;
        }
    }
    public override void CommandToTaunt()
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
        _tauntCounter = _tauntCooldown;
        FadeActions();

        CanControl = false;
        HideWays();
        OnActorTaunt?.Invoke(this);
        anim.SetTrigger("Taunt");
        AudioManager.Instance.Play("BruteBattleCry");
    }

    public override void SpinAttackHit()
    {
        List<Actor> adjacentActors = TileManager.Instance.GetAdjacentActors(currentTile);
        foreach (var actor in adjacentActors)
        {
            AudioManager.Instance.Play("AxeHit");
            BasicAttackFight(actor, this);
        }
    }
    protected override bool ValidateBrute(HeroesActions action)
    {
        if (action == HeroesActions.Taunt)
        {
            return _tauntCounter == 0;
        }
        else if (action == HeroesActions.Spin)
        {
            return _spinCounter == 0;
        }
        return false;
    }

    public override void CommandToSpinAttack()
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
        _spinCounter = _spinAttackCooldown;

        FadeActions();

        CanControl = false;
        HideWays();
        anim.SetTrigger("Spin");
        AudioManager.Instance.Play("SpinAxe");
        AudioManager.Instance.Play("MaleAttackGrunt");
        OnActorStartSpinAttack?.Invoke(this);
    }
    public override void ResetActions()
    {
        base.ResetActions();
        if (_spinCounter > 0)
        {
            _spinCounter--;
            if (_spinCounter == 0)
            {
                ActionSelector.RemoveFade(HeroesActions.Spin);
            }
            else
            {
                ActionSelector.FadeAction(HeroesActions.Spin, _spinCounter);
            }
        }
        if (_tauntCounter > 0)
        {
            _tauntCounter--;
            if (_tauntCounter == 0)
            {
                ActionSelector.RemoveFade(HeroesActions.Taunt);
                OnActorEndTaunt?.Invoke(this);
            }
            else
            {
                ActionSelector.FadeAction(HeroesActions.Taunt, _tauntCounter);
            }
        }
    }
    public override void SendHoverCommand(HeroesActions action)
    {
        if(_spinCounter > 0)
        {
            return;
        }

        switch (action)
        {
            case HeroesActions.Spin:
                ShowWarningMarks();
                break;
        }
    }
    public override void SendLeaveHoverCommand(HeroesActions action)
    {
        if (_spinCounter > 0)
        {
            return;
        }
        switch (action)
        {
            case HeroesActions.Spin:
                HideWays();
                break;
        }
    }
    protected override void BasicAttackFight(Actor opponent, Actor attackingActor)
    {
        int damage = _characterInfo.Strength + _characterInfo.BasicAttack;
        opponent.TakeDamage(damage - opponent.GetCharacterDefense(), attackingActor);
    }
    public override void SelectHero()
    {
        base.SelectHero();
        PlayRandomSelectionAudio();
    }

    public override void PlayOutOfRangeSound()
    {
        AudioManager.Instance.Play("BruteFora");
    }

    private void PlayRandomSelectionAudio()
    {
        int random = Random.Range(0, 3);

        switch (random)
        {
            case 0:
                AudioManager.Instance.Play("BruteCountOnMe");
                break;
            case 1:
                AudioManager.Instance.Play("BruteHellYeah");
                break;
            case 2:
                AudioManager.Instance.Play("BruteYes");
                break;
        }
    }

    public override void PlayDamageSound()
    {
        AudioManager.Instance.Play("MaleTakeHit");
    }
    public override void PerformDeathSpecifcsActions()
    {
        AudioManager.Instance.Play("MaleDeath", true);
    }

}
