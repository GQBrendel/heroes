using AStar_2D.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage : HeroController
{
    [SerializeField, Range(1,500)] private int _healFactor;
    private int _thunderAttackCount;

    public override void CommandToThunder(Tile tile)
    {
        if (_thunderAttackCount > 0)
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
            anim.SetTrigger("Thunder");
        }
        else
        {
            return;
        }

        _thunderAttackCount = _secondSpecialAttackCoolDownTime;
        _enemyTileMenu.FadeAction("Thunder", _thunderAttackCount);
    }

    public void ThunderHit()
    {
        Fight(CurrentEnemy, true);
    }
    public override void CommandToHeal(Tile tile)
    {
        CurrentAlly = tile.tileActor;

        if (TryHeal(tile))
        {
            anim.SetTrigger("Heal");
            Heal(CurrentAlly, _healFactor);
        }
    }

    private bool TryHeal(Tile tile)
    {
        if (EuclidianDistance(this, tile.tileActor) > attackRange)
        {
            TileManager.Instance.ShowFeedbackMesage(tile, "Out of Range");
            Debug.Log("Enemy out of Range");
            return false;
        }
        if (mainAction)
        {
            Debug.Log("Ja usou main action neste turno");
            return false;
        }
        else
        {
            CanControl = false;
            CurrentAlly = tile.tileActor;
            transform.LookAt(CurrentAlly.transform);
            HideWays();
            OnActorStartAttack?.Invoke(this);
            return true;
        }
    }

    private void Heal(Actor ally, int healValue)
    {
        ally.Heal(healValue);
    }

}
