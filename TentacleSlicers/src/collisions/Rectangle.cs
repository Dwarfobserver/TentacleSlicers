using System;
using TentacleSlicers.general;

namespace TentacleSlicers.collisions
{
    /// <summary>
    /// Une forme dont la zone de collision est rectangulaire.
    /// Il s'agit de la forme utilisé pour les collisions des acteurs, donc toutes les formes implémentent un test de
    /// collision avec les rectangles.
    /// </summary>
    public class Rectangle : Form
    {
        public double Width { get; }
        public double Height { get; }

        /// <summary>
        /// Crée un rectangle avec les dimensons données.
        /// </summary>
        /// <param name="witdh"> La largeur du rectangle </param>
        /// <param name="height"> La hauteur du rectangle </param>
        public Rectangle(double witdh, double height)
        {
            Width = witdh;
            Height = height;
        }

        /// <summary>
        /// Indique si les deux rectangles ssont en collision.
        /// </summary>
        /// <param name="hitbox"> Le rectangle indiqué </param>
        /// <param name="p1"> Le centre du rectangle courant </param>
        /// <param name="p2"> Le centre du rectangle indiqué </param>
        /// <returns> Vrai si les deux rectangles sont en collision </returns>
        public override bool Collides(Rectangle hitbox, Point p1, Point p2)
        {
            if (p1.X + Width/2 < p2.X - hitbox.Width/2 || p2.X + hitbox.Width/2 <  p1.X - Width/2) return false;
            if (p1.Y +Height/2 < p2.Y -hitbox.Height/2 || p2.Y +hitbox.Height/2 <  p1.Y -Height/2) return false;
            return true;
        }

        /// <summary>
        /// Retourne le rectangle courant (car il s'inclut lui-même) avec la position indiquée.
        /// </summary>
        /// <param name="position"> La position indiquée </param>
        /// <returns> Le rectangle courant avec la position indiquée </returns>
        public override ShiftedRectangle IncludingRectangle(Point position)
        {
            return new ShiftedRectangle(position, this);
        }

        /// <summary>
        /// Replace la position du rectangle courant en fonction du rectangle indiqué.
        /// </summary>
        /// <param name="hitbox"> Le rectangle indiqué </param>
        /// <param name="p1"> Le centre du rectangle courant </param>
        /// <param name="p2"> Le centre du rectangle indiqué </param>
        /// <returns></returns>
        public Point Replaces(Rectangle hitbox, Point p1, Point p2)
        {
            var p = new Point(p1.X, p1.Y);
            // x et y représentent la taille de "l'empiètement" entre les rectangles
            double x, y;
            if (p1.X < p2.X)
            {
                x = p2.X - hitbox.Width/2 - p1.X - Width/2;
            }
            else
            {
                x = p2.X + hitbox.Width/2 - p1.X + Width/2;
            }
            if (p1.Y < p2.Y)
            {
                y = p2.Y - hitbox.Height/2 - p1.Y - Height/2;
            }
            else
            {
                y = p2.Y + hitbox.Height/2 - p1.Y + Height/2;
            }
            // On replace là où "l'empiètement" est le plus petit
            if (Math.Abs(x) > Math.Abs(y))
            {
                p += new Point(0, y);
            }
            else
            {
                p += new Point(x, 0);
            }
            return p;
        }
    }
}
