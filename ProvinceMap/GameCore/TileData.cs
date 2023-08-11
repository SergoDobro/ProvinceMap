using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvinceMap.GameCore
{
    internal class TileData
    {
        Tile tile;
        public string tag;
        public TileData(Tile tile)
        {
            this.tile = tile;
        }
        public void ChangeTag(string tag)
        {
            if (tile == null) Map.MapInstance.RandomiseColor(tile);
            if (tag == "RED")
                Map.MapInstance.RandomiseColor(tile, Color.IndianRed);
            if (tag == "BLU")
                Map.MapInstance.RandomiseColor(tile, Color.DeepSkyBlue);
            this.tag = tag;
        }

    }
}
