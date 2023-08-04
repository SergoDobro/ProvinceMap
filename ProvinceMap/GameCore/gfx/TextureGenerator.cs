using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvinceMap.GameCore.gfx
{
    static class TextureGenerator
    {
        public static GraphicsDevice GraphicsDevice;
        public static ContentManager ContentManager;
        private static Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
        public static Texture2D GetCircle()
        {
            if (textures.ContainsKey("circle"))
                return textures["circle"];
            
            Texture2D texture2D;
            texture2D = new Texture2D(GraphicsDevice, 256, 256);
            Color[] colors = new Color[256 * 256];
            for (int i = 0; i < 256; i++)
            {
                for (int j = 0; j < 256; j++)
                {
                    int distSq = (128 - i) * (128 - i) + (128 - j) * (128 - j);
                    //int satur = 64 - (int)Math.Sqrt(distSq);
                    int satur = (int)(1.5 * (64 - (int)Math.Pow(Math.Abs(Math.Sqrt(distSq) - 64), 1.5)));
                    colors[i * 256 + j] = new Color(satur, satur, satur, satur);
                }
            }
            texture2D.SetData(colors);
            textures.Add("circle", texture2D);
            return texture2D;
        }
        public static Texture2D GetFlat()
        {
            if (textures.ContainsKey("flat"))
                return textures["flat"];
            Texture2D texture2D;
            texture2D = new Texture2D(GraphicsDevice, 1, 1);
            texture2D.SetData(new Color[1] { Color.White });
            textures.Add("flat", texture2D);
            return texture2D;
        }
        public static Texture2D GetTexture(string name)
        {
            if (textures.ContainsKey(name))
                return textures[name];
            if (!LoadTexture(name, name))
                return GetFlat();
            return textures[name];
        }
        public static bool LoadTexture(string name, string path)
        {
            try
            {
                if (textures.ContainsKey(name))
                    return false;
                textures.Add(name, ContentManager.Load<Texture2D>(path));
            }
            catch 
            {
                return false;
            }
            return true;
        }
    }
}
