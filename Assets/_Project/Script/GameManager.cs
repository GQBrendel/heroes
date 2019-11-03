using AStar_2D.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private List<Actor> _heroes;

	[SerializeField] private CanvasManager _canvasManager;
    [SerializeField] private TileManager _tileManager;
    [SerializeField] private Camera _mainCamera;

    private void Awake()
    {
        _heroes = new List<Actor>();

        _tileManager.OnAllHeroesSpawned += HandleAllHeroesSpawned;
    }
    private void Start()
    {
    }

    private void HandleAllHeroesSpawned(List<Actor> actors)
    {
        _heroes = actors;
        foreach(Actor actor in _heroes)
        {
            actor.OnActorStartAttack += HandleActorAttack;
            actor.OnActorFinishAttack += HandleActorFinishedAttack;
        }
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

    private void OnDestroy()
    {
        _tileManager.OnAllHeroesSpawned -= HandleAllHeroesSpawned;

        foreach(Actor actor in _heroes)
        {
            actor.OnActorStartAttack -= HandleActorAttack;
            actor.OnActorFinishAttack -= HandleActorFinishedAttack;
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
