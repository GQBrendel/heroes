using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Import the AStar_2D namespace
using AStar_2D;
using AStar_2D.Pathfinding;

// Namespace
namespace AStar_2D.Demo
{
    /// <summary>
    /// Inherits the AStar class that allows the user to specify what INode to use for pathfinding, in this case Tile.
    /// By default, AIManager is a singleton which can be accessed anywhere in code using AIManager.Instance.
    /// This allows access to the pathfinding methods within.
    /// </summary>
	public class TileManager1 : AStarGrid
    {
        // Private
        private Tile[,] tiles;

        // Public
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

            tiles = new Tile[gridX, gridY];

            for (int i = 0; i < gridX; i++)
            {
                for (int j = 0; j < gridY; j++)
                {
                    // Create the tile at its location
                    GameObject obj = MonoBehaviour.Instantiate(tilePrefab, new Vector3((i - (gridX / 2)) * 0.6f,0, (j - (gridY / 2)) * 0.6f), Quaternion.identity) as GameObject;
                    obj.transform.rotation =  Quaternion.Euler(90, 0, 0);
                    // Add the tile script
                    tiles[i, j] = obj.GetComponent<Tile>();
                    tiles[i, j].index = new Index(i, j);

                    // Add an event listener
                    tiles[i, j].onTileSelected += onTileSelected;

                    // Check for preview
                    if (showPreviewPath == true)
                        tiles[i, j].onTileHover += onTileHover;

                    // Add the tile as a child to keep the scene view clean
                    obj.transform.SetParent(transform);
                }
            }

            tiles[3, 3].diagonalMode = PathNodeDiagonalMode.NoDiagonal;

            // Pass the arry to the search grid
            constructGrid(tiles);
        }

        /// <summary>
        /// Called by Unity.
        /// Left blank for demonstration.
        /// </summary>
        public void Update()
        {
            // Do stuff
        }

        private void onTileSelected(Tile tile, int mouseButton)
        {
            // Check for button
            if(mouseButton == 0)
            {
                // Set the destination
                Agent[] agents = Component.FindObjectsOfType<Agent>();

                // Set the target for all agents
                foreach (Agent agent in agents)
                    agent.setDestination(tile.WorldPosition);
            }
            else if(mouseButton == 1)
            {
                // Toggle the walkable status
                tile.toggleWalkable();
            }
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
    }
}
