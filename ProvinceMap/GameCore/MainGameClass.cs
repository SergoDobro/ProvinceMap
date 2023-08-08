using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Drawing;
using System.IO;
using System.Data.Common;
using Point = Microsoft.Xna.Framework.Point;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using ProvinceMap.GameCore.gfx;

namespace ProvinceMap.GameCore
{
    internal class MainGameClass : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Map map;
        AnimationManager animationManager = new AnimationManager();
        public MainGameClass()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            _graphics.PreferHalfPixelOffset = true;
            _graphics.PreferredBackBufferHeight = 675;
            _graphics.PreferredBackBufferWidth = 1200;
            _graphics.ApplyChanges();
            map = new Map();

        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            TextureGenerator.GraphicsDevice = GraphicsDevice;
            TextureGenerator.ContentManager = Content;
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            map.Loadgame(GraphicsDevice, _graphics);
            Line.LoadContentGlobal(GraphicsDevice);
            _graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            curT = map.GetTileById(4269);
            curT2 = map.GetTileById(0);
            tiles_to_checkNow.Add(curT);
        }

        MouseState mouseState, mouseState_prev;
        Tile curT = null;
        Tile curT2 = null;
        Random random = new Random();
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton != mouseState_prev.LeftButton)
            { 
                try
                {
                    var tile = map.GetTileByClick(mouseState.Position);
                    foreach (var t in tile.connections)
                    {
                        foreach (var tt in t.connections)
                        {
                            map.RandomiseColor(tt);
                        }
                    }
                    foreach (var t in tile.connections)
                    {
                        map.RandomiseColor(t, Color.Orange);
                    }
                    map.RandomiseColor(tile, Color.Red);
                    System.Diagnostics.Debug.WriteLine(map.GetTileByClick(mouseState.Position).id);
                }
                catch
                {
                }
            }
            if (random.NextDouble() < 1)
            {
                Iterate(gameTime);
                //map.RandomiseColor(curT, new Color(DateTime.Now.Millisecond / 10, DateTime.Now.Millisecond / 10, DateTime.Now.Millisecond / 10, (byte)255));
                //curT = curT.connections[random.Next(0, curT.connections.Length)];
                //map.RandomiseColor(curT2, new Color(DateTime.Now.Millisecond / 10, DateTime.Now.Millisecond / 10, DateTime.Now.Millisecond / 10, (byte)255));
                //curT2 = curT.connections[random.Next(0, curT.connections.Length)];

            }
            Controls();
            mouseState_prev = mouseState;
            base.Update(gameTime);
        }

        public void Controls()
        {

            if (mouseState.Position.X > _graphics.PreferredBackBufferWidth - 20
                )//&& map.Dx + map.mapTexture.Width * map.mult > _graphics.PreferredBackBufferWidth - 20)
            {
                map.Dx -= 10;
            }
            if (mouseState.Position.X < 20
                )//&& map.Dx < 20)
            {
                map.Dx += 10;
            }
            if (mouseState.Position.Y > _graphics.PreferredBackBufferHeight - 20
                && map.dy + map.mapTexture.Height * map.mult > _graphics.PreferredBackBufferHeight + 10)
            {
                map.dy -= 10;
            }
            if (mouseState.Position.Y < 20
                && map.dy < 0)
            {
                map.dy += 10;
            }
            if (mouseState.ScrollWheelValue > mouseState_prev.ScrollWheelValue+0.1)
            {
                map.ChangeMult(map.mult * 1.2);
            }
            if (mouseState.ScrollWheelValue < mouseState_prev.ScrollWheelValue- 0.1)
            {
                map.ChangeMult(map.mult / 1.2);
            }
        }

        List<Tile> tiles_checked = new List<Tile>();
        List<Tile> tiles_to_checkNext = new List<Tile>();
        List<Tile> tiles_to_checkNow = new List<Tile>();
        int k = 0;
        public void Iterate(GameTime gameTime)
        {
            k++;
            tiles_to_checkNext = new List<Tile>();
            foreach (var t in tiles_to_checkNow)
            {
                //map.RandomiseColor(t, new Color(((float)(Math.Sin(gameTime.TotalGameTime.Milliseconds / 200f) / 2 + 0.5f)),
                //    ((float)(Math.Sin(gameTime.TotalGameTime.Milliseconds / 200f) / 2 + 0.5f)),
                //    ((float)(Math.Sin(gameTime.TotalGameTime.Milliseconds / 200f) / 2 + 0.5f)),
                //    (float)1));
                float kl = (1 - k / 50f)*(float)random.NextDouble();// 1 - k / 50f;
                map.RandomiseColor(t, new Color(kl, kl, kl,
                    (float)1));
                tiles_checked.Add(t);
                foreach (var n in t.connections)
                {
                    if (!tiles_to_checkNext.Contains(n))
                    {
                        if (!tiles_checked.Contains(n))
                        {
                            tiles_to_checkNext.Add(n);
                        }
                    }
                }
            }
            if (tiles_to_checkNow.Count == 0)
            {

            }
            tiles_to_checkNow = tiles_to_checkNext;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(null);
            //GraphicsDevice.Clear(Color.DarkBlue);

            map.Draw(_spriteBatch);
            _spriteBatch.Begin();
            animationManager.Draw(_spriteBatch, gameTime);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
    class MapIDDecoder
    {
        Dictionary<System.Drawing.Color, int> colorToID = new Dictionary<System.Drawing.Color, int>();
        Dictionary<int, System.Drawing.Color> iDToColor = new Dictionary<int, System.Drawing.Color>();

        public void Load()
        {
            string way_map_tiles_dec = PathWays.Path["way_map_tiles_dec"];
            using (StreamReader reader = new StreamReader(way_map_tiles_dec))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    colorToID.Add(System.Drawing.Color.FromArgb(int.Parse(line.Split(';')[0]), int.Parse(line.Split(';')[1]), int.Parse(line.Split(';')[2])),
                        int.Parse(line.Split(';')[3]));
                    iDToColor.Add(int.Parse(line.Split(';')[3]),
                        System.Drawing.Color.FromArgb(int.Parse(line.Split(';')[0]),
                        int.Parse(line.Split(';')[1]), int.Parse(line.Split(';')[2])));
                }
            }
        }
        public Dictionary<System.Drawing.Color, int> GetId { get { return colorToID; } }
        public Dictionary<int, System.Drawing.Color> GetColor { get { return iDToColor; } }
        public int this[System.Drawing.Color index]
        {
            get { return colorToID[index]; }
        }
    }
    static class PathWays
    {
        public static Dictionary<string, string> Path { get; set; } = new Dictionary<string, string>();
    }
}
