using AStar_2D.Demo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private List<Actor> _heroes;

	[SerializeField] private CanvasManager _canvasManager;
    [SerializeField] private TileManager _tileManager;
    [SerializeField] private EnemiesController _enemiesController;
    [SerializeField] private Camera _mainCamera;

    private int _currentTurn;
    private int _turnToEndTaunt;

    private void Awake()
    {
        _heroes = new List<Actor>();

        _tileManager.OnAllHeroesSpawned += HandleAllHeroesSpawned;
        _tileManager.OnTurnOver += HandleTurnOver;
    }
    private void Start()
    {
        StartAudio();
    }

    private void StartAudio()
    {
        AudioManager.Instance.Play("Ambience");
        AudioManager.Instance.Play("DungeonMusic");
    }

    private void HandleTurnOver()
    {
        _currentTurn++;
    }

    private void HandleAllHeroesSpawned(List<Actor> actors)
    {
        _heroes = actors;
        foreach(Actor actor in _heroes)
        {
            actor.OnActorStartAttack += HandleActorAttack;
            actor.OnActorFinishAttack += HandleActorFinishedAttack;
            actor.OnActorStartSpinAttack += HandleSpinAttack;
            actor.OnActorFinishSpinAttack += HandleFinishSpinAttack;

            actor.OnActorTaunt += HandleActorTaunt;
            actor.OnActorEndTaunt += HandleActorEndTaunt;
            actor.OnActorEndTauntAnimation += HandleActorEndTauntAnimation;

            HeroController hero = actor as HeroController;
            hero.OnLevelUp += HandleHeroLevelUp;
        }
    }

    private void HandleHeroLevelUp(HeroController hero)
    {

    }

    private void HandleFinishSpinAttack(Actor actor)
    {
        _mainCamera.gameObject.SetActive(true);
        actor.SpinCamera.gameObject.SetActive(false);
    }

    private void HandleSpinAttack(Actor actor)
    {
        _mainCamera.gameObject.SetActive(false);
        actor.SpinCamera.gameObject.SetActive(true);
    }

    private void HandleActorAttack(Actor actor)
    {
        _mainCamera.gameObject.SetActive(false);
        actor.Camera.gameObject.SetActive(true);
    }
    private void HandleActorFinishedAttack(Actor actor)
    {
        _mainCamera.gameObject.SetActive(true);
        actor.Camera.gameObject.SetActive(false);
    }
    private void HandleActorTaunt(Actor actor)
    {
        if (actor.CompareTag("Hero"))
        {
            _enemiesController.HeroTaunted(actor);
            _mainCamera.gameObject.SetActive(false);
            actor.Camera.gameObject.SetActive(true);
        }
        else
        {
            //Enemy taunt
        }
    }

    private void HandleActorEndTauntAnimation(Actor actor)
    {
        _mainCamera.gameObject.SetActive(true);
        actor.Camera.gameObject.SetActive(false);
    }

    private void HandleActorEndTaunt(Actor tauntUnit)
    {
        if (tauntUnit.CompareTag("Hero"))
        {
            _enemiesController.EndTaunt(tauntUnit);
        }
    }

    private void OnDestroy()
    {
        _tileManager.OnAllHeroesSpawned -= HandleAllHeroesSpawned;
        _tileManager.OnTurnOver -= HandleTurnOver;

        foreach(Actor actor in _heroes)
        {
            actor.OnActorStartAttack -= HandleActorAttack;
            actor.OnActorFinishAttack -= HandleActorFinishedAttack;
            actor.OnActorEndTaunt -= HandleActorEndTaunt;
        }
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

    private void Update ()
    {
        LevelUpCheat();
        if (!update)
        {
            return;
        }
		if (EnemiesController.Instance.enemyUnits == 0)
        {
            StartCoroutine(DelayAndSendMessage("setVictory"));
            update = false;
		}

		if (EnemiesController.Instance.heroUnits == 0)
        {
            StartCoroutine(DelayAndSendMessage("setDefeat"));
            update = false;
		}
	}


    private IEnumerator DelayAndSendMessage(string message)
    {
        yield return new WaitForSeconds(2f);
        _canvasManager.SendMessage(message);
    }
    bool update = true;
}
