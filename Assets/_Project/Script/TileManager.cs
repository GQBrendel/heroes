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
        public delegate void HeroSpawnHandler(List<Actor> actors);
        public delegate void TurnOverHandler();

        public HeroSpawnHandler OnAllHeroesSpawned;
        public TurnOverHandler OnTurnOver;

        public static TileManager Instance;
        public int alliesNumber, enemiesNumber;
        [SerializeField] private GameObject _mage;
        public GameObject brute;
        public GameObject archer;
        public GameObject skelletonPrefab;
        public GameObject selectedHero;
        [SerializeField] private GameObject _feedbackMessage;
        [SerializeField] private DamagePopUp _damagePopUp;

        [SerializeField] MainInfoPanel _mainInfoPanel;
        public HeroController MovingHero;
        public HeroController AttackingHero;
        public HeroController FrostingHero;
        public HeroController PetHero;
        public HeroController ThunderHero;
        public HeroController HealingHero;



        public GameObject UiIcon;
        public List<GameObject> heroesList;
        private List<Actor> _heroes;
        private EnemiesController tacticalAgent;
        bool aHeroIsSelected;
        public GameObject myCamera;
        int nActions = 0;
        bool soDeTesteMudarDepois = false;

        public int CurrentTurn { get; set; }

        [SerializeField] private List<Vector2> _notwalkable;


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

            CurrentTurn = 1;
            _heroes = new List<Actor>();
            Instance = this;
            tiles = new Tile[gridX, gridY];

            for (int i = 0; i < gridX; i++)
            {
                for (int j = 0; j < gridY; j++)
                {
                    // Create the tile at its location
                    GameObject obj = MonoBehaviour.Instantiate(tilePrefab, new Vector3((i - (gridX / 2)) * 0.6f, 0, (j - (gridY / 2)) * 0.6f), Quaternion.identity) as GameObject;
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

            if (GameObject.Find("TacticalAgent") != null)
                tacticalAgent = GameObject.Find("TacticalAgent").GetComponent<EnemiesController>();

            _feedbackMessage.gameObject.SetActive(false);
        }
        private void Start()
        {
            spawnActors();
            SetupNotWalkableTiles();
        }

        private void spawnActors()
        {
            GenerateActor(brute, 7, 1);
            GenerateActor(archer, 8, 0);
            GenerateActor(_mage, 6, 0);

          //  GenerateActor(skelletonPrefab, 8, 2);
            GenerateActor(skelletonPrefab, 4, 8);
            GenerateActor(skelletonPrefab, 10, 9);
            GenerateActor(skelletonPrefab, 6, 10);
            GenerateActor(skelletonPrefab, 2, 14);
            GenerateActor(skelletonPrefab, 13, 14);

            OnAllHeroesSpawned?.Invoke(_heroes);

            tacticalAgent.IdentifyEnemies();
            tacticalAgent.IdentifyPlayers();
        }

        private void SetupNotWalkableTiles()
        {
            foreach(var tile in tiles)
            {
                if (_notwalkable.Contains(tile.getPos()))
                {
                    tile.toggleWalkable();
                }
            }
        }

        public void ShowFeedbackMesage(Tile tile, string message)
        {
            _feedbackMessage.GetComponentInChildren<TMPro.TextMeshPro>().text  = message;
            _feedbackMessage.transform.position = tile.transform.position;
            _feedbackMessage.gameObject.SetActive(true);
        }

        public void ShowDamageMessage(Tile tile, int damageAmount, bool isCritical)
        {
            DamagePopUp.Create(_damagePopUp.gameObject, tile.DamagePosiiton.position, damageAmount, isCritical);
        }

        /// <summary>
        /// Called by Unity.
        /// Left blank for demonstration.
        /// </summary>
        public void Update()
        {
            
        }

        private void onTileSelectedMouse(Tile tile, int mouseButton)
        {
            if(mouseButton!= 0)
            {
                return;
            }

            if (MovingHero)
            {
                if (tile.IsWalkable)
                {
                    MovingHero.CommandToMove(tile);
                }
                return;
            }
            else if (AttackingHero)
            {
                if(AttackingHero == tile.tileActor)
                {
                    AttackingHero.HideWays();
                    AttackingHero = null;
                    pickHero((int)tile.getPos().x, (int)tile.getPos().y); 
                }
                else if (tile.tileActor != null)
                {
                    AttackingHero.CommandToAttack(tile);                   
                }
                return;
            }
            else if (FrostingHero)
            {
                if (FrostingHero == tile.tileActor)
                {
                    FrostingHero.HideWays();
                    FrostingHero = null;
                    pickHero((int)tile.getPos().x, (int)tile.getPos().y);
                }
                else if (tile.tileActor != null)
                {
                    FrostingHero.CommandToFrost(tile);
                }
                return;
            }
            else if (PetHero)
            {
                if (PetHero == tile.tileActor)
                {
                    PetHero.HideWays();
                    PetHero = null;
                    pickHero((int)tile.getPos().x, (int)tile.getPos().y);
                }
                else if (tile.tileActor != null)
                {
                    PetHero.CommandToSummonPet(tile);
                }
                return;
            }
            else if (ThunderHero)
            {
                if (ThunderHero == tile.tileActor)
                {
                    ThunderHero.HideWays();
                    ThunderHero = null;
                    pickHero((int)tile.getPos().x, (int)tile.getPos().y);
                }
                else if (tile.tileActor != null)
                {
                    ThunderHero.CommandToThunder(tile);
                }
                return;
            }
            else if (HealingHero)
            {
              //  if (HealingHero == tile.tileActor)
                {
               //     HealingHero.HideWays();
                //    HealingHero = null;
                //    HealingHero.CommandToHeal(tile);
//                    pickHero((int)tile.getPos().x, (int)tile.getPos().y);
                }
                /*else*/ if (tile.tileActor != null)
                {
                    HealingHero.CommandToHeal(tile);
                }
                return;
            }

            if (!aHeroIsSelected)
            {
                pickHero((int)tile.getPos().x, (int)tile.getPos().y); //Pega o heroi naquela posição
                if (selectedHero == null) {
                    return;
                }

            }
            else if (aHeroIsSelected)
            {
                HeroController heroScript = selectedHero.GetComponent<HeroController>();
               // selectedHero.GetComponent<HeroController>().Act(tile);
            }
        }

        public List<Actor> GetAdjacentActors(Tile tileReference)
        {
            List<Actor> result = new List<Actor>();
            int x = tileReference.posX;
            int y = tileReference.posY;
            
            foreach (var tile in tiles)
            {
                Actor tileActor = null;

                if (tile.getPos() == new Vector2(x-1, y+1))
                {
                    tileActor = tile.tileActor;
                }
                else if (tile.getPos() == new Vector2(x, y+1))
                {
                    tileActor = tile.tileActor;
                }
                else if (tile.getPos() == new Vector2(x+1, y+1))
                {
                    tileActor = tile.tileActor;
                }
                else if (tile.getPos() == new Vector2(x + 1, y))
                {
                    tileActor = tile.tileActor;
                }
                else if (tile.getPos() == new Vector2(x + 1, y - 1))
                {
                    tileActor = tile.tileActor;
                }
                else if (tile.getPos() == new Vector2(x , y - 1))
                {
                    tileActor = tile.tileActor;
                }
                else if (tile.getPos() == new Vector2(x - 1, y - 1))
                {
                    tileActor = tile.tileActor;
                }
                else if (tile.getPos() == new Vector2(x - 1, y))
                {
                    tileActor = tile.tileActor;
                }

                if (tileActor)
                {
                    result.Add(tileActor);
                }
            }
            return result;
        }

        public void RemoveHeroFromList(HeroController hero)
        {
            heroesList.Remove(hero.gameObject);
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
                  //  go.name = "Hero";
                }
                else
                {
                    //go.name = "Archer";
                }
                alliesNumber++;
                heroesList.Add(go);
                _heroes.Add(go.GetComponent<Actor>());
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
                    selectedHero.GetComponent<HeroController>().SelectHero();
                    aHeroIsSelected = true;
                    break;
                }
            }
        }

        void endAction()
        {
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
            _mainInfoPanel.EnableAiPanel();
            tacticalAgent.enemiesTurn();
            StartCoroutine(waitForIAActions());
        }

        IEnumerator waitForIAActions()
        {
            yield return new WaitUntil(() => tacticalAgent.ActiveIA == false);
            nActions = 0;
            CurrentTurn++;
            OnTurnOver?.Invoke();
            Debug.Log("Turno: " + CurrentTurn);
            GameObject[] activeHeroes;
            activeHeroes = GameObject.FindGameObjectsWithTag("Hero");
            foreach (GameObject hero in activeHeroes)
            {
                hero.GetComponent<Actor>().ResetActions();
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
        public bool IsMovementValid(int x, int y)
        {
            return (tiles[x,y].tileActor == null && tiles[x, y].IsWalkable);
        }
    }
}
