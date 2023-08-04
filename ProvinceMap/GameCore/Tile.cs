using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ProvinceMap.GameCore
{
    internal class Tile
    {
        public int id;
        public Point _center;
        public Rectangle bounds
        {
            get { return _bounds; }
            set
            {
                _bounds = value;
                _center = _bounds.Center;
            }
        }
        private Rectangle _bounds;
        public Point[] boundsGFX;

        public Tile[] connections;

        public TileElement[] gFXElementOnMaps = new TileElement[1];
        //For paths
        int weight = -1;
        int lastID = -1;
    }
}