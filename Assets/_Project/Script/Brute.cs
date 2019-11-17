using AStar_2D.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brute : HeroController
{
    public override void AttackHit()
    {
        base.AttackHit();
        AudioManager.Instance.Play("AxeHit");
    }

    public override void CommandToAttack(Tile tile)
    {
        if (TryAttack(tile))
        {
            AudioManager.Instance.Play("MaleAttackGrunt");
            anim.SetTrigger("Attack");
            FadeActions();
        }
    }
    public override void SpinAttackHit()
    {
        List<Actor> adjacentActors = TileManager.Instance.GetAdjacentActors(currentTile);
        foreach (var actor in adjacentActors)
        {
            AudioManager.Instance.Play("AxeHit");
            Fight(actor);
        }

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

        _spinAttackCounter = _specialAttackCoolDownTime;

        FadeActions();

        CanControl = false;
        HideWays();
        anim.SetTrigger("Spin");
        AudioManager.Instance.Play("SpinAxe");
        AudioManager.Instance.Play("MaleAttackGrunt");
        OnActorStartAttack?.Invoke(this);
    }

    public override void PlayDamageSound()
    {
        AudioManager.Instance.Play("MaleTakeHit");
    }
    public override void PlayDeathSound()
    {
        AudioManager.Instance.Play("MaleDeath");
    }

}
