using AStar_2D.Demo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        _tileManager.OnEnemyEndTurn += HandleEnemyTurnOver;
        _tileManager.OnHeroesEndTurn += HandleHeroesTurnOver;
        _tileManager.OnAllHeroesDead += HandleAllHeroesDead;

        _canvasManager.OnBackToMenu += HandleBackToMenu;
        _canvasManager.OnNextLevel += HandleNextLevel;
        _canvasManager.OnRetryLevel += HandleRestartLevel;
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

    private void HandleAllHeroesDead()
    {
        StartCoroutine(LevelFailed());
    }

    private void HandleTurnOver()
    {
        _currentTurn++;
    }
    private void HandleEnemyTurnOver()
    {
        _canvasManager.ShowPlayerPhaseMessage();
    }
    private void HandleHeroesTurnOver()
    {
        _canvasManager.ShowEnemyPhaseMessage();
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
        _tileManager.OnAllHeroesDead -= HandleAllHeroesDead;


        foreach (Actor actor in _heroes)
        {
            actor.OnActorStartAttack -= HandleActorAttack;
            actor.OnActorFinishAttack -= HandleActorFinishedAttack;
            actor.OnActorEndTaunt -= HandleActorEndTaunt;
        }
    }
    private void LevelUpCheat()
    {
#if UNITY_EDITOR

        if (Input.GetKeyDown(KeyCode.F1))
        {
            PlayerPrefs.SetInt("Brute" + "Level", 1);
            PlayerPrefs.SetInt("Lydia" + "Level", 1);
            PlayerPrefs.SetInt("Yanling" + "Level", 1);
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            PlayerPrefs.SetInt("Brute" + "Level", 2);
            PlayerPrefs.SetInt("Lydia" + "Level", 2);
            PlayerPrefs.SetInt("Yanling" + "Level", 2);
        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            PlayerPrefs.SetInt("Brute" + "Level", 3);
            PlayerPrefs.SetInt("Lydia" + "Level", 3);
            PlayerPrefs.SetInt("Yanling" + "Level", 3);
        }
        else if (Input.GetKeyDown(KeyCode.F4))
        {
            PlayerPrefs.SetInt("Brute" + "Level", 4);
            PlayerPrefs.SetInt("Lydia" + "Level", 4);
            PlayerPrefs.SetInt("Yanling" + "Level", 4);
        }
        else if (Input.GetKeyDown(KeyCode.F5))
        {
            PlayerPrefs.SetInt("Brute" + "Level", 5);
            PlayerPrefs.SetInt("Lydia" + "Level", 5);
            PlayerPrefs.SetInt("Yanling" + "Level", 5);
        }
        else if (Input.GetKeyDown(KeyCode.F6))
        {
            PlayerPrefs.SetInt("Brute" + "Level", 1);
            PlayerPrefs.SetInt("Lydia" + "Level", 2);
            PlayerPrefs.SetInt("Yanling" + "Level", 1);
        }
#endif
    }

    public void SaveGame(CharacterInfo character)
    {
        PlayerPrefs.SetInt(character.Name + "Level", character.Level);
        PlayerPrefs.SetInt(character.Name + "CurrentXP", character.CurrentXP);
        PlayerPrefs.SetInt("HasSavedGame", 0);
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
            StartCoroutine(LevelCompleted());
            update = false;
		}

		if (EnemiesController.Instance.heroUnits == 0)
        {
            StartCoroutine(LevelFailed());
            update = false;
		}
	}

    private IEnumerator LevelCompleted()
    {
        CharacterInfo[] characters = FindObjectsOfType<CharacterInfo>();
        foreach (var character in characters)
        {
            SaveGame(character);
        }
        int level = _tileManager.CurrentLevel + 1;
        string currentLevelToSet = "Level" + level + "Completed";
        PlayerPrefs.SetInt(currentLevelToSet, 0);
        yield return new WaitForSeconds(2f);
        if(_tileManager.CurrentLevel == 4)
        {
            SceneManager.LoadScene("Epilogue");
        }
        else
        {
            _canvasManager.ShowVictoryScreen();
        }
    }
    private IEnumerator LevelFailed()
    {
        yield return new WaitForSeconds(2f);
        _canvasManager.ShowDefeatScreen();
    }
    bool update = true;

    private void HandleRestartLevel()
    {
        SceneManager.LoadScene(_tileManager.CurrentLevel);
    }
    private void HandleNextLevel()
    {
        SceneManager.LoadScene(_tileManager.CurrentLevel + 1);
    }
    private void HandleBackToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
