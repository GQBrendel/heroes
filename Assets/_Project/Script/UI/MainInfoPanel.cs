using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum PanelType
{
    Extended,
    Short,
}

public class MainInfoPanel : MonoBehaviour
{
    [SerializeField] private PanelType _panelType;
    [Header("Images")]
    [SerializeField] private Image _portraitImage;
    [SerializeField] private Image _healthBar;
    [SerializeField] private Image _xpBar;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI _characterName;
    [SerializeField] private TextMeshProUGUI _className;
    [SerializeField] private TextMeshProUGUI _characterLevel;
    [SerializeField] private TextMeshProUGUI _currentHp;
    [SerializeField] private TextMeshProUGUI _currentXp;
 //   [Header("Info")]
  //  [SerializeField] private GameObject _infoPanel;
  //  [SerializeField] private Button _infoButton;
 //   [SerializeField] private Button _closeInfoButton;
    [Header("Images")]
    [SerializeField] private GameObject _aiTurnPanel;
    [SerializeField] private GameObject _hightLight;
    [Header("Attributes")]
    [SerializeField] private TextMeshProUGUI _strength;
    [SerializeField] private TextMeshProUGUI _dexterity;
    [SerializeField] private TextMeshProUGUI _constitution;
    [SerializeField] private TextMeshProUGUI _intelligence;
    [SerializeField] private TextMeshProUGUI _speed;
    [SerializeField] private TextMeshProUGUI _astral;

    private void Start()
    {/*
        _infoButton?.onClick.AddListener(() => { _infoPanel.SetActive(!_infoPanel.activeInHierarchy); });
        _closeInfoButton?.onClick.AddListener(() => { _infoPanel.SetActive(false); });
        if (_infoPanel)
        {
            _infoPanel.SetActive(false);
        }

        if (_aiTurnPanel)
        {
          //  _aiTurnPanel.SetActive(true);
        }*/
    } 

    public void EnableAiPanel()
    {
       // _aiTurnPanel.SetActive(true);
    }


    public void UpdateInfoUI(CharacterInfo character, bool disableAiPanel = true)
    {
        _portraitImage.sprite = character.Image;       
        _characterLevel.SetText(character.Level.ToString());
        _currentHp.SetText(character.CurrentHP + "/" + character.MaxHP);
        _currentXp.SetText(character.CurrentXP + "/" + character.XPToNextLevel);

        float scaleX = (float)character.CurrentHP / (float)character.MaxHP;
        _healthBar.transform.localScale = new Vector3(scaleX, 1f, 1f);

        scaleX = (float)character.CurrentXP / (float)character.XPToNextLevel;
        _xpBar.transform.localScale = new Vector3(scaleX, 1f, 1f);

        if (_panelType == PanelType.Short)
        {
            _characterName.SetText(character.Name);
            _className.SetText(character.Class);
        }
        else if (_panelType == PanelType.Extended)
        {
            _strength.SetText(character.Strength.ToString());
            _dexterity.SetText(character.Dexterity.ToString());
            _constitution.SetText(character.Constitution.ToString());
            _intelligence.SetText(character.Intelligence.ToString());
            _speed.SetText(character.Speed.ToString());
            _astral.SetText(character.Astral.ToString());
        }

        if (_aiTurnPanel && disableAiPanel)
        {
            _aiTurnPanel.SetActive(false);
        }
    }

    public void EnableHightLight()
    {
        _hightLight.SetActive(true);
    }
    public void DisableHighLight()
    {
        _hightLight.SetActive(false);
    }
}
