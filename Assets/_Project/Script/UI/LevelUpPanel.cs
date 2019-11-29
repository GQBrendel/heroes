using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpPanel : MonoBehaviour
{
    [Header("Images")]
    [SerializeField] private Image _portraitImage;
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI _nameLevelMessage;
    [SerializeField] private TextMeshProUGUI _previousValues;
    [SerializeField] private TextMeshProUGUI _currentValues;
    [Header("Buttons")]
    [SerializeField] private Button _continueGameButton;
    [Header("New Skill")]
    [SerializeField] private GameObject _newSkillPanel;
    [SerializeField] private TextMeshProUGUI _skillName;
    [SerializeField] private TextMeshProUGUI _skillDescription;
    [SerializeField] private Image _skillImage;

    private void Start()
    {
        _continueGameButton.onClick.AddListener(() => gameObject.SetActive(false));
    }
    public void UpdateUI(CharacterInfo previousInfo, CharacterInfo CurrentInfo, Skill skill = null)
    {
        _portraitImage.sprite = CurrentInfo.Image;
        _nameLevelMessage.SetText(CurrentInfo.Name + " - level " + CurrentInfo.Level);

        _previousValues.SetText(
            previousInfo.MaxHP.ToString() + "\n" +
            previousInfo.Strength.ToString() + "\n" +
            previousInfo.Dexterity.ToString() + "\n" +
            previousInfo.Constitution.ToString() + "\n" +
            previousInfo.Intelligence.ToString() + "\n" +
            previousInfo.Speed.ToString() + "\n" +
            previousInfo.Astral.ToString() + "\n");

        _currentValues.SetText(
            CurrentInfo.MaxHP.ToString() + "\n" +
            CurrentInfo.Strength.ToString() + "\n" +
            CurrentInfo.Dexterity.ToString() + "\n" +
            CurrentInfo.Constitution.ToString() + "\n" +
            CurrentInfo.Intelligence.ToString() + "\n" +
            CurrentInfo.Speed.ToString() + "\n" +
            CurrentInfo.Astral.ToString() + "\n");

        if (skill)
        {
            _newSkillPanel.SetActive(true);
            _skillName.SetText(skill.Name);
            _skillDescription.SetText(skill.Description);
            _skillImage.sprite = skill.Sprite;
        }
        else
        {
            _newSkillPanel.SetActive(false);
        }
    }
}
