using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSelector : MonoBehaviour
{
    [SerializeField] private RectTransform _menuTransform;
    [SerializeField] private ActionSelectorButton _skillLevel2;
    [SerializeField] private ActionSelectorButton _skillLevel3;

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

    public void Updatelevel(int level, HeroController controller)
    {
        if(level >= 3)
        {
            _skillLevel3.gameObject.SetActive(true);
            _skillLevel3.Active = true;
            _skillLevel3.SetController(controller);
        }
        if(level >= 2)
        {
            _skillLevel2.gameObject.SetActive(true);
            _skillLevel2.Active = true;
            _skillLevel2.SetController(controller);
        }
    }
}
