using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private bool _hasSavedGame;

    [SerializeField] private GameObject _loading;

    [Header("Menu Buttons")]
    [SerializeField] private Button _newGameButton;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _optionButton;
    [SerializeField] private Button _creditsButton;
    [SerializeField] private Button _exitButton;

    [Header("Confirm new Game Menu")]
    [SerializeField] private GameObject _startNewGamePanel;
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _recuseButton;

    [Header("Grayed Buttons")]
    [SerializeField] private GameObject _greyedContinueButton;
    [SerializeField] private GameObject _uiBlocker;

    [Header("Credits")]
    [SerializeField] private GameObject _creditsPanel;


    private void Awake()
    {
        _hasSavedGame = PlayerPrefs.GetInt("HasSavedGame", 1) == 0;
        if (_hasSavedGame)
        {
            _greyedContinueButton.SetActive(false);
            _continueButton.gameObject.SetActive(true);
        }
        else
        {
            _greyedContinueButton.SetActive(true);
            _continueButton.gameObject.SetActive(false);
        }

        _newGameButton.onClick.AddListener(HandleNewGame);
        _continueButton.onClick.AddListener(HandleContinue);
        _optionButton.onClick.AddListener(HandleOptions);
        _creditsButton.onClick.AddListener(HandleCredits);
        _exitButton.onClick.AddListener(HandleExit);
        _confirmButton.onClick.AddListener(EnterInGameScene);
        _recuseButton.onClick.AddListener(HandleRecuse);
    }

    private void HandleNewGame()
    {
        if (_hasSavedGame)
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetInt("HasSavedGame", 0);
            _startNewGamePanel.SetActive(true);
            _uiBlocker.SetActive(true);
        }
        else
        {
            EnterInGameScene();
        }
    }
    private void EnterInGameScene()
    {
        _loading.SetActive(true);
        PlayerPrefs.SetInt("HasSavedGame", 0);
        SceneManager.LoadScene(1);
    }
    private void HandleContinue()
    {
        EnterInGameScene();
    }
    private void HandleOptions()
    {

    }
    private void HandleCredits()
    {
        _creditsPanel.SetActive(true);
    }
    private void HandleRecuse()
    {
        _uiBlocker.SetActive(false);
        _startNewGamePanel.SetActive(false);
    }

    private void HandleExit()
    {
        Application.Quit();
    }

    private void Update()
    {
        LevelUpCheat();
    }

    private void LevelUpCheat()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            PlayerPrefs.SetInt("Brute" + "Level", 1);
            PlayerPrefs.SetInt("Arya" + "Level", 1);
            PlayerPrefs.SetInt("Yanling" + "Level", 1);
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            PlayerPrefs.SetInt("Brute" + "Level", 2);
            PlayerPrefs.SetInt("Arya" + "Level", 2);
            PlayerPrefs.SetInt("Yanling" + "Level", 2);
        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            PlayerPrefs.SetInt("Brute" + "Level", 3);
            PlayerPrefs.SetInt("Arya" + "Level", 3);
            PlayerPrefs.SetInt("Yanling" + "Level", 3);
        }

    }
}
