using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        _heroController.SendCommand(_action);
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

    private void OnDisable()
    {
        _circle.color = _defaultColor;
    }
}
