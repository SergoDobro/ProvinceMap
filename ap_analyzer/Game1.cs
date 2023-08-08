using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonogameLibrary.UI.Elements;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace ap_analyzer
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        MonogameLibrary.UI.Compounds.BasicDrawableCompound basicDrawableCompound;
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

            MonogameLibrary.UI.Base.MonoClassManagerUI.InitManager(GraphicsDevice, Content, "File");

            basicDrawableCompound = new MonogameLibrary.UI.Compounds.BasicDrawableCompound() { rectangle = new Rectangle(0,0, _graphics.PreferredBackBufferWidth/5, _graphics.PreferredBackBufferHeight) };
            var k = new Button() { text = "open file", rectangle = new Rectangle(0,0,600,40)};
            basicDrawableCompound.Add("b1", k); 
            var k2 = new Label() { text = "path here", textAligment = MonogameLibrary.UI.Enums.TextAligment.Left };
            basicDrawableCompound.Add("l1", k2);
            var mainT = Thread.CurrentThread;
            k.LeftButtonPressed += () => {
                Thread thread = new Thread(() => {
                    using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
                    {
                        openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\..\\..\\..\\..";
                        //openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                        openFileDialog.FilterIndex = 2;
                        openFileDialog.RestoreDirectory = true;

                        if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            k2.text = openFileDialog.FileName;
                            Loadmap(Path.GetRelativePath(AppDomain.CurrentDomain.BaseDirectory, openFileDialog.FileName));
                        }
                    }
                });//System.Windows.Forms.Clipboard.SetText(file_track_title.ReadLine())
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join();

                if (texture == null)
                    texture = Texture2D.FromFile(GraphicsDevice, gp);
                loaded = true;
            };
            var k3 = new Button() { text = "start", rectangle = new Rectangle(0, 0, 600, 40) };
            basicDrawableCompound.Add("b2", k3);
            k3.LeftButtonPressed += () => {
                DetectColors();
            };
            k4 = new Label() { text = "total: 0", textAligment = MonogameLibrary.UI.Enums.TextAligment.Left };
            basicDrawableCompound.Add("l2", k4);
            basicDrawableCompound.LoadTexture();
        }
        Label k4;
        Bitmap bitmap;
        Texture2D texture;
        bool loaded = false;
        string gp;
        public void Loadmap(string path)
        {
            gp = path;
            bitmap = (Bitmap)Bitmap.FromFile(path); 
            Task thread = new Task(() => { 
            });
            //thread.SetApartmentState(ApartmentState.STA);
            //thread.Start();
            //thread.Join();
            thread.Start();
            thread.Wait();
        }
        public void DetectColors()
        {
            Color[] colors = new Color[bitmap.Width * bitmap.Height];
            texture.GetData(colors);
            List<Color> colors2 = new List<Color>();
            List<string> colorsList = new List<string>();
            int da = 0;
            for (int i = 0; i < colors.Length; i++)
            {
                if (!colors2.Contains(colors[i]))
                {
                    if (colors[i].A != 0)
                    {
                        colors2.Add(colors[i]);
                        k4.text = "total: " + colors2.Count;
                        colorsList.Add($"{colors[i].R};{colors[i].G};{colors[i].B};{colors2.Count-1-da};tile");
                    }
                    else
                    {
                    }
                }
            }
            File.WriteAllLines("provinces.txt", colorsList);
            Process.Start("notepad.exe", "provinces.txt");

        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            MonogameLibrary.UI.Base.MonoClassManagerUI.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightSlateGray);

            MonogameLibrary.UI.Base.MonoClassManagerUI.Draw(_spriteBatch);

            _spriteBatch.Begin();
            if (loaded)
            {
                _spriteBatch.Draw(texture, new Rectangle(basicDrawableCompound.rectangle.Width, 0, _graphics.PreferredBackBufferWidth - basicDrawableCompound.rectangle.Width,
                    _graphics.PreferredBackBufferHeight), Color.White);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}