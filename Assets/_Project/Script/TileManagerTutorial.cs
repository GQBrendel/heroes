using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fungus;

namespace AStar_2D.Demo
{
	public class TileManagerTutorial : TileManager
    {
        Fungus.EventHandler EventHandler;

        protected override void onTileSelectedMouse(Tile tile, int mouseButton)
        {
            if (!ShouldExecuteActions)
            {
                return;
            }

            if (mouseButton != 0)
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
                if (AttackingHero == tile.tileActor)
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
                if (tile.tileActor != null)
                {
                    HealingHero.CommandToHeal(tile);
                }
                return;
            }

            if (!aHeroIsSelected)
            {
                pickHero((int)tile.getPos().x, (int)tile.getPos().y); 
                if (selectedHero == null)
                {
                    return;
                }

            }
            else if (aHeroIsSelected)
            {
                HeroController heroScript = selectedHero.GetComponent<HeroController>();
            }
        }
    }
}
