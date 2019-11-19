using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ActionSelectorButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private HeroesActions _action;
    [SerializeField] private Image _circle;
    [SerializeField] private Image _icon;
    [SerializeField] private Color _defaultColor;

    private Button _currentButton;
    private HeroController _heroController;



    internal void SetController(HeroController heroController)
    {
        _heroController = heroController;
        _currentButton = GetComponent<Button>();
        _currentButton.onClick.AddListener(HandleButtonClick);
    }

    private void HandleButtonClick()
    {
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        _heroController.SendCommand(_action);
         _circle.color = _defaultColor;
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        _defaultColor = _circle.color;
        _circle.color = Color.white;
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        _circle.color = _defaultColor;
    }
}
