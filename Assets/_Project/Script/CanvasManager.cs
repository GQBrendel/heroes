using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour {

    private bool victory;
    private bool defeated;

    private GameObject uiBG;
    private Text victoryUi;
    private Text defeatedUi;

    void Awake()
    {
        if(!tag.Contains("DebugText"))
        {

            uiBG = GameObject.Find("UiBackground");
            Text[] childrens = uiBG.gameObject.GetComponentsInChildren<Text>();

            foreach (Text item in childrens)
            {
                if (item.name == "VictoryBoard")
                {
                    victoryUi = item;
                }
                else if (item.name == "DefeatBoard")
                {
                    defeatedUi = item;
                }
            }

            victoryUi.gameObject.SetActive(false);
            victory = false;
            defeatedUi.gameObject.SetActive(false);
            defeated = false;

            uiBG.SetActive(false);
        }
    }

    void Update()
    {
        if (!tag.Contains("DebugText"))
        {
            if (victory)
            {
                if (victoryUi.gameObject.active == false)
                {
                    uiBG.SetActive(true);
                    victoryUi.gameObject.SetActive(true);
                }

                if (Input.GetMouseButtonDown(0))
                {
                    SceneManager.LoadScene("Teste");
                }
            }

            if (defeated)
            {
                if (!defeatedUi.gameObject.active)
                {
                    uiBG.SetActive(true);
                    defeatedUi.gameObject.SetActive(true);
                }

                if (Input.GetMouseButtonDown(0))
                {
                    SceneManager.LoadScene("Teste");
                }
            }
        }

    }

	public void showMessage(string message)
    {
        GetComponent<Text>().text = message;
    }

    /**
     * Ativa tela de vitoria
     */
    public void setVictory()
    {
        victory = true;
    }

    /**
     * Ativa tela de gameover
     */
    public void setDefeat()
    {
        defeated = true;
    }
}
