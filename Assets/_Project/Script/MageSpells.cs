using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageSpells : MonoBehaviour
{
    [SerializeField] private RFX4_EffectEvent _effectEvent;

    public void ActivateCharacterEffect()
    {
        _effectEvent.ActivateCharacterEffect();
    }
    public void ActivateEffect()
    {
        _effectEvent.ActivateEffect();
    }
}
