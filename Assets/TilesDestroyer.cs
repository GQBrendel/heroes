using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesDestroyer : MonoBehaviour
{

    [SerializeField] private List<Vector2> _notwalkable;


    public void DestroyTiles(AStar_2D.Demo.Tile[,] tiles)
    {
        foreach (var tile in tiles)
        {
            if (_notwalkable.Contains(tile.getPos()))
            {
                tile.toggleWalkable();
                tile.DisableVisibility();
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
