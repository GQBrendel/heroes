using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LevelSelection : MonoBehaviour
{
    [SerializeField] private Button _level1Button;
    [SerializeField] private Button _level2Button;
    [SerializeField] private Button _level3Button;
    [SerializeField] private Button _level4Button;
    [SerializeField] private Button _returnButton;
    [SerializeField] private GameObject _loading;

    private void Awake()
    {
        _level1Button.onClick.AddListener(() => { _loading.SetActive(true); SceneManager.LoadScene("Level1"); });
        _level2Button.onClick.AddListener(() => { _loading.SetActive(true); SceneManager.LoadScene("Level2"); });
        _level3Button.onClick.AddListener(() => { _loading.SetActive(true); SceneManager.LoadScene("Level3"); });
        _level4Button.onClick.AddListener(() => { _loading.SetActive(true); SceneManager.LoadScene("Level4"); });
        _returnButton.onClick.AddListener(() => gameObject.SetActive(false));
    }
}
