using AStar_2D.Demo;
using Fungus;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
    [SerializeField] private GameObject _introPanel;
    [SerializeField] private Button _skipButton;
    [SerializeField] private Flowchart _flowChart;
    [SerializeField] private TileManagerTutorial _tileManager;

   // private bool _tutorialSelect;
   // private bool _enemiesTurn;

    private void Awake()
    {
        _skipButton.onClick.AddListener(FinishIntro);
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
