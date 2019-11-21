using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfo : MonoBehaviour
{
    [SerializeField] private Sprite m_PortraitImage;
    [SerializeField] private string m_CharacterName;
    [SerializeField] private string m_CharacterClass;
    [SerializeField] private CharacterLevel[] _characterLevels;

    private MainInfoPanel _mainInfoPanel;
    private MainInfoPanel _classInfoPanel;

    public string Class
    {
        get { return m_CharacterClass; }
        set { m_CharacterClass = value; }
    }

    public string Name
    {
        get { return m_CharacterName; }
        set { m_CharacterName = value; }
    }
    public Sprite Image
    {
        get { return m_PortraitImage; }
        set { m_PortraitImage = value; }
    }

    public int Level { get; set; }
    public int MaxHP { get; set; }
    public int CurrentHP { get; set; }
    public int CurrentXP { get; set; }
    public int XPToNextLevel { get; set; } = 50;

    public int Strength { get; set; }
    public int Dexterity { get; set; }
    public int Constitution { get; set; }
    public int Intelligence { get; set; }
    public int Speed { get; set; }
    public int Astral { get; set; }

    private void Start()
    {
        LinkWithInfoPanels();
        Level = PlayerPrefs.GetInt(Name + "Level", 1);
        CurrentXP = PlayerPrefs.GetInt(Name + "CurrentXP", 0);

        XPToNextLevel = _characterLevels[Level - 1].XPToNextLevel;
        UpdateLevelInfo();
    }
    private void LinkWithInfoPanels()
    {
        var panels = Resources.FindObjectsOfTypeAll<MainInfoPanel>();
        foreach (var panel in panels)
        {
            if (panel.name == Class + "InfoPanel")
            {
                _classInfoPanel = panel;
            }
            else if (panel.name == "MainInfoPanel")
            {
                _mainInfoPanel = panel;
            }
        }
    }

    public void UpdateCharacterInfo(HeroController character, bool disableAiPanel = true)
    {
        CurrentHP = (int)character.Health;
        MaxHP = (int)character.MaxhHealth;
        _mainInfoPanel.UpdateInfoUI(this, disableAiPanel);
        _classInfoPanel.UpdateInfoUI(this, disableAiPanel);
    }
    public void UpdateCharacterInfoNoSelection(HeroController character)
    {
        CurrentHP = (int)character.Health;
        MaxHP = (int)character.MaxhHealth;
        _classInfoPanel.UpdateInfoUI(this);
    }

    public bool ObtainXP(int xPObtained)
    {
        int xpExtra;
        CurrentXP += xPObtained;

        if(CurrentXP >= XPToNextLevel)
        {
            xpExtra = CurrentXP - XPToNextLevel;
            CurrentXP = xpExtra;
            Level++;
            XPToNextLevel = _characterLevels[Level-1].XPToNextLevel;
            UpdateLevelInfo();
            return true;
        }
        else
        {
            return false;
        }
    }
    private void UpdateLevelInfo()
    {
        Strength = _characterLevels[Level - 1].Strength;
        Dexterity = _characterLevels[Level - 1].Dexterity;
        Constitution = _characterLevels[Level - 1].Constitution;
        Intelligence = _characterLevels[Level - 1].Intelligence;
        Speed = _characterLevels[Level - 1].Speed;
        Astral = _characterLevels[Level - 1].Astral;
    }
}
