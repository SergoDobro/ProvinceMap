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
using System.Security.Cryptography.X509Certificates;

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

            map = new Map();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            map.Loadmap(GraphicsDevice, "map.bmp", "mapdec.csv", "temp_bounds.txt", "temp_bounds_gfx.txt");
            TextureGenerator.GraphicsDevice = GraphicsDevice;
        }

        MouseState mouseState, mouseState_prev;
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton != mouseState_prev.LeftButton)
            {
                try
                {
                    map.RandomiseColor(map.GetTileByClick(mouseState.Position));
                    System.Diagnostics.Debug.WriteLine(map.GetTileByClick(mouseState.Position).id);
                }
                catch
                {
                }
            }
            if (mouseState.Position.X > _graphics.PreferredBackBufferWidth - 20
                && map.dx + map.mapTexture.Width > _graphics.PreferredBackBufferWidth - 20)
            {
                map.dx -= 10;
            }
            if (mouseState.Position.X < 20
                && map.dx < 20)
            {
                map.dx += 10;
            }
            if (mouseState.Position.Y > _graphics.PreferredBackBufferHeight - 20
                && map.dy + map.mapTexture.Height > _graphics.PreferredBackBufferHeight - 20)
            {
                map.dy -= 10;
            }
            if (mouseState.Position.Y < 20
                && map.dy < 20)
            {
                map.dy += 10;
            }

            mouseState_prev = mouseState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            map.Draw(_spriteBatch);
            _spriteBatch.Begin();
            animationManager.Draw(_spriteBatch, gameTime);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
    class Map
    {
        public Texture2D mapTexture;
        Tile[] tiles;
        MapIDDecoder mapIDDecoder;
        Bitmap bitmap;
        public int dx = 0;
        public int dy = 0;
        public void Loadmap(GraphicsDevice graphicsDevice, string way_map, string way_map_tiles_dec, string way_bounds, string way_bounds_tiles)
        {
            mapIDDecoder = new MapIDDecoder();
            mapIDDecoder.Load(way_map_tiles_dec);
            tiles = new Tile[mapIDDecoder.GetId.Count];
            bitmap = new Bitmap(way_map);

            foreach (var item in mapIDDecoder.GetId)
            {
                tiles[item.Value] = new Tile();
                tiles[item.Value].id = item.Value;
            }

            mapTexture = Texture2D.FromFile(graphicsDevice, way_map);
            ScanProvsForColor(way_bounds, way_map, way_bounds_tiles);
            colors = new Color[mapTexture.Width * mapTexture.Height];
            mapTexture.GetData(colors);
        }
        public Tile GetTileByClick(Point positionInWorld)
        {
            return GetTileByPos(positionInWorld - new Point(dx, dy));
        }
        public Tile GetTileByPos(Point positionInWorld)
        {
            return tiles[mapIDDecoder[bitmap.GetPixel(positionInWorld.X, positionInWorld.Y)]];
        }
        Random random = new Random();
        Color[] colors;
        int ddx = 0;
        public void RandomiseColor(Tile tile)
        {
            Color ranColor = new Color((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
            var gotCol = mapIDDecoder.GetColor[tile.id];
            for (int x = tile.bounds.Left; x < tile.bounds.Right; x++)
            {
                for (int y = tile.bounds.Top; y < tile.bounds.Bottom; y++)
                {
                    if (bitmap.GetPixel(x, y) == gotCol)
                    {
                        colors[x + ddx + y * mapTexture.Width] = ranColor;
                    }
                }
            }
            ranColor = new Color((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
            foreach (var p in tile.boundsGFX)
            {
                colors[p.X + ddx + p.Y * mapTexture.Width] = ranColor;
            }
            mapTexture.SetData(colors);
        }
        public void ScanProvsForColor(string way_bounds, string way_map, string way_bounds_tiles)
        {
            if (File.GetLastAccessTime(way_map) > File.GetLastAccessTime(way_bounds))
            {

            }
            Rectangle[] bounds = new Rectangle[tiles.Length];
            if (!File.Exists(way_bounds))
            {
                var stream = File.Create(way_bounds);
                stream.Close();
            }
            var lines = File.ReadAllLines(way_bounds);
            bool recalc = false;
            if (lines.Length != mapIDDecoder.GetId.Count)
                recalc = true;
            if (recalc)
            {
                for (int i = 0; i < mapIDDecoder.GetId.Count; i++)
                {
                    var clr = mapIDDecoder.GetColor[i];
                    int leftest = bitmap.Width;
                    int rightest = 0;
                    int topest = bitmap.Height;
                    int lowest = 0;
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        for (int y = 0; y < bitmap.Height; y++)
                        {
                            if (bitmap.GetPixel(x, y) == clr)
                            {
                                if (x < leftest) leftest = x;
                                if (x > rightest) rightest = x;
                                if (y < topest) topest = y;
                                if (y > lowest) lowest = y;
                            }
                        }
                    }
                    bounds[i] = new Rectangle(leftest, topest, rightest - leftest + 1, lowest - topest + 1);
                    if (rightest - leftest + 1 < 0 || lowest - topest + 1 < 0)
                    {
                        bounds[i] = new Rectangle(0, 0, 10, 10);
                    }
                    System.Diagnostics.Debug.WriteLine(bounds[i]);
                }
                File.WriteAllLines(way_bounds, bounds.Select(a => $"{a.Left};{a.Top};{a.Right};{a.Bottom}"));
            }
            else
            {
                bounds = lines.Select(a =>
                {
                    var k = a.Split(';');
                    return new Rectangle(int.Parse(k[0]), int.Parse(k[1]), int.Parse(k[2]) - int.Parse(k[0]), int.Parse(k[3]) - int.Parse(k[1]));
                }).ToArray();
            }
            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i].bounds = bounds[i];
            }


            if (!File.Exists(way_bounds_tiles))
            {
                var stream = File.Create(way_bounds_tiles);
                stream.Close();
            }
            var lines2 = File.ReadAllLines(way_bounds_tiles);
            if (lines2.Length != mapIDDecoder.GetId.Count || lines2[0] == "")
                recalc = true;
            List<Point>[] tileBoundsGfx = new List<Point>[tiles.Length]; 
            if (recalc)
            {
                for (int i = 0; i < bounds.Length; i++)
                {
                    var bnd = bounds[i];
                    var clr = mapIDDecoder.GetColor[i];
                    tileBoundsGfx[i] = new List<Point>();
                    for (int x = bnd.Left + 1; x < bnd.Right - 1; x++)
                    {
                        for (int y = bnd.Top + 1; y < bnd.Bottom - 1; y++)
                        {
                            if (bitmap.GetPixel(x, y) == clr)
                            {
                                if (bitmap.GetPixel(x + 1, y) != clr
                                    || bitmap.GetPixel(x - 1, y) != clr
                                    || bitmap.GetPixel(x, y + 1) != clr
                                    || bitmap.GetPixel(x, y - 1) != clr
                                    )
                                {
                                    tileBoundsGfx[i].Add(new Point(x, y)); 
                                }
                            }
                        }
                    }
                }
                
                File.WriteAllLines(way_bounds_tiles, tileBoundsGfx.Select(pl=>string.Join(';', pl.Select(p=>p.X+"-"+p.Y))));
            }
            else
            { 
                tileBoundsGfx = lines2.Select(a =>
                {
                    var k = a.Split(';');
                    return k.Select(kp=>new Point(int.Parse(kp.Split('-')[0]), int.Parse(kp.Split('-')[1]))).ToList();
                }).ToArray();
            }
            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i].boundsGFX = tileBoundsGfx[i].ToArray();
            }

            //bitmap.Dispose();

        }

        #region Graphics
        public void Draw(SpriteBatch _spriteBatch)
        {
            int scale = 1;
            _spriteBatch.Begin();
            _spriteBatch.Draw(mapTexture, new Rectangle(dx, dy, mapTexture.Width / scale, mapTexture.Height / scale), Color.White);
            foreach (var item in tiles)
            {
                //_spriteBatch.Draw(TextureGenerator.GetFlat(), new Rectangle(item.bounds.X + dx, item.bounds.Y + dy, item.bounds.Width, item.bounds.Height), new Color(50, 50, 50, 0));
            }
            _spriteBatch.End();
        }
        #endregion


    }
    class MapIDDecoder
    {
        Dictionary<System.Drawing.Color, int> colorToID = new Dictionary<System.Drawing.Color, int>();
        Dictionary<int, System.Drawing.Color> iDToColor = new Dictionary<int, System.Drawing.Color>();

        public void Load(string path)
        {
            using (StreamReader reader = new StreamReader(path))
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
}
