using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// Import the AStar_2D namespace
using AStar_2D;
using AStar_2D.Pathfinding;
using System;

// Namespace
namespace AStar_2D.Demo
{
    /// <summary>
    /// Inherits the AStar class that allows the user to specify what INode to use for pathfinding, in this case Tile.
    /// By default, AIManager is a singleton which can be accessed anywhere in code using AIManager.Instance.
    /// This allows access to the pathfinding methods within.
    /// </summary>
	public class TileManager : AStarGrid
    {
        //Variaveis nossas:
        public static TileManager Instance;
        public int alliesNumber, enemiesNumber;
        public GameObject knight;
        public GameObject archer;
        public GameObject imp;
        public GameObject selectedHero;

        public GameObject UiIcon;
        public List<GameObject> heroesList;
        private EnemiesController tacticalAgent;
        bool aHeroIsSelected;
        public GameObject myCamera;
        //RaycastHit hit;
        int turnCounter = 1;
        int nActions = 0;
        bool soDeTesteMudarDepois = false;

        //Variaveis do Asset:
        private Tile[,] tiles;
        /// <summary>
        /// How many tiles to create in the X axis.
        /// </summary>
        public int gridX = 4;
        /// <summary>
        /// How many tiles to create in the Y axis.
        /// </summary>
        public int gridY = 4;
        /// <summary>
        /// The prefab that represents an individual tile.
        /// </summary>
        public GameObject tilePrefab;
        /// <summary>
        /// When true, a preview path will be shown when the mouse hovers over a tile.
        /// </summary>
        public bool showPreviewPath = false;

        // Methods
        /// <summary>
        /// Called by Unity.
        /// Note that the base method is called. This is essential to ensure that the base class initializes correctly.
        /// </summary>
        public override void Awake()
        {
            base.Awake();

            Instance = this;
            tiles = new Tile[gridX, gridY];

            for (int i = 0; i < gridX; i++)
            {
                for (int j = 0; j < gridY; j++)
                {
                    // Create the tile at its location
                    GameObject obj = MonoBehaviour.Instantiate(tilePrefab, new Vector3((i - (gridX / 2)) * 0.6f,0, (j - (gridY / 2)) * 0.6f), Quaternion.identity) as GameObject;
                   // obj.transform.rotation =  Quaternion.Euler(90, 0, 0);
                    obj.GetComponent<Tile>().setPos(i, j);
                    // Add the tile script
                    tiles[i, j] = obj.GetComponent<Tile>();
                    tiles[i, j].index = new Index(i, j);

                    // Add an event listener
                    tiles[i, j].onTileSelected += onTileSelectedMouse;

                    // Check for preview
                    if (showPreviewPath == true)
                        tiles[i, j].onTileHover += onTileHover;

                    // Add the tile as a child to keep the scene view clean
                    obj.transform.SetParent(transform);
                    obj.name = "tile " + i + " " + j;
                }
            }

            tiles[3, 3].diagonalMode = PathNodeDiagonalMode.NoDiagonal;

            // Pass the arry to the search grid
            constructGrid(tiles);
            
            if(GameObject.Find("TacticalAgent") != null)
            tacticalAgent = GameObject.Find("TacticalAgent").GetComponent<EnemiesController>();
            spawnActors();
        }

        void spawnActors()
        {
            GenerateActor(knight, 2, 0);
            GenerateActor(archer, 5, 0);
            GenerateActor(imp, 2, 7);
            GenerateActor(imp, 5, 7);

            tacticalAgent.IdentifyEnemies();
            tacticalAgent.IdentifyPlayers();
        }

        /// <summary>
        /// Called by Unity.
        /// Left blank for demonstration.
        /// </summary>
        public void Update()
        {
            // Do stuff
            if(Input.GetKeyDown(KeyCode.Q))
            {
                MoveTeste();
            }
            
        }

        private void onTileSelectedMouse(Tile tile, int mouseButton)
        {
            // Check for button
            if(mouseButton == 0 && !aHeroIsSelected)
            {
                pickHero((int)tile.getPos().x, (int)tile.getPos().y); //Pega o heroi naquela posição
                if (selectedHero == null) {
                    return;
                }

            }
            else if (mouseButton == 0 && aHeroIsSelected)
            {
                HeroController heroScript = selectedHero.GetComponent<HeroController>();

                if (tile.getPos().x == heroScript.posX && tile.getPos().y == heroScript.posY) {
                    cancelAction();
                } else {
                    selectedHero.GetComponent<HeroController>().Act(tile);
                }
            } else if (mouseButton == 1) {
                tile.toggleWalkable();
            }
        }
        public void RemoveHeroFromList(HeroController hero)
        {
            heroesList.Remove(hero.gameObject);
        }

        private void tryMove(Tile tile)
        {
            // tiles[selectedHero.GetComponent<Actor>().posX, selectedHero.GetComponent<Actor>().posY].toggleWalkable();         //Marca o tile Origem como n�o caminh�vel
            selectedHero.GetComponent<Actor>().tryMove(tile);
        } 

        private void onTileHover(Tile tile)
        {
            // Find the first agent
            Agent agent = Component.FindObjectOfType<Agent>();

            if (agent != null)
            {
                // Find the tile index
                Index current = findNearestIndex(agent.transform.position);

                // Request a path but dont assign it to the agent - this will allow the preview to be shown without the agent following it
                findPath(current, tile.index, (Path result, PathRequestStatus status) =>
                {
                    // Do nothing
                });
            }
        }

        private void GenerateActor(GameObject UnitOfType, int x, int y)
        {
            GameObject go = MonoBehaviour.Instantiate(
                UnitOfType, 
                new Vector3((x - (gridX / 2)) * 0.6f, 0, (y - (gridY / 2)) * 0.6f),
                Quaternion.identity
            ) as GameObject;

            if (UnitOfType.tag == "Enemy")
            {
                go.transform.rotation = new Quaternion(0, 90, 0, 0);
                enemiesNumber++;
            }

            else if (UnitOfType.tag == "Hero")
            {
                if(!soDeTesteMudarDepois)
                {
                    soDeTesteMudarDepois = true;
                    go.name = "Hero";
                }
                else
                {
                    go.name = "Archer";
                }
                alliesNumber++;
                heroesList.Add(go);
            }

            go.transform.SetParent(transform);
            Actor actor = go.GetComponent<Actor>();
            actor.setPos(x, y);
            actor.setCurrentTile(tiles[x, y]);
            tiles[x, y].toggleWalkable();
            tiles[x, y].tileActor = actor;
        }

        void MoveTeste()
        {
            // Set the destination
            Agent[] agents = Component.FindObjectsOfType<Agent>();

            // Set the target for all agents
            foreach (Agent agent in agents)
                agent.setDestination(tiles[0,4].WorldPosition);

        }

        void pickHero(int x, int y)
        {
            foreach (GameObject hero in heroesList)
            {
                if(hero.GetComponent<Actor>().checkPos(x,y) && !hero.GetComponent<Actor>().acted) //Se � o actor da posi��o do tile e ele ainda n�o agiu
                {
                    selectedHero = hero;
                    selectedHero.GetComponent<HeroController>().selectHero();
                    aHeroIsSelected = true;
                    break;
                }
            }
        }

        public void EndAction()
        {
            if (!selectedHero)
            {
                return;
            }
            selectedHero.GetComponent<Actor>().rotate = true;
           // tiles[selectedHero.GetComponent<Actor>().posX, selectedHero.GetComponent<Actor>().posY].toggleWalkable();
            aHeroIsSelected = false;
            selectedHero.GetComponent<HeroController>().unSelect();
            selectedHero.GetComponent<Actor>().acted = true;
            selectedHero = null;
            nActions++;
            if (nActions == heroesList.Count)
            {
                endTurn();
            }
        }

        private void endTurn()
        {
            tacticalAgent.enemiesTurn();
            StartCoroutine(waitForIAActions());
        }

        IEnumerator waitForIAActions()
        {
            yield return new WaitUntil(() => tacticalAgent.ActiveIA == false);
            nActions = 0;
            turnCounter++;
            Debug.Log("Turno: " + turnCounter);
            GameObject[] activeHeroes;
            activeHeroes = GameObject.FindGameObjectsWithTag("Hero");
            foreach (GameObject hero in activeHeroes)
            {
                hero.GetComponent<Actor>().resetActions();
            }
        }

        public void cancelAction()
        {
            if(aHeroIsSelected)
            {
                aHeroIsSelected = false;
                selectedHero.GetComponent<HeroController>().unSelect();
                selectedHero = null;

            }
        }

        void onTileSelected()
        {
            Tile tile;
            Ray ray = Camera.main.ScreenPointToRay(UiIcon.transform.position);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Debug.Log(hit.collider.name);
                if (hit.transform.gameObject.tag.Contains("Tile"))
                {
                    tile = hit.collider.gameObject.GetComponent<Tile>();
                    if (!aHeroIsSelected)
                    {
                        pickHero((int)tile.getPos().x, (int)tile.getPos().y); //Pega o heroi naquela posi��o
                        if (selectedHero == null) //Retorna se n�o tem her�i selecionado
                            return;
                    }
                    else if (aHeroIsSelected)
                    {
                        if (tile.getPos().x == selectedHero.GetComponent<Actor>().posX && tile.getPos().y == selectedHero.GetComponent<Actor>().posY)
                            cancelAction();
                        else
                        {
                            selectedHero.GetComponent<HeroController>().Act(tile);
                        }
                    }
                }
            }
        }
            
        public void Action()
        {
            onTileSelected();
        }
        public void toggleTile(Tile tile)
        {
            tile.toggleWalkable();
        }

        public Tile getObjectOnPosition(int x, int y)
        {
            return tiles[x, y];
        }

        public void setActorOnPosition(int x, int y, Actor obj)
        {
            tiles[x, y].tileActor = obj;
        }
        public int getLimitX()
        {
            return gridX * 2;
        }
        public int getLimitY()
        {
            return gridY * 2;
        }
        public Vector2 ValidMovement(int x, int y)
        {
            if(tiles[x,y].IsWalkable)
            {
                return new Vector2(x, y);
            }
            else
            {
                x++;
                ValidMovement(x, y);
                return new Vector2(x,y);
            }
        }

    }
}
