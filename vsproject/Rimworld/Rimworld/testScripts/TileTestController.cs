using Rimworld.model.entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TileTestController : MonoBehaviour {

    public Sprite floorSprite;
    public Sprite emptySprite;

    public GameObject parent;

    World world;
    // Use this for initialization
    void Start () {

        world = World.current;
        RandomizeTiles(world);
        for (int x = 0; x < world.width; x++)
        {
            for (int y = 0; y < world.height; y++)
            {
                GameObject tile_go = new GameObject();
                tile_go.name = "Tile_" + x + "_" + y;

                SpriteRenderer tile_sr = tile_go.AddComponent<SpriteRenderer>();
                Tile tile_data = world.GetTileAt(x, y);
                tile_go.transform.position = new Vector3(x / 2f, y / 2f, y/10f);
                if (tile_data.Type == Rimworld.model.GameConsts.TileType.Floor)
                {
                    tile_sr.sprite = floorSprite;
                } else
                {
                    tile_sr.sprite = emptySprite;
                }
                tile_go.transform.parent = parent.transform;
            }
        }
	}

    private void RandomizeTiles(World world)
    {
        for (int x = 0; x < world.width; x++)
        {
            for (int y = 0; y < world.height; y++)
            {
                if (UnityEngine.Random.Range(0, 2) == 0)
                {
                    world.GetTileAt(x, y).Type = Rimworld.model.GameConsts.TileType.Floor;
                } else
                {
                    world.GetTileAt(x, y).Type = Rimworld.model.GameConsts.TileType.Empty;
                }
            }
        }
    }

   

    // Update is called once per frame
    void Update () {
		
	}
}
