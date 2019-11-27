using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public delegate void CommandsHandler();
    public CommandsHandler OnNextLevel;
    public CommandsHandler OnRetryLevel;
    public CommandsHandler OnBackToMenu;

    [SerializeField] private GameObject _victoryScreen;
    [SerializeField] private GameObject _defeatScreen;

    [SerializeField] private GameObject _playerPhaseMessage;
    [SerializeField] private GameObject _enemyPhaseMessage;

    [SerializeField] private Button _menuButtonVictory;
    [SerializeField] private Button _menuButtonDefeat;
    [SerializeField] private Button _nextLevelButton;
    [SerializeField] private Button _retryButton;

    void Awake()
    {
        _victoryScreen.SetActive(false);
        _defeatScreen.SetActive(false);

        _menuButtonDefeat.onClick.AddListener(() => OnBackToMenu?.Invoke());
        _menuButtonVictory.onClick.AddListener(() => OnBackToMenu?.Invoke());
        _nextLevelButton.onClick.AddListener(() => OnNextLevel?.Invoke());
        _retryButton.onClick.AddListener(() => OnRetryLevel?.Invoke());
    }

    public void ShowVictoryScreen()
    {
        _victoryScreen.SetActive(true);
    }   
    public void ShowDefeatScreen()
    {
        _defeatScreen.SetActive(true);
    }

    public void ShowPlayerPhaseMessage()
    {
        _playerPhaseMessage.SetActive(true);
        StartCoroutine(DisablePanel(_playerPhaseMessage));
    }
    public void ShowEnemyPhaseMessage()
    {
        _enemyPhaseMessage.SetActive(true);
        StartCoroutine(DisablePanel(_enemyPhaseMessage));
    }
    private IEnumerator DisablePanel(GameObject panel)
    {
        yield return new WaitForSeconds(3f);
        panel.SetActive(false);
    }

}
