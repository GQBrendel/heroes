using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialMenuSpawner : MonoBehaviour
{

    public static RadialMenuSpawner instance;
    public RadialMenu menuPrefab;

    private void Awake()
    {
       instance = this;
    }

    public void SpawnMenu(Interactable obj, HeroController heroController, AStar_2D.Demo.Tile tile)
    {/*
        transform.SetParent(obj.transform);
        RadialMenu newMenu = Instantiate(menuPrefab) as RadialMenu;
        newMenu.transform.SetParent(transform, false);
        newMenu.transform.position = Input.mousePosition;
      //  newMenu._labelText.text = obj.title.ToUpper();
        newMenu.SpawnButton(obj, heroController, tile);*/
    }
}
