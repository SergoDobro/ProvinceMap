using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;

namespace ShaderTests
{
    struct TileData
    {
        public Vector3 colorA;
        public Vector3 colorB;
    }
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Effect effect;
        Effect effect2;
        Texture2D texture2D;

        const int ResolutionX = 1280;
        const int ResolutionY = 720;
        int maxTiles = 1000;

        StructuredBuffer particleBuffer; // stores all the particle information, will be updated by the compute shader
        VertexBuffer vertexBuffer; // used for drawing the particles

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = ResolutionX;
            _graphics.PreferredBackBufferHeight = ResolutionY;
            _graphics.ApplyChanges();


            base.Initialize();
        }
        List<TileData> tileDatas = new List<TileData>();
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            effect = Content.Load<Effect>("Effect");
            effect2 = Content.Load<Effect>("File");
            texture2D = Texture2D.FromFile(GraphicsDevice, "map_v2.bmp");
            //vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(TileData), maxTiles, BufferUsage.WriteOnly); // no need to initialize, all the data for drawing the particles is coming from the structured buffer

            string way_map_tiles_dec = "mapdec.csv";// PathWays.Path["way_map_tiles_dec"]; 
            using (StreamReader reader = new StreamReader(way_map_tiles_dec))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var clr = System.Drawing.Color.FromArgb(int.Parse(line.Split(';')[0]),
                        int.Parse(line.Split(';')[1]),
                        int.Parse(line.Split(';')[2]));
                    tileDatas.Add(new TileData() { colorA = new Vector3(clr.R / 255f, clr.G / 255f, clr.B / 255f), colorB = new Vector3(0,1,0) }) ;
                }
            }
            particleBuffer = new StructuredBuffer(GraphicsDevice, typeof(TileData), tileDatas.Count, BufferUsage.None, ShaderAccess.ReadWrite);

            particleBuffer.SetData(tileDatas.ToArray());
            effect.Parameters["DatasReadOnly"].SetValue(particleBuffer); 
            effect.Parameters["totalArr"].SetValue(tileDatas.Count);
        }

        Random random = new Random(); 
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit(); 
            effect.Parameters["DeltaTime"].SetValue((float)gameTime.TotalGameTime.TotalSeconds);
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                int k = random.Next(0, tileDatas.Count);
                tileDatas[k] = new TileData()
                {
                    colorA = tileDatas[k].colorA,
                    colorB = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble())
                };
                particleBuffer.SetData(tileDatas.ToArray());
                effect.Parameters["DatasReadOnly"].SetValue(particleBuffer);
            };
                //= new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
            
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            _spriteBatch.Begin(effect: effect);
            _spriteBatch.Draw(texture2D, new Rectangle(0, 0, ResolutionX, ResolutionY), Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}