using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AStar_2D.Demo;

public class EnemiesController : MonoBehaviour
{
    private WaitForSeconds _waitForOneSecond = new WaitForSeconds(1f);

    private Actor _targetHero;
    private Actor _tauntHero;

    public bool _taunted;

    public static EnemiesController Instance;
    public int enemyUnits = 0;
    public int heroUnits = 0;
    public List<GameObject> enemiesList;
    public List<GameObject> heroList;
    [HideInInspector]
    public List<GameObject> tilesList;
    GameObject destinyTile;



    Enemy activeEnemy;
    Vector3 boardOffset;
    Vector3 unitsOffset;
    public bool ActiveIA = false;

    int bestX, bestY;

    void Start()
    {
        Instance = this;
        identifyTiles();
    }

    public void setOffsets(Vector3 _board, Vector3 _units)
    {
        boardOffset = _board;
        unitsOffset = _units;
    }

    public void HeroTaunted(Actor tauntUnit)
    {
        _taunted = true;
        _tauntHero = tauntUnit;
    }
    public void EndTaunt(Actor tauntUnit)
    {
        _taunted = false;
        _tauntHero = null;
    }

    public void enemiesTurn()
    {
        ActiveIA = true;
        IdentifyEnemies();
        StartCoroutine(ControlMovement());
    }

    void commandToMove(int posX, int posY)
    {
        if (activeEnemy.Frosted)
        {
            posX = activeEnemy.posX;
            posY = activeEnemy.posY;
        }
        chooseDestination(posX, posY);                                  //Verifica o destino
        activeEnemy.GetComponent<AStar_2D.Demo.AnimatedAgent>().moved = false;
        activeEnemy.TryMove(destinyTile.GetComponent<AStar_2D.Demo.Tile>());  //manda mover
        activeEnemy.GetComponent<Actor>().checkActions();
    }

    void chooseDestination(int posX, int posY)
    {
        foreach (GameObject tile in tilesList)
        {
            if(tile.GetComponent<AStar_2D.Demo.Tile>().posX == posX && tile.GetComponent<AStar_2D.Demo.Tile>().posY == posY)
            {
                destinyTile = tile;
                break;
            }
        }
    }

    public void IdentifyEnemies()
    {
        if (enemyUnits != 0) {
            enemyUnits = AStar_2D.Demo.TileManager.Instance.enemiesNumber;
            return;
        }

        enemyUnits = AStar_2D.Demo.TileManager.Instance.enemiesNumber;        
        enemiesList = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));

        int lastId = enemiesList[enemiesList.Count -1 ].GetComponent<Enemy>().id;

        foreach (GameObject enemy in enemiesList) {
            enemy.GetComponent<Enemy>().setId(lastId++);
        }
    }

    internal void RemoveEnemyFromList(Enemy enemy)
    {
        enemiesList.Remove(enemy.gameObject);
    }

    public void IdentifyPlayers()
    {
        if (heroUnits != 0) {
            heroUnits = AStar_2D.Demo.TileManager.Instance.alliesNumber;
            return;
        }

        GameObject[] activeHeroes;
        activeHeroes = GameObject.FindGameObjectsWithTag("Hero");
        foreach (GameObject hero in activeHeroes)
        {
            heroList.Add(hero);
            heroUnits++;
        }

        int lastId = heroList[heroList.Count - 1].GetComponent<HeroController>().id;

        foreach (GameObject hero in heroList)
        {
            hero.GetComponent<HeroController>().setId(lastId++);
        }
    }

    void identifyTiles()
    {
        GameObject[] tilesInScene;
        tilesInScene = GameObject.FindGameObjectsWithTag("Tile");
        foreach(GameObject tile in tilesInScene)
        {
            tilesList.Add(tile);
        }
    }

    IEnumerator ControlMovement()
    {
        yield return _waitForOneSecond;
        for (int i = 0; i < enemyUnits; i++)
        {
            activeEnemy = enemiesList[i].GetComponent<Enemy>();
            _targetHero = GetTargetHero();

            if(euclidianDistance(_targetHero, activeEnemy) < activeEnemy.BasicAttackRange && !activeEnemy.mainAction)
            {
                activeEnemy.GetComponent<Enemy>().Attack(_targetHero);

                yield return new WaitUntil(() => activeEnemy.mainAction);
            }
            else
            {
                moveNextToClosestHero();
                yield return new WaitUntil(() => activeEnemy.GetComponent<AStar_2D.Demo.AnimatedAgent>().moved == true); //Espera a IA anterior terminar de se mover para chamar a próxima
                activeEnemy.currentTile.toggleWalkable();
                activeEnemy.rotate = true;
                if (!activeEnemy.finishedAllActions()) //Se não terminou todas as ações entra no loop do mesmo inimigo
                {
                    i--;
                }
                else
                {
                    yield return _waitForOneSecond;
                }
            }
        }
        endOfIAturn();        
    }

    void endOfIAturn()
    {
        foreach(GameObject enemy in enemiesList)
        {
            enemy.GetComponent<AStar_2D.Demo.AnimatedAgent>().moved = false;
            enemy.GetComponent<Actor>().ResetActions();
        }

        ActiveIA = false;
    }

    private Actor GetTargetHero()
    {
        Actor target = null;

        if (_taunted && _tauntHero.isActiveAndEnabled)
        {
            target = _tauntHero;
        }
        else
        {
            float minDistance = 100;

            foreach (GameObject hero in heroList)
            {
                float heroDis = euclidianDistance(hero.GetComponent<Actor>(), activeEnemy);
                if (minDistance > heroDis)
                {
                    minDistance = heroDis;
                    target = hero.GetComponent<Actor>();
                }
            }
        }
        activeEnemy.TargetToLookAt = target;
        return target;
    }
    void FindRoute(float distance, Vector2 heroPostion)
    {
        bestX = activeEnemy.posX;
        bestY = activeEnemy.posY;
        int heroPosX = (int)heroPostion.x;
        int heroPosY = (int)heroPostion.y;

        for (int i = 1; i <= activeEnemy.getMoveDis(); i++)
        {
            if (distance >= 2.0f && distance <= 3.0f) //Quebra o loop quando o inimigo precisa se mover apenas 01 quadrado
                i++;
            
            if(heroPosX > activeEnemy.posX)
            {
                bestX++;
                if (bestX > TileManager.Instance.getLimitX())
                    bestX--;
            }
            else if (heroPosX < activeEnemy.posX)
            {
                bestX--;
                if (bestX < 0)
                    bestX++;
            }
            if (heroPosY > activeEnemy.posY)
            {
                bestY++;
                if (bestY > TileManager.Instance.getLimitY())
                    bestY--;
            }
            else if (heroPosY < activeEnemy.posY)
            {
                bestY--;
                if (bestY < 0)
                    bestY++;
            }
        }
        Debug.Log("Best X excolhido foi " + bestX);
        Debug.Log("Best Y escolhido foi " + bestY);

    }
    float euclidianDistance(Actor A1, Actor A2)
    {
        int x1, x2, y1, y2;
        x1 = (int)A1.getPos().x;
        y1 = (int)A1.getPos().y;
        x2 = (int)A2.getPos().x;
        y2 = (int)A2.getPos().y;

        //Debug.Log("Calculada Distancia entre " + x1 + " " + y1 + " || " + x2 + " " + y2 + " O Resultado foi: " + (float)Math.Sqrt(((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1))));
        return (float)Math.Sqrt(((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1)));
    }
    float euclidianDistance(int x1, int y1, int x2, int y2)
    {
        return (float)Math.Sqrt(((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1)));
    }

    public bool foundPosition;

    void moveNextToClosestHero()
    {
        float distance;
        _targetHero = GetTargetHero();
        distance = (euclidianDistance(activeEnemy.GetComponent<Actor>(), _targetHero));
        if (distance < 1.5f)
        {
            return;
        }
        else
        {
            int heroPosX = _targetHero.posX;
            int heroPosY = _targetHero.posY;

            Vector2 heroPos = new Vector2(heroPosX, heroPosY);

            ReValidateMovement(distance, heroPos);
            if (foundPosition)
            {
                commandToMove(bestX, bestY);
            }
            else
            {

            }
        }
    }

    private void ReValidateMovement(float distance, Vector2 heroPos)
    {
        foundPosition = true;
        FindRoute(distance, heroPos);
        int x = (int)heroPos.x;
        int y = (int)heroPos.y;

        if (!TileManager.Instance.IsMovementValid(bestX, bestY))
        {
            //Search for a valid position adjacent
            List<Vector2> adjacentPositions = Utils.GetAdjacentPoints(new Vector2(bestX, bestY));
            foreach(var point in adjacentPositions)
            {
                if(TileManager.Instance.IsMovementValid((int)point.x, (int)point.y))
                {
                    bestX = (int)point.x;
                    bestY = (int)point.y;
                    return;
                }
            }

            //Double the search
            foreach (var point in adjacentPositions)
            {
                List<Vector2> secondLayerAdjacentPoints = Utils.GetAdjacentPoints(point);
                foreach (var deepoPoint in secondLayerAdjacentPoints)
                {
                    if (TileManager.Instance.IsMovementValid((int)point.x, (int)point.y))
                    {
                        bestX = (int)point.x;
                        bestY = (int)point.y;
                        Debug.LogError("Had to appel to second layer of search");
                        return;
                    }
                }
            }

        }
    }
    public void RemoveHeroFromList(HeroController hero)
    {
        heroList.Remove(hero.gameObject);
    }
}
