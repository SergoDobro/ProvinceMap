using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvinceMap.GameCore.gfx
{
    class Line : IClonable, IDeletable
    {
        public static List<Line> linePositions = new List<Line>();
        private Vector2 position1, position2;
        public Vector2 P1 { get { return position1; } set { OnPointsPositionsChange(); position1 = value; } }
        public Vector2 P2 { get { return position2; } set { OnPointsPositionsChange(); position2 = value; } }
        public Color Color { get; set; }
        public Texture2D Texture { get; set; }
        public static Texture2D TextureGlobal { get; set; }
        public int Thickness { get; set; } = 1;
        public Map mapped; 
        public Line(Vector2 p1, Vector2 p2, Color color, int newThickness, Map map = null)
        {
            P1 = p1;
            P2 = p2;
            Color = color;
            Thickness = newThickness;
            linePositions.Add(this);
            this.mapped = map;
        }
        public void LoadContent(GraphicsDevice graphics)
        {
            Texture = new Texture2D(graphics, 1, 1);
            Texture.SetData(new[] { Color.White });
        }
        public void LoadContent(Texture2D texture)
        {
            Texture = texture;
        }
        public static void LoadContentGlobal(GraphicsDevice graphics)
        {
            TextureGlobal = new Texture2D(graphics, 1, 1);
            TextureGlobal.SetData(new[] { Color.White });
        }
        public static void LoadContentGlobal(Texture2D texture)
        {
            TextureGlobal = texture;
        }
        public static void DrawAll(SpriteBatch spriteBatch)
        {
            foreach (var l in linePositions)
            {
                l.Draw(spriteBatch);
            }
        }
        public static void DrawAllGlobal(SpriteBatch spriteBatch)
        {
            foreach (var l in linePositions)
            {
                l.DrawGlobal(spriteBatch);
            }
        }
        double length = 0;
        public void OnPointsPositionsChange()
        {
            length = Math.Sqrt((P2.X - P1.X) * (P2.X - P1.X) + (P2.Y - P1.Y) * (P2.Y - P1.Y));
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            OnPointsPositionsChange();
            if (mapped!=null)
            {
                //spriteBatch.Draw(TextureGlobal, new Rectangle((int)(P1.X * mapped.mult + mapped.Dx),
                //    (int)(P1.Y * mapped.mult + mapped.dy),
                //    (int)(length * mapped.mult),
                //    (int)(Thickness * mapped.mult)), null, Color,
                //(float)Math.Atan2((P2.Y - P1.Y) * mapped.mult, (P2.X - P1.X) * mapped.mult),
                //Vector2.Zero, SpriteEffects.None, 0f);
                spriteBatch.Draw(TextureGlobal, new Rectangle((int)(P1.X * Map.ObjectBufferSize.X  ),
                    (int)(P1.Y * Map.ObjectBufferSize.X),
                    (int)(length * Map.ObjectBufferSize.X),
                    (int)(Thickness )), null, Color,
                (float)Math.Atan2((P2.Y - P1.Y) * Map.ObjectBufferSize.X, (P2.X - P1.X) * Map.ObjectBufferSize.X),
                Vector2.Zero, SpriteEffects.None, 0f);
                //if (mapped.Dx + mapped.mapTexture.Width * mapped.mult < 1200)
                //{
                //    spriteBatch.Draw(TextureGlobal, new Rectangle((int)(P1.X * mapped.mult + mapped.Dx + mapped.mapTexture.Width * mapped.mult),
                //        (int)(P1.Y * mapped.mult + mapped.dy),
                //        (int)(length * mapped.mult),
                //        (int)(Thickness * mapped.mult)), null, Color,
                //    (float)Math.Atan2((P2.Y - P1.Y) * mapped.mult, (P2.X - P1.X) * mapped.mult),
                //    Vector2.Zero, SpriteEffects.None, 0f);
                //}
                //*Map.ObjectBufferSize.X
            }
            else
                spriteBatch.Draw(TextureGlobal, new Rectangle((int)P1.X, (int)P1.Y, (int)(length), Thickness), null, Color,
                (float)Math.Atan2(P2.Y - P1.Y, P2.X - P1.X),
                Vector2.Zero, SpriteEffects.None, 0f);
        }
        public void DrawGlobal(SpriteBatch spriteBatch)
        {
            OnPointsPositionsChange();
            if (mapped != null)
            {
                spriteBatch.Draw(TextureGlobal, new Rectangle((int)(P1.X + mapped.Dx), (int)(P1.Y + mapped.dy), (int)(length), Thickness), null, Color,
                (float)Math.Atan2(P2.Y - P1.Y, P2.X - P1.X),
                Vector2.Zero, SpriteEffects.None, 0f);
            }
            else
                spriteBatch.Draw(TextureGlobal, new Rectangle((int)P1.X, (int)P1.Y, (int)(length), Thickness), null, Color,
                (float)Math.Atan2(P2.Y - P1.Y, P2.X - P1.X),
                Vector2.Zero, SpriteEffects.None, 0f);
        }
        private float Vector_mult(float x1, float y1, float x2, float y2)
        {
            return x1 * y2 - x2 * y1;
        }
        public bool AreCrossing(Line l2)
        {
            Vector2 p1 = P1;
            Vector2 p2 = P2;
            Vector2 p3 = l2.P1;
            Vector2 p4 = l2.P2;
            float v1 = Vector_mult(p4.X - p3.X, p4.Y - p3.Y, p1.X - p3.X, p1.Y - p3.Y);
            float v2 = Vector_mult(p4.X - p3.X, p4.Y - p3.Y, p2.X - p3.X, p2.Y - p3.Y);
            float v3 = Vector_mult(p2.X - p1.X, p2.Y - p1.Y, p3.X - p1.X, p3.Y - p1.Y);
            float v4 = Vector_mult(p2.X - p1.X, p2.Y - p1.Y, p4.X - p1.X, p4.Y - p1.Y);
            return (v1 * v2 < 0 && v3 * v4 < 0);
        }
        /// <summary>
        /// поиск точки пересечения
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public Vector2 CrossingPoint(Line line)
        {
            var l1 = LineEquation(P1, P2);
            var l2 = LineEquation(line.P1, line.P2);

            Vector2 pt = new Vector2();
            double d = (double)(l1.a * l2.b - l1.b * l2.a);
            double dx = (double)(-l1.c * l2.b + l1.b * l2.c);
            double dy = (double)(-l1.a * l2.c + l1.c * l2.a);
            pt.X = (int)(dx / d);
            pt.Y = (int)(dy / d);
            return pt;
        }
        public (float a, float b, float c) LineEquation(Vector2 p1, Vector2 p2)
        {
            return (p2.Y - p1.Y, p1.X - p2.X, -p1.X * (p2.Y - p1.Y) + p1.Y * (p2.X - p1.X));
        }
        public bool IsInsideTheCircle(Vector2 center, float length)
        {
            Vector2 pos1fromCenter = new Vector2(P1.X - center.X, P1.Y - center.Y);
            float toCenter1 = (float)Math.Sqrt(pos1fromCenter.X * pos1fromCenter.X + pos1fromCenter.Y * pos1fromCenter.Y);
            Vector2 pos2fromCenter = new Vector2(P2.X - center.X, P2.Y - center.Y);
            float toCenter2 = (float)Math.Sqrt(pos2fromCenter.X * pos2fromCenter.X + pos2fromCenter.Y * pos2fromCenter.Y);
            if (toCenter1 <= length)
                return true;
            if (toCenter2 <= length)
                return true;
            return false;
        }
        public List<Vector2> IntersectingCircle(Vector2 center, float radius)
        {
            radius *= radius;
            Vector2 posotn1 = P1 - center;
            Vector2 posotn2 = P2 - center;
            float k = (posotn1.Y - posotn2.Y) / (posotn1.X - posotn2.X);
            float b = (posotn1.X * posotn2.Y - posotn2.X * posotn1.Y) / (posotn1.X - posotn2.X);
            if (b * b <= radius * k * k + radius)
            {
                float d = radius * k * k + radius - b * b;
                if (d == 0)
                {
                    float xPos = (-b * k) / (k * k + 1);
                    return new List<Vector2>() { new Vector2(xPos, k * xPos + b) + center };
                }
                else if (d > 0)
                {
                    float xPos1 = (-b * k - (float)Math.Sqrt(d)) / (k * k + 1);
                    float xPos2 = (-b * k + (float)Math.Sqrt(d)) / (k * k + 1);
                    return new List<Vector2>() { new Vector2(xPos1, k * xPos1 + b) + center, new Vector2(xPos2, k * xPos2 + b) + center };
                }
            }
            return new List<Vector2>();
        }
        public List<Vector2> OtresokIntersectingCircle(Vector2 center, float radius)
        {
            List<Vector2> vectors = IntersectingCircle(center, 100);
            for (int i = 0; i < vectors.Count; i++)
            {
                if (!((vectors[i].X > P1.X && vectors[i].X < P2.X) || (vectors[i].X > P2.X && vectors[i].X < P1.X)))
                {
                    vectors.RemoveAt(i);
                    i--;
                }
            }
            return vectors;
        }
        public IClonable Clone() => new Line(P1, P2, Color, Thickness) { Texture = this.Texture };
        public void Delete() => linePositions.Remove(this);
    }
    interface IClonable
    {
        IClonable Clone();
    }
    interface IDeletable
    {
        void Delete();
    }
}
