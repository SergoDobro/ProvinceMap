using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProvinceMap.GameCore.gfx
{
    class AnimationManager
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
        AnimationManager aniamtionManagerLink;
        Vector2 center;
        public Animation(AnimationManager aniamtionManager)
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
            rectangle.Width = (int)(dc2 * 64);
            rectangle.Height = (int)(dc2 * 48);
            if (texture == null)
            {
                texture = TextureGenerator.GetCircle();
            }
            spriteBatch.Draw(texture, rectangle, new Color(dc, dc, 0, 1));
            if (dt >= Math.PI * 3 / 4)
            {
                aniamtionManagerLink.EndAnimation(this);
            }
            dt += gameTime.ElapsedGameTime.TotalSeconds * 3;
        }
    }
}
