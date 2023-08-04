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

            mouseState_prev = mouseState;
            base.Update(gameTime);
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
                float kl = 1 - k / 50f;
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
            GraphicsDevice.Clear(Color.DarkBlue);

            map.Draw(_spriteBatch);
            _spriteBatch.Begin();
            animationManager.Draw(_spriteBatch, gameTime);
            Line.DrawAll(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
    class Map
    {
        private Tile[] tiles;
        private MapIDDecoder mapIDDecoder;
        private Random random = new Random();
        private Color[] colors;
        private int ddx = -0;
        private int ddy = -0;
        private bool clrsChanged = false;

        public Tile GetTileByClick(Point positionInWorld)
        {
            if ((positionInWorld.X - dx) / mult > bitmap.Width)
            {
                positionInWorld.X -= bitmap.Width*mult;
            }
            return GetTileByPos((positionInWorld - new Point(dx, dy)));
        }
        public Tile GetTileByPos(Point positionInWorld)
        {
            return tiles[mapIDDecoder[bitmap.GetPixel(positionInWorld.X / mult, positionInWorld.Y / mult)]];
        }
        public Tile GetTileById(int id)
        {
            return tiles[id];
        }
        public void RandomiseColor(Tile tile, Color ranColor = new Color())
        {
            var gotCol = mapIDDecoder.GetColor[tile.id];
            if (ranColor == Color.Transparent)
                ranColor = Color.Black;//new Color(255 - gotCol.R, 255 - gotCol.G, 255 - gotCol.B);//(float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
            for (int x = tile.bounds.Left; x < tile.bounds.Right; x++)
            {
                for (int y = tile.bounds.Top; y < tile.bounds.Bottom; y++)
                {
                    if (bitmap.GetPixel(x, y) == gotCol)
                    {
                        colors[x + ddx + (y + ddy) * mapTexture.Width] = ranColor;
                    }
                }
            }
            //ranColor = new Color(125 - gotCol.R, 125 - gotCol.G, 125 - gotCol.B);//(float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
            //foreach (var p in tile.boundsGFX)
            //{
            //    //colors[p.X + ddx + (p.Y + ddy) * mapTexture.Width] = ranColor;
            //    //colors[p.X + ddx - 1  + (p.Y + ddy - 1) * mapTexture.Width] = ranColor; 
            //}
            clrsChanged = true;
        }



        #region Initialisation
        public void Loadgame(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphicsDeviceManager)
        {
            LoadPaths();

            mapIDDecoder = new MapIDDecoder();
            mapIDDecoder.Load();

            tiles = new Tile[mapIDDecoder.GetId.Count];
            foreach (var item in mapIDDecoder.GetId)
            {
                tiles[item.Value] = new Tile();
                tiles[item.Value].id = item.Value;
            }

            rectangle_Graphics = new Rectangle(0, 0, graphicsDeviceManager.PreferredBackBufferWidth, graphicsDeviceManager.PreferredBackBufferHeight);
            bitmap = new Bitmap(PathWays.Path["way_map"]);
            mapTexture = Texture2D.FromFile(graphicsDevice, PathWays.Path["way_map"]);
            colors = new Color[mapTexture.Width * mapTexture.Height];
            mapTexture.GetData(colors);
            effect = TextureGenerator.ContentManager.Load<Effect>("File");

            LoadScanning();
            TileBuilding();
        }
        public void LoadPaths()
        {
            var lines = File.ReadAllLines("paths.txt").Select(x => x.Trim());
            foreach (var line in lines)
            {
                PathWays.Path.Add(line.Split('=')[0].Trim(), line.Split('=')[1].Trim());
            }
        }
        public void LoadScanning()
        {
            //PathWays.Path.Add("way_connections", "temp_connections.txt");
            //PathWays.Path.Add("way_bounds", "temp_bounds.txt");
            //PathWays.Path.Add("way_bounds_tiles", "temp_bounds_gfx.txt");
            //way_connections = temp_connections.txt
            //way_bounds = temp_bounds.txt
            //way_bounds_tiles = temp_bounds_gfx.txt
            ScanProvsForColor();
            ScanForConnections();
        }
        public void ScanProvsForColor()
        {
            Rectangle[] bounds = new Rectangle[tiles.Length];
            string way_bounds = PathWays.Path["way_bounds"];
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
                    int w = bitmap.Width;
                    var clr = mapIDDecoder.GetColor[i];
                    Color ourColor = new Color(clr.R, clr.G, clr.B);
                    int leftest = w;
                    int rightest = 0;
                    int topest = bitmap.Height;
                    int lowest = 0;
                    for (int x = 0; x < w; x++)
                    {
                        for (int y = 0; y < bitmap.Height; y++)
                        {
                            if (colors[x + y * w] == ourColor)
                            {
                                if (x > rightest)
                                {
                                    rightest = x;
                                }
                                else if (x < leftest)
                                {
                                    leftest = x;
                                }

                                if (y < topest) topest = y;
                                else if (y > lowest) lowest = y;
                            }
                            if (x - rightest > 100 && y - lowest > 100 && rightest != 0 && lowest != 0)
                            {
                                goto exitscan1;
                            }
                        }
                    }
                exitscan1:
                    bounds[i] = new Rectangle(leftest, topest, rightest - leftest + 1, lowest - topest + 1);
                    if (rightest - leftest + 1 < 0 || lowest - topest + 1 < 0)
                    {
                        bounds[i] = new Rectangle(0, 0, 10, 10);
                    }
                    if (i % 50 == 0)
                    {
                        System.Diagnostics.Debug.WriteLine(Math.Round(100 * (float)i / mapIDDecoder.GetId.Count, 2) + "%");
                    }
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

            string way_bounds_tiles = PathWays.Path["way_bounds_tiles"];
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

                File.WriteAllLines(way_bounds_tiles, tileBoundsGfx.Select(pl => string.Join(';', pl.Select(p => p.X + "-" + p.Y))));
            }
            else
            {
                tileBoundsGfx = lines2.Select(a =>
                {
                    if (a == "")
                    {
                        return new List<Point>();
                    }
                    var k = a.Split(';');
                    return k.Select(kp => new Point(int.Parse(kp.Split('-')[0]), int.Parse(kp.Split('-')[1]))).ToList();
                }).ToArray();
            }
            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i].boundsGFX = tileBoundsGfx[i].ToArray();
            }

            //bitmap.Dispose();

        }
        public void ScanForConnections()
        {
            string way_connections = PathWays.Path["way_connections"];

            if (!File.Exists(way_connections))
            {
                var stream = File.Create(way_connections);
                stream.Close();
            }
            var lines = File.ReadAllLines(way_connections);
            bool recalc = false;
            if (lines.Length != mapIDDecoder.GetId.Count || lines[0] == "restart")
                recalc = true;
            Color conColor = new Color(50, 50, 0, 0);
            if (recalc)
            {
                for (int i = 0; i < tiles.Length; i++)
                {
                    var bnd = tiles[i].bounds;
                    var clr = mapIDDecoder.GetColor[i];
                    List<Tile> connections = new List<Tile>();
                    List<System.Drawing.Color> checkedColors = new List<System.Drawing.Color>() { clr };
                    for (int x = bnd.Left; x < bnd.Right; x++)
                    {
                        if (x < 1)
                        {
                            x = 1;
                        }
                        if (x >= bitmap.Width - 1)
                        {
                            break;
                        }
                        for (int y = bnd.Top; y < bnd.Bottom; y++)
                        {
                            if (y < 1)
                            {
                                y = 1;
                            }
                            if (y >= bitmap.Height - 1)
                            {
                                break;
                            }
                            if (bitmap.GetPixel(x, y) == clr)
                            {
                                var clr2 = bitmap.GetPixel(x + 1, y);
                                if (mapIDDecoder.GetId.ContainsKey(clr2) && clr2 != clr && !checkedColors.Contains(clr2))
                                {
                                    checkedColors.Add(clr2);
                                    connections.Add(tiles[mapIDDecoder.GetId[clr2]]);
                                }
                                var clr3 = bitmap.GetPixel(x - 1, y);
                                if (mapIDDecoder.GetId.ContainsKey(clr3) && clr3 != clr && !checkedColors.Contains(clr3))
                                {
                                    checkedColors.Add(clr3);
                                    connections.Add(tiles[mapIDDecoder.GetId[clr3]]);
                                }
                                var clr4 = bitmap.GetPixel(x, y + 1);
                                if (mapIDDecoder.GetId.ContainsKey(clr4) && clr4 != clr && !checkedColors.Contains(clr4))
                                {
                                    checkedColors.Add(clr4);
                                    connections.Add(tiles[mapIDDecoder.GetId[clr4]]);
                                }
                                var clr5 = bitmap.GetPixel(x, y - 1);
                                if (mapIDDecoder.GetId.ContainsKey(clr5) && clr5 != clr && !checkedColors.Contains(clr5))
                                {
                                    checkedColors.Add(clr5);
                                    connections.Add(tiles[mapIDDecoder.GetId[clr5]]);
                                }
                            }
                        }
                    }
                    if (bnd.Left == 0)
                    {
                        for (int y = bnd.Top; y < bnd.Bottom; y++)
                        {
                            if (y < 1)
                            {
                                y = 1;
                            }
                            if (y >= bitmap.Height - 1)
                            {
                                break;
                            }
                            if (bitmap.GetPixel(0, y) == clr)
                            {
                                var clr2 = bitmap.GetPixel(bitmap.Width - 1, y);
                                if (mapIDDecoder.GetId.ContainsKey(clr2) && clr2 != clr && !checkedColors.Contains(clr2))
                                {
                                    checkedColors.Add(clr2);
                                    connections.Add(tiles[mapIDDecoder.GetId[clr2]]);
                                } 
                            }
                        }
                    }
                    if (bnd.Right == bitmap.Width - 1)
                    {
                        for (int y = bnd.Top; y < bnd.Bottom; y++)
                        {
                            if (y < 1)
                            {
                                y = 1;
                            }
                            if (y >= bitmap.Height - 1)
                            {
                                break;
                            }
                            if (bitmap.GetPixel(bitmap.Width - 1, y) == clr)
                            {
                                var clr2 = bitmap.GetPixel(0, y);
                                if (mapIDDecoder.GetId.ContainsKey(clr2) && clr2 != clr && !checkedColors.Contains(clr2))
                                {
                                    checkedColors.Add(clr2);
                                    connections.Add(tiles[mapIDDecoder.GetId[clr2]]);
                                } 
                            }
                        }
                    }
                    tiles[i].connections = connections.ToArray();
                    foreach (var t in tiles[i].connections)
                    {
                        new Line(tiles[i].bounds.Center.ToVector2(), t.bounds.Center.ToVector2(), conColor, 1, this);
                    }
                }
                File.WriteAllLines(way_connections, tiles.Select(t => string.Join(';', t.connections.Select(c => c.id))));
            }
            else
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i] != "")
                    {
                        tiles[i].connections = lines[i].Split(';').Select(x => tiles[int.Parse(x)]).ToArray();
                    }
                    else
                    {
                        tiles[i].connections = new Tile[0];
                    }
                    foreach (var t in tiles[i].connections)
                    {
                        if (t.bounds.Center.X- tiles[i].bounds.Center.X>200)
                        {
                            new Line(tiles[i].bounds.Center.ToVector2(), t.bounds.Center.ToVector2()-new Vector2(bitmap.Width, 0), conColor, 1, this);
                        }
                        else
                        { 
                            new Line(tiles[i].bounds.Center.ToVector2(), t.bounds.Center.ToVector2(), conColor, 1, this);
                        }
                    }
                }
            }
        }
        public void TileBuilding()
        {
            foreach (var t in tiles)
            {
                RandomiseColor(t, Color.DarkGreen);
                t.gFXElementOnMaps[0] = new GFX_TileElement(t);
            }
        }
        #endregion

        #region Graphics

        Effect effect;

        public Texture2D mapTexture;
        private Rectangle rectangle_Graphics;
        private Bitmap bitmap;
        public int Dx
        {
            get { return dx; }
            set
            {
                dx = value; 
                if (dx > 0)
                {
                    dx = -mapTexture.Width * mult;
                }
                if (dx < -mapTexture.Width * mult)
                {
                    dx = 0;
                }
            } }
        private int dx = 0;
        public int dy = 0;
        public int mult = 5;


        public void ChangeMult(int newMult)
        {
            dx = newMult * mapTexture.Width * dx / (mapTexture.Width * mult);
            dy = newMult * mapTexture.Height * dy / (mapTexture.Height * mult);
            mult = newMult;
        }
        public void Draw(SpriteBatch _spriteBatch)
        {

            if (Keyboard.GetState().IsKeyDown(Keys.D1))
                ChangeMult(1);
            else if (Keyboard.GetState().IsKeyDown(Keys.D2))
                ChangeMult(2);
            else if (Keyboard.GetState().IsKeyDown(Keys.D3))
                ChangeMult(3);
            else if (Keyboard.GetState().IsKeyDown(Keys.D4))
                ChangeMult(4);
            else if (Keyboard.GetState().IsKeyDown(Keys.D5))
                ChangeMult(5);
            if (clrsChanged)
            {
                mapTexture.SetData(colors);
                clrsChanged = false;
            }
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: effect);
            _spriteBatch.Draw(mapTexture, new Rectangle(dx, dy, mapTexture.Width * mult, mapTexture.Height * mult), Color.White);
            if (dx+ mapTexture.Width * mult<1200)
            {
                _spriteBatch.Draw(mapTexture, new Rectangle(dx+ mapTexture.Width * mult, dy, mapTexture.Width * mult, mapTexture.Height * mult), Color.White);
            }
            _spriteBatch.End();
            return;
            foreach (var item in tiles)
            {
                if (item.bounds.X < rectangle_Graphics.Width
                    && item.bounds.Y < rectangle_Graphics.Height
                    && item.bounds.X + item.bounds.Width > 0
                    && item.bounds.Y + item.bounds.Height > 0
                    )
                {
                    if (item.gFXElementOnMaps[0] is GFX_TileElement)
                    {
                        (item.gFXElementOnMaps[0] as GFX_TileElement).Draw(_spriteBatch, dx, dy, mult);
                    }
                }
                //_spriteBatch.Draw(TextureGenerator.GetFlat(), new Rectangle(item.bounds.X * mult + dx, item.bounds.Y * mult + dy, item.bounds.Width * mult, item.bounds.Height * mult), new Color(50, 50, 50, 0));
            }
            _spriteBatch.End();
        }
        #endregion


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
