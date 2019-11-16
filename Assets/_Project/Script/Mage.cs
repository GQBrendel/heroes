using AStar_2D.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage : HeroController
{
    [SerializeField, Range(1,500)] private int _healFactor;
    [Range(1, 5), SerializeField] protected int _healSpellCooldown;
    [SerializeField] private AudioSource _healSound;

    private int _healCounter;

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
        FadeActions();
    }

    public void ThunderHit()
    {
        Fight(CurrentEnemy, true);
    }
    public override void CommandToHeal(Tile tile)
    {
        if (_healCounter > 0)
        {
            return;
        }
        if (mainAction)
        {
            Debug.LogError("Heal Spell being called but this unity already used main action");
            return;
        }

        if (TryHeal(tile))
        {
            _healCounter = _healSpellCooldown;

            FadeActions();

            CurrentAlly = tile.tileActor;

            anim.SetBool("SelfHeal", tile == currentTile);
            anim.SetTrigger("Heal");
            if(tile == currentTile)
            {
                _healSound.Play();
            }
        }
    }

    protected override void FadeActions()
    {
        base.FadeActions();
        _selfTileMenu.FadeAction("Heal", _healCounter);
        _friendlyTileMenu.FadeAction("Heal", _healCounter);
        _enemyTileMenu.FadeAction("Thunder", _thunderAttackCount);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            _enemyTileMenu.FadeAction("Attack");
            _enemyTileMenu.FadeAction("Thunder", _thunderAttackCount);
        }
    }

    public void HealHit()
    {
        Heal(CurrentAlly, _healFactor);
    }

    private bool TryHeal(Tile tile)
    {
        if (tile.tileActor.FullHealth())
        {
          //  TileManager.Instance.ShowFeedbackMesage(tile, "HEALTH FULL");
          //  return false;
        }
        if (EuclidianDistance(this, tile.tileActor) > attackRange)
        {
            TileManager.Instance.ShowFeedbackMesage(tile, "Out of Range");
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


    public override void ResetActions()
    {
        base.ResetActions();
        if (_thunderAttackCount > 0)
        {
            _thunderAttackCount--;
            if (_thunderAttackCount == 0)
            {
                _enemyTileMenu.RemoveFade("Thunder");
            }
            else
            {
                _enemyTileMenu.FadeAction("Thunder", _thunderAttackCount);
            }
        }
        else
        {
            _enemyTileMenu.RemoveFade("Thunder");
        }

        if (_healCounter > 0)
        {
            _healCounter--;
            if (_healCounter == 0)
            {
                _selfTileMenu.RemoveFade("Heal");
                _friendlyTileMenu.RemoveFade("Heal");
            }
            else
            {
                _selfTileMenu.FadeAction("Heal", _healCounter);
                _friendlyTileMenu.FadeAction("Heal", _healCounter);
            }
        }
        else
        {
            _enemyTileMenu.RemoveFade("Heal");
        }
    }

}
