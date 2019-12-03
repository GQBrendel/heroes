using AStar_2D.Demo;
using Fungus;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NarrativeManager : MonoBehaviour
{
    [SerializeField] private GameObject _introPanel;
    [SerializeField] private Button _skipButton;
    [SerializeField] private Flowchart _flowChart;

    [SerializeField] private Button _nextSentenceButon;
    [SerializeField] private TextMeshProUGUI[] _sentences;

    [SerializeField] private IntroMessages _introMessages;

    private bool _isEpilogue;

    public int _currentSentenceIndex = 0;

    public void ShowLastMessage()
    {
        _isEpilogue = true;
        _introPanel.SetActive(true);
    }

    private void ShowNextSentence()
    {
        _sentences[_currentSentenceIndex].gameObject.SetActive(false);
        _currentSentenceIndex++;
        if (_currentSentenceIndex == _sentences.Length)
        {
            if (_isEpilogue)
            {
                SceneManager.LoadScene(0);
            }
            else
            {
                FinishIntro();
            }
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
        SetMessages();
    }

    private void SetMessages()
    {
        for (int i = 0; i < _sentences.Length; i++)
        {
            _sentences[i].text = _introMessages.Messages[i];
        }
    }    

    private void FinishIntro()
    {
        _introPanel.SetActive(false);
        _flowChart.SendFungusMessage("Start");
    }

    public void ReadyToStartBattle()
    {
        TileManager.Instance.ShouldExecuteActions = true;
    }
}
