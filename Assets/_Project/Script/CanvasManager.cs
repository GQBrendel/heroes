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

    [Header("Pause Menu")]
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _uiBlocker;
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _returnToMenuButton;

    [Header("Confirm Restart")]
    [SerializeField] private GameObject _confirmRestart;
    [SerializeField] private Button _yesRestartButton;
    [SerializeField] private Button _noRestartButton;

    [Header("Confirm Return to Menu")]
    [SerializeField] private GameObject _confirmReturnToMenu;
    [SerializeField] private Button _yesReturnButton;
    [SerializeField] private Button _noReturnButton;

    [Header("Info Menu")]
    [SerializeField] private GameObject _extendedInfoPanel;
    [SerializeField] private Button _infoButton;
    [SerializeField] private Button _closeInfoButton;

    private void Start()
    {
        _victoryScreen.SetActive(false);
        _defeatScreen.SetActive(false);
        _extendedInfoPanel.SetActive(false);

        _menuButtonDefeat.onClick.AddListener(() => OnBackToMenu?.Invoke());
        _menuButtonVictory.onClick.AddListener(() => OnBackToMenu?.Invoke());
        _nextLevelButton.onClick.AddListener(() => OnNextLevel?.Invoke());
        _retryButton.onClick.AddListener(() => OnRetryLevel?.Invoke());

        _resumeButton.onClick.AddListener(ResumeGame);

        _restartButton.onClick.AddListener(() => _confirmRestart.SetActive(true));
        _returnToMenuButton.onClick.AddListener(() => _confirmReturnToMenu.SetActive(true));

        _noRestartButton.onClick.AddListener(() => _confirmRestart.SetActive(false));
        _noReturnButton.onClick.AddListener(() => _confirmReturnToMenu.SetActive(false));

        _yesRestartButton.onClick.AddListener(() => OnRetryLevel?.Invoke());
        _yesReturnButton.onClick.AddListener(() => OnBackToMenu?.Invoke());

        _infoButton.onClick.AddListener(() => _extendedInfoPanel.SetActive(!_extendedInfoPanel.activeInHierarchy));
        _closeInfoButton.onClick.AddListener(() => _extendedInfoPanel.SetActive(false));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_pauseMenu.activeInHierarchy)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    private void PauseGame()
    {
        _pauseMenu.SetActive(true);
        _uiBlocker.SetActive(true);
    }
    private void ResumeGame()
    {
        _pauseMenu.SetActive(false);
        _uiBlocker.SetActive(false);
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
