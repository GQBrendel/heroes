using AStar_2D.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Necromancer : Enemy
{
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

       // transform.LookAt(tileDestino.transform);  

        currentTile.toggleWalkable();                             
        currentTile = tileDestino;

        animatedAgent.PlaceAt(tileDestino.WorldPosition);

        setPos((int)tileDestino.getPos().x, (int)tileDestino.getPos().y);  
    }
    private IEnumerator test(Tile tileDestino)
    {
        yield return new WaitForSeconds(2f);



    }

}
