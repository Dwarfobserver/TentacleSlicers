using System;
using TentacleSlicers.general;

namespace TentacleSlicers.collisions
{
    /// <summary>
    /// Une forme dont la zone de collision est un cercle.
    /// </summary>
    public class Circle : Form
    {
        private readonly double _rayon;

        /// <summary>
        /// Crée un cercle avec le rayon indiqué.
        /// </summary>
        /// <param name="rayon"> Le rayon du cercle </param>
        public Circle(double rayon)
        {
            _rayon = rayon;
        }

        /// <summary>
        /// Indique si le cerce est en collision avec le rectangle indiqué.
        /// </summary>
        /// <param name="hitbox"> Le rectangle testé </param>
        /// <param name="p1"> Le centre du cercle </param>
        /// <param name="p2"> Le centre du rectangle </param>
        /// <returns> Vrai si le cercle et le rectangle sont en collision </returns>
        public override bool Collides(Rectangle hitbox, Point p1, Point p2)
        {
            // Simple test
            if (p1.Length(p2) < _rayon) return true;

            // Prendre le point à la limite du cercle en direction du rectangle.
            // On teste si il est contenu par le rectangle.
            var x = p2.X - p1.X;
            var y = p2.Y - p1.Y;
            var a = Math.Atan2(y, x);
            var p = p1 + new Point(x * Math.Cos(a), y * Math.Sin(a));
            if (p.X < p1.X - hitbox.Width / 2 || p.X > p1.X + hitbox.Width / 2) return false;
            if (p.Y < p1.Y - hitbox.Height / 2 || p.Y > p1.Y + hitbox.Height / 2) return false;

            return true;
        }

        /// <summary>
        /// Retourne un rectangle incluant le cercle à la position indiquée.
        /// Le rectangle a pour largeur et hauteur le diamètre du cercle.
        /// </summary>
        /// <param name="position"> Le centre du cercle </param>
        /// <returns> Le rectangle incluant le cercle et son centre </returns>
        public override ShiftedRectangle IncludingRectangle(Point position)
        {
            return new ShiftedRectangle(position, new Rectangle(_rayon * 2, _rayon * 2));
        }
    }
}