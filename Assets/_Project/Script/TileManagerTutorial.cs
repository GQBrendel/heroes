using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fungus;

namespace AStar_2D.Demo
{
    public class TileManagerTutorial : TileManager
    {
        [SerializeField] private IntroManager _introManager;
        [SerializeField] private Vector2 _tutorialMovePosition;
        [SerializeField] private GameObject _moveHerePrefab;

        private GameObject _moveHereMark;

        Fungus.EventHandler EventHandler;

        private bool _tutorialIsActive = true;

        protected override void Start()
        {
            base.Start();
            ShouldExecuteActions = false;


            _tutorialIsActive = PlayerPrefs.GetInt("HasSavedGame", 1) != 0;

            if (!_tutorialIsActive)
            {
                EndTutorial();
            }
        }


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

            if (MovingHero && _tutorialIsActive)
            {
                if (tile.IsWalkable && tile.getPos() == _tutorialMovePosition)
                {
                    MovingHero.CommandToMove(tile);
                    _moveHereMark.SetActive(false);
                }
                return;
            }
            else if (MovingHero)
            {
                if (tile.IsWalkable)
                {
                    MovingHero.CommandToMove(tile);
                    if (_tutorialIsActive)
                    {
                        _moveHereMark.SetActive(false);
                    }
                }
                return;
            }
            else if (AttackingHero)
            {
                if (AttackingHero == tile.tileActor)
                {
                    if (!_tutorialIsActive)
                    {
                        AttackingHero.HideWays();
                        AttackingHero = null;
                        pickHero((int)tile.getPos().x, (int)tile.getPos().y);
                    }
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
        protected override void pickHero(int x, int y)
        {
            foreach (GameObject hero in heroesList)
            {
                hero.GetComponent<CharacterInfo>().DisableHighlight();
                if (hero.GetComponent<Actor>().checkPos(x, y) && !hero.GetComponent<Actor>().acted)
                {
                    selectedHero = hero;
                    selectedHero.GetComponent<HeroController>().SelectHeroTutorial();
                    hero.GetComponent<CharacterInfo>().EnableHighLight();
                    aHeroIsSelected = true;
                    if (_tutorialIsActive)
                    {
                        _introManager.LydiaSelected();
                    }
                    break;
                }
            }
        }
        public override void MoveSelected()
        {
            if (!_tutorialIsActive)
            {
                return;
            }

            _introManager.MoveSelected();
            _moveHereMark = Instantiate(_moveHerePrefab, tiles[(int)_tutorialMovePosition.x, (int)_tutorialMovePosition.y].gameObject.transform.position, Quaternion.identity);
        }


        protected override void endTurn()
        {
            if (!_tutorialIsActive)
            {
                EnemiesTurnReadyStart();
            }
            return;
        }
        public void EnemiesTurnReadyStart()
        {
            tacticalAgent.enemiesTurn();
            StartCoroutine(WaitForIAActions());
        }

        public void EndTutorial()
        {
            foreach(var hero in heroesList)
            {
                hero.GetComponent<HeroController>().IsTutorial = false;
            }
            _tutorialIsActive = false;
            ShouldExecuteActions = true;
        }
    }
}
