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
        string type = "TriLinePoint";
        Texture2D textureRef;
        Rectangle drawRect;
        public GFX_TileElement(Tile tile) : base(tile)
        {
            textureRef = TextureGenerator.GetTexture(type);
            drawRect = new Rectangle(0, 0, 1, 1);
            //drawRect.Inflate((int)Math.Sqrt(tile.bounds.Width * tile.bounds.Height) / 5,
            //    (int)Math.Sqrt(tile.bounds.Width * tile.bounds.Height) / 5);
            drawRect.Width = (int)(drawRect.Width * Math.Sqrt(tile.connections.Length * 3 + 1) / 1.0);
            drawRect.Height = (int)(drawRect.Height * Math.Sqrt(tile.connections.Length * 3 + 1) / 1.0);
        }
        public void Draw(SpriteBatch spriteBatch, double dx, double dy, double mult)
        {
            //spriteBatch.Draw(textureRef, new Rectangle((int)(dx + (tile._center.X - drawRect.Width / 2) * mult),
            //    (int)(dy + (tile._center.Y - drawRect.Height / 2) * mult),
            //    (int)(drawRect.Width * mult),
            //    (int)(drawRect.Height * mult)), Color.White);
            spriteBatch.Draw(textureRef, new Rectangle((int)( (tile._center.X - drawRect.Width / 2f) * Map.ObjectBufferSize.X),
                (int)( (tile._center.Y - drawRect.Height / 2f) * Map.ObjectBufferSize.X),
                (int)(drawRect.Width * Map.ObjectBufferSize.X),
                (int)(drawRect.Height * Map.ObjectBufferSize.X)), Color.White);
        }
    }
}
