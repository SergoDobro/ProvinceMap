using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace ShaderTests_SimpleParticles
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        RenderTarget renderTarget;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            renderTarget = new RenderTarget();
            _graphics.PreferredBackBufferWidth = RenderTarget.ResolutionX;
            _graphics.PreferredBackBufferHeight = RenderTarget.ResolutionY;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            renderTarget.Load(GraphicsDevice, Content);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            renderTarget.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkBlue);

            renderTarget.Draw(GraphicsDevice, _spriteBatch);

            base.Draw(gameTime);
        }
    } 
    public class RenderTarget
    {


        public const int ResolutionX = 1280;
        public const int ResolutionY = 720;
        int maxTiles = 1000;

        //StructuredBuffer particleBuffer; // stores all the particle information, will be updated by the compute shader
        //VertexBuffer vertexBuffer; // used for drawing the particles
        //Particle[] Particles;
        public void Load(GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            
            effect = contentManager.Load<Effect>("File");
            texture2D = Texture2D.FromFile(graphicsDevice, "map_v2.bmp");
            //vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(TileData), maxTiles, BufferUsage.WriteOnly); // no need to initialize, all the data for drawing the particles is coming from the structured buffer
           // shader_map = new RenderTarget2D(graphicsDevice, 800, 600);
            shader_map = Texture2D.FromFile(graphicsDevice, "map_v2_shaderMap.png");
            renderTarget = new RenderTarget2D(graphicsDevice, 800, 600);
        }
        Random random = new Random();
        public void Update(GameTime gameTime)
        {

            ////effect.Parameters["DeltaTime"].SetValue((float)gameTime.TotalGameTime.TotalSeconds);
            //if (Keyboard.GetState().IsKeyDown(Keys.Space))
            //{
            //    int k = random.Next(0, Particles.Count);
            //    Particles[k] = new Particle()
            //    {
            //        colorA = tileDatas[k].colorA,
            //        colorB = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble())
            //    };
            //    particleBuffer.SetData(tileDatas.ToArray());
            //    effect.Parameters["DatasReadOnly"].SetValue(particleBuffer);
            //}; 
        }

        Effect effect;
        Texture2D texture2D;
        Texture2D shader_map;
        RenderTarget2D renderTarget;
        Matrix matrix = new Matrix();
        public void Draw(GraphicsDevice graphicsDevice, SpriteBatch _spriteBatch)
        {
           // effect.Parameters["displacementMap"].SetValue(shader_map); // задаем карту шейдеру 
            //effect.Parameters["matrixx"].SetValue(matrix); // задаем карту шейдеру 
            //graphicsDevice.SetRenderTarget(renderTarget);
            _spriteBatch.Begin( effect: effect);
            _spriteBatch.Draw(texture2D, new Rectangle(0, 0, 1000, 1000), Color.White);
            _spriteBatch.End();
            //graphicsDevice.SetRenderTarget(null);

            //_spriteBatch.Begin(); 
            //_spriteBatch.Draw(renderTarget, new Rectangle(10, 10, 800, 800), Color.White);
            //_spriteBatch.End();

        }
    }
}