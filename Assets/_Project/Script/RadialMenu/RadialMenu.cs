﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialMenu : MonoBehaviour {

    [SerializeField] public Text _labelText;
    public RadialButton buttonPrefab;
    public RadialButton selected;
    private HeroController _currentHero;
    private AStar_2D.Demo.Tile _currentTile;

	public void SpawnButton (Interactable obj, HeroController heroController, AStar_2D.Demo.Tile tile) {

        _currentHero = heroController;
        _currentTile = tile;
        StartCoroutine(AnimateButtons(obj));	
	}

    IEnumerator AnimateButtons(Interactable obj)
    {
        for (int i = 0; i < obj.options.Length; i++)
        {

            RadialButton newButton = Instantiate(buttonPrefab) as RadialButton;
            newButton.transform.SetParent(transform, false);
            float theta = (2 * Mathf.PI / obj.options.Length) * i;
            float xPos = Mathf.Sin(theta);
            float yPos = Mathf.Cos(theta);
            newButton.transform.localPosition = new Vector3(xPos, yPos, 0f) * 50f;

            newButton.circle.color = obj.options[i].color;
            newButton.icon.sprite = obj.options[i].sprite;
            newButton.title = obj.options[i].title;
            newButton.myMenu = this;
            newButton.Animate();
            yield return new WaitForSeconds(0.02f);
        }
    }
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonUp(0))
        {
            if (selected)
            {
                switch (selected.title)
                {
                    case "Move":
                        _currentHero.CommandToMove(_currentTile);
                        break;
                    case "Attack":
                        _currentHero.CommandToAttack(_currentTile);
                        break;
                    case "EditDialogue":
                        break;
                }
                Debug.Log(selected.title + " is selected");
            }
            Destroy(gameObject);
        }
	}
}
