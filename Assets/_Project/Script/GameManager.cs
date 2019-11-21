﻿using AStar_2D.Demo;
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
    private void Update ()
    {
		if (EnemiesController.Instance.enemyUnits == 0) {
			_canvasManager.SendMessage("setVictory");
		}

		if (EnemiesController.Instance.heroUnits == 0) {
			_canvasManager.SendMessage("setDefeat");
		}
	}
}
