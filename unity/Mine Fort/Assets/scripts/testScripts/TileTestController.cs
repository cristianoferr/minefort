using Rimworld.model.entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Rimworld;
using Rimworld.model.entities.map;
using Rimworld.controllers;

public class TileTestController : MonoBehaviour {

    public Sprite floorSprite;
    public Sprite emptySprite;


    World world;
    // Use this for initialization
    void Start () {

        world = World.current;
        Biome biome = world.biome;
        for (int x = 0; x < world.width; x++)
        {
            for (int y = 0; y < world.height; y++)
            {
                GameObject tile_go = new GameObject();
                tile_go.name = "Tile_" + x + "_" + y;

                SpriteRenderer tile_sr = tile_go.AddComponent<SpriteRenderer>();
                Tile tile_data = world.GetTileAt(x, y);
                float tx = x;
                float ty = y;

                tile_data.RegisterTileTypeChangedCallback((tile)=> { OnTileTypeChanged(tile, tile_go,tx,ty); });
                tile_go.transform.parent = this.gameObject.transform;
                tile_data.Type = biome.RandomTile();// Rimworld.model.GameConsts.TileType.Floor;
                
            }
        }

        RandomizeTiles(world);
    }




    /*
     function isoTo2D(pt:Point):Point{
  var tempPt:Point = new Point(0, 0);
  tempPt.x = (2 * pt.y + pt.x) / 2;
  tempPt.y = (2 * pt.y - pt.x) / 2;
  return(tempPt);
}
         function twoDToIso(pt:Point):Point{
  var tempPt:Point = new Point(0,0);
  tempPt.x = pt.x - pt.y;
  tempPt.y = (pt.x + pt.y) / 2;
  return(tempPt);
}
         */



    private Vector3 CalcPosition(GameObject tile_go, Tile tile)
    {

        Vector3 pos = Utils.TwoDToIso(tile.X, tile.Y, tile.height);
        return pos;
    }


    private void RandomizeTiles(World world)
    {
        for (int x = 0; x < world.width; x++)
        {
            for (int y = 0; y < world.height; y++)
            {
                Tile tile = world.GetTileAt(x, y);
                RandomizeTile(tile);

            }
        }
    }

    private void RandomizeTile(Tile tile)
    {
        tile.Type = world.biome.RandomTile();
        tile.height = UnityEngine.Random.Range(tile.Type.MinHeight, tile.Type.MaxHeight);
    }

    float randomizeTileTimer = 2f;

    // Update is called once per frame
    void Update () {
      /*  randomizeTileTimer -= Time.deltaTime;
        if (randomizeTileTimer < 0)
        {
            RandomizeTiles(world);
            randomizeTileTimer = 2f;
        }*/
	}

    void OnTileTypeChanged(Tile tile_data,GameObject tile_go,float x,float y)
    {
        tile_go.GetComponent<SpriteRenderer>().sprite = SpriteManager.current.GetSprite("Tile", tile_data.Type.fileName);
      /*  if (tile_data.Type == Rimworld.model.GameConsts.TileType.Floor)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = floorSprite;
        }
        else
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = emptySprite;
        }*/

        tile_go.transform.position = CalcPosition(tile_go, tile_data);
        

    }
}
