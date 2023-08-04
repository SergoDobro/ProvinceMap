using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProvinceMap.GameCore.gfx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace ProvinceMap.GameCore
{
    class TileElement
    {
        public Tile ElementsTile { get { return tile; } }
        protected Tile tile;
        public TileElement(Tile tile)
        {
            this.tile = tile;
        } 
    }
    class GFX_TileElement : TileElement
    {
        string type = "castle";
        Texture2D textureRef;
        Rectangle drawRect;
        public GFX_TileElement(Tile tile) : base(tile)
        {
            textureRef = TextureGenerator.GetTexture(type);
            drawRect = new Rectangle(0, 0, 1, 1);
            //drawRect.Inflate((int)Math.Sqrt(tile.bounds.Width * tile.bounds.Height) / 5,
            //    (int)Math.Sqrt(tile.bounds.Width * tile.bounds.Height) / 5);
            drawRect.Inflate((int)Math.Sqrt(tile.connections.Length * 2 + 1) / 1,
                (int)Math.Sqrt(tile.connections.Length * 2 + 1) / 1);
        }
        public void Draw(SpriteBatch spriteBatch, int dx, int dy, int mult)
        {
            spriteBatch.Draw(textureRef, new Rectangle(dx + (tile._center.X - drawRect.Width / 2) * mult,
                dy + (tile._center.Y - drawRect.Height / 2) * mult,
                drawRect.Width * mult,
                drawRect.Height * mult), Color.White);
        }
    }
}
