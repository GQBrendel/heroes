using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionSelectorButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public HeroesActions Action;
    [SerializeField] private Image _circle;
    [SerializeField] private Image _icon;
    [SerializeField] private Color _defaultColor;
    [SerializeField] private Image _fadeEffect;
    [SerializeField] private Text _countDownText;

    private Button _currentButton;
    private HeroController _heroController;

    internal void SetController(HeroController heroController)
    {
        _heroController = heroController;
        _currentButton = GetComponent<Button>();
        RemoveFade();
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if(Time.timeScale == 0)
        {
            return;
        }
        _heroController.SendCommand(Action);
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if (Time.timeScale == 0)
        {
            return;
        }
        _heroController.SendHoverCommand(Action);
        _defaultColor = _circle.color;
        _circle.color = Color.white;
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        _heroController.SendLeaveHoverCommand(Action);
        _circle.color = _defaultColor;
    }

    public void Fade(int coolDownTime)
    {
        _fadeEffect.gameObject.SetActive(true);
        if(coolDownTime > 0)
        {
            _countDownText.gameObject.SetActive(true);
            _countDownText.text = coolDownTime.ToString();
        }
    }
    public void RemoveFade()
    {
        if (!_fadeEffect)
        {
            return;
        }
        _fadeEffect.gameObject.SetActive(false);
        _countDownText.gameObject.SetActive(false);
    }


    private void OnDisable()
    {
        _circle.color = _defaultColor;
    }
}
