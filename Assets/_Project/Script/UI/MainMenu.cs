using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _loading;

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            _loading.SetActive(true);
            SceneManager.LoadScene(1);
        }
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
