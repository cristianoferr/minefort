using MineFort.model.entities.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MineFort.model.entities
{
    class CelularRule
    {
        internal string description;

        public string CellTag;
        public string ChangeTo;

        public string TagBelow;
        public string AnySideTag;

        public float ChangeHeight = 0;

        internal void CheckRule(Tile tile,Biome biome)
        {
            bool checkTag = CheckTag(tile);
            if (!checkTag) return;
            bool checkBelow=checkTagBelow(tile);
            if (!checkBelow) return;
            bool checkSide = CheckSides(tile);
            if (!checkSide) return;

            if (ChangeTo != null || ChangeTo != "")
            {
                TileType tt = biome.GetTileWithTag(ChangeTo);
                if (tt == null) { Utils.LogError("No Tile with tag " + ChangeTo + " found."); }
                tile.SetTileType(tt, false);
            }
            tile.height = ChangeHeight;
        }

        private bool CheckTag(Tile tile)
        {
            if (CellTag == null || CellTag == "") return true;
            return tile.Type.ContainsTags(CellTag);
        }

        private bool CheckSides(Tile tile)
        {
            if (AnySideTag == null|| AnySideTag=="") return true;
            Tile west = tile.West();
            if (west!=null && west.Type.ContainsTags(AnySideTag)) return true;
            Tile east= tile.East();
            if (east!=null && east.Type.ContainsTags(AnySideTag)) return true;
            Tile south= tile.South();
            if (south != null && south.Type.ContainsTags(AnySideTag)) return true;
            Tile north = tile.North();
            if (north != null && north.Type.ContainsTags(AnySideTag)) return true;
            Tile t = tile.SW();
            if (t != null && t.Type.ContainsTags(AnySideTag)) return true;
            t = tile.NW();
            if (t != null && t.Type.ContainsTags(AnySideTag)) return true;
            t = tile.SE();
            if (t != null && t.Type.ContainsTags(AnySideTag)) return true;
            t = tile.NE();
            if (t != null && t.Type.ContainsTags(AnySideTag)) return true;
            return false;
        }

        private bool checkTagBelow(Tile tile)
        {
            if (TagBelow == null|| TagBelow == "") return true;
            Tile down = tile.Down();
            if (down == null) return false;
            return (down.Type.ContainsTags(TagBelow));
        }
    }
    public class AutomatoCelular
    {
        private Map map;
        IList<CelularRule> startRules;
        IList<CelularRule> runningRules;

        public AutomatoCelular(Map map)
        {
            this.map = map;
            startRules = new List<CelularRule>();
            runningRules = new List<CelularRule>();

        }

        public void Start()
        {
            for (int i = 0; i < map.width; i++)
            {
                for (int j = 0; j < map.height; j++)
                {
                    for (int k = 0; k < map.depth; k++)
                    {
                        ApplyStartRules(i, j, k);
                    }
                }
            }
        }

        private void ApplyStartRules(int x, int y, int z)
        {
            Tile tile = map.GetTileAt(x, y, z);
            foreach (CelularRule rule in startRules)
            {
                rule.CheckRule(tile,map.biome);
            }
        }

        public void LoadTileTypeFromCSV(string[] lineData)
        {
            if (lineData.Length < 3) return;
            CelularRule rule = new CelularRule();

            int i = 0;
            rule.description = lineData[i++];
            string when=lineData[i++];
            if (when == "start")
            {
                startRules.Add(rule);
            } else
            {
                runningRules.Add(rule);
            }
            rule.CellTag = lineData[i++];
            rule.ChangeTo = lineData[i++];
            rule.TagBelow= lineData[i++];
            rule.AnySideTag= lineData[i++];
            rule.ChangeHeight = float.Parse(lineData[i++]);
            /*td.fileName = lineData[i++];
            td.BaseMovementCost = float.Parse(lineData[i++]);
            string[] tags = lineData[i++].Split(',');
            td.CanBuild = lineData[i++] == "1";
            td.BelowTileTag = lineData[i++];
            foreach (string tag in tags)
            {
                td.AddTag(tag);
            }*/
        }
    }
}
