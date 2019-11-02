using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAfterAnimation : MonoBehaviour
{
    [SerializeField] private GameObject _objectToDisable;

    public void OnAnimationEnded()
    {
        _objectToDisable.SetActive(false);
    }
}
