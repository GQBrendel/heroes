using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using AStar_2D;
using AStar_2D.Demo;

[ExecuteInEditMode]
public class EditorTilemap : AStarGrid
{
    // Private    
    [SerializeField]
    [HideInInspector]
    private Tile[] editorTiles = null; // Editor tiles must be stored in a single dimension array so Unity can serialzie them
    private Tile[,] tiles = null; // At runtime we relink the tile reference into this game array
    private List<GameObject> destroyList = new List<GameObject>();

    // Public
    public GameObject tilePrefab;
    public int gridWidth = 32;
    public int gridHeight = 24;
    public float gridSpacingX = 0.6f;
    public float gridSpacingY = 0.6f;
    public bool showPreviewPath = false;

    // Methods
    public override void Awake()
    {
        // Make sure the game is playing
        if (Application.isPlaying == true)
        {
            base.Awake();

            // Load the references to each tile from the edtitor array
            restoreTileReferences();

            // Check for valid tiles
            if (tiles == null)
            {
                Debug.LogError("The tiles have not been created. Make sure you can see the tiles in the editor before you start the game");
                return;
            }            

            // Listen for mouse events in game so we can set destinations and toggle walkable tiles.
            registerForClickEvents();

            // Create the grid
            constructGrid(tiles);
        }
    }

    public void Update()
    {
        // This method will be called in the editor since we have the 'ExecuteInEditMode' attribute
        // We use this method to destory any left over editor objects because we cannot destory them in 'OnValidate'
        foreach (GameObject go in destroyList)
            if (go != null)
                DestroyImmediate(go);

        destroyList.Clear();
    }

    public void OnValidate()
    {
        // Create our tiles in editor
        recreateTileGrid();
    }

    // Only call this method from an editor function
    private void recreateTileGrid()
    {
        // Dont recreate tiles in play mode
        if (Application.isPlaying == true)
            return;

        // Destroy old tiles
        if (editorTiles != null)
        {
            for (int i = 0; i < editorTiles.Length; i++)
            {
                // Check for existing tiles
                if (editorTiles[i] != null)
                {
                    // Try to convert to script
                    Tile result = editorTiles[i];

                    // Make sure we have a script
                    if (result != null)
                        destroyList.Add(result.gameObject);
                }
            }
        }

        // Make sure we have a tile prefab
        if (tilePrefab == null)
        {
            Debug.LogWarning("Please assign a valid tile prefab before adjusting the settings");
            return;
        }

        // Reallocate the array
        editorTiles = new Tile[gridWidth * gridHeight];

        // Space the tiles
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // Calcualte the array index
                int index = (gridWidth * y) + x;

                // Create an instance of the tile prefab
                GameObject go = Instantiate(tilePrefab,new Vector3(
                    (x - (gridWidth / 2)) * gridSpacingX,
                    (y - (gridHeight / 2)) * gridSpacingY), 
                    Quaternion.identity) as GameObject;

                // Get the script
                Tile node = go.GetComponent<Tile>();

                // Check for error
                if (node == null)
                    throw new System.Exception("The tile prefab provided does not contan a script that implements the IPathNode interface");

                // Set the parent
                go.transform.SetParent(transform);

                // Add to the array
                editorTiles[index] = node;
            }
        }
    }

    private void restoreTileReferences()
    {
        // Create the array
        tiles = new Tile[gridWidth, gridHeight];

        for (int x = 0; x < tiles.GetLength(0); x++)
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                // Calcualte the index
                int index = (gridWidth * y) + x;

                // Assign the tile reference
                tiles[x, y] = editorTiles[index];
                tiles[x, y].index = new Index(x, y);
            }
        }
    }

    private void registerForClickEvents()
    {
        for(int x = 0; x < tiles.GetLength(0); x++)
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                // Try to get the node as the tile class
                Tile tile = tiles[x, y] as Tile;

                // Make sure the cast was successful
                if (tile != null)
                {
                    tile.onTileSelected += onTileSelected;

                    // Check for preview
                    if (showPreviewPath == true)
                        tiles[x, y].onTileHover += onTileHover;
                }
            }
        }
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
                if(status == PathRequestStatus.PathFound)
                    if (tile.isTouchingPath(result) == true)
                        tile.touchingPathFlag = true;
            });
        }
    }
}
