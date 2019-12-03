using AStar_2D.Demo;
using Fungus;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
    [SerializeField] private GameObject _introPanel;
    [SerializeField] private Button _skipButton;
    [SerializeField] private Flowchart _flowChart;
    [SerializeField] private TileManagerTutorial _tileManager;

    [SerializeField] private Button _nextSentenceButon;
    [SerializeField] private TextMeshProUGUI[] _sentences;

    [SerializeField] private IntroMessages _introMessages;

    public int _currentSentenceIndex = 0;

    private void ShowNextSentence()
    {
        _sentences[_currentSentenceIndex].gameObject.SetActive(false);
        _currentSentenceIndex++;
        if(_currentSentenceIndex == _sentences.Length)
        {
            FinishIntro();
        }
        else
        {
            _sentences[_currentSentenceIndex].gameObject.SetActive(true);
        }
    }

    private void Awake()
    {
        _skipButton.onClick.AddListener(FinishIntro);
        _nextSentenceButon.onClick.AddListener(ShowNextSentence);
        _flowChart.SetStringVariable("PlayerName", PlayerPrefs.GetString("PlayerName", "Guardião"));

        if (PlayerPrefs.GetInt("HasSavedGame", 1) == 0)
        {
            _introPanel.SetActive(false);
            _tileManager.EndTutorial();
        }
        SetMessages();
    }

    private void SetMessages()
    {
        for (int i = 0; i < _sentences.Length; i++)
        {
            _sentences[i].text = _introMessages.Messages[i];
        }

    }

    public void LydiaSelected()
    {
        _flowChart.SendFungusMessage("LydiaSelected");
    }
    public void MoveSelected()
    {
        _flowChart.SendFungusMessage("MoveSelected");
    }

    private void FinishIntro()
    {
        _introPanel.SetActive(false);
        _flowChart.SendFungusMessage("Start");
    }

    public void TutorialSelect()
    {
       _tileManager.ShouldExecuteActions = true;
    }
    public void AITurn()
    {
        _tileManager.EnemiesTurnReadyStart();
        _tileManager.ShouldExecuteActions = true;
        _tileManager.EndTutorial();
    }

    public void AttackSelected()
    {
        _flowChart.SendFungusMessage("AttackSelected");
    }

    public void FinishedMovement()
    {
        _flowChart.SendFungusMessage("FinishedMovement");
    }

    public void LevelUp()
    {
        _flowChart.SendFungusMessage("LevelUpMessage");
    }
}
