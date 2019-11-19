using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSelector : MonoBehaviour
{
    [SerializeField] private RectTransform _menuTransform;
    private ActionSelectorButton[] _actionSelectorButtons;
    private HeroController _controller;

    public void SetController(HeroController heroController)
    {
        _controller = heroController;
        _actionSelectorButtons = GetComponentsInChildren<ActionSelectorButton>();

        foreach (var button in _actionSelectorButtons)
        {
            button.SetController(heroController);
        }
    }
    public void SetPosition(Vector3 positionReference)
    {
        Vector3 actorPos = Camera.main.WorldToScreenPoint(positionReference);
        _menuTransform.transform.position = actorPos;
    }

    public void FadeAction(HeroesActions action, int coolDownTime = 0)
    {
        foreach (var button in _actionSelectorButtons)
        {
            if (button.Action == action)
            {
                button.Fade(coolDownTime);
            }
        }
    }

    internal void RemoveFade(HeroesActions action)
    {
        foreach (var button in _actionSelectorButtons)
        {
            if (button.Action == action)
            {
                button.RemoveFade();
            }
        }
    }
}
