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

    [Header("Skills")]
    [SerializeField] private Skill _skillLevel2;
    [SerializeField] private Skill _skillLevel3;

    private MainInfoPanel _mainInfoPanel;
    private MainInfoPanel _classInfoPanel;
    private LevelUpPanel _levelUpPanel;

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

    public int BasicAttack { get; set; }
    public int SpinAttack { get; set; }

    public int FrostShot { get; set; }
    public int PetSummon { get; set; }

    public int Heal { get; set; }
    public int Thunder { get; set; }

    private void Start()
    {
        LinkWithInfoPanels();
        Level = PlayerPrefs.GetInt(Name + "Level", 1);
        CurrentXP = PlayerPrefs.GetInt(Name + "CurrentXP", 0);

        MaxHP = _characterLevels[Level - 1].HP;
        CurrentHP = MaxHP;
        XPToNextLevel = _characterLevels[Level - 1].XPToNextLevel;
        UpdateLevelInfo();
    }
    private void LinkWithInfoPanels()
    {
      //  var panels = Resources.FindObjectsOfTypeAll<MainInfoPanel>();
     //   foreach (var panel in panels)
        {
       //     if (panel.name == Class + "InfoPanel")
            {
        //        _classInfoPanel = panel;
            }
          //  else if (panel.name == "MainInfoPanel_" + Name)
            {
               // _mainInfoPanel = panel;
            }
        }

        _classInfoPanel = InfoPanelManager.Instance.GetClassInfoPanel(this);
        _mainInfoPanel = InfoPanelManager.Instance.GetInfoPanel(this);
        var levelUp = Resources.FindObjectsOfTypeAll<LevelUpPanel>();
        _levelUpPanel = levelUp[0];
    }

    public void UpdateCharacterInfo(HeroController character, bool disableAiPanel = true)
    {
        MaxHP = _characterLevels[Level - 1].HP;
        _mainInfoPanel.UpdateInfoUI(this, disableAiPanel);
        _classInfoPanel.UpdateInfoUI(this, disableAiPanel);
    }
    public void UpdateCharacterInfoNoSelection(HeroController character)
    {
        MaxHP = _characterLevels[Level - 1].HP;
        _classInfoPanel.UpdateInfoUI(this);
    }

    public bool ObtainXP(int xPObtained)
    {
        int xpExtra;
        CurrentXP += xPObtained;

        if(CurrentXP >= XPToNextLevel)
        {
            CharacterInfo previousCharacter = GetCharacterState();

            xpExtra = CurrentXP - XPToNextLevel;
            CurrentXP = xpExtra;
            Level++;
            XPToNextLevel = _characterLevels[Level-1].XPToNextLevel;     

            UpdateLevelInfo();


            if(Level == 2)
            {
                _levelUpPanel.UpdateUI(previousCharacter, this, _skillLevel2);
            }
            else if (Level == 4)
            {
                _levelUpPanel.UpdateUI(previousCharacter, this, _skillLevel3);
            }
            else
            {
                _levelUpPanel.UpdateUI(previousCharacter, this);
            }


            StartCoroutine(DelayAndShowLevelUpPanel());
            return true;
        }
        else
        {
            return false;
        }
    }

    private IEnumerator DelayAndShowLevelUpPanel()
    {
        yield return new WaitForSeconds(1f);
        _levelUpPanel.gameObject.SetActive(true);
    }
    private CharacterInfo GetCharacterState()
    {
        CharacterInfo character = new CharacterInfo();
        character.MaxHP = MaxHP;
        character.Strength = Strength;
        character.Dexterity = Dexterity;
        character.Constitution = Constitution;
        character.Intelligence = Intelligence;
        character.Speed = Speed;
        character.Astral = Astral;

        return character;
    }
    private void UpdateLevelInfo()
    {
        
        MaxHP = _characterLevels[Level - 1].HP;
        CurrentHP = MaxHP;
        Strength = _characterLevels[Level - 1].Strength;
        Dexterity = _characterLevels[Level - 1].Dexterity;
        Constitution = _characterLevels[Level - 1].Constitution;
        Intelligence = _characterLevels[Level - 1].Intelligence;
        Speed = _characterLevels[Level - 1].Speed;
        Astral = _characterLevels[Level - 1].Astral;

        BasicAttack = _characterLevels[Level - 1].BasicAttack;
        SpinAttack = _characterLevels[Level - 1].SpinAttack;

        FrostShot = _characterLevels[Level - 1].FrostShot;
        PetSummon = _characterLevels[Level - 1].PetSummon;

        Heal = _characterLevels[Level - 1].Heal;
        Thunder = _characterLevels[Level - 1].Thunder;


}

    public void DisableHighlight()
    {
        _mainInfoPanel.DisableHighLight();
    }
    public void EnableHighLight()
    {
        _mainInfoPanel.EnableHightLight();
    }
}
