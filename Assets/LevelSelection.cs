using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LevelSelection : MonoBehaviour
{
    [SerializeField] private GameObject _loading;
    [Header("Buttons")]
    [SerializeField] private Button _level1Button;
    [SerializeField] private Button _level2Button;
    [SerializeField] private Button _level3Button;
    [SerializeField] private Button _level4Button;
    [SerializeField] private Button _returnButton;

    [Header("GreyedOutEffects")]
    [SerializeField] private GameObject _greyedLevel2Button;
    [SerializeField] private GameObject _greyedLevel3Button;
    [SerializeField] private GameObject _greyedLevel4Button;

    private void Awake()
    {
        _level1Button.onClick.AddListener(() => { _loading.SetActive(true); SceneManager.LoadScene("Level1"); });
        _level2Button.onClick.AddListener(() => { _loading.SetActive(true); SceneManager.LoadScene("Level2"); });
        _level3Button.onClick.AddListener(() => { _loading.SetActive(true); SceneManager.LoadScene("Level3"); });
        _level4Button.onClick.AddListener(() => { _loading.SetActive(true); SceneManager.LoadScene("Level4"); });
        _returnButton.onClick.AddListener(() => gameObject.SetActive(false));

        if(PlayerPrefs.GetInt("Level2Completed", 1) == 0)
        {
            _greyedLevel2Button.SetActive(false);
            _level2Button.gameObject.SetActive(true);
        }
        else
        {
            _greyedLevel2Button.SetActive(true);
            _level2Button.gameObject.SetActive(false);
        }
        if (PlayerPrefs.GetInt("Level3Completed", 1) == 0)
        {
            _greyedLevel3Button.SetActive(false);
            _level3Button.gameObject.SetActive(true);
        }
        else
        {
            _greyedLevel3Button.SetActive(true);
            _level3Button.gameObject.SetActive(false);
        }
        if (PlayerPrefs.GetInt("Level4Completed", 1) == 0)
        {
            _greyedLevel4Button.SetActive(false);
            _level4Button.gameObject.SetActive(true);
        }
        else
        {
            _greyedLevel4Button.SetActive(true);
            _level4Button.gameObject.SetActive(false);
        }
    }
}
