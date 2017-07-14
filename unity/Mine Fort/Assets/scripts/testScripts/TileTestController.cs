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
                tile_go.transform.position = CalcPosition(tile_go,x, y,45);
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



    private Vector3 CalcPosition(GameObject tile_go,float x, float y,float angle)
    {
        /*   var eulers = tile_go.transform.rotation.eulerAngles;
           //tile_go.transform.rotation = Quaternion.Euler(angle, eulers.y, eulers.z);

           Vector3 pos=new Vector3(x/2f , y/2f , 0);
           pos=Quaternion.AngleAxis(30, Vector3.up) * pos;
           pos = Quaternion.AngleAxis(45, Vector3.left) * pos;
           return pos;*/
        x = x / 2f;
        y = y / 2f;
        Vector3 pos = new Vector3(x-y,(x+y)/2, -y/2f);
        return pos;
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
