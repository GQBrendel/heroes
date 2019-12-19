using UnityEngine;
using System.Collections;

// Import the AStar_2D namespace
using AStar_2D;

// Namespace
namespace AStar_2D.Demo
{
    /// <summary>
    /// This would be the base tile class for a tile based game which contains properties about the appearance etc of the tile
    /// Can inherit any class required but must implement the INode interface to be able to be used by the pathfinding system
    /// </summary>
	public class Tile : MonoBehaviour, IPathNode
	{
        [SerializeField] private MeshRenderer[] myRenderesrs;
        public Transform DamagePosiiton;
        public GameObject AttackMark;
        //Vairaiveis nossas
        public int posX, posY;
        public Actor tileActor;
        [SerializeField] private GameObject _hightLight;

        public void DisableVisibility()
        {
            foreach (var render in myRenderesrs)
            {
                render.enabled = false;
            }
        }

        //  public bool hasActor = false;
        public void setPos(int x, int y)
        {
            posX = x;
            posY = y;
        }

        public Vector2 getPos()
        {
            return new Vector2(posX, posY);
        }

        private Camera _mainCamera;
        private Ray ray;
        private RaycastHit hit;

        public bool touchingPathFlag = false;
        // Delegates
        /// <summary>
        /// Delegate used when tiles are selected by a mouse click.
        /// </summary>
        /// <param name="tile">The tile that was clicked.</param>
        /// <param name="mouseButton">The mouse button that was pressed</param>
        public delegate void TileSelectedDelegate(Tile tile, int mouseButton);

        /// <summary>
        /// Delegate used when tiles are hovered by the mouse
        /// </summary>
        /// <param name="tile"></param>
        public delegate void TileHoverDelegate(Tile tile);

        // Events
        /// <summary>
        /// Event that triggers when this tile has been selected a mouse click.
        /// Informs the tile manager so that the appropriate action can be taken.
        /// </summary>
        public event TileSelectedDelegate onTileSelected;

        /// <summary>
        /// Event that triggers when this tile is hovered by the mouse.
        /// Informs the tile manager so that a preview path can be shown.
        /// </summary>
        public event TileHoverDelegate onTileHover;
        public event TileHoverDelegate onTileExitHover;

        // Private
        [SerializeField]
        private bool walkable = true;
        private bool canSend = true;
        private float lastTime = 0;

        // Public
        /// <summary>
        /// The index into the grid that this tile is located at.
        /// </summary>
        public Index index = new Index();

        /// <summary>
        /// The method used to connect adjacent nodes.
        /// </summary>
        public PathNodeDiagonalMode diagonalMode = PathNodeDiagonalMode.UseGlobal;

		// Properties
        /// <summary>
        /// Implement the IsWalkable property in the INode interface
        /// It is up to the user to determine whether or not a tile should be walkable
        /// </summary>
        public bool IsWalkable
        {
            get { return walkable; } // Only need to implement the get but set is useful
            set { walkable = value; }
        }

        /// <summary>
        /// Implement the Weighting property in the INode interface
        /// It is up to the user to decide a suitable value between 0 and 1 which represents avoidance of tiles to a certain level
        /// e.g. Grass tiles might have a higher weighting value when there are path tile to walk on
        /// The lower the weighting value, the less resistance there will be
        /// </summary>
        public float Weighting
        {
            get { return index.Equals(Index.zero) ? 1 : 0; }
        }

        /// <summary>
        /// The position in 3D space that this tile is located at.
        /// </summary>
        public Vector3 WorldPosition
        {
            get { return transform.position; }
        }

        public PathNodeDiagonalMode DiagonalMode
        {
            get { return diagonalMode; }
        }

		// Methods
        /// <summary>
        /// Called by Unity.
        /// Left blank for demonstration.
        /// </summary>
		public void Start () 
		{
            _mainCamera = Camera.main;
		}


        public LayerMask _tileLayer;

		public void Update ()
        {
            // Make sure events can be sent
            if (canSend == false)
                return;

            if (!_mainCamera)
            {
                return;
            }

            ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray,out hit,100,_tileLayer))
            {
                if(hit.collider == GetComponent<Collider>())
                {
                    // Check for mouse button
                    if (Input.GetMouseButtonDown(0) == true)
                    {
                        // Block message sending
                        canSend = false;

                        // Trigger the event
                        if (onTileSelected != null)
                            onTileSelected(this, 0);
                    }
                    else if (Input.GetMouseButtonDown(1) == true)
                    {
                        // Block message sending
                        canSend = false;

                        // Trigger the event
                        if (onTileSelected != null)
                            onTileSelected(this, 1);
                    }
                }
               // print(hit.collider.name);
            }
        }

        public void ToggleHightLight()
        {
           _hightLight.SetActive(!_hightLight.activeInHierarchy);
        }
      /*  public void EnableHighlight()
        {
            _hightLight.SetActive(true);
        }
        public void DisableHightlight()
        {
            _hightLight.SetActive(false);
        }*/

        /// <summary>
        /// Called by Unity.
        /// </summary>
        public void LateUpdate()
        {
            if (Time.time > (lastTime + 0.2f))
            {
                canSend = true;
                lastTime = Time.time;
            }
        }

        public void OnMouseExit()
        {
            // Trigger the exit hover event
            if (onTileExitHover != null)
            {
                onTileExitHover(this);
            }
        }

        public void OnMouseEnter()
        {
            // Trigger the hover event
            if (onTileHover != null)
                onTileHover(this);
        }

        /// <summary>
        /// Called by Unity.
        /// </summary>
        public void OnMouseOver()
        {
        }
        public Material walkableMaterial;
        public Material NOTkableMaterial;

        /// <summary>
        /// Toggle the walkable state of this tile.
        /// </summary>
        public void toggleWalkable()
        {
            walkable = !walkable;

            // Get the sprite renderer
            MeshRenderer renderer = GetComponent<MeshRenderer>();

            // Check if the tile is walkable
            if(IsWalkable == true)
            {
                //renderer.material = walkableMaterial;
            }
            else
            {
                //renderer.material = NOTkableMaterial;
            }
        }

        public bool isTouchingPath(Path path)
        {
            // Check if this tile is a node in the specified path
            foreach (PathRouteNode node in path)
                if (node.Index.Equals(index) == true)
                    return true;

            // Not in the path
            return false;
        }
	}
}
