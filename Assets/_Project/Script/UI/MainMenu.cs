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

    [Header("new Game Menu")]
    [SerializeField] private TMPro.TMP_InputField _nameInputField;

    [SerializeField] private GameObject _startNewGamePanel;
    [SerializeField] private GameObject _inputNamePanel;
    
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _recuseButton;

    [SerializeField] private Button _confirmNameButton;
    [SerializeField] private Button _cancelNameButton;

    [Header("Grayed Buttons")]
    [SerializeField] private GameObject _greyedContinueButton;
    [SerializeField] private GameObject _uiBlocker;

    [Header("Credits")]
    [SerializeField] private GameObject _creditsPanel;

    [Header("LevelSelection")]
    [SerializeField] private GameObject _levelSelectionPanel;


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

        _nameInputField.characterLimit = 20;

        _newGameButton.onClick.AddListener(HandleNewGame);
        _confirmNameButton.onClick.AddListener(HandleConfirmNameButtonClicked);
        _cancelNameButton.onClick.AddListener(HandleCancelNameButtonClicked);
        _continueButton.onClick.AddListener(HandleContinue);
        _optionButton.onClick.AddListener(HandleOptions);
        _creditsButton.onClick.AddListener(HandleCredits);
        _exitButton.onClick.AddListener(HandleExit);
        _confirmButton.onClick.AddListener(() => {
            PlayerPrefs.DeleteAll();
            EnterInGameScene();
        });
        _recuseButton.onClick.AddListener(HandleRecuse);
    }

    private void HandleNewGame()
    {
        if (_hasSavedGame)
        {
            _startNewGamePanel.SetActive(true);
            _uiBlocker.SetActive(true);
        }
        else
        {
            _inputNamePanel.SetActive(true);
            _uiBlocker.SetActive(true);
        }
    }
    private void HandleConfirmNameButtonClicked()
    {
        if (_nameInputField.text != "")
        {
            PlayerPrefs.SetString("PlayerName", _nameInputField.text);
        }
        else
        {
            PlayerPrefs.SetString("PlayerName", "Guardião");
        }
        EnterInGameScene();
    }
    private void EnterInGameScene()
    {
        _loading.SetActive(true);
        SceneManager.LoadScene(1);
    }
    private void HandleContinue()
    {
        _levelSelectionPanel.SetActive(true);
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

    private void HandleCancelNameButtonClicked()
    {
        _uiBlocker.SetActive(false);
        _inputNamePanel.SetActive(false);
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
            PlayerPrefs.SetInt("Lydia" + "Level", 1);
            PlayerPrefs.SetInt("Yanling" + "Level", 1);
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            PlayerPrefs.SetInt("Brute" + "Level", 2);
            PlayerPrefs.SetInt("Lydia" + "Level", 2);
            PlayerPrefs.SetInt("Yanling" + "Level", 2);
        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            PlayerPrefs.SetInt("Brute" + "Level", 3);
            PlayerPrefs.SetInt("Lydia" + "Level", 3);
            PlayerPrefs.SetInt("Yanling" + "Level", 3);
        }
        else if (Input.GetKeyDown(KeyCode.F4))
        {
            PlayerPrefs.SetInt("Brute" + "Level", 4);
            PlayerPrefs.SetInt("Lydia" + "Level", 4);
            PlayerPrefs.SetInt("Yanling" + "Level", 4);
        }
        else if (Input.GetKeyDown(KeyCode.F5))
        {
            PlayerPrefs.SetInt("Brute" + "Level", 5);
            PlayerPrefs.SetInt("Lydia" + "Level", 5);
            PlayerPrefs.SetInt("Yanling" + "Level", 5);
        }

    }
}
