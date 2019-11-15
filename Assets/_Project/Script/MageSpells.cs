using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageSpells : MonoBehaviour
{
    [SerializeField] private GameObject _healSpellPrefab;
    [SerializeField] private Transform _healSpellOrigin;
    [SerializeField] private RFX4_EffectEvent _oneHandEffectEvent;
    [SerializeField] private RFX4_EffectEvent _twoHandEffectEvent;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            CastHealSpell();
        }
    }

    public void CastHealSpell()
    {
        Instantiate(_healSpellPrefab, _healSpellOrigin.position, _healSpellOrigin.rotation);
    }

    public void OneHandActivateEffect()
    {
        _oneHandEffectEvent.ActivateCharacterEffect();
    }
    public void OneHandReleaseEffect()
    {
        _oneHandEffectEvent.ActivateEffect();
    }

    public void TwoHandActivateEffect()
    {
        _twoHandEffectEvent.ActivateCharacterEffect();
        _twoHandEffectEvent.ActivateCharacterEffect2();
    }
    public void TwoHandReleaseEffect()
    {
        _twoHandEffectEvent.ActivateEffect();
    }
}
