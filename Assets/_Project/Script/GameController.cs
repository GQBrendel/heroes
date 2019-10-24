using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	CanvasManager canvas;
	
	void Start () {
		canvas  = GameObject.Find("Canvas").GetComponent<CanvasManager>();
	}

	void Update () {
		if (EnemiesController.Instance.enemyUnits == 0) {
			canvas.SendMessage("setVictory");
		}

		if (EnemiesController.Instance.heroUnits == 0) {
			canvas.SendMessage("setDefeat");
		}
	}
}
