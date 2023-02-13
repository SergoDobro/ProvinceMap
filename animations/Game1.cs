using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace animations
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        AniamtionManager aniamtion = new AniamtionManager();
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            TextureGenerator.GraphicsDevice = (GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            aniamtion.Draw(_spriteBatch, gameTime);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
    class AniamtionManager
    {
        MouseState ms, pms;
        List<Animation> animations = new List<Animation>();
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            ms = Mouse.GetState();
            if (ms.LeftButton == ButtonState.Pressed && pms.LeftButton != ms.LeftButton)
            {
                animations.Add(new Animation(this));
            }
            for (int i = 0; i < animations.Count; i++)
            {
                animations[i].Draw(spriteBatch, gameTime);
            }
            pms = ms;
        }
        public void EndAnimation(Animation animation)
        {
            animations.Remove(animation);
            
        }
    }
    class Animation
    {
        public Texture2D texture;
        double dt = 0;
        AniamtionManager aniamtionManagerLink;
        Vector2 center;
        public Animation(AniamtionManager aniamtionManager)
        {
            dt = Math.PI / 4;
            aniamtionManagerLink = aniamtionManager;
            center = Mouse.GetState().Position.ToVector2();
        }
        Rectangle rectangle = new Rectangle(0, 0, 256, 256);
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {

            rectangle.X = (int)center.X - rectangle.Width / 2;
            rectangle.Y = (int)center.Y - rectangle.Height / 2;
            float dc = (float)Math.Sin(dt);
            float dc2 = (float)Math.Sin(dt - Math.PI / 4);
            rectangle.Width = (int)(dc2 * 128);
            rectangle.Height = (int)(dc2 * 128);
            if (texture == null)
            {
                texture = TextureGenerator.GetCircle();
            }
            spriteBatch.Draw(texture, rectangle, new Color(dc, dc, 0, 1)); 
            if (dt > Math.PI * 3 / 4)
            { 
                aniamtionManagerLink.EndAnimation(this);
                new Thread(()=>{ Thread.Sleep(100);
                    texture.Dispose();
                }).Start();
            }
            dt += gameTime.ElapsedGameTime.TotalSeconds * 2;
        }
    }
    static class TextureGenerator
    {
        public static GraphicsDevice GraphicsDevice;

        public static Texture2D GetCircle()
        {
            Texture2D texture2D;
            texture2D = new Texture2D(GraphicsDevice, 256, 256);
            Color[] colors = new Color[256 * 256];
            for (int i = 0; i < 256; i++)
            {
                for (int j = 0; j < 256; j++)
                {
                    int distSq = (128 - i) * (128 - i) + (128 - j) * (128 - j);
                    //int satur = 64 - (int)Math.Sqrt(distSq);
                    int satur = (int)(1.5*(64 - (int)Math.Pow(Math.Abs(Math.Sqrt(distSq) - 64), 1.5)));
                    colors[i * 256 + j] = new Color(satur, satur, satur, satur);
                }
            }
            texture2D.SetData(colors);
            return texture2D;
        }
    }
}