using System;
using TentacleSlicers.general;

namespace TentacleSlicers.collisions
{
    /// <summary>
    /// Définit une hitbox rectangulaire.
    /// Cette classe permet de tester une collision de hitbox et de replacer un Actor.
    /// </summary>
    public class Hitbox
    {
        private const double Merge = 0.01;
        private readonly double _width;
        private readonly double _height;

        public Hitbox(double witdh, double height)
        {
            _width = witdh;
            _height = height;
        }

        public static bool Collides(Hitbox hb1, Point p1, Hitbox hb2, Point p2)
        {
            if (p1.X + hb1._width <  p2.X || p2.X + hb2._width <  p1.X) return false;
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (p1.Y + hb1._height <  p2.Y || p2.Y + hb2._height <  p1.Y) return false;
            return true;
        }

        public static void Replace(Hitbox hb1, Point p1, Hitbox hb2, Point p2)
        {
            var x1 = p1.X + hb1._width - p2.X;
            var x2 = p2.X + hb2._width - p1.X;
            var y1 = p1.Y + hb1._height - p2.Y;
            var y2 = p2.Y + hb2._height - p1.Y;

            var x = x1 < x2 ? -x1 : x2;
            var y = y1 < y2 ? -y1 : y2;
            if (Math.Abs(x) < Math.Abs(y))
            {
                p1.X += x;
            }
            else
            {
                p1.Y += y;
            }
        }
    }
}
