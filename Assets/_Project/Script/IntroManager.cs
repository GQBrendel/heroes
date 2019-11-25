using AStar_2D.Demo;
using Fungus;
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

    private bool _tutorialSelect;

    private void Awake()
    {
        _skipButton.onClick.AddListener(FinishIntro);
    }

    private void FinishIntro()
    {
        _introPanel.SetActive(false);
        _flowChart.SendFungusMessage("Start");
    }

    private void CheckForTutorialSelect()
    {
        if (_tutorialSelect)
        {
            return;
        }
        if (_flowChart.GetBooleanVariable("TutorialSelect"))
        {
            _tileManager.ShouldExecuteActions = true;
            _tutorialSelect = true;
        }
    }

    void Update()
    {
        CheckForTutorialSelect();
    }
}
