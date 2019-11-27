using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharactersInfoPanel : MonoBehaviour
{
    [Header("Brute")]
    [SerializeField] private Button _bruteButton;
    [SerializeField] private GameObject _bruteSelectionHighLight;
    [SerializeField] private GameObject _brutePanel;

    [Header("Lydia")]
    [SerializeField] private Button _lydiaButton;
    [SerializeField] private GameObject _lydiaSelectionHighLight;
    [SerializeField] private GameObject _lydiaPanel;

    [Header("Yanling")]
    [SerializeField] private Button _yanlingButton;
    [SerializeField] private GameObject _yanlingSelectionHighLight;
    [SerializeField] private GameObject _yanlingPanel;
    private void Awake()
    {
        _bruteButton.onClick.AddListener(SelectBrute);
        _lydiaButton.onClick.AddListener(SelectLydia);
        _yanlingButton.onClick.AddListener(SelectYanling);
    }
    private void SelectBrute()
    {
        _bruteSelectionHighLight.SetActive(true);
        _brutePanel.SetActive(true);

        _lydiaSelectionHighLight.SetActive(false);
        _lydiaPanel.SetActive(false);

        _yanlingSelectionHighLight.SetActive(false);
        _yanlingPanel.SetActive(false);
    }

    private void SelectLydia()
    {
        _bruteSelectionHighLight.SetActive(false);
        _brutePanel.SetActive(false);

        _lydiaSelectionHighLight.SetActive(true);
        _lydiaPanel.SetActive(true);

        _yanlingSelectionHighLight.SetActive(false);
        _yanlingPanel.SetActive(false);
    }

    private void SelectYanling()
    {
        _bruteSelectionHighLight.SetActive(false);
        _brutePanel.SetActive(false);

        _lydiaSelectionHighLight.SetActive(false);
        _lydiaPanel.SetActive(false);

        _yanlingSelectionHighLight.SetActive(true);
        _yanlingPanel.SetActive(true);
    }

}
