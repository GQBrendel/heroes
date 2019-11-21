using AStar_2D.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage : HeroController
{
    [SerializeField, Range(1,500)] private int _healFactor;
    [Range(1, 5), SerializeField] protected int _healSpellCooldown;
    [Range(1, 5), SerializeField] protected int _thunderAttackCooldown;
    [SerializeField] private AudioSource _healSound;


    public int ThundertAttackRange = 5;
    public int HealAttackRange = 4;

    private int _healCounter;
    private int _thunderCounter;

    //private int _thunderAttackCount;

    protected override void Start()
    {
        base.Start();
        _healCounter = 0;
        _thunderCounter = 0;
    }

    public override void CommandToThunder(Tile tile)
    {
        if (mainAction)
        {
            Debug.LogError("Thunder Attack being called but this unity already used main action");
            return;
        }

        if (TryAttack(tile, ThundertAttackRange))
        {
            anim.SetTrigger("Thunder");
        }
        else
        {
            return;
        }

        _thunderCounter = _thunderAttackCooldown;

        ActionSelector.FadeAction(HeroesActions.Thunder, _thunderCounter);
        FadeActions();
    }

    public void ThunderHit()
    {
        mainAction = true;
        TileManager.Instance.ThunderHero = null;
        Fight(CurrentEnemy, this, true);
    }

    protected override bool ValidateMage(HeroesActions action)
    {
        if(action == HeroesActions.Thunder)
        {
            return _thunderCounter == 0;
        }
        else if(action == HeroesActions.Heal)
        {
            return _healCounter == 0;
        }
        return false;
    }

    protected override void CommandToThunderNew()
    {
        TileManager.Instance.ThunderHero = this;
        ShowAttackMarks(ThundertAttackRange);
    }
    protected override void CommandToHealNew()
    {
        TileManager.Instance.HealingHero = this;
        ShowAttackMarks(HealAttackRange);
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

            if (tile == currentTile)
            {
                _healSound.Play();
            }
        }
    }

    public override void PlayOutOfRangeSound()
    {
        AudioManager.Instance.Play("MageOutOfRange");
    }

    public void HealHit()
    {
        AudioManager.Instance.Play("MageHeal");
        ActionSelector.FadeAction(HeroesActions.Heal, _healCounter);
        TileManager.Instance.HealingHero = null;
        Heal(CurrentAlly, _healFactor);
    }

    public override void PerformDeathSpecifcsActions()
    {
        AudioManager.Instance.Play("MageDeath");
    }

    private bool TryHeal(Tile tile)
    {
        if (tile.tileActor.FullHealth())
        {
            TileManager.Instance.ShowFeedbackMesage(tile, "Full Health");
            HideWays();
            ShowOptionsforActions(false);
            TileManager.Instance.HealingHero = null;
            return false;
        }
        //if (EuclidianDistance(this, tile.tileActor) > HealAttackRange)
        if (tile.AttackMark == null && tile != currentTile)
        {
            TileManager.Instance.ShowFeedbackMesage(tile, "Out of Range");
            PlayOutOfRangeSound();
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
            UpdateCharacterInfoNoSelection();
            return true;
        }
    }

    private void Heal(Actor ally, int healValue)
    {
        ally.Heal(healValue);
    }

    public override void PlayDamageSound()
    {
        int random = Random.Range(0, 2);
        if (random == 0)
        {
            AudioManager.Instance.Play("MageHit");
        }
        else if (random == 1)
        {
            AudioManager.Instance.Play("MageHit2");
        }
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
                AudioManager.Instance.Play("MageYes");
                break;
            case 1:
                AudioManager.Instance.Play("MageReady");
                break;
        }
    }

    public override void ResetActions()
    {
        base.ResetActions();
        if(_thunderCounter > 0)
        {
            _thunderCounter--;
            if (_thunderCounter == 0)
            {
                ActionSelector.RemoveFade(HeroesActions.Thunder);
            }
            else
            {
                ActionSelector.FadeAction(HeroesActions.Thunder, _thunderCounter);
            }
        }
        if (_healCounter > 0)
        {
            _healCounter--;
            if (_healCounter == 0)
            {
                ActionSelector.RemoveFade(HeroesActions.Heal);
            }
            else
            {
                ActionSelector.FadeAction(HeroesActions.Heal, _healCounter);
            }
        }
    }

}
