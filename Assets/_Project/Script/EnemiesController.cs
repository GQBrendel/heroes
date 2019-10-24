using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Import the AStar_2D namespace
using AStar_2D;
using AStar_2D.Pathfinding;
using System;
using AStar_2D.Demo;

public class EnemiesController : MonoBehaviour
{
    private WaitForSeconds _waitForOneSecond = new WaitForSeconds(1f);
    public static EnemiesController Instance;
    public int enemyUnits = 0;
    public int heroUnits = 0;
    public List<GameObject> enemiesList;
    public List<GameObject> heroList;
    [HideInInspector]
    public List<GameObject> tilesList;
    GameObject destinyTile;
    GameObject closestHero;

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

    public void enemiesTurn()
    {
        ActiveIA = true;
        IdentifyEnemies();
        StartCoroutine(controlMovement());
    }

    void commandToMove(int posX, int posY)
    {
        chooseDestination(posX, posY);                                  //Verifica o destino
        activeEnemy.GetComponent<AStar_2D.Demo.AnimatedAgent>().moved = false;
        activeEnemy.tryMove(destinyTile.GetComponent<AStar_2D.Demo.Tile>());  //manda mover
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

    IEnumerator controlMovement()
    {
        for (int i = 0; i < enemyUnits; i++) {
            activeEnemy = enemiesList[i].GetComponent<Enemy>();

            if(euclidianDistance(GetClosestHero().GetComponent<Actor>(), activeEnemy) < activeEnemy.attackRange && !activeEnemy.mainAction)
            {
                activeEnemy.Attack(closestHero.GetComponent<Actor>());
                activeEnemy.mainAction = true;
                yield return _waitForOneSecond;
            }
            else
            {
                moveNextToClosestHero();
                yield return new WaitUntil(() => activeEnemy.GetComponent<AStar_2D.Demo.AnimatedAgent>().moved == true); //Espera a IA anterior terminar de se mover para chamar a próxima
                activeEnemy.currentTile.toggleWalkable();
                activeEnemy.rotate = true;
                if (!activeEnemy.finishedAllActions()) //Se não terminou todas as ações entra no loop do mesmo inimigo
                    i--;
            }

        }
        endOfIAturn();
        
    }

    void endOfIAturn()
    {
        foreach(GameObject enemy in enemiesList)
        {
            enemy.GetComponent<AStar_2D.Demo.AnimatedAgent>().moved = false;
            enemy.GetComponent<Actor>().resetActions();
        }

        ActiveIA = false;
    }

    GameObject GetClosestHero()
    {

        float minDistance = 100;

        foreach(GameObject hero in heroList)
        {
            float heroDis = euclidianDistance(hero.GetComponent<Actor>(), activeEnemy);
            if (minDistance > heroDis)
            {
                minDistance = heroDis;
                closestHero = hero;
            }
        }
     //  Debug.Log("A posição do heroi mais proximo é " + closestHero.GetComponent<Actor>().getPos());

        
        return closestHero;
    }
    void findRoute(float distance)
    {
        bestX = activeEnemy.posX;
        bestY = activeEnemy.posY;
        int heroPosX = closestHero.GetComponent<Actor>().posX;
        int heroPosY = closestHero.GetComponent<Actor>().posY;

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
        Vector2 destination =  TileManager.Instance.ValidMovement(bestX, bestY);
        bestX = (int)destination.x;
        bestY = (int)destination.y;

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
    void moveNextToClosestHero()
    {
        float distance;
        GetClosestHero();
        distance = (euclidianDistance(activeEnemy.GetComponent<Actor>(), closestHero.GetComponent<Actor>()));
        if (distance < 1.5f)
        {
           // Debug.Log("Menor que 1.5f - A Distancia Euclidiana entre o inimigo e o heroi mais próximo é " + euclidianDistance(activeEnemy.GetComponent<Actor>(), closestHero.GetComponent<Actor>()));
            return;
        }
        else
        {
            Debug.Log("Maior que 1.5f - A Distancia Euclidiana entre o inimigo e o heroi mais próximo é " + euclidianDistance(activeEnemy.GetComponent<Actor>(), closestHero.GetComponent<Actor>()));
            findRoute(distance);
            commandToMove(bestX, bestY);
        }
    }

    public void RemoveHeroFromList(HeroController hero)
    {
        heroList.Remove(hero.gameObject);
    }
}
