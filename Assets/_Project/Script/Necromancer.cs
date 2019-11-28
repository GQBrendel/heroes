using AStar_2D.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Necromancer : Enemy
{
    [SerializeField] private GameObject _skeletonPrefab;

    public override void TryMoveEnemy(Tile tileDestino)
    {
        animatedAgent = GetComponent<AnimatedAgent>();

        if (tileDestino == currentTile)
        {

        }

        else if (animatedAgent.IsMoving || !tileDestino.IsWalkable)
        {
            return;
        }

        currentTile.toggleWalkable();                             
        currentTile = tileDestino;

        animatedAgent.PlaceAt(tileDestino.WorldPosition);

        setPos((int)tileDestino.getPos().x, (int)tileDestino.getPos().y);  
    }

    public void SummonSkeletons()
    {
        Vector2 spawnPos = GetSpawnForSkeleton();     
        TileManager.Instance.GenerateActor(_skeletonPrefab, spawnPos);

        spawnPos = GetSpawnForSkeleton();
        TileManager.Instance.GenerateActor(_skeletonPrefab, spawnPos);
    }
    private Vector2 GetSpawnForSkeleton()
    {
        Vector2 spawnPos = Vector2.zero;
        int limitX = TileManager.Instance.gridX;
        int limitY = TileManager.Instance.gridY;
        int randomX = 0;
        int randomY = 0;

        do
        {
            randomX = Random.Range(posX - 4, posX + 4);
            randomY = Random.Range(posY - 4, posY + 4);

            if (randomX > limitX)
            {
                randomX = limitX;
            }
            else if (randomX < 0)
            {
                randomX = 0;
            }
            if (randomY > limitY)
            {
                randomY = limitY;
            }
            else if (randomY < 0)
            {
                randomY = 0;
            }
        } while (!TileManager.Instance.tiles[randomX, randomY].IsWalkable);

        spawnPos = new Vector2(randomX, randomY);
        return spawnPos;
    }
}
