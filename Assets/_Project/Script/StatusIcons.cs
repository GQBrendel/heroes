using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum StatusType
{
    Taunt,
    Frost
}

public class StatusIcons : MonoBehaviour
{
    [SerializeField] private Sprite _frostSprite;
    [SerializeField] private Sprite _tauntSprite;
    [SerializeField] private Actor _actor;
    private Image[] _icons;
    private int _totalStatus;

    void Awake()
    {
        _icons = GetComponentsInChildren<Image>();
        _actor.OnActorTaunt += (Actor a) => { ShowStatus(StatusType.Taunt); };
        _actor.OnActorEndTaunt += (Actor a) => { HideStatus(StatusType.Taunt); };
        _actor.OnActorFrosted += (Actor a) => { ShowStatus(StatusType.Frost); };
        _actor.OnActorEndFrosted += (Actor a) => { HideStatus(StatusType.Frost); };

        foreach (var icon in _icons)
        {
            icon.gameObject.SetActive(false);
        }
    }
    private void ShowStatus(StatusType statusType)
    {
        switch (statusType)
        {
            case StatusType.Taunt:
                _icons[_totalStatus].sprite = _tauntSprite;
                _icons[_totalStatus].gameObject.SetActive(true);
                break;
            case StatusType.Frost:
                _icons[_totalStatus].sprite = _frostSprite;
                _icons[_totalStatus].gameObject.SetActive(true);
                break;
        }
        _totalStatus++;
    }
    private void HideStatus(StatusType statusType)
    {
        switch (statusType)
        {
            case StatusType.Taunt:
                foreach(var icon in _icons)
                {
                    if(icon.sprite == _tauntSprite)
                    {
                        icon.sprite = null;
                        icon.gameObject.SetActive(false);
                        break;
                    }
                }
                break;
            case StatusType.Frost:
                foreach (var icon in _icons)
                {
                    if (icon.sprite == _frostSprite)
                    {
                        icon.sprite = null;
                        icon.gameObject.SetActive(false);
                        break;
                    }
                }
                break;
        }
        _totalStatus--;
    }

}
